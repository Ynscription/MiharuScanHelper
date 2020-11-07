
using Miharu.BackEnd.Data.KanjiByRad;
using Miharu.Control;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Miharu.FrontEnd.Input
{
	/// <summary>
	/// Interaction logic for KanjiByRadInputControl.xaml
	/// </summary>
	public partial class KanjiByRadInputControl : UserControl
	{


		private KanjiInputManager _kanjiInputManager = null;

		public KanjiByRadInputControl(KanjiInputManager kanjiInputManager)
		{
			InitializeComponent();

			_kanjiInputManager = kanjiInputManager;


			try {
				Mouse.OverrideCursor = Cursors.Wait;
				int currStrokes = 0;
				foreach (JPChar r in _kanjiInputManager.KanjiList) {
					if (r.Strokes > currStrokes) {
						KanjiWrapPanel.Children.Add(new InputBoxInverted(r));
						currStrokes = r.Strokes;
					}
					KanjiWrapPanel.Children.Add(new InputBox(r, _kanjiInputManager));
				}


			
				currStrokes = 0;
				foreach (JPChar r in _kanjiInputManager.RadList) {
					if (r.Strokes > currStrokes) {
						RadWrapPanel.Children.Add(new InputBoxInverted(r));
						currStrokes = r.Strokes;
					}
					RadWrapPanel.Children.Add(new InputBox(r, _kanjiInputManager));
				}
			}
			finally {
				Mouse.OverrideCursor = null;
			}

			_kanjiInputManager.KanjiListChanged += _kanjiInputManager_KanjiListChanged;
		}

		private void _kanjiInputManager_KanjiListChanged(object sender, EventArgs e)
		{
			try {
				Mouse.OverrideCursor = Cursors.Wait;
				KanjiWrapPanel.Children.Clear();
				int currStrokes = 0;
				foreach (JPChar r in _kanjiInputManager.KanjiList) {
					if (r.Strokes > currStrokes) {
						KanjiWrapPanel.Children.Add(new InputBoxInverted(r));
						currStrokes = r.Strokes;
					}
					KanjiWrapPanel.Children.Add(new InputBox(r, _kanjiInputManager));
				}
			}
			finally {
				Mouse.OverrideCursor = null;
			}
		}
	}
}
