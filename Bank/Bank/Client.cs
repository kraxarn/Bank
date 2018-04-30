using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		    name = Application.Current.Properties.ContainsKey("name")
			    ? Application.Current.Properties["name"] as string
			    : "NAME";

		    avatar = Application.Current.Properties.ContainsKey("avatar")
			    ? int.Parse(Application.Current.Properties["avatar"].ToString())
			    : 0;
	    }

	    public bool TestConnection()
	    {
		    using (var tcp = new TcpClient())
		    {
			    try
			    {
				    return tcp.ConnectAsync(address, port).Wait(500);
			    }
			    catch (SocketException)
			    {
				    return false;
			    }
			    catch (AggregateException)
			    {
				    return false;
			    }
			    finally
			    {
					tcp.Close();
			    }
		    }
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
				    Application.Current.MainPage.DisplayAlert("Invalid response", 
					    $"Got invalid response from server: \n{response}", "OK");

			    stream.Close();
			    client.Close();
		    }
		    catch (SocketException)
		    {
			    client.Close();
			    return false;
		    }
		    catch (IOException)
		    {
				client.Close();
			    return false;
		    }

		    return true;
		}
    }
}
