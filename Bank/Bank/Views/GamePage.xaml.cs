using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage
	{
		private readonly Client client;
		private readonly List<User> users;
		private readonly User currentUser;

		public GamePage(Client client, IEnumerable<User> serverUsers)
		{
			InitializeComponent();

			this.client = client;
			users = new List<User>(serverUsers);

			// Get local user
			// TODO: This doesn't work if we're connecting to a server
			// TODO: Create a new user identical to the current one instead
			currentUser = users.Single(u => u.Address == "127.0.0.1");

			// Don't show ourselves in users list
			users.Remove(currentUser);

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;

			// Set view source
			ViewUsers.ItemsSource = users;

			// Add test user
			//users.Add(new User("Test user", 0, "127.0.0.1"));

			// Set handler for clicking user
			ViewUsers.ItemSelected += ViewUsersOnItemSelected;

			// Catch events
			client.PlayerJoined += user => Debug.WriteLine($"User joined: {user.Name}");

			client.MoneyChanged += user =>
			{
				Debug.WriteLine($"MoneyChanged: {user.Name} = {user.FormattedMoney} / {user.Money}");

				if (user.Address == "127.0.0.1")
				{
					currentUser.Money = user.Money;
					UpdateSelf();
				}

				/*
				var ur = users.SingleOrDefault(u => u.Address == user.Address);

				Debug.WriteLine($"Modifying user '{ur?.Name}'");
				
				void AddUser()
				{
					ur.Money = user.Money;
					ViewUsers.BeginRefresh();
				}

				if (Device.RuntimePlatform == Device.UWP)
					Device.BeginInvokeOnMainThread(AddUser);
				else
					AddUser();

				Debug.WriteLine($"{ur.Name} ({user.Name}) now has {user.FormattedMoney}");
				*/
			};
		}

		private void UpdateSelf()
		{
			void Update()
			{
				ImageAvatar.Source = currentUser.Avatar;
				LabelName.Text = currentUser.Name;
				LabelMoney.Text = currentUser.FormattedMoney;
			}

			if (Device.RuntimePlatform == Device.UWP)
				Device.BeginInvokeOnMainThread(Update);
			else
				Update();
		}

		private async void ViewUsersOnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (ViewUsers.SelectedItem == null)
				return;

			await Navigation.PushModalAsync(new NavigationPage(new MoneyPage(client, MoneyPage.Mode.OtherPlayer, currentUser, users[0])));
			ViewUsers.SelectedItem = null;
		}

		private async void ButtonBack_OnClicked(object sender, EventArgs e)
		{
			string action;

			if (Device.RuntimePlatform == Device.iOS)
				action = await DisplayActionSheet("Are you sure?", "Cancel", "Quit without saving", "Save and quit");
			else
				action = await DisplayActionSheet("Are you sure?", "Cancel", null, "Quit without saving", "Save and quit");

			switch (action)
			{
				case "Save and quit":
					await Application.Current.MainPage.DisplayAlert("Sorry",
						"This feature hasn't been implemented yet.", "Dismiss");
					break;

				case "Quit without saving":
					await Navigation.PopModalAsync();
					break;
			}
		}

		private async void ButtonSelfRemoveClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(new NavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfRemove, currentUser)));

		private async void ButtonSelfAddClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(new NavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfAdd, currentUser)));

		private async void ButtonDice_OnClicked(object sender, EventArgs e)
		{
			var rng = new Random();

			// First dice
			var d1 = rng.Next(6) + 1;

			// Second dice
			var d2 = rng.Next(6) + 1;

			// 'Double' message
			var d = d1 == d2 ? "Double!" : null;

			await DisplayAlert($"You rolled {d1 + d2}", $"{GetDiceEmoji(d1)} + {GetDiceEmoji(d2)} \n{d}", "Neat!");
		}

		private static string GetDiceEmoji(int index)
		{
			switch (index)
			{
				case 1:  return "1️⃣";
				case 2:  return "2️⃣";
				case 3:  return "3️⃣";
				case 4:  return "4️⃣";
				case 5:  return "5️⃣";
				case 6:  return "6️⃣";
				default: return "0️⃣";
			}
		}
	}
}