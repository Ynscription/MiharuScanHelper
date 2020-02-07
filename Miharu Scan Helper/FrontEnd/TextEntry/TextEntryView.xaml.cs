using Miharu.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Miharu.FrontEnd.TextEntry
{
	/// <summary>
	/// Interaction logic for TextEntryView.xaml
	/// </summary>
	public partial class TextEntryView : UserControl
	{
		private TextEntryManager _textEntryManager;
		private PageManager _pageManager;

		public TextEntryView(TextEntryManager textEntryManager)
		{
			InitializeComponent();
			_textEntryManager = textEntryManager;
			_textEntryManager.TextChanged += OnTextEntryChanged;
			_pageManager = _textEntryManager.pageManager;
		}

		private void OnTextEntryChanged(object sender, EventArgs e)
		{
			if (_textEntryManager.CurrentText != null)
				TextEntryArea.Content = new TextEntryControl(_textEntryManager.CurrentText);
			else
				TextEntryArea.Content = null;
		}
	}
}
