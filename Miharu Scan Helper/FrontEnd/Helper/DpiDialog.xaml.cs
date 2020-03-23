using MahApps.Metro.Controls;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Miharu.FrontEnd.Helper
{
	/// <summary>
	/// Interaction logic for DpiWindow.xaml
	/// </summary>
	public partial class DpiDialog : MetroWindow
	{

		
		public const string OK = "Ok";
		public const string CANCEL = "Cancel";

		public string Choice {
			get; private set;
		}

		public bool CurrentDPI {
			get; private set;
		}

		public double DpiX {
			get; private set;
		}
		public double DpiY {
			get; private set;
		}

		public DpiDialog()
		{
			InitializeComponent();
			Graphics g = Graphics.FromHwnd(IntPtr.Zero);
			double dpiX = g.DpiX;
			double dpiY = g.DpiY;
			DpiX = dpiX;
			DpiY = dpiY;
			DpiXTxtBox.Text = "" + dpiX;
			DpiXTxtBox.Text = "" + dpiY;
			CurrentDPI = true;
			
		}

		private void CurrentButton_Click(object sender, RoutedEventArgs e)
		{
			Graphics g = Graphics.FromHwnd(IntPtr.Zero);
			double dpiX = g.DpiX;
			double dpiY = g.DpiY;
			DpiX = dpiX;
			DpiY = dpiY;
			DpiXTxtBox.Text = "" + dpiX;
			DpiXTxtBox.Text = "" + dpiY;
			CurrentDPI = true;
			
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Choice = OK;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Choice = CANCEL;
			Close();
		}

		private void DpiXTxtBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			double newDpiX;
			if (double.TryParse(DpiXTxtBox.Text, out newDpiX)) {
				DpiX = newDpiX;
				CurrentDPI = false;
			}
			else {
				DpiXTxtBox.Text = "" + DpiX;
			}
		}

		private void DpiYTxtBox_TextChanged(object sender, TextChangedEventArgs e) {
		
			double newDpiY;
			if (double.TryParse(DpiYTxtBox.Text, out newDpiY)) {
				DpiY = newDpiY;
				CurrentDPI = false;
			}
			else {
				DpiYTxtBox.Text = "" + DpiY;
			}

		}
	}
}
