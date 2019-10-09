

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Manga_Scan_Helper.BackEnd
{
    public class Ripper
    {
		private string _url;
		public string _destinationFolder = null;
		//private static Regex PageRegex = new Regex(@"<img class='chapter-img' src='(.*).jpg", RegexOptions.Compiled);


		public Ripper (string url) {
			Uri uriResult = null;
			if (!url.Contains("lhscan.net"))
				throw new Exception("The URL belongs to a non supported site. Only lhscan.net pages supported (for now).");
			bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
						&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
			if (!result)
				throw new Exception("URL could not be recognized as a valid URL scheme.");
			_url = url;

		}


		public void SetDestinationFolder (string path) {
			if (Directory.Exists(path)) {
				if (!(path[path.Length-1] == '\\' || path [path.Length - 1] == '/'))
					path += "\\";
				_destinationFolder = path;
			}
			else
				throw new Exception ("Could not find or acces the directory " + path);
		}


		public string Rip () {
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(_url);
			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse) request.GetResponse();
			}catch (Exception e){
				Exception ex = new Exception("There was a HTTP response error.", e);
				throw ex;
			}
			if (response.StatusCode == HttpStatusCode.OK) {
				Stream receiveStream = response.GetResponseStream();
				StreamReader readStream = null;

				if (response.CharacterSet == null) {
					readStream = new StreamReader(receiveStream);
				}
				else {
					readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
				}
				
				
				int i = 1;
				string leading = "'chapter-img' src='";
				while (!readStream.EndOfStream) {
					string input = readStream.ReadLine();

					if (input.Contains("chapter-img")) {
						string imgurl = input.Substring(input.IndexOf(leading) + leading.Length);
						imgurl = imgurl.Substring(0, imgurl.IndexOf(".jpg") + 4);
						DownloadImage(imgurl, i++);						
					}
				}



				readStream.Close();
				response.Close();
				

			}
			else
				throw new Exception ("HTTP response returned with status code " + response.StatusCode + ": " + response.StatusDescription);


			return _destinationFolder;
		}

		private void DownloadImage (string url, int index) {
			WebClient client = new WebClient();
			client.DownloadFile(new Uri(url), _destinationFolder + index.ToString("D3") + ".jpg");				
			
		}


	}
}
