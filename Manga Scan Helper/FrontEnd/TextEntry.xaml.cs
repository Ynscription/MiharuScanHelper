
using Manga_Scan_Helper.BackEnd;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manga_Scan_Helper.FrontEnd {

	/// <summary>
	/// Interaction logic for TextEntry.xaml
	/// </summary>
	public partial class TextEntry : UserControl, TranslationConsumer{
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject ([In] IntPtr hObject);

		
		
		private MainWindow _parent;
		private Text _textEntry;

		private void InitializeParsedTextBox () {
			ParsedTextBox.Text = _textEntry.ParsedText;
			string refinedText = _textEntry.ParsedText.Replace(@"
", " ");
			JishoLinkLabel.Content = "https://jisho.org/search/" + refinedText;
			//Translate(refinedText);
		}
		public string ParsedText {
			get => _textEntry.ParsedText;
			set {
				_textEntry.ParsedText = value;
				ParsedTextBox.Text = _textEntry.ParsedText;
				string refinedText = _textEntry.ParsedText.Replace(@"
", " ");
				JishoLinkLabel.Content = "https://jisho.org/search/" + refinedText;
				//Translate(refinedText);
				
			}
		}

		public TextEntry (Text textEntry, MainWindow parent) :base(){
			InitializeComponent();
			_parent = parent;
			ShowImageFromBitmap(textEntry.Source);
			_textEntry = textEntry;
			InitializeParsedTextBox();
			GoogleTranslationLabel.Text = textEntry.GoogleTranslatedText;
			BingTranslationLabel.Text = textEntry.BingTranslatedText;
			//OnoTranslationText.Text = textEntry.OnoTranslatedText;
			TranslatedTextBox.Text = textEntry.TranslatedText;
			VerticalCheckBox.IsChecked = textEntry.Vertical;
			
			
		}

		private void RefreshParseButton_Click (object sender, RoutedEventArgs e) {
			Mouse.SetCursor(Cursors.Wait);
			_textEntry.Invalidate();
			ParsedTextBox.Text = _textEntry.ParsedText;
			string refinedText = _textEntry.ParsedText.Replace(@"
", " ");
			JishoLinkLabel.Content = "https://jisho.org/search/" + refinedText;
			Mouse.SetCursor(Cursors.Arrow);
		}

		private void RefreshTranslateButton_Click (object sender, RoutedEventArgs e) {
			string refinedText = ParsedText.Replace(@"
", " ");
			Translate(refinedText);			
		}

		

		private void ParsedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (ParsedText != ParsedTextBox.Text)
				ParsedText = ParsedTextBox.Text;
		}

		private void TranslatedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (_textEntry.TranslatedText != TranslatedTextBox.Text)
				_textEntry.TranslatedText = TranslatedTextBox.Text;
		}

		private void JishoLinkLabel_MouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e) {
			System.Diagnostics.Process.Start((string)JishoLinkLabel.Content);
		}

		private void ShowImageFromBitmap (Bitmap src) {
			var handle = src.GetHbitmap();
			try {
				ImageSource dest = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				PreviewIMG.Source = dest;
			}
			finally { DeleteObject(handle); }
		}

		public void ForceTranslation () {
			
			string refinedText = ParsedText.Replace(@"
", " ");
			Translate(refinedText);
			
		}

		private bool _awaitingGoogle = false;
		private bool _awaitingBing = false;
		private void Translate (string text) {
			RefreshTranslateButton.IsEnabled = false;
			VerticalCheckBox.IsEnabled = false;
			_awaitingGoogle = true;
			HTTPTranslator.GoogleTranslate(this, text);
			_awaitingBing = true;
			HTTPTranslator.BingTranslate(this, text);
			_textEntry.OnoTranslatedText = SFXDictionary.Translate(text);
			
		}

		public void TranslationCallback (string translation, TranslationType type) {
			try {
				Dispatcher.Invoke(() => {
					if (type == TranslationType.Google) {
						GoogleTranslationLabel.Text = translation;
						_textEntry.GoogleTranslatedText = translation;
						_awaitingGoogle = false;
					}
					else if (type == TranslationType.Bing) {
						BingTranslationLabel.Text = translation;
						_textEntry.BingTranslatedText = translation;
						_awaitingBing = false;
					}
					if (!_awaitingGoogle && !_awaitingBing) {
						RefreshTranslateButton.IsEnabled = true;
						VerticalCheckBox.IsEnabled = true;
					}
				});
			}
			catch (TaskCanceledException) { }
		}

		public void TranslationFailed (Exception e, TranslationType type) {
			try {
				Dispatcher.Invoke(() => {
					if (type == TranslationType.Google)
						_awaitingGoogle = false;

					else if (type == TranslationType.Bing)
						_awaitingBing = false;
					
					if (!_awaitingGoogle && !_awaitingBing) {
						RefreshTranslateButton.IsEnabled = true;
						VerticalCheckBox.IsEnabled = true;
					}
				});
			}
			catch (TaskCanceledException) { }
		}

		



		private void VerticalCheckBox_Checked (object sender, RoutedEventArgs e) {
			if (_textEntry != null && !_textEntry.Vertical)
				_textEntry.Vertical = true;
		}

		private void VerticalCheckBox_Unchecked (object sender, RoutedEventArgs e) {
			if (_textEntry != null && _textEntry.Vertical)
				_textEntry.Vertical = false;
		}

		private void DeleteButton_Click (object sender, RoutedEventArgs e) {
			_parent.RemoveTextEntry(this);
		}

		private void MoveUpButton_Click (object sender, RoutedEventArgs e) {
			_parent.MoveTextEntry(this, true);
		}

		private void MoveDownButton_Click (object sender, RoutedEventArgs e) {
			_parent.MoveTextEntry(this, false);
		}

		
	}
}
