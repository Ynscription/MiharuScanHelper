using Miharu.BackEnd.Data;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Miharu.FrontEnd.TextEntry
{
	/// <summary>
	/// Interaction logic for JishoDictEntryView.xaml
	/// </summary>
	public partial class JishoDictEntryView : UserControl
	{

		public Tuple<JPWord, string> _entry;
		
		public Tuple<JPWord, string> Entry {
			get => _entry;
			set { 
				_entry = value;
				if (_entry != null) {
					WordHyperText.Word = _entry.Item1;
					DefinitionsTextBlock.Text = _entry.Item2;
				}
			}
		}
		
        

			

		public JishoDictEntryView()
		{
			InitializeComponent();
		}

		public JishoDictEntryView(Tuple<JPWord, string> entry)
		{
			InitializeComponent();

			Entry = entry;
		}

		
	}
}
