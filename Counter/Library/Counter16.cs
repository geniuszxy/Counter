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
		private object _lock = new object();
		private ulong _vl;
		private ulong _vh;

		public override int Size
		{
			get { return 16; }
		}

		unsafe public override bool GetNextNumbers(byte[] output, int count, int offset)
		{
			ulong vl, vh;

			lock (_lock)
			{
				unchecked
				{
					vl = _vl;
					vh = _vh;

					if(vl > (ulong.MaxValue - (ulong)count)) //overflow
					{
						if (vh == ulong.MaxValue)
							return false;
						else
							_vh = vh + 1; //high + 1
					}
					_vl = vl + (ulong)count;
				}
			}

			var pb = (byte*)&vl;
			for (int i = 0; i < 8; i++)
				output[offset + i] = *(pb + i);

			pb = (byte*)&vh;
			for (int i = 8; i < 16; i++)
				output[offset + i] = *(pb + i);

			return true;
		}

		unsafe public override void PeekNextNumber(byte[] output, int offset)
		{
			ulong vl, vh;

			lock (_lock)
			{
				vl = _vl;
				vh = _vh;
			}

			var pb = (byte*)&vl;
			for (int i = 0; i < 8; i++)
				output[offset + i] = *(pb + i);

			pb = (byte*)&vh;
			for (int i = 8; i < 16; i++)
				output[offset + i] = *(pb + i);
		}

		public override void SetNextNumber(byte[] input, int offset)
		{
			lock (_lock)
			{
				_vl = BitConverter.ToUInt64(input, offset);
				_vh = BitConverter.ToUInt64(input, offset + 8);
			}
		}
	}
}
