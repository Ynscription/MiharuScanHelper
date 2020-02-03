
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Manga_Scan_Helper.BackEnd;
using Manga_Scan_Helper.BackEnd.Translation;

namespace Manga_Scan_Helper.FrontEnd
{
	/// <summary>
	/// Interaction logic for TranslationSourceView.xaml
	/// </summary>
	public partial class TranslationSourceView : UserControl
	{

		private const string _YANDEX_LINK= "https://translate.yandex.com/";

		
		public TranslationType Type {
			get; private set;
		}
		private Text _textEntry;
		private TextEntryControl _parent;

		public TranslationSourceView(TextEntryControl parent, TranslationType type, Text textEntry)
		{
			InitializeComponent();
			Type = type;
			if (Type == TranslationType.Yandex_API) {
				SourceLabel.Content = "Powered by Yandex.Translate";
				LinkLabel.Content = _YANDEX_LINK;
			}
			else
				SourceLabel.Content = Type.ToString();
			_textEntry = textEntry;
			_textEntry.TextChanged += TextEntry_TextChanged;
			_parent = parent;
			_parent.TranslationFail += OnTranslationFailed;
			TranslationTextBox.Text = _textEntry.GetTranslation(Type);
		}

		private void TextEntry_TextChanged(object sender, TxtChangedEventArgs args)
		{
			if (args.TranslationType.HasValue && args.ChangeType == TextChangeType.TranslationSource) {
				if (args.TranslationType.Value == Type) {
					TranslationTextBox.Text = args.Text;
					RefreshButton.IsEnabled = true;
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

		public void AwaitTranslation () {
			RefreshButton.IsEnabled = false;
			ErrorIMG.Visibility = Visibility.Hidden;
			ErrorIMG.ToolTip = null;
		}
		
		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			_parent.RequestTranslation(Type);
			RefreshButton.IsEnabled = false;
			ErrorIMG.Visibility = Visibility.Hidden;
			ErrorIMG.ToolTip = null;
		}

		


		private void LinkLabel_Click (object sender, RoutedEventArgs e) {
			System.Diagnostics.Process.Start((string)LinkLabel.Content);
		}

	}
}
