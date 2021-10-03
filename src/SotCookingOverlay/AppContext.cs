using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	public static class AppContext
	{
		public static string Title { get; set; } = "SoT Cooking Overlay";
		public static Int32 Width { get; set; } = 400;
		public static Int32 Height { get; set; } = 200;
		public static UInt32 TransparentColor { get; set; } = 1234;
		public static Int32 TextBackgroundColor { get; set; } = 12345;
		public static Int32 TextForegroundColor { get; set; } = 54321;

		public static IntPtr hWnd { get; set; }
		public static DateTimeOffset Start { get; set; }

		public static int MegFull => 40;
		public static int MegRemaining { get; set; }
		//public static string MegText => isactive terenary $"Meg: {MegRemaining}s";
		public static string MegText => $"Meg: {MegRemaining}s";

		public static async Task Tick()
		{
			Start = DateTimeOffset.UtcNow;
			var lastElapsedSeconds = 0;
			while (true)
			{
				await Task.Delay(100);
				//handle width height here
				var elapsedSeconds = (int)(DateTimeOffset.UtcNow - Start).TotalSeconds;
				if (elapsedSeconds != lastElapsedSeconds)
				{
					lastElapsedSeconds = elapsedSeconds;
					MegRemaining = MegFull - elapsedSeconds;
					WinApiInterop.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase);
				}
			}
		}
	}
}
