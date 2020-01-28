using Manga_Scan_Helper.BackEnd;
using Ookii.Dialogs.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Manga_Scan_Helper.FrontEnd
{
	/// <summary>
	/// Interaction logic for EditChapterWindow.xaml
	/// </summary>
	public partial class EditChapterWindow : Window
	{
		private Chapter _loadedChapter = null;
		public int SelectedIndex {
			get;
			private set;
		}

		private void UpdateButtons () {
			int index = PagesListBox.SelectedIndex;
			UpPageButton.IsEnabled = index > 0;
			DownPageButton.IsEnabled = index < _loadedChapter.TotalPages -1 && index >= 0;
			DelPageButton.IsEnabled = _loadedChapter.TotalPages > 1 && index >= 0;
			SelectedIndex = index >= 0 ? index : 0;
		}

		public EditChapterWindow (Chapter loadedChapter, int currentPage)
		{
			InitializeComponent();
			_loadedChapter = loadedChapter;
			
			PagesListBox.ItemsSource = _loadedChapter.Pages;
			PagesListBox.Items.Refresh();
			PagesListBox.SelectedIndex = currentPage;
			
			UpdateButtons();
		}

		private void AddPageButton_Click(object sender, RoutedEventArgs e)
		{
			//TODO open a dialog to choose 1 file and add it to the pages
			
			VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
			fileDialog.Multiselect = true;
			fileDialog.Title = "Select Image";
			fileDialog.AddExtension = true;
			fileDialog.CheckFileExists = true;
			fileDialog.CheckPathExists = true;
			fileDialog.DefaultExt = ".png";
			fileDialog.Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				int previousIndex = PagesListBox.SelectedIndex;
				try {
					Mouse.SetCursor(Cursors.Wait);
					int index = _loadedChapter.TotalPages -1;
					if (PagesListBox.SelectedIndex > -1)
						index = PagesListBox.SelectedIndex;
					
					foreach (string file in fileDialog.FileNames)
						_loadedChapter.AddPage(++index, file);		
					PagesListBox.Items.Refresh();
					PagesListBox.SelectedIndex = index;
					UpdateButtons();
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					PagesListBox.SelectedIndex = previousIndex;
					TaskDialog dialog = new TaskDialog();
					dialog.WindowTitle = "Error";
					dialog.MainIcon = TaskDialogIcon.Error;
					dialog.MainInstruction = "No images found.";
					dialog.Content = ex.Message;
					TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
					dialog.Buttons.Add(okButton);
					TaskDialogButton button = dialog.ShowDialog(this);
				
				}
			}
		}
		
		private void DelPageButton_Click(object sender, RoutedEventArgs e)
		{
			//TODO warn if page contains translations
			//TODO warn if about to delete last page
			int index = PagesListBox.SelectedIndex;
			_loadedChapter.RemovePage (index);
			PagesListBox.Items.Refresh();
			PagesListBox.SelectedIndex = (index >= _loadedChapter.TotalPages) ? _loadedChapter.TotalPages -1 : index;
			UpdateButtons();
			
		}


		private void UpPageButton_Click(object sender, RoutedEventArgs e)
		{
			
			_loadedChapter.MovePageUp(PagesListBox.SelectedIndex);
			PagesListBox.Items.Refresh();
			UpdateButtons();
		}

		private void DownPageButton_Click(object sender, RoutedEventArgs e)
		{
			_loadedChapter.MovePageDown(PagesListBox.SelectedIndex);
			PagesListBox.Items.Refresh();
			UpdateButtons();
		}
		

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void PagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateButtons();
		}
	}
}
