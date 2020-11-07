
using ControlzEx.Theming;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Miharu.FrontEnd.Helper
{
	public class LinkLabel : Label
	{

		private TextBlock _text;

		public string Link {
			get; set;
		}

		public bool WebBrowser {
			get; set;
		} = true;


		public SolidColorBrush LightThemeColor {
			get; set;
		} = new SolidColorBrush(Color.FromRgb(0x00, 0x13, 0xCF));


		public SolidColorBrush DarkThemeColor {
			get; set;
		} = new SolidColorBrush(Color.FromRgb(0x00, 0x91, 0xff));


		public LinkLabel() : base()
		{
			MouseLeftButtonUp += OpenLink;
			try
			{
				if (ThemeManager.Current.DetectTheme().DisplayName.Contains("Dark"))
					Foreground = DarkThemeColor;
				else
					Foreground = LightThemeColor;
			}
			catch (Exception)
			{
				Foreground = LightThemeColor;
			}
			ThemeManager.Current.ThemeChanged += Current_ThemeChanged;
			MouseEnter += LinkLabel_MouseEnter;
			MouseLeave += LinkLabel_MouseLeave;
			_text = new TextBlock();
			Initialized += LinkLabel_Initialized;
			

		}

		

		private void LinkLabel_Initialized(object sender, EventArgs e)
		{
			if (Content != null && Content != _text && Content is string) {
				string ctnt = (string)Content;
				_selfChangeContent = true;
				Content = _text;
				_text.Text = ctnt;
			}
		}

		private bool _selfChangeContent = false;
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if (!_selfChangeContent) {
				if (newContent is string)
					_text.Text = (string)newContent;
				_selfChangeContent = true;
				Content = _text;
			}
			else
				_selfChangeContent = false;
		}
		
		
		private void Current_ThemeChanged(object sender, ThemeChangedEventArgs e)
		{
			if (e.NewTheme.DisplayName.Contains("Dark"))
				Foreground = DarkThemeColor;
			else
				Foreground = LightThemeColor;
		}


		private void OpenLink(object sender, MouseButtonEventArgs e)
		{
			if (Link == null)
			{
				System.Diagnostics.Process.Start((string)Content);
			}
			else
			{
				if (WebBrowser)
					System.Diagnostics.Process.Start(Link);
				else
					ShowFileInExplorer(Link);
			}
		}

		private void ShowFileInExplorer(string file)
		{
			if (!File.Exists(file))
				return;
			string argument = "/select, \"" + file + "\"";
			System.Diagnostics.Process.Start("explorer.exe", argument);
		}

		private void LinkLabel_MouseEnter(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Hand;
			_text.TextDecorations = TextDecorations.Underline;
			_text.InvalidateVisual();
		}

		private void LinkLabel_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = null;
			_text.TextDecorations = null;
			_text.InvalidateVisual();
		}		

	}
}
