using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Counter
{
	public partial class CounterService : ServiceBase
	{
		static CounterService _inst;
		TcpListener _tcpListener;
		AsyncCallback _onConnect;
		List<Peer> _peers;
		Manager _manager;

		public CounterService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_inst = this;
			_onConnect = OnConnect;
			_peers = new List<Peer>();
			_manager = new Manager();

			Peer.InitCommandDelegates();

			_tcpListener = new TcpListener(IPAddress.Any, 0);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(_onConnect, null);
		}

		protected override void OnStop()
		{
			lock(_peers)
			{
				foreach (var peer in _peers)
					peer.Stop();
				_peers.Clear();
			}
			_tcpListener.Stop();
			_inst = null;
		}

		private void OnConnect(IAsyncResult asyncResult)
		{
			var tcpClient = _tcpListener.EndAcceptTcpClient(asyncResult);
			_tcpListener.BeginAcceptTcpClient(_onConnect, null);

			var peer = new Peer(tcpClient);
			lock(_peers)
				_peers.Add(peer);
			peer.Start();
		}

		internal static Manager Manager
		{
			get { return _inst._manager; }
		}
	}
}
