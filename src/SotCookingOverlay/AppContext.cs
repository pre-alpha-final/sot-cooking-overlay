using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	public static class AppContext
	{
		public static string Title { get; set; } = "SoT Cooking Overlay";
		public static string TargetTitle { get; set; } = "new 1 - Notepad++";
		public static int Width { get; set; } = 400;
		public static int Height { get; set; } = 200;
		public static uint TransparentColor { get; set; } = 1234;
		public static int TextBackgroundColor { get; set; } = 12345;
		public static int TextForegroundColor { get; set; } = 54321;

		public static IntPtr WindowHandle { get; set; }
		public static bool IsActive { get; set; }
		public static DateTimeOffset StartTime { get; set; }

		public static int MegFull => 40;
		public static int MegRemaining { get; set; }
		public static string MegText => IsActive ? $"Meg: {MegRemaining}s" : string.Empty;

		public static async Task Tick()
		{
			StartTime = DateTimeOffset.UtcNow;
			while (true)
			{
				Reposition();
				UpdateContent();
				await Task.Delay(10);
			}
		}

		private static void Reposition()
		{
			var targetWindow = WinApiInterop.FindWindow(null, TargetTitle);
			if (targetWindow != IntPtr.Zero)
			{
				var foregroundWindow = WinApiInterop.GetForegroundWindow();
				IsActive = targetWindow == foregroundWindow;
				if (targetWindow == foregroundWindow)
				{
					WinApiInterop.GetWindowRect(targetWindow, out var rect);
					WinApiInterop.SetWindowPos(WindowHandle, (IntPtr)WinApiInterop.HWND_TOPMOST, rect.Right - Width - 100,
						rect.Top + 100, Width, Height, SetWindowPosFlags.ShowWindow);
				}
			}
		}

		private static void UpdateContent()
		{
			var elapsedSeconds = (int)(DateTimeOffset.UtcNow - StartTime).TotalSeconds;
			MegRemaining = MegFull - elapsedSeconds;
			WinApiInterop.RedrawWindow(WindowHandle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase);
		}
	}
}
