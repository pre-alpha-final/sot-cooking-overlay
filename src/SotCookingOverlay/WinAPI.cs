using System;
using System.Runtime.InteropServices;

/*
 * Reference https://www.pinvoke.net
 */
namespace SotCookingOverlay
{
	//typedef unsigned short ATOM;

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

	[StructLayout(LayoutKind.Sequential)]
	public struct MSG
	{
		IntPtr hwnd;
		uint message;
		UIntPtr wParam;
		IntPtr lParam;
		int time;
		POINT pt;
		int lPrivate;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static implicit operator System.Drawing.Point(POINT p)
		{
			return new System.Drawing.Point(p.X, p.Y);
		}

		public static implicit operator POINT(System.Drawing.Point p)
		{
			return new POINT(p.X, p.Y);
		}
	}

	public class WinAPI
	{
		public const UInt32 WS_EX_TOPMOST = 0x00000008;
		public const UInt32 WS_EX_TRANSPARENT = 0x00000020;
		public const UInt32 WS_EX_LAYERED = 0x80000;
		public const UInt32 WS_POPUP = 0x80000000;
		public const UInt32 WS_VISIBLE = 0x10000000;
		public const UInt32 CS_USEDEFAULT = 0x80000000;
		public const UInt32 CS_DBLCLKS = 8;
		public const UInt32 CS_VREDRAW = 1;
		public const UInt32 CS_HREDRAW = 2;
		public const UInt32 COLOR_WINDOW = 5;
		public const UInt32 COLOR_BACKGROUND = 1;
		public const UInt32 LWA_COLORKEY = 1;
		public const UInt32 IDC_ARROW = 32512;
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
			UInt16 lpClassName, // replacing "string lpClassName" to use the ATOM variant
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

		[DllImport("user32.dll")]
		public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("user32.dll")]
		public static extern bool TranslateMessage([In] ref MSG lpMsg);

		[DllImport("user32.dll")]
		public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

		[DllImport("user32.dll")]
		public static extern void PostQuitMessage(int nExitCode);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateSolidBrush(uint crColor);

		[DllImport("user32.dll")]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
	}
}
