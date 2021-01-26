using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Translation.HTTPTranslators
{
	class HTTPTJNTranslator : HTTPTranslator
	{
		private const string _URL1 = "http://thejadednetwork.com/sfx/search/?keyword=";
		private const string _URL2 = "&submitSearch=Search+SFX&x=";


		public override TranslationType Type {
			get { return TranslationType.Jaded_Network; }
		}

		private static Regex _jadedNetworkRegex = new Regex("<td class=\"jap\" (lang=\"ja\"|xml:lang=\"ja\") (lang=\"ja\"|xml:lang=\"ja\")>((.|\\n)*)<a href=\"http://thejadednetwork[.]com/sfx/browse/");

		protected override string GetUri(string text)
		{
			return _URL1
					+ Uri.EscapeDataString(text)
					+ _URL2;
		}

		protected override string ProcessResponse(string res)
		{
			MatchCollection matches = _jadedNetworkRegex.Matches(res);

			if (matches.Count > 0)
				res = matches[0].Groups[3].Value;
			else
				throw new Exception("No results found or there was an error parsing the source.");
			return res;
		}

		protected override Encoding GetEncoding(string received)
		{
			return Encoding.UTF8;
		}
	}
}
