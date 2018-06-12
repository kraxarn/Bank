using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;

namespace Bank
{
	internal abstract class Tools
	{
		public static string Version => "v1.0.0-beta.1";

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
    }
}
