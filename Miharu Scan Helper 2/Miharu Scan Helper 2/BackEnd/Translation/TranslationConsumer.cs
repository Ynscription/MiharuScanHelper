using Miharu2.BackEnd.Data;
using System;

namespace Miharu2.BackEnd.Translation {
	public interface TranslationConsumer {

		void TranslationCallback (Text destination, string translation, TranslationType type);
		void TranslationFailed(Exception e, TranslationType type);
	}
}
