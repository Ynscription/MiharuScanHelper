using System;
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

	public class TranslationProvider
	{

		private static readonly TranslationProvider _instance = new TranslationProvider();
		private static TranslationProvider Instance {
			get => _instance;
		}


		private Dictionary<TranslationType, Translator> _translators;

		private TranslationProvider () {
			
			_translators = new Dictionary<TranslationType, Translator>();
			
			//_translators.Add();

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


	}
}
