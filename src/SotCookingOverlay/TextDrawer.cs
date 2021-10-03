using System;

namespace SotCookingOverlay
{
	public class TextDrawer
	{
		public int BackgroundColor { get; set; }
		public int ForegroundColor { get; set; }

		public TextDrawer(int backgroundColor, int foregroundColor)
		{
			BackgroundColor = backgroundColor;
			ForegroundColor = foregroundColor;
		}

		public void DrawText(IntPtr hWnd, IntPtr hDc, string text, int x, int y)
		{
			SwapFont(hDc, BackgroundColor);
			InnerDrawText(hWnd, hDc, text, x + 3, y + 3);
			SwapFont(hDc, ForegroundColor);
			InnerDrawText(hWnd, hDc, text, x, y);
		}

		private void InnerDrawText(IntPtr hWnd, IntPtr hDc, string text, int x, int y)
		{
			WinApiInterop.GetClientRect(hWnd, out var rect);
			rect.Left = x;
			rect.Top = y;
			WinApiInterop.DrawText(hDc, text, -1, ref rect, WinApiInterop.DT_SINGLELINE | WinApiInterop.DT_NOCLIP);
		}

		private void SwapFont(IntPtr hDc, int color)
		{
			LOGFONT logfont = new LOGFONT
			{
				lfFaceName = "Comic Sans MS",
				lfHeight = 60,
				lfWeight = 500,
				lfQuality = 3,
			};
			IntPtr hNewFont = WinApiInterop.CreateFontIndirect(logfont);
			IntPtr hOldFont = WinApiInterop.SelectObject(hDc, hNewFont);
			WinApiInterop.DeleteObject(hOldFont);
			WinApiInterop.SetTextColor(hDc, color);
			WinApiInterop.SetBkMode(hDc, WinApiInterop.TRANSPARENT);
		}
	}
}
