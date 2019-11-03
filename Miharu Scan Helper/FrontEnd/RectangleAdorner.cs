

using Manga_Scan_Helper.BackEnd;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Manga_Scan_Helper.FrontEnd {
	internal class RectangleAdorner :Adorner{

		private List<Text> _textEntries;

		public Rect? DragRect;
		
		public RectangleAdorner (UIElement adornedElement, List<Text> textEntries)
		  : base(adornedElement) {
			_textEntries = textEntries;
		}

		protected override void OnRender (DrawingContext drawingContext) {

			Pen renderPen = new Pen(new SolidColorBrush(Colors.Red), 1.0f);
			
			
			foreach (Text t in _textEntries)
				drawingContext.DrawRectangle(null, renderPen, t.Rectangle);

			if (DragRect.HasValue)
				drawingContext.DrawRectangle(null, renderPen, DragRect.Value);

		}
	}
}
