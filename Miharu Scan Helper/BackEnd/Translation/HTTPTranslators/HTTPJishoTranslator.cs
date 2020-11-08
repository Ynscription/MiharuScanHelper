using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Miharu.BackEnd.Translation.HTTPTranslators
{
	class HTTPJishoTranslator: HTTPTranslator
	{
		private const string _URL = "https://jisho.org/search/";


		public override TranslationType Type {
			get { return TranslationType.Jaded_Network; }
		}

		

		protected override string GetUri(string text)
		{
			return _URL
					+ Uri.EscapeDataString(text);
		}

		protected override string ProcessResponse(string res)
		{
			StringBuilder result = new StringBuilder();
			XmlReaderSettings settings = new XmlReaderSettings();
			
			using (Stream s = new MemoryStream(Encoding.UTF8.GetBytes(res))) {
				using(XmlReader reader = XmlReader.Create(res, settings)) {
					while (reader.ReadToFollowing("li")) {
						if ((reader.GetAttribute("class") ?? "") != "clearfix japanese_word")
							continue;

						using (XmlReader subReader = reader.ReadSubtree()) {
							while(subReader.ReadToFollowing("span")) {
								if ((reader.GetAttribute("class") ?? "") != "japanese_word__text_wrapper")
									continue;
								if(reader.HasValue)
									result.AppendLine(reader.Value);
							}
						}
					}
				}
			}			
			

			return result.ToString();
		}

		protected override Encoding GetEncoding(string received)
		{
			return Encoding.UTF8;
		}
	}
}
