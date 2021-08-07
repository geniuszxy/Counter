using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Counter
{
	public abstract class Counter
	{
		/// <summary>
		/// Byte length
		/// </summary>
		public abstract int Size { get; }

		/// <summary>
		/// Put next {count} numbers into the buffer and advance the counter
		/// </summary>
		public abstract bool GetNextNumbers(byte[] output, int count, int offset);

		/// <summary>
		/// Reset the next number
		/// </summary>
		public abstract void SetNextNumber(byte[] input, int offset);

		/// <summary>
		/// Peek the next number
		/// </summary>
		public abstract void PeekNextNumber(byte[] output, int offset);

		/// <summary>
		/// Create a counter
		/// </summary>
		public static Counter CreateCounter(int size)
		{
			switch (size)
			{
				case 4: return new Counter4();
				case 8: return new Counter8();
				case 16: return new Counter16();
				default: return null;
			}
		}

		/// <summary>
		/// Create a counter and set its initial value
		/// </summary>
		public static Counter CreateCounter(int size, byte[] initalValue, int offset)
		{
			var counter = CreateCounter(size);
			if (counter != null)
				counter.SetNextNumber(initalValue, offset);
			return counter;
		}
	}
}
