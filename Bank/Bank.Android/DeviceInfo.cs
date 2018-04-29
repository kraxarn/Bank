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
		public string OSVersion  => $"Android {Build.VERSION.Sdk}";
	}
}