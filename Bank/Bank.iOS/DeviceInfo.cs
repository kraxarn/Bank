using Bank.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof (DeviceInfo))]
namespace Bank.iOS
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => UIDevice.CurrentDevice.Name;
	}
}