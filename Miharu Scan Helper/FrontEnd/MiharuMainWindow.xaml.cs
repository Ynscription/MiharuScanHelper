using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Miharu.BackEnd;
using Miharu.Control;
using Miharu.FrontEnd.Helper;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Miharu.FrontEnd
{
	/// <summary>
	/// Interaction logic for MiharuMainWindow.xaml
	/// </summary>
	public partial class MiharuMainWindow : MetroWindow
	{

		private readonly ChapterManager _chapterManager = null;

		public MiharuMainWindow(ChapterManager chapterManager)
		{
			InitializeComponent();
			_chapterManager = chapterManager;
			_chapterManager.ChapterChanged += OnChapterChanged;
			_chapterManager.SaveChanged += OnSaveChanged;

			
		}




		#region Events

		private void OnChapterChanged(object sender, EventArgs e)
		{
			SetTopMenuItems();
		}

		private void OnSaveChanged(object sender, EventArgs e)
		{
			try {
				Dispatcher.Invoke(() => {
					SetTopMenuItems();
					string title = "";
					if (_chapterManager.IsChapterLoaded) {
						if (_chapterManager.SaveFileExists) {
							FileInfo fi = new FileInfo(_chapterManager.CurrentSaveFile);
							title += fi.Name.Replace(fi.Extension, "");
						}
						else
							title += "untitled";
						if (!_chapterManager.IsChapterSaved)
							title += "*";
						title += " - Miharu Scan Helper";
					}
					else
						title += "Miharu Scan Helper";
					Title = title;
				});
			}
			catch (TaskCanceledException) { }
		}


		#endregion

		private string GetResource (string key) {
			return (string)Application.Current.TryFindResource(key) ?? "";
		}



		private void SetTopMenuItems () {
			bool set = _chapterManager.IsChapterLoaded;

			SaveChapterMenuItem.IsEnabled = set;
			SaveAsChapterMenuItem.IsEnabled = set;

			CloseChapterMenuItem.IsEnabled = set;

			ExportAsTSScriptMenuItem.IsEnabled = set;

			ExportAsJPScriptMenuItem.IsEnabled = set;

			ExportAsRScriptMenuItem.IsEnabled = set;

			EditChapterPagesMenuItem.IsEnabled = set;

		}

		private string WarnNotSaved () {
			string res = "";

			using (TaskDialog dialog = new TaskDialog()) {
				dialog.WindowTitle = "Warning";
				dialog.MainIcon = TaskDialogIcon.Warning;
				dialog.MainInstruction = "There are unsaved changes.";
				dialog.Content = "Would you like to save the current chapter?";

				Application app = Application.Current;
				TaskDialogButton saveButton = new TaskDialogButton(GetResource("SaveButtonText"));
				dialog.Buttons.Add(saveButton);
				TaskDialogButton discardSaveButton = new TaskDialogButton(GetResource("DiscardButtonText"));
				dialog.Buttons.Add(discardSaveButton);
				TaskDialogButton cancelButton = new TaskDialogButton(GetResource("CancelButtonText"));
				dialog.Buttons.Add(cancelButton);
				TaskDialogButton button = dialog.ShowDialog(this);


				res  = button.Text;				
			}
			return res;
		}

		private void DoSave (string saveFile = null) {
			try {
				Mouse.SetCursor(Cursors.Wait);
				_chapterManager.SaveChapter(saveFile);
				Mouse.SetCursor(Cursors.Arrow);
			}
			catch (Exception ex) {
				Mouse.SetCursor(Cursors.Arrow);				
				using (TaskDialog dialog = new TaskDialog()) {
					dialog.WindowTitle = "Error";
					dialog.MainIcon = TaskDialogIcon.Error;
					dialog.MainInstruction = "There was an error saving the chapter.";
					dialog.Content = ex.Message;
					TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
					dialog.Buttons.Add(okButton);
					TaskDialogButton button = dialog.ShowDialog(this);
				}
			}
		}

		
		
		
		
		
		
		private async void NewChapterFolderMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string saveRes = WarnNotSaved();
				if (saveRes == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (saveRes == GetResource("CancelButtonText")) {
					return;
				}
			}


			VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
			bool? res = folderDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					Mouse.SetCursor(Cursors.Wait);
					_chapterManager.NewChapter(folderDialog.SelectedPath);
					GC.Collect();
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error loading the chapter.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}
		
		private void NewChapterFilesMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string saveRes = WarnNotSaved();
				if (saveRes == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (saveRes == GetResource("CancelButtonText")) {
					return;
				}
			}

			VistaOpenFileDialog filesDialog = new VistaOpenFileDialog();
			filesDialog.Multiselect = true;
			filesDialog.Title = "Select Images";
			filesDialog.AddExtension = true;
			filesDialog.CheckFileExists = true;
			filesDialog.CheckPathExists = true;
			filesDialog.DefaultExt = ".png";
			filesDialog.Filter = "Images (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
			bool? res = filesDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					Mouse.SetCursor(Cursors.Wait);
					_chapterManager.NewChapter(filesDialog.FileNames);
					GC.Collect();
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error loading the chapter.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}

		private void OpenChapterMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string saveRes = WarnNotSaved();
				if (saveRes == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (saveRes == GetResource("CancelButtonText")) {
					return;
				}
			}

			VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.CheckFileExists = true;
			fileDialog.CheckPathExists = true;
			fileDialog.DefaultExt = ".scan";
			fileDialog.Filter = "Scans files (*.scan)|*.scan";
			fileDialog.Multiselect = false;
			fileDialog.Title = "Open Chapter";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					_chapterManager.LoadChapter(fileDialog.FileName);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error loading the chapter.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}

		private void CloseChapterMenuItem_Click (object sender, RoutedEventArgs e) {
			if (!_chapterManager.IsChapterSaved) {
				string res = WarnNotSaved();
				if (res == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (res == GetResource("CancelButtonText")) {
					return;
				}
			}

			Mouse.SetCursor(Cursors.Wait);
			_chapterManager.UnloadChapter();
			GC.Collect();
			Mouse.SetCursor(Cursors.Arrow);
		}

		
		
		private void SaveAsChapterMenuItem_Click (object sender, RoutedEventArgs e) {
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".scan";
			fileDialog.Filter = "Scans files (*.scan)|*.scan";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Save Chapter";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				DoSave(fileDialog.FileName);
			}

		}

		private void SaveChapterMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.SaveFileExists)
				DoSave();
			else
				SaveAsChapterMenuItem_Click(sender, e);

		}



		private void ExportAsScriptMenuItem_Click (object sender, RoutedEventArgs e) {
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".txt";
			fileDialog.Filter = "Script files (*.txt)|*.txt";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Export Script";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					Mouse.SetCursor(Cursors.Wait);
					_chapterManager.ExportScript(fileDialog.FileName);
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error exporting the script.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}

		private void ExportAsJPScriptMenuItem_Click (object sender, RoutedEventArgs e) {
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".txt";
			fileDialog.Filter = "Transcription files (*.txt)|*.txt";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Export Transcription";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					Mouse.SetCursor(Cursors.Wait);
					_chapterManager.ExportJPScript(fileDialog.FileName);
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error exporting the transcription.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}

		private void ExportAsCompleteMenuItem_Click (object sender, RoutedEventArgs e) {
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".txt";
			fileDialog.Filter = "Script files (*.txt)|*.txt";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Export Complete Script";
			bool? res = fileDialog.ShowDialog(this);
			if (res ?? false) {
				try {
					Mouse.SetCursor(Cursors.Wait);					
					_chapterManager.ExportCompleteScript(fileDialog.FileName);
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);

					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error exporting the complete script.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}



		private void ExitMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string res = WarnNotSaved();
				if (res == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (res == GetResource("CancelButtonText")) {
					return;
				}
			}

			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			Application.Current.Shutdown(0);
		}




		private void EditChapterPagesMenuItem_Click (object sender, RoutedEventArgs e) {
			if (!_chapterManager.AllPagesReady) {
				Mouse.SetCursor(Cursors.Wait);
				_chapterManager.WaitForPages();
				Mouse.SetCursor(Cursors.Arrow);
			}
			EditChapterWindow editPagesDialog = new EditChapterWindow(_chapterManager);
			editPagesDialog.Owner = this;
			editPagesDialog.ShowDialog();
			_chapterManager.ReloadChapter(editPagesDialog.SelectedIndex);
			GC.Collect();
		}
			   
		private void PreferencesMenuItem_Click (object sender, RoutedEventArgs e) {
			PreferencesDialog pd = new PreferencesDialog();
			pd.Owner = this;
			pd.ShowDialog();
		}




		private void RipMenuItem_Click (object sender, RoutedEventArgs e) {
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string saveRes = WarnNotSaved();
				if (saveRes == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (saveRes == GetResource("CancelButtonText")) {
					return;
				}
			}

			RipDialog ripDialog = new RipDialog();
			ripDialog.Owner = this;
			ripDialog.ShowDialog();
			if (ripDialog.Success) {
				try {
					Mouse.SetCursor(Cursors.Wait);
					string dest = Ripper.FileRip(ripDialog.File, ripDialog.DestinationPath);
					_chapterManager.NewChapter(dest);
					GC.Collect();
					Mouse.SetCursor(Cursors.Arrow);
				}
				catch (Ripper.RipperException ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error while ripping.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
				catch (Exception ex) {
					Mouse.SetCursor(Cursors.Arrow);
					using (TaskDialog dialog = new TaskDialog()) {
						dialog.WindowTitle = "Error";
						dialog.MainIcon = TaskDialogIcon.Error;
						dialog.MainInstruction = "There was an error opening the ripped images.";
						dialog.Content = ex.Message;
						TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
						dialog.Buttons.Add(okButton);
						TaskDialogButton button = dialog.ShowDialog(this);
					}
				}
			}
		}

		private void CrashSimulatorItemItem_Click(object sender, RoutedEventArgs e)
		{
			throw new Exception("This was a Simulated Crash");
		}

		
		
		
		private void AboutMenuItem_Click (object sender, RoutedEventArgs e) {
			AboutDialog aboutDialog = new AboutDialog();
			aboutDialog.Owner = this;
			aboutDialog.ShowDialog();
		}




#region Window Management
		
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.S 
				&& e.KeyboardDevice.Modifiers == ModifierKeys.Control 
				&& !_chapterManager.IsChapterSaved 
				&& _chapterManager.IsChapterLoaded)
				SaveChapterMenuItem_Click(sender, e);
		}
		
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
			if (_chapterManager.IsChapterLoaded && !_chapterManager.IsChapterSaved) {
				string res = WarnNotSaved();
				if (res == GetResource("SaveButtonText"))
					SaveChapterMenuItem_Click(sender, new RoutedEventArgs());
				else if (res == GetResource("CancelButtonText")) {
					e.Cancel = true;
					return;
				}
			}
		}




		#endregion

		
	}
}
