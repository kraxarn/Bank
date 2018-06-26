using Android.App;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Bank.Droid;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.Droid
{
	public class DeviceInfo : IDeviceInfo
	{
		private static Activity _activity;

		public static void SetContext(Activity activity) => _activity = activity;

		public string DeviceName => Settings.Secure.GetString(Application.Context.ContentResolver, "bluetooth_name");
		
		public string OsVersion  => $"Android {Build.VERSION.Sdk}";

		public bool KeepScreenOn
		{
			set
			{
				if (value)
					_activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
				else
					_activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
			}
		}

		public void DisplayToast(string title, string content) 
			=> Toast.MakeText(Application.Context, content, ToastLength.Short).Show();


		/*
		 * TODO
		 * Either we use the themes we manually define
		 * or we use the default Android dark theme
		 */
		public void SetDarkStatusBar()  { }

		public void SetLightStatusBar() { }
	}
}