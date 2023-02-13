using System;
using System.Collections.Generic;
using System.Text;
using Gdk;

namespace Microsoft.Maui.Controls.Platform
{
	public class ImageDimensions
	{
		public ImageDimensions()
		{
			this.PixbufReturned = null;
			this.SizeWidth = 0;
			this.SizeHeight = 0;
			this.Title = string.Empty;
			this.Obj = null;
		}

		public Pixbuf PixbufReturned { get; set; }
		public int SizeWidth { get; set; }
		public int SizeHeight { get; set; }
		public string Title { get; set; }
		public object Obj { get; set; }

		public void Dispose()
		{
			this.PixbufReturned = null;
		}
	}
}