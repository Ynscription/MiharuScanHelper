
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd
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

		/*private string SanitizeString (string input) {
			return input;
			/*string sanitized = input.Replace(Environment.NewLine, " ");
			sanitized = sanitized.Replace("\n", " ");

			return sanitized;
		}*/

		private void InitializeParsedTextBox () {
			ParsedTextBox.Text = _textEntry.ParsedText;
			JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(_textEntry.ParsedText);
		}

		public string ParsedText {
			get => _textEntry.ParsedText;
			set {
				_textEntry.ParsedText = value;				
			}
		}


		public TextEntryControl(Text textEntry, MainWindow parent) :base(){
			InitializeComponent();
			
			InitializeSAnimation();
			_textEntry = textEntry;
			_textEntry.TextChanged += _textEntry_TextChanged;
			_parent = parent;
			
			ShowImageFromBitmap(textEntry.Source);
			InitializeParsedTextBox();

			foreach (TranslationType t in TranslationProvider.Instance) {
				if (t != TranslationType.Jaded_Network)
					TranslationSourcesStackPanel.Children.Add(new TranslationSourceView (this, t, _textEntry));
			}

			SFXTranslationGrid.Children.Add(new TranslationSourceJadedNetworkView(this, _textEntry));

			TranslatedTextBox.Text = textEntry.TranslatedText;
			VerticalCheckBox.IsChecked = textEntry.Vertical;

		}

		private void _textEntry_TextChanged(object sender, TxtChangedEventArgs e)
		{
			if (e.ChangeType == TextChangeType.Parse) {
				ParsedTextBox.Text = e.Text;
				JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(e.Text);
			}
			else if (e.ChangeType == TextChangeType.Translation)
				TranslatedTextBox.Text = e.Text;

		}

		private void RefreshParseButton_Click (object sender, RoutedEventArgs e) {
			Mouse.SetCursor(Cursors.Wait);
			RefreshParseButton.IsEnabled = false;
			VerticalCheckBox.IsEnabled = false;
			_textEntry.Invalidate();
			ParsedTextBox.Text = _textEntry.ParsedText;
			JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(_textEntry.ParsedText);
			RefreshParseButton.IsEnabled = true;
			VerticalCheckBox.IsEnabled = true;
			Mouse.SetCursor(Cursors.Arrow);
		}

		
		

		private void ParsedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (ParsedText != ParsedTextBox.Text)
				ParsedText = ParsedTextBox.Text;
			CheckAnimation(ParsedTextBox.Text);
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

		private void RefreshAllButton_Click(object sender, RoutedEventArgs e)
		{
			ForceTranslation();
		}

		public void ForceTranslation () {

			foreach (TranslationSourceView t in TranslationSourcesStackPanel.Children)
				t.AwaitTranslation();


			foreach (TranslationType t in TranslationProvider.Instance) {
				if (t != TranslationType.Jaded_Network)
					TranslationProvider.Translate(t, ParsedText, this);
			}
			
			//It makes sense to not ask for an SFX translation by default.
			
		}

		public void RequestTranslation (TranslationType type) {
			/*if (type == TranslationType.GoogleAPI)
				HTTPTranslator.GoogleTranslate(this, refinedText);*/
			TranslationProvider.Translate(type, ParsedText, this);
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

		
		private Storyboard _showS;
		private Storyboard _hideS;
		private enum AnimState {
			Hidden,
			Showing,
			Shown,
			Hiding
		}
		private volatile AnimState _currentState = AnimState.Hidden;
		private volatile AnimState _desiredState = AnimState.Hidden;

		private void InitializeSAnimation () {
			_showS = new Storyboard();

			ThicknessAnimation animation = new ThicknessAnimation();
			animation.From = new Thickness(S_Tail.Margin.Left, S_Tail.Margin.Top, S_Tail.Margin.Right, S_Tail.Margin.Bottom);
			animation.To = new Thickness(S_Tail.Margin.Left, S_Tail.Margin.Top, S_Tail.Margin.Right, -1);
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));

			
			_showS.Children.Add(animation);
			Storyboard.SetTargetName(animation, S_Tail.Name);
			Storyboard.SetTargetProperty(animation, new PropertyPath(System.Windows.Controls.Image.MarginProperty));

						
			
			animation = new ThicknessAnimation();
			animation.From = new Thickness(S_Head.Margin.Left, S_Head.Margin.Top, S_Tail.Margin.Right, S_Head.Margin.Bottom);
			animation.To = new Thickness(S_Head.Margin.Left, S_Head.Margin.Top, S_Head.Margin.Right, -1);
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));

			_showS.Children.Add(animation);
			Storyboard.SetTargetName(animation, S_Head.Name);
			Storyboard.SetTargetProperty(animation, new PropertyPath(System.Windows.Controls.Image.MarginProperty));




			_hideS = new Storyboard();
			
			animation = new ThicknessAnimation();
			animation.To = new Thickness(S_Tail.Margin.Left, S_Tail.Margin.Top, S_Tail.Margin.Right, S_Tail.Margin.Bottom);
			animation.From = new Thickness(S_Tail.Margin.Left, S_Tail.Margin.Top, S_Tail.Margin.Right, -1);
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));

			
			_hideS.Children.Add(animation);
			Storyboard.SetTargetName(animation, S_Tail.Name);
			Storyboard.SetTargetProperty(animation, new PropertyPath(System.Windows.Controls.Image.MarginProperty));




			animation = new ThicknessAnimation();
			animation.To = new Thickness(S_Head.Margin.Left, S_Head.Margin.Top, S_Tail.Margin.Right, S_Head.Margin.Bottom);
			animation.From = new Thickness(S_Head.Margin.Left, S_Head.Margin.Top, S_Head.Margin.Right, -1);
			animation.Duration = new Duration(TimeSpan.FromSeconds(1));

			_hideS.Children.Add(animation);
			Storyboard.SetTargetName(animation, S_Head.Name);
			Storyboard.SetTargetProperty(animation, new PropertyPath(System.Windows.Controls.Image.MarginProperty));

			_hideS.Completed += HideComplete;
			_showS.Completed += ShowComplete;
		}

		

		private void AnimateS () {
			S_Head.Visibility = Visibility.Visible;
			S_Tail.Visibility = Visibility.Visible;
			_currentState = AnimState.Showing;
			_showS.Begin(this);
		}

		private void StopAnimateS () {
			_currentState = AnimState.Hiding;
			_hideS.Begin(this);
		}

		private void ShowComplete(object sender, EventArgs e)
		{
			_currentState = AnimState.Shown;
			if (_desiredState == AnimState.Hidden)
				StopAnimateS();
		}

		private void HideComplete(object sender, EventArgs e)
		{
			S_Head.Visibility = Visibility.Hidden;
			S_Tail.Visibility = Visibility.Hidden;
			_currentState = AnimState.Hidden;
			if (_desiredState == AnimState.Shown)
				AnimateS();
			
		}

		private void CheckAnimation (string parse) {
			if (parse == "おかえりなのじゃ") {
				 _desiredState = AnimState.Shown;
			}
			else {
				_desiredState = AnimState.Hidden;
			}

			if ((_currentState == AnimState.Hidden || _currentState == AnimState.Shown) && _currentState != _desiredState) {
				if (_desiredState == AnimState.Shown)
					AnimateS();
				else if (_desiredState == AnimState.Hidden)
					StopAnimateS();
			}
		}	
	}
}
