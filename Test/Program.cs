using System;
using System.Threading;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			int i = int.MaxValue, j;
			var k = checked(j = Interlocked.Increment(ref i));
			Console.WriteLine(k);
			Console.WriteLine(j);
		}
	}
}
