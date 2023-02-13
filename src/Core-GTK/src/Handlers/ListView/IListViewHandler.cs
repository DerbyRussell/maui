using System;
using System.Collections.Generic;
using System.Text;

using PlatformView = Gtk.ScrolledWindow;

namespace Microsoft.Maui.Handlers
{
	public interface IListViewHandler : IViewHandler
	{
		new IListView VirtualView { get; }
		new PlatformView PlatformView { get; }
	}
}
