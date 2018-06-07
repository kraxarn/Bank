using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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

		    while (Running)
		    {
				// Blocking call to accept requests
			    TcpClient client = null;
			    try
			    {
				    client = server.AcceptTcpClient();
			    }
			    catch (SocketException)
			    {
				    client?.Close();
				    continue;
			    }
			    catch (TargetInvocationException)
			    {
					client?.Close();
					continue;
			    }

			    // Stream object for reading/writing
			    var stream = client.GetStream();

				// Get all data sent by client
			    int i;
			    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
			    {
				    var data = Encoding.ASCII.GetString(bytes, 0, i);
				    var ok   = Encoding.ASCII.GetBytes("OK");

					if (data == "STOP")
				    {
						stream.Write(ok, 0, ok.Length);
					    Running = false;

					    // UWP needs this to run on the main thread
					    if (Device.RuntimePlatform == Device.UWP)
						    Device.BeginInvokeOnMainThread(() => Users.Clear());
					    else
						    Users.Clear();

						break;
				    }

				    var dat = data.Split(',');

					/*
					 * TODO: Use ID instead of IP?
					 * JOIN: name,avatarIndex	// For when someone joins
					 * ADD:  ip,amount			// Added money to user
					 * REM:  ip,amount			// Removed money from user
					 * SET:  ip.amount			// Just set a new value for someone
					 * NEW:  ip.amount			// A user has a new amount of money
					 *
					 * TODO: Just add on 'JOIN' and then always broadcast?
					 */
					
				    if (dat[0] == "JOIN")
				    {
					    var user = new User(dat[1], int.Parse(dat[2]), ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString());

						// UWP needs this to run on the main thread
						if (Device.RuntimePlatform == Device.UWP)
							Device.BeginInvokeOnMainThread(() => Users.Add(user));
						else
							Users.Add(user);
				    }
					else if (dat[0] == "ADD")
				    {
						// TODO: Should this be here or just in the listener?
						/*
					    var user = Users.Single(u => u.Address == dat[1]);

					    if (user != null && uint.TryParse(dat[2], out var amount))
					    {
						    user.Money += amount;
							Broadcast($"ADD,{user.Address},{user.Money}");
					    }
						*/
				    }

					// Send data to clients (listeners)
					Debug.WriteLine($"BROADCAST: '{data}'");
					Broadcast(data);

					// Send 'OK' back to the client
				    stream.Write(ok, 0, ok.Length);
			    }

				client.Close();
		    }

			server.Stop();
		}

	    private void Broadcast(string message)
	    {
		    foreach (var user in Users)
			    Send(user.Address, message);
	    }

	    private void Send(string address, string message)
	    {
		    using (var client = new TcpClient(address, 13000))
		    {
				// Similar to Client.Send
			    var data = Encoding.ASCII.GetBytes(message);
			    var stream = client.GetStream();
				stream.Write(data, 0, data.Length);

			    data = new byte[256];
			    var response = string.Empty;
			    var bytes = stream.Read(data, 0, data.Length);
			    response = Encoding.ASCII.GetString(data, 0, bytes);

			    if (response != "OK")
				    Application.Current.MainPage.DisplayAlert("Invalid response", response, "OK");

				stream.Close();
				client.Close();
		    }

			/*
			// TODO: Don't use client here
		    var client = new Client(address);
		    return client.Connect(out _) && client.Send(message, out _);
			*/
	    }
    }
}
