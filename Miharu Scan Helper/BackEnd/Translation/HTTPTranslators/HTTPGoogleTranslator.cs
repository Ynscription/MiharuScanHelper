using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.HTTPTranslators
{
	public class HTTPGoogleTranslator : HTTPTranslator
	{
		public override TranslationType Type {
			get { return TranslationType.Google_API; }
		}


		private const string _URL = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ja&tl=en&dt=t&q=";

		protected override string GetUri(string text)
		{
			return _URL + Uri.EscapeDataString(text);
		}

		protected override string ProcessResponse(string res)
		{
			
			int firstString = res.IndexOf("\"") + 1;
			if (res.IndexOf("null") <= firstString)
				throw new Exception("Google API translation failed");
			else {
				res = res.Substring(firstString);
				res = res.Substring(0, res.IndexOf("\",\""));
				if (res.Contains("\\u"))
					res = DecodeEncodedUnicodeCharacters(res);
			}

			return res;
		}

		
	}
}
