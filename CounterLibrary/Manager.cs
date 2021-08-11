using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Counter
{
	public class Manager
	{
		const string CONFIG_FILE = "counters";
		private delegate void Command(byte[] data, ref int offset);
		Command[] _commands;
		Counter[] _counters;

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

			LoadCounters();
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

		/// <summary>
		/// Load and initial counters from the config file
		/// </summary>
		private void LoadCounters()
		{
			if(!File.Exists(CONFIG_FILE))
			{
				_counters = new Counter[4];
				return;
			}

			var lines = File.ReadAllLines(CONFIG_FILE);
			int maxCounterId = 0;
			var counters = new Dictionary<int, Counter>(lines.Length);
			foreach (var line in lines)
			{
				if(Counter.TryUnserialize(line, out int counterId, out Counter counter))
				{
					counters.Add(counterId, counter);
					if (counterId > maxCounterId)
						maxCounterId = counterId;
				}
				//TODO handle error
			}

			if(maxCounterId > 3)
				_counters = new Counter[maxCounterId + 1];
			foreach (var entry in counters)
				_counters[entry.Key] = entry.Value;
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
			int counterId = BitConverter.ToUInt16(data, offset);

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
