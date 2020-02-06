using System;

namespace Miharu.BackEnd.Translation.HTTPTranslators
{
	class HTTPYandexTranslator : HTTPTranslator
	{

		private const string _URL = "https://translate.yandex.net/api/v1.5/tr.json/translate";
		


		public override TranslationType Type {
			get { return TranslationType.Yandex_API; }
		}

		protected override string GetUri(string text)
		{
			return _URL 
					+ "?lang=" + "ja-en"
					+ "&key=" + _Y
					+ "&text=" + Uri.EscapeDataString(text);
		}

		protected override string ProcessResponse(string res)
		{
			int firstBracket = res.IndexOf('[') + 2;
			res = res.Substring(firstBracket, (res.LastIndexOf(']') - 1) - firstBracket );
			if (res.Contains("\\u"))
				res = DecodeEncodedUnicodeCharacters(res);
			res = CleanNewLines(res);
			return res;
		}
	}
}
