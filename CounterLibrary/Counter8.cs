using System;
using System.Threading;

namespace Counter
{
	sealed class Counter8 : Counter
	{
		private long _v;

		public override int Size
		{
			get { return 8; }
		}

		unsafe public override bool GetNextNumbers(byte[] output, int count, int offset)
		{
			long v0, v1;
			ulong max = ulong.MaxValue - (ulong)count;

			do
			{
				v0 = _v;
				if (unchecked((ulong)v0 > max))
					return false;

				v1 = v0 + count;
			}
			while (v0 != Interlocked.CompareExchange(ref _v, v1, v0));

			var pb = (byte*)&v0;
			output[offset] = *pb;
			output[offset + 1] = *(pb + 1);
			output[offset + 2] = *(pb + 2);
			output[offset + 3] = *(pb + 3);
			output[offset + 4] = *(pb + 4);
			output[offset + 5] = *(pb + 5);
			output[offset + 6] = *(pb + 6);
			output[offset + 7] = *(pb + 7);
			return true;
		}

		unsafe public override void PeekNextNumber(byte[] output, int offset)
		{
			fixed (long* pi = &_v)
			{
				var pb = (byte*)pi;
				output[offset] = *pb;
				output[offset + 1] = *(pb + 1);
				output[offset + 2] = *(pb + 2);
				output[offset + 3] = *(pb + 3);
				output[offset + 4] = *(pb + 4);
				output[offset + 5] = *(pb + 5);
				output[offset + 6] = *(pb + 6);
				output[offset + 7] = *(pb + 7);
			}
		}

		public override void SetNextNumber(byte[] input, int offset)
		{
			_v = unchecked((long)BitConverter.ToUInt64(input, offset));
		}
	}
}
