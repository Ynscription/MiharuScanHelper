
using Miharu.BackEnd;
using Miharu.BackEnd.Translation.WebCrawlers;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;

namespace Miharu {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		public App () {
			InitializeComponent();
		}

		[STAThread]
		public static void Main()
		{
			MainWindow mainWindow = null;
			try {
				var application = new App();

				//Initialize stuff
				mainWindow = new MainWindow();

				application.Run(mainWindow);
			}
			catch (Exception e) {
				CrashHandler.HandleCrash(mainWindow.LoadedChapter, mainWindow.CurrentSaveFile, e);
				FileInfo crashFileInfo = new FileInfo(Logger.CurrentCrashLog);

				TaskDialog dialog = new TaskDialog();
				dialog.WindowTitle = "Fatal Crash";
				dialog.MainIcon = TaskDialogIcon.Error;
				dialog.MainInstruction = "There was a fatal crash. Details can be found in the generated crash log:";
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
	}
}
