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

			AppContext.hWnd = hWnd;
			var _ = Task.Run(AppContext.Tick);

			WinApiHelper.MessageLoop();
		}

		private static void OnPaint(IntPtr hWnd, IntPtr hDc)
		{
			var textDrawer = new TextDrawer(AppContext.TextBackgroundColor, AppContext.TextForegroundColor);
			textDrawer.DrawText(hWnd, hDc, AppContext.MegText, 20, 20);
		}
	}
}
