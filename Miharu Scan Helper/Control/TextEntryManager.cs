using Miharu.BackEnd;
using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.BackEnd.Translation.Threading;
using System;
using System.Collections.Generic;

namespace Miharu.Control
{
	public class TextEntryManager
	{

		public event EventHandler TextChanged;
		public event EventHandler TextIndexChanged;

		public int TabIndex {
			get; set;
		} = 0;

		public PageManager PageManager {
			get;
			private set;
		} = null;

		public TranslationManager TranslationManager {
			get;
			private set;
		} = null;


		private Text _currentText = null;
		public Text CurrentText {
			get => _currentText;
			set {
				_currentText = value;
				TextChanged?.Invoke(this, new EventArgs());
			}
		}
		public bool IsTextSelected {
			get { return CurrentText != null; }
		}

		private int _currentTextIndex = -1;
		public int CurrentTextIndex {
			get => _currentTextIndex;
			set {
				_currentTextIndex = value;
				TextIndexChanged?.Invoke(this, new EventArgs());
			}
		}


		public int CurrentTextNotesCount {
			get => _currentText.NotesCount;
		}
		public IEnumerable<Note> CurrentTextNotesEnumerator {
			get => _currentText.NotesEnumerator;
		}
		public float Rotation { 
			get => _currentText.Rotation;
			set {
				_currentText.Rotation = value;
				PageManager.ChapterManager.IsChapterSaved = false;

			}
		}

		public Guid CurrentTextAddNote (string text) {
			PageManager.ChapterManager.IsChapterSaved = false;
			return _currentText.AddNote(text);
		}
		public void CurrentTextRemoveNote (Guid guid) {
			_currentText.RemoveNote(guid);
			PageManager.ChapterManager.IsChapterSaved = false;
		}
		public void CurrentTextSetNote (Guid guid, string text) {
			_currentText.SetNote(guid, text);
			PageManager.ChapterManager.IsChapterSaved = false;
		}
		public Note CurrentTextGetNote (Guid guid) {
			return _currentText.GetNote(guid);
		}


		public TextEntryManager(PageManager pageManager, TranslatorThread translatorThread)
		{
			PageManager = pageManager;
			PageManager.PageChanged += OnPageChanged;
			TranslationManager = new TranslationManager(this, translatorThread);
		}

		

		public void SelectTextEntry(Text entry, int index)
		{
			CurrentText = entry;
			CurrentTextIndex = index;
		}



		public void Unload()
		{
			CurrentText = null;
			CurrentTextIndex = -1;
		}

		public void RemovedTextEntry(Text textEntry)
		{
			if (textEntry == CurrentText)
				Unload();
		}

		public void MovedTextEntry(int oldIndex, Text textEntry, int newIndex)
		{
			if (textEntry == CurrentText)
				CurrentTextIndex = newIndex;
			else if (newIndex == CurrentTextIndex)
				CurrentTextIndex = oldIndex;
		}

		public string ReParse()
		{
			try {
				CurrentText.Invalidate();
				string res = CurrentText.ParsedText;
				PageManager.ChapterManager.IsChapterSaved = false;
				return res;
			}
			catch (Exception e) {
				Logger.Log(e);
				throw e;
			}
		}

		public void ChangeParsedText(string text)
		{
			CurrentText.ParsedText = text;
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		public void SetVertical(bool v)
		{
			CurrentText.Vertical = v;
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		public void SetTranslation(TranslationType type, string translation)
		{
			CurrentText.SetTranslation(type, translation);
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		public void TranslationChanged(Text dest)
		{

		}

		public void ChangeTranslatedText(string text)
		{
			CurrentText.TranslatedText = text;
			PageManager.ChapterManager.IsChapterSaved = false;
		}


		private void OnPageChanged(object sender, EventArgs e)
		{
			if (PageManager.IsPageLoaded) {
				if (PageManager.CurrentPageTextEntries.Count > 0)
					SelectTextEntry(PageManager.CurrentPageTextEntries[0], 0);
				else
					SelectTextEntry(null, 0);
			}
		}

	}
}
