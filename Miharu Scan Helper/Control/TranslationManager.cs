using System;
using Miharu.BackEnd;
using Miharu.BackEnd.Translation;

namespace Miharu.Control
{
	public class TranslationManager : TranslationConsumer
	{


		public event TranslationFailEventHandler TranslationFail;

		public TextEntryManager TextEntryManager {
			get;
			private set;
		} = null;


		public TranslationManager (TextEntryManager textEntryManager) {
			TextEntryManager = textEntryManager;
		}

		
		public void RequestTranslation (TranslationType t) {
			TranslationProvider.Translate(t, TextEntryManager.CurrentText.ParsedText, this);
		}


		internal void TranslateAll()
		{
			foreach (TranslationType t in TranslationProvider.Instance) {
				if (t != TranslationType.Jaded_Network)
					TranslationProvider.Translate(t, TextEntryManager.CurrentText.ParsedText, this);
			}
		}




		public void TranslationCallback(string translation, TranslationType type)
		{
			translation = translation.Replace("\\\"", "\"");			
			TextEntryManager.SetTranslation(type, translation);
			
		}

		public void TranslationFailed(Exception e, TranslationType type)
		{
			Logger.Log(e);
			TranslationFail?.Invoke(this, new TranslationFailEventArgs(e, type));
		}
	}
}
