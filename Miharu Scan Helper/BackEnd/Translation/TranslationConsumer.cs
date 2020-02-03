using System;

namespace Manga_Scan_Helper.BackEnd.Translation {
	public interface TranslationConsumer {

		void TranslationCallback (string translation, TranslationType type);
		void TranslationFailed(Exception e, TranslationType type);
	}
}
