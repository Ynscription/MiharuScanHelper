using MahApps.Metro.Controls;
using System;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd.Helper {
	/// <summary>
	/// Interaction logic for ImageDisplay.xaml
	/// </summary>
	public partial class ImageDisplay : MetroWindow {
		public ImageDisplay (string src) {
			InitializeComponent();
			BitmapImage imgSrc = new BitmapImage();
			imgSrc.BeginInit();
			imgSrc.UriSource = new Uri (src, UriKind.Relative);
			imgSrc.CacheOption = BitmapCacheOption.OnLoad;
			imgSrc.EndInit();
			DisplayImage.Source = imgSrc;
			DisplayImage.Width = imgSrc.Width;
			DisplayImage.Height = imgSrc.Height;
		}
	}
}
