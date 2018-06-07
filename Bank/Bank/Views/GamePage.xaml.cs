using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
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
			currentUser = users.Single(u => u.Address == "127.0.0.1");

			// Don't show ourselves in users list
			users.Remove(currentUser);

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;

			// Set view source
			ViewUsers.ItemsSource = users;

			// Add test user
			users.Add(new User("Test user", 0, "0.0.0.0"));

			// Set handler for clicking user
			ViewUsers.ItemSelected += ViewUsersOnItemSelected;
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
				// For now, quit without saving for both
				case "Quit without saving":
				case "Save and quit":
					await Navigation.PopModalAsync();
					break;
			}
		}

		private async void ButtonSelfRemoveClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(new NavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfRemove, currentUser)));

		private async void ButtonSelfAddClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(new NavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfAdd, currentUser)));
	}
}