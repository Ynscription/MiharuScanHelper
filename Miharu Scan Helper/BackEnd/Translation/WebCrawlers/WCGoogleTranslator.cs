using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.WebCrawlers
{
	class WCGoogleTranslator : WebCrawlerTranslator
	{
		public override TranslationType Type {
			get { return TranslationType.Google_Web; }
		}

		public override string Translate(string text)
		{
			return "";
		}
	}
}
