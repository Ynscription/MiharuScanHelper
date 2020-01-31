
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Manga_Scan_Helper.BackEnd;

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
			if (Type == TranslationType.Yandex) {
				SourceLabel.Content = "Powered by Yandex.Translate";
				LinkLabel.Content = _YANDEX_LINK;
			}
			else
				SourceLabel.Content = Type.ToString();
			_textEntry = textEntry;
			_textEntry.TextChanged += TextEntry_TextChanged;
			_parent = parent;
			TranslationTextBox.Text = _textEntry.GetTranslation(Type);
		}

		private void TextEntry_TextChanged(object sender, TxtChangedEventArgs args)
		{
			if (args.TranslationType.HasValue && args.ChangeType == TextChangeType.TranslationSource) {
				if (args.TranslationType.Value == Type) {
					TranslationTextBox.Text = args.Text;
				}
			}
			
		}

		
		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			_parent.RequestTranslation(Type);
			_parent.TranslationFail += OnTranslationFailed;
		}

		private void OnTranslationFailed(object sender, TranslationFailEventArgs e)
		{
			try {
				Dispatcher.Invoke(() => {
					
				});
			}
			catch (TaskCanceledException) { }
		}


		private void LinkLabel_Click (object sender, RoutedEventArgs e) {
			System.Diagnostics.Process.Start((string)LinkLabel.Content);
		}
	}
}
