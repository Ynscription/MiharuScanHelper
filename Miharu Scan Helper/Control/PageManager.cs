using Miharu.BackEnd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu.Control
{
	public class PageManager
	{

		private ChapterManager _chapterManager = null;

		public event EventHandler PageChanged;
		private Page _currentPage = null;
		private Page CurrentPage {
			get => _currentPage;
			set {
				_currentPage = value;
				PageChanged?.Invoke(this, new EventArgs());
			}
		}
		public bool IsPageLoaded {
			get { return _currentPage != null; }
		}


		public event EventHandler PageIndexChanged;
		private int _currentPageIndex = 0;
		public int CurrentPageIndex {
			get => _currentPageIndex;
			set {
				_currentPageIndex = value;
				PageIndexChanged?.Invoke(this, new EventArgs());
			}
		}

		public PageManager (ChapterManager chapterManager) {
			_chapterManager = chapterManager;
		}

		public void LoadPage(Page page, int index)
		{
			CurrentPage = page;
			CurrentPageIndex = index;
		}
	}
}
