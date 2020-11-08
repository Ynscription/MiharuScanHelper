using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Miharu.FrontEnd.TextEntry
{
	/// <summary>
	/// Interaction logic for TranslationSourceJisho.xaml
	/// </summary>
	public partial class TranslationSourceJishoView : UserControl
	{


		public TranslationType Type {
			get {
				return TranslationType.Jisho;
			}
		}


		private TranslationManager _translationManager;
		private List<JPDictionaryEntry> _dictEntries;
		private List<Tuple<string, string>> _currentWordMeanings;
		private const string _JADED_NETWORK_URL = "http://thejadednetwork.com/sfx";

		public TranslationSourceJishoView(TranslationManager translationManager, Text textEntry)
		{
			InitializeComponent();

			textEntry.TextChanged += TextEntry_TextChanged;

			_translationManager = translationManager;
			_translationManager.TranslationFail += OnTranslationFailed;


			JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(textEntry.ParsedText);

			_dictEntries = new List<JPDictionaryEntry>();
			string src = null;
			if ((src = textEntry.GetTranslation(Type)) != null)
				ProcessSourceDict(src);
			WordListBox.ItemsSource = _currentWordMeanings;
			WordListBox.Items.Refresh();
		}

		private void ProcessSourceDict(string src)
		{
			
		}

		private void TextEntry_TextChanged(object sender, TxtChangedEventArgs e)
		{
			
			if (e.TranslationType.HasValue && e.ChangeType == TextChangeType.TranslationSource) {
				if (e.TranslationType.Value == Type) {
					try {
						Dispatcher.Invoke(() => {
							string src = null;
							if ((src = _translationManager.TextEntryManager.CurrentText.GetTranslation(Type)) != null)
								ProcessSourceDict(src);
							WordListBox.Items.Refresh();
							RefreshButton.IsEnabled = true;

							WorkingRect.Visibility = Visibility.Hidden;
							WorkingRect.ToolTip = null;

							ErrorRect.Visibility = Visibility.Hidden;
							ErrorRect.ToolTip = null;
					
							SuccessRect.Visibility = Visibility.Visible;
						});
					}
					catch (Exception ex) {
						OnTranslationFailed(this, new TranslationFailEventArgs(ex, Type));
					}
				}
			}
			else if (e.ChangeType == TextChangeType.Parse) {
				Dispatcher.Invoke(() => {
					JishoLinkLabel.Content = "https://jisho.org/search/" + Uri.EscapeDataString(e.Text);
				});
			}
			
		}


		private void OnTranslationFailed(object sender, TranslationFailEventArgs e)
		{
			try {
				Dispatcher.Invoke(() => {
					if (e.Type == Type) {
						RefreshButton.IsEnabled = true;

						SuccessRect.Visibility = Visibility.Hidden;

						WorkingRect.Visibility = Visibility.Hidden;
						WorkingRect.ToolTip = null;

						ErrorRect.Visibility = Visibility.Visible;
						ErrorRect.ToolTip = e.Exception.Message;
					}
				});
			}
			catch (TaskCanceledException) { }
		}


		private void WordListBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			MainScrollViewer.ScrollToVerticalOffset (MainScrollViewer.VerticalOffset - e.Delta);
			e.Handled = true;
		}


		public void AwaitTranslation () {
			RefreshButton.IsEnabled = false;

			SuccessRect.Visibility = Visibility.Hidden;

			ErrorRect.Visibility = Visibility.Hidden;
			ErrorRect.ToolTip = null;

			WorkingRect.Visibility = Visibility.Visible;
			WorkingRect.ToolTip = "Working...";
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			AwaitTranslation();
			_translationManager.RequestTranslation(Type);	
		}

		
	}
}
