using Eto.Forms;
using Miharu2.Control;
using System;

namespace Miharu2.Wpf
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
		
			Application app = new Application(Eto.Platforms.Wpf);

			InitForm iform = new InitForm();

			app.Run(iform);

			if (iform.Success)
				app.Run(new MainForm(iform.ChapterManager, iform.StartChapter));
		}
	}
}
