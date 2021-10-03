using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SotCookingOverlay
{
	public static class WinApiHelper
	{
		public static (WNDCLASSEX windowClassEx, UInt16 registerClassAtom) RegisterWindowClass(WndProc windowProcedure)
		{
			var windowClassEx = new WNDCLASSEX
			{
				cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
				style = (int)(WinApiInterop.CS_HREDRAW | WinApiInterop.CS_VREDRAW),
				hbrBackground = WinApiInterop.CreateSolidBrush(AppContext.TransparentColor),
				cbClsExtra = 0,
				cbWndExtra = 0,
				hInstance = Process.GetCurrentProcess().Handle,
				hIcon = IntPtr.Zero,
				hCursor = WinApiInterop.LoadCursor(IntPtr.Zero, (int)WinApiInterop.IDC_ARROW),
				lpszMenuName = "lpszMenuName",
				lpszClassName = "lpszClassName",
				lpfnWndProc = Marshal.GetFunctionPointerForDelegate(windowProcedure),
				hIconSm = IntPtr.Zero
			};
			ushort registerClassAtom = WinApiInterop.RegisterClassEx(ref windowClassEx);

			return (windowClassEx, registerClassAtom);
		}

		public static IntPtr CreateOwnerWindow(WNDCLASSEX windowClassEx, ushort registerClassAtom)
		{
			var hWnd = WinApiInterop.CreateWindowEx((int)(WinApiInterop.WS_EX_TOPMOST | WinApiInterop.WS_EX_TRANSPARENT | WinApiInterop.WS_EX_LAYERED),
				registerClassAtom, AppContext.Title, WinApiInterop.WS_POPUP | WinApiInterop.WS_SYSMENU | WinApiInterop.WS_CAPTION, 0, 0, 0, 0, IntPtr.Zero,
				IntPtr.Zero, windowClassEx.hInstance, IntPtr.Zero);
			WinApiInterop.SetLayeredWindowAttributes(hWnd, AppContext.TransparentColor, 0, WinApiInterop.LWA_COLORKEY);
			WinApiInterop.ShowWindow(hWnd, WinApiInterop.SW_NORMAL);

			return hWnd;
		}

		public static IntPtr CreatePopupWindow(WNDCLASSEX windowClassEx, ushort registerClassAtom)
		{
			IntPtr hWnd = WinApiInterop.CreateWindowEx((int)(WinApiInterop.WS_EX_TOPMOST | WinApiInterop.WS_EX_TRANSPARENT | WinApiInterop.WS_EX_LAYERED), registerClassAtom,
				string.Empty, WinApiInterop.WS_POPUP, 0, 0, AppContext.Width, AppContext.Height, IntPtr.Zero, IntPtr.Zero, windowClassEx.hInstance, IntPtr.Zero);
			WinApiInterop.SetLayeredWindowAttributes(hWnd, AppContext.TransparentColor, 0, WinApiInterop.LWA_COLORKEY);
			WinApiInterop.ShowWindow(hWnd, WinApiInterop.SW_NORMAL);

			return hWnd;
		}

		public static void MessageLoop()
		{
			while (WinApiInterop.GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
			{
				WinApiInterop.TranslateMessage(ref msg);
				WinApiInterop.DispatchMessage(ref msg);
			}
		}
	}

	public class WindowProcedureWrapper
	{
		private readonly Action<IntPtr, IntPtr> _onPaintAction;

		public WndProc WindowProcedure => InnerWindowProcedure;

		public WindowProcedureWrapper(Action<IntPtr, IntPtr> onPaintAction)
		{
			_onPaintAction = onPaintAction;
		}

		private IntPtr InnerWindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WinApiInterop.WM_PAINT:
					var hDc = WinApiInterop.BeginPaint(hWnd, out PAINTSTRUCT ps);
					var hCompatibleDc = WinApiInterop.CreateCompatibleDC(hDc);
					var hCompatibleBitmap = WinApiInterop.CreateCompatibleBitmap(hDc, AppContext.Width, AppContext.Height);
					var hOldBitmap = WinApiInterop.SelectObject(hCompatibleDc, hCompatibleBitmap);
					var hBrush = WinApiInterop.CreateSolidBrush(AppContext.TransparentColor);
					var rect = new RECT(0, 0, AppContext.Width, AppContext.Height);
					WinApiInterop.FillRect(hCompatibleDc, ref rect, hBrush);
					WinApiInterop.TransparentBlt(hCompatibleDc, 0, 0, AppContext.Width, AppContext.Height, hCompatibleDc, 0, 0,
						AppContext.Width, AppContext.Height, AppContext.TransparentColor);

					_onPaintAction.Invoke(hWnd, hCompatibleDc);

					WinApiInterop.BitBlt(hDc, 0, 0, AppContext.Width, AppContext.Height, hCompatibleDc, 0, 0, TernaryRasterOperations.SRCCOPY);
					WinApiInterop.EndPaint(hWnd, ref ps);
					WinApiInterop.DeleteDC(hCompatibleDc);
					WinApiInterop.DeleteObject(hCompatibleBitmap);
					WinApiInterop.DeleteObject(hOldBitmap);
					WinApiInterop.DeleteObject(hBrush);
					break;

				case WinApiInterop.WM_ERASEBKGND:
					return IntPtr.Zero;

				case WinApiInterop.WM_DESTROY:
					WinApiInterop.PostQuitMessage(0);
					break;

				default:
					break;
			}

			return WinApiInterop.DefWindowProc(hWnd, msg, wParam, lParam);
		}
	}
}
