namespace Bank
{
	public interface IDeviceInfo
	{
		string DeviceName { get; }

		string OsVersion  { get; }
	}
}