﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
		private Client client;

		public GamePage(Client client, IEnumerable<User> users)
		{
			InitializeComponent();

			this.client = client;

			// Get local user
			// TODO: This doesn't work if we're connecting to a server
			var currentUser = users.Single(u => u.Address == "127.0.0.1");

			ImageAvatar.Source = currentUser.Avatar;
			LabelName.Text     = currentUser.Name;
			LabelMoney.Text    = currentUser.FormattedMoney;
		}

		private async void ButtonBack_OnClicked(object sender, EventArgs e)
		{
			var action = await DisplayActionSheet("Are you sure?", "Cancel", "Quit without saving", "Save and quit");
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