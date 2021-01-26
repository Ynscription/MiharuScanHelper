

using System;
using System.IO;

namespace Miharu2.BackEnd
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

		private const string _IMAGE_TAG = "<img class=\"chapter-img\" src=";

		public static string FileRip(string src, string path) {
			string destinationFolder = null;
			try {				
				if (Directory.Exists(path)) {
					if (!(path [path.Length - 1] == '\\' || path [path.Length - 1] == '/'))
						path += "\\";
					destinationFolder = path;
				}
				else
					throw new RipperException("Could not find or access the directory " + path);

				if (!File.Exists(src))
					throw new RipperException("Could not open file " + src);
				FileInfo srcInfo = new FileInfo(src);

				using (StreamReader reader = new StreamReader(src)) {
					int i = 1;
					while (!reader.EndOfStream) {
						string input = reader.ReadLine();

						if (input.Contains(_IMAGE_TAG)) {
							string img = input.Substring(input.IndexOf(_IMAGE_TAG) + _IMAGE_TAG.Length + 1);
							img = img.Substring(0, img.IndexOf(".jpg") + 4);
							img = img.Replace("%20", " ");
							if (!img.Contains(".html"))
								CopyImage(srcInfo.DirectoryName + "/" + img, i++, destinationFolder);

						}
					}
				}
			}
			catch (RipperException e) {
				Logger.Log(e);
				throw e;
			}
			catch(Exception e) {
				RipperException ex = new RipperException("Something went wrong while ripping.", e);
				throw ex;
			}
			return destinationFolder;
		}

		private static void CopyImage (string img, int index, string destinationFolder) {
			
			File.Copy(img, destinationFolder + index.ToString("D3") + ".jpg");			
		}


		
		/*public static string Rip (string url, string path) {
			Uri uriResult = null;
			if (!(url.Contains("lhscan.net") | url.Contains("loveheaven.net")))
				throw new RipperException("The URL belongs to a non supported site. Only lhscan.net pages supported (for now).");

			bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
						&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
			if (!result)
				throw new RipperException("URL could not be recognized as a valid URL scheme.");
			


			


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
				string find = "data-original=";
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
		}*/

		

		/*private static void DownloadImage (string url, int index, string destinationFolder) {
			using (WebClient client = new WebClient()) {
				client.DownloadFile(new Uri(url), destinationFolder + index.ToString("D3") + ".jpg");				
			}
			
		}*/


	}
}
