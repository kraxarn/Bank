using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Xamarin.Forms;

namespace Bank
{
    public class Client
    {
	    private TcpClient client;
	    private readonly Listener listener;
	    private readonly string address;
	    private readonly int port;

	    private readonly string name;
	    private readonly int    avatar;

	    public event Listener.JoinEvent  PlayerJoined;
	    public event Listener.MoneyEvent MoneyChanged;
	    public event Listener.ReadyEvent Ready;

	    public ObservableCollection<User> Users => listener.Users;

	    public bool ListenerRunning => listener.Running;

	    public Client(string address, int port = 13001)
	    {
		    this.address = address;
		    this.port    = port;

			// We prob want to update these if settings changes
		    name = Application.Current.Properties.ContainsKey("name")
			    ? Application.Current.Properties["name"] as string
			    : "NAME";

		    avatar = Application.Current.Properties.ContainsKey("avatar")
			    ? int.Parse(Application.Current.Properties["avatar"].ToString())
			    : 0;

			listener = new Listener();
		    if (!listener.Start(out var err))
			    Application.Current.MainPage.DisplayAlert("Failed to start listener", err, "Dismiss");

		    listener.PlayerJoined += user => PlayerJoined?.Invoke(user);
		    listener.MoneyChanged += user => MoneyChanged?.Invoke(user);

		    listener.Ready += () =>
		    {
				Device.BeginInvokeOnMainThread(() =>
				{
					Ready?.Invoke();
				});
		    };
		}

	    public bool TestConnection(out string message)
	    {
		    message = "Connection timed out";

		    using (var tcp = new TcpClient())
		    {
			    try
			    {
				    return tcp.ConnectAsync(address, port).Wait(500);
			    }
			    catch (SocketException e)
			    {
				    message = e.Message;
				    return false;
			    }
			    catch (AggregateException e)
			    {
				    message = e.Message;
				    return false;
			    }
			    finally
			    {
					tcp.Close();
			    }
		    }
	    }

	    public bool Connect(out Exception error)
	    {
		    var result = Send($"JOIN,{name},{avatar},{Tools.IPAddress}", out var e);
		    error = e;
		    return result;
	    }

	    public bool Send(string message, out Exception error)
		{
			error = null;

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

			    // Read first batch of response
			    var bytes = stream.Read(data, 0, data.Length);
			    var response = Encoding.ASCII.GetString(data, 0, bytes);

			    // Use response here
			    if (response != "OK" && response.Count(c => c == ',') != 2)
				    Application.Current.MainPage.DisplayAlert("Invalid response", 
					    $"Got invalid response from server: \n{response}", "OK");

			    stream.Close();
			    client.Close();
		    }
		    catch (SocketException e)
		    {
			    error = e;
			    client?.Close();
			    return false;
		    }
		    catch (IOException e)
		    {
			    error = e;
				client?.Close();
			    return false;
		    }

		    return true;
		}
    }
}
