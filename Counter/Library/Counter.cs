using System;
using System.Globalization;
using System.Text;

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
		/// Serialize the counter to string
		/// </summary>
		public virtual void Serialize(int counterId, StringBuilder output)
		{
			var buffer = new byte[Size];
			PeekNextNumber(buffer, 0);
			output.Append(counterId).Append(' ')
				.Append(Size).Append(' ')
				.Append(buffer[0]);
			for (int i = 1; i < buffer.Length; i++)
				output.Append(' ').Append(buffer[i]);
			output.AppendLine();
		}

		/// <summary>
		/// Try deserialize a counter from string
		/// </summary>
		public static bool TryUnserialize(string line, out int counterId, out Counter counter)
		{
			counterId = 0;
			counter = null;

			var parts = line.Split(' ');
			if (parts.Length < 6)
				return false;

			//Counter ID

			if (!int.TryParse(parts[0], out counterId))
				return false;

			if (counterId < 0 || counterId >= 65536)
				return false;

			//Size

			if (!int.TryParse(parts[1], out int size))
				return false;

			counter = CreateCounter(size);
			if (counter == null)
				return false;

			//Value

			if (parts.Length - 2 != size)
				return false;

			var buffer = new byte[size];
			for (int i = 0; i < size; i++)
				if (!byte.TryParse(parts[i + 2], out buffer[i]))
					return false;

			counter.SetNextNumber(buffer, 0);

			return true;
		}

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
