using Ookii.Dialogs.Wpf;
using System;
using System.Windows;

namespace Manga_Scan_Helper.FrontEnd {
	/// <summary>
	/// Interaction logic for RipDialog.xaml
	/// </summary>
	public partial class RipDialog : Window {
		public bool Success {
			get; private set;
		}

		public string URL {
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
				URL = URLTextBox.Text;
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
	}
}
