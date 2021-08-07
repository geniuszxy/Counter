using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Counter
{
	sealed class Counter4 : Counter
	{
		private uint _v;

		public override int Size
		{
			get { return 4; }
		}

		unsafe public override bool GetNextNumbers(byte[] output, int count, int offset)
		{
			//Interlocked.Increment(ref)

			fixed (uint* pi = &_v)
			{
				var pb = (byte*)pi;
				output[offset] = *pb;
				output[offset + 1] = *(pb + 1);
				output[offset + 2] = *(pb + 2);
				output[offset + 3] = *(pb + 3);
			}
		}

		unsafe public override void PeekNextNumber(byte[] output, int offset)
		{
			fixed(uint* pi = &_v)
			{
				var pb = (byte*)pi;
				output[offset] = *pb;
				output[offset + 1] = *(pb + 1);
				output[offset + 2] = *(pb + 2);
				output[offset + 3] = *(pb + 3);
			}
		}

		unsafe public override void SetNextNumber(byte[] input, int offset)
		{
			throw new NotImplementedException();
		}
	}
}
