﻿using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bank
{
	internal abstract class Tools
	{
		public static string Version
		{
			get
			{
				var ver = "v1.0.0-beta.4";
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
	}
}