using Miharu2.BackEnd.Translation.WebCrawlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Miharu2.BackEnd.Translation.Threading
{
	public class TranslatorThread
	{

		public volatile ConcurrentQueue<TranslationRequest> _workQueue;
		private volatile bool _end = false;
		private WebDriverManager _webDriverManager;
		private TranslationProvider _translationProvider;
		private AutoResetEvent _workHandle;
		private volatile bool _initialized = false;
		private ManualResetEvent _initializeHandle;

		public IEnumerable<TranslationType> AvailableTranslations {
			get {
				if (!_initialized)
					_initializeHandle.WaitOne();
				return _translationProvider;
			}
		}

		public bool IsWebDriverAvailable {
			get {
				if (!_initialized)
					_initializeHandle.WaitOne();
				return _webDriverManager != null ? _webDriverManager.IsAlive : false;
			}
		}



		private TranslatorThread () {
			_initializeHandle = new ManualResetEvent(false);
			_workHandle = new AutoResetEvent(false);
		}

		public static TranslatorThread StartThread () {
			TranslatorThread res = new TranslatorThread();
			
			Thread thread = new Thread (new ThreadStart(res.Work));
			thread.Start();

			return res;
		}




		private void Init () {
			_webDriverManager = new WebDriverManager();
			_translationProvider = new TranslationProvider(_webDriverManager);
			_workQueue = new ConcurrentQueue<TranslationRequest>();
			_initialized = true;
			_initializeHandle.Set();
		}

		private void End() {
			_webDriverManager?.Dispose();
		}

		

		public void Work () {
			try {
				Init();
			
				while(!_end) {
					_workHandle.WaitOne();
					TranslationRequest req = null;
					while (_workQueue.TryDequeue(out req))
						_translationProvider.Translate(req);
				}
			}
			finally {
				End();
			}
		}

		public void Translate (TranslationRequest request) {
			if (!_initialized)
				_initializeHandle.WaitOne();
			_workQueue.Enqueue(request);
			_workHandle.Set();
		}

		public void TranslateAll(TranslationRequest request)
		{
			if (!_initialized)
				_initializeHandle.WaitOne();
			

			string disabledTypes = Settings.Get<string>("DisabledTranslationSources");			
			foreach (TranslationType t in _translationProvider) {
				if (t.HasFlag(TranslationType.Web)){
					if (!disabledTypes.Contains(t.ToString()))
						_workQueue.Enqueue(new TranslationRequest(request.Destination, t, request.Text, request.Consumer));
					else
						request.Consumer.TranslationFailed(new Exception("Translation source is disabled."), t);
				}
			}
			_workHandle.Set();
		}


		public void FinalizeThread () {
			if (!_initialized)
				_initializeHandle.WaitOne();
			_end = true;
			_workHandle.Set();
		}

		

	}
}
