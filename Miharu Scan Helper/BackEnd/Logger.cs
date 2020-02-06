using System;
using System.IO;

namespace Miharu.BackEnd {
	public static class Logger {

		
		public static string CurrentCrashLog {
			get; private set;
		}
		private static string _currLog = null;
		private static string _currSessionLog = null;

		public static void CrashLog(string log) {
			if (CurrentCrashLog == null)
				CurrentCrashLog = "Crash " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

			StreamWriter writer = new StreamWriter(CurrentCrashLog, true);
			writer.Write(log + Environment.NewLine + Environment.NewLine);
			writer.Close();
		}

		public static void SessionLog (string log) {
			StreamWriter writer;
			if (_currSessionLog == null) {
				_currSessionLog = "SessionLog.log";
				writer = new StreamWriter(_currSessionLog, false);
			}
			else
			 writer = new StreamWriter(_currSessionLog, true);
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
