using System;
using System.Runtime.InteropServices;
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
				v0 = Volatile.Read(ref _v);
				if (unchecked((ulong)v0 > max))
					return false;

				v1 = v0 + count;
			}
			while (v0 != Interlocked.CompareExchange(ref _v, v1, v0));

			Marshal.Copy(new IntPtr(&v0), output, offset, 8);
			return true;
		}

		unsafe public override void PeekNextNumber(byte[] output, int offset)
		{
			fixed (long* p = &_v)
			{
				Marshal.Copy(new IntPtr(p), output, offset, 8);
			}
		}

		public override void SetNextNumber(byte[] input, int offset)
		{
			_v = unchecked((long)BitConverter.ToUInt64(input, offset));
		}
	}
}
