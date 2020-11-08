
using MahApps.Metro.Controls;
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using Miharu.FrontEnd.Input;
using Miharu.FrontEnd.TextEntry;
using Ookii.Dialogs.Wpf;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
	public partial class TextEntryControl : UserControl {
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject ([In] IntPtr hObject);



		private TextEntryManager _textEntryManager;
		private KanjiInputManager _kanjiInputManager;

        


		private void InitializeParsedTextBox () {
			if (_textEntryManager.IsTextSelected) {
				ParsedTextBox.Text = _textEntryManager.CurrentText.ParsedText;
			}
		}

		

		public TextEntryControl (TextEntryManager textEntryManager, KanjiInputManager kanjiInputManager, KanjiByRadInputControl kanjiByRadInput) :base () {
			_textEntryManager = textEntryManager;
			_kanjiInputManager = kanjiInputManager;
			InitializeComponent();
			

			InitializeSAnimation();


			
			if (_textEntryManager.IsTextSelected)
				LoadTextEntry();
			_textEntryManager.PageManager.TextEntryRequiresTranslation += OnTextEntryRequiresTranslation;
			TextTabControl.SelectedIndex = _textEntryManager.TabIndex;
			
			KanjiInputExpander.Content = kanjiByRadInput;
			KanjiInputExpander.IsExpanded = _kanjiInputManager.KanjiInputWindowVisibility;
			_kanjiInputManager.VisibilityChanged += KanjiInputManager_VisibilityChanged;
			_kanjiInputManager.KanjiInputEvent += _kanjiInputManager_KanjiInputEvent;

		}

		

		private TextBox NewNoteTextBox (string s = "") {
			TextBox tb = new TextBox ();
			tb.TextWrapping = TextWrapping.Wrap;
			tb.FontSize = 16;
			tb.AcceptsReturn = true;
			tb.AcceptsTab = true;
			tb.IsReadOnly = false;
			tb.Margin = new Thickness(5);
			tb.Text = s;
			tb.TextChanged += OnNoteTextChanged;
			tb.LostFocus += OnNoteLostFocus;
			tb.Tag = null;
			return tb;
		}

		private void OnNoteLostFocus(object sender, RoutedEventArgs e)
		{
			TextBox senderTB = (TextBox)sender;
			int index = NotesStackPanel.Children.IndexOf(senderTB);
			if (_textEntryManager.IsTextSelected && index < _textEntryManager.CurrentTextNotesCount && senderTB.Text == "") {
				if (senderTB.Tag != null) {
					Guid guid = (Guid)senderTB.Tag;
					_textEntryManager.CurrentTextRemoveNote(guid);
					NotesStackPanel.Children.RemoveAt(index);
				}
			}
		}

		private void OnNoteTextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox senderTB = (TextBox)sender;
			int index = NotesStackPanel.Children.IndexOf(senderTB);
			if (_textEntryManager.IsTextSelected && index < _textEntryManager.CurrentTextNotesCount) {
				if (senderTB.Tag != null) {
					Guid guid = (Guid)senderTB.Tag;
					_textEntryManager.CurrentTextSetNote(guid, senderTB.Text);
				}
			}
			else {
				Guid guid = _textEntryManager.CurrentTextAddNote (senderTB.Text);
				senderTB.Tag = guid;
				NotesStackPanel.Children.Add(NewNoteTextBox());
			}

		}

		private void LoadTextEntry()
		{			
			_textEntryManager.CurrentText.TextChanged += OnTextContentChanged;
			
			InitializeParsedTextBox();

			TranslationSourcesStackPanel.Children.Clear();
			foreach (TranslationType t in _textEntryManager.TranslationManager.AvailableTranslations) {
				if (t.HasFlag(TranslationType.Web))
					TranslationSourcesStackPanel.Children.Add(new TranslationSourceView (
						_textEntryManager.TranslationManager, t, _textEntryManager.CurrentText));
			}

			SFXTranslationGrid.Children.Clear();
			SFXTranslationGrid.Children.Add(new TranslationSourceJadedNetworkView(_textEntryManager.TranslationManager, _textEntryManager.CurrentText));

			DictTranslationGrid.Children.Clear();
			DictTranslationGrid.Children.Add(new TranslationSourceJishoView(_textEntryManager.TranslationManager, _textEntryManager.CurrentText));

			NotesStackPanel.Children.Clear();
			foreach (Note n in _textEntryManager.CurrentTextNotesEnumerator) {
				TextBox tb = NewNoteTextBox(n.Content);
				tb.Tag = n.Uuid;
				NotesStackPanel.Children.Add(tb);
			}
			NotesStackPanel.Children.Add(NewNoteTextBox());

			TranslatedTextBox.Text = _textEntryManager.CurrentText.TranslatedText;
			VerticalToggleSwitch.IsOn = _textEntryManager.CurrentText.Vertical;

		}

		

		private void OnTextContentChanged(object sender, TxtChangedEventArgs e)
		{
			if (e.ChangeType == TextChangeType.Parse) {
				if (ParsedTextBox.Text != e.Text)
					ParsedTextBox.Text = e.Text;
				RefreshParseButton.IsEnabled = true;
				VerticalToggleSwitch.IsEnabled = true;
			}
			else if (e.ChangeType == TextChangeType.Translation)
				TranslatedTextBox.Text = e.Text;

		}

		private void RefreshParseButton_Click (object sender, RoutedEventArgs e) {
			try {
				Mouse.SetCursor(Cursors.Wait);
				RefreshParseButton.IsEnabled = false;
				VerticalToggleSwitch.IsEnabled = false;
				_textEntryManager.ReParse();			
				Mouse.SetCursor(Cursors.Arrow);
			}
			catch (Exception ex) {
				Mouse.SetCursor(Cursors.Arrow);
				RefreshParseButton.IsEnabled = true;
				VerticalToggleSwitch.IsEnabled = true;
				using (TaskDialog dialog = new TaskDialog()) {
					dialog.WindowTitle = "Error";
					dialog.MainIcon = TaskDialogIcon.Error;
					dialog.MainInstruction = "There was an error parsing the image.";
					dialog.Content = ex.Message;
					TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
					dialog.Buttons.Add(okButton);
					TaskDialogButton button = dialog.ShowDialog();
				}
			}
		}

		
		

		private void ParsedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (_textEntryManager.CurrentText.ParsedText != ParsedTextBox.Text)
				_textEntryManager.ChangeParsedText(ParsedTextBox.Text);
			CheckAnimation(ParsedTextBox.Text);
		}

		private void TranslatedTextBox_TextChanged (object sender, TextChangedEventArgs e) {
			if (_textEntryManager.CurrentText.TranslatedText != TranslatedTextBox.Text)
				_textEntryManager.ChangeTranslatedText(TranslatedTextBox.Text);
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

		

		
		private void VerticalToggleSwitched_Toggled (object sender, RoutedEventArgs e) {
			
			if (_textEntryManager.IsTextSelected && _textEntryManager.CurrentText.Vertical != VerticalToggleSwitch.IsOn)
				_textEntryManager.SetVertical(VerticalToggleSwitch.IsOn);
		}


		private void DeleteButton_Click (object sender, RoutedEventArgs e) {
			//TODO
			//_parent.RemoveTextEntry(this);
		}


		#region Animation


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
			if (parse.Contains("なのじゃ")) {
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



		#endregion 

		
		
		private void TextTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_textEntryManager.TabIndex = TextTabControl.SelectedIndex;
		}

		private void ParsedTextAndIMGPreviewGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (_textEntryManager.IsTextSelected)
				ShowImageFromBitmap(_textEntryManager.CurrentText.Source);
		}




		#region KanjiInput


		private void KanjiInputExpander_Collapsed(object sender, RoutedEventArgs e)
		{
			_kanjiInputManager.KanjiInputWindowVisibility = false;
			KanjiInputExpander.ReleaseMouseCapture();
		}

		private void KanjiInputExpander_Expanded(object sender, RoutedEventArgs e)
		{
			_kanjiInputManager.KanjiInputWindowVisibility = true;
			Mouse.Capture(KanjiInputExpander, CaptureMode.SubTree);
			KanjiInputExpander.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
			ParsedTextBox.GotMouseCapture += ParsedTextBox_GotMouseCapture;
		}

		private void ParsedTextBox_GotMouseCapture(object sender, MouseEventArgs e)
		{
			if (KanjiInputExpander.IsExpanded) {
				Mouse.Capture(KanjiInputExpander, CaptureMode.SubTree);
				KanjiInputExpander.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
			}
		}

		private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
		{
			_kanjiInputManager.KanjiInputWindowVisibility = false;

			ReleaseMouseCapture();
		}

		private void KanjiInputManager_VisibilityChanged(object sender, EventArgs e)
		{
			if (KanjiInputExpander.IsExpanded != _kanjiInputManager.KanjiInputWindowVisibility)
				KanjiInputExpander.IsExpanded = _kanjiInputManager.KanjiInputWindowVisibility;
		}

		private void _kanjiInputManager_KanjiInputEvent(object sender, KanjiInputEventArgs e)
		{
			int caretIndex = ParsedTextBox.CaretIndex;
			ParsedTextBox.Text = ParsedTextBox.Text.Insert(caretIndex, e.Character.Lit);
			ParsedTextBox.CaretIndex = caretIndex + e.Character.Lit.Length;
		}

		private void KanjiInputExpander_GotFocus(object sender, RoutedEventArgs e)
		{
			ParsedTextBox.Focus();
		}


		


		#endregion

	}
}
