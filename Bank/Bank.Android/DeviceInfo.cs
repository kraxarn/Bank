using Android.App;
using Android.OS;
using Android.Provider;
using Bank.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.Droid
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => Settings.Secure.GetString(Application.Context.ContentResolver, "bluetooth_name");
		
		public string OsVersion  => $"Android {Build.VERSION.Sdk}";

		
		/*
		 * TODO
		 * Either we use the themes we manually define
		 * or we use the default Android dark theme
		 */
		public void SetDarkStatusBar()  { }

		public void SetLightStatusBar() { }
	}
}