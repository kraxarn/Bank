using Bank.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof (DeviceInfo))]
namespace Bank.iOS
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => UIDevice.CurrentDevice.Name;

		public string OsVersion => $"{UIDevice.CurrentDevice.SystemName} {UIDevice.CurrentDevice.SystemVersion}";

		public void SetLightStatusBar() 
			=> UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, true);

		public void SetDarkStatusBar() 
			=> UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, true);

		public bool KeepScreenOn
		{
			set => UIApplication.SharedApplication.IdleTimerDisabled = value;
		}
	}
}