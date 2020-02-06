using Miharu.BackEnd;
using Miharu.BackEnd.Data;
using System;

namespace Miharu.Control
{
	public class ChapterManager
	{

		#region Events

		public event EventHandler ChapterChanged;
		public event EventHandler SaveChanged;

		#endregion

		
		#region Data
				
		private Chapter _loadedChapter = null;
		private Chapter LoadedChapter {
			get => _loadedChapter;
			set {
				_loadedChapter = value;
				ChapterChanged?.Invoke(this, new EventArgs());
			}
		}
		public bool IsChapterLoaded {
			get { return _loadedChapter != null; }
		}
					   		
		
		private string _currentSaveFile = null;
		public  string CurrentSaveFile {
			get => _currentSaveFile;
			private set {
				_currentSaveFile = value;
			}
		}
		public bool SaveFileExists {
			get { return _currentSaveFile != null; }
		}


		private bool _isChapterSaved = false;
		public bool IsChapterSaved {
			get => _isChapterSaved;
			private set {
				_isChapterSaved = value;
				SaveChanged?.Invoke(this, new EventArgs());
			}
		}


			   		
		public PageManager PageManager {
			get;
			private set;
		}


		public bool AllPagesReady { 
			get {
				return LoadedChapter.AllPagesReady;
			}
		}

		#endregion


		public ChapterManager (string chapterToLoad = null) {
			try {
				if (chapterToLoad != null)
					LoadChapter(chapterToLoad);
				}
			catch (Exception e) {
				LoadedChapter = null;
				CurrentSaveFile = null;
				IsChapterSaved = false;
				Logger.Log(e);
				throw e;
			}


			PageManager = new PageManager(this);
		}

		
		

		public void NewChapter (string sourceFolder) {
			try {
				LoadedChapter = new Chapter(sourceFolder);
				PageManager.LoadPage(LoadedChapter.Pages[0], 0);
				CurrentSaveFile = null;
				IsChapterSaved = false;
			}
			catch (Exception e) {
				LoadedChapter = null;
				CurrentSaveFile = null;
				IsChapterSaved = false;
				Logger.Log(e);
				throw e;
			}
		}

		public void NewChapter (string [] files) {
			try {
				LoadedChapter = new Chapter(files);
				PageManager.LoadPage(LoadedChapter.Pages[0], 0);
				CurrentSaveFile = null;
				IsChapterSaved = false;
			}
			catch (Exception e) {
				LoadedChapter = null;
				CurrentSaveFile = null;
				IsChapterSaved = false;
				Logger.Log(e);
				throw e;
			}
		}

		public void LoadChapter (string source) {
			try {
				int index = 0;
				string finalSource = source;
				LoadedChapter = Chapter.Load(source, out index, out finalSource);
				PageManager.LoadPage(LoadedChapter.Pages[index], index);
				CurrentSaveFile = finalSource;
				IsChapterSaved = true;
			}
			catch (Exception e) {
				LoadedChapter = null;
				CurrentSaveFile = null;
				IsChapterSaved = false;
				Logger.Log(e);
				throw e;
			}
		}

		public void UnloadChapter()
		{
			LoadedChapter = null;
			CurrentSaveFile = null;
			IsChapterSaved = false;
		}
			   		 
		public void SaveChapter (string newSaveFile = null) {
			try {
				string file = newSaveFile ?? CurrentSaveFile;
				LoadedChapter.Save(file);
				CurrentSaveFile = file;
				IsChapterSaved = true;
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		

		public void ExportScript(string fileName)
		{
			try {
				LoadedChapter.ExportScript(fileName);
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		public void ExportJPScript(string fileName)
		{
			try {
				LoadedChapter.ExportJPScript(fileName);
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		public void ExportCompleteScript(string fileName)
		{
			try {
				LoadedChapter.ExportCompleteScript(fileName);
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}



		public void WaitForPages()
		{
			LoadedChapter.ChapterWaitHandle.WaitOne();
		}
	}
}
