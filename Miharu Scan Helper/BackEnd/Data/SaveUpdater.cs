using Miharu.FrontEnd.Helper;
using Miharu.Properties;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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

		
		private static void Update (string destination, string data, int page, int versionNumber) {
			Chapter c = null;
			if (versionNumber < 3) {
				ConvertFrom2to3(ref data);				
			}
			if (versionNumber < 4) {
				c = ConvertFrom3to4(ref data, destination, page);
			}
			if (c != null)
				c.Save(destination, page);
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
						Update(source, data, page, versionNumber);
					}
					else if (warnRes == _CANCEL)
						throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
					else {
						if ((finalSource = NewFile()) == null)
							throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
						Update(finalSource, reader.ReadToEnd(), page, versionNumber);
					}
				}
			}

			return finalSource;
		}

		private static void ConvertFrom2to3 (ref string data) {
			data = data.Replace("\"Google2\":", "\"Google_API\":");
			data = data.Replace("\"Bing\":", "\"Bing_API\":");
			data = data.Replace("\"Yandex\":", "\"Yandex_API\":");
			data = data.Replace("\"JadedNetwork\":", "\"Jaded_Network\":");
		}

		private static Chapter ConvertFrom3to4 (ref string data, string destination, int page) {

			Chapter c = JsonConvert.DeserializeObject<Chapter>(data);
			c.MakePagesAbsolute(destination);
			
			DpiDialog dialog = new DpiDialog();			
			dialog.ShowDialog();
			if (dialog.Choice == DpiDialog.OK) {
				if (!dialog.CurrentDPI) {
					foreach (var p in c.Pages) {
						foreach(var t in p.TextEntries) {
							t.DpiAwareRectangle.DpiX = dialog.DpiX;
							t.DpiAwareRectangle.DpiY = dialog.DpiY;
						}
					}
				}
			}
			else {
				throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
			}
			data = c.ToString(destination, page);
			return c;
		}

		
	}
}
