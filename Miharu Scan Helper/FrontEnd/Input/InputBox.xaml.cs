using ControlzEx.Theming;
using MahApps.Metro;
using Miharu.BackEnd.Data.KanjiByRad;
using Miharu.Control;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Miharu.FrontEnd.Input
{
	/// <summary>
	/// Interaction logic for InputBox.xaml
	/// </summary>
	public partial class InputBox : UserControl
	{
		private JPChar _txtContent;
		private KanjiInputManager _kanjiInputManager;

		
		public InputBox(JPChar txtContent, KanjiInputManager kanjiInputManager)
		{
			InitializeComponent();
			_txtContent = txtContent;
			ContentLabel.Content = _txtContent.Lit;
			SelectedCircleRectangle.Visibility = _txtContent.IsSelected ? Visibility.Visible : Visibility.Hidden;
			_kanjiInputManager = kanjiInputManager;
			_txtContent.SelectedChanged += _txtContent_SelectedChanged;
			_txtContent.EnabledChanged += _txtContent_EnabledChanged;
		}

		private void _txtContent_EnabledChanged(object sender, EventArgs e)
		{
			ContentLabel.IsEnabled = _txtContent.IsEnabled;
		}

		private void _txtContent_SelectedChanged(object sender, EventArgs e)
		{
			SelectedCircleRectangle.Visibility = _txtContent.IsSelected ? Visibility.Visible : Visibility.Hidden;
		}

		private static readonly Brush RealDarkGray = new SolidColorBrush(
			Color.FromRgb(0x35, 0x35, 0x35));

		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			if (_txtContent.IsEnabled) {
				Mouse.OverrideCursor = Cursors.Hand;
				if (ThemeManager.Current.DetectTheme().BaseColorScheme == "Dark")
					BackgroundRectangle.Fill = RealDarkGray;
				else
					BackgroundRectangle.Fill = Brushes.LightGray;
			}
		
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = null;
			BackgroundRectangle.Fill = Brushes.Transparent;
		}

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_txtContent.IsRad) {
				if (_txtContent.IsEnabled) {
					if (!_txtContent.IsSelected)
						_kanjiInputManager.SelectRad(_txtContent);
					else
						_kanjiInputManager.DeselectRad(_txtContent);
				}
			}
			else
				_kanjiInputManager.InputKanji(_txtContent);
		}
	}
}
