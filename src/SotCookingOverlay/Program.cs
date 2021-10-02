using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SotCookingOverlay
{
	class Program
	{
		static async Task Main(string[] args)
		{
			WndProc delegWndProc = myWndProc;

			WNDCLASSEX windowClassEx = new WNDCLASSEX();
			windowClassEx.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
			windowClassEx.style = (int)(WinAPI.CS_HREDRAW | WinAPI.CS_VREDRAW | WinAPI.CS_DBLCLKS); //Doubleclicks are active
			windowClassEx.hbrBackground = (IntPtr)WinAPI.COLOR_BACKGROUND + 1; //Black background, +1 is necessary
			windowClassEx.cbClsExtra = 0;
			windowClassEx.cbWndExtra = 0;
			windowClassEx.hInstance = Process.GetCurrentProcess().Handle;
			windowClassEx.hIcon = IntPtr.Zero;
			windowClassEx.hCursor = WinAPI.LoadCursor(IntPtr.Zero, (int)WinAPI.IDC_CROSS);// Crosshair cursor;
			windowClassEx.lpszMenuName = null;
			windowClassEx.lpszClassName = "myClass";
			windowClassEx.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
			windowClassEx.hIconSm = IntPtr.Zero;
			ushort registerClassEx = WinAPI.RegisterClassEx(ref windowClassEx);

			IntPtr hwnd = WinAPI.CreateWindowExModified(0, registerClassEx, "Hello Win32", WinAPI.WS_OVERLAPPEDWINDOW | WinAPI.WS_VISIBLE, 0, 0, 300, 400, IntPtr.Zero, IntPtr.Zero, windowClassEx.hInstance, IntPtr.Zero);
			WinAPI.ShowWindow(hwnd, 1);
			WinAPI.UpdateWindow(hwnd);

			await Task.Delay(3000);
		}

		private static IntPtr myWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WinAPI.WM_PAINT:
					break;

				default:
					break;
			}
			return WinAPI.DefWindowProc(hWnd, msg, wParam, lParam);
		}
	}
}
