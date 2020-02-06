
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;

namespace Miharu.FrontEnd
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

					WorkingRect.Visibility = Visibility.Hidden;
					WorkingRect.ToolTip = null;

					ErrorRect.Visibility = Visibility.Hidden;
					ErrorRect.ToolTip = null;
					
					SuccessRect.Visibility = Visibility.Visible;
			}
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
			_parent.RequestTranslation(Type);
			AwaitTranslation();
		}

		


		private void LinkLabel_Click (object sender, RoutedEventArgs e) {
			System.Diagnostics.Process.Start((string)LinkLabel.Content);
		}

	}
}
