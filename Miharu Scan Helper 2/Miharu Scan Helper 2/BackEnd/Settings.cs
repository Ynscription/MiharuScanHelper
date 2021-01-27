using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Miharu2.BackEnd
{
	public class Settings {

		private static readonly string USER_SETTINGS = "./UserSettings.cfg";


		private static Dictionary<string, object> _settings;

		private static void DefaultSettings() {
			_settings = new Dictionary<string, object>
			{
				["TesseractPath"] = "./Resources/Redist/Tesseract-OCR/tesseract.exe",
				["SaveVersion"] = "v4",
				["AutoTranslateEnabled"] = true,
				["DisabledTranslationSources"] = "",
				["WarnTextDeletion"] = true,
				["UseScreenDPI"] = false
			};

			SaveSettings();
		}
		
		private static void CheckSettingsExists()
		{
			if (!File.Exists(USER_SETTINGS))
				DefaultSettings();

			using (StreamReader reader = new StreamReader(USER_SETTINGS)) {
				_settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd());
			}
		}

		private static void SaveSettings()
		{
			using (StreamWriter writer = new StreamWriter(USER_SETTINGS, false, Encoding.UTF8)) {
				writer.Write(JsonConvert.SerializeObject(_settings, Formatting.Indented));
			}
		}




		public static T Get<T>(string key)
		{
			CheckSettingsExists();
			
			return (T)_settings[key];
		}

		

		public static void Set<T>(string key, T value)
		{
			CheckSettingsExists();

			_settings[key] = value;

			SaveSettings();
		}


		
	}
}
