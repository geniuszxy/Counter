using System;
using System.Collections.Generic;
using System.Text;

namespace Counter
{
	public class Manager
	{
		private delegate void Command(byte[] data, ref int offset);
		Command[] _commands;

		public Manager()
		{
			_commands = new Command[]
			{
				LoginWithPassword, //0
				QueryNextNumber, //1
				QueryNumbers, //2
				QueryCounterStatus, //3
				AddCounter, //4
				DeleteCounter, //5
				SetCounter, //6
			};
		}

		public void RunCommand(byte[] data, ref int offset)
		{
			while (offset < data.Length)
			{
				int method = data[offset++];
				var command = _commands[method];
				command(data, ref offset);
			}
		}

		#region Socket Commands

		private void LoginWithPassword(byte[] data, ref int offset)
		{
			int passwordLength = BitConverter.ToUInt16(data, offset);
			offset += 2;
			var password = Encoding.UTF8.GetString(data, offset, passwordLength);
			offset += passwordLength;
		}

		private void QueryNextNumber(byte[] data, ref int offset)
		{

		}

		private void QueryNumbers(byte[] data, ref int offset)
		{

		}

		private void QueryCounterStatus(byte[] data, ref int offset)
		{
		}

		private void AddCounter(byte[] data, ref int offset)
		{

		}

		private void DeleteCounter(byte[] data, ref int offset)
		{

		}

		private void SetCounter(byte[] data, ref int offset)
		{

		}

		#endregion Socket Commands

		#region Internal Commands

		private void LoginWithPassword(string password)
		{

		}

		private void QueryNextNumber(ushort counterId)
		{

		}

		private void QueryNumbers(ushort counterId, byte numberCount)
		{

		}

		private void QueryCounterStatus(ushort counterId)
		{
		}

		private void AddCounter(ushort counterId, byte counterSize)
		{

		}

		private void DeleteCounter(ushort counterId)
		{

		}

		private void SetCounter(ushort counterId, byte counterSize, byte[] number)
		{

		}

		#endregion Internal Commands
	}
}
