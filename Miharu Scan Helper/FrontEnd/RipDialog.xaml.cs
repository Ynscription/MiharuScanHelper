using Ookii.Dialogs.Wpf;
using System;
using System.Windows;

namespace Miharu.FrontEnd {
	/// <summary>
	/// Interaction logic for RipDialog.xaml
	/// </summary>
	public partial class RipDialog : Window {
		public bool Success {
			get; private set;
		}

		public string File {
			get; private set;
		}

		public string DestinationPath {
			get; private set;
		}


		public RipDialog () {
			InitializeComponent();
			Success = false;
		}

		private void BrowseFolderButton_Click (object sender, RoutedEventArgs e) {
			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			bool? res = folderDialog.ShowDialog(this);
			if (res ?? false) {
				FolderTextBox.Text = folderDialog.SelectedPath;
			}
		}

		

		private void CancelButton_Click (object sender, RoutedEventArgs e) {
			Close();
		}

		private void RipButton_Click (object sender, RoutedEventArgs e) {
			try {
				File = HTMLFileTextBox.Text;
				DestinationPath = FolderTextBox.Text;
				Success = true;
				Close();
			}
			catch (Exception ex) {
				TaskDialog dialog = new TaskDialog();
				dialog.WindowTitle = "Error";
				dialog.MainIcon = TaskDialogIcon.Error;
				dialog.MainInstruction = "Invalid input.";
				dialog.Content = ex.Message;
				TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
				dialog.Buttons.Add(okButton);
				TaskDialogButton button = dialog.ShowDialog(this);
			}
		}

		private void HTMLFileButton_Click(object sender, RoutedEventArgs e)
		{
			VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.CheckFileExists = true;
			fileDialog.CheckPathExists = true;
			fileDialog.DefaultExt = ".html";
			fileDialog.Filter = "HTML files (*.html)|*.html";
			fileDialog.Multiselect = false;
			fileDialog.Title = "Choose source HTML";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				HTMLFileTextBox.Text = fileDialog.FileName;
			}
		}
	}
}
