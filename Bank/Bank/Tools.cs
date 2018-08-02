using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
				var ver = "v1.0.0-beta.11";
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

	    public static string Seperate(uint value)
	    {
		    var format = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
		    format.NumberGroupSeparator = ",";
			return value.ToString("#,0", format);
		}

		public static object GetProperty(string key, object fallback)
		{
			// At least for now, we do specific code for Android
			if (Device.RuntimePlatform == Device.Android)
				return DependencyService.Get<IProperties>().GetProperty(key, fallback);

			return Application.Current.Properties.ContainsKey(key) ? Application.Current.Properties[key] : fallback;
		}

		public static void SetProperty(string key, object value)
		{
			// Android specific again
			if (Device.RuntimePlatform == Device.Android)
				DependencyService.Get<IProperties>().SetProperty(key, value);
			else
				Application.Current.Properties[key] = value;
		}

		public static bool ContainsProperty(string key)
			=> Application.Current.Properties.ContainsKey(key);

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

		public static bool SendMessage(string address, string message, out Exception error)
		{
			// TODO: This probably also (randomly) throws timeouts
			try
			{
				using (var client = new TcpClient(address, 13000))
				{
					// Similar to Client.Send / Server.Send

					var data = Encoding.ASCII.GetBytes(message);
					var stream = client.GetStream();
					stream.Write(data, 0, data.Length);

					data = new byte[256];
					var bytes = stream.Read(data, 0, data.Length);
					var response = Encoding.ASCII.GetString(data, 0, bytes);

					if (response != "OK")
						Application.Current.MainPage.DisplayAlert("Invalid response", response, "Dismiss");

					stream.Close();
					client.Close();

					error = null;
					return true;
				}
			}
			catch (Exception e)
			{
				error = e;
				return false;
			}
		}

		public static object GetPropertyFromFile(string key)
		{
			using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (!appStorage.FileExists("PropertyStore.forms"))
					return null;

				using (var fileStream = appStorage.OpenFile("PropertyStorage.forms", FileMode.Open, FileAccess.Read))
				{
					using (var reader = XmlDictionaryReader.CreateBinaryReader(fileStream, XmlDictionaryReaderQuotas.Max))
					{
						if (fileStream.Length == 0L)
							return null;

						var dict = (Dictionary<string, object>) new DataContractSerializer(typeof(Dictionary<string, object>)).ReadObject(reader);

						return dict.ContainsKey(key) ? dict[key] : null;
					}
				}
			}
		}
	}
}