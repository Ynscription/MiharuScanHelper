using System;

namespace Miharu2.BackEnd
{
	public class Settings {

		
		private static void CheckDBExists()
		{
			throw new NotImplementedException();
		}




		public static T Get<T>(string key)
		{
			CheckDBExists();
			using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + "./userSettings.db" + "; Version=3; Read Only = True; FailIfMissing=True;")) {

			}
		}

		

		public static void Set<T>(string key, T value)
		{
			CheckDBExists();
			using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + "./userSettings.db" + "; Version=3; Read Only = False; FailIfMissing=True;")) {

			}
		}


		
	}
}
