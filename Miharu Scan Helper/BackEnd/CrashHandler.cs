using System;
using System.IO;
using System.Text;

namespace Manga_Scan_Helper.BackEnd
{
    static class CrashHandler
    {
		private const string EMERGENCY_FILE = @"\.crash";
		private static bool _emergencyHandled = false;
		public static bool LastSessionCrashed {
			get {
				return File.Exists(EMERGENCY_FILE);
			}
		}

		public static void HandleCrash (Chapter chapter, string currSavedFile, Exception e) {
			if (!_emergencyHandled) {
				try {
					Logger.CrashLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
					
					if (chapter != null) {
						string dest = "";
						if (currSavedFile != null)
							dest = currSavedFile.Insert(currSavedFile.LastIndexOf("."), "_RECOVERY" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
						else
							dest = @"\recovery\" + "RECOVERY" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".scan";
						chapter.Save(dest);

						StreamWriter writer = new StreamWriter(EMERGENCY_FILE, false, Encoding.UTF8);
						writer.WriteLine(dest);
						writer.Close();
					}
					
					_emergencyHandled = true;
				}
				catch { }
			}
		}

		public static string RecoverLastSessionFile () {
			string res = null;

			if (LastSessionCrashed) {
				StreamReader reader = new StreamReader(EMERGENCY_FILE);
				res = reader.ReadLine();
				reader.Close();
				File.Delete(EMERGENCY_FILE);
			}
			
			return res;
		}
    }
}
