using System;
using System.Threading.Tasks;

namespace SotCookingOverlay
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var windowProcedureWrapper = new WindowProcedureWrapper(OnPaint);
			(WNDCLASSEX windowClassEx, ushort registerClassAtom) = WinApiHelper.RegisterWindowClass(windowProcedureWrapper.WindowProcedure);
			var hWnd = WinApiHelper.CreatePopupWindow(windowClassEx, registerClassAtom);

			AppContext.hWnd = hWnd;
			var _ = Task.Run(AppContext.Tick);

			WinApiHelper.MessageLoop();
		}

		private static void OnPaint(IntPtr hWnd, IntPtr hDc)
		{
			TextDrawer textDrawer = new TextDrawer(AppContext.TextBackgroundColor, AppContext.TextForegroundColor);
			textDrawer.DrawText(hWnd, hDc, AppContext.MegText, 20, 20);
		}
	}
}
