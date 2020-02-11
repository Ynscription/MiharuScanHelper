using Miharu.BackEnd.Data;
using Miharu.BackEnd.Translation.HTTPTranslators;
using Miharu.BackEnd.Translation.Threading;
using Miharu.BackEnd.Translation.WebCrawlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation
{

	[Flags]
	public enum TranslationType {
		Text = 0x1000,
		SFX = 0x2000,
		//subtypes
		Google_Web = 0x1001,
		Google_API = 0x1002,
		Bing_Web = 0x1004,
		Bing_API = 0x1008,
		Yandex_API = 0x1010,
		Yandex_Web = 0x1020,
		Jaded_Network = 0x2001,
	}

	public class TranslationProvider : IEnumerable<TranslationType>
	{		

		private Dictionary<TranslationType, Translator> _translators;

		public TranslationProvider (WebDriverManager wdManager) {

			_translators = new Dictionary<TranslationType, Translator>();

			_translators.Add(TranslationType.Google_API, new HTTPGoogleTranslator());
			if (wdManager.IsAlive)
				_translators.Add(TranslationType.Google_Web, new WCGoogleTranslator(wdManager));
			_translators.Add(TranslationType.Bing_API, new HTTPBingTranslator());
			_translators.Add(TranslationType.Yandex_API, new HTTPYandexTranslator());
			if (wdManager.IsAlive)
				_translators.Add(TranslationType.Yandex_Web, new WCYandexTranslator(wdManager));
			_translators.Add(TranslationType.Jaded_Network, new HTTPTJNTranslator());

		}

		public void Translate(TranslationRequest req)
		{
			Task.Run(() => internalTranslate(req.Destination, req.Type.Value, req.Text, req.Consumer));
		}

		private async void internalTranslate (Text destination, TranslationType type, string text, TranslationConsumer consumer) {
			try {
				string res = await _translators[type].Translate(text);
				consumer.TranslationCallback(destination, res, type);
			}
			catch (Exception e) {
				consumer.TranslationFailed(e, type);
			}
		}

		public IEnumerator<TranslationType> GetEnumerator()
		{
			return _translators.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _translators.Keys.GetEnumerator();
		}





	}
}
