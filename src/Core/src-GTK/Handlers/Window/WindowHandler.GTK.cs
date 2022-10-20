using System;

namespace Microsoft.Maui.Handlers
{
	public partial class WindowHandler : ElementHandler<IWindow, Gtk.Window>
	{
		public static void MapTitle(IWindowHandler handler, IWindow window) { }

		public static void MapContent(IWindowHandler handler, IWindow window)
		{
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			_ = CreateRootViewFromContent(handler, window);
			// handler.PlatformView.SetContentView(rootView);
			// handler.PlatformView = rootView;
			//if (window.VisualDiagnosticsOverlay != null && rootView is ViewGroup group)
			//	window.VisualDiagnosticsOverlay.Initialize();
		}

		public static void MapToolbar(IWindowHandler handler, IWindow view)
		{
			if (view is IToolbarElement tb)
				ViewHandler.MapToolbar(handler, tb);
		}

		public static void MapRequestDisplayDensity(IWindowHandler handler, IWindow window, object? args)
		{
			//if (args is DisplayDensityRequest request)
			//	request.SetResult(handler.PlatformView.GetDisplayDensity());
		}

		internal static Gtk.Window? CreateRootViewFromContent(IWindowHandler handler, IWindow window)
		{
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");
			var rootManager = handler.MauiContext.GetNavigationRootManager();
			rootManager.Connect(window.Content);
			return rootManager.RootView;
			
			//var winuiWindow = new Gtk.Fixed();

			//foreach(var child in window.all)

			//return winuiWindow;
		}
	}
}