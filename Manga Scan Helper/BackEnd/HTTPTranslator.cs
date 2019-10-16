using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd {

	public enum TranslationType {
		Google
	}

	public static class HTTPTranslator {
		private static readonly string _googleTranslateURL = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ja&tl=en&dt=t&q=";
		/*var url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" 
            + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + encodeURI(sourceText);*/
		public static void GoogleTranslate (TranslationConsumer consumer, string source) {
			/*
			 * TODO I can feel the gods of concurrency looking down on me with disgust, 
			 * but not putting any thread safety yet, cuz I don't feel like it
			 * It'll probably be fine
			 * */
			Task.Run(() => internalGoogleTranslate(consumer, source)); 
		}

		public static void internalGoogleTranslate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;
			
			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_googleTranslateURL + Uri.EscapeUriString(src));
				response = (HttpWebResponse) request.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
					
					res = readStream.ReadToEnd();
					
					receiveStream.Close();

					int firstString = res.IndexOf("\"") + 1;
					if (res.IndexOf("null") <= firstString) {
						consumer.TranslationFailed(new Exception("Google translation failed"));
					}
					else {
						res = res.Substring(firstString);
						res = res.Substring(0, res.IndexOf("\""));
						consumer.TranslationCallback(res, TranslationType.Google);
					}
				}
				else {
					consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine 
															 + response.StatusDescription));
				}
				response.Close();
			}
			catch (Exception e) {
				
				receiveStream?.Close();
				response?.Close();
				consumer.TranslationFailed(e);
			}
		}
	}
}
