using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;

namespace Counter
{
	public partial class CounterService : ServiceBase
	{
		static CounterService _inst;
		TcpListener _tcpListener;
		AsyncCallback _onConnect;
		Manager _manager;

		public CounterService()
		{
			ServiceName = "Counter";
		}

		protected override void OnStart(string[] args)
		{
			_inst = this;
			_onConnect = OnConnect;
			_manager = new Manager();

			Peer.InitCommandDelegates();

			_tcpListener = new TcpListener(IPAddress.Any, 0);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(_onConnect, null);
		}

		protected override void OnStop()
		{
			_tcpListener.Stop();
			_inst = null;
		}

		private void OnConnect(IAsyncResult asyncResult)
		{
			var tcpClient = _tcpListener.EndAcceptTcpClient(asyncResult);
			_tcpListener.BeginAcceptTcpClient(_onConnect, null);

			//Create a peer to handle client's commands
			Peer.Create(tcpClient);
		}

		internal static CounterService Instance
		{
			get { return _inst; }
		}

		internal static Manager Manager
		{
			get { return _inst._manager; }
		}
	}
}
