
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Miharu.FrontEnd {
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window {
		public AboutDialog () {
			InitializeComponent();
		}

		private readonly static string MiharuLicenseFile = @".\Resources\Licenses\Miharu LICENSE";

		private readonly static string NewtosoftLicenseFile = @".\Resources\Licenses\Newtonsoft.Json LICENSE.md";
		private readonly static string OokiiLicenseFile = @".\Resources\Licenses\Ookii.Dialogs  LICENSE";
		private readonly static string TesseractLicenseFile = @".\Resources\Licenses\Tesseract OCR LICENSE";
		
		private readonly static string WpfGifLicenseFile = @".\Resources\Licenses\WpfAnimatedGif LICENSE";
		private readonly static string SeleniumLicenseFile = @".\Resources\Licenses\Selenium LICENSE";
		private readonly static string GeckoLicenseFile = @".\Resources\Licenses\GeckoDriver LICENSE";



		

		private void ShowFileInExplorer (string file) {
			if (!File.Exists(file))
				return;
			string argument = "/select, \"" + file + "\"";
			System.Diagnostics.Process.Start("explorer.exe", argument);
		}

		private void MiharuLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(MiharuLicenseFile);
		}

		private void NewtonsoftLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(NewtosoftLicenseFile);
		}

		private void OokiiLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(OokiiLicenseFile);
		}


		private void WpfGifLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(WpfGifLicenseFile);
		}

		private void TesseractLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(TesseractLicenseFile);
		}

		private void SeleniumLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(SeleniumLicenseFile);
		}

		private void GeckoLicense_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ShowFileInExplorer(GeckoLicenseFile);
		}


		private void LinkLabel_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			System.Diagnostics.Process.Start((string)((Label)sender).Content);
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
