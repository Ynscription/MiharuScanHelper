using Miharu.BackEnd.Data.KanjiByRad;
using System;

namespace Miharu.Control
{
	public delegate void KanjiInputEventHandler (object sender, KanjiInputEventArgs e);
	
	
	
	public class KanjiInputEventArgs : EventArgs
	{
		public JPChar Character {
			get; set;
		}

		public KanjiInputEventArgs (JPChar character) {
			Character = character;
		}


	}
}
