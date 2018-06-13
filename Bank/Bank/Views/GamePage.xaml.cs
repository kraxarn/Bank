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
		private readonly UserCollection users;
		private readonly User currentUser;

		public GamePage(Client client, IEnumerable<User> serverUsers)
		{
			InitializeComponent();

			this.client = client;
			users       = new UserCollection(serverUsers);

			var ipAddress = Tools.IPAddress;

			// Get local user
			currentUser = users.Single(u => u.Address == ipAddress);

			// Don't show ourselves in users list
			users.Remove(currentUser);

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;

			// Set view source
			ViewUsers.ItemsSource = users;

			// Set handler for clicking user
			ViewUsers.ItemSelected += ViewUsersOnItemSelected;

			// Catch events
			client.PlayerJoined += user => Debug.WriteLine($"User joined: {user.Name}");

			client.MoneyChanged += user =>
			{
				Debug.WriteLine($"MoneyChanged: {user.Name} = {user.FormattedMoney} / {user.Money}");

				if (user.Address == ipAddress)
				{
					currentUser.Money = user.Money;
					UpdateSelf();
				}
				else
					Device.BeginInvokeOnMainThread(() => users[user.Address] = user.Money);
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

			Device.BeginInvokeOnMainThread(Update);
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
			if (await DisplayAlert("Are you sure?", "All progress will be lost", "Yes", "No"))
				await Navigation.PopModalAsync();
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

		private void LabelMoney_OnTapped(object sender, EventArgs e) 
			=> DisplayAlert("Your money", $"${Tools.Seperate(currentUser.Money)}", "Dismiss");
	}
}