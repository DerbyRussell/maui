﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.ApplicationModel;

#if WINDOWS
#if __GTK__
using NativeApplication = Microsoft.Maui.MauiGTKApplication;
using NativeWindow = Gtk.Window;
#else
using NativeApplication = Microsoft.UI.Xaml.Application;
using NativeWindow = Microsoft.UI.Xaml.Window;
#endif
#elif __IOS__ || __MACCATALYST__
using NativeApplication = UIKit.IUIApplicationDelegate;
using NativeWindow = UIKit.UIWindow;
#elif __ANDROID__
using NativeApplication = Android.App.Application;
using NativeWindow = Android.App.Activity;
#elif TIZEN
using NativeApplication = Tizen.Applications.CoreApplication;
using NativeWindow =  ElmSharp.Window;
#else
using NativeApplication = System.Object;
using NativeWindow = System.Object;
#endif

namespace Microsoft.Maui
{
	internal static partial class MauiContextExtensions
	{
		public static IAnimationManager GetAnimationManager(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<IAnimationManager>();

		public static IDispatcher GetDispatcher(this IMauiContext mauiContext) =>
			mauiContext.Services.GetRequiredService<IDispatcher>();

		public static IDispatcher? GetOptionalDispatcher(this IMauiContext mauiContext) =>
			mauiContext.Services.GetService<IDispatcher>();

		public static IMauiContext MakeApplicationScope(this IMauiContext mauiContext, NativeApplication platformApplication)
		{
			var scopedContext = new MauiContext(mauiContext.Services);

			scopedContext.AddSpecific(platformApplication);

			scopedContext.InitializeScopedServices();

			return scopedContext;
		}

		public static IMauiContext MakeWindowScope(this IMauiContext mauiContext, NativeWindow platformWindow, out IServiceScope scope)
		{
			scope = mauiContext.Services.CreateScope();

#if ANDROID
			var scopedContext = new MauiContext(scope.ServiceProvider, platformWindow);
#else
			var scopedContext = new MauiContext(scope.ServiceProvider);
#endif

			scopedContext.AddWeakSpecific(platformWindow);

#if ANDROID || (WINDOWS && __GTK__)
			scopedContext.AddSpecific(new NavigationRootManager(scopedContext));
#endif
#if WINDOWS && !__GTK__
			scopedContext.AddSpecific(new NavigationRootManager(platformWindow));
#endif

			return scopedContext;
		}

		public static void InitializeScopedServices(this IMauiContext scopedContext)
		{
			var scopedServices = scopedContext.Services.GetServices<IMauiInitializeScopedService>();

			foreach (var service in scopedServices)
				service.Initialize(scopedContext.Services);
		}

		public static FlowDirection GetFlowDirection(this IMauiContext mauiContext)
		{
			var appInfo = AppInfo.Current;

			if (appInfo.RequestedLayoutDirection == LayoutDirection.RightToLeft)
				return FlowDirection.RightToLeft;

			return FlowDirection.LeftToRight;
		}
	}
}
