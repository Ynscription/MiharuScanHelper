﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd {


	public enum TranslationType {
		Google,
		Google2,
		Bing,
		Yandex
	}

	public static class HTTPTranslator {
		private const string _googleTranslateURL = "";
		private const string _google2TranslateURL = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=ja&tl=en&dt=t&q=";
		private const string _bingTranslateURL = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=ja&to=en";
		//TODO Add notice "Powered by Yandex https://tech.yandex.com/translate/doc/dg/concepts/design-requirements-docpage/"
		private const string _yandexTranslateURL = "https://translate.yandex.net/api/v1.5/tr.json/translate";
		//Your Azure Cognitive Services key here
		private const string _A = "";
		private const string _B = "";

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

		private static async Task internalGoogleTranslate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_googleTranslateURL + Uri.EscapeDataString(src));//Uri.EscapeUriString(src));
				request.Method = "GET";
				request.ContentType = "application/x-www-form-urlencoded";
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

					receiveStream.Close();

					consumer.TranslationFailed(new NotImplementedException(), TranslationType.Google);
				}
				else {
					consumer.TranslationFailed(new Exception("HTTP bad response (" + response.StatusCode.ToString() + "):" + Environment.NewLine
															 + response.StatusDescription), TranslationType.Google);
				}
				response.Close();
			}
			catch (Exception e) {

				receiveStream?.Close();
				response?.Close();
				consumer.TranslationFailed(e, TranslationType.Google);
			}
		}

		private static async Task internalGoogle2Translate (TranslationConsumer consumer, string src) {
			string res = "";
			HttpWebResponse response = null;
			Stream receiveStream = null;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_google2TranslateURL + Uri.EscapeDataString(src));//Uri.EscapeUriString(src));
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

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
					request.Headers.Add("Ocp-Apim-Subscription-Key", _A);

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

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_yandexTranslateURL 
					+ "?lang=" + "ja-en"
					+ "&key=" + _B
					+ "&text=" + Uri.EscapeDataString(src));
				response = (HttpWebResponse) await request.GetResponseAsync();
				if (response.StatusCode == HttpStatusCode.OK) {
					receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (response.CharacterSet == null)
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					res = await readStream.ReadToEndAsync();

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

				receiveStream?.Close();
				response?.Close();
				consumer.TranslationFailed(e, TranslationType.Yandex);
			}
		}
	}
}
