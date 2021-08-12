using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Counter
{
	class Peer
	{
		private TcpClient _client;
		private AsyncCallback _onRead;
		private AsyncCallback _onWrite;
		private byte[] _buffer;

		public Peer(TcpClient client)
		{
			_client = client;
			_onRead = OnRead;
			_onWrite = OnWrite;
			_buffer = new byte[128];
		}

		public void Start()
		{
			var stream = _client.GetStream();
			stream.BeginRead(_buffer, 0, 128, _onRead, null);
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		private void OnRead(IAsyncResult asyncResult)
		{
			var stream = _client.GetStream();
			var readBytes = stream.EndRead(asyncResult);

			int offset = 0;
			while (offset < readBytes)
			{
				int method = _buffer[offset++];
				var command = _commandHandlers[method];
				command(this);
			}
		}

		private void OnWrite(IAsyncResult asyncResult)
		{

		}

		#region Command Handler

		enum CommandResult
		{
			OK,
		}

		delegate CommandResult CommandHandler(Peer peer);
		static CommandHandler[] _commandHandlers;

		public static void InitCommandDelegates()
		{
			var type = typeof(CommandHandler);
			var target = typeof(Peer);

			_commandHandlers = new CommandHandler[]
			{
				CreateCommandHandler(type, target, "LoginWithPassword"), //0
				CreateCommandHandler(type, target, "QueryNextNumber"), //1
				CreateCommandHandler(type, target, "QueryNumbers"), //2
				CreateCommandHandler(type, target, "QueryCounterStatus"), //3
				CreateCommandHandler(type, target, "AddCounter"), //4
				CreateCommandHandler(type, target, "DeleteCounter"), //5
				CreateCommandHandler(type, target, "SetCounter"), //6
			};
		}

		private static CommandHandler CreateCommandHandler(Type type, Type target, string methodName)
		{
			var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			return (CommandHandler)Delegate.CreateDelegate(type, methodInfo);
		}

		#endregion

		#region Commands

		private CommandResult LoginWithPassword()
		{
			//int passwordLength = BitConverter.ToUInt16(data, offset);
			//offset += 2;
			//var password = Encoding.UTF8.GetString(data, offset, passwordLength);
			//offset += passwordLength;
			return CommandResult.OK;
		}

		private CommandResult QueryNextNumber()
		{
			//int counterId = BitConverter.ToUInt16(data, offset);
			return CommandResult.OK;
		}

		private CommandResult QueryNumbers()
		{
			return CommandResult.OK;
		}

		private CommandResult QueryCounterStatus()
		{
			return CommandResult.OK;
		}

		private CommandResult AddCounter()
		{
			return CommandResult.OK;
		}

		private CommandResult DeleteCounter()
		{
			return CommandResult.OK;
		}

		private CommandResult SetCounter()
		{
			return CommandResult.OK;
		}

		#endregion Commands
	}
}
