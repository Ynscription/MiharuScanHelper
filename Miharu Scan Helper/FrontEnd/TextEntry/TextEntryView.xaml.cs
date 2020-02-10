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
			_textEntryManager.TextIndexChanged += OnTextEntryIndexChanged;
			_pageManager = _textEntryManager.pageManager;
			_pageManager.PageChanged += OnPageChanged;
			_pageManager.TextEntryMoved += OnTextEntryMoved;
			_pageManager.TextEntryRemoved += OnTextEntryRemoved;
		}

		private void OnTextEntryRemoved(object sender, ListModificationEventArgs e)
		{
			TextEntriesStackPanel.Children.RemoveAt(e.EventOldIndex);
		}

		private void OnTextEntryMoved(object sender, ListModificationEventArgs e)
		{
			var tmp2 = TextEntriesStackPanel.Children[e.EventOldIndex];
			TextEntriesStackPanel.Children.RemoveAt(e.EventOldIndex);
			TextEntriesStackPanel.Children.Insert(e.EventNewIndex, tmp2);
		}

		private void OnPageChanged(object sender, EventArgs e)
		{
			if (_pageManager.IsPageLoaded) {
				if (!_pageManager.IsPageReady) {
					Mouse.SetCursor(Cursors.Wait);
					_pageManager.WaitForPage();
					Mouse.SetCursor(Cursors.Arrow);
				}

				TextEntriesStackPanel.Children.Clear();
				for (int i = 0; i < _pageManager.CurrentPageTextEntries.Count; i++) {
					TextEntriesStackPanel.Children.Add(
						new TextEntryListView(
							_pageManager.CurrentPageTextEntries[i],
							_pageManager));
				}
			}
			else
				TextEntriesStackPanel.Children.Clear();

			TextEntriesStackPanel.InvalidateVisual();
		}

		private void OnTextEntryChanged(object sender, EventArgs e)
		{
			if (_textEntryManager.CurrentText != null)
				TextEntryArea.Content = new TextEntryControl(_textEntryManager.CurrentText);
			else
				TextEntryArea.Content = null;
		}



		private int _previousIndex = -1;
		private void OnTextEntryIndexChanged(object sender, EventArgs e)
		{
			if (_previousIndex >= 0 && _previousIndex < TextEntriesStackPanel.Children.Count)
				((TextEntryListView)TextEntriesStackPanel.Children[_previousIndex]).Selected = false;
			if (_textEntryManager.CurrentTextIndex >= 0 && _textEntryManager.CurrentTextIndex < TextEntriesStackPanel.Children.Count)
				((TextEntryListView)TextEntriesStackPanel.Children[_textEntryManager.CurrentTextIndex]).Selected = true;
			_previousIndex = _textEntryManager.CurrentTextIndex;
		}
	}
}
