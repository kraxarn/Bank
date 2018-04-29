using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bank
{
    public class Server
    {
	    private TcpListener server;

	    public Server(string address, int port = 13000)
	    {
			server = new TcpListener(IPAddress.Parse(address), port);
			server.Start();

		    var bytes = new byte[256];
		    string data = null;

		    while (true)
		    {

		    }
	    }

	    public void Stop() => server.Stop();
    }
}
