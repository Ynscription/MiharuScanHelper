using Miharu2.BackEnd.Data.KanjiByRad;
using System;

namespace Miharu2.Control
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
