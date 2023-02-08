﻿using System;
using System.Runtime.InteropServices;
using GLib;
using Gtk;
using Microsoft.Maui.Devices;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui
{
	public class MauiGTKWindow : Gtk.Window
	{
		public MauiGTKWindow(string title) : base(title)
		{
			Destroyed += MauiGTKWindow_Destroyed;
			// ModifyBg(StateType.Normal, new Gdk.Color(200, 0, 200));
		}

		private void MauiGTKWindow_Destroyed(object? sender, EventArgs e)
		{
			Gtk.Application.Quit();
		}

		//public void PopulateFromXaml(IWindow window, IMauiContext mauiContext)
		//{
		//	if (window is Window win)
		//	{
		//		foreach (var child in window.NativeWindow.Children)
		//		{
		//			Add(child);
		//		}
		//	}
		//}

		public void RemovePage(Gtk.Window previousRootView)
		{
			foreach (var child in previousRootView.Children)
			{
				Remove(child);
			}
		}

		public void AddPage(ContentViewGroup RootView)
		{
			//var child = RootView.GetChild();

			Add(RootView.GetChild());
			ShowAll();
			//if (child != null)
			//{
			//	RootView.RemoveChild();
			//	Add(child);
			//}
			//foreach (var child in RootView.GetView())
			//{
			//	Add(child);
			//	return;
			//}
		}
	}
}
