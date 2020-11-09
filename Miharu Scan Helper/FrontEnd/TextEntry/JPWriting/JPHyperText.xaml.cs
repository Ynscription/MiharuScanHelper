using Miharu.BackEnd.Data;
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
	/// Interaction logic for JPHyperText.xaml
	/// </summary>
	public partial class JPHyperText : UserControl
	{

		
		public event EventHandler OnHyperTextClick;


		private JPWord _word;

		public JPWord Word {
			get => _word;
			set {
				_word = value;

				WordStackPanel.Children.Clear();

				if (_word.Furigana.Count == 1 && _word.Furigana[0].Item1 == -1) {
					JPCharView c = new JPCharView(_word.Word, _word.Furigana[0].Item2);
					c.FontSize = _word.FontSize;
					WordStackPanel.Children.Add(c);
				}
				else if (_word.Furigana.Count > 0){
					int index = 0;
					foreach (var furigana in _word.Furigana) {
						string character;
						for (; index < furigana.Item1; index++) {
							character = "" + _word.Word[index];
							if (Char.IsSurrogate(_word.Word[index]))
								character += _word.Word[++index];
							JPCharView characterView = new JPCharView(character);
							characterView.FontSize = _word.FontSize;
							WordStackPanel.Children.Add(characterView);
						}
						
						character = "" + _word.Word[index];
						if (Char.IsSurrogate(_word.Word[index]))
							character += _word.Word[++index];
						JPCharView c = new JPCharView(character, furigana.Item2);
						c.FontSize = _word.FontSize;
						WordStackPanel.Children.Add(c);
						index++;
					}
					JPCharView endingChar = new JPCharView(_word.Word.Substring(index));
					endingChar.FontSize = _word.FontSize;
					WordStackPanel.Children.Add(endingChar);
				}
				else {
					JPCharView c = new JPCharView(_word.Word);
					c.FontSize = _word.FontSize;
					WordStackPanel.Children.Add(c);
				}

			}
		}
		

		private bool _isClickable = false;
		public bool IsClickable {
			get => _isClickable;
			set {
				_isClickable = value;
				Underline.Visibility = _isClickable ? Visibility.Visible : Visibility.Hidden;
			}
		}
		



		public JPHyperText()
		{
			InitializeComponent();
			MouseLeftButtonUp += OnClick;
			MouseEnter += LinkLabel_MouseEnter;
			MouseLeave += LinkLabel_MouseLeave;
		}

		public JPHyperText(JPWord word)
		{
			InitializeComponent();
			MouseLeftButtonUp += OnClick;
			MouseEnter += LinkLabel_MouseEnter;
			MouseLeave += LinkLabel_MouseLeave;

			Word = word;
		}

		private void OnClick(object sender, MouseButtonEventArgs e)
		{
			if (IsClickable)
				OnHyperTextClick?.Invoke(Tag, new EventArgs());
		}

		private void LinkLabel_MouseEnter(object sender, MouseEventArgs e)
		{
			if (IsClickable) {
				Mouse.OverrideCursor = Cursors.Hand;
				Underline.StrokeThickness = 2;
			}
		}

		private void LinkLabel_MouseLeave(object sender, MouseEventArgs e)
		{
			if (IsClickable) {
				Mouse.OverrideCursor = null;
				Underline.StrokeThickness = 1;
			}
		}	
	}
}
