using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd.Helper
{
	/// <summary>
	/// Interaction logic for CaptchaDialog.xaml
	/// </summary>
	public partial class CaptchaDialog : MetroWindow
	{

		public string CaptchaInput {
			get; private set;
		}

		public CaptchaDialog(string imgSrc)
		{
			InitializeComponent();

			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.UriSource = new Uri(imgSrc, UriKind.Absolute);
			bi.EndInit();

			CaptchaImage.Source = bi;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			CaptchaInput = SolveTextBox.Text;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			CaptchaInput = null;
			Close();
		}

		private void SolveTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				CaptchaInput = SolveTextBox.Text;
				Close();
			}
			else if (e.Key == Key.Escape) {
				CaptchaInput = null;
				Close();
			}
		}
	}
}
