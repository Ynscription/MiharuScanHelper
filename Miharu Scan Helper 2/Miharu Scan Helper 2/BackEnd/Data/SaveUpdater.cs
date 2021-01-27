
using Eto.Forms;
using Miharu2.BackEnd;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

namespace Miharu2.BackEnd.Data
{
	public static class SaveUpdater
	{
		private const string _OVERWRITE = "Overwrite";
		private const string _NEW_FILE = "New File";
		private const string _CANCEL = "Cancel";

		private static DialogResult WarnUpdateRequired () {
			
			return MessageBox.Show(@"The file was saved in an old format.
What would you like to do?", 
				"Warning: Outdated File", 
				MessageBoxButtons.YesNoCancel, 
				MessageBoxType.Warning, 
				MessageBoxDefaultButton.No);
			
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
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filters.Add(new FileFilter("Scans files", ".scan"));
			fileDialog.Title = "Save Updated Chapter";
			fileDialog.CheckFileExists = true;
			//fileDialog.OverwritePrompt = true;
			
			DialogResult r = fileDialog.ShowDialog(null);
			if (r == DialogResult.Ok)
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
				else if (update = version != Settings.Get<string>("SaveVersion")) {
					page = int.Parse(reader.ReadLine());
					versionNumber = int.Parse(version.Substring(1));
				}

				if (update) {
					DialogResult warnRes = WarnUpdateRequired();
					if (warnRes == DialogResult.Yes) {
						string data = reader.ReadToEnd();
						reader.Close();
						Update(source, data, page, versionNumber);
					}
					else if (warnRes == DialogResult.Cancel)
						throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
					else if (warnRes == DialogResult.No){
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
			
			/*DpiDialog dialog = new DpiDialog();			
			dialog.ShowDialog();
			if (dialog.Choice == DpiDialog.OK) {
				if (!dialog.CurrentDPI) {*/
					foreach (var p in c.Pages) {
						foreach(var t in p.TextEntries) {
							t.DpiAwareRectangle.DpiX = 96;
							t.DpiAwareRectangle.DpiY = 96;
						}
					}
				/*}
			}
			else {
				throw new Exception ("Couldn't open file due to file version missmatch. Consider updating the file or using an older version of Miharu.");
			}*/
			data = c.ToString(destination, page);
			return c;
		}

		
	}
}
