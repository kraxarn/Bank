﻿using System;
using Bank.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace Bank.UWP
{
	public class DeviceInfo : IDeviceInfo
	{
		public string DeviceName => Environment.MachineName;

		public string OsVersion  => $"{Environment.OSVersion}";
	}
}
