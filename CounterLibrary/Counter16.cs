using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Counter
{
	sealed class Counter16 : Counter
	{
		private ulong _vl;
		private ulong _vh;

		public override int Size
		{
			get { return 16; }
		}

		public override bool GetNextNumbers(byte[] output, int count, int offset)
		{
			throw new NotImplementedException();
		}

		public override void PeekNextNumber(byte[] output, int offset)
		{
			throw new NotImplementedException();
		}

		public override void SetNextNumber(byte[] input, int offset)
		{
			throw new NotImplementedException();
		}
	}
}
