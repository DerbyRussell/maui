using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Handlers.Compatibility
{
	public sealed class EntryCellView : MauiView, INativeElementView
	{
		public const double DefaultMinHeight = 55;

		readonly Cell _cell;
		readonly Gtk.Label _label;
		readonly Gtk.Box _hbox;

		//Color _labelTextColor;
		string _labelTextText;

		public EntryCellView(Cell cell)
		{
			_cell = cell;
			//SetMinimumWidth((int)context.ToPixels(50));
			//SetMinimumHeight((int)context.ToPixels(85));
			//Orientation = Orientation.Horizontal;

			//var padding = (int)context.ToPixels(8);
			//SetPadding((int)context.ToPixels(15), padding, padding, padding);

			_label = new Gtk.Label();
			_hbox = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
			//TextViewCompat.SetTextAppearance(_label, global::Android.Resource.Style.TextAppearanceSmall);

			//var layoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) { Gravity = GravityFlags.CenterVertical };
			//using (layoutParams)
			//	AddView(_label, layoutParams);
			// Add(_label);

			EditText = new Gtk.Entry();
			//EditText.AddTextChangedListener(this);
			//EditText.OnFocusChangeListener = this;
			//EditText.SetOnEditorActionListener(this);
			//EditText.ImeOptions = ImeAction.Done;
			//EditText.BackButtonPressed += OnBackButtonPressed;
			//editText.SetBackgroundDrawable (null);
			//layoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) { Width = 0, Weight = 1, Gravity = GravityFlags.FillHorizontal | GravityFlags.Center };
			//using (layoutParams)
			//	AddView(EditText, layoutParams);
			_hbox.PackStart(_label, false, false, 0);
			_hbox.PackStart(EditText, false, false, 0);
			Add(_hbox);
		}

		public Action EditingCompleted { get; set; }

		public Gtk.Entry EditText { get; }

		public Action<bool> FocusChanged { get; set; }

		public string LabelText
		{
			get { return _labelTextText; }
			set
			{
				if (_labelTextText == value)
					return;

				_labelTextText = value;
				_label.Text = value;
			}
		}

		public Action<string> TextChanged { get; set; }

		public Element Element
		{
			get { return _cell; }
		}

		//bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
		//{
		//	if (actionId == ImeAction.Done)
		//	{
		//		OnKeyboardDoneButtonPressed(EditText, EventArgs.Empty);
		//		EditText.ClearFocus();
		//		v.HideKeyboard();
		//	}

		//	// Fire Completed and dismiss keyboard for hardware / physical keyboards
		//	if (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter)
		//	{
		//		OnKeyboardDoneButtonPressed(EditText, EventArgs.Empty);
		//		EditText.ClearFocus();
		//		v.HideKeyboard();
		//	}

		//	return true;
		//}

		//void IOnFocusChangeListener.OnFocusChange(global::Android.Views.View view, bool hasFocus)
		//{
		//	Action<bool> focusChanged = FocusChanged;
		//	if (focusChanged != null)
		//		focusChanged(hasFocus);
		//}

		//void ITextWatcher.AfterTextChanged(IEditable s)
		//{
		//}

		//void ITextWatcher.BeforeTextChanged(ICharSequence s, int start, int count, int after)
		//{
		//}

		//void ITextWatcher.OnTextChanged(ICharSequence s, int start, int before, int count)
		//{
		//	Action<string> changed = TextChanged;
		//	if (changed != null)
		//		changed(s != null ? s.ToString() : null);
		//}

		public void SetLabelTextColor(Color color, int defaultColorResourceId)
		{
			//if (_labelTextColor == color)
			//	return;

			//_labelTextColor = color;
			//_label.SetTextColor(color.ToPlatform(defaultColorResourceId, _label.Context));
		}

		public void SetRenderHeight(double height)
		{
			// SetMinimumHeight((int)Context.ToPixels(height == -1 ? DefaultMinHeight : height));
		}

		void OnBackButtonPressed(object sender, EventArgs e)
		{
			// TODO Clear focus
		}

		void OnKeyboardDoneButtonPressed(object sender, EventArgs e)
		{
			// TODO Clear focus
			Action editingCompleted = EditingCompleted;
			if (editingCompleted != null)
				editingCompleted();
		}
	}
}