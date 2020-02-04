
using OpenQA.Selenium;
using System;

namespace Manga_Scan_Helper.BackEnd.Translation.WebCrawlers
{
	class WCGoogleTranslator : WebCrawlerTranslator {
	
		private const string _URL = "https://translate.google.com/#view=home&op=translate&sl=ja&tl=en&text=";

		public override TranslationType Type {
			get { return TranslationType.Google_Web; }
		}

		protected override By FetchBy {
			get { return By.XPath("//span[@class='tlid-translation translation']"); }
		}

		protected override string GetUri(string text)
		{
			return _URL + Uri.EscapeDataString(text);
		}
	}
}

