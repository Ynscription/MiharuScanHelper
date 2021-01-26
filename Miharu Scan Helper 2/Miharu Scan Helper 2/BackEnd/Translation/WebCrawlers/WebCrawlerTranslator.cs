using OpenQA.Selenium;

using System.Threading.Tasks;

namespace Miharu2.BackEnd.Translation.WebCrawlers
{
	public abstract class WebCrawlerTranslator : Translator
	{
		protected abstract string GetUri (string text);

		protected abstract By FetchBy {
			get;
		}

		protected WebDriverManager _webDriverManager;

		public WebCrawlerTranslator(WebDriverManager webDriverManager) {
			_webDriverManager = webDriverManager;
		}

		public virtual string ProcessResult (IWebElement result) {
			return result.Text;
		}

		public IWebElement ResultIsNotEmpty (IWebDriver driver) {
			string res = "";
			while ((res = driver.FindElement(FetchBy).Text) == "");
			return driver.FindElement(FetchBy);
		}

		public override async Task<string> Translate(string text)
		{
			string res = "";
			if (text == "")
				return res;

			res = _webDriverManager.NavigateAndFetch(GetUri(text), FetchBy, ProcessResult);


			return res;
		}
	}
}
