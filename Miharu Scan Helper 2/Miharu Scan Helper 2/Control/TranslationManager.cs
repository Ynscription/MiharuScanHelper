using System;
using System.Collections.Generic;
using System.Threading;
using Miharu2.BackEnd;
using Miharu2.BackEnd.Data;
using Miharu2.BackEnd.Translation;
using Miharu2.BackEnd.Translation.Threading;

namespace Miharu2.Control
{
	public class TranslationManager : TranslationConsumer
	{
		private TranslatorThread _translatorThread;
		public bool IsWebDriverAvailable {
			get { return _translatorThread.IsWebDriverAvailable; }
		}


		public event TranslationFailEventHandler TranslationFail;

		public TextEntryManager TextEntryManager {
			get;
			private set;
		} = null;


		public TranslationManager (TextEntryManager textEntryManager, TranslatorThread translatorThread) {
			TextEntryManager = textEntryManager;
			_translatorThread = translatorThread;
		}


		public void RequestTranslation (TranslationType t) {
			_translatorThread.Translate(new TranslationRequest(TextEntryManager.CurrentText, t, TextEntryManager.CurrentText.ParsedText, this));
		}


		public void TranslateAll()
		{
			_translatorThread.TranslateAll(new TranslationRequest(TextEntryManager.CurrentText, null, TextEntryManager.CurrentText.ParsedText, this));
		}




		public void TranslationCallback(Text dest, string translation, TranslationType type)
		{
			if (type != TranslationType.Jisho)
				translation = translation.Replace("\\\"", "\"");
			Monitor.Enter(TextEntryManager.PageManager.ChapterManager.LoadedChapter);
			try {
				dest.SetTranslation(type, translation);
			}
			finally {
				Monitor.Exit(TextEntryManager.PageManager.ChapterManager.LoadedChapter);
			}
			TextEntryManager.PageManager.ChapterManager.IsChapterSaved = false;
			TextEntryManager.TranslationChanged(dest);

		}

		public void TranslationFailed(Exception e, TranslationType type)
		{
			Logger.Log(e);
			TranslationFail?.Invoke(this, new TranslationFailEventArgs(e, type));
		}

		public IEnumerable<TranslationType> AvailableTranslations{
			get {
				return _translatorThread.AvailableTranslations;
			}
		}
	}
}
