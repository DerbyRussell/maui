using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gtk;
using Microsoft.Maui.Cells;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls
{
	// button
	// The button which was pressed or released,
	// numbered from 1 to 5.
	// Normally button 1 is the left mouse button,
	// 2 is the middle button,
	// and 3 is the right button.
	// On 2-button mice, the middle button can often be simulated by pressing both mouse buttons together.

	public enum GtkMouseButton
	{
		Left = 1,
		Middle = 2,
		Right = 3
	}

	public class ListItemTappedEventArgs : EventArgs
	{
		private object _item;
		private GtkMouseButton _mouseButton;

		public object Item
		{
			get
			{
				return _item;
			}
		}

		public GtkMouseButton MouseButton
		{
			get
			{
				return _mouseButton;
			}
		}

		public ListItemTappedEventArgs(object item, GtkMouseButton button)
		{
			_item = item;
			_mouseButton = button;
		}
	}

	public class SelectedItemEventArgs : EventArgs
	{
		private object _selectedItem;

		public object SelectedItem
		{
			get
			{
				return _selectedItem;
			}
		}

		public SelectedItemEventArgs(object selectedItem)
		{
			_selectedItem = selectedItem;
		}
	}

	public class ListViewSeparator : EventBox
	{
		public ListViewSeparator()
		{
			HeightRequest = 1;
			VisibleWindow = false;
		}
	}

	public enum State : uint
	{
		Started,
		Loading,
		Completed,
		Finished
	};

	public class IdleData
	{
		public State LoadState;
		public uint LoadId;
		public ListStore ListStore = null!;
		public int NumItems;
		public int NumLoaded;
		public System.Collections.Generic.List<CellBase> Items = null!;
	}

	public class ListViewControl : ScrolledWindow
	{
		private const int RefreshHeight = 48;

		private Box _root = null!;
		private EventBox _headerContainer = null!;
		private Widget _header = null!;
		private Box _list = null!;
		private EventBox _footerContainer = null!;
		private Widget _footer = null!;
		private Viewport _viewPort = null!;
		private IEnumerable<Widget> _cells = null!;
		private List<ListViewSeparator> _separators = null!;
		private object _selectedItem = null!;
		private Gtk.Grid _refreshHeader = null!;
		private MauiGTKButton _refreshButton = null!;
		private Gtk.Label _refreshLabel = null!;
		private bool _isPullToRequestEnabled;
		private bool _refreshing;
		private IdleData _data = null!;
		private ListStore _store = null!;
		private List<CellBase> _items = null!;
		private CellBase _selectedCell = null!;
		private Gdk.Color _selectionColor;

		public delegate void ListItemTappedEventHandler(object sender, ListItemTappedEventArgs args);
		public event ListItemTappedEventHandler OnItemTapped = null!;

		public delegate void SelectedItemEventHandler(object sender, SelectedItemEventArgs args);
		public event SelectedItemEventHandler OnSelectedItemChanged = null!;

		public delegate void RefreshEventHandler(object sender, EventArgs args);
		public event RefreshEventHandler OnRefresh = null!;

		public ListViewControl()
		{
			BuildListView();

			_selectionColor = DefaultSelectionColor;
		}

		public override void Destroy()
		{
			_store?.Dispose();
			_store = null!;
			_root = null!;
			_refreshButton = null!;
			_refreshLabel = null!;
			_headerContainer = null!;
			_header = null!;
			_list = null!;
			_footerContainer = null!;
			_footer = null!;
			_viewPort = null!;
			_refreshHeader = null!;
			base.Destroy();
		}

		public static Gdk.Color DefaultSelectionColor = new Gdk.Color(0x34, 0x98, 0xD8);

		public Widget Header
		{
			get
			{
				return _header;
			}
			set
			{
				if (_header != value)
				{
					_header = value;
					RefreshHeader(_header);
				}
			}
		}

		public IEnumerable<Widget> Items
		{
			get
			{
				return _cells;
			}
			set
			{
				_cells = value;
				_items = new List<CellBase>();
				PopulateData(_items);
			}
		}

		public Widget Footer
		{
			get
			{
				return _footer;
			}
			set
			{
				if (_footer != value)
				{
					_footer = value;
					RefreshFooter(_footer);
				}
			}
		}

		public object SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (value != _selectedItem)
				{
					UpdateSelectedItem(value);
				}
			}
		}

		public bool IsPullToRequestEnabled
		{
			get { return _isPullToRequestEnabled; }
			set { _isPullToRequestEnabled = value; }
		}

		public bool Refreshing
		{
			get { return _refreshing; }
			set { _refreshing = value; }
		}

		public Gdk.Color SelectionColor
		{
			get
			{
				return _selectionColor;
			}

			set
			{
				_selectionColor = value;
				SelectionColorUpdated();
			}
		}

		public void SetBackgroundColor(Gdk.Color backgroundColor)
		{
			//if (_root != null)
			//{
			//	_root.ModifyBg(StateType.Normal, backgroundColor);
			//	_viewPort.ModifyBg(StateType.Normal, backgroundColor);

			//	if (_headerContainer != null && !_headerContainer.Children.Any())
			//	{
			//		_headerContainer.ModifyBg(StateType.Normal, backgroundColor);
			//	}

			//	if (_footerContainer != null && !_footerContainer.Children.Any())
			//	{
			//		_footerContainer.ModifyBg(StateType.Normal, backgroundColor);
			//	}
			//}
		}

		public void SetSeparatorColor(Gdk.Color separatorColor)
		{
			foreach (var separator in _separators)
			{
				//separator.ModifyBg(StateType.Normal, separatorColor);
				separator.VisibleWindow = true;
			}
		}

		public void SetSeparatorVisibility(bool visible)
		{
			foreach (var separator in _separators)
			{
				separator.HeightRequest = visible ? 1 : 0;
			}
		}

		public void UpdatePullToRefreshEnabled(bool isPullToRequestEnabled)
		{
			IsPullToRequestEnabled = isPullToRequestEnabled;

			if (_refreshHeader == null)
			{
				return;
			}

			if (IsPullToRequestEnabled)
			{
				_root.RemoveFromContainer(_refreshHeader);
				_root.PackStart(_refreshHeader, false, false, 0);
				_root.ReorderChild(_refreshHeader, 0);
			}
			else
			{
				_root.RemoveFromContainer(_refreshHeader);
			}
		}

		public void UpdateIsRefreshing(bool refreshing)
		{
			Refreshing = refreshing;

			if (Refreshing)
			{
				_refreshHeader.Attach(_refreshLabel, 0, 1, 0, 1);
			}
			else
			{
				_refreshHeader.RemoveFromContainer(_refreshLabel);
			}

			//_refreshButton.Visible = !Refreshing;
			_refreshLabel.Visible = Refreshing;
		}

		public void SetSeletedItem(object selectedItem)
		{
			if (selectedItem == null)
			{
				return;
			}

			SelectedItem = selectedItem;
		}

		private void BuildListView()
		{
			_items = new List<CellBase>();

			CanFocus = true;
			ShadowType = ShadowType.None;
			BorderWidth = 0;
			HscrollbarPolicy = PolicyType.Never;
			VscrollbarPolicy = PolicyType.Automatic;

			_root = new Box(Orientation.Vertical, 0);
			_refreshHeader = new Gtk.Grid();
			_refreshHeader.HeightRequest = RefreshHeight;

			// Refresh Loading
			_refreshLabel = new Gtk.Label();
			_refreshLabel.Text = "Loading";

			// Refresh Button
			_refreshButton = new MauiGTKButton("Refresh", null!, Stock.Refresh);
			//_re
			//_refreshButton.Label.SetTextFromSpan(
			//	new Gtk.Span()
			//	{
			//		Text = "Refresh"
			//	});
			//_refreshButton.ImageWidget.Stock = Stock.Refresh;
			//_refreshButton.SetImagePosition(PositionType.Left);
			_refreshButton.Clicked += (sender, args) =>
			{
				OnRefresh?.Invoke(this, new EventArgs());
			};

			_refreshHeader.Attach(_refreshButton, 0, 1, 0, 1);

			_root.PackStart(_refreshHeader, false, false, 0);

			// Header
			_headerContainer = new EventBox();
			_root.PackStart(_headerContainer, false, false, 0);

			// List
			_list = new Box(Orientation.Vertical, 0);
			_separators = new List<ListViewSeparator>();
			_root.PackStart(_list, true, true, 0);

			// Footer
			_footerContainer = new EventBox();
			_root.PackStart(_footerContainer, false, false, 0);

			_viewPort = new Viewport();
			_viewPort.ShadowType = ShadowType.None;
			_viewPort.BorderWidth = 0;
			_viewPort.Add(_root);

			Add(_viewPort);

			ShowAll();
		}

		private void RefreshHeader(Widget newHeader)
		{
			if (_headerContainer != null)
			{
				foreach (var child in _headerContainer.Children)
				{
					_headerContainer.RemoveFromContainer(child);
					child.Dispose();
					child.Destroy();
				}
			}

			if (newHeader != null && _headerContainer != null)
			{
				_header = newHeader;
				_headerContainer.Add(_header);
				_header.ShowAll();
			}
		}

		private void RefreshFooter(Widget newFooter)
		{
			if (_footerContainer != null)
			{
				foreach (var child in _footerContainer.Children)
				{
					_footerContainer.RemoveFromContainer(child);
					child.Dispose();
					child.Destroy();
				}
			}

			if (newFooter != null && _footerContainer != null)
			{
				_footer = newFooter;
				_footerContainer.Add(_footer);
				_footer.ShowAll();
			}
		}

		private void LazyLoadItems(System.Collections.Generic.List<CellBase> items)
		{
			_data = new IdleData();

			_data.Items = items;
			_data.NumItems = 0;
			_data.NumLoaded = 0;
			_data.ListStore = _store;
			_data.LoadState = Controls.State.Started;
			_data.LoadId = GLib.Idle.Add(new GLib.IdleHandler(LoadItems));
		}

		private bool LoadItems()
		{
			IdleData id = _data;
			CellBase obj = null;
			TreeIter? iter = null;

			// Make sure we're in the right state 
			var isLoading = (id.LoadState == Controls.State.Started) ||
				(id.LoadState == Controls.State.Loading);

			if (!isLoading)
			{
				id.LoadState = Controls.State.Completed;
				return false;
			}

			// No items 
			if (id.Items.Count == 0)
			{
				id.LoadState = Controls.State.Completed;
				return false;
			}

			// First run 
			if (id.NumItems == 0)
			{
				id.NumItems = id.Items.Count;
				id.NumLoaded = 0;
				id.LoadState = Controls.State.Loading;
			}

			// Get the item in the list at pos n_loaded 
			if (id != null && id.Items != null && id.NumLoaded >= 0 && id.Items[id.NumLoaded] != null)
			{
				try
				{
					var item = id.Items[id.NumLoaded];
					if (item != null)
					{
						obj = item as CellBase;
					}
				} catch
				{
					
				}
			}

			// Append the row to the store
			if (obj != null && id != null)
			{
				iter = id.ListStore.AppendValues(obj);
			}

			// Fill in the row at position n_loaded
			if (iter != null && id != null)
			{
				id.ListStore.SetValue(iter.Value, 0, obj);
			}

			if (id != null)
			{
				id.NumLoaded += 1;
			}

			// Update UI with every item
			if (obj != null)
			{
				UpdateItem(obj);
			}

			// We loaded everything, so we can change state and remove the idle callback function
			if (id != null && id.NumLoaded == id.NumItems)
			{
				id.LoadState = Controls.State.Completed;
				id.NumLoaded = 0;
				id.NumItems = 0;
				id.Items = null!;

				CleanupLoadItems();

				return false;
			}
			else
			{
				return true;
			}
		}

		private void UpdateItem(CellBase cell)
		{
			cell.ButtonPressEvent += (sender, args) =>
			{
				var gtkCell = sender as CellBase;

				if (gtkCell != null && gtkCell.Item != null)
				{
					SelectedItem = gtkCell.Item;

					MarkCellAsSelected(gtkCell);

					OnItemTapped?.Invoke(this, new ListItemTappedEventArgs(SelectedItem, (GtkMouseButton)args.Event.Button - 1));
				}
			};

			cell.VisibleWindow = false;

			_list.PackStart(cell, false, false, 0);
			cell.ShowAll();

			var separator = new ListViewSeparator();
			_separators.Add(separator);
			_list.PackStart(separator, false, false, 0);
			separator.ShowAll();
		}

		private void CleanupLoadItems()
		{
			Debug.Assert(_data.LoadState == Controls.State.Completed);

			_list.ShowAll();

			if (_data.ListStore == null)
				Debug.WriteLine("Something was wrong!");
		}

		private void PopulateData(System.Collections.Generic.List<CellBase> items)
		{
			_store = new ListStore(typeof(CellBase));

			foreach (var cell in _cells)
			{
				items.Append(cell);
			}

			ClearList();
			LazyLoadItems(items);
		}

		private void ClearList()
		{
			_selectedCell = null!;

			if (_list != null)
			{
				foreach (var child in _list.Children)
				{
					_list.RemoveFromContainer(child);
				}
			}

			if (_separators != null)
			{
				_separators.Clear();
			}
		}

		private void UpdateSelectedItem(object value)
		{
			_selectedItem = value;

			CellBase cell = _list.Children.OfType<CellBase>().FirstOrDefault(c => c.Item == value);
			MarkCellAsSelected(cell);

			OnSelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(_selectedItem));
		}

		private void MarkCellAsSelected(CellBase cell)
		{
			if (cell == null)
				return;

			if (cell.Cell.GetIsGroupHeader<ItemsView<Cell_GTK>, Cell_GTK>())
				return;

			foreach (var childCell in _list.Children.OfType<CellBase>())
			{
				bool isTargetCell = cell == childCell;

				childCell.VisibleWindow = isTargetCell;

				if (isTargetCell)
				{
					_selectedCell = childCell;
					//childCell.ModifyBg(StateType.Normal, _selectionColor);
				}
			}
		}

		private void SelectionColorUpdated()
		{
			if (_selectedCell == null)
				return;

			//_selectedCell.ModifyBg(StateType.Normal, _selectionColor);
		}
	}
}
