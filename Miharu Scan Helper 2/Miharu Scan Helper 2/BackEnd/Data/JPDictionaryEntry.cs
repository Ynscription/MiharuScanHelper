using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Data
{
	public class JPDictionaryEntry
	{


		public JPWord Word {
			get; set;
		}

		public List<string> Forms {
			get; set;
		}

		public JPDictionaryEntry FormGuess {
			get; set;
		}

		public List<Tuple<JPWord, string>> ExactMeanings {
			get; set;
		}

		public List<Tuple<JPWord, string>> Concepts {
			get; set;
		}


		public JPDictionaryEntry () {
			Word = new JPWord();
			ExactMeanings = new List<Tuple<JPWord, string>>();
			Concepts = new List<Tuple<JPWord, string>>();
		}

		public JPDictionaryEntry (JPWord word) {
			Word = word;
			ExactMeanings = new List<Tuple<JPWord, string>>();
			Concepts = new List<Tuple<JPWord, string>>();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			return Word + " - " + (ExactMeanings.Count + Concepts.Count) + " meanings." ;
		}
	}
}
