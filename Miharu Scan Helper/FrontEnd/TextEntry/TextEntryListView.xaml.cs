
using Miharu.BackEnd.Data;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd.TextEntry {

	/// <summary>
	/// Interaction logic for TextEntry.xaml
	/// </summary>
	public partial class TextEntryListView : UserControl {
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject ([In] IntPtr hObject);

		//FF535353
		private static System.Windows.Media.Brush _normalBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 83, 83, 83));
		
		private MainWindow _parent;
		private Text _textEntry;

		private bool _selected;
		public bool Selected {
			get => _selected;
			set {
				_selected = value;
				BrushConverter converter = new BrushConverter();
				EntryBorder.BorderBrush = _selected ? System.Windows.Media.Brushes.Blue : _normalBrush;
				EntryBorder.BorderThickness = new Thickness(_selected ? 2 : 1);
			}
		}
		
		public TextEntryListView (Text textEntry, MainWindow parent) :base(){
			InitializeComponent();
			_textEntry = textEntry;
			_parent = parent;
			TranslationLabel.Text = _textEntry.TranslatedText;
			ParsedLabel.Text = _textEntry.ParsedText;
			_textEntry.TextChanged += OnTextChanged;
			ShowImageFromBitmap(textEntry.Source);
		}

		private void ShowImageFromBitmap (Bitmap src) {
			var handle = src.GetHbitmap();
			try {
				ImageSource dest = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				PreviewIMG.Source = dest;
				double desiredWidth = PreviewIMG.Height* (dest.Width / dest.Height);
				if (desiredWidth > PreviewImgColumn.MaxWidth)
					desiredWidth = PreviewImgColumn.MaxWidth;
				PreviewIMG.Width = desiredWidth;
			}
			finally { DeleteObject(handle); }
		}

		
		private void OnTextChanged (object sender, TxtChangedEventArgs args) {
			try {
				Dispatcher.Invoke(() => {
					if (args.ChangeType == TextChangeType.Parse)
						ParsedLabel.Text = _textEntry.ParsedText;
					else if (args.ChangeType == TextChangeType.Translation)
						TranslationLabel.Text = _textEntry.TranslatedText;
					
				});
			}
			catch (TaskCanceledException) { }
		}

		private void DeleteButton_Click (object sender, RoutedEventArgs e) {
			_parent.RemoveTextEntry(_textEntry);
		}

		private void MoveUpButton_Click (object sender, RoutedEventArgs e) {
			_parent.MoveTextEntry(_textEntry, true);
		}

		private void MoveDownButton_Click (object sender, RoutedEventArgs e) {
			_parent.MoveTextEntry(_textEntry, false);
		}

				

		private void Event_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_parent.SelectTextEntry(this);
		}
	}
}
