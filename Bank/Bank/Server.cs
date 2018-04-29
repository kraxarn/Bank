using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Bank
{
    public class Server
    {
	    private readonly TcpListener server;
	    public  readonly ObservableCollection<User> Users;

	    public bool Running;

	    public Server(string address = null, int port = 13000)
	    {
		    var ip = address == null 
			    ? IPAddress.Any 
			    : IPAddress.Parse(address); 

			Users  = new ObservableCollection<User>();
		    server = new TcpListener(ip, port);
		    Running = false;
	    }

	    public bool Start()
	    {
		    try
		    {
			    server.Start();
			    var thread = new Thread(ServerThread);
			    thread.Start();
			}
		    catch (SocketException)
		    {
			    server.Stop();
				return false;
		    }

		    Running = true;
		    return true;
	    }

	    private void ServerThread()
	    {
		    var bytes = new byte[256];
		    string data = null;

		    while (true)
		    {
				// Blocking call to accept requests
			    TcpClient client = null;
			    try
			    {
				    client = server.AcceptTcpClient();
			    }
			    catch (SocketException e)
			    {
					client?.Close();
				    continue;
			    }


			    data = null;

				// Stream object for reading/writing
			    var stream = client.GetStream();

				// Get all data sent by client
			    int i;
			    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
			    {
				    data = Encoding.ASCII.GetString(bytes, 0, i);
				    var dat = data.Split(',');


				    if (dat[0] == "JOIN")
				    {
						Users.Add(new User(dat[1], int.Parse(dat[2]), ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()));
				    }

				    var msg = Encoding.ASCII.GetBytes("OK");
					stream.Write(msg, 0, msg.Length);

					// Process data here and send response
			    }

				client.Close();
		    }
		}

	    public void Stop() => server.Stop();
    }
}
