using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage
	{
		private readonly IDeviceInfo device;
		private byte tapped;

		public SettingsPage()
		{
			InitializeComponent();

			tapped = 0;
			device = DependencyService.Get<IDeviceInfo>();
			LabelVersion.Text = Tools.Version.Substring(1);

			LabelTheme.Text = Device.RuntimePlatform == Device.UWP ? "System" : $"{Tools.CurrentTheme}";

			SwitchPreventSleep.IsToggled   = (bool) Tools.GetProperty("preventSleep",   false);
			SwitchNotifications.IsToggled  = (bool) Tools.GetProperty("notifications",  false);
			SwitchAutoRerollDice.IsToggled = (bool) Tools.GetProperty("autoRerollDice", false);
		}

		private async void ButtonProfile_OnClicked(object sender, EventArgs e) 
			=> await Navigation.PushAsync(new ProfilePage());

		protected override async void OnDisappearing() 
			=> await Application.Current.SavePropertiesAsync();

		private void LabelVersion_OnTapped(object sender, EventArgs e)
		{
			if (tapped < 3)
				tapped++;
			else
			{
				tapped = 0;
				Tools.DisplayAlert("Device info", $"Name {device.DeviceName}\nOS: {device.OsVersion}");
			}
		}

		private async void StackTheme_OnTapped(object sender, EventArgs e)
		{
			if (Device.RuntimePlatform == Device.UWP)
			{
				await DisplayAlert("Changing Theme", "Please select a theme from the system settings instead", "Dismiss");
				return;
			}

			var action = await DisplayActionSheet("Select theme", "Cancel", null, "Light", "Dark", "Black");

			switch (action)
			{
				case "Light":
					Tools.CurrentTheme = Tools.Theme.Light;
					Tools.SetProperty("theme", "light");
					break;

				case "Dark":
					Tools.CurrentTheme = Tools.Theme.Dark;
					Tools.SetProperty("theme", "dark");
					break;

				case "Black":
					Tools.CurrentTheme = Tools.Theme.Black;
					Tools.SetProperty("theme", "black");
					break;
			}

			if (action != "Cancel")
				LabelTheme.Text = action;
		}

		private void SwitchPreventSleep_OnToggled(object sender, ToggledEventArgs e) 
			=> Tools.SetProperty("preventSleep", SwitchPreventSleep.IsToggled);

		private void SwitchNotifications_OnToggled(object sender, ToggledEventArgs e)
			=> Tools.SetProperty("notifications", SwitchNotifications.IsToggled);

		private void SwitchAutoRerollDice_OnToggled(object sender, ToggledEventArgs e)
			=> Tools.SetProperty("autoRerollDice", SwitchAutoRerollDice.IsToggled);

		private async void Setting_OnTapped(object sender, EventArgs e)
		{
			var args = (TappedEventArgs) e;

			var info = "No info found for setting";

			switch (args.Parameter)
			{
				case "Shorten Money":
					info = "Shorten and round money. For example, $1,450 will show as $1.5k";
					break;

				case "Notifications":
					info = "Display a notification when someone transfers money to someone";
					break;

				case "Prevent Sleep":
					info = "Prevents the device from going to sleep while on the game page";
					break;

				case "Auto roll dice":
					info = "Automatically roll dice again and sum results if you roll double (up to 3 times)";
					break;
			}

			await DisplayAlert($"{args.Parameter}", info, "Dismiss");
		}
	}
}