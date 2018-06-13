using System;
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

	    private uint startingMoney;

	    public uint StartingMoney
		{
			set => startingMoney = value;
		}

		public Server(uint startingMoney, string address = null, int port = 13001)
	    {
		    var ip = address == null 
			    ? IPAddress.Any 
			    : IPAddress.Parse(address); 

			Users  = new ObservableCollection<User>();
		    server = new TcpListener(ip, port);
			Running = false;

			// Set starting money
		    this.startingMoney = startingMoney;
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
					 * JOIN: name,avatarIndex,ip,money	// For when someone joins
					 * ADD:  ip,amount					// Added money to user
					 * REM:  ip,amount					// Removed money from user
					 * SET:  ip.amount					// Just set a new value for someone
					 * NEW:  ip.amount					// A user has a new amount of money
					 * BYE:  ip							// Removes the user
					 * GO:								// Everyone's in, lets go!
					 *
					 * TODO: Are ADD/REM/SET even used?
					 */

					Debug.WriteLine($"ServerData: '{data}'");
					
				    if (dat[0] == "JOIN")
				    {
						var user = new User(dat[1], int.Parse(dat[2]), ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString(), startingMoney);

						// Add user if it does not already exist
					    // TODO: Maybe we want to use UserCollection here as well
					    if (!Users.Contains(user))
					    {
						    // UWP needs this to run on the main thread
						    if (Device.RuntimePlatform == Device.UWP)
							    Device.BeginInvokeOnMainThread(() => Users.Add(user));
						    else
							    Users.Add(user);
					    }
					    else
						    Debug.WriteLine($"Warning: Server tried to add duplicate user ({user.Address})");
				    }
					else if (dat[0] == "BYE")
				    {
					    var user = Users.SingleOrDefault(u => u.Address == dat[1]);

					    if (Device.RuntimePlatform == Device.UWP)
						    Device.BeginInvokeOnMainThread(() => Users.Remove(user));
					    else
						    Users.Remove(user);
					}
					else if (dat[0] == "ADD" || dat[0] == "REM")
				    {
					    var user = Users.SingleOrDefault(u => u.Address == dat[1]);

					    if (user != default(User) && uint.TryParse(dat[2], out var amount))
					    {
						    if (dat[0] == "ADD")
							    user.Money += amount;
						    else
							    user.Money -= amount;

						    Broadcast($"SET,{user.Address},{user.Money}");
					    }
					}
				    else
				    {
					    // Send data to clients (listeners)
						// TODO: Does this really need to be done on main thead?
					    void B() => Broadcast(data);

					    if (Device.RuntimePlatform == Device.UWP)
						    Device.BeginInvokeOnMainThread(B);
					    else
						    B();
					}

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

		/// <summary>
		/// Broadcasts all users known to the server and then 'GO'
		/// </summary>
	    public void BroadcastUsers()
	    {
		    foreach (var user in Users)
			    Broadcast($"JOIN,{user.Name},{user.AvatarIndex},{user.Address},{startingMoney}");

		    Broadcast("GO");
		}

	    public void Send(string address, string message)
	    {
			Debug.WriteLine($"ServerSend: '{message}' to '{address}'");

			// TODO: This (randomly) throws timed out errors
		    try
		    {
			    using (var client = new TcpClient(address, 13000))
			    {
				    // Similar to Client.Send
				    var data = Encoding.ASCII.GetBytes(message);
				    var stream = client.GetStream();
				    stream.Write(data, 0, data.Length);

				    data = new byte[256];
				    var bytes = stream.Read(data, 0, data.Length);
				    var response = Encoding.ASCII.GetString(data, 0, bytes);

				    if (response != "OK")
					    Application.Current.MainPage.DisplayAlert("Invalid response", response, "OK");

				    stream.Close();
				    client.Close();
			    }
		    }
		    catch (Exception e)
		    {
				Debug.WriteLine($"{e.GetType().FullName}: {e.Message}");
		    }
	    }
    }
}
