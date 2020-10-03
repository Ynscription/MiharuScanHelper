using Miharu.FrontEnd;
using Miharu.FrontEnd.Helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Miharu.BackEnd.Translation.WebCrawlers
{
	class WCYandexTranslator :WebCrawlerTranslator {
	
		private const string _URL = "https://translate.yandex.com/?lang=ja-en&text=";

		public WCYandexTranslator (WebDriverManager webDriverManager) :base(webDriverManager){

		}

		public override TranslationType Type {
			get { return TranslationType.Yandex_Web; }
		}

		protected override By FetchBy {
			get { return By.XPath("//span[@data-complaint-type='fullTextTranslation']"); }
		}

		protected override string GetUri(string text)
		{
			return _URL + Uri.EscapeDataString(text);
		}

		

		public IWebElement OverrideNavigation (IWebDriver driver, string url) {
			IWebElement result = null;

			driver.Navigate().GoToUrl(url);

			while (driver.Url.Contains("showcaptcha")) {
				IWebElement captcha = driver.FindElement(By.XPath("//div[@class='captcha__image']"));
				string captchaSrc = captcha.FindElement(By.XPath("//img")).GetAttribute("src");
				CaptchaDialog captchaDialog = null;
				Application.Current.Dispatcher.Invoke((Action)delegate{
					captchaDialog = new CaptchaDialog(captchaSrc);
					captchaDialog.ShowDialog();
				});
				

				if (captchaDialog.CaptchaInput == null)
					throw new Exception("Captcha was not solved.");

				IWebElement inputBox = driver.FindElement(By.XPath("//input[@class='input-wrapper__content']"));
				inputBox.SendKeys(captchaDialog.CaptchaInput);
				inputBox.Submit();
				Thread.Sleep(500);				
			}

			//Wait for result to be available
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
			result = wait.Until(ResultIsNotEmpty);
					

			return result;
		}

		public override async Task<string> Translate(string text)
		{
			string res = "";
			if (text == "")
				return res;
			res = _webDriverManager.NavigateAndFetch(GetUri(text), FetchBy, ProcessResult, OverrideNavigation);


			return res;
		}
	}
}
