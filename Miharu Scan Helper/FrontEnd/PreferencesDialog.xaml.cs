using MahApps.Metro;
using MahApps.Metro.Controls;
using Miharu.BackEnd.Translation;
using Miharu.BackEnd.Data;
using Miharu.Control;
using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System;
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

		private ChapterManager _chapterManager;

		private bool _originalUseScreenDPI = false;

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

		public PreferencesDialog(TranslationManager translationManager, ChapterManager chapterManager)
		{
			InitializeComponent();

			_chapterManager = chapterManager;

			TesseractPathTextBox.Text = (string)Settings.Default["TesseractPath"];
			ApplyButton.IsEnabled = false;

			ThemeBaseColorListBox.ItemsSource = BaseColors;
			ThemeBaseColorListBox.SelectedItem = (string)Settings.Default["Theme"];
			ThemeAccentColorListBox.ItemsSource = AccentColors;
			ThemeAccentColorListBox.SelectedItem = (string)Settings.Default["Accent"];

			_originalUseScreenDPI = (bool)Settings.Default["UseScreenDPI"];
			UseScreenDPIToggleSwitch.IsChecked = _originalUseScreenDPI;			

			WarnTextDeletionToggleSwitch.IsChecked = (bool)Settings.Default["WarnTextDeletion"];

			AutoTranslateToggleSwitch.IsChecked = (bool)Settings.Default["AutoTranslateEnabled"];
			

			string disabledTypes = (string)Settings.Default["DisabledTranslationSources"];
			foreach(TranslationType t in translationManager.AvailableTranslations) {
				if (t.HasFlag(TranslationType.Text)) {
					ToggleSwitch ts = new ToggleSwitch ();
					ts.Content = t;
					ts.IsChecked = !disabledTypes.Contains(t.ToString());
					ts.IsEnabled = AutoTranslateToggleSwitch.IsChecked ?? true;
					ts.IsCheckedChanged += CheckChanged;
					TranslationSourcesStackPanel.Children.Add(ts);
				}
			}
			ApplyButton.IsEnabled = false;
			AutoTranslateToggleSwitch.IsCheckedChanged += OnAutoTranslateChackChange;
			
		}

		private void OnAutoTranslateChackChange(object sender, EventArgs e)
		{
			foreach (ToggleSwitch ts in TranslationSourcesStackPanel.Children)
				ts.IsEnabled = AutoTranslateToggleSwitch.IsChecked ?? true;
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

			Settings.Default["UseScreenDPI"] = UseScreenDPIToggleSwitch.IsChecked;

			Settings.Default["WarnTextDeletion"] = WarnTextDeletionToggleSwitch.IsChecked;

			Settings.Default["AutoTranslateEnabled"] = AutoTranslateToggleSwitch.IsChecked;


			string disabledSources = "";
			foreach (ToggleSwitch ts in TranslationSourcesStackPanel.Children) {
				if (!ts.IsChecked ?? true)
					disabledSources += ts.Content.ToString() + ";";
			}
			if (disabledSources.Length > 0)
				disabledSources = disabledSources.Substring(0, disabledSources.Length-1);
			Settings.Default["DisabledTranslationSources"] = disabledSources;
			Settings.Default.Save();

			if (_chapterManager.IsChapterLoaded && _originalUseScreenDPI != (UseScreenDPIToggleSwitch.IsChecked ?? false)) {
				_originalUseScreenDPI = UseScreenDPIToggleSwitch.IsChecked ?? false;
				Miharu.BackEnd.Data.Page.UseScreenDPI = _originalUseScreenDPI;
				_chapterManager.ReloadPages();
				_chapterManager.PageManager.ChangePage(_chapterManager.PageManager.CurrentPageIndex);
			}
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

		private void CheckChanged(object sender, EventArgs e)
		{
			ApplyButton.IsEnabled = true;
		}
	}
}
