using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xamarin.Forms;

namespace Bank
{
    public class Client
    {
	    private readonly TcpListener server;
	    private TcpClient client;
	    private readonly string address;
	    private readonly int port;

	    private readonly string name;
	    private readonly int    avatar;

	    public Client(string address = "127.0.0.1", int port = 13000)
	    {
		    this.address = address;
		    this.port    = port;

		    name   = Application.Current.Properties["name"] as string;
		    avatar = int.Parse(Application.Current.Properties["avatar"].ToString());
	    }

		public bool Connect() => Send($"JOIN,{name},{avatar}");

		public bool Send(string message)
	    {
		    try
		    {
				// Create client
			    client = new TcpClient(address, port)
			    {
				    ReceiveTimeout = 5000
			    };

			    // Translate message
				var data = Encoding.ASCII.GetBytes(message);

				// Stream for reading/writing
			    var stream = client.GetStream();
				stream.Write(data, 0, data.Length);

				// Recieve response
				data = new byte[256];

				// String to store response
			    var response = string.Empty;

				// Read first batch of response
			    var bytes = stream.Read(data, 0, data.Length);
			    response = Encoding.ASCII.GetString(data, 0, bytes);

				// Use response here
			    if (response != "OK" && response.Count(c => c == ',') != 2)
				    Application.Current.MainPage.DisplayAlert("Invalid response", $"Got invalid response from server: \n{response}", "OK");

				stream.Close();
				client.Close();
		    }
		    catch (SocketException e)
		    {
			    client.Close();
			    return false;
		    }

		    return true;
		}
    }
}
