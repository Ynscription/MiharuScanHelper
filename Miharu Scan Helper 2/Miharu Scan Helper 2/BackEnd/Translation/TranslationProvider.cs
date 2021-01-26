using Miharu2.BackEnd.Data;
using Miharu2.BackEnd.Translation.HTTPTranslators;
using Miharu2.BackEnd.Translation.Threading;
using Miharu2.BackEnd.Translation.WebCrawlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Translation
{

	[Flags]
	public enum TranslationType {
		Web = 0x10,
		SFX = 0x20,
		Dict = 0x40,
		//subtypes
		Google_Web = 0x11,
		Google_API = 0x12,
		Bing_Web = 0x13,
		Bing_API = 0x14,
		Yandex_API = 0x15,
		Yandex_Web = 0x16,
		DeepL_Web = 0x17,
		Jaded_Network = 0x21,
		Jisho = 0x41
	}

	public class TranslationProvider : IEnumerable<TranslationType>
	{		

		private Dictionary<TranslationType, Translator> _translators;

		public TranslationProvider (WebDriverManager wdManager) {

			_translators = new Dictionary<TranslationType, Translator>();

			_translators.Add(TranslationType.Google_API, new HTTPGoogleTranslator());
			if (wdManager.IsAlive)
				_translators.Add(TranslationType.Google_Web, new WCGoogleTranslator(wdManager));

			if (wdManager.IsAlive)
				_translators.Add(TranslationType.DeepL_Web, new WCDeepLTranslator(wdManager));

			_translators.Add(TranslationType.Bing_API, new HTTPBingTranslator());

			if (wdManager.IsAlive)
				_translators.Add(TranslationType.Yandex_Web, new WCYandexTranslator(wdManager));

			_translators.Add(TranslationType.Jaded_Network, new HTTPTJNTranslator());

			_translators.Add(TranslationType.Jisho, new HTTPJishoTranslator());

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
