
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Manga_Scan_Helper.BackEnd {
	static class SFXDictionary {
		private static Dictionary<string, string> dictionary = null;


		private const string DICTIONARY_FILE = @".\Resources\Data\onomatopoeia.json";

		public static void LoadDictionary (string path) {
			if (path == null || !File.Exists(path))
				path = DICTIONARY_FILE;
			
			using (StreamReader reader = new StreamReader(path)) {
				dictionary = JsonConvert.DeserializeObject< Dictionary<string, string>>(reader.ReadToEnd());
			}

		}

		public static string Translate (string input) {
			string result = null;
			while (input.Length > 1 && result == null) {
				dictionary?.TryGetValue(input, out result);
				input = input.Substring(0, input.Length -1);
			}

			return result;
		}


	}
}
