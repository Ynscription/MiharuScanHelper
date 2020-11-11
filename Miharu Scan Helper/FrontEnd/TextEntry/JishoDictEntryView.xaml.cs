using Miharu.BackEnd.Data;
using System;
using System.Windows.Controls;
using System.Windows.Documents;

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
					ProcessDefinitions(_entry.Item2);
				}
			}
		}
		
        private void ProcessDefinitions (string def) {
			DefinitionsTextBlock.Inlines.Clear();
			bool first = true;
			foreach (string line in def.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) {
				char flag = line [0];
				Run r;
				switch (flag) {
					case '$':
						if (first)
							first = false;
						else
							DefinitionsTextBlock.Inlines.Add(new LineBreak());
						r = new Run();
						r.Style = (System.Windows.Style)Resources["SmallAndGray"];
						r.Text = line.Substring(1);
						DefinitionsTextBlock.Inlines.Add(r);						
						break;
					case '#':
						DefinitionsTextBlock.Inlines.Add(new LineBreak());
						r = new Run();
						r.Style = (System.Windows.Style)Resources["Gray"];
						r.Text = line.Substring(1) + " ";
						DefinitionsTextBlock.Inlines.Add(r);
						break;
					case '@':
						r = new Run();
						r.Text = line.Substring(1) + " ";
						DefinitionsTextBlock.Inlines.Add(r);
						break;
					case '&':
						r = new Run();
						r.Style = (System.Windows.Style)Resources["SmallAndGray"];
						r.Text = line.Substring(1) + " ";
						DefinitionsTextBlock.Inlines.Add(r);	
						break;
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
