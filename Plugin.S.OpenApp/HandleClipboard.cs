using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Plugin.S.OpenApp;

public class HandleClipboard
{
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	public static void Paste(string k)
	{
		string text = Encoding.UTF8.GetString(Convert.FromBase64String(k));
		for (int i = 0; i < text.Length; i++)
		{
			int num = Strings.AscW(text[i].ToString());
			if (num == 8 || num == 13)
			{
				PostMessage(HandelMouse.ActiveHandel, 256u, (IntPtr)Conversions.ToInteger("&H" + Conversion.Hex(Strings.AscW(text[i].ToString()))), CreateLParamFor_WM_KEYDOWN(1, 30, IsExtendedKey: false, DownBefore: false));
				PostMessage(HandelMouse.ActiveHandel, 257u, (IntPtr)Conversions.ToInteger("&H" + Conversion.Hex(Strings.AscW(text[i].ToString()))), CreateLParamFor_WM_KEYUP(1, 30, IsExtendedKey: false));
			}
			else
			{
				PostMessage(HandelMouse.ActiveHandel, 258u, (IntPtr)Strings.AscW(text[i].ToString()), (IntPtr)1);
			}
		}
	}

	public static IntPtr CreateLParamFor_WM_KEYUP(ushort RepeatCount, byte ScanCode, bool IsExtendedKey)
	{
		return KeysLParam(RepeatCount, ScanCode, IsExtendedKey, DownBefore: true, State: true);
	}

	public static IntPtr CreateLParamFor_WM_KEYDOWN(ushort RepeatCount, byte ScanCode, bool IsExtendedKey, bool DownBefore)
	{
		return KeysLParam(RepeatCount, ScanCode, IsExtendedKey, DownBefore, State: false);
	}

	public static IntPtr KeysLParam(ushort RepeatCount, byte ScanCode, bool IsExtendedKey, bool DownBefore, bool State)
	{
		int num = RepeatCount | (ushort)(ScanCode << 16);
		if (IsExtendedKey)
		{
			num |= 0x10000;
		}
		if (DownBefore)
		{
			num |= 0x40000000;
		}
		if (State)
		{
			num = (int)((long)num | -2147483648L);
		}
		return new IntPtr(num);
	}
}
