using System;
using Microsoft.Maui.Platform;

namespace Microsoft.Maui.Handlers
{
	public partial class ApplicationHandler : ElementHandler<IApplication, Gtk.Application>
	{
		public static void MapTerminate(ApplicationHandler handler, IApplication application, object? args)
		{
			Environment.Exit(0);
		}

		public static void MapOpenWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			handler.PlatformView?.CreatePlatformWindow(application);
		}

		public static void MapCloseWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			//if (args is IWindow window)
			//{
			//	if (window.Handler?.PlatformView is Activity activity)
			//		activity.Finish();
			//}
		}
	}
}