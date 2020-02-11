using Miharu.BackEnd.Data;
using System;

namespace Miharu.BackEnd.Translation {
	public interface TranslationConsumer {

		void TranslationCallback (Text destination, string translation, TranslationType type);
		void TranslationFailed(Exception e, TranslationType type);
	}
}
