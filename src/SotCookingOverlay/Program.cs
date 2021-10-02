﻿using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SotCookingOverlay
{
	class Program
	{
		static uint _transparentColor = 1234;

		static async Task Main(string[] args)
		{
			WndProc windowProcedureDelegate = WindowProcedure;

			WNDCLASSEX windowClassEx = new WNDCLASSEX
			{
				cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
				style = (int)(WinAPI.CS_HREDRAW | WinAPI.CS_VREDRAW),
				hbrBackground = WinAPI.CreateSolidBrush(_transparentColor),
				cbClsExtra = 0,
				cbWndExtra = 0,
				hInstance = Process.GetCurrentProcess().Handle,
				hIcon = IntPtr.Zero,
				hCursor = WinAPI.LoadCursor(IntPtr.Zero, (int)WinAPI.IDC_ARROW),
				lpszMenuName = "lpszMenuName",
				lpszClassName = "lpszClassName",
				lpfnWndProc = Marshal.GetFunctionPointerForDelegate(windowProcedureDelegate),
				hIconSm = IntPtr.Zero
			};
			UInt16 registerClassEx = WinAPI.RegisterClassEx(ref windowClassEx);

			IntPtr hwnd = WinAPI.CreateWindowEx((int)(WinAPI.WS_EX_TOPMOST | WinAPI.WS_EX_TRANSPARENT | WinAPI.WS_EX_LAYERED), registerClassEx,
				string.Empty, WinAPI.WS_POPUP, 0, 0, 300, 400, IntPtr.Zero, IntPtr.Zero, windowClassEx.hInstance, IntPtr.Zero);
			WinAPI.SetLayeredWindowAttributes(hwnd, _transparentColor, 0, WinAPI.LWA_COLORKEY);
			WinAPI.ShowWindow(hwnd, 1);
			WinAPI.UpdateWindow(hwnd);

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
