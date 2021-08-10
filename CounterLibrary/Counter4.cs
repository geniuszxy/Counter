using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Counter
{
	sealed class Counter4 : Counter
	{
		private int _v;

		public override int Size
		{
			get { return 4; }
		}

		unsafe public override bool GetNextNumbers(byte[] output, int count, int offset)
		{
			int v0, v1;
			uint max = uint.MaxValue - (uint)count;

			do
			{
				v0 = _v;
				if (unchecked((uint)v0 > max))
					return false;

				v1 = v0 + count;
			}
			while (v0 != Interlocked.CompareExchange(ref _v, v1, v0));

			Marshal.Copy(new IntPtr(&v0), output, offset, 4);
			return true;
		}

		unsafe public override void PeekNextNumber(byte[] output, int offset)
		{
			fixed(int* p = &_v)
			{
				Marshal.Copy(new IntPtr(p), output, offset, 4);
			}
		}

		public override void SetNextNumber(byte[] input, int offset)
		{
			_v = unchecked((int)BitConverter.ToUInt32(input, offset));
		}
	}
}
