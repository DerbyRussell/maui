using System;
using System.Threading;
using GLib;

namespace Microsoft.Maui.Controls.Platform
{
	public class GtkSynchronizationContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state)
		{
			Gtk.Application.Invoke((s, e) =>
			{
				d(state);
			});
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			if (System.Threading.Thread.CurrentThread.ManagedThreadId == MauiWindow.MainThreadID)
			{
				d(state);
			}
			else
			{
				var evt = new ManualResetEvent(false);
				Exception exception = null!;

				Gtk.Application.Invoke((s, e) =>
				{
					try
					{
						d(state);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						evt.Set();
					}
				});

				evt.WaitOne();

				if (exception != null)
					throw exception;
			}
		}
	}
}
