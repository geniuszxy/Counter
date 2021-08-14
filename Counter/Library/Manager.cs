using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Counter
{
	public class Manager
	{
		const string CONFIG_FILE = "counters";
		Counter[] _counters;

		public Manager()
		{
			LoadCounters();
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

		public Counter GetCounter(int counterId)
		{
			return _counters[counterId];
		}

		public bool AddCounter(int counterId, Counter newCounter)
		{
			lock (_counters)
			{
				if (counterId < 0)
					return false;
				else if (counterId >= _counters.Length)
				{
					var newCounters = new Counter[counterId + 1];
					Array.Copy(_counters, newCounters, _counters.Length);
					newCounters[counterId] = newCounter;
					//Set _counters at last
					_counters = newCounters;
				}
				//Check if there is a counter with the same id added before you call AddCounter
				else if (_counters[counterId] != null)
					return false;
				else
					_counters[counterId] = newCounter;
			}

			return true;
		}

		public bool DeleteCounter(int counterId)
		{
			lock(_counters)
			{
				if (counterId < 0 || counterId >= _counters.Length || _counters[counterId] == null)
					return false;

				_counters[counterId] = null;
				return true;
			}
		}

		public bool SetCounter(int counterId, int size, byte[] initialValue, int offset)
		{

		}
	}
}
