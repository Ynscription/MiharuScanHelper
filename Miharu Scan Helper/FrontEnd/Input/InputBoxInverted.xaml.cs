
using Miharu.BackEnd.Data.KanjiByRad;
using System.Windows.Controls;

namespace Miharu.FrontEnd.Input
{
	/// <summary>
	/// Interaction logic for InputBox.xaml
	/// </summary>
	public partial class InputBoxInverted : UserControl
	{
		private JPChar _txtContent;

		public JPChar TextContent {
			get {
				return _txtContent;
			}
			set {
				_txtContent = value;
				ContentLabelInverted.Content = "" + _txtContent.Strokes;
			}
		}

		public InputBoxInverted(JPChar txtContent)
		{
			InitializeComponent();
			TextContent = txtContent;
		}
	}
}
