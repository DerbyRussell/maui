using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public partial class ListView : IListView, IList<IView>, IBindableLayout, IPaddingElement
	{
		// The actual backing store for the IViews in the ILayout
		readonly List<IView> _children = new();

		// This provides a Children property for XAML 
		public IList<IView> Children => this;

		IList IBindableLayout.Children => _children;

		internal override IReadOnlyList<Element> LogicalChildrenInternal =>
			_logicalChildren ??= new ReadOnlyCastingList<Element, IView>(_children);

		public int Count => _children.Count;

		public bool IsReadOnly => ((ICollection<IView>)_children).IsReadOnly;

		public IView this[int index]
		{
			get => _children[index];
			set
			{
				var old = _children[index];

				if (old == value)
				{
					return;
				}

				if (old is Element oldElement)
				{
					oldElement.Parent = null;
					VisualDiagnostics.OnChildRemoved(this, oldElement, index);
				}

				_children[index] = value;

				if (value is Element newElement)
				{
					newElement.Parent = this;
					VisualDiagnostics.OnChildAdded(this, newElement);
				}

				OnUpdate(index, value, old);
			}
		}

		public static readonly BindableProperty PaddingProperty = PaddingElement.PaddingProperty;

		public Thickness Padding
		{
			get => (Thickness)GetValue(PaddingElement.PaddingProperty);
			set => SetValue(PaddingElement.PaddingProperty, value);
		}

		public IEnumerator<IView> GetEnumerator() => _children.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

		public override SizeRequest Measure(double widthConstraint, double heightConstraint, MeasureFlags flags = MeasureFlags.None)
		{
			var size = (this as IView).Measure(widthConstraint, heightConstraint);
			return new SizeRequest(size);
		}

		protected override void InvalidateMeasureOverride()
		{
			base.InvalidateMeasureOverride();
		}

		public void Add(IView child)
		{
			if (child == null)
				return;

			var index = _children.Count;
			_children.Add(child);

			OnAdd(index, child);
		}

		public void Clear()
		{
			for (var index = Count - 1; index >= 0; index--)
			{
				if (this[index] is Element element)
				{
					OnChildRemoved(element, index);
				}
			}

			_children.Clear();
			OnClear();
		}

		public bool Contains(IView item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(IView[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public int IndexOf(IView item)
		{
			return _children.IndexOf(item);
		}

		public void Insert(int index, IView child)
		{
			if (child == null)
				return;

			_children.Insert(index, child);

			OnInsert(index, child);
		}

		public bool Remove(IView child)
		{
			if (child == null)
				return false;

			var index = _children.IndexOf(child);

			if (index == -1)
			{
				return false;
			}

			RemoveAt(index);

			return true;
		}

		public void RemoveAt(int index)
		{
			if (index >= Count)
			{
				return;
			}

			var child = _children[index];

			_children.RemoveAt(index);

			OnRemove(index, child);
		}

		protected virtual void OnAdd(int index, IView view)
		{
			NotifyHandler(nameof(ILayoutHandler.Add), index, view);

			// Make sure CascadeInputTransparent is applied, if necessary
			//Handler?.UpdateValue(nameof(CascadeInputTransparent));

			// Take care of the Element internal bookkeeping
			if (view is Element element)
			{
				OnChildAdded(element);
			}
		}

		protected virtual void OnClear()
		{
			Handler?.Invoke(nameof(ILayoutHandler.Clear));
		}

		protected virtual void OnRemove(int index, IView view)
		{
			NotifyHandler(nameof(ILayoutHandler.Remove), index, view);

			// Take care of the Element internal bookkeeping
			if (view is Element element)
			{
				OnChildRemoved(element, index);
			}
		}

		protected virtual void OnInsert(int index, IView view)
		{
			NotifyHandler(nameof(ILayoutHandler.Insert), index, view);

			// Make sure CascadeInputTransparent is applied, if necessary
			//Handler?.UpdateValue(nameof(CascadeInputTransparent));

			// Take care of the Element internal bookkeeping
			if (view is Element element)
			{
				OnChildAdded(element);
			}
		}

		protected virtual void OnUpdate(int index, IView view, IView oldView)
		{
			NotifyHandler(nameof(ILayoutHandler.Update), index, view);

			// Make sure CascadeInputTransparent is applied, if necessary
			//Handler?.UpdateValue(nameof(CascadeInputTransparent));
		}

		void NotifyHandler(string action, int index, IView view)
		{
			Handler?.Invoke(action, new Maui.Handlers.LayoutHandlerUpdate(index, view));
		}

		void IPaddingElement.OnPaddingPropertyChanged(Thickness oldValue, Thickness newValue)
		{
			(this as IView).InvalidateMeasure();
		}

		Thickness IPaddingElement.PaddingDefaultValueCreator()
		{
			return new Thickness(0);
		}

		public static IPropertyMapper<IView, IViewHandler> ControlsLayoutMapper = new PropertyMapper<Layout, LayoutHandler>(ControlsVisualElementMapper)
		{
#if !__GTK__
			[nameof(CascadeInputTransparent)] = MapInputTransparent,
			[nameof(IView.InputTransparent)] = MapInputTransparent,
#endif
		};

		void UpdateDescendantInputTransparent()
		{
			if (!InputTransparent)
			{
				// We only need to propagate values if the layout is InputTransparent AND Cascade is true
				return;
			}

			// Set all the child InputTransparent values to match this one
			for (int n = 0; n < Count; n++)
			{
				if (this[n] is VisualElement visualElement)
				{
					visualElement.InputTransparent = true;
				}
			}
		}
	}
}
