
using Manga_Scan_Helper.BackEnd.Translation;
using Manga_Scan_Helper.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace Manga_Scan_Helper.BackEnd
{
	[JsonObject(MemberSerialization.OptOut)]
	public class Text
    {
		
		public event TxtEventHandler TextChanged;

		[JsonIgnore]
		private readonly static string TEMP_IMG = @"./tmp.png";
		[JsonIgnore]
		private readonly static string TEMP_TXT = @"./tmp.txt";

		[JsonIgnore]
		private bool _parseInvalidated = true;


		public void Invalidate () {
			_parseInvalidated = true;
		}

		private bool _vertical;
		public bool Vertical {
			get => _vertical;
			set {
				_vertical = value;
				TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Vertical, null, null));
			}
		}
		[JsonIgnoreAttribute]
		public Bitmap Source { get; private set; }
		public Rect Rectangle { get; private set; }

		private string _parsedText = null;
		public string ParsedText {
			get {
				if (_parsedText == null || _parseInvalidated) {
					_parsedText = ParseText();
					_parseInvalidated = false;
				}
				return _parsedText;
			}
			set {
				_parsedText = value;
				TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Parse, null, _parsedText));
			}
		}

		[JsonProperty]
		private Dictionary<TranslationType, string> _translations;
		public string GetTranslation (TranslationType type) {
			string res = null;
			if (!_translations.TryGetValue(type, out res))
				return null;
			return res;
		}
		public void SetTranslation (TranslationType type, string value) {
			_translations[type] = value;
			TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.TranslationSource, type, value));
		}

	
		

		private string _translatedText;
		public string TranslatedText {
			get => _translatedText;
			set {
				_translatedText = value;
				TextChanged?.Invoke(this, new TxtChangedEventArgs(TextChangeType.Translation, null, value));
			}
		}

		public Text (Bitmap src, Rect rect) {
			Source = src;
			Rectangle = rect;
			TranslatedText = "";
			Vertical = src.Height >= src.Width;
			_translations = new Dictionary<TranslationType, string>();
		}

		
		
		//There are legacy parameters, so loading old saves still works
		[JsonConstructor]
		public Text (Rect rectangle, bool vertical, bool parseInvalidated,
					string parsedText, Dictionary<TranslationType, string> translations, 
					string googleTranslatedText, string bingTranslatedText,
					string translatedText) {
			Rectangle = rectangle;
			Vertical = vertical;
			_parseInvalidated = parseInvalidated;
			ParsedText = parsedText;
			if (translations != null)
				_translations = translations;
			else {
				_translations = new Dictionary<TranslationType, string>();
				_translations[TranslationType.Google_API] = googleTranslatedText;
				_translations[TranslationType.Bing_API] = bingTranslatedText;
			}
			TranslatedText = translatedText;			
		}

		

		private string ParseText () {
			if (Source == null)
				return "";

			Source.Save(TEMP_IMG, ImageFormat.Png);
			
			Process pProcess = new System.Diagnostics.Process();
			pProcess.StartInfo.FileName = (string)Settings.Default["TesseractPath"];
			string vert = Vertical ? "jpn_vert" : "jpn";
			pProcess.StartInfo.Arguments = TEMP_IMG + " tmp -l " + vert; //argument
			pProcess.StartInfo.UseShellExecute = false;
			//pProcess.StartInfo.RedirectStandardOutput = true;
			pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
			pProcess.Start();
			pProcess.WaitForExit();

			string output = "";
			try {
				StreamReader reader = new StreamReader (TEMP_TXT);
				output = reader.ReadToEnd();
				output = output.TrimEnd();
				reader.Close();
			}catch (IOException) {}

			return output;
		}

		
		public void Load (Bitmap src) {
			Source = src;
		}




	}
}
