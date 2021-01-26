
using System;

namespace Miharu2.BackEnd.Translation
{

	public delegate void TranslationFailEventHandler (object sender, TranslationFailEventArgs e);
	
	
	
	public class TranslationFailEventArgs : EventArgs
	{
		public Exception Exception {
			get; set;
		}

		public TranslationType Type {
			get; set;
		}

		public TranslationFailEventArgs (Exception e, TranslationType type) {
			Exception = e;
			Type = type;
		}


	}
}
