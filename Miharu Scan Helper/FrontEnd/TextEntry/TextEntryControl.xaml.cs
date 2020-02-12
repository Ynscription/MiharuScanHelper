
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using Miharu.FrontEnd.TextEntry;
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
using WpfAnimatedGif;

namespace Miharu.FrontEnd
{
	/// <summary>
	/// Interaction logic for TextEntryControl.xaml
	/// </summary>
	public partial class TextEntryControl : UserControl {
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject ([In] IntPtr hObject);



		private TextEntryManager _textEntryManager;


		private void InitializeParsedTextBox () {
			if (_textEntryManager.IsTextSelected) {
				ParsedTextBox.Text = _textEntryManager.CurrentText.ParsedText;
				JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(_textEntryManager.CurrentText.ParsedText);
			}
		}

		

		public TextEntryControl (TextEntryManager textEntryManager) :base () {
			InitializeComponent();
			

			InitializeSAnimation();


			_textEntryManager = textEntryManager;
			if (_textEntryManager.IsTextSelected)
				LoadTextEntry();
			_textEntryManager.PageManager.TextEntryRequiresTranslation += OnTextEntryRequiresTranslation;
		}

		

		private void LoadTextEntry()
		{			
			_textEntryManager.CurrentText.TextChanged += OnTextContentChanged;
			
			InitializeParsedTextBox();

			TranslationSourcesStackPanel.Children.Clear();
			foreach (TranslationType t in _textEntryManager.TranslationManager.AvailableTranslations) {
				if (t.HasFlag(TranslationType.Text))
					TranslationSourcesStackPanel.Children.Add(new TranslationSourceView (
						_textEntryManager.TranslationManager, t, _textEntryManager.CurrentText));
			}

			SFXTranslationGrid.Children.Clear();
			SFXTranslationGrid.Children.Add(new TranslationSourceJadedNetworkView(_textEntryManager.TranslationManager, _textEntryManager.CurrentText));

			TranslatedTextBox.Text = _textEntryManager.CurrentText.TranslatedText;
			VerticalCheckBox.IsChecked = _textEntryManager.CurrentText.Vertical;

		}

		

		private void OnTextContentChanged(object sender, TxtChangedEventArgs e)
		{
			if (e.ChangeType == TextChangeType.Parse) {
				if (ParsedTextBox.Text != e.Text)
					ParsedTextBox.Text = e.Text;
				JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(e.Text);
				RefreshParseButton.IsEnabled = true;
				VerticalCheckBox.IsEnabled = true;
			}
			else if (e.ChangeType == TextChangeType.Translation)
				TranslatedTextBox.Text = e.Text;

		}

		private void RefreshParseButton_Click (object sender, RoutedEventArgs e) {
			Mouse.SetCursor(Cursors.Wait);
			RefreshParseButton.IsEnabled = false;
			VerticalCheckBox.IsEnabled = false;
			_textEntryManager.ReParse();			
			Mouse.SetCursor(Cursors.Arrow);
		}

		
		

		private void ParsedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (_textEntryManager.CurrentText.ParsedText != ParsedTextBox.Text)
				_textEntryManager.ChangeParsedText(ParsedTextBox.Text);
			CheckAnimation(ParsedTextBox.Text);
		}

		private void TranslatedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (_textEntryManager.CurrentText.TranslatedText != TranslatedTextBox.Text)
				_textEntryManager.CurrentText.TranslatedText = TranslatedTextBox.Text;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			ShowImageFromBitmap(_textEntryManager.CurrentText.Source);
		}
				
		private void ShowImageFromBitmap (Bitmap src) {
			var handle = src.GetHbitmap();
			try {
				ImageSource dest = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				PreviewIMG.Source = dest;
				double desiredSize = PreviewIMGBorder.ActualHeight * (dest.Width / dest.Height);
				if (desiredSize > ParsedTextAndIMGPreviewGrid.ActualWidth / 3)
					desiredSize = ParsedTextAndIMGPreviewGrid.ActualWidth / 3;
				PreviewIMG.Width = desiredSize;
			}
			finally { DeleteObject(handle); }
		}

		private void RefreshAllButton_Click(object sender, RoutedEventArgs e)
		{
			TranslateAll();
		}

		private void OnTextEntryRequiresTranslation(object sender, EventArgs e)
		{
			if ((bool)Properties.Settings.Default["AutoTranslateEnabled"])
				TranslateAll();
		}

		public void TranslateAll () {

			foreach (TranslationSourceView t in TranslationSourcesStackPanel.Children)
				t.AwaitTranslation();

			_textEntryManager.TranslationManager.TranslateAll();
			
			
			//It makes sense to not ask for an SFX translation by default.
			
		}

		

		
		private void VerticalCheckBox_Checked (object sender, RoutedEventArgs e) {
			if (_textEntryManager.IsTextSelected && !_textEntryManager.CurrentText.Vertical)
				_textEntryManager.SetVertical(true);
		}

		private void VerticalCheckBox_Unchecked (object sender, RoutedEventArgs e) {
			if (_textEntryManager.IsTextSelected && _textEntryManager.CurrentText.Vertical)
				_textEntryManager.SetVertical(false);
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
			if (parse.Contains("おかえりなのじゃ")) {
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
