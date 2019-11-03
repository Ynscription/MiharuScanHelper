
using System.IO;
using System.Windows;

namespace Manga_Scan_Helper.FrontEnd {
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window {
		public AboutDialog () {
			InitializeComponent();
		}

		private readonly static string ScanHelperLicenseFile = @".\Resources\Licenses\Scan Helper LICENSE";

		private readonly static string NewtosoftLicenseFile = @".\Resources\Licenses\Newtonsoft.Json LICENSE.md";
		private readonly static string OokiiLicenseFile = @".\Resources\Licenses\Ookii.Dialogs  LICENSE";
		
		private readonly static string TesseractLicenseFile = @".\Resources\Licenses\Tesseract OCR LICENSE";
		

		private void ShowFileInExplorer (string file) {
			if (!File.Exists(file))
				return;
			string argument = "/select, \"" + file + "\"";
			System.Diagnostics.Process.Start("explorer.exe", argument);
		}

		private void ScanHelperLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(ScanHelperLicenseFile);
		}

		private void NewtonsoftLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(NewtosoftLicenseFile);
		}

		private void OokiiLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(OokiiLicenseFile);
		}

		private void TesseractLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(TesseractLicenseFile);
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
