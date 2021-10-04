using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	class Program
	{
		static void Main(string[] args)
		{
			var windowProcedureWrapper = new WindowProcedureWrapper(OnPaint);
			(WNDCLASSEX windowClassEx, ushort registerClassAtom) = WinApiHelper.RegisterWindowClass(windowProcedureWrapper.WindowProcedure);
			WinApiHelper.CreateOwnerWindow(windowClassEx, registerClassAtom);
			var hWnd = WinApiHelper.CreatePopupWindow(windowClassEx, registerClassAtom);

			AppContext.WindowHandle = hWnd;
			var _ = Task.Run(AppContext.Tick);

			WinApiHelper.MessageLoop();
		}

		private static void OnPaint(IntPtr hWnd, IntPtr hDc)
		{
			var height = 50;
			var textDrawer = new TextDrawer(AppContext.TextBackgroundColor, AppContext.TextForegroundColor);
			textDrawer.DrawText(hWnd, hDc, AppContext.FishText, 0, height * 0);
			textDrawer.DrawText(hWnd, hDc, AppContext.TrophyFishText, 0, height * 1);
			textDrawer.DrawText(hWnd, hDc, AppContext.MeatText, 0, height * 2);
			textDrawer.DrawText(hWnd, hDc, AppContext.KrakenMegText, 0, height * 3);
		}
	}
}
