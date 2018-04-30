using System;
using Xamarin.Forms;

namespace Bank.Views
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();

			// Add tab icons on iOS
			if (Device.RuntimePlatform == Device.iOS)
			{
				PageCreate.Icon   = "images/ui/host.png";
				PageJoin.Icon     = "images/ui/join.png";
				PageSettings.Icon = "images/ui/settings.png";
			}

			if (!Application.Current.Properties.ContainsKey("name") || !Application.Current.Properties.ContainsKey("avatar"))
			{
				Application.Current.Properties["name"]   = DependencyService.Get<IDeviceInfo>().DeviceName;
				Application.Current.Properties["avatar"] = 0;
			}
		}
	}
}