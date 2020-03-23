using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Miharu.BackEnd.Data
{
	public class DPIAwareRectangle
	{
		public double DpiX { get; set; }
		public double DpiY { get; set; }

		public Rect Rectangle { get; set; }

		public double X { get => Rectangle.X; }
		public double Y { get => Rectangle.Y; }
		public double Width { get => Rectangle.Width; }
		public double Height { get => Rectangle.Height; }


		public DPIAwareRectangle (double x, double y, double width, double height, double dpiX, double dpiY) {
			Rectangle = new Rect(x, y, width, height);
			DpiX = dpiX;
			DpiY = dpiY;
		}

		[JsonConstructor]
		public DPIAwareRectangle (Rect rectangle, double dpiX, double dpiY) {
			Rectangle = new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			DpiX = dpiX;
			DpiY = dpiY;
		}


		public Rect ConvertToDpi (double newDpiX, double newDpiY) {
			Rect res = new Rect(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
			if (newDpiX != DpiX) {
				double mod = DpiX / newDpiX;
				res.X = res.X * mod;
				res.Width = res.Width * mod;
			}
			if (newDpiY != DpiY) {
				double mod = DpiY / newDpiY;
				res.Y = res.Y * mod;
				res.Height = res.Height * mod;
			}
			return res;
		}
	}
}
