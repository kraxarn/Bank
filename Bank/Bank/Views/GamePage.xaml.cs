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
					// Someone lost money
					lastUser = user.Name;

					// Check value after 0.2 sec
					Task.Run(() =>
					{
						Thread.Sleep(200);

						// If it already is null, we already sent a 'transferred' notification
						if (lastUser != null)
						{
							DisplayToast($"{CheckName(user.Name)} paid ${diffstr}");
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
					DisplayToast($"{CheckName(user.Name)} received ${diffstr}");

				// Fuck it
				UpdateSelf();
			};

			// Set if notifications should be enabled
			notif = (bool) Tools.GetProperty("notifications", false);

			// Fix some sizes on Android
			if (Device.RuntimePlatform == Device.Android)
			{
				ButtonAdd.WidthRequest    = 36;
				ButtonRemove.WidthRequest = 36;

				LabelName.FontSize = 18;
			}
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

		private static void RollDice(out int dice1, out int dice2)
		{
			var rng = new Random();

			dice1 = rng.Next(6) + 1;
			dice2 = rng.Next(6) + 1;
		}
		
		private async void ButtonDice_OnClicked(object sender, EventArgs e)
		{
			RollDice(out var d1, out var d2);

			// Check if we should auto reroll
			if ((bool) Tools.GetProperty("autoRerollDice", false))
			{
				// List to store all rolls
				var rolls = new List<int>(new []{d1, d2});

				// Roll while we're getting doubles
				while (d1 == d2 && rolls.Count < 6)
				{
					RollDice(out d1, out d2);
					rolls.AddRange(new []{d1, d2});
				}

				// Display result
				var title   = $"You rolled {rolls.Sum()}";
				var message = "";

				// Add all dice rolls
				for (var i = 0; i < rolls.Count; i += 2)
					message += $"{GetDiceEmoji(rolls[i])} + {GetDiceEmoji(rolls[i + 1])}\n";

				// Add double message
				if (rolls.Count == 4)
					message += "Double!";
				else if (rolls.Count > 4)
				{
					if (d1 == d2)
						message += "3 doubles!";
					else
						message += "2 doubles!";
				}

				// Display message
				await DisplayAlert(title, message, "Neat!");
			}
			else
			{
				// Get text to display
				var title = $"You rolled {d1 + d2}";
				var message = $"{GetDiceEmoji(d1)} + {GetDiceEmoji(d2)} \n{(d1 == d2 ? "Double!" : null)}";

				// Check if double
				if (d1 == d2)
				{
					if (await DisplayAlert(title, message, "Roll again", "I'm good"))
						await Task.Run(() => Device.BeginInvokeOnMainThread(() => ButtonDice_OnClicked(this, null)));
				}
				else
					await DisplayAlert(title, message, "Neat!");
			}
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