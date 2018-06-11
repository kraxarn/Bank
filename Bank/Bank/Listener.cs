using System.Collections.Generic;
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
	/*
	 * A listener is similar to the server,
	 * but it only listens for data and triggers
	 * events based on the response.
	 */

	/*
	 * OnJoin
	 * OnAdd
	 * OnRemove
	 * OnSet
	 * OnNew
	 */

	public class Listener
	{

		#region Events

		public delegate void JoinEvent(User joinedUser);
		public delegate void MoneyEvent(User changedUser); // Or just address and money

		/// <summary>
		/// A player has joined (or already joined)
		/// </summary>
		public event JoinEvent  PlayerJoined;

		/// <summary>
		/// Money has changed for a user. 
		/// This is triggered twice if money is transfered.
		/// </summary>
		public event MoneyEvent MoneyChanged;

		#endregion

		private readonly TcpListener server;
		private readonly ObservableCollection<User>  users;
		
		public bool Running { get; private set; }

		public ObservableCollection<User> Users => users;

		public delegate void ReadyEvent();

		public event ReadyEvent Ready;

		public Listener()
		{
			var ip = IPAddress.Any;
			// Assume port
			server = new TcpListener(ip, 13000);
			Running = false;

			users = new ObservableCollection<User>();

			Debug.WriteLine($"Listener started on {ip}");
		}

		private void InvokeNewPlayer(User user) 
			=> PlayerJoined?.Invoke(user);

		private void InvokeMoneyChange(User user)
			=> MoneyChanged?.Invoke(user);

		public bool Start(out string error)
		{
			try
			{
				server.Start();
				var thread = new Thread(Thread);
				thread.Start();
			}
			catch (SocketException e)
			{
				server?.Stop();
				error = e.Message;
				return false;
			}

			Running = true;
			error = null;
			return true;
		}

		private void Thread()
		{
			// Similar to Server.ServerThread
			var bytes = new byte[256];

			while (Running)
			{
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

				var ok = Encoding.ASCII.GetBytes("OK");
				var stream = client.GetStream();
				int i;

				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					var data = Encoding.ASCII.GetString(bytes, 0, i);

					if (data == "STOP")
					{
						stream.Write(ok, 0, ok.Length);
						Running = false;
						break;
					}

					var dat = data.Split(',');

					Debug.WriteLine($"ListenerData: '{data}'");

					if (dat[0] == "JOIN")
					{
						var ip = ((IPEndPoint) client.Client.LocalEndPoint).Address;

						Debug.WriteLine($"Added user: '{dat[1]}' from '{ip}'");

						var u = new User(dat[1], int.Parse(dat[2]), dat[3]);
						InvokeNewPlayer(u);
						users.Add(u);
					}
					else if (dat[0] == "ADD")
					{
						Debug.WriteLine($"Users: {users.Count}");

						foreach (var u in users)
							Debug.WriteLine($"{u.Name}: {u.Address} / {u.FormattedAddress}");

						// TODO: Temporary workaround for multiple users bug
						var user = users.FirstOrDefault(u => u.Address == dat[1]);

						if (user != default(User) && uint.TryParse(dat[2], out var amount))
						{
							user.Money += amount;
							InvokeMoneyChange(user);
						}
						else
							DisplayAlert("Failed to add money", "The specified user could not be found");
					}
					else if (dat[0] == "REM")
					{
						// TODO: Yet another workround
						var user = users.FirstOrDefault(u => u.Address == dat[1]);

						if (user != default(User) && uint.TryParse(dat[2], out var amount))
						{
							user.Money -= amount;
							InvokeMoneyChange(user);
						}
						else
							DisplayAlert("Failed to add money", "The specified user could not be found");
					}
					else if (dat[0] == "GO")
						Ready?.Invoke();

					stream.Write(ok, 0, ok.Length);
				}

				client.Close();
			}
			
			server.Stop();
		}

		private static void DisplayAlert(string title, string message, string button = "Dismiss") 
			=> Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(title, message, button));
	}
}