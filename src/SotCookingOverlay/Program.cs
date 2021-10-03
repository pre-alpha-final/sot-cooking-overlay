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
		const UInt32 TransparentColor = 0; // 0 = black since CreateCompatibleBitmap would turn it to black anyway
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
					IntPtr compatibleDc = WinAPI.CreateCompatibleDC(hDc);
					IntPtr compatibleBitmap = WinAPI.CreateCompatibleBitmap(hDc, Width, Height);
					WinAPI.SelectObject(compatibleDc, compatibleBitmap);

					var textDrawer = new TextDrawer(BackgroundColor, ForegroundColor);
					textDrawer.DrawText(hWnd, compatibleDc, AppContext.MegText, 20, 20);

					/*
					 * Last param here should be TransparentColor not 0, but CreateCompatibleBitmap above
					 * does not support transparency, so the originally transparent regions become 0 (black)
					 */
					WinAPI.TransparentBlt(hDc, 0, 0, Width, Height, compatibleDc, 0, 0, Width, Height, 0);
					WinAPI.EndPaint(hWnd, ref ps);
					WinAPI.DeleteDC(compatibleDc);
					WinAPI.DeleteObject(compatibleBitmap);
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
