using MahApps.Metro.Controls;
using System;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd {
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
			Width = imgSrc.Width + 5;
			Height = imgSrc.Height + 29;
			DisplayImage.Source = imgSrc;
		}
	}
}
