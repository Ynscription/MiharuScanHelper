
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

		public double DpiX;
		public double DpiY;

		private PageManager _pageManager;

		private Pen normalPen = new Pen(new SolidColorBrush(Colors.Red), 1.0f);
		private Pen highlightPen = new Pen(new SolidColorBrush(Colors.Red), 2.0f);
		private Pen selectedPen = new Pen(new SolidColorBrush(Colors.Blue), 1.0f);
		private Pen selectedHighlightPen = new Pen(new SolidColorBrush(Colors.Blue), 2.0f);


		public RectangleOverlay (UIElement adornedElement, PageManager pageManager, double dpiX, double dpiY)
		  : base(adornedElement) {
			MouseOverRect = -1;
			_pageManager = pageManager;
			_pageManager.PageChanged += OnPageChanged;
			_pageManager.TextEntryManager.TextIndexChanged += OnTextIndexChanged;
			_pageManager.TextEntryMoved += OnTextEntryMoved;
			DpiX = dpiX;
			DpiY = dpiY;
		}

		private void OnTextEntryMoved(object sender, ListModificationEventArgs e)
		{
			if (MouseOverRect == e.EventOldIndex)
				MouseOverRect = e.EventNewIndex;
		}

		private void OnTextIndexChanged(object sender, EventArgs e)
		{
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
				if (_textEntries[i].DpiAwareRectangle.ConvertToDpi(DpiX, DpiY).Contains(mousePos)) {
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
					if (i == _pageManager.TextEntryManager.CurrentTextIndex && i == MouseOverRect)
						drawingContext.DrawRectangle(null, selectedHighlightPen, _textEntries[i].DpiAwareRectangle.ConvertToDpi(DpiX, DpiY));
					else if (i == _pageManager.TextEntryManager.CurrentTextIndex)
						drawingContext.DrawRectangle(null, selectedPen, _textEntries[i].DpiAwareRectangle.ConvertToDpi(DpiX, DpiY));
					else if (i == MouseOverRect)
						drawingContext.DrawRectangle(null, highlightPen, _textEntries[i].DpiAwareRectangle.ConvertToDpi(DpiX, DpiY));
					else
						drawingContext.DrawRectangle(null, normalPen, _textEntries[i].DpiAwareRectangle.ConvertToDpi(DpiX, DpiY));
				}

				if (DragRect.HasValue)
					drawingContext.DrawRectangle(null, normalPen, DragRect.Value);
			}

		}
	}
}
