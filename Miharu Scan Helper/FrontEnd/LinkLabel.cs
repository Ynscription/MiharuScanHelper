
using MahApps.Metro;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Miharu.FrontEnd
{
	class LinkLabel : Label
	{

		public string Link {
			get; set;
		}

		public bool WebBrowser {
			get; set;
		} = true;


		public SolidColorBrush LightThemeColor {
			get; set;
		} = new SolidColorBrush(Color.FromRgb(0x00,0x13,0xCF));
		
		
		public SolidColorBrush DarkThemeColor {
			get; set;
		} = new SolidColorBrush (Color.FromRgb(0x00,0x91,0xff));


		public LinkLabel () : base() {
			MouseLeftButtonUp += OpenLink;
			try {
				if (ThemeManager.DetectTheme().DisplayName.Contains("Dark"))
					Foreground = DarkThemeColor;
				else
					Foreground = LightThemeColor;
			}
			catch (Exception) {
				Foreground = LightThemeColor;
			}
			ThemeManager.IsThemeChanged += OnThemeChange;

		}

		private void OnThemeChange(object sender, OnThemeChangedEventArgs e)
		{
			if(e.Theme.DisplayName.Contains("Dark"))
				Foreground = DarkThemeColor;
			else
				Foreground = LightThemeColor;
				
		}

		private void OpenLink(object sender, MouseButtonEventArgs e)
		{
			if (Link == null) {
				System.Diagnostics.Process.Start((string)Content);
			}
			else {
				if (WebBrowser)
					System.Diagnostics.Process.Start(Link);
				else
					ShowFileInExplorer(Link);
			}
		}

		private void ShowFileInExplorer (string file) {
			if (!File.Exists(file))
				return;
			string argument = "/select, \"" + file + "\"";
			System.Diagnostics.Process.Start("explorer.exe", argument);
		}

		
	}
}
