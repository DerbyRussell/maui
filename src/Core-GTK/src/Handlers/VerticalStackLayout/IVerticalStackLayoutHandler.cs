﻿namespace Microsoft.Maui
{
	public interface IVerticalStackLayoutHandler : IViewHandler
	{
		new ILayout VirtualView { get; }
		new System.Object PlatformView { get; }

		void Add(IView view);
		void Remove(IView view);
		void Clear();
		void Insert(int index, IView view);
		void Update(int index, IView view);
		void UpdateZIndex(IView view);
	}
}