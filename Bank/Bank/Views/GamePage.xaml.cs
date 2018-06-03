using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
		private Client client;

		public GamePage(Client client, ObservableCollection<User> users)
		{
			InitializeComponent();

			this.client = client;

			// Get local user
			// TODO: This doesn't work if we're connecting to a server
			var currentUser = users.Single(u => u.Address == "127.0.0.1");

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;

			// Set view source
			ViewUsers.ItemsSource = users;
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
	}
}