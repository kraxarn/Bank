using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bank.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();

			var device = DependencyService.Get<IDeviceInfo>();
			LabelDeviceName.Text = device.DeviceName;
			LabelDeviceOs.Text    = device.OSVersion;
		}

		private void DarkMode_OnChanged(object sender, ToggledEventArgs e)
		{
			if (!e.Value)
				return;

			Application.Current.MainPage.DisplayAlert("Sorry", "Not implemented yet", "OK");
			CellDarkMode.On = false;
		}

		private async void ButtonProfile_OnClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfilePage());
		}
	}
}