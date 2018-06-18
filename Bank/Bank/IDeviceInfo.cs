namespace Bank
{
	public interface IDeviceInfo
	{
		string DeviceName { get; }

		string OsVersion  { get; }

		void SetDarkStatusBar();

		void SetLightStatusBar();

		bool KeepScreenOn { set; }
	}
}