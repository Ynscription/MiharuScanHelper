using System;
using System.IO;

namespace Manga_Scan_Helper.BackEnd {
	public static class Logger {

		private static string _currCrashLog = null;
		private static string _currLog = null;

		public static void CrashLog(string log) {
			if (_currCrashLog == null)
				_currCrashLog = "Crash " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

			StreamWriter writer = new StreamWriter(_currCrashLog, true);
			writer.Write(log + Environment.NewLine + Environment.NewLine);
			writer.Close();
		}

		public static void Log (string log) {
			if (_currLog == null)
				_currLog = "Log " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

			StreamWriter writer = new StreamWriter(_currLog, true);
			writer.Write(log + Environment.NewLine + Environment.NewLine);
			writer.Close();
		}
	}
}
