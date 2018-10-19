using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Counter
{
	class BigInteger
	{
		private ulong _base;
		private ulong[] _uppers;

		public BigInteger(byte[] data)
		{
			_base = _Bytes2ULongLE(data, 0);

			if (data.Length <= 8)
				_uppers = new ulong[0];
			else
			{

			}
		}

		/// <summary>
		/// Increase integer by 1 and return bytes
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public byte[] Increase()
		{
			if (_base == ulong.MaxValue)
			{
				_base = 0UL;
				_IncreateUppers();
			}
			else
				_base++;

			return ToByteArray();
		}

		private void _IncreateUppers()
		{
			var uppers = _uppers;
			int uppersCount = uppers.Length;
			for (int i = 0; i < uppersCount; i++)
			{
				if (uppers[i] < ulong.MaxValue)
				{
					uppers[i]++;
					return; //End
				}

				uppers[i] = 0UL;
			}

			//Add more upper, initialize with 1
			Array.Resize(ref _uppers, uppersCount + 1);
			_uppers[uppersCount] = 1UL;
		}

		// Calculate minimal bytes for ulong
		private static int _CalculateBytes(ulong data)
		{
			if (data <= ushort.MaxValue)
				return data <= byte.MaxValue ? 1 : 2;
			else if (data <= uint.MaxValue)
				return data <= 16777215UL ? 3 : 4;
			else if (data <= 1099511627775UL)
				return 5;
			else if (data <= 281474976710655UL)
				return 6;
			else if (data <= 72057594037927935UL)
				return 7;
			else
				return 8;
		}

		// Convert ulong to bytes, little endian
		private static void _ULong2BytesLE(ulong data, byte[] buf, int offset)
		{
			do
			{
				buf[offset++] = (byte)data;
				data >>= 8;
			}
			while (data > 0);
		}

		// Convert bytes to ulong, little endian
		private static ulong _Bytes2ULongLE(byte[] buf, int offset)
		{

		}

		/// <summary>
		/// Convert integer to bytes, little endian
		/// </summary>
		public byte[] ToByteArray()
		{
			byte[] buf;
			var uppers = _uppers;
			if (uppers.Length == 0)
				buf = new byte[_CalculateBytes(_base)];
			else
			{
				buf = new byte[_CalculateBytes(uppers[uppers.Length - 1])];
				for (int i = 0, pos = 8; i < uppers.Length; i++, pos += 8)
					_ULong2BytesLE(uppers[i], buf, pos);
			}

			_ULong2BytesLE(_base, buf, 0);
			return buf;
		}
	}
}
