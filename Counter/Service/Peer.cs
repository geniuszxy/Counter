using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Counter
{
	class Peer
	{
		private TcpClient _client;
		private NetworkStream _stream;
		private BinaryReader _reader;
		private Permission _permission;

		public static void Create(TcpClient tcpClient, Permission initialPermission)
		{
			var stream = tcpClient.GetStream();

			var peer = new Peer
			{
				_client = tcpClient,
				_stream = stream,
				_reader = new BinaryReader(new BufferedStream(stream, 128)),
				_permission = initialPermission,
			};

			new Thread(peer.Job) { IsBackground = true }.Start();
		}

		private void Job()
		{
			try
			{
				while (CounterService.Instance != null)
				{
					var method = _reader.ReadByte();
					if (method >= _commands.Length)
						throw new Error(ErrorCode.InvalidInput);
					_commands[method].Run(this);
				}
			}
			catch (Error e)
			{
				//Send the error
				_stream.WriteByte(e.code);
			}
			catch
			{

			}
			finally
			{
				//Release objects
				_reader = null;
				_stream = null;
				_permission = Permission.None;
				_client.Close();
				_client = null;
			}
		}

		#region Group and Permission

		[Flags]
		public enum Permission
		{
			None = 0,
			Full = -1,
			
			Query = 0b_0001,
			AddCounter = 0b_0010,
			DeleteCounter = 0b_0100,
			SetCounter = 0b_1000,
		}

		public enum Group
		{
			Unauthenticated,
			User,
			Administrator,
		}

		#endregion Group and Permission

		#region Command Handler

		struct Command
		{
			public delegate void Handler(Peer peer);

			public readonly Handler handler;
			public readonly Permission permission;

			public Command(string methodName, Permission requiredPermission = Permission.Query)
			{
				var methodInfo = typeof(Peer).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
				handler = (Handler)Delegate.CreateDelegate(typeof(Handler), methodInfo);
				permission = requiredPermission;
			}

			public void Run(Peer peer)
			{
				if((peer._permission & permission) == 0)
					throw new Error(ErrorCode.PermissionDenied);

				handler(peer);
			}
		}

		static Command[] _commands;

		public static void InitCommands()
		{
			_commands = new Command[]
			{
				/* 0 */ new Command(nameof(LoginWithPassword), Permission.None),
				/* 1 */ new Command(nameof(QueryNextNumber)),
				/* 2 */ new Command(nameof(QueryNumbers)),
				/* 3 */ new Command(nameof(QueryCounterStatus)),
				/* 4 */ new Command(nameof(AddCounter), Permission.AddCounter),
				/* 5 */ new Command(nameof(DeleteCounter), Permission.DeleteCounter),
				/* 6 */ new Command(nameof(SetCounter), Permission.SetCounter),
			};
		}

		#endregion

		#region Error Code

		public enum ErrorCode : byte
		{
			Success = 0,
			WrongCounterID = 1,
			CounterOverflow = 2,
			AuthenticationFailed = 3,
			ExistCounterID = 4,
			InvalidCounterSize = 5,
			PermissionDenied = 6,
			InvalidInput = 7,
		}

		#endregion Error Code

		#region Commands

		private void LoginWithPassword()
		{
			int passwordLength = _reader.ReadUInt16();
			var bytes = _reader.ReadBytes(passwordLength);
			var password = Encoding.UTF8.GetString(bytes, 0, passwordLength);
			SendSuccess();
		}

		private void QueryNextNumber()
		{
			int counterId = _reader.ReadUInt16();
			QueryNumbers(counterId, 1);
		}

		private void QueryNumbers()
		{
			int counterId = _reader.ReadUInt16();
			int numberCount = _reader.ReadByte();
			if (numberCount < 1)
				throw new Error(ErrorCode.InvalidInput);

			QueryNumbers(counterId, numberCount);
		}

		private void QueryCounterStatus()
		{
			int counterId = _reader.ReadUInt16();
			var counter = CounterService.Manager.GetCounter(counterId);
			if (counter == null)
				throw new Error(ErrorCode.WrongCounterID);

			int size = counter.Size;
			var output = new byte[2 + size];
			output[0] = (byte)ErrorCode.Success;
			output[1] = (byte)size;
			counter.PeekNextNumber(output, 2);
			_stream.Write(output);
		}

		private void AddCounter()
		{
			int counterId = _reader.ReadUInt16();
			var counter = CounterService.Manager.GetCounter(counterId);
			if(counter != null)
				throw new Error(ErrorCode.ExistCounterID);

			int counterSize = _reader.ReadByte();
			var newCounter = Counter.CreateCounter(counterSize);
			if (newCounter == null)
				throw new Error(ErrorCode.InvalidCounterSize);

			if (!CounterService.Manager.AddCounter(counterId, newCounter))
				throw new Error(ErrorCode.ExistCounterID);

			SendSuccess();
		}

		private void DeleteCounter()
		{
			int counterId = _reader.ReadUInt16();
			if(!CounterService.Manager.DeleteCounter(counterId))
				throw new Error(ErrorCode.WrongCounterID);

			SendSuccess();
		}

		private void SetCounter()
		{
			int counterId = _reader.ReadUInt16();
			var counter = CounterService.Manager.GetCounter(counterId);
			if (counter == null)
				throw new Error(ErrorCode.WrongCounterID);

			int counterSize = _reader.ReadByte();


			var data = _reader.ReadBytes(counterSize);

			CounterService.Manager.SetCounter(counterId, counterSize, data, 0);
		}

		#endregion Commands

		#region Command Helpers

		class Error : Exception
		{
			public readonly byte code;

			public Error(ErrorCode e)
			{
				code = (byte)e;
			}
		}

		private void SendSuccess()
		{
			_stream.WriteByte((byte)ErrorCode.Success);
		}

		private void QueryNumbers(int counterId, int numberCount)
		{
			var counter = CounterService.Manager.GetCounter(counterId);
			if (counter == null)
				throw new Error(ErrorCode.WrongCounterID);

			var output = new byte[1 + counter.Size];
			if (!counter.GetNextNumbers(output, numberCount, 1))
				throw new Error(ErrorCode.CounterOverflow);

			output[0] = (byte)ErrorCode.Success;
			_stream.Write(output);
		}

		#endregion Command Helpers
	}
}
