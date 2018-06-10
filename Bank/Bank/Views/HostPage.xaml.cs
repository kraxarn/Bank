using System;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HostPage
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

			LabelRoom.Text += ip == null ? "Error" : ip.Substring(ip.LastIndexOf('.') + 1);

			// Disable selection of users
			ViewUsers.ItemSelected += (sender, args) => ViewUsers.SelectedItem = null;

			EntryMoney.Text = "1500";
			PickerMoney.SelectedIndex = 2;
		}

		private async void ButtonStart_OnClicked(object sender, EventArgs e)
		{
			if (server.Users.Count < 2)
			{
				if (await Application.Current.MainPage.DisplayAlert("Not enough players",
					"Try adding more players", "OK", "Ignore"))
					return;
			}

			if (!uint.TryParse(EntryMoney.Text, out _) || PickerMoney.SelectedIndex < 0)
			{
				await Application.Current.MainPage.DisplayAlert("No starting money", "Please select starting money first", "OK");
				return;
			}	

			await Navigation.PushModalAsync(new NavigationPage(new GamePage(client, server.Users)));
		}

		protected override void OnAppearing()
		{
			if (!server.Running)
			{
				// Start server
				if (!server.Start())
					Application.Current.MainPage.DisplayAlert("Server Error", "Error starting local server", "OK");
				else if (!client.Connect(out var error))
					Application.Current.MainPage.DisplayAlert($"Server Error ({error.GetType().FullName})", error.Message, "OK");
			}

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			// If modal is null, we just switched tab and can stop the server
			if (Tools.CurrentModalPage == default(Page) && server.Running)
				client.Send("STOP", out _);

			base.OnDisappearing();
		}
	}
}