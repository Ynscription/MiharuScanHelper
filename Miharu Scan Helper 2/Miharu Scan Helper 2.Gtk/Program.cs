using Eto.Forms;
using System;

namespace Miharu2.Gtk
{
	class MainClass
{
	[STAThread]
	public static void Main(string[] args)
	{
		new Application(Eto.Platforms.Gtk).Run(new MiharuMainWindow(null, ""));
	}
}
}
