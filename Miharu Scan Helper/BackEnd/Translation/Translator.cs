using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
