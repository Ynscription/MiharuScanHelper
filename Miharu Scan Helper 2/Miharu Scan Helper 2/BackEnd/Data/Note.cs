using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Data
{
	public class Note
	{
		public readonly Guid Uuid;

		public string Content {
			get; set;
		}

		public Note () {
			Uuid = Guid.NewGuid();
		}
		public Note (string content) {
			Uuid = Guid.NewGuid();
			Content = content;
		}

	}
}
