using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manga_Scan_Helper.FrontEnd {
	/// <summary>
	/// Interaction logic for ImageDisplay.xaml
	/// </summary>
	public partial class ImageDisplay : Window {
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
