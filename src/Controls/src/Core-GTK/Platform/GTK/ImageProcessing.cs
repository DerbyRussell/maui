using System;
using System.IO;
using Gdk;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Microsoft.Maui.Controls.Platform {
	public class ImageProcessing {
		private double _appScaler;

		public ImageProcessing() {

		}

		public ImageDimensions Dimensions { get; private set; }

		public event EventHandler CallBackGetNativeImage;

		private void StartGetNativeImage(ImageSource imageSource, string title, object obj) {
			if (imageSource == null || imageSource.IsEmpty) {
				return;
			}

			Dimensions = new ImageDimensions();
			Dimensions.Title = title;
			Dimensions.Obj = obj;


			var fileImageSource = (IFileImageSource)imageSource;

			if (fileImageSource != null) {
				var file = fileImageSource.File;
				if (!string.IsNullOrEmpty(file)) {
					var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

					if (File.Exists(imagePath)) {
						Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() => {
							Dimensions.PixbufReturned = new Pixbuf(imagePath);
							CallBackGetNativeImage.Invoke(this, new EventArgs());
						});
						//GLib.Idle.Add(
						//	delegate {
						//		{
						//			image = new Pixbuf(imagePath);
						//		};
						//		return false;
						//	}
						//);
					}
				}
			}

			//if (image == null)
			//	return null!;

			//return image;

			//////return Task.FromResult<IImageSourceServiceResult<Gdk.Pixbuf>?>(new ImageSourceServiceResult(image));

			////var handler = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(imageSource);
			////if (handler == null) {
			////	return null;
			////}

			////try {
			////	return await handler.LoadImageAsync(imageSource, cancellationToken).ConfigureAwait(false);
			////} catch (OperationCanceledException) {
			////	LogTool.GetInstance().Warning("Image loading", "Image load cancelled");
			////} catch (Exception ex) {
			////	LogTool.GetInstance().Warning("Image loading", $"Image load failed: {ex}");
			////}

			////return null;
		}

		//public ImageDimensions FinalApplyNativeImageDesigned(Action<Pixbuf> onSet) {
		//	_ = onSet ?? throw new ArgumentNullException(nameof(onSet));

		//	int nSizeHeight = Dimensions.PixbufReturned.Height;
		//	int nSizeWidth = Dimensions.PixbufReturned.Width;

		//	using (var drawableScaled = Dimensions.PixbufReturned.ScaleSimple(nSizeWidth, nSizeHeight, InterpType.Bilinear)) {
		//		// only set if we are still on the same image
		//		onSet(drawableScaled);
		//	}


		//	return Dimensions;
		//}

		public void StartApplyNativeImageFromFilename(string fileName, string title, object obj) {
			ImageSource imageSourceInternal = "Resources/Images/";
			imageSourceInternal += fileName;
			StartGetNativeImage(imageSourceInternal, title, obj);
		}

		public void StartApplyNativeImageFromSource(ImageSource source, string title, object obj)
		{
			StartGetNativeImage(source, title, obj);
		}
	}
}
