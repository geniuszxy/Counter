using System;
using System.Threading;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			byte[] bts = new byte[4];
			if(GetNextNumbers(bts, 1, 0))
				Console.WriteLine(BitConverter.ToUInt32(bts));
			_v = unchecked((int)(uint.MaxValue - 1));
			if(GetNextNumbers(bts, 1, 0))
				Console.WriteLine(BitConverter.ToUInt32(bts));
			if (GetNextNumbers(bts, 1, 0))
				Console.WriteLine(BitConverter.ToUInt32(bts));
		}

		static int _v;

		unsafe static public bool GetNextNumbers(byte[] output, int count, int offset)
		{
			int v0, v1;
			uint max = uint.MaxValue - (uint)count;

			do
			{
				v0 = _v;
				if ((uint)v0 > max)
					return false;

				v1 = v0 + count;
			}
			while (v0 != Interlocked.CompareExchange(ref _v, v1, v0));

			var pb = (byte*)&v0;
			output[offset] = *pb;
			output[offset + 1] = *(pb + 1);
			output[offset + 2] = *(pb + 2);
			output[offset + 3] = *(pb + 3);
			return true;
		}
	}
}
