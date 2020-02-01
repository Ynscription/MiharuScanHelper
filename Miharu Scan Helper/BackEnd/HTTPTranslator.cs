
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Manga_Scan_Helper.BackEnd {


	public enum TranslationType {
		//GoogleAPI,
		Google2,
		Bing,
		Yandex,
		JadedNetwork
	}

	public static class HTTPTranslator {
		private const string _googleTranslateURL = "";
		
		private const string _google2TranslateURL = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ja&tl=en&dt=t&q=";
		
		private const string _bingTranslateURL = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=ja&to=en";
		
		private const string _yandexTranslateURL = "https://translate.yandex.net/api/v1.5/tr.json/translate";
		
		private const string _jadedNetworkURL1 = "http://thejadednetwork.com/sfx/search/?keyword=";
		private const string _jadedNetworkURL2 = "&submitSearch=Search+SFX&x=";
		
		//Your Google Cloud Translation API key here
		private const string _A = "";
		//Your Azure Cognitive Services key here
		private const string _B = "";
		//Your Yandex Translate API key here
		private const string _C = "";

		public static void GoogleTranslate (TranslationConsumer consumer, string source) {
			Task.Run(() => internalGoogleTranslate(consumer, source));

		}

		public static void Google2Translate (TranslationConsumer consumer, string source) {
			Task.Run(() => internalGoogle2Translate(consumer, source));

		}

		public static void BingTranslate (TranslationConsumer consumer, string source) {
			Task.Run(() => internalBingTranslate(consumer, source));
		}

		public static void YandexTranslate (TranslationConsumer consumer, string source) {
			Task.Run(() => internalYandexTranslate(consumer, source));
		}

		public static void JadedNetworkTranslate (TranslationConsumer consumer, string source) {
			Task.Run (() => internalJadedNetworkTranslate(consumer, source));
		}

		private static async Task internalGoogleTranslate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;
			StreamReader readStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_googleTranslateURL);
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

					readStream.Close();
					receiveStream.Close();
					
					//consumer.TranslationFailed(new Exception("Google 2 translation failed"), TranslationType.Google2);
					
				}
				else {
					//consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine + response.StatusDescription), TranslationType.Google2);
				}
				response.Close();
			}
			catch (Exception e) {
				readStream?.Close();
				receiveStream?.Close();
				response?.Close();
				//consumer.TranslationFailed(e, TranslationType.GoogleAPI);
			}
		}

		private static async Task internalGoogle2Translate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;
			StreamReader readStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_google2TranslateURL + Uri.EscapeDataString(src));//Uri.EscapeUriString(src));
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					
					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

					readStream.Close();
					receiveStream.Close();

					int firstString = res.IndexOf("\"") + 1;
					if (res.IndexOf("null") <= firstString) {
						consumer.TranslationFailed(new Exception("Google 2 translation failed"), TranslationType.Google2);
					}
					else {
						res = res.Substring(firstString);
						res = res.Substring(0, res.IndexOf("\",\""));
						consumer.TranslationCallback(res, TranslationType.Google2);
					}
				}
				else {
					consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine
															 + response.StatusDescription), TranslationType.Google2);
				}
				response.Close();
			}
			catch (Exception e) {
				readStream?.Close();
				receiveStream?.Close();
				response?.Close();
				consumer.TranslationFailed(e, TranslationType.Google2);
			}
		}

		private static async Task internalBingTranslate (TranslationConsumer consumer, string src) {
			object [] body = new object [] { new { Text = src} };
			string requestBody = JsonConvert.SerializeObject(body);

			try {
				using (HttpClient client = new HttpClient())
				using (HttpRequestMessage request = new HttpRequestMessage()) {
					request.Method = HttpMethod.Post;
					request.RequestUri = new Uri(_bingTranslateURL);
					request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
					request.Headers.Add("Ocp-Apim-Subscription-Key", _B);

					HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

					if (response.StatusCode == HttpStatusCode.OK) {
						string result = await response.Content.ReadAsStringAsync();
						string find = "\"text\":";
						if (result.Contains(find)){
							result = result.Substring(result.IndexOf(find) + find.Length);
							result = result.Substring(result.IndexOf("\"") + 1);
							result = result.Substring(0, result.IndexOf("\",\""));
							consumer.TranslationCallback(result, TranslationType.Bing);
						}
						else {
							consumer.TranslationFailed(new Exception("Bad response format"), TranslationType.Bing);
						}
					}
					else {
						consumer.TranslationFailed (new Exception("HTTP bad response (" + response.StatusCode.ToString() + ")"), TranslationType.Bing);
					}
				}
			}
			catch (Exception e) {
				consumer.TranslationFailed(e, TranslationType.Bing);
			}

		}


		private static async Task internalYandexTranslate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;
			StreamReader readStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_yandexTranslateURL 
					+ "?lang=" + "ja-en"
					+ "&key=" + _C
					+ "&text=" + Uri.EscapeDataString(src));
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

					readStream.Close();
					receiveStream.Close();

					int firstBracket = res.IndexOf('[') + 2;
					res = res.Substring(firstBracket, (res.LastIndexOf(']') - 1) - firstBracket );

					consumer.TranslationCallback(res, TranslationType.Yandex);
				}
				else {
					consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine
															 + response.StatusDescription), TranslationType.Yandex);
				}
				response.Close();
			}
			catch (Exception e) {

				readStream?.Close();
				receiveStream?.Close();
				response?.Close();
				consumer.TranslationFailed(e, TranslationType.Yandex);
			}
		}



		
		private static async Task internalJadedNetworkTranslate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;
			StreamReader readStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_jadedNetworkURL1
					+ Uri.EscapeDataString(src)
					+ _jadedNetworkURL2);
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					readStream = null;

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

					readStream.Close();
					receiveStream.Close();

					List<string> chunks = new List<string>();
					
					XElement xres = XElement.Parse(res, );


					/*int startIndex = 0;
					while ((startIndex = res.IndexOf("<td class=\"romaji\">")) < res.Length) {
						res = res.Substring(startIndex);
						int endIndex = res.IndexOf("<a href=\"http://thejadednetwork.com/sfx/browse/");
						chunks.Add(res.Substring(0, endIndex));
						res = res.Substring(endIndex);
					}*/

					
					consumer.TranslationCallback(res, TranslationType.JadedNetwork);
				}
				else {
					consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine
															 + response.StatusDescription), TranslationType.JadedNetwork);
				}
				response.Close();
			}
			catch (Exception e) {

				readStream?.Close();
				response?.Close();
				receiveStream?.Close();
				consumer.TranslationFailed(e, TranslationType.JadedNetwork);
			}
		}
	}
}
