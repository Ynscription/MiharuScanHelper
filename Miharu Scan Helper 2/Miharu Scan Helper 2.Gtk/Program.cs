using Eto.Forms;
using Miharu2.BackEnd;
using Miharu2.BackEnd.Translation.Threading;
using Miharu2.Control;
using System;
using System.IO;

namespace Miharu2.Gtk
{
	class MainClass
{
	[STAThread]
	public static void Main(string[] args)
	{
		Application app = new Application(Eto.Platforms.Wpf);

			ChapterManager chapterManager = null;
			string startChapter = null;
			KanjiInputManager kanjiInputManager = null;
			TranslatorThread translatorThread = null;
			MiharuMainWindow mainWindow = null;

			try {
				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());

				if (Init.CheckForTesseract()) {
					if (Init.CheckForGecko())
						translatorThread = TranslatorThread.StartThread();
								
					startChapter = Init.CheckCrash();
					if (startChapter == null && args.Length > 0 && File.Exists(args [0]))
						startChapter = args[0];

					kanjiInputManager = new KanjiInputManager();
					
					chapterManager = new ChapterManager(kanjiInputManager, translatorThread);

					mainWindow = new MiharuMainWindow(chapterManager, startChapter);

					app.Run(mainWindow);
				}
			}
			catch (Exception e) {
				CrashHandler.HandleCrash(chapterManager, e);
				FileInfo crashFileInfo = new FileInfo(Logger.CurrentCrashLog);

				MessageBox.Show("There was a fatal error. Details can be found in the generated crash log:" + Environment.NewLine +
				crashFileInfo.FullName, 
				"Fatal Error", 
				MessageBoxButtons.OK, 
				MessageBoxType.Error, 
				MessageBoxDefaultButton.OK);
			}
			finally {
				mainWindow?.Close();
				translatorThread?.FinalizeThread();				
			}
	}
}
}
