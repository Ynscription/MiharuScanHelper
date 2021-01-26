using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace Miharu2.BackEnd {
	public static class Logger {

		
		private static object _crashLock = new object();
		private static object _logLock = new object();

		public static string CurrentCrashLog {
			get; private set;
		}
		private static string _currLog = null;


		public static void CrashLog(string log) {
			Monitor.Enter(_crashLock);
			try {
				if (!Directory.Exists("Logs"))
					Directory.CreateDirectory("Logs");
				if (CurrentCrashLog == null)
					CurrentCrashLog = "Logs/Crash " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

				using (StreamWriter writer = new StreamWriter(CurrentCrashLog, true)) {
					writer.Write(log + Environment.NewLine + Environment.NewLine);
					writer.Close();
				}
			}
			finally {
				Monitor.Exit(_crashLock);
			}
		}

		public static void CrashLog (Exception e) {
			CrashLog(GetExceptionString(e));
		}

		

		public static void Log (string log) {
			Monitor.Enter(_logLock);
			try {
				if (!Directory.Exists("Logs"))
					Directory.CreateDirectory("Logs");
				if (_currLog == null)
					_currLog = "Logs/Log " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log";

				using (StreamWriter writer = new StreamWriter(_currLog, true)) {
					writer.Write(log + Environment.NewLine + Environment.NewLine);
					writer.Close();
				}
			}
			finally {
				Monitor.Exit(_logLock);
			}
		}

		public static void Log (Exception e) {
			Log(GetExceptionString(e));
		}


		private static XElement ExceptionToXml (Exception e) {
			XElement root = new XElement(e.GetType().ToString());

			if (e.Message != null)
				root.Add(new XElement("Message", e.Message));

			if (e.StackTrace != null)
				root.Add(new XElement("StackTrace", e.StackTrace));

			if (e.Data.Count > 0) {
				XElement data = new XElement ("Data");
				foreach (var key in e.Data.Keys) {
					data.Add(new XElement(key.ToString(), e.Data[key]));
				}
				root.Add(data);
			}
			if (e.InnerException != null)
				root.Add(new XElement("InnerException", ExceptionToXml(e.InnerException)));

			return root;
		}
		
		private static string GetExceptionString (Exception e) {
			
			return "!!!!![Exception]!!!!!" + ExceptionToXml(e).ToString();

		}
	}
}
