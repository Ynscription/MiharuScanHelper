﻿
using MahApps.Metro;
using Miharu.BackEnd;
using Miharu.BackEnd.Translation.WebCrawlers;
using Miharu.Control;
using Miharu.FrontEnd;
using Miharu.FrontEnd.Page;
using Miharu.FrontEnd.TextEntry;
using Miharu.Properties;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Miharu {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {


		#region Main
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

		private static bool CheckForTesseract() {
			if (!File.Exists((string)Settings.Default["TesseractPath"])) {
				if (ExistsInPath("tesseract.exe")) {
					Settings.Default ["TesseractPath"] = "tesseract.exe";
					Settings.Default.Save();
					return true;
				}
				TaskDialog dialog = new TaskDialog();
				dialog.WindowTitle = "Warning Tesseract Not Found";
				dialog.MainIcon = TaskDialogIcon.Warning;
				dialog.MainInstruction = "Tesseract executable could not be located.";
				dialog.Content = @"Miharu requires Tesseract to function.
Would you like to locate the Tesseract exectutable manually?";

				TaskDialogButton okButton = new TaskDialogButton(ButtonType.Yes);
				dialog.Buttons.Add(okButton);
				TaskDialogButton cancelButton = new TaskDialogButton(ButtonType.No);
				dialog.Buttons.Add(cancelButton);
				TaskDialogButton button = dialog.ShowDialog();
				if (button.ButtonType == ButtonType.No)
					return false;

				VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
				fileDialog.AddExtension = true;
				fileDialog.CheckFileExists = true;
				fileDialog.CheckPathExists = true;
				fileDialog.DefaultExt = ".exe";
				fileDialog.Filter = "Tesseract (tesseract.exe)|tesseract.exe";
				fileDialog.Multiselect = false;
				fileDialog.Title = "Select Tesseract Executable";
				bool? res = fileDialog.ShowDialog();
				if (res ?? false) {
					Settings.Default ["TesseractPath"] = fileDialog.FileName;
					Settings.Default.Save();
				}
				else {
					return false;
				}
			}
			return true;
		}

		private static string CheckCrash () {
			string res = null;
			if (CrashHandler.LastSessionCrashed) {
				TaskDialog dialog = new TaskDialog();
				dialog.WindowTitle = "Warning";
				dialog.MainIcon = TaskDialogIcon.Warning;
				dialog.MainInstruction = "It seems like a file can be recovered from last session.";
				dialog.Content = "Would you like to attempt to recover the file?";
				TaskDialogButton saveButton = new TaskDialogButton(ButtonType.Yes);
				saveButton.Text = "Yes";
				dialog.Buttons.Add(saveButton);
				TaskDialogButton noSaveButton = new TaskDialogButton(ButtonType.No);
				noSaveButton.Text = "No";
				dialog.Buttons.Add(noSaveButton);
				TaskDialogButton button = dialog.ShowDialog();
				string temp = CrashHandler.RecoverLastSessionFile();
				if (button.ButtonType == ButtonType.Yes) {
					res = temp;
				}
			}
			return res;
		}

		private static void InitWebDriver () {
			Task.Run(() => {
				WebDriverManager.Init();
				if (!WebDriverManager.IsAlive) {
					
					TaskDialog dialog = new TaskDialog();
					dialog.WindowTitle = "Warning";
					dialog.MainIcon = TaskDialogIcon.Warning;
					dialog.MainInstruction = "Could not load Firefox Web Driver.";
					dialog.Content = "To access more and better translation sources, Firefox Web Browser must be installed in your system.";

					TaskDialogButton okButton = new TaskDialogButton("Ok");
					okButton.ButtonType = ButtonType.Ok;
					dialog.Buttons.Add(okButton);
					Current.Dispatcher.Invoke((Action)delegate{
						TaskDialogButton button = dialog.ShowDialog();
					});
					
				}
			});
		}

		#endregion

		[STAThread]
		public static void Main(string [] args)
		{
			
			ChapterManager chapterManager = null;
			try {

				App application = new App();
				
				
				/*MainWindow mw = new MainWindow();
				application.Run(mw);*/			
				
				
				//Initialize stuff
				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());
				if (CheckForTesseract()) {
					InitWebDriver();

					string startChapter = null;
					startChapter = CheckCrash();
					if (startChapter == null && args.Length > 1 && File.Exists(args [1]))
						startChapter = args[1];
					try {
						chapterManager = new ChapterManager(startChapter);
					}
					catch (Exception) {
						chapterManager = new ChapterManager();
					}

					MiharuMainWindow mainWindow = new MiharuMainWindow(chapterManager);

					PageControl pageControl = new PageControl(chapterManager.PageManager);
					mainWindow.PageControlArea.Child = pageControl;

					TextEntryView textEntryView = new TextEntryView(chapterManager.PageManager.TextEntryManager);
					mainWindow.TextEntryArea.Child = textEntryView;

					application.Run(mainWindow);
				}
			}
			catch (Exception e) {
				CrashHandler.HandleCrash(chapterManager, e);
				FileInfo crashFileInfo = new FileInfo(Logger.CurrentCrashLog);

				TaskDialog dialog = new TaskDialog();
				dialog.WindowTitle = "Fatal Error";
				dialog.MainIcon = TaskDialogIcon.Error;
				dialog.MainInstruction = "There was a fatal error. Details can be found in the generated crash log:";
				dialog.Content = crashFileInfo.FullName;

				TaskDialogButton okButton = new TaskDialogButton("Ok");
				okButton.ButtonType = ButtonType.Ok;
				dialog.Buttons.Add(okButton);
				TaskDialogButton button = dialog.ShowDialog();
			}
			finally {
				WebDriverManager.Instance.Dispose();
			}			
		}

		#endregion

		
				
		
		public App () {
			InitializeComponent();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			ThemeManager.ChangeTheme(Current, 
				(string)Settings.Default["Theme"] + "." + 
				(string)Settings.Default["Accent"]);

			base.OnStartup(e);
		}


		/*public void ChangeSkin(string newSkin) {
			Uri skinDictUri;
			if (newSkin != null && Uri.TryCreate(newSkin,UriKind.Absolute, out skinDictUri)) {
				ResourceDictionary skinDict = (ResourceDictionary)LoadComponent(skinDictUri);
				Collection<ResourceDictionary> mergedDicts = base.Resources.MergedDictionaries;
				mergedDicts.Add(skinDict);
			}
						
		}*/

	}
}
