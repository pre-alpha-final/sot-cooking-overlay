using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SotCookingOverlay
{
	class Program
	{
		const Int32 Width = 200;
		const Int32 Height = 100;
		const UInt32 TransparentColor = 12345678;
		const Int32 ForegroundColor = 54321;
		const Int32 BackgroundColor = 12345;

		static async Task Main(string[] args)
		{
			var windowClassEx = new WNDCLASSEX
			{
				cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
				style = (int)(WinAPI.CS_HREDRAW | WinAPI.CS_VREDRAW),
				hbrBackground = WinAPI.CreateSolidBrush(TransparentColor),
				cbClsExtra = 0,
				cbWndExtra = 0,
				hInstance = Process.GetCurrentProcess().Handle,
				hIcon = IntPtr.Zero,
				hCursor = WinAPI.LoadCursor(IntPtr.Zero, (int)WinAPI.IDC_ARROW),
				lpszMenuName = "lpszMenuName",
				lpszClassName = "lpszClassName",
				lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProc)WindowProcedure),
				hIconSm = IntPtr.Zero
			};
			UInt16 registerClassEx = WinAPI.RegisterClassEx(ref windowClassEx);

			IntPtr hWnd = WinAPI.CreateWindowEx((int)(WinAPI.WS_EX_TOPMOST | WinAPI.WS_EX_TRANSPARENT | WinAPI.WS_EX_LAYERED), registerClassEx,
				string.Empty, WinAPI.WS_POPUP, 0, 0, Width, Height, IntPtr.Zero, IntPtr.Zero, windowClassEx.hInstance, IntPtr.Zero);
			WinAPI.SetLayeredWindowAttributes(hWnd, TransparentColor, 0, WinAPI.LWA_COLORKEY);
			WinAPI.ShowWindow(hWnd, WinAPI.SW_NORMAL);
			//WinAPI.UpdateWindow(hWnd);

			while (WinAPI.GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
			{
				WinAPI.TranslateMessage(ref msg);
				WinAPI.DispatchMessage(ref msg);
			}
		}

		private static IntPtr WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WinAPI.WM_PAINT:
					// Bitmap refernce
					//PAINTSTRUCT ps;
					//HDC kon, pom;
					//kon = BeginPaint(hwnd, &ps);
					//pom = CreateCompatibleDC(kon);
					//SelectObject(pom, this->bmp);
					//BitBlt(kon, 0, 0, this->width, this->height, pom, 0, 0, SRCCOPY);
					//DeleteDC(pom);
					//EndPaint(hwnd, &ps);

					IntPtr hDc = WinAPI.BeginPaint(hWnd, out var ps);
					var textDrawer = new TextDrawer(BackgroundColor, ForegroundColor);
					textDrawer.DrawText(hWnd, hDc, "Foo Bar", 20, 20);
					WinAPI.EndPaint(hWnd, ref ps);

					break;

				case WinAPI.WM_DESTROY:
					WinAPI.PostQuitMessage(0);
					break;

				default:
					break;
			}
			return WinAPI.DefWindowProc(hWnd, msg, wParam, lParam);
		}
	}
}
