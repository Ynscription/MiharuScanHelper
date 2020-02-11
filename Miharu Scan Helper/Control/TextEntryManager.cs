using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation;
using Miharu.BackEnd.Translation.Threading;
using System;

namespace Miharu.Control
{
	public class TextEntryManager
	{

		public event EventHandler TextChanged;
		public event EventHandler TextIndexChanged;

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


		public TextEntryManager(PageManager pageManager, TranslatorThread translatorThread)
		{
			PageManager = pageManager;
			TranslationManager = new TranslationManager(this, translatorThread);
		}

		

		internal void SelectTextEntry(Text entry, int index)
		{
			CurrentText = entry;
			CurrentTextIndex = index;
		}

		

		internal void Unload()
		{
			CurrentText = null;
			CurrentTextIndex = -1;
		}

		internal void RemovedTextEntry(Text textEntry)
		{
			if (textEntry == CurrentText)
				Unload();
		}

		internal void MovedTextEntry(int oldIndex, Text textEntry, int newIndex)
		{
			if (textEntry == CurrentText)
				CurrentTextIndex = newIndex;
			else if (newIndex == CurrentTextIndex)
				CurrentTextIndex = oldIndex;
		}

		internal void ReParse()
		{
			CurrentText.Invalidate();
			string tmp = CurrentText.ParsedText;
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		internal void ChangeParsedText(string text)
		{
			CurrentText.ParsedText = text;
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		internal void SetVertical(bool v)
		{
			CurrentText.Vertical = v;
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		internal void SetTranslation(TranslationType type, string translation)
		{
			CurrentText.SetTranslation(type, translation);
			PageManager.ChapterManager.IsChapterSaved = false;
		}

		internal void TranslationChanged(Text dest)
		{
			
		}
	}
}
