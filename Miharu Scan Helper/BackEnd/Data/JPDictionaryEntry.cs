using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Data
{
	public class JPDictionaryEntry
	{

		public JPWord Word {
			get; set;
		}

		public string Forms {
			get; set;
		}

		public JPDictionaryEntry FormGuess {
			get; set;
		}

		public List<Tuple<JPWord, string>> Meanings {
			get; set;
		}


		public JPDictionaryEntry () {
			Word = new JPWord();
			Meanings = new List<Tuple<JPWord, string>>();
		}

		public JPDictionaryEntry (JPWord word) {
			Word = word;
			Meanings = new List<Tuple<JPWord, string>>();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			return Word + " " + Meanings.Count + " meanings." ;
		}
	}
}
