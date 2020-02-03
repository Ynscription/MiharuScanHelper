using Manga_Scan_Helper.BackEnd;
using Manga_Scan_Helper.BackEnd.Translation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Manga_Scan_Helper.FrontEnd
{
	/// <summary>
	/// Interaction logic for TranslationSourceJadedNetworkView.xaml
	/// </summary>
	public partial class TranslationSourceJadedNetworkView : UserControl
	{
		public TranslationType Type {
			get {
				return TranslationType.Jaded_Network;
			}
		}
		private Text _textEntry;
		private TextEntryControl _parent;
		private List<SFXContainer> _sfxEntries;
		private const string _JADED_NETWORK_URL = "http://thejadednetwork.com/sfx";

		public TranslationSourceJadedNetworkView(TextEntryControl parent, Text textEntry)
		{
			InitializeComponent();
			_textEntry = textEntry;
			_textEntry.TextChanged += TextEntry_TextChanged;
			_parent = parent;
			_parent.TranslationFail += OnTranslationFailed;
			_sfxEntries = new List<SFXContainer>();
			string src = null;
			if ((src = _textEntry.GetTranslation(Type)) != null)
				ProcessSourceSFX(src);
			SFXListBox.ItemsSource = _sfxEntries;
			SFXListBox.Items.Refresh();
		}

		private const string _BREAK = "<br />";
		private const string _START_TD = "<td>";
		private const string _END_TD = "</td>";
		private const string _START_KANA = "lang=\"ja\">";
		private const string _START_ROMAJI = "<td class=\"romaji\">";
		private const string _END_EXPLANATION = "<br /><br />";

		private void ProcessSourceSFX (string src) {

			bool end = false;

			List<SFXContainer> list = new List<SFXContainer>();
			
			while (!end) {
				SFXContainer sfxContainer = new SFXContainer();

				int kanaEndIndex = src.IndexOf(_END_TD);
				sfxContainer.Kana = src.Substring(0, kanaEndIndex);
				sfxContainer.Kana = sfxContainer.Kana.Replace(_BREAK, "");

				src = src.Substring(src.IndexOf(_START_ROMAJI) + _START_ROMAJI.Length);
				int romajiEndIndex =  src.IndexOf(_END_TD);
				sfxContainer.Romaji = src.Substring(0, romajiEndIndex);
			
				src = src.Substring(src.IndexOf(_START_TD) + _START_TD.Length);
				int engEndIndex = src.IndexOf(_END_TD);
				sfxContainer.English = src.Substring(0, engEndIndex);
				sfxContainer.English = sfxContainer.English.Replace(_BREAK, "");

				src = src.Substring(src.IndexOf(_START_TD) + _START_TD.Length);
				int explanationEndIndex = src.IndexOf(_END_EXPLANATION);
				sfxContainer.Explanation = src.Substring(0, explanationEndIndex);
				sfxContainer.Explanation = sfxContainer.Explanation.Replace(_BREAK, "");
				sfxContainer.Explanation = sfxContainer.Explanation.Replace("\t", "");
				sfxContainer.Explanation = sfxContainer.Explanation.Trim();

				list.Add(sfxContainer);
				if (!(end = src.IndexOf(_START_KANA) == -1))
					src = src.Substring(src.IndexOf(_START_KANA) + _START_KANA.Length);
			}
			_sfxEntries.Clear();
			_sfxEntries.AddRange(list);

		}

		

		private void TextEntry_TextChanged(object sender, TxtChangedEventArgs e)
		{
			if (e.TranslationType.HasValue && e.ChangeType == TextChangeType.TranslationSource) {
				if (e.TranslationType.Value == Type) {
					try {
						string src = null;
						if ((src = _textEntry.GetTranslation(Type)) != null)
							ProcessSourceSFX(src);
						SFXListBox.Items.Refresh();
						RefreshButton.IsEnabled = true;
					}
					catch (Exception ex) {
						OnTranslationFailed(this, new TranslationFailEventArgs(ex, Type));
					}
				}
			}
		}

		private void OnTranslationFailed(object sender, TranslationFailEventArgs e)
		{
			try {
				Dispatcher.Invoke(() => {
					if (e.Type == Type) {
						ErrorIMG.Visibility = Visibility.Visible;
						ErrorIMG.ToolTip = e.Exception.Message;
						RefreshButton.IsEnabled = true;
					}
				});
			}
			catch (TaskCanceledException) { }
		}

		private void SFXListBox_PreviewMouseWheel (object sender, MouseWheelEventArgs e) {
			MainScrollViewer.ScrollToVerticalOffset (MainScrollViewer.VerticalOffset - e.Delta);
			e.Handled = true;
		}



		private void RefreshButton_Click (object sender, RoutedEventArgs e) {
			ErrorIMG.Visibility = Visibility.Hidden;
			RefreshButton.IsEnabled = false;
			_parent.RequestTranslation(Type);
		}


		private void JadedNetworkButton_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(_JADED_NETWORK_URL);
		}


	}
}
