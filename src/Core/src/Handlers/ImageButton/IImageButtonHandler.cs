﻿#if __IOS__ || MACCATALYST
using PlatformView = UIKit.UIButton;
#elif MONOANDROID
using PlatformView = Google.Android.Material.ImageView.ShapeableImageView;
#elif WINDOWS
#if __GTK__
using PlatformView = Microsoft.Maui.Platform.MauiImageButton;
#else
using PlatformView = Microsoft.UI.Xaml.Controls.Button;
#endif
#elif TIZEN
using PlatformView = Microsoft.Maui.Platform.MauiImageButton;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID && !TIZEN)
using PlatformView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public interface IImageButtonHandler : IImageHandler
	{
		new IImageButton VirtualView { get; }
		new PlatformView PlatformView { get; }
	}
}