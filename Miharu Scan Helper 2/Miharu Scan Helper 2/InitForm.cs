using Eto.Drawing;
using Eto.Forms;
using Miharu2.BackEnd;
using Miharu2.BackEnd.Translation.Threading;
using Miharu2.Control;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Miharu2
{
	public partial class InitForm : Form {

		public bool Success { 
			get; private set; 
		} = false;
		public ChapterManager ChapterManager {
			get; private set;
		}
		public string StartChapter {
			get; private set;
		}

		private string [] Args {
			get; set;
		}

		public InitForm() {
			
			Title = "Initializing...";
			Size = new Size(600, 200);
			//Args = args;
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			DoInit(Args);
			Close();
		}


		#region Helper Methods
		private static bool ExistsInPath (string fileName) {
			bool res = false;

			string path = Environment.GetEnvironmentVariable("PATH");
			string [] values = path.Split(Path.PathSeparator);
			for (int i = 0; i < values.Length && !res; i++) {
				var fullPath = Path.Combine(values[i], fileName);
				res = File.Exists(fullPath);
			}

			return res;
		}

		private bool CheckForTesseract() {
			if (!File.Exists(Settings.Get<string>("TesseractPath"))) {
				if (ExistsInPath("tesseract.exe")) {
					Settings.Set ("TesseractPath", "tesseract.exe");
					return true;
				}
				DialogResult dr = MessageBox.Show("Warning Tesseract Not Found.", 
					@"Tesseract executable could not be located.
Miharu requires Tesseract to function.
Would you like to locate the Tesseract exectutable manually?", 
					MessageBoxButtons.YesNo, 
					MessageBoxType.Warning, 
					MessageBoxDefaultButton.Yes);

				if (dr == DialogResult.No)
					return false;

				OpenFileDialog fileDialog = new OpenFileDialog();
				fileDialog.CheckFileExists = true;
				fileDialog.CurrentFilter = new FileFilter("Tesseract", ".exe");
				fileDialog.Title = "Select Tesseract Executable";
				fileDialog.MultiSelect = false;
				dr = fileDialog.ShowDialog(this);

				if (dr == DialogResult.Ok)
					Settings.Set ("TesseractPath", fileDialog.FileName);

				else
					return false;
			}
			return true;
		}

		private static string CheckCrash () {
			string res = null;
			if (CrashHandler.LastSessionCrashed) {
				DialogResult dr = MessageBox.Show("It seems like a file can be recovered from last session.", 
					"Would you like to attempt to recover the file?", 
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

		#endregion

		private void DoInit(string [] args) {
			ChapterManager = null;
			StartChapter = null;
			KanjiInputManager kanjiInputManager = null;
			TranslatorThread translatorThread = null;
						
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());

			if (CheckForTesseract()) {
				translatorThread = TranslatorThread.StartThread();
								
				StartChapter = CheckCrash();
				if (StartChapter == null && args.Length > 0 && File.Exists(args [0]))
					StartChapter = args[0];

					kanjiInputManager = new KanjiInputManager();
					
					ChapterManager = new ChapterManager(kanjiInputManager, translatorThread);

					Success = true;
			}

		}
	}
}
