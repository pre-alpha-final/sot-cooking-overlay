using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	public static class AppContext
	{
		public static IntPtr hWnd { get; set; }
		public static DateTimeOffset Start { get; set; }

		public static int MegFull => 40;
		public static int MegRemaining { get; set; }
		public static string MegText => $"Meg: {MegRemaining}s";

		public static async Task Tick()
		{
			Start = DateTimeOffset.UtcNow;
			var lastElapsedSeconds = 0;
			while (true)
			{
				await Task.Delay(100);
				var elapsedSeconds = (int)(DateTimeOffset.UtcNow - Start).TotalSeconds;
				if (elapsedSeconds != lastElapsedSeconds)
				{
					lastElapsedSeconds = elapsedSeconds;
					MegRemaining = MegFull - elapsedSeconds;
					WinAPI.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase);
				}
			}
		}
	}
}
