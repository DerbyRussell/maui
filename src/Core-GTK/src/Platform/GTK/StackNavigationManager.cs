﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Maui.Platform
{
	public class StackNavigationManager
	{
		IView? _currentPage;
		IMauiContext _mauiContext;
		// Frame? _navigationFrame;
		ContentViewGroup _navigationFrame;
		Action? _pendingNavigationFinished;

		protected NavigationRootManager WindowManager => _mauiContext.GetNavigationRootManager();
		internal IStackNavigation? NavigationView { get; private set; }
		public IReadOnlyList<IView> NavigationStack { get; set; } = new List<IView>();
		public IMauiContext MauiContext => _mauiContext;
		public IView CurrentPage
			=> _currentPage ?? throw new InvalidOperationException("CurrentPage cannot be null");
		//public Frame NavigationFrame =>
		//	_navigationFrame ?? throw new InvalidOperationException("NavigationFrame Null");

		public ContentViewGroup NavigationFrame =>
			_navigationFrame ?? throw new InvalidOperationException("NavigationFrame Null");

		public StackNavigationManager(ContentViewGroup view, IMauiContext mauiContext)
		{
			_navigationFrame = view;
			_mauiContext = mauiContext;
		}

		public virtual void Connect(IStackNavigation navigationView, ContentViewGroup navigationFrame)
		{
			//if (_navigationFrame != null)
			//	_navigationFrame.Navigated -= OnNavigated;

			FirePendingNavigationFinished();

			//navigationFrame.Navigated += OnNavigated;
			//_navigationFrame = navigationFrame;
			NavigationView = (IStackNavigation)navigationView;

			//if (WindowManager?.RootView is NavigationView nv)
			//	nv.IsPaneVisible = true;
		}

		public virtual void Disconnect(IStackNavigation navigationView, ContentViewGroup navigationFrame)
		{
			//if (_navigationFrame != null)
			//	_navigationFrame.Navigated -= OnNavigated;

			FirePendingNavigationFinished();
			// _navigationFrame = null;
			NavigationView = null;
		}

		public virtual void NavigateTo(NavigationRequest args)
		{
			if (_navigationFrame == null)
			{
				return;
			}

			IReadOnlyList<IView> newPageStack = new List<IView>(args.NavigationStack);
			var previousNavigationStack = NavigationStack;
			var previousNavigationStackCount = previousNavigationStack.Count;
			bool initialNavigation = NavigationStack.Count == 0;

			// User has modified navigation stack but not the currently visible page
			// So we just sync the elements in the stack
			if (!initialNavigation &&
				newPageStack[newPageStack.Count - 1] ==
				previousNavigationStack[previousNavigationStackCount - 1])
			{
				//SyncBackStackToNavigationStack(newPageStack);
				//NavigationStack = newPageStack;
				//FireNavigationFinished();
				return;
			}

			// NavigationTransitionInfo? transition = GetNavigationTransition(args);
			_currentPage = newPageStack[newPageStack.Count - 1];

			_ = _currentPage ?? throw new InvalidOperationException("Navigation Request Contains Null Elements");
			if (previousNavigationStack.Count < args.NavigationStack.Count)
			{
				// Type destinationPageType = GetDestinationPageType();
				NavigationStack = newPageStack;
				// NavigationFrame.Navigate(destinationPageType, null, transition);
				_navigationFrame.RemoveChild();
				if (_currentPage is IContentView contentView)
				{
					if (contentView.PresentedContent is IView view)
					{
						_navigationFrame.AddChild(view.ToPlatform(_mauiContext));
					}
				}
			}
			else if (previousNavigationStack.Count == args.NavigationStack.Count)
			{
				// Type destinationPageType = GetDestinationPageType();
				NavigationStack = newPageStack;
				// NavigationFrame.Navigate(destinationPageType, null, transition);
				_navigationFrame.RemoveChild();
				if (_currentPage is IContentView contentView)
				{
					if (contentView.PresentedContent is IView view)
					{
						_navigationFrame.AddChild(view.ToPlatform(_mauiContext));
					}
				}
			}
			else
			{
				NavigationStack = newPageStack;
				_navigationFrame.RemoveChild();
				if (_currentPage is IContentView contentView)
				{
					if (contentView.PresentedContent is IView view)
					{
						_navigationFrame.AddChild(view.ToPlatform(_mauiContext));
					}
				}
				// NavigationFrame.GoBack(transition);
			}

			FireNavigationFinished();
		}

		//protected virtual Type GetDestinationPageType() =>
		//	typeof(Page);

		//protected virtual NavigationTransitionInfo? GetNavigationTransition(NavigationRequest args)
		//{
		//	if (!args.Animated)
		//		return null;

		//	// GoBack just plays the animation in reverse so we always just return the same animation
		//	return new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
		//}

		void SyncBackStackToNavigationStack(IReadOnlyList<IView> pageStack)
		{
			//// Back stack depth doesn't count the currently visible page
			//var nativeStackCount = NavigationFrame.BackStackDepth + 1;

			//// BackStack entries have no hard relationship with a specific IView
			//// Everytime an entry is about to become visible it just grabs whatever
			//// IView is going to be the visible so all we're doing here is syncing
			//// up the number of things on the stack
			//while (nativeStackCount != pageStack.Count)
			//{
			//	if (nativeStackCount > pageStack.Count)
			//	{
			//		NavigationFrame.BackStack.RemoveAt(0);
			//	}
			//	else
			//	{
			//		NavigationFrame.BackStack.Insert(
			//			0, new PageStackEntry(GetDestinationPageType(), null, null));
			//	}

			//	nativeStackCount = NavigationFrame.BackStackDepth + 1;
			//}
		}

		//// This is used to fire NavigationFinished back to the xplat view
		//// Firing NavigationFinished from Loaded is the latest reliable point
		//// in time that I know of for firing `NavigationFinished`
		//// Ideally we could fire it when the `NavigationTransitionInfo` is done but
		//// I haven't found a way to do that
		//void OnNavigated(object sender, UI.Xaml.Navigation.NavigationEventArgs e)
		//{
		//	// If the user has inserted or removed any extra pages
		//	SyncBackStackToNavigationStack(NavigationStack);

		//	if (e.Content is not FrameworkElement fe)
		//		return;

		//	if (e.Content is not Page page)
		//		return;

		//	var nv = NavigationView;
		//	ContentPresenter? presenter;

		//	if (page.Content == null)
		//	{
		//		presenter = new ContentPresenter()
		//		{
		//			HorizontalAlignment = UI.Xaml.HorizontalAlignment.Stretch,
		//			VerticalAlignment = UI.Xaml.VerticalAlignment.Stretch
		//		};

		//		page.Content = presenter;
		//	}
		//	else
		//	{
		//		presenter = page.Content as ContentPresenter;
		//	}

		//	// At this point if the Content isn't a ContentPresenter the user has replaced
		//	// the conent so we just let them take control
		//	if (presenter == null || _currentPage == null)
		//		return;

		//	try
		//	{
		//		presenter.Content = _currentPage.ToPlatform(MauiContext);
		//	}
		//	catch (Exception)
		//	{
		//		FireNavigationFinished();
		//		throw;
		//	}

		//	_pendingNavigationFinished = () =>
		//	{
		//		if (presenter?.Content is not FrameworkElement pc)
		//		{
		//			FireNavigationFinished();
		//		}
		//		else
		//		{
		//			pc.OnLoaded(FireNavigationFinished);
		//		}

		//		if (NavigationView is IView view)
		//		{
		//			view.Arrange(fe);
		//		}
		//	};

		//	fe.OnLoaded(FirePendingNavigationFinished);
		//}

		void FireNavigationFinished()
		{
			_pendingNavigationFinished = null;
			NavigationView?.NavigationFinished(NavigationStack);
		}

		void FirePendingNavigationFinished()
		{
			Interlocked.Exchange(ref _pendingNavigationFinished, null)?.Invoke();
		}
	}
}