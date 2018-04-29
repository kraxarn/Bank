using Android.OS;
using Bank.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.Droid
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => Build.Model;
		public string OSVersion  => $"Android {Build.VERSION.Sdk}";
	}
}