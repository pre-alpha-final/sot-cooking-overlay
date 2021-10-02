using System;
using System.Runtime.InteropServices;

namespace SotCookingOverlay
{
	delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WNDCLASSEX
	{
		[MarshalAs(UnmanagedType.U4)]
		public int cbSize;
		[MarshalAs(UnmanagedType.U4)]
		public int style;
		public IntPtr lpfnWndProc;
		public int cbClsExtra;
		public int cbWndExtra;
		public IntPtr hInstance;
		public IntPtr hIcon;
		public IntPtr hCursor;
		public IntPtr hbrBackground;
		public string lpszMenuName;
		public string lpszClassName;
		public IntPtr hIconSm;
	}

	public class WinAPI
	{
		public const UInt32 WS_OVERLAPPEDWINDOW = 0xcf0000;
		public const UInt32 WS_VISIBLE = 0x10000000;
		public const UInt32 CS_USEDEFAULT = 0x80000000;
		public const UInt32 CS_DBLCLKS = 8;
		public const UInt32 CS_VREDRAW = 1;
		public const UInt32 CS_HREDRAW = 2;
		public const UInt32 COLOR_WINDOW = 5;
		public const UInt32 COLOR_BACKGROUND = 1;
		public const UInt32 IDC_CROSS = 32515;
		public const UInt32 WM_DESTROY = 2;
		public const UInt32 WM_PAINT = 0x0f;
		public const UInt32 WM_LBUTTONUP = 0x0202;
		public const UInt32 WM_LBUTTONDBLCLK = 0x0203;

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
		public static extern UInt16 RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
		public static extern IntPtr CreateWindowEx(
			int dwExStyle,
			string lpClassName,
			string lpWindowName,
			UInt32 dwStyle,
			int x,
			int y,
			int nWidth,
			int nHeight,
			IntPtr hWndParent,
			IntPtr hMenu,
			IntPtr hInstance,
			IntPtr lpParam);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
		public static extern IntPtr CreateWindowExModified(
			int dwExStyle,
			UInt16 regResult,
			//string lpClassName,
			string lpWindowName,
			UInt32 dwStyle,
			int x,
			int y,
			int nWidth,
			int nHeight,
			IntPtr hWndParent,
			IntPtr hMenu,
			IntPtr hInstance,
			IntPtr lpParam);

		[DllImport("user32.dll")]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool UpdateWindow(IntPtr hWnd);
	}
}
