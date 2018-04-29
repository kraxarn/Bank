using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.UWP
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => Environment.MachineName;
		public string OSVersion  => $"{Environment.OSVersion}";
	}
}
