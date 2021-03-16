using OpenQA.Selenium;
using System;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
	public class WCPapagoTranslator : WebCrawlerTranslator
	{
		private const string _URL = "https://papago.naver.com/?sk=ja&tk=en&st=";

		public WCPapagoTranslator(WebDriverManager webDriverManager) : base(webDriverManager)
		{
		}

		public override TranslationType Type {
			get { return TranslationType.Papago_Web; }
		}

		protected override By FetchBy {
			get { return By.XPath("//div[@class='edit_box___1KtZ3 active___3VPGL']"); }
		}

		protected override string GetUri(string text)
		{
			return _URL + Uri.EscapeDataString(text);
		}


		public override string ProcessResult(IWebElement result)
		{
			string res = "";

			result.FindElement(By.TagName("span"));

			res += result.Text;
				
			return res;
		}
	}
}
