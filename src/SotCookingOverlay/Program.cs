using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SotCookingOverlay
{
	class Program
	{
		const Int32 Width = 400;
		const Int32 Height = 200;
		const UInt32 TransparentColor = 1234;
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

			AppContext.hWnd = hWnd;
			var _ = Task.Run(AppContext.Tick);

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
					IntPtr hDc = WinAPI.BeginPaint(hWnd, out var ps);
					IntPtr hCompatibleDc = WinAPI.CreateCompatibleDC(hDc);
					IntPtr hCompatibleBitmap = WinAPI.CreateCompatibleBitmap(hDc, Width, Height);
					IntPtr hOldBitmap = WinAPI.SelectObject(hCompatibleDc, hCompatibleBitmap);
					IntPtr hBrush = WinAPI.CreateSolidBrush(TransparentColor);
					var rect = new RECT(0, 0, Width, Height);
					WinAPI.FillRect(hCompatibleDc, ref rect, hBrush);
					WinAPI.TransparentBlt(hCompatibleDc, 0, 0, Width, Height, hCompatibleDc, 0, 0, Width, Height, TransparentColor);

					var textDrawer = new TextDrawer(BackgroundColor, ForegroundColor);
					textDrawer.DrawText(hWnd, hCompatibleDc, AppContext.MegText, 20, 20);

					WinAPI.BitBlt(hDc, 0, 0, Width, Height, hCompatibleDc, 0, 0, TernaryRasterOperations.SRCCOPY);
					WinAPI.EndPaint(hWnd, ref ps);
					WinAPI.DeleteDC(hCompatibleDc);
					WinAPI.DeleteObject(hCompatibleBitmap);
					WinAPI.DeleteObject(hOldBitmap);
					WinAPI.DeleteObject(hBrush);
					break;

				case WinAPI.WM_ERASEBKGND:
					return IntPtr.Zero;

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
