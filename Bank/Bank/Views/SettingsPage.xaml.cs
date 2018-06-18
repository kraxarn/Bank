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

			if (Tools.CurrentTheme == Tools.Theme.Black)
				CellDarkMode.On = true;
		}

		private void DarkMode_OnChanged(object sender, ToggledEventArgs e)
		{
			Application.Current.Properties["theme"] = e.Value ? "dark" : "light";
			Tools.CurrentTheme = CellDarkMode.On ? Tools.Theme.Black : Tools.Theme.Light;
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
	}
}