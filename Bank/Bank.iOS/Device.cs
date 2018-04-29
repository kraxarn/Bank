using Bank.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof (Device))]
namespace Bank.iOS
{
	public class Device : IDeviceInfo
	{
		public string GetDeviceName() => UIDevice.CurrentDevice.Name;
	}
}