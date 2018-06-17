using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HostPage
	{ 
		private Server server;
		private Client client;

		public HostPage()
		{
			InitializeComponent();

			Enter();

			var ip = Tools.IPAddress;
			LabelRoom.Text += ip == null ? "Error" : ip.Substring(ip.LastIndexOf('.') + 1);

			// Disable selection of users
			ViewUsers.ItemSelected += (sender, args) => ViewUsers.SelectedItem = null;

			// Set starting money
			var money = Convert.ToUInt32(Tools.GetProperty("startingMoney", 1500));

			if (money >= 1000000)
			{
				// M
				EntryMoney.Text = $"{money / 1000000f}";
				PickerMoney.SelectedIndex = 0;
			}
			else if (money >= 1000)
			{
				// K
				EntryMoney.Text = $"{money / 1000f}";
				PickerMoney.SelectedIndex = 1;
			}
			else
			{
				// -
				EntryMoney.Text = $"{money}";
				PickerMoney.SelectedIndex = 2;
			}

			server.Stopped += () =>
			{
				server = null;
				client = null;
			};
		}

		private void Enter()
		{
			client = new Client(Tools.IPAddress);

			server = new Server(1500);
			ViewUsers.ItemsSource = server.Users;
		}

		private async void ButtonStart_OnClicked(object sender, EventArgs e)
		{
			if (server.Users.Count < 2)
			{
				const string title = "Not enough players";
				const string msg   = "Add more players and try again";

				#if DEBUG
					if (await Application.Current.MainPage.DisplayAlert(title, msg, "Dismiss", "Ignore"))
						return;
				#else
					await Application.Current.MainPage.DisplayAlert(title, msg, "Dismiss");
					return;
				#endif
			}

			if (!float.TryParse(EntryMoney.Text.Replace(',', '.'), out var startingMoney) || PickerMoney.SelectedIndex < 0)
			{
				await Application.Current.MainPage.DisplayAlert("No or invalid starting money", "Please select starting money first", "OK");
				return;
			}

			// Save starting money to settings
			switch (PickerMoney.SelectedIndex)
			{
				case 0: startingMoney *= 1000000; break;
				case 1: startingMoney *= 1000;    break;
			}

			server.StartingMoney = (uint) startingMoney;

			Tools.SetProperty("startingMoney", startingMoney);
			await Tools.SavePropertiesAsync();

			// Send users to other players
			server.BroadcastUsers();

			await Navigation.PushModalAsync(new NavigationPage(new GamePage(client, client.Users)));
		}

		protected override void OnAppearing()
		{
			// Start everything again
			if (server == null)
				Enter();
				
			if (!server.Running)
			{
				// Start server
				if (!server.Start())
					Application.Current.MainPage.DisplayAlert("Server Error", "Error starting local server", "OK");
				else if (!client.Connect(out var error))
					Navigation.PushModalAsync(new NavigationPage(new ErrorPage("Server error", error)));
			}

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			// If modal is null, we switched tab
			if (Tools.CurrentModalPage == default(Page))
			{
				// Shut down listener if running
				if (client.ListenerRunning)
					server.Send(Tools.IPAddress, "STOP");

				// Shut down server if running
				if (server.Running)
					client.Send("STOP", out _);
			}

			base.OnDisappearing();
		}
	}
}