using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Bank.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.UWP
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => Environment.MachineName;

		public string OsVersion  => $"{Environment.OSVersion}";

		public void SetDarkStatusBar()  { }

		public void SetLightStatusBar() { }

		public bool KeepScreenOn { get; set; }

		public void DisplayToast(string title, string content)
		{
			var notifier = ToastNotificationManager.CreateToastNotifier();
			var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
			var toastNodes = toastXml.GetElementsByTagName("text");

			toastNodes.Item(0)?.AppendChild(toastXml.CreateTextNode(title));
			toastNodes.Item(1)?.AppendChild(toastXml.CreateTextNode(content));

			var toast = new ToastNotification(toastXml);

			Task.Run(() =>
			{
				notifier.Show(toast);
				Thread.Sleep(2000);
				notifier.Hide(toast);
			});
		}
	}
}
