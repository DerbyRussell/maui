﻿using System;
using System.Threading;
#if __IOS__ || MACCATALYST
using PlatformView = UIKit.UIImageView;
#elif MONOANDROID
using PlatformView = Android.Widget.ImageView;
#elif WINDOWS
#if __GTK__
using PlatformView = Gtk.Image;
#else
using PlatformView = Microsoft.UI.Xaml.Controls.Image;
#endif
#elif TIZEN
using PlatformView = Tizen.UIExtensions.ElmSharp.Image;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID && !TIZEN)
using PlatformView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public partial class ImageHandler : IImageHandler
	{
		public static IPropertyMapper<IImage, IImageHandler> Mapper = new PropertyMapper<IImage, IImageHandler>(ViewHandler.ViewMapper)
		{
#if __ANDROID__ || WINDOWS || TIZEN
			[nameof(IImage.Background)] = MapBackground,
#endif
			[nameof(IImage.Aspect)] = MapAspect,
			[nameof(IImage.IsAnimationPlaying)] = MapIsAnimationPlaying,
			[nameof(IImage.Source)] = MapSource,
		};

		public static CommandMapper<IImage, IImageHandler> CommandMapper = new(ViewHandler.ViewCommandMapper)
		{
		};

		ImageSourcePartLoader? _imageSourcePartLoader;
		public ImageSourcePartLoader SourceLoader =>
			_imageSourcePartLoader ??= new ImageSourcePartLoader(this, () => VirtualView, OnSetImageSource);

		public ImageHandler() : base(Mapper)
		{
		}

		public ImageHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
		{
		}

		// TODO MAUI: Should we remove all shadowing? 
		IImage IImageHandler.VirtualView => VirtualView;

#if __GTK__
		PlatformView IImageHandler.PlatformView => PlatformView.ImageWidget;
#else
		PlatformView IImageHandler.PlatformView => PlatformView;
#endif
	}
}
