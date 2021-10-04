using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	public static class AppContext
	{
		public static string Title { get; set; } = "SoT Cooking Overlay";
		public static string TargetTitle { get; set; } = "Sea of Thieves";
		public static int Width { get; set; } = 400;
		public static int Height { get; set; } = 225;
		public static uint TransparentColor { get; set; } = 1234;
		public static int TextBackgroundColor { get; set; } = 12345;
		public static int TextForegroundColor { get; set; } = 54321;

		public static IntPtr WindowHandle { get; set; }
		public static bool IsForeground { get; set; }
		public static bool IsActive { get; set; }
		public static DateTimeOffset StartTime { get; set; }

		public static int FishFull => 45;
		public static int FishRemaining { get; set; }
		public static string FishText => IsForeground && IsActive ? $"Fish: {FishRemaining}s" : string.Empty;

		public static int TrophyFishFull => 95;
		public static int TrophyFishRemaining { get; set; }
		public static string TrophyFishText => IsForeground && IsActive ? $"Trophy fish: {TrophyFishRemaining}s" : string.Empty;

		public static int MeatFull => 65;
		public static int MeatRemaining { get; set; }
		public static string MeatText => IsForeground && IsActive ? $"Meat: {MeatRemaining}s" : string.Empty;

		public static int KrakenMegFull => 125;
		public static int KrakenMegRemaining { get; set; }
		public static string KrakenMegText => IsForeground && IsActive ? $"Krak and Meg: {KrakenMegRemaining}s" : string.Empty;

		public static async Task Tick()
		{
			while (true)
			{
				CheckKey();
				Reposition();
				UpdateContent();
				await Task.Delay(10);
			}
		}

		private static void CheckKey()
		{
			if (IsForeground == false || DateTimeOffset.UtcNow - StartTime < TimeSpan.FromMilliseconds(250))
			{
				return;
			}

			if ((WinApiInterop.GetKeyState(VirtualKeyStates.VK_NUMPAD0) & 0x010000) != 0)
			{
				StartTime = DateTimeOffset.UtcNow;
				IsActive = !IsActive;
			}
		}

		private static void Reposition()
		{
			var targetWindow = WinApiInterop.FindWindow(null, TargetTitle);
			if (targetWindow != IntPtr.Zero)
			{
				var foregroundWindow = WinApiInterop.GetForegroundWindow();
				IsForeground = targetWindow == foregroundWindow;
				if (targetWindow == foregroundWindow)
				{
					WinApiInterop.GetWindowRect(targetWindow, out var rect);
					WinApiInterop.SetWindowPos(WindowHandle, (IntPtr)WinApiInterop.HWND_TOPMOST, rect.Right - Width - 10,
						rect.Top + 10, Width, Height, SetWindowPosFlags.ShowWindow);
				}
			}
		}

		private static void UpdateContent()
		{
			var elapsedSeconds = (int)(DateTimeOffset.UtcNow - StartTime).TotalSeconds;
			FishRemaining = FishFull - elapsedSeconds;
			TrophyFishRemaining = TrophyFishFull - elapsedSeconds;
			MeatRemaining = MeatFull - elapsedSeconds;
			KrakenMegRemaining = KrakenMegFull - elapsedSeconds;
			WinApiInterop.RedrawWindow(WindowHandle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase);
		}
	}
}
