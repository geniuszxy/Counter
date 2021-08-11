using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
	}
}
