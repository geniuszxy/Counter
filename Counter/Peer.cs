using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Counter
{
	class Peer
	{
		private TcpClient _client;
		private NetworkStream _stream;

		public Peer(TcpClient client)
		{
			_client = client;
			_stream = client.GetStream();
		}

		public void Start()
		{

		}
	}
}
