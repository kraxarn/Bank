using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bank
{
    abstract class Tools
    {
	    public static string GetIPAddress()
	    {
		    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
		    {
			    socket.Connect("8.8.8.8", 65530);
			    var endPoint = socket.LocalEndPoint as IPEndPoint;
			    return endPoint?.Address.ToString();
		    }
		}
    }
}
