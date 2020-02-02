using Manga_Scan_Helper.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;

namespace Manga_Scan_Helper.FrontEnd
{
	/// <summary>
	/// Interaction logic for PreferencesDialog.xaml
	/// </summary>
	public partial class PreferencesDialog : Window
	{

		private string _tesseractPath;
		public string TesseractPath {
			get => _tesseractPath;
			private set {
				_tesseractPath = value;
				ApplyButton.IsEnabled = true;
			}
		}

		public PreferencesDialog()
		{
			InitializeComponent();
			TesseractPathTextBox.Text = (string)Settings.Default["TesseractPath"];
			ApplyButton.IsEnabled = false;
			
		}

		private void TesseractPathButton_Click(object sender, RoutedEventArgs e)
		{
			VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.CheckFileExists = true;
			fileDialog.CheckPathExists = true;
			fileDialog.DefaultExt = ".exe";
			fileDialog.Filter = "Tesseract (tesseract.exe)|tesseract.exe";
			fileDialog.Multiselect = false;
			fileDialog.Title = "Select Tesseract Executable";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false)
				TesseractPath = fileDialog.FileName;
				
		}

		private void TesseractPathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			TesseractPath = TesseractPathTextBox.Text;			
		}

		public void WarnBadPath (string reason) {

		}

		private string _failReason;
		private bool CheckTesseractPath () {
			bool res = false;
			if (res = File.Exists(TesseractPath)) {
				FileInfo fi = new FileInfo(TesseractPath);
				if(!(res = fi.Extension == ".exe"))
					_failReason = "File must be an executable (Extension .exe)";
			}
			else
				_failReason = "Could not find file at " + TesseractPath;
			return res;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckTesseractPath()) {
				Settings.Default ["TesseractPath"] = TesseractPath;
				Settings.Default.Save();
				Close();
			}
			else
				WarnBadPath(_failReason);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckTesseractPath()) {
				Settings.Default ["TesseractPath"] = TesseractPath;
				Settings.Default.Save();
				ApplyButton.IsEnabled = false;
			}
			else
				WarnBadPath(_failReason);
		}
	}
}
