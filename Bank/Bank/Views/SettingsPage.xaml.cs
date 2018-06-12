﻿using System;
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
		}

		private void DarkMode_OnChanged(object sender, ToggledEventArgs e) 
			=> Application.Current.Properties["theme"] = e.Value ? "dark" : "light";

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
				Tools.DisplayAlert("Device info", $"Name {device.DeviceName}\nOS: {device.OsVersion}", "Dismiss");
			}
		}
	}
}