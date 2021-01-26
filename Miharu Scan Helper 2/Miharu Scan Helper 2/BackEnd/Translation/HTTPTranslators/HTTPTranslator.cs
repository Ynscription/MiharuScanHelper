using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Translation.HTTPTranslators {


	

	public abstract class HTTPTranslator : Translator {
		
		//Your Azure Cognitive Services key here
		protected const string _A = "";


		private static Regex _unicodeReplacer = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})");
		

		protected abstract string GetUri (string text);

		protected abstract string ProcessResponse (string response);

		protected virtual Encoding GetEncoding (string received) {
			if (received == null)
				return Encoding.UTF8;
			else
				return Encoding.GetEncoding(received);
		}

		protected static string DecodeEncodedUnicodeCharacters(string src)
		{
			return _unicodeReplacer.Replace(src, m => {
                return ((char) int.Parse( m.Groups["Value"].Value, NumberStyles.HexNumber )).ToString();
            } );
		}

		protected static string CleanNewLines (string src) {
			string res = src;
			res = res.Replace("\\n", Environment.NewLine);
			res = res.Replace("\\r", "");
			return res;
		}
		
		public override async Task<string> Translate(string text)
		{
			string res = "";
			if (text == "")
				return res;
						
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(GetUri(text));
			using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync()) {
				if (response.StatusCode == HttpStatusCode.OK) {
					using (Stream receiveStream = response.GetResponseStream())
					using (StreamReader readStream = 
							new StreamReader(receiveStream, GetEncoding(response.CharacterSet))) {

						res = await readStream.ReadToEndAsync();

						readStream.Close();
					
					}
				}
			}

			return ProcessResponse(res);
		}
		

	}
}
