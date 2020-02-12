using Miharu.BackEnd;
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation.Threading;
using System;
using System.Threading;

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
		public Chapter LoadedChapter {
			get => _loadedChapter;
			set {
				_loadedChapter = value;
				ChapterChanged?.Invoke(this, new EventArgs());
			}
		}
		public bool IsChapterLoaded {
			get { return _loadedChapter != null; }
		}
		public int ChapterTotalPages {
			get { return _loadedChapter.TotalPages; }
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
			set {
				_isChapterSaved = value;
				SaveChanged?.Invoke(this, new EventArgs());
			}
		}


			   		
		public PageManager PageManager {
			get;
			private set;
		} = null;


		public bool AllPagesReady { 
			get {
				return LoadedChapter.AllPagesReady;
			}
		}

		#endregion


		public ChapterManager (TranslatorThread translatorThread) {
			PageManager = new PageManager(this, translatorThread);
		}

		
		

		public void NewChapter (string sourceFolder) {
			try {
				UnloadChapter();
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
				UnloadChapter();
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
				UnloadChapter();
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
			PageManager.Unload();
		}

		public void ReloadChapter (int index = 0) {
			int innerIndex = index >= 0 && index < LoadedChapter.Pages.Count ? index : 0;
			Chapter aux = LoadedChapter;
			string source = CurrentSaveFile;
			UnloadChapter();
			LoadedChapter = aux;
			PageManager.LoadPage(LoadedChapter.Pages[innerIndex], innerIndex);
			CurrentSaveFile = source;
			IsChapterSaved = false;
		}
			   		 
		public void SaveChapter (string newSaveFile = null) {
			
			try {
				string file = newSaveFile ?? CurrentSaveFile;
				Monitor.Enter(LoadedChapter);
				try {
					LoadedChapter.Save(file, PageManager.CurrentPageIndex);
				}
				finally {
					Monitor.Exit(LoadedChapter);
				}
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



		


		public void AddPage(int v, string file)
		{
			LoadedChapter.AddPage(v, file);
			IsChapterSaved = false;
		}

		public void RemovePage(int index)
		{
			LoadedChapter.RemovePage(index);
			IsChapterSaved = false;
		}

		public void MovePageUp(int selectedIndex)
		{
			LoadedChapter.MovePageUp(selectedIndex);
			IsChapterSaved = false;
		}

		public void MovePageDown(int selectedIndex)
		{
			LoadedChapter.MovePageDown(selectedIndex);
			IsChapterSaved = false;
		}
	}
}
