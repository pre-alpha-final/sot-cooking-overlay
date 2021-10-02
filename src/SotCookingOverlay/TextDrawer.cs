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

		private void SwapFont(IntPtr hDc, int color)
		{
			LOGFONT logfont = new LOGFONT
			{
				lfFaceName = "Comic Sans MS",
				lfHeight = 60,
				lfWeight = 500,
				lfQuality = 3,
			};
			IntPtr hNewFont = WinAPI.CreateFontIndirect(logfont);
			IntPtr hOldFont = WinAPI.SelectObject(hDc, hNewFont);
			WinAPI.DeleteObject(hOldFont);
			WinAPI.SetTextColor(hDc, color);
			WinAPI.SetBkMode(hDc, WinAPI.TRANSPARENT);
		}

		private void InnerDrawText(IntPtr hWnd, IntPtr hDc, string text, int x, int y)
		{
			WinAPI.GetClientRect(hWnd, out var rect);
			rect.Left = x;
			rect.Top = y;
			WinAPI.DrawText(hDc, text, -1, ref rect, WinAPI.DT_SINGLELINE | WinAPI.DT_NOCLIP);
		}
	}
}
