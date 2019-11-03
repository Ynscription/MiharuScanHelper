

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Manga_Scan_Helper.BackEnd
{
    public static class Ripper
    {

		public class RipperException : Exception{

			public RipperException () : base() {

			}

			public RipperException (string message) : base(message) {

			}

			public RipperException (string message, Exception innerException) : base (message, innerException) {

			}
		}
		
		public static string Rip (string url, string path) {
			Uri uriResult = null;
			if (!url.Contains("lhscan.net"))
				throw new RipperException("The URL belongs to a non supported site. Only lhscan.net pages supported (for now).");

			bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
						&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
			if (!result)
				throw new RipperException("URL could not be recognized as a valid URL scheme.");
			


			string destinationFolder = null;
			if (Directory.Exists(path)) {
				if (!(path [path.Length - 1] == '\\' || path [path.Length - 1] == '/'))
					path += "\\";
				destinationFolder = path;
			}
			else
				throw new RipperException("Could not find or acces the directory " + path);


			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse) request.GetResponse();
			}catch (Exception e){
				throw new RipperException("There was a HTTP response error.", e);
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
				string find = "data-src=";
				while (!readStream.EndOfStream) {
					string input = readStream.ReadLine();

					if (input.Contains(find)) {
						string imgurl = input.Substring(input.IndexOf(find) + find.Length + 1);
						imgurl = imgurl.Substring(0, imgurl.IndexOf(".jpg") + 4);
						DownloadImage(imgurl, i++, destinationFolder);						
					}
				}



				readStream.Close();
				response.Close();
				

			}
			else
				throw new RipperException("HTTP response returned with status code " + response.StatusCode + ": " + response.StatusDescription);


			return destinationFolder;
		}

		private static void DownloadImage (string url, int index, string destinationFolder) {
			using (WebClient client = new WebClient()) {
				client.DownloadFile(new Uri(url), destinationFolder + index.ToString("D3") + ".jpg");				
			}
			
		}


	}
}
