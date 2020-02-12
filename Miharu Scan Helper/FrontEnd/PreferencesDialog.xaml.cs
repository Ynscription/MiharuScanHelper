using MahApps.Metro;
using MahApps.Metro.Controls;
using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;

namespace Miharu.FrontEnd
{
	/// <summary>
	/// Interaction logic for PreferencesDialog.xaml
	/// </summary>
	public partial class PreferencesDialog : MetroWindow
	{

		private readonly string [] BaseColors = { "Dark", "Light" };
		private readonly string [] AccentColors = { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };

		private string _tesseractPath;
		public string TesseractPath {
			get => _tesseractPath;
			private set {
				_tesseractPath = value;
				ApplyButton.IsEnabled = true;
			}
		}

		public string ThemeBaseColor {
			get;
			private set;
		} = null;

		public string ThemeAccentColor {
			get;
			private set;
		} = null;

		public PreferencesDialog()
		{
			InitializeComponent();
			TesseractPathTextBox.Text = (string)Settings.Default["TesseractPath"];
			ApplyButton.IsEnabled = false;
			ThemeBaseColorListBox.ItemsSource = BaseColors;
			ThemeBaseColorListBox.SelectedItem = (string)Settings.Default["Theme"];
			ThemeAccentColorListBox.ItemsSource = AccentColors;
			ThemeAccentColorListBox.SelectedItem = (string)Settings.Default["Accent"];
			AutoTranslateToggleSwitch.IsChecked = (bool)Settings.Default["AutoTranslateEnabled"];
			
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
			using (TaskDialog dialog = new TaskDialog()) {
				dialog.WindowTitle = "Error";
				dialog.MainIcon = TaskDialogIcon.Error;
				dialog.MainInstruction = "Can't apply changes.";
				dialog.Content = reason;
				TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
				dialog.Buttons.Add(okButton);
				TaskDialogButton button = dialog.ShowDialog(this);
			}
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

		private void SavePreferences () {
			Settings.Default ["TesseractPath"] = TesseractPath;
			Settings.Default["Theme"] = (string)ThemeBaseColorListBox.SelectedValue;
			Settings.Default["Accent"] = (string)ThemeAccentColorListBox.SelectedValue;
			Settings.Default["AutoTranslateEnabled"] = AutoTranslateToggleSwitch.IsChecked;
			Settings.Default.Save();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckTesseractPath()) {
				SavePreferences();
				ThemeManager.ChangeTheme(Application.Current, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
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
				SavePreferences();
				ThemeManager.ChangeTheme(Application.Current, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
				ApplyButton.IsEnabled = false;
			}
			else
				WarnBadPath(_failReason);
		}

		private void ThemeSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			ThemeManager.ChangeTheme(this, (string)ThemeBaseColorListBox.SelectedValue + "." + (string)ThemeAccentColorListBox.SelectedValue);
			ApplyButton.IsEnabled = true;
		}

	}
}
