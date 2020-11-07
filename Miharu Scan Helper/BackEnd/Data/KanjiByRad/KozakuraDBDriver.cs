using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Miharu.BackEnd.Data.KanjiByRad
{
	class KozakuraDBDriver : IDisposable
	{

		private const string KANJI_FROM_RADS_QUERY = @"
			SELECT
				k.id, k.lit, k.strokes
			FROM
				KanjiByRad AS kbr
			LEFT JOIN Kanji AS k ON
				kbr.kanji = k.id
			WHERE
				kbr.rad = @id;";


		private const string GET_RADS_QUERY = @"
			SELECT
				*
			FROM
				Rad
			ORDER BY
				strokes ASC, id ASC;";

		private SQLiteConnection _dbConnection;
		private bool _disposedValue;

		public KozakuraDBDriver()
		{
			_dbConnection = new SQLiteConnection("Data Source=" + "./Resources/Data/KozakuraDB" + "; Version=3; Read Only = True; FailIfMissing=True;");
			_dbConnection.Open();
		}

		public HashSet<JPChar> KanjiByRad(JPChar rad)
		{
			HashSet<JPChar> res = new HashSet<JPChar>();

			SQLiteCommand command = new SQLiteCommand(_dbConnection);
			command.CommandText = KANJI_FROM_RADS_QUERY;
			command.Parameters.Add(new SQLiteParameter("@id", rad.Id));
			SQLiteDataReader result = command.ExecuteReader(System.Data.CommandBehavior.KeyInfo);
			while (result.Read())
			{
				if (result.IsDBNull(0))
					continue;				
				Int32 id = result.GetInt32(0);
				string lit = result.GetString(1);
				int strokes = result.GetInt32(2);
				JPChar k = new JPChar((int)id, lit, strokes, false);
				res.Add(k);
			}
			result.Close();

			return res;
		}

		public List<JPChar> GetRadList()
		{
			List<JPChar> res = new List<JPChar>();

			SQLiteCommand command = new SQLiteCommand(_dbConnection);
			command.CommandText = GET_RADS_QUERY;
			SQLiteDataReader result = command.ExecuteReader(System.Data.CommandBehavior.KeyInfo);
			while (result.Read())
			{
				int id = result.GetInt32(0);
				string lit = result.GetString(1);
				int strokes = result.GetInt32(2);
				JPChar r = new JPChar(id, lit, strokes, true);
				res.Add(r);
			}
			result.Close();

			return res;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					_dbConnection.Close();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_dbConnection = null;
				_disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~KozakuraDBDriver()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
