using System;

namespace Microsoft.Maui.Controls
{
	public interface IListViewController : IViewController
	{
		event EventHandler<ScrollToRequestedEventArgs> ScrollToRequested;

		ListViewCachingStrategy CachingStrategy { get; }
		Element FooterElement { get; }
		Element HeaderElement { get; }
		bool RefreshAllowed { get; }

		string GetDisplayTextFromGroup(object cell);
#if __GTK__
		Cell_GTK CreateDefaultCell(object item);
		void NotifyRowTapped(int index, int inGroupIndex, Cell_GTK cell);
		void NotifyRowTapped(int index, int inGroupIndex, Cell_GTK cell, bool isContextMenuRequested);
		void NotifyRowTapped(int index, Cell_GTK cell);
		void NotifyRowTapped(int index, Cell_GTK cell, bool isContextMenuRequested);
		void SendCellAppearing(Cell_GTK cell);
		void SendCellDisappearing(Cell_GTK cell);
		void SendRefreshing();
#else
		Cell CreateDefaultCell(object item);
		void NotifyRowTapped(int index, int inGroupIndex, Cell cell);
		void NotifyRowTapped(int index, int inGroupIndex, Cell cell, bool isContextMenuRequested);
		void NotifyRowTapped(int index, Cell cell);
		void NotifyRowTapped(int index, Cell cell, bool isContextMenuRequested);
		void SendCellAppearing(Cell cell);
		void SendCellDisappearing(Cell cell);
		void SendRefreshing();
#endif
	}
}