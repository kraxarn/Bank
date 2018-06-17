using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bank.Views
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				var ex = args.ExceptionObject as Exception;
				DisplayAlert("Unhandled Exception!", ex?.Message + Environment.NewLine + ex?.StackTrace, "wtf?");
			};
			TaskScheduler.UnobservedTaskException += (sender, args) =>
			{
				var ex = args.Exception;
				DisplayAlert("Unobserved Task Exception!", ex?.Message + Environment.NewLine + ex?.StackTrace, "wtf?");
			};

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

			if (Application.Current.Properties.ContainsKey("theme") && Application.Current.Properties["theme"] as string == "dark")
				Tools.CurrentTheme = Tools.Theme.Black;
		}

		protected override void OnAppearing()
		{
			if (!IsPrivateNetwork())
				DisplayAlert("Warning", "It looks like you aren't connected to WiFi, which is required for this app to work", "OK");

			base.OnAppearing();
		}

		private static bool IsPrivateNetwork()
		{
			var ipstr = Tools.IPAddress.Split('.');
			var ip    = new int[ipstr.Length];

			for (var i = 0; i < ipstr.Length; i++)
				ip[i] = int.Parse(ipstr[i]);

			// 16-bit block (192.168.0.0 - 192.168.255.255)
			if (ip[0] == 192 && ip[1] == 168)
				return true;

			// 20-bit block (172.16.0.0 - 172.31.255.255)
			if (ip[0] == 172 && ip[1] >= 16 && ip[1] <= 31)
				return true;

			// 24-bit block (10.0.0.0 - 10.255.255.255)
			if (ip[0] == 10)
				return true;

			return false;
		}
	}
}