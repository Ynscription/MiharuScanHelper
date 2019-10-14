using System;
using System.IO;

namespace Manga_Scan_Helper.BackEnd {
	public class Logger {

		private static string _currCrashLog = null;

		public static void CrashLog(string log) {
			if (_currCrashLog == null)
				_currCrashLog = "Crash " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

			StreamWriter writer = new StreamWriter(_currCrashLog, true);
			writer.Write(log + Environment.NewLine + Environment.NewLine);
			writer.Close();
		}
	}
}
