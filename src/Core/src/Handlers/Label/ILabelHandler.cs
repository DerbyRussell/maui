﻿#if __IOS__ || MACCATALYST
using PlatformView = Microsoft.Maui.Platform.MauiLabel;
#elif MONOANDROID
using PlatformView = AndroidX.AppCompat.Widget.AppCompatTextView;
#elif WINDOWS
#if __GTK__
using PlatformView = Microsoft.Maui.Platform.CustomAltView;
#else
using PlatformView = Microsoft.UI.Xaml.Controls.TextBlock;
#endif
#elif TIZEN
using PlatformView = Tizen.UIExtensions.ElmSharp.Label;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !TIZEN)
using PlatformView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
#if __GTK__
	public partial interface ILabelHandler : IAltViewHandler
#else
	public partial interface ILabelHandler : IViewHandler
#endif
	{
		new ILabel VirtualView { get; }
		new PlatformView PlatformView { get; }
	}
}