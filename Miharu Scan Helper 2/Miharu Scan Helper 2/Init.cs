using Eto.Forms;
using Miharu2.BackEnd;
using Miharu2.BackEnd.Translation.WebCrawlers;
using System;
using System.IO;

namespace Miharu2
{
	public static class Init
	{
		public static bool ExistsInPath (string fileName) {
			bool res = false;

			string path = Environment.GetEnvironmentVariable("PATH");
			string [] values = path.Split(Path.PathSeparator);
			for (int i = 0; i < values.Length && !res; i++) {
				var fullPath = Path.Combine(values[i], fileName);
				res = File.Exists(fullPath);
			}

			return res;
		}

		public static bool CheckForTesseract() {
			if (!File.Exists(Settings.Get<string>("TesseractPath"))) {
				if (ExistsInPath("tesseract.exe")) {
					Settings.Set ("TesseractPath", "tesseract.exe");
					return true;
				}
				DialogResult dr = MessageBox.Show(@"Tesseract executable could not be located.
Miharu requires Tesseract to function.
Would you like to locate the Tesseract exectutable manually?", 
					"Warning: Tesseract Not Found",
					MessageBoxButtons.YesNo, 
					MessageBoxType.Warning, 
					MessageBoxDefaultButton.Yes);

				if (dr == DialogResult.No)
					return false;

				OpenFileDialog fileDialog = new OpenFileDialog();
				fileDialog.CheckFileExists = true;
				fileDialog.Filters.Add(new FileFilter("tesseract", ".exe"));
				fileDialog.Title = "Select Tesseract Executable";
				fileDialog.MultiSelect = false;
				dr = fileDialog.ShowDialog(null);

				if (dr == DialogResult.Ok)
					Settings.Set ("TesseractPath", fileDialog.FileName);

				else
					return false;
			}
			return true;
		}

		public static bool CheckForGecko()
		{
			bool res = false;
			if (!(res = File.Exists(WebDriverManager.GECKO_DRIVER_PATH))) {
				DialogResult dr = MessageBox.Show(@"Could not find Gecko Driver.
Some features will be missing.", 
					"Warning: Gecko Driver Not Found",
					MessageBoxButtons.OK, 
					MessageBoxType.Warning, 
					MessageBoxDefaultButton.OK);
			}

			return res;
		}

		public static string CheckCrash () {
			string res = null;
			if (CrashHandler.LastSessionCrashed) {
				DialogResult dr = MessageBox.Show(@"It seems like a file can be recovered from last session.
Would you like to attempt to recover the file?", 
					"Warning: Recover Lost Work", 
					MessageBoxButtons.YesNo, 
					MessageBoxType.Warning, 
					MessageBoxDefaultButton.Yes);
				
				string temp = CrashHandler.RecoverLastSessionFile();
				if (dr == DialogResult.Yes) {
					res = temp;
				}
			}
			return res;
		}

	}
}
