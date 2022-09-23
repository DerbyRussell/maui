﻿using System;
using Gtk;
using Pango;

namespace Microsoft.Maui.Platform
{
	public class StepperControl : MauiView
	{
		public StepperControl(double min, double max, double step)
		{
			StepperWidget = new Gtk.SpinButton(min, max, step);
			Add(StepperWidget);
		}

		public Gtk.SpinButton StepperWidget { get; set; }
	}
}
