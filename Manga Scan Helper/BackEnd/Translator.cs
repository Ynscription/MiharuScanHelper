
using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Manga_Scan_Helper.BackEnd
{
	public class TranslatorEventArgs : EventArgs {
		public string GoogleTranslatedText {
			get;
			set;
		}
		public string BingTranslatedText {
			get;
			set;
		}
		public TranslatorEventArgs () : base() {
			GoogleTranslatedText = "";
			BingTranslatedText = "";
		}
		public TranslatorEventArgs (string googleText, string bingText) : base() {
			GoogleTranslatedText = googleText;
			BingTranslatedText = bingText;
		}

	}


    class Translator
    {
		//private static readonly 
		private volatile static IWebDriver _driver;
		private static IWebDriver Driver {
			get {
				if (_driver == null)
					internalInnit();
				return _driver;
			}
		}

		public static event EventHandler TranslationResponse;
		public static event EventHandler TranslationCancel;
		public static event EventHandler TranslationStart;
		public static event EventHandler TranslationEnd;

		private static void internalInnit () {
			FirefoxDriverService ffds = FirefoxDriverService.CreateDefaultService();
			ffds.HideCommandPromptWindow = true; 
			FirefoxOptions ffo = new FirefoxOptions();
			ffo.PageLoadStrategy = PageLoadStrategy.Eager;
			ffo.SetPreference("Headless", true); 
			ffo.AddArgument("-headless");
			_driver = new FirefoxDriver(ffds, ffo);
						
		}

		public static void BeginInnit() {
			internalInnit();
		}

		public static void CleanUp() {
			while (_driver == null); //driver should never be null... hopefully
			
			_driver?.Close();
			_driver?.Quit();
		}

		public static readonly string _translateURL = "https://translate.google.com/#ja/en/";


		private static volatile string _textToTranslate;
		private static volatile Thread _translationThread;
		public static void Translate (string text) {
			if (text.Trim() == "")return;
			if (_translationThread != null && _translationThread.IsAlive)
				throw new Exception ("Must call Cancel before attempting a new translation.");
			_textToTranslate = text;

			ThreadStart ts = new ThreadStart (internalTranslate);
			_translationThread = new Thread(ts);
			_translationThread.Start();

		}


		private static void internalTranslate () {
			TranslationStart?.Invoke(null, EventArgs.Empty);
			string googleTranslation = "";
			string bingTranslation = "";
			try {
				
				Driver.Url = @"https://translate.google.com/#ja/en/";
				Driver.Navigate();

				IWebElement query = Driver.FindElement(By.Id("source"));
				query.SendKeys(_textToTranslate);

				WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
				wait.Timeout = TimeSpan.FromSeconds(10);
				wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@class='tlid-translation translation']")));
				
				googleTranslation = Driver.FindElement(By.XPath("//span[@class='tlid-translation translation']")).Text;


				
				Driver.Url = @"https://www.bing.com/translator/";
				Driver.Navigate();

				query = Driver.FindElement(By.Id("tta_srcsl"));
				query.Click();
				query = Driver.FindElement(By.XPath("//option[@value='ja']"));
				query.Click();
				query.FindElement(By.XPath("//textarea[@id='tta_input']"));
				query.SendKeys(_textToTranslate);

				wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
				wait.Timeout = TimeSpan.FromSeconds(10);
				wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class='tta_outtxt']")));

				IWebElement webE = Driver.FindElement(By.XPath("//div[@class='tta_outtxt']"));
				
				TranslationResponse?.Invoke(null, new TranslatorEventArgs(googleTranslation, bingTranslation));
			}
			catch (ThreadAbortException) {
				TranslationResponse?.Invoke(null, new TranslatorEventArgs(googleTranslation, bingTranslation));
			}
			catch (NoSuchElementException) {
				TranslationResponse?.Invoke(null, new TranslatorEventArgs(googleTranslation, bingTranslation));
			}
			catch (WebDriverTimeoutException) {
				TranslationResponse?.Invoke(null, new TranslatorEventArgs(googleTranslation, bingTranslation));
			}
			TranslationEnd?.Invoke(null, new TranslatorEventArgs());
		}

		public static void Cancel() {
			TranslationCancel?.Invoke(null, EventArgs.Empty);
			if (_translationThread != null && _translationThread.IsAlive)
				_translationThread.Abort();
		}
	}
}
