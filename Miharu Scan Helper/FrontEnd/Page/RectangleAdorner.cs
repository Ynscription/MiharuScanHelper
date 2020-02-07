

using Miharu.BackEnd.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Miharu.FrontEnd.Page {
	internal class RectangleAdorner :Adorner{

		private List<Text> _textEntries;

		public Rect? DragRect;

		public int MouseOverRect {
			get; set;
		}
		public int SelectedRect {
			get; set;
		}

		private Pen normalPen = new Pen(new SolidColorBrush(Colors.Red), 1.0f);
		private Pen highlightPen = new Pen(new SolidColorBrush(Colors.Red), 2.0f);
		private Pen selectedPen = new Pen(new SolidColorBrush(Colors.Blue), 1.0f);
		private Pen selectedHighlightPen = new Pen(new SolidColorBrush(Colors.Blue), 2.0f);

		
		public RectangleAdorner (UIElement adornedElement, List<Text> textEntries)
		  : base(adornedElement) {
			_textEntries = textEntries;
			MouseOverRect = -1;
			SelectedRect = -1;
		}

	
		protected override void OnRender (DrawingContext drawingContext) {
			
			for (int i = 0; i < _textEntries.Count; i++) {
				if (i == SelectedRect && i == MouseOverRect)
					drawingContext.DrawRectangle(null, selectedHighlightPen, _textEntries[i].Rectangle);
				else if (i == SelectedRect)
					drawingContext.DrawRectangle(null, selectedPen, _textEntries[i].Rectangle);
				else if (i == MouseOverRect)
					drawingContext.DrawRectangle(null, highlightPen, _textEntries[i].Rectangle);
				else
					drawingContext.DrawRectangle(null, normalPen, _textEntries[i].Rectangle);
			}

			if (DragRect.HasValue)
				drawingContext.DrawRectangle(null, normalPen, DragRect.Value);

		}
	}
}
