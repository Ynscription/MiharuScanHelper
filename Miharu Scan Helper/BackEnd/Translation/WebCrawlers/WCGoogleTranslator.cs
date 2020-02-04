
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.WebCrawlers
{
	class WCGoogleTranslator : WebCrawlerTranslator {
	
		private const string _URL = "https://translate.google.com/#view=home&op=translate&sl=ja&tl=en&text=";

		public override TranslationType Type {
			get { return TranslationType.Google_Web; }
		}

		

		public override async Task<string> Translate(string text)
		{
			string res = "";

			res = WebDriverManager.NavigateAndFetch(_URL + Uri.EscapeDataString(text), By.XPath("//span[@class='tlid-translation translation']"));


			return res;
		}
	}
}
