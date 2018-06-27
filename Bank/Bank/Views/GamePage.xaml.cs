using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		private readonly IDeviceInfo device;

		private readonly bool notif;

		private string lastUser;

		public GamePage(Client client, IEnumerable<User> serverUsers)
		{
			InitializeComponent();

			this.client = client;
			users       = new UserCollection(serverUsers);

			var ipAddress = Tools.IPAddress;

			device = DependencyService.Get<IDeviceInfo>();

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

			client.MoneyChanged += (user, oldMoney) =>
			{
				var diff = (int)user.Money - oldMoney;
				var diffstr = Tools.Seperate((uint)(diff >= 0 ? diff : -diff));

				if (diff < 0)
				{
					// TODO: Nothing actually happens if a user just loses money

					// Someone lost money
					lastUser = user.Name;

					// Check value after 0.1 sec
					Task.Run(() =>
					{
						Thread.Sleep(100);

						// If it already is null, we already sent a 'transferred' notification
						if (lastUser != null)
						{
							DisplayToast($"{CheckName(user.Name)} lost ${diffstr}");
							lastUser = null;
						}
					});
				}
				else if (lastUser != null)
				{
					DisplayToast($"{CheckName(lastUser)} sent ${diffstr} to {CheckName(user.Name)}");
					lastUser = null;
				}
				else
					DisplayToast($"{CheckName(user.Name)} got ${diffstr}");

				// Fuck it
				UpdateSelf();
			};

			// Set if notifications should be enabled
			notif = (bool) Tools.GetProperty("notifications", false);
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

		private string CheckName(string name)
			=> name == currentUser.Name ? "You" : name;

		private async void ViewUsersOnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (ViewUsers.SelectedItem == null)
				return;

			await Navigation.PushModalAsync(Tools.CreateNavigationPage(new MoneyPage(client, MoneyPage.Mode.OtherPlayer, currentUser, e.SelectedItem as User)));
			ViewUsers.SelectedItem = null;
		}

		private async void ButtonBack_OnClicked(object sender, EventArgs e)
		{
			if (await DisplayAlert("Are you sure?", "All progress will be lost", "Yes", "No"))
				await Navigation.PopModalAsync();
		}

		private async void ButtonSelfRemoveClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(Tools.CreateNavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfRemove, currentUser)));

		private async void ButtonSelfAddClicked(object sender, EventArgs e) 
			=> await Navigation.PushModalAsync(Tools.CreateNavigationPage(new MoneyPage(client, MoneyPage.Mode.SelfAdd, currentUser)));

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

		private void DisplayToast(string message)
		{
			if (notif)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					device.DisplayToast("Money transferred", message);
				});
			}
		}

		protected override void OnAppearing()
		{
			if ((bool) Tools.GetProperty("preventSleep", false))
				DependencyService.Get<IDeviceInfo>().KeepScreenOn = true;

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			if ((bool) Tools.GetProperty("preventSleep", false))
				DependencyService.Get<IDeviceInfo>().KeepScreenOn = false;

			base.OnDisappearing();
		}
	}
}