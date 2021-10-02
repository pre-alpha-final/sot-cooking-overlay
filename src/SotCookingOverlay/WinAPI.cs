using System;
using System.Runtime.InteropServices;

/*
 * Reference https://www.pinvoke.net
 */
namespace SotCookingOverlay
{
	//typedef unsigned short ATOM;

	delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	#region structs
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

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left, Top, Right, Bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

		public int X
		{
			get { return Left; }
			set { Right -= (Left - value); Left = value; }
		}

		public int Y
		{
			get { return Top; }
			set { Bottom -= (Top - value); Top = value; }
		}

		public int Height
		{
			get { return Bottom - Top; }
			set { Bottom = value + Top; }
		}

		public int Width
		{
			get { return Right - Left; }
			set { Right = value + Left; }
		}

		public System.Drawing.Point Location
		{
			get { return new System.Drawing.Point(Left, Top); }
			set { X = value.X; Y = value.Y; }
		}

		public System.Drawing.Size Size
		{
			get { return new System.Drawing.Size(Width, Height); }
			set { Width = value.Width; Height = value.Height; }
		}

		public static implicit operator System.Drawing.Rectangle(RECT r)
		{
			return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		public static implicit operator RECT(System.Drawing.Rectangle r)
		{
			return new RECT(r);
		}

		public static bool operator ==(RECT r1, RECT r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(RECT r1, RECT r2)
		{
			return !r1.Equals(r2);
		}

		public bool Equals(RECT r)
		{
			return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
		}

		public override bool Equals(object obj)
		{
			if (obj is RECT)
				return Equals((RECT)obj);
			else if (obj is System.Drawing.Rectangle)
				return Equals(new RECT((System.Drawing.Rectangle)obj));
			return false;
		}

		public override int GetHashCode()
		{
			return ((System.Drawing.Rectangle)this).GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PAINTSTRUCT
	{
		public IntPtr hdc;
		public bool fErase;
		public RECT rcPaint;
		public bool fRestore;
		public bool fIncUpdate;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class LOGFONT
	{
		public int lfHeight = 0;
		public int lfWidth = 0;
		public int lfEscapement = 0;
		public int lfOrientation = 0;
		public int lfWeight = 0;
		public byte lfItalic = 0;
		public byte lfUnderline = 0;
		public byte lfStrikeOut = 0;
		public byte lfCharSet = 0;
		public byte lfOutPrecision = 0;
		public byte lfClipPrecision = 0;
		public byte lfQuality = 0;
		public byte lfPitchAndFamily = 0;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string lfFaceName = string.Empty;
	}
	#endregion

	public class WinAPI
	{
		#region consts
		public const UInt32 WS_EX_TOPMOST = 0x00000008;
		public const UInt32 WS_EX_TRANSPARENT = 0x00000020;
		public const UInt32 WS_EX_LAYERED = 0x80000;
		public const UInt32 WS_POPUP = 0x80000000;
		public const UInt32 CS_VREDRAW = 1;
		public const UInt32 CS_HREDRAW = 2;
		public const UInt32 LWA_COLORKEY = 1;
		public const Int32 SW_NORMAL = 1;
		public const Int32 TRANSPARENT = 1;
		public const Int32 DT_SINGLELINE = 0x00000020;
		public const Int32 DT_NOCLIP = 0x00000100;
		public const UInt32 IDC_ARROW = 32512;
		public const UInt32 WM_DESTROY = 2;
		public const UInt32 WM_PAINT = 0x0f;
		#endregion

		#region imports
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

		[DllImport("user32.dll")]
		public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("gdi32.dll")]
		public static extern uint SetTextColor(IntPtr hdc, int crColor);

		[DllImport("gdi32.dll")]
		public static extern int SetBkMode(IntPtr hdc, int iBkMode);

		[DllImport("user32.dll")]
		public static extern int DrawText(IntPtr hdc, string lpchText, int cchText,
			ref RECT lprc, uint dwDTFormat);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

		[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);
		#endregion
	}
}
