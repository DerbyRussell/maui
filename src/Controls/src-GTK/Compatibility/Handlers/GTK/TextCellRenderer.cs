﻿using System.ComponentModel;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Handlers.Compatibility
{
	public class TextCellRenderer : CellRenderer
	{
		internal TextCellView View { get; private set; }

		protected override System.Object GetCellCore(Cell item, System.Object parent)
		{
			View = new TextCellView(item);

			UpdateMainText();
			UpdateDetailText();
			UpdateHeight();
			UpdateIsEnabled();
			UpdateFlowDirection();
			UpdateAutomationId();
			View.SetImageVisible(false);

			return View;
		}

		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			//if (View.IsDisposed())
			//{
			//	return;
			//}

			if (args.PropertyName == TextCell.TextProperty.PropertyName || args.PropertyName == TextCell.TextColorProperty.PropertyName)
				UpdateMainText();
			else if (args.PropertyName == TextCell.DetailProperty.PropertyName || args.PropertyName == TextCell.DetailColorProperty.PropertyName)
				UpdateDetailText();
			else if (args.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled();
			else if (args.PropertyName == "RenderHeight")
				UpdateHeight();
			else if (args.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
			else if (args.PropertyName == VisualElement.AutomationIdProperty.PropertyName)
				UpdateAutomationId();
		}

		void UpdateAutomationId()
		{
			//View.ContentDescription = Cell.AutomationId;
		}

		void UpdateDetailText()
		{
			var cell = (TextCell)Cell;
			View.DetailText = cell.Detail;
			View.SetDetailTextColor(cell.DetailColor);
		}

		void UpdateHeight()
		{
			View.SetRenderHeight(Cell.RenderHeight);
		}

		void UpdateIsEnabled()
		{
			var cell = (TextCell)Cell;
			View.SetIsEnabled(cell.IsEnabled);
		}

		void UpdateFlowDirection()
		{
			View.UpdateFlowDirection(ParentView);
		}

		void UpdateMainText()
		{
			var cell = (TextCell)Cell;
			View.MainText = cell.Text;

			if (!cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
				View.SetDefaultMainTextColor(Application.AccentColor);
			else
				View.SetDefaultMainTextColor(null);

			View.SetMainTextColor(cell.TextColor);
		}

		// ensure we don't get other people's BaseCellView's
		internal class TextCellView : BaseCellView
		{
			public TextCellView(Cell cell) : base(cell)
			{
			}
		}
	}
}