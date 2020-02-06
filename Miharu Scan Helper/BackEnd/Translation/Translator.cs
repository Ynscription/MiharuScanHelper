

using System.Threading.Tasks;

namespace Miharu.BackEnd.Translation
{
	public abstract class Translator
	{

		public abstract Task<string> Translate (string text);

		public abstract TranslationType Type {
			get;
		}

	}
}
