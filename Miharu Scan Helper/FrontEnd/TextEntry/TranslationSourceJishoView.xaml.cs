using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.Control;
using Miharu.FrontEnd.TextEntry.JPWriting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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

		
		private const string _JISHO_URL = "https://jisho.org/search/";

		public TranslationType Type {
			get {
				return TranslationType.Jisho;
			}
		}


		private TranslationManager _translationManager;
		
		private List<Tuple<JPWord, string>> _currentWordMeanings;

		private JPDictionaryEntry _currentEntry;
		private JPDictionaryEntry CurrentEntry {
			get {
				return _currentEntry;
			}
			set {
				_currentEntry = value;
				_currentWordMeanings.Clear();
				if (_currentEntry != null) {
					if (_currentEntry.FormGuess == null) {
						FormBorder.Visibility = Visibility.Collapsed;
						FormSeparator.Visibility = Visibility.Collapsed;
					
						foreach (var m in _currentEntry.ExactMeanings)
							_currentWordMeanings.Add(m);
						foreach (var m in _currentEntry.Concepts)
							_currentWordMeanings.Add(m);
					}
					else {
						FormBorder.Visibility = Visibility.Visible;
						FormSeparator.Visibility = Visibility.Visible;

						StringBuilder sb = new StringBuilder();
						sb.Append(_currentEntry.Word);
						sb.Append(" could be an inflection of ");
						sb.Append(_currentEntry.FormGuess.Word);
						sb.Append(", with these forms: ");
						foreach (string l in _currentEntry.Forms) {
							sb.AppendLine();
							sb.Append("  · ");
							sb.Append(l);
						}
						FormTextBlock.Text = sb.ToString();

						HashSet<string> meanings = new HashSet<string>();
						foreach (var m in _currentEntry.FormGuess.ExactMeanings) {
							if (!meanings.Contains(m.Item1.Word)) {
								_currentWordMeanings.Add(m);
								meanings.Add(m.Item1.Word);
							}
						}
						foreach (var m in _currentEntry.ExactMeanings) {
							if (!meanings.Contains(m.Item1.Word)) {
								_currentWordMeanings.Add(m);
								meanings.Add(m.Item1.Word);
							}
						}
						foreach (var m in _currentEntry.Concepts) {
							if (!meanings.Contains(m.Item1.Word)) {
								_currentWordMeanings.Add(m);
								meanings.Add(m.Item1.Word);
							}
						}
					}
				}

				
				RefreshMeaningList();

			}
		}

		private void RefreshMeaningList()
		{
			MeaningsStackPanel.Children.Clear();
			foreach (var m in _currentWordMeanings) {
				MeaningsStackPanel.Children.Add (new JishoDictEntryView(m));
			}
		}

		private List<JPDictionaryEntry> _dictEntries;

		private List<JPDictionaryEntry> DictEntries {
			get {
				return _dictEntries;
			}
			set {
				_dictEntries = value;

				if (_dictEntries.Count == 1) {
					SentenceSeparator.Visibility = Visibility.Collapsed;
					SentenceGrid.Visibility = Visibility.Collapsed;

					CurrentEntry = _dictEntries[0];
				}
				else if (_dictEntries.Count > 1) {
					SentenceSeparator.Visibility = Visibility.Visible;
					SentenceGrid.Visibility = Visibility.Visible;

					SentenceWrapPanel.Children.Clear();

					bool currEntrySet = false;

					foreach (JPDictionaryEntry entry in _dictEntries) {
						JPHyperText jpht = new JPHyperText(entry.Word);
						jpht.Tag = entry;
						jpht.IsClickable = entry.ExactMeanings.Count + entry.Concepts.Count > 0 ||
							(entry.FormGuess != null && (entry.FormGuess.ExactMeanings.Count + entry.FormGuess.Concepts.Count > 0));
						if (jpht.IsClickable) {
							jpht.OnHyperTextClick += Jpht_OnHyperTextClick;
							if (!currEntrySet) {
								CurrentEntry = entry;
								currEntrySet = true;
							}
						}
						SentenceWrapPanel.Children.Add(jpht);
					}
				}
				else {
					SentenceSeparator.Visibility = Visibility.Collapsed;
					SentenceGrid.Visibility = Visibility.Collapsed;

					CurrentEntry = null;

					SentenceWrapPanel.Children.Clear();
				}

			}
		}

		private void Jpht_OnHyperTextClick(object sender, EventArgs e)
		{
			try {
				CurrentEntry = (JPDictionaryEntry)sender;
			}
			catch (Exception) { }
		}

		public TranslationSourceJishoView(TranslationManager translationManager, Text textEntry)
		{
			
			InitializeComponent();

			_currentWordMeanings = new List<Tuple<JPWord, string>>();
			

			textEntry.TextChanged += TextEntry_TextChanged;

			_translationManager = translationManager;
			_translationManager.TranslationFail += OnTranslationFailed;


			JishoLinkLabel.Content = _JISHO_URL + Uri.EscapeDataString(textEntry.ParsedText);

			string src = null;
			try {
				if ((src = textEntry.GetTranslation(Type)) != null)
					DictEntries = JsonConvert.DeserializeObject<List<JPDictionaryEntry>>(src);
			}
			catch (Exception e) {
				WorkingRect.Visibility = Visibility.Hidden;
				WorkingRect.ToolTip = null;

				ErrorRect.Visibility = Visibility.Visible;
				ErrorRect.ToolTip = "Error while processing source.";
					
				SuccessRect.Visibility = Visibility.Hidden;
			}
			
		}

		

		private void TextEntry_TextChanged(object sender, TxtChangedEventArgs e)
		{
			
			if (e.TranslationType.HasValue && e.ChangeType == TextChangeType.TranslationSource) {
				if (e.TranslationType.Value == Type) {
					try {
						Dispatcher.Invoke(() => {
							string src = null;
							try {
								if ((src = _translationManager.TextEntryManager.CurrentText.GetTranslation(Type)) != null)
									DictEntries = JsonConvert.DeserializeObject<List<JPDictionaryEntry>>(src);

								RefreshButton.IsEnabled = true;

								WorkingRect.Visibility = Visibility.Hidden;
								WorkingRect.ToolTip = null;

								ErrorRect.Visibility = Visibility.Hidden;
								ErrorRect.ToolTip = null;
					
								SuccessRect.Visibility = Visibility.Visible;
							}
							catch (Exception ex) {
								RefreshButton.IsEnabled = true;

								WorkingRect.Visibility = Visibility.Hidden;
								WorkingRect.ToolTip = null;

								ErrorRect.Visibility = Visibility.Visible;
								ErrorRect.ToolTip = "Error while processing source.";
					
								SuccessRect.Visibility = Visibility.Hidden;
							}
						});
					}
					catch (Exception ex) {
						OnTranslationFailed(this, new TranslationFailEventArgs(ex, Type));
					}
				}
			}
			else if (e.ChangeType == TextChangeType.Parse) {
				Dispatcher.Invoke(() => {
					JishoLinkLabel.Content = _JISHO_URL + Uri.EscapeDataString(e.Text);
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
