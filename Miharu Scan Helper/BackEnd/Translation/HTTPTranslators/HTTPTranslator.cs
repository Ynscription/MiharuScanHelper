using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.HTTPTranslators {


	

	public abstract class HTTPTranslator : Translator {
		
		//Your Azure Cognitive Services key here
		protected const string _A = "";
		//Your Yandex Translate API key here
		protected const string _Y = "";


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
		
		public override string Translate(string text)
		{
			string res = "";
						
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(GetUri(text));
			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse()) {
				if (response.StatusCode == HttpStatusCode.OK) {
					using (Stream receiveStream = response.GetResponseStream())
					using (StreamReader readStream = 
							new StreamReader(receiveStream, GetEncoding(response.CharacterSet))) {

						res = readStream.ReadToEnd();

						readStream.Close();
					
					}
				}
			}

			return ProcessResponse(res);
		}
		

	}
}
