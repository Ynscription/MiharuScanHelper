using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu.BackEnd.Data
{
	class JPDictionaryEntry
	{

		public string Word {
			get; set;
		}

		public string Form {
			get; set;
		}

		public List<Tuple<string, string>> Meanings {
			get; set;
		}



	}
}
