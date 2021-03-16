using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Translation.WebCrawlers
{
	public class WebDriverManager : IDisposable
	{
		public const string GECKO_DRIVER_PATH = @".\Resources\Redist\GeckoDriver\geckodriver.exe";

		private volatile IWebDriver _driver = null;
		private volatile object _lock = new object();


		public WebDriverManager () {
			FileInfo geckoDriverFile = new FileInfo(GECKO_DRIVER_PATH);
			FirefoxDriverService ffds = FirefoxDriverService.CreateDefaultService(geckoDriverFile.DirectoryName);
			ffds.HideCommandPromptWindow = true;
			FirefoxOptions ffo = new FirefoxOptions();
			ffo.PageLoadStrategy = PageLoadStrategy.Eager;
			ffo.SetPreference("Headless", true);
			ffo.AddArgument("-headless");
			try {
				_driver = new FirefoxDriver(ffds, ffo);
				_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
				_driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
			}
			catch (Exception e) {
				Logger.Log(e);
				_driver = null;				
			}
		}

		public bool IsAlive {
			get {
				return _driver != null;
			}
		}

		public string NavigateAndFetch (string url, By by, Func<IWebElement, string> processResult, Func<IWebDriver, string, IWebElement> overrideNavigation = null) {
			string res = "";
			Monitor.Enter(_lock);
			try  {
				IWebElement result = null;
				if (overrideNavigation != null)
					result = overrideNavigation(_driver, url);
				else {
					_driver.Navigate().GoToUrl(url);
					Thread.Sleep(500);
					result = _driver.FindElement(by);
				}

				res = processResult(result);

			}
			finally {
				_driver?.Navigate().GoToUrl("about:blank");
				Monitor.Exit(_lock);
			}

			return res;
		}

		

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{

				}
				Monitor.Enter(_lock);
				try {
					_driver?.Quit();
					_driver = null;
				}
				finally {
					Monitor.Exit(_lock);
				}
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~WebDriverManager()
		{
		// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}


		#endregion

	}
}
