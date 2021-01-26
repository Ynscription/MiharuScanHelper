using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Data
{
	public class JPWord
	{
		
		public List<Tuple<int, string>> Furigana {
			get; set;
		}

		public string Word {
			get; set;
		}

		public JPWord () {
			Furigana = new List<Tuple<int, string>>();
			Word = "";
		}

		public override string ToString()
		{
			return Word;
		}

		
	}
}
