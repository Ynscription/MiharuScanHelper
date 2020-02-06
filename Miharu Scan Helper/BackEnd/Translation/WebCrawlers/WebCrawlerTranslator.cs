using OpenQA.Selenium;

using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
	public abstract class WebCrawlerTranslator : Translator
	{
		protected abstract string GetUri (string text);

		protected abstract By FetchBy {
			get;
		}

		public virtual string ProcessResult (IWebElement result) {
			return result.Text;
		}

		public override async Task<string> Translate(string text)
		{
			string res = "";

			res = WebDriverManager.NavigateAndFetch(GetUri(text), FetchBy, ProcessResult);


			return res;
		}
	}
}
