using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;

namespace Miharu.BackEnd.Data
{
	public static class SaveUpdater
	{
		private const string _OVERWRITE = "Overwrite";
		private const string _NEW_FILE = "New File";
		private const string _CANCEL = "Cancel";

		private static string WarnUpdateRequired () {
			TaskDialog dialog = new TaskDialog();
			dialog.WindowTitle = "Warning";
			dialog.MainIcon = TaskDialogIcon.Warning;
			dialog.MainInstruction = "The file was saved in an old format.";
			dialog.Content = "What would you like to do?";
			
			TaskDialogButton saveButton = new TaskDialogButton(_OVERWRITE);
			dialog.Buttons.Add(saveButton);
			TaskDialogButton noSaveButton = new TaskDialogButton(_NEW_FILE);
			dialog.Buttons.Add(noSaveButton);
			TaskDialogButton cancelButton = new TaskDialogButton(_CANCEL);
			dialog.Buttons.Add(cancelButton);
			TaskDialogButton button = dialog.ShowDialog();

			return button.Text;
		}

		private static void Overwrite (string source, string data, int page, int versionNumber) {
			using (StreamWriter writer = new StreamWriter(source)){
				writer.WriteLine((string)Settings.Default["SaveVersion"]);
				writer.WriteLine(page);
				if (versionNumber == 1 || versionNumber == 2)
					writer.Write(ConvertFrom2to3(data));
			}
		}

		private static string NewFile () {
			VistaSaveFileDialog fileDialog = new VistaSaveFileDialog();
			fileDialog.AddExtension = true;
			fileDialog.DefaultExt = ".scan";
			fileDialog.Filter = "Scans files (*.scan)|*.scan";
			fileDialog.OverwritePrompt = true;
			fileDialog.Title = "Save Updated Chapter";
			bool? res = fileDialog.ShowDialog();
			if (res ?? false)
				return fileDialog.FileName;
			return null;
		}

		public static string CheckSaveVersion (string source) {
			string finalSource = source;
			
			using(StreamReader reader = new StreamReader(source)) {
				string version = reader.ReadLine();
				int page = 0;
				bool update = false;
				int versionNumber = 0;
				
				if (update = version.StartsWith("{"))
					versionNumber = 1;
				else if (update = !version.StartsWith("v")) {
					page = int.Parse(version);
					versionNumber = 2;
				}
				else if (update = version != (string)Settings.Default["SaveVersion"]) {
					page = int.Parse(reader.ReadLine());
					versionNumber = int.Parse(version.Substring(1));
				}

				if (update) {
					string warnRes = WarnUpdateRequired();
					if (warnRes == _OVERWRITE) {
						string data = reader.ReadToEnd();
						reader.Close();
						Overwrite(source, data, page, versionNumber);
					}
					else if (warnRes == _CANCEL)
						throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
					else {
						if ((finalSource = NewFile()) == null)
							throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
						using (StreamWriter writer = new StreamWriter(finalSource)){
							writer.WriteLine((string)Settings.Default["SaveVersion"]);
							writer.WriteLine(page);
							while(!reader.EndOfStream) {
								writer.WriteLine(ConvertFrom2to3(reader.ReadLine()));
							}
						}
					}
				}
			}

			return finalSource;
		}

		private static string ConvertFrom2to3 (string data) {
			data = data.Replace("\"Google2\":", "\"Google_API\":");
			data = data.Replace("\"Bing\":", "\"Bing_API\":");
			data = data.Replace("\"Yandex\":", "\"Yandex_API\":");
			data = data.Replace("\"JadedNetwork\":", "\"Jaded_Network\":");
			return data;
		}

		
	}
}
