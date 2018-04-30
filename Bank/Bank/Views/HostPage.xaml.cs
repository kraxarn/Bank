using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HostPage : ContentPage
	{ 
		private readonly Server server;
		private readonly Client client;

		public HostPage()
		{
			InitializeComponent();

			string ip;
			using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			{
				socket.Connect("8.8.8.8", 65530);
				var endPoint = socket.LocalEndPoint as IPEndPoint;
				ip = endPoint?.Address.ToString();
			}

			client = new Client();

			server = new Server();
			ViewUsers.ItemsSource = server.Users;

			//var name = Application.Current.Properties["name"] as string;
			//var avatar = (int)Application.Current.Properties["avatar"];

			//users.Add(new User(name, avatar, ip));
			LabelRoom.Text += ip.Substring(ip.LastIndexOf('.') + 1);
		}

		private void ButtonStart_OnClicked(object sender, EventArgs e)
		{
			if (server.Users.Count < 2)
			{
				Application.Current.MainPage.DisplayAlert("Not enough players", "Try adding more players and try again", "OK");
				return;
			}
		}

		protected override void OnAppearing()
		{
			if (!server.Running)
			{
				// Start server
				if (!server.Start())
					Application.Current.MainPage.DisplayAlert("Socket Error", "Error starting server", "OK");
				else if (!client.Connect())
					Application.Current.MainPage.DisplayAlert("Socket Error", "Error connecting to server", "OK");
			}

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			if (server.Running)
				client.Send("STOP");

			base.OnDisappearing();
		}
	}
}