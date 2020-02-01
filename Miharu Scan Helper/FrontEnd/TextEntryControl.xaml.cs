
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

namespace Manga_Scan_Helper.FrontEnd
{
	/// <summary>
	/// Interaction logic for TextEntryControl.xaml
	/// </summary>
	public partial class TextEntryControl : UserControl, TranslationConsumer{
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject ([In] IntPtr hObject);


		public event TranslationFailEventHandler TranslationFail;

		private MainWindow _parent;
		private Text _textEntry;

		private string SanitizeString (string input) {
			string sanitized = input.Replace(Environment.NewLine, " ");
			sanitized = sanitized.Replace("\n", " ");

			return sanitized;
		}

		private void InitializeParsedTextBox () {
			ParsedTextBox.Text = _textEntry.ParsedText;
			string refinedText = SanitizeString(_textEntry.ParsedText);
			JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(refinedText);
		}

		public string ParsedText {
			get => _textEntry.ParsedText;
			set {
				_textEntry.ParsedText = value;				
			}
		}


		public TextEntryControl(Text textEntry, MainWindow parent) :base(){
			InitializeComponent();
			_textEntry = textEntry;
			_textEntry.TextChanged += _textEntry_TextChanged;
			_parent = parent;
			
			ShowImageFromBitmap(textEntry.Source);
			InitializeParsedTextBox();

			//TODO
			//GoogleTranslationLabel.Text = textEntry.GetTranslation(TranslationType.Google2);
			//BingTranslationLabel.Text = textEntry.GetTranslation(TranslationType.Bing);
			foreach (TranslationType t in Enum.GetValues(typeof (TranslationType))) {
				if (t != TranslationType.JadedNetwork)
					TranslationSourcesStackPanel.Children.Add(new TranslationSourceView (this, t, _textEntry));
			}

			TranslatedTextBox.Text = textEntry.TranslatedText;
			VerticalCheckBox.IsChecked = textEntry.Vertical;
			

		}

		private void _textEntry_TextChanged(object sender, TxtChangedEventArgs e)
		{
			if (e.ChangeType == TextChangeType.Parse) {
				ParsedTextBox.Text = e.Text;
				string refinedText = SanitizeString(e.Text);
				JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(refinedText);
			}
			else if (e.ChangeType == TextChangeType.Translation)
				TranslatedTextBox.Text = e.Text;

		}

		private void RefreshParseButton_Click (object sender, RoutedEventArgs e) {
			Mouse.SetCursor(Cursors.Wait);
			_textEntry.Invalidate();
			ParsedTextBox.Text = _textEntry.ParsedText;
			string refinedText = SanitizeString(_textEntry.ParsedText);
			JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(refinedText);
			Mouse.SetCursor(Cursors.Arrow);
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
				PreviewIMG.Width = PreviewIMG.Height* (dest.Width / dest.Height);
			}
			finally { DeleteObject(handle); }
		}

		public void ForceTranslation () {

			string refinedText = SanitizeString(ParsedText);
			//TODO
			//RefreshTranslateButton.IsEnabled = false;
			//VerticalCheckBox.IsEnabled = false;

			/*HTTPTranslator.GoogleTranslate(this, refinedText);*/
			HTTPTranslator.Google2Translate(this, refinedText);
			HTTPTranslator.BingTranslate(this, refinedText);
			HTTPTranslator.YandexTranslate(this, refinedText);

			HTTPTranslator.JadedNetworkTranslate(this, refinedText);
			
		}

		public void RequestTranslation (TranslationType type) {
			string refinedText = SanitizeString(ParsedText);
			/*if (type == TranslationType.GoogleAPI)
				HTTPTranslator.GoogleTranslate(this, refinedText);*/
			if (type == TranslationType.Google2)
				HTTPTranslator.Google2Translate(this, refinedText);
			else if (type == TranslationType.Bing)
				HTTPTranslator.BingTranslate(this, refinedText);
			else if (type == TranslationType.Yandex)
				HTTPTranslator.YandexTranslate(this, refinedText);
			else if (type == TranslationType.JadedNetwork)
				HTTPTranslator.JadedNetworkTranslate(this, refinedText);
		}

		
		public void TranslationCallback (string translation, TranslationType type) {
			translation = translation.Replace("\\\"", "\"");
			try {
				Dispatcher.Invoke(() => {
					_textEntry.SetTranslation(type, translation);
				});
			}
			catch (TaskCanceledException) { }
		}

		public void TranslationFailed (Exception e, TranslationType type) {
			TranslationFail?.Invoke(this, new TranslationFailEventArgs(e, type));
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
			//TODO
			//_parent.RemoveTextEntry(this);
		}


	}
}
