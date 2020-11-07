using Miharu.BackEnd.Data.KanjiByRad;
using System;
using System.Collections.Generic;

namespace Miharu.Control
{
	public class KanjiInputManager
	{


		#region Events

		public event EventHandler VisibilityChanged;


		public event KanjiInputEventHandler KanjiInputEvent;

		public event EventHandler KanjiListChanged;
		

		#endregion

		private KanjiByRad _kanjiByRad = null;


		private bool _visibility;
		private List<JPChar> _cachedKanjiList = null;

		public bool KanjiInputWindowVisibility {
			get {
				return _visibility;
			}
			set {
				_visibility = value;
				VisibilityChanged?.Invoke(this, new EventArgs());
			}
		}


		public List<JPChar> RadList { get { return _kanjiByRad.GetRadList(); } }

		public List<JPChar> KanjiList {
			get => _cachedKanjiList;
		}


		public KanjiInputManager () {
			KanjiInputWindowVisibility = false;
			_kanjiByRad = new KanjiByRad();
			_cachedKanjiList = _kanjiByRad.GetKanjiList();
			_kanjiByRad.KanjiListChanged += _kanjiByRad_KanjiListChanged;
		}

		private void _kanjiByRad_KanjiListChanged(object sender, EventArgs e)
		{
			_cachedKanjiList = _kanjiByRad.GetKanjiList();
			KanjiListChanged?.Invoke(this, e);
		}

		public void SelectRad (JPChar rad) {
			_kanjiByRad.SelectRad(rad);
		}

		public void DeselectRad (JPChar rad) {
			_kanjiByRad.DeselectRad(rad);
		}

		internal void InputKanji(JPChar txtContent)
		{
			KanjiInputEvent?.Invoke(this, new KanjiInputEventArgs(txtContent));
		}
	}
}
