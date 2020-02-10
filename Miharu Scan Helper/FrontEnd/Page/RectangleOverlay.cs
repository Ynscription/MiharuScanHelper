
using Miharu.BackEnd.Data;
using Miharu.Control;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Miharu.FrontEnd.Page
{
	class RectangleOverlay:Adorner{

		private List<Text> _textEntries {
			get; set;
		}

		public Rect? DragRect;

		public int MouseOverRect {
			get; set;
		}


		private PageManager _pageManager;

		private Pen normalPen = new Pen(new SolidColorBrush(Colors.Red), 1.0f);
		private Pen highlightPen = new Pen(new SolidColorBrush(Colors.Red), 2.0f);
		private Pen selectedPen = new Pen(new SolidColorBrush(Colors.Blue), 1.0f);
		private Pen selectedHighlightPen = new Pen(new SolidColorBrush(Colors.Blue), 2.0f);


		private int _page = -1;
		public RectangleOverlay (UIElement adornedElement, PageManager pageManager)
		  : base(adornedElement) {
			MouseOverRect = -1;
			_pageManager = pageManager;
			_pageManager.PageChanged += OnPageChanged;
			_pageManager.TextEntryManager.TextIndexChanged += OnTextIndexChanged;
			_pageManager.TextEntryMoved += OnTextEntryMoved;
			_page = _pageManager.CurrentPageIndex;
		}

		private void OnTextEntryMoved(object sender, ListModificationEventArgs e)
		{
			if (MouseOverRect == e.EventOldIndex)
				MouseOverRect = e.EventNewIndex;
		}

		private void OnTextIndexChanged(object sender, EventArgs e)
		{
			_page = _pageManager.CurrentPageIndex;
			MouseOverRect = NextRectangle(Mouse.GetPosition(this));
			InvalidateVisual();
		}

		private void OnPageChanged(object sender, EventArgs e)
		{
			if (_pageManager.IsPageLoaded)
				_textEntries = _pageManager.CurrentPageTextEntries;
			else
				_textEntries = null;
			InvalidateVisual();
		}

		
		public int NextRectangle (System.Windows.Point mousePos) {
			if (_textEntries == null)
				return -1;
			int index = -1;
			int firstIndex = -1;

			bool next = false;
			bool done = false;
			
			for (int i = 0; i < _textEntries.Count && !done; i++) {
				if (_textEntries[i].Rectangle.Contains(mousePos)) {
					if (index < 0) {
						index = i;
						firstIndex = i;
					}
					else if (next) {
						index = i;
						done = true;
					}
					next = i == _pageManager.TextEntryManager.CurrentTextIndex;
				}
			}

			if (!done)
				index = firstIndex;

			return index;
		}

	
		protected override void OnRender (DrawingContext drawingContext) {
			
			if (_textEntries != null) {
				for (int i = 0; i < _textEntries.Count; i++) {
					if (_page == _pageManager.CurrentPageIndex) {
						if (i == _pageManager.TextEntryManager.CurrentTextIndex && i == MouseOverRect)
							drawingContext.DrawRectangle(null, selectedHighlightPen, _textEntries[i].Rectangle);
						else if (i == _pageManager.TextEntryManager.CurrentTextIndex)
							drawingContext.DrawRectangle(null, selectedPen, _textEntries[i].Rectangle);
						else if (i == MouseOverRect)
							drawingContext.DrawRectangle(null, highlightPen, _textEntries[i].Rectangle);
						else
							drawingContext.DrawRectangle(null, normalPen, _textEntries[i].Rectangle);
					}
					else {
						if (i == MouseOverRect)
							drawingContext.DrawRectangle(null, highlightPen, _textEntries[i].Rectangle);
						else
							drawingContext.DrawRectangle(null, normalPen, _textEntries[i].Rectangle);
					}
				}

				if (DragRect.HasValue)
					drawingContext.DrawRectangle(null, normalPen, DragRect.Value);
			}

		}
	}
}
