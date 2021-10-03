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
			while (true)
			{
				await Task.Delay(100);
				MegRemaining = MegFull - (int)(DateTimeOffset.UtcNow - Start).TotalSeconds;

				WinAPI.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase);
			}
		}
	}
}
