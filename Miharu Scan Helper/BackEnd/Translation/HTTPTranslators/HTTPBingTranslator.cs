using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Manga_Scan_Helper.BackEnd.Translation.HTTPTranslators
{
	class HTTPBingTranslator : HTTPTranslator
	{
		private const string _URL = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=ja&to=en";
		

		public override TranslationType Type{
			get { return TranslationType.Bing_API; }
		}

		protected override string GetUri(string text)
		{
			return _URL;
		}

		protected override string ProcessResponse(string response)
		{
			string result = response;
			string find = "\"text\":";
			if (result.Contains(find)){
				result = result.Substring(result.IndexOf(find) + find.Length);
				result = result.Substring(result.IndexOf("\"") + 1);
				result = result.Substring(0, result.IndexOf("\",\""));
				if (result.Contains("\\u"))
					result = DecodeEncodedUnicodeCharacters(result);
				result = CleanNewLines(result);
			}
			else {
				throw new Exception("Bad response format");
			}
			return result;
		}

		public override async Task<string> Translate(string text)
		{
			string result = "";
			object [] body = new object [] { new { Text = text} };
			string requestBody = JsonConvert.SerializeObject(body);

			
			using (HttpClient client = new HttpClient())
			using (HttpRequestMessage request = new HttpRequestMessage()) {
				request.Method = HttpMethod.Post;
				request.RequestUri = new Uri(GetUri(text));
				request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
				request.Headers.Add("Ocp-Apim-Subscription-Key", _A);

				HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

				if (response.StatusCode == HttpStatusCode.OK) {
					result = await response.Content.ReadAsStringAsync();
					result = ProcessResponse(result);
				}
				else {
					throw new Exception("HTTP bad response (" + response.StatusCode.ToString() + ")");
				}
			}
			
			return result;
		}
	}
}
