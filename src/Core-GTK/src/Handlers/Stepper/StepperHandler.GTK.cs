using System;
using Gtk;

namespace Microsoft.Maui.Handlers
{
	public partial class StepperHandler : ViewHandler<IStepper, MauiStepper>
	{
		protected override MauiStepper CreatePlatformView(IView stepper)
		{
			double stepping = Math.Min((VirtualView.Maximum - VirtualView.Minimum) / 10, 1);

			var stepperLayout = new MauiStepper(VirtualView.Minimum, VirtualView.Maximum, stepping);
			stepperLayout.StepperWidget.ValueChanged += StepperLayout_ValueChanged;

			Gtk.Widget widget = stepperLayout;
			SetMargins(stepper, ref widget);

			if (stepper is IStepper stepperView)
			{
				if (stepperView.Visibility == Visibility.Visible)
				{
					stepperLayout.Show();
				}
			}
			return stepperLayout;
		}

		public static void MapMinimum(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.StepperWidget.SetRange(stepper.Minimum, stepper.Maximum);
		}

		public static void MapMaximum(IStepperHandler handler, IStepper stepper)
		{
			handler.PlatformView?.StepperWidget.SetRange(stepper.Minimum, stepper.Maximum);
		}

		public static void MapIncrement(IStepperHandler handler, IStepper stepper)
		{
			if (handler.PlatformView != null)
			{
				handler.PlatformView.StepperWidget.Value = stepper.Value;
			}
		}

		public static void MapValue(IStepperHandler handler, IStepper stepper)
		{
			if (handler.PlatformView != null)
			{
				handler.PlatformView.StepperWidget.Value = stepper.Value;
			}
		}

		private void StepperLayout_ValueChanged(object? sender, EventArgs e)
		{
			if (sender is SpinButton stepper)
			{
				if (VirtualView != null)
				{
					VirtualView.Value = stepper.Value;
				}
			}
		}
	}
}