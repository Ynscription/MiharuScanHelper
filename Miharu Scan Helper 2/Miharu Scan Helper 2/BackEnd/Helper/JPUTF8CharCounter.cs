using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miharu2.BackEnd.Helper
{
	public static class JPUTF8CharCounter
	{

		public static int StringLength(string s) {
			int length = 0;

			for (int i = 0; i < s.Length; i++) {
				length++;
				if (Char.IsSurrogate(s[i]))
					i++;
			}

			return length;
		}

	}
}
