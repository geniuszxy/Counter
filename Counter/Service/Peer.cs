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

		public static void Create(TcpClient tcpClient)
		{
			var stream = tcpClient.GetStream();

			var peer = new Peer
			{
				_client = tcpClient,
				_stream = stream,
				_reader = new BinaryReader(new BufferedStream(stream, 128)),
			};

			new Thread(peer.Job) { IsBackground = true }.Start();
		}

		private void Job()
		{
			while (CounterService.Instance != null)
			{
				var method = _reader.ReadByte();
				var command = _commandHandlers[method];
				//TODO check auth and permission
				command(this);
			}
		}

		#region Command Handler

		delegate void CommandHandler(Peer peer);
		static CommandHandler[] _commandHandlers;

		public static void InitCommandDelegates()
		{
			_commandHandlers = new CommandHandler[]
			{
				CreateCommandHandler("LoginWithPassword"), //0
				CreateCommandHandler("QueryNextNumber"), //1
				CreateCommandHandler("QueryNumbers"), //2
				CreateCommandHandler("QueryCounterStatus"), //3
				CreateCommandHandler("AddCounter"), //4
				CreateCommandHandler("DeleteCounter"), //5
				CreateCommandHandler("SetCounter"), //6
			};
		}

		private static CommandHandler CreateCommandHandler(string methodName)
		{
			var methodInfo = typeof(Peer).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (CommandHandler)Delegate.CreateDelegate(typeof(CommandHandler), methodInfo);
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
			WrongCounterSize = 5,
			PermissionDenied = 6,
			WrongInput = 7,
		}

		#endregion Error Code

		#region Commands

		private void LoginWithPassword()
		{
			int passwordLength = _reader.ReadUInt16();
			var bytes = _reader.ReadBytes(passwordLength);
			var password = Encoding.UTF8.GetString(bytes, 0, passwordLength);
			SendError(ErrorCode.Success);
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
				SendError(ErrorCode.WrongInput);
			else
				QueryNumbers(counterId, numberCount);
		}

		private void QueryCounterStatus()
		{
			int counterId = _reader.ReadUInt16();
			var counter = CounterService.Manager.GetCounter(counterId);
			if (counter == null)
			{
				SendError(ErrorCode.WrongCounterID);
				return;
			}

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
			int counterSize = _reader.ReadByte();
			var newCounter = Counter.CreateCounter(counterSize);
			if(newCounter == null)
			{
				SendError(ErrorCode.WrongCounterSize);
				return;
			}
		}

		private void DeleteCounter()
		{
			int counterId = _reader.ReadUInt16();
		}

		private void SetCounter()
		{
			int counterId = _reader.ReadUInt16();
			int counterSize = _reader.ReadByte();
		}

		#endregion Commands

		#region Command Helpers

		private void SendError(ErrorCode e)
		{
			_stream.WriteByte((byte)e);
		}

		private void QueryNumbers(int counterId, int numberCount)
		{
			var counter = CounterService.Manager.GetCounter(counterId);
			if (counter == null)
			{
				SendError(ErrorCode.WrongCounterID);
				return;
			}

			var output = new byte[1 + counter.Size];
			if (!counter.GetNextNumbers(output, numberCount, 1))
			{
				SendError(ErrorCode.CounterOverflow);
				return;
			}

			output[0] = (byte)ErrorCode.Success;
			_stream.Write(output);
		}

		#endregion Command Helpers
	}
}
