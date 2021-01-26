using Miharu2.BackEnd.Translation;
using System;

namespace Miharu2.BackEnd.Data
{

	public delegate void TxtEventHandler (object sender, TxtChangedEventArgs e);
	public enum TextChangeType {
		Vertical,
		Parse,
		TranslationSource,
		Translation
	}
	public class TxtChangedEventArgs : EventArgs
	{
		
		public TextChangeType ChangeType {
			get;
		}

		public TranslationType? TranslationType {
			get;
		}

		public string Text {
			get;
		}

		public TxtChangedEventArgs (TextChangeType textChangeType, TranslationType? translationType, string text) {
			ChangeType = textChangeType;
			TranslationType = translationType;
			Text = text;
		}


	}
}
