using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd {
	public interface TranslationConsumer {

		void TranslationCallback (string translation, TranslationType type);
		void TranslationFailed(Exception e, TranslationType type);
	}
}
