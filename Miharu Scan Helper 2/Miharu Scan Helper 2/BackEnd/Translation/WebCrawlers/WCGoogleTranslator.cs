
using OpenQA.Selenium;
using System;

namespace Miharu2.BackEnd.Translation.WebCrawlers
{
	class WCGoogleTranslator : WebCrawlerTranslator {
	
		private const string _URL = "https://translate.google.com/#view=home&op=translate&sl=ja&tl=en&text=";

		public WCGoogleTranslator(WebDriverManager webDriverManager) : base(webDriverManager)
		{
		}

		public override TranslationType Type {
			get { return TranslationType.Google_Web; }
		}

		protected override By FetchBy {
			get { return By.XPath("//div[@class='zkZ4Kc dHeVVb']"); }
		}

		protected override string GetUri(string text)
		{
			return _URL + Uri.EscapeDataString(text);
		}


		public override string ProcessResult(IWebElement result)
		{
			string res = "";

			res += result.GetAttribute("data-text");
				
			return res;
		}
	}
}

