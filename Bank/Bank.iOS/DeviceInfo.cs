using Bank.iOS;
using Foundation;
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

		public void DisplayToast(string title, string content)
		{
			NSTimer alertDelay      = null;
			UIAlertController alert = null;

			alertDelay = NSTimer.CreateScheduledTimer(2, action =>
			{
				alert?.DismissViewController(true, null);
				alertDelay?.Dispose();
			});

			alert = UIAlertController.Create(title, content, UIAlertControllerStyle.Alert);

			var rootController = ((AppDelegate) UIApplication.SharedApplication.Delegate).Window.RootViewController.PresentedViewController;
			
			if (rootController is UINavigationController navcontroller)
				rootController = navcontroller.VisibleViewController;

			rootController.PresentViewController(alert, true, null);
		}
	}
}