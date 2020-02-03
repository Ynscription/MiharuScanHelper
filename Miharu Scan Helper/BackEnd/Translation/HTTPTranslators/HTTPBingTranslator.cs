using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.HTTPTranslators
{
	class HTTPBingTranslator : HTTPTranslator
	{
		//Your Azure Cognitive Services key here
		private const string _B = "";

		public override TranslationType Type{
			get { return TranslationType.Bing_API; }
		}

		protected override string GetUri(string text)
		{
			throw new NotImplementedException();
		}

		protected override string ProcessResponse(string response)
		{
			throw new NotImplementedException();
		}
	}
}
