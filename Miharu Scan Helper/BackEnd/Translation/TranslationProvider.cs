using Miharu.BackEnd.Translation.HTTPTranslators;
using Miharu.BackEnd.Translation.WebCrawlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation
{


	public enum TranslationType {
		Google_Web,
		Google_API,
		Bing_Web,
		Bing_API,
		Yandex_API,
		Yandex_Web,
		Jaded_Network,
	}

	public class TranslationProvider : IEnumerable<TranslationType>
	{
		public static TranslationProvider Instance { get; } = new TranslationProvider();

		

		private Dictionary<TranslationType, Translator> _translators;

		private TranslationProvider () {

			_translators = new Dictionary<TranslationType, Translator>();

			_translators.Add(TranslationType.Google_API, new HTTPGoogleTranslator());
			if (WebDriverManager.IsAlive)
				_translators.Add(TranslationType.Google_Web, new WCGoogleTranslator());
			_translators.Add(TranslationType.Bing_API, new HTTPBingTranslator());
			_translators.Add(TranslationType.Yandex_API, new HTTPYandexTranslator());
			if (WebDriverManager.IsAlive)
				_translators.Add(TranslationType.Yandex_Web, new WCYandexTranslator());
			_translators.Add(TranslationType.Jaded_Network, new HTTPTJNTranslator());

		}



		public static void Translate (TranslationType type, string text, TranslationConsumer consumer) {
			Task.Run(() => internalTranslate(type, text, consumer));

		}

		private static async void internalTranslate (TranslationType type, string text, TranslationConsumer consumer) {
			try {
				string res = await Instance._translators[type].Translate(text);
				consumer.TranslationCallback(res, type);
			}
			catch (Exception e) {
				consumer.TranslationFailed(e, type);
			}
		}

		public IEnumerator<TranslationType> GetEnumerator()
		{
			return Instance._translators.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Instance._translators.Keys.GetEnumerator();
		}





	}
}
