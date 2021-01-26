

using Miharu2.BackEnd.Data;

namespace Miharu2.BackEnd.Translation.Threading
{
	public class TranslationRequest
	{
		public Text Destination { get; set; }
		public TranslationType? Type {get; set; }
		public string Text {get; set; }
		public TranslationConsumer Consumer {get; set; }

		public TranslationRequest (Text destination, TranslationType? t, string text, TranslationConsumer consumer) {
			Destination = destination;
			Type = t;
			Text = text;
			Consumer = consumer;
		}

	}
}
