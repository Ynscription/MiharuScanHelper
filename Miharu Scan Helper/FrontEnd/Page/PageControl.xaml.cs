using Miharu.BackEnd.Data;
using Miharu.Control;
using Ookii.Dialogs.Wpf;
using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Miharu.FrontEnd.Page
{
	/// <summary>
	/// Interaction logic for PageControl.xaml
	/// </summary>
	public partial class PageControl : UserControl
	{
		private double _dpiX;
		private double _dpiY;
		private PageManager _pageManager;
		private RectangleOverlay _rectangleOverlay;

		public PageControl(PageManager pageManager)
		{
			InitializeComponent();
			_pageManager = pageManager;
			_pageManager.PageChanged += OnPageChanged;
			_pageManager.PageIndexChanged += OnPageIndexChanged;
			Graphics g = Graphics.FromHwnd(IntPtr.Zero);
			_dpiX = g.DpiX;
			_dpiY = g.DpiY;
			_rectangleOverlay = new RectangleOverlay(PreviewIMG, _pageManager, _dpiX, _dpiY);
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(PreviewIMG);
			adornerLayer.Add(_rectangleOverlay);
		}

		

		private string _previousCurrPageTBText;

		private BitmapSource ChangeImageDPI (BitmapImage src) {
			int width = src.PixelWidth;
			int height = src.PixelHeight;
			PixelFormat pxFormat = src.Format;

			int stride = width * 4; // 4 bytes per pixel
			byte [] pixelData = new byte [stride * height];
			src.CopyPixels(pixelData, stride, 0);

			BitmapSource result = BitmapSource.Create(width, height, _dpiX, _dpiY, pxFormat, null, pixelData, stride);

			return result;
		}
		
		private void OnPageChanged (object sender, EventArgs e) {
			if (CurrPageTextBox.IsEnabled = _pageManager.IsPageLoaded) {
				CurrPageLabel.Text = (_pageManager.CurrentPageName);

				BitmapImage imgSrc = new BitmapImage();
				imgSrc.BeginInit();
				imgSrc.UriSource = new Uri(_pageManager.CurrentPagePath, UriKind.Relative);
				imgSrc.CacheOption = BitmapCacheOption.OnLoad;
				imgSrc.EndInit();

				if (imgSrc.DpiX != _dpiX || imgSrc.DpiY != _dpiY)
					PreviewIMG.Source = ChangeImageDPI(imgSrc);
				else
					PreviewIMG.Source = imgSrc;

				PreviewIMGScroll.ScrollToTop();
				PreviewIMGScroll.ScrollToRightEnd();

				if (!_pageManager.IsPageReady) {
					Mouse.SetCursor(Cursors.Wait);
					_pageManager.WaitForPage();
					Mouse.SetCursor(Cursors.Arrow);
				}

			}
			else {
				CurrPageLabel.Text = "";
				PreviewIMG.Source = null;
				PreviewIMG.InvalidateVisual();
			}
			_rectangleOverlay.InvalidateVisual();
			
		}

		private void OnPageIndexChanged(object sender, EventArgs e)
		{
			if (_pageManager.ChapterManager.IsChapterLoaded) {
				int totalPages = 0;
				if (_pageManager.ChapterManager.IsChapterLoaded && _pageManager.IsPageLoaded)
					totalPages = _pageManager.ChapterManager.ChapterTotalPages;					
				
				
				CurrPageTextBox.Text = (_pageManager.CurrentPageIndex + 1) + " / " + totalPages;
				_previousCurrPageTBText = CurrPageTextBox.Text;
				PrevPageButton.IsEnabled = _pageManager.CurrentPageIndex > 0;
				NextPageButton.IsEnabled = _pageManager.CurrentPageIndex < totalPages - 1;
			}
			else {
				CurrPageTextBox.Text = "/";
				_previousCurrPageTBText = CurrPageTextBox.Text;
				PrevPageButton.IsEnabled = false;
				NextPageButton.IsEnabled = false;
			}
		}


		private void NextPageButton_Click (object sender, RoutedEventArgs e) {
			if (_pageManager.CurrentPageIndex < _pageManager.ChapterManager.ChapterTotalPages - 1)
				_pageManager.NextPage();
		}

		private void PrevPageButton_Click (object sender, RoutedEventArgs e) {
			if (_pageManager.CurrentPageIndex > 0)
				_pageManager.PrevPage();
		}

		private void CurrPageTextBox_LostFocus (object sender, RoutedEventArgs e) {
			CurrPageTextBox.Text = _previousCurrPageTBText;
			Keyboard.ClearFocus();
		}

		private void CurrPageTextBox_PreviewKeyDown (object sender, KeyEventArgs e) {
			if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return) {
				e.Handled = true;
			}
		}

		private void CurrPageTextBox_PreviewKeyUp (object sender, KeyEventArgs e) {
			if (_previousCurrPageTBText != CurrPageTextBox.Text && (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)) {
				ParseCurrPageTextBox();
				Keyboard.ClearFocus();
				e.Handled = true;
			}
			else if (e.Key == System.Windows.Input.Key.Escape) {
				Keyboard.ClearFocus();
				CurrPageTextBox.Text = _previousCurrPageTBText;
				e.Handled = true;
			}

		}



		private void ParseCurrPageTextBox () {
			string significant = CurrPageTextBox.Text;
			if (significant.Contains("/"))
				significant = significant.Substring(0, significant.IndexOf('/'));
			int result = _pageManager.CurrentPageIndex;
			if (Int32.TryParse(significant, out result) && result > 0 && result <= _pageManager.ChapterManager.ChapterTotalPages) {
				_pageManager.ChangePage(result -1);
			}
			else {
				CurrPageTextBox.Text = _previousCurrPageTBText;
				System.Media.SystemSounds.Exclamation.Play();
			}


		}

		private void CurrPageTextBox_GotMouseCapture (object sender, MouseEventArgs e) {
			CurrPageTextBox.SelectAll();
		}


		#region ImageCropping

		//private ImageProcessing _imageProcessing;

		private void PreviewIMGScroll_PreviewMouseWheel (object sender, MouseWheelEventArgs e) {
			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
				PreviewIMGScroll.ScrollToHorizontalOffset(PreviewIMGScroll.HorizontalOffset - e.Delta);
				e.Handled = true;
			}
		}

		private System.Windows.Point _startingPoint;
		bool _previousMouseState = false;


		

		private void PreviewIMG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			System.Windows.Point mousePos = e.GetPosition(PreviewIMG);

			int index = _rectangleOverlay.NextRectangle(mousePos);
			if (index >= 0)
				_pageManager.SelectTextEntry (index);
		}


		private void PreviewIMG_PreviewMouseRightButtonDown (object sender, MouseButtonEventArgs e) {
			if (_pageManager.ChapterManager.IsChapterLoaded) {
				_startingPoint = e.GetPosition(PreviewIMG);

				_startingPoint = e.GetPosition(PreviewIMG);
				Rect rect = new Rect(_startingPoint, _startingPoint);
				_rectangleOverlay.DragRect = rect;
				_rectangleOverlay.InvalidateVisual();

				_previousMouseState = true;
			}

		}

		private void PreviewIMG_MouseMove (object sender, MouseEventArgs e) {
			if (_pageManager.ChapterManager.IsChapterLoaded) {
				System.Windows.Point mousePos = e.GetPosition(PreviewIMG);

				int index = _rectangleOverlay.NextRectangle(mousePos);
				_rectangleOverlay.MouseOverRect = index;
				_rectangleOverlay.InvalidateVisual();


				if (e.RightButton == MouseButtonState.Pressed && _previousMouseState) {
					Rect rect = new Rect(_startingPoint, e.GetPosition(PreviewIMG));
					_rectangleOverlay.DragRect = rect;
					_rectangleOverlay.InvalidateVisual();

				}
				else if (_previousMouseState) {
					Mouse.SetCursor(Cursors.Wait);
					_previousMouseState = false;
					DPIAwareRectangle rect = new DPIAwareRectangle (_rectangleOverlay.DragRect.Value.X,
											_rectangleOverlay.DragRect.Value.Y,
											_rectangleOverlay.DragRect.Value.Width,
											_rectangleOverlay.DragRect.Value.Height,
											_dpiX, _dpiY);
					_rectangleOverlay.DragRect = null;

					if (rect .Width == 0 || rect.Height == 0) {
						Mouse.SetCursor(Cursors.Arrow);
						_rectangleOverlay.InvalidateVisual();
						return;
					}
					_pageManager.AddTextEntry(rect);
					_rectangleOverlay.InvalidateVisual();

					Mouse.SetCursor(Cursors.Arrow);
				}
			}
		}



		#endregion
	}
}
