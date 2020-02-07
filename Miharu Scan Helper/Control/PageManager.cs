using Miharu.BackEnd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Miharu.Control
{
	public class PageManager
	{

		#region Events
		public event EventHandler PageChanged;
		public event EventHandler PageIndexChanged;
		#endregion

		#region Data
		public ChapterManager ChapterManager {
			get; private set;
		} = null;

		public TextEntryManager TextEntryManager {
			get; private set;
		} = null;

		
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
		public bool IsPageReady {
			get { return _currentPage.Ready; }
		}
		public string CurrentPagePath {
			get { return CurrentPage.Path; }
		}
		public string CurrentPageName {
			get { return CurrentPage.Name; }
		}

		public List<Text> CurrentPageTextEntries {
			get { return CurrentPage.TextEntries; }
		}


		
		private int _currentPageIndex = 0;
		public int CurrentPageIndex {
			get => _currentPageIndex;
			set {
				_currentPageIndex = value;
				PageIndexChanged?.Invoke(this, new EventArgs());
			}
		}

		#endregion



		public PageManager (ChapterManager chapterManager) {
			ChapterManager = chapterManager;
			TextEntryManager = new TextEntryManager(this);
		}

		public void LoadPage(Page page, int index)
		{
			CurrentPage = page;
			CurrentPageIndex = index;
		}


		public void WaitForPage () {
			CurrentPage.PageWaitHandle.WaitOne();
		}



		public void NextPage()
		{
			ChangePage(CurrentPageIndex +1);
		}

		public void PrevPage()
		{
			ChangePage(CurrentPageIndex -1);
		}

		public void ChangePage(int index)
		{
			CurrentPageIndex = index;
			CurrentPage = ChapterManager.LoadedChapter.Pages[index];
		}

		public void AddTextEntry(Rect rect)
		{
			Text text = CurrentPage.AddTextEntry(rect);
			TextEntryManager.SelectTextEntry(text, CurrentPage.TextEntries.Count -1);
		}

		public void SelectTextEntry(int index)
		{
			if (index < 0 || index >= CurrentPage.TextEntries.Count) 
				TextEntryManager.SelectTextEntry(null, index);
			else
				TextEntryManager.SelectTextEntry(CurrentPage.TextEntries[index], index);
			
		}

		public  void Unload()
		{
			CurrentPage = null;
			CurrentPageIndex = 0;
			TextEntryManager.Unload();
		}
	}
}
