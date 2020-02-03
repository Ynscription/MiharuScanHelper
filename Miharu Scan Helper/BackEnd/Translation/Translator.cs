

namespace Manga_Scan_Helper.BackEnd.Translation
{
	public abstract class Translator
	{

		public abstract string Translate (string text);

		public abstract TranslationType Type {
			get;
		}

	}
}
