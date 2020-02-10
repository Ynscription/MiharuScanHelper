using Miharu.BackEnd;
using Miharu.BackEnd.Data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Miharu.Control
{
	public class PageManager
	{

		#region Events
		public event EventHandler PageChanged;
		public event EventHandler PageIndexChanged;
		public event ListModificationEventHandler TextEntryRemoved;
		public event ListModificationEventHandler TextEntryMoved;
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
			try {
				CurrentPage = ChapterManager.LoadedChapter.Pages[index];
				CurrentPageIndex = index;
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		public void AddTextEntry(Rect rect)
		{
			Text text = null;
			try {
				text = CurrentPage.AddTextEntry(rect);
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
			TextEntryManager.SelectTextEntry(text, CurrentPage.TextEntries.Count -1);
			ChapterManager.IsChapterSaved = false;
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

		internal void RemoveTextEntry(Text textEntry)
		{
			try {
				int index = CurrentPage.TextEntries.IndexOf(textEntry);
				CurrentPage.RemoveTextEntry(index);
				TextEntryManager.RemovedTextEntry(textEntry);
				TextEntryRemoved?.Invoke(this, new ListModificationEventArgs(index, textEntry));
				ChapterManager.IsChapterSaved = false;
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		internal void MoveTextEntry(Text textEntry, bool up)
		{
			try {
				int offset = up ? -1 : 1;
				int index = CurrentPage.TextEntries.IndexOf(textEntry);
				if ((up && index > 0) || (!up && index < CurrentPage.TextEntries.Count-1)) {
					CurrentPage.MoveTextEntry(index, index + offset);					
					TextEntryMoved?.Invoke(this, new ListModificationEventArgs(index, textEntry, index + offset));
					TextEntryManager.MovedTextEntry(index, textEntry, index + offset);
					ChapterManager.IsChapterSaved = false;
				}
			}
			catch(Exception e) {
				Logger.Log(e);
				throw e;
			}

		}

		internal void SelectTextEntry(Text textEntry)
		{
			int index = CurrentPage.TextEntries.IndexOf(textEntry);
			TextEntryManager.SelectTextEntry(textEntry, index);
		}
	}
}
