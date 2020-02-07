
using MahApps.Metro.Controls;
using System.IO;
using System.Windows;

namespace Miharu.FrontEnd.Helper {
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : MetroWindow {
		public AboutDialog () {
			InitializeComponent();
			
		}

		private readonly string MiharuLicenseFile = @".\Resources\Licenses\Miharu LICENSE";

		private readonly static string NewtosoftLicenseFile = @".\Resources\Licenses\Newtonsoft.Json LICENSE.md";
		private readonly static string OokiiLicenseFile = @".\Resources\Licenses\Ookii.Dialogs  LICENSE";
		private readonly static string WpfGifLicenseFile = @".\Resources\Licenses\WpfAnimatedGif LICENSE";
		private readonly static string SeleniumLicenseFile = @".\Resources\Licenses\Selenium LICENSE";
		private readonly static string MahAppsMetroLicenseFile = @".\Resources\Licenses\MahApps.Metro LICENSE";
		private readonly static string MahAppsMetroIconPacksLicenseFile = @".\Resources\Licenses\MahApps.Metro.IconPacks LICENSE";
		
		private readonly static string TesseractLicenseFile = @".\Resources\Licenses\Tesseract OCR LICENSE";
		private readonly static string GeckoLicenseFile = @".\Resources\Licenses\GeckoDriver LICENSE";



		

		private void ShowFileInExplorer (string file) {
			if (!File.Exists(file))
				return;
			string argument = "/select, \"" + file + "\"";
			System.Diagnostics.Process.Start("explorer.exe", argument);
		}

				

		private void Button_Click (object sender, RoutedEventArgs e) {
			Close();
		}

		int counter = 0;
		private void Rectangle_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (counter >= 3){
				counter = 0;
				ImageDisplay id = new ImageDisplay (@".\Resources\Graphics\CnC_EE.png");
				id.Owner = this;
				id.ShowDialog();
			} else
				counter++;
		}

		
	}
}
