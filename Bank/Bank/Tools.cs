using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bank
{
	internal abstract class Tools
	{
		public enum Theme { Light, Black, Dark }

		public static string Version
		{
			get
			{
				var ver = "v1.0.0-beta.7";
				#if DEBUG
					ver += "-dev";
				#endif
				return ver;
			}
		}

		public static string IPAddress
		{
			get
			{
				using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
				{
					socket.Connect("8.8.8.8", 65530);
					var endPoint = socket.LocalEndPoint as IPEndPoint;
					return endPoint?.Address.ToString();
				}
			}
		}

		public static void DisplayAlert(string title, string message, string button = "Dismiss")
	    {
		    void Alert() => Application.Current.MainPage.DisplayAlert(title, message, button);

			if (Device.IsInvokeRequired)
				Device.BeginInvokeOnMainThread(Alert);
			else
				Alert();
	    }

		public static Page CurrentModalPage => Application.Current.MainPage.Navigation.ModalStack.FirstOrDefault();

	    public static Page CurruentPage => Application.Current.MainPage.Navigation.NavigationStack.FirstOrDefault();

	    public static string Seperate(uint value)
	    {
		    var format = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
		    format.NumberGroupSeparator = ",";
			return value.ToString("#,0", format);
		}

		public static object GetProperty(string key, object fallback) 
			=> Application.Current.Properties.ContainsKey(key) ? Application.Current.Properties[key] : fallback;

		public static void SetProperty(string key, object value) 
			=> Application.Current.Properties[key] = value;

		public static async Task SavePropertiesAsync() 
			=> await Application.Current.SavePropertiesAsync();
			
		public static void SaveProperties()
		{
			var task = SavePropertiesAsync();
			task.Wait();
		}

		public static NavigationPage CreateNavigationPage(Page root)
		{
			var page = new NavigationPage(root);

			if (Application.Current.Resources.ContainsKey("backgroundColor"))
				page.BarBackgroundColor = (Color) Application.Current.Resources["backgroundColor"];

			if (Application.Current.Resources.ContainsKey("textColor"))
				page.BarTextColor = (Color)Application.Current.Resources["textColor"];

			return page;
		}

		public static Theme CurrentTheme
		{
			get
			{
				switch (GetProperty("theme", "light"))
				{
					case "dark":  return Theme.Dark;
					case "black": return Theme.Black;
					default:      return Theme.Light;
				}
			}
			set
			{
				var device = DependencyService.Get<IDeviceInfo>();

				switch (value)
				{
					case Theme.Light:
						Application.Current.Resources["backgroundColor"] = Color.Default;
						Application.Current.Resources["textColor"]       = Color.Default;
						Application.Current.Resources["controlColor"]    = Color.Default;
						Application.Current.Resources["infoColor"]       = Color.FromHex("#b0bec5");
						device.SetDarkStatusBar();
						break;

					case Theme.Black:
						Application.Current.Resources["backgroundColor"] = Color.Black;
						Application.Current.Resources["textColor"]       = Color.FromHex("#f5f5f5");
						Application.Current.Resources["controlColor"]    = Color.FromHex("#424242");
						Application.Current.Resources["infoColor"]       = Color.FromHex("#78909c");
						device.SetLightStatusBar();
						break;

					case Theme.Dark:
						Application.Current.Resources["backgroundColor"] = Color.FromHex("#2b3138");
						Application.Current.Resources["textColor"]       = Color.FromHex("#fafafa");
						Application.Current.Resources["controlColor"]    = Color.FromHex("#20252f");
						Application.Current.Resources["infoColor"]       = Color.FromHex("#90a4ae");
						device.SetLightStatusBar();
						break;
				}
			}
		}
	}
}