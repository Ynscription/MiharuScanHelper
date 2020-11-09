using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Miharu.FrontEnd.TextEntry.JPWriting
{
	/// <summary>
	/// Interaction logic for JPCharView.xaml
	/// </summary>
	
	public partial class JPCharView : UserControl
	{
		private string _character;
		public string Character {
			get => _character;
			set {
				_character = value;
				CharacterLabel.Content = _character;
			}
		}


		private string _furigana;
		public string Furigana {
			get => _furigana;
			set {
				_furigana = value;
				FuriganaLabel.Content = _furigana;
			}
		}



		public new double FontSize {
			get => CharacterLabel.FontSize;
			set {
				double v = value;
				if (v == 0)
					v = 12;
				CharacterLabel.FontSize = v;
				FuriganaLabel.FontSize = v/2.5;
			}
		}

		
		
		public JPCharView()
		{
			InitializeComponent();
		}

		public JPCharView(string character)
		{
			InitializeComponent();
			Character = character;
			Furigana = "";
		}

		public JPCharView(string character, string furigana)
		{
			InitializeComponent();
			Character = character;
			Furigana = furigana;
		}
	}
}
