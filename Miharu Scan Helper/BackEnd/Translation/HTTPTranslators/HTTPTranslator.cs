
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation {


	

	public abstract class HTTPTranslator : Translator {
		
		private const string _bingTranslateURL = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=ja&to=en";
		
		
		
		
		
		
		

		private static Regex _unicodeReplacer = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})");
		

		protected abstract string GetUri (string text);

		protected abstract string ProcessResponse (string response);

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
							new StreamReader(receiveStream, 
								response.CharacterSet == null ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet))) {

						res = readStream.ReadToEnd();

						readStream.Close();
					
					}
				}
			}

			return ProcessResponse(res);
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
							if (result.Contains("\\u"))
								result = DecodeEncodedUnicodeCharacters(result);
							consumer.TranslationCallback(result, TranslationType.Bing_API);
						}
						else {
							consumer.TranslationFailed(new Exception("Bad response format"), TranslationType.Bing_API);
						}
					}
					else {
						consumer.TranslationFailed (new Exception("HTTP bad response (" + response.StatusCode.ToString() + ")"), TranslationType.Bing_API);
					}
				}
			}
			catch (Exception e) {
				consumer.TranslationFailed(e, TranslationType.Bing_API);
			}

		}


		

	}
}
