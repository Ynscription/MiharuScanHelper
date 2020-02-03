using Manga_Scan_Helper.BackEnd.Translation.HTTPTranslators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation
{

	//TODO write a script to convert old saves to new ones!!!!
	/*
	 * Google2 -> Google_API
	 * Bing -> Bing_API
	 * Yandex -> Yandex_API
	 * */
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

		private static readonly TranslationProvider _instance = new TranslationProvider();
		public static TranslationProvider Instance {
			get => _instance;
		}


		private Dictionary<TranslationType, Translator> _translators;

		private TranslationProvider () {
			
			_translators = new Dictionary<TranslationType, Translator>();
			
			_translators.Add(TranslationType.Google_API, new HTTPGoogleTranslator());
			_translators.Add(TranslationType.Bing_API, new HTTPBingTranslator());
			_translators.Add(TranslationType.Yandex_API, new HTTPYandexTranslator());
			_translators.Add(TranslationType.Jaded_Network, new HTTPTJNTranslator());

		}

		

		public static void Translate (TranslationType type, string text, TranslationConsumer consumer) {
			Task.Run(() => internalTranslate(type, text, consumer));

		}

		private static void internalTranslate (TranslationType type, string text, TranslationConsumer consumer) {
			try {
				string res = Instance._translators[type].Translate(text);
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
