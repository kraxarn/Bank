using System;
using Xamarin.Forms;

namespace Bank.Views
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();

			if (!Application.Current.Properties.ContainsKey("name") || !Application.Current.Properties.ContainsKey("avatar"))
			{
				Application.Current.Properties["name"]   = DependencyService.Get<IDeviceInfo>().DeviceName;
				Application.Current.Properties["avatar"] = 0;
			}
		}
	}
}