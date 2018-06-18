using System;
using System.Diagnostics;
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

			LabelTheme.Text = $"{Tools.CurrentTheme}";
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

		private void ButtonCrash_OnClicked(object sender, EventArgs e)
		{
			try
			{
				int[] x = {0, 1};
				var y = x[2];
			}
			catch (IndexOutOfRangeException err)
			{
				Navigation.PushModalAsync(Tools.CreateNavigationPage(new ErrorPage("Test error", err)));
			}
		}

		private async void StackTheme_OnTapped(object sender, EventArgs e)
		{
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

			await Tools.SavePropertiesAsync();
		}
	}
}