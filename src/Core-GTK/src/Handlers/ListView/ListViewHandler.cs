using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GLib;
using Gtk;
using Microsoft.Maui.Platform.GTK;

using PlatformView = Gtk.ScrolledWindow;

namespace Microsoft.Maui.Handlers
{
	public partial class ListViewHandler : ViewHandler<IListView, Gtk.ScrolledWindow>, IListViewHandler
	{
		public static IPropertyMapper<IListView, IListViewHandler> Mapper = new PropertyMapper<IListView, IListViewHandler>(ViewMapper)
		{
			[nameof(IListView.Content)] = MapContent,
			[nameof(IListView.HorizontalScrollBarVisibility)] = MapHorizontalScrollBarVisibility,
			[nameof(IListView.VerticalScrollBarVisibility)] = MapVerticalScrollBarVisibility,
			[nameof(IListView.Orientation)] = MapOrientation,
		};

		public static CommandMapper<IListView, IListViewHandler> CommandMapper = new(ViewCommandMapper)
		{
			[nameof(IListView.RequestScrollTo)] = MapRequestScrollTo
		};

		protected override Gtk.ScrolledWindow CreatePlatformView(IView scrollView)
		{
			//var plat = new MauiView();
			//plat.Add(new Gtk.ScrolledWindow());
			var scroller = new Gtk.ScrolledWindow();
			Gtk.Widget widget = scroller;
			SetMargins(scrollView, ref widget);
			scroller.ShowAll();
			return scroller;
		}

		public ListViewHandler() : base(Mapper, CommandMapper)
		{

		}

		public ListViewHandler(IPropertyMapper? mapper)
			: base(mapper ?? Mapper, CommandMapper)
		{
		}

		public ListViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
			: base(mapper ?? Mapper, commandMapper ?? CommandMapper)
		{
		}

		IListView IListViewHandler.VirtualView => VirtualView;

		PlatformView IListViewHandler.PlatformView => PlatformView;

		protected override void ConnectHandler(Gtk.ScrolledWindow platformView)
		{
			base.ConnectHandler(platformView);
			platformView.ScrollEvent += PlatformView_ScrollEvent;
		}

		protected override void DisconnectHandler(Gtk.ScrolledWindow platformView)
		{
			base.DisconnectHandler(platformView);
			platformView.ScrollEvent -= PlatformView_ScrollEvent;
		}

		private void PlatformView_ScrollEvent(object o, Gtk.ScrollEventArgs args)
		{
			//var context = (sender as View)?.Context;

			//if (context == null)
			//{
			//	return;
			//}

			//VirtualView.VerticalOffset = Context.FromPixels(e.ScrollY);
			//VirtualView.HorizontalOffset = Context.FromPixels(e.ScrollX);
		}

		public static void MapContent(IListViewHandler handler, IListView scrollView)
		{
			if (handler.PlatformView == null || handler.MauiContext == null)
				return;

			// UpdateContentPanel(scrollView, handler);
		}

		public static void MapHorizontalScrollBarVisibility(IListViewHandler handler, IListView scrollView)
		{
			//handler.PlatformView.SetHorizontalScrollBarVisibility(scrollView.HorizontalScrollBarVisibility);
		}

		public static void MapVerticalScrollBarVisibility(IListViewHandler handler, IListView scrollView)
		{
			//handler.PlatformView.SetVerticalScrollBarVisibility(scrollView.HorizontalScrollBarVisibility);
		}

		public static void MapOrientation(IListViewHandler handler, IListView scrollView)
		{
			//handler.PlatformView.SetOrientation(scrollView.Orientation);
		}

		public static void MapRequestScrollTo(IListViewHandler handler, IListView scrollView, object? args)
		{
			//if (args is not ScrollToRequest request)
			//{
			//	return;
			//}

			//var context = handler.PlatformView.Context;

			//if (context == null)
			//{
			//	return;
			//}

			//var horizontalOffsetDevice = (int)context.ToPixels(request.HoriztonalOffset);
			//var verticalOffsetDevice = (int)context.ToPixels(request.VerticalOffset);

			//handler.PlatformView.ScrollTo(horizontalOffsetDevice, verticalOffsetDevice,
			//	request.Instant, () => handler.VirtualView.ScrollFinished());
		}
	}
}
