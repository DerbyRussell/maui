using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gtk;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;

namespace Microsoft.Maui.Cells
{
	public abstract class CellBase : EventBox
	{
		private Cell_GTK _cell = null!;
		private int _desiredHeight;
		private IList<Controls.MenuItem> _contextActions = null!;
		private Dictionary<string, Gtk.Image> _contextActionItems = null!;
		private Gtk.Menu _menu;

		public Action<object, PropertyChangedEventArgs> PropertyChanged = null!;

		protected CellBase()
		{
			_contextActionItems = new Dictionary<string, Gtk.Image>();
			ButtonReleaseEvent += OnClick;
		}

		public Cell_GTK Cell
		{
			get { return _cell; }
			set
			{
				if (_cell == value)
					return;

				if (_cell != null)
					Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(_cell.SendDisappearing);
					// Device.BeginInvokeOnMainThread(_cell.SendDisappearing);

				_cell = value;
				UpdateCell();
				// _contextActions = Cell.ContextActions;

				if (_cell != null)
					Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(_cell.SendAppearing);
					// Device.BeginInvokeOnMainThread(_cell.SendAppearing);
			}
		}

		public object Item => Cell?.BindingContext;

		//protected bool ParentHasUnevenRows
		//{
		//	get
		//	{
		//		var table = Cell.RealParent as TableView;
		//		if (table != null)
		//			return table.HasUnevenRows;

		//		var list = Cell.RealParent as ListView;
		//		if (list != null)
		//			return list.HasUnevenRows;

		//		return false;
		//	}
		//}

		public int DesiredHeight
		{
			get
			{
				return _desiredHeight;
			}

			set
			{
				_desiredHeight = value;
			}
		}

		public void SetDesiredHeight(int height)
		{
			DesiredHeight = height;

			if (IsRealized)
			{
				HeightRequest = DesiredHeight;
			}
		}

		public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		protected override void OnRealized()
		{
			base.OnRealized();

			HeightRequest = DesiredHeight;
		}

		protected override void OnDestroyed()
		{
			base.OnDestroyed();

			ButtonReleaseEvent -= OnClick;
		}

		protected virtual void UpdateCell()
		{
		}

		private void OnClick(object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3)  // Right button
			{
				return;
			}

			if (_contextActions.Any())
			{
				OpenContextMenu();
			}
		}

		private void OpenContextMenu()
		{
			var menu = new Gtk.Menu();

			SetupMenuItems(menu);
			menu.ShowAll();
			menu.Popup();
		}

		private void SetupMenuItems(Gtk.Menu menu)
		{
			_menu = menu;

			foreach (Controls.MenuItem item in Cell.ContextActions)
			{
				//var menuItem = new Gtk.ImageMenuItem(item.Text);
				//GtkWidget* box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 6);
				//GtkWidget* icon = gtk_image_new_from_icon_name("folder-music-symbolic", GTK_ICON_SIZE_MENU);
				//GtkWidget* label = gtk_label_new("Music");
				//GtkWidget* menu_item = gtk_menu_item_new();
				//
				//gtk_container_add(GTK_CONTAINER(box), icon);
				//gtk_container_add(GTK_CONTAINER(box), label);
				//
				//gtk_container_add(GTK_CONTAINER(menu_item), box);
				//
				//gtk_widget_show_all(menu_item);
				var menuItemBox = new Gtk.Box(Orientation.Horizontal, 6);
				var menuImageIcon = new Gtk.Image();
				var menuItemLabel = new Gtk.Label(item.Text);
				var menuItem = new Gtk.MenuItem();

				menuItemBox.Add(menuImageIcon);
				menuItemBox.Add(menuItemLabel);

				menuItem.Add(menuItemBox);
				menuItem.ShowAll();

				_contextActionItems.Add(item.Text, menuImageIcon);

				var imageProccessor = new ImageProcessing();
				imageProccessor.CallBackGetNativeImage += ImageProccessor_CallBackGetNativeImage;

				if (item.GetValue(Controls.MenuItem.IconImageSourceProperty) is ImageSource imageSource)
				{
					if (imageSource == null || imageSource.IsEmpty)
						return;

					imageProccessor.StartApplyNativeImageFromSource(imageSource, item.Text, (object)item);
				}

				//_ = item.ApplyNativeImageAsync(Controls.MenuItem.IconImageSourceProperty, icon =>
				//{
				//	if (icon != null)
				//		menuItem.Image = new Gtk.Image(icon);
				//});

				menuItem.ButtonPressEvent += (sender, args) =>
				{
					((IMenuItemController)item).Activate();
				};

				menu.Add(menuItem);
			}
		}

		private void ImageProccessor_CallBackGetNativeImage(object sender, EventArgs e)
		{
			if (sender is ImageProcessing imageProcessor)
			{
				var title = imageProcessor.Dimensions.Title;
				//var menuImageIcon = _contextActionItems[title];
				var pixBuf = imageProcessor.Dimensions.PixbufReturned;

				if (pixBuf != null && _menu != null)
				{
					//var menuItem = new Gtk.ImageMenuItem(item.Text);
					//GtkWidget* box = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 6);
					//GtkWidget* icon = gtk_image_new_from_icon_name("folder-music-symbolic", GTK_ICON_SIZE_MENU);
					//GtkWidget* label = gtk_label_new("Music");
					//GtkWidget* menu_item = gtk_menu_item_new();
					//
					//gtk_container_add(GTK_CONTAINER(box), icon);
					//gtk_container_add(GTK_CONTAINER(box), label);
					//
					//gtk_container_add(GTK_CONTAINER(menu_item), box);
					//
					//gtk_widget_show_all(menu_item);
					var menuItemBox = new Gtk.Box(Orientation.Horizontal, 6);
					var menuImageIcon = new Gtk.Image(pixBuf);
					var menuItemLabel = new Gtk.Label(title);
					var menuItem = new Gtk.MenuItem();

					menuItemBox.Add(menuImageIcon);
					menuItemBox.Add(menuItemLabel);

					menuItem.Add(menuItemBox);
					menuItem.ShowAll();

					//_ = item.ApplyNativeImageAsync(Controls.MenuItem.IconImageSourceProperty, icon =>
					//{
					//	if (icon != null)
					//		menuItem.Image = new Gtk.Image(icon);
					//});

					if (imageProcessor.Dimensions.Obj is Controls.MenuItem item)
					{
						menuItem.ButtonPressEvent += (sender, args) =>
						{
							((IMenuItemController)item).Activate();
						};
					}

					_menu.Add(menuItem);
				}
			}
		}
	}
}
