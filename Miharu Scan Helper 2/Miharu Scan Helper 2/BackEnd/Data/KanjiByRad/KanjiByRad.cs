using System;
using System.Collections.Generic;
using System.Linq;

namespace Miharu2.BackEnd.Data.KanjiByRad
{
	class KanjiByRad
	{
		private KozakuraDBDriver _driver;


		private List<JPChar> _selectedRads;
		private Dictionary<JPChar, HashSet<JPChar>> _kanjiByRads;
		private List<JPChar> _radList;



		private HashSet<JPChar> _currentKanjiSelection;


		public event EventHandler KanjiListChanged;


		public KanjiByRad()
		{
			_driver = new KozakuraDBDriver();
			_radList = new List<JPChar>();
			_kanjiByRads = new Dictionary<JPChar, HashSet<JPChar>>();
			_selectedRads = new List<JPChar>();
			_currentKanjiSelection = new HashSet<JPChar>();
			FillRads();
		}

		private void FillRads()
		{
			foreach (JPChar rad in _driver.GetRadList()) {
				_radList.Add(rad);
				HashSet<JPChar> kanji = _driver.KanjiByRad(rad);
				_kanjiByRads.Add(rad, kanji);
			}
		}

		public void SelectRad(JPChar rad)
		{
			
			
			HashSet<JPChar> kanji = _kanjiByRads[rad];
			if (_currentKanjiSelection.Count == 0 && _selectedRads.Count == 0)
				_currentKanjiSelection.UnionWith(kanji);				
			else if (_currentKanjiSelection.Count > 0)
				_currentKanjiSelection.IntersectWith(kanji);

			KanjiListChanged?.Invoke(this, new EventArgs());

			_selectedRads.Add(rad);
			rad.IsSelected = true;

			foreach (JPChar r in _radList) {
				if (r.IsSelected || !r.IsEnabled)
					continue;

				var checkKanji = _currentKanjiSelection.Intersect(_kanjiByRads[r]);
				if (checkKanji.Count() == 0)
					r.IsEnabled = false;
			}

		}

		internal void ClearRads()
		{
			foreach (JPChar rad in _selectedRads)
				rad.IsSelected = false;
			_selectedRads.Clear();
			_currentKanjiSelection.Clear();
			KanjiListChanged?.Invoke(this, new EventArgs());

			foreach (JPChar r in _radList)
				r.IsEnabled = true;

		}

		public void DeselectRad(JPChar rad)
		{
			_selectedRads.Remove(rad);
			rad.IsSelected = false;

			_currentKanjiSelection.Clear();



			bool first = true;
			foreach (JPChar r in _selectedRads)
			{
				HashSet<JPChar> kanji = _kanjiByRads[r];
				if (first) {
					_currentKanjiSelection.UnionWith(kanji);
					first = false;
				}
				else
					_currentKanjiSelection.IntersectWith(kanji);

			}

			KanjiListChanged?.Invoke(this, new EventArgs());

			foreach (JPChar r in _radList) {
				if (r.IsSelected || r.IsEnabled)
					continue;

				if (_selectedRads.Count > 0) {
					var checkKanji = _currentKanjiSelection.Intersect(_kanjiByRads[r]);
					if (checkKanji.Count() > 0)
						r.IsEnabled = true;
				}
				else
					r.IsEnabled = true;

			}

		}

		public List<JPChar> GetRadList()
		{
			return _radList;
		}

		public List<JPChar> GetKanjiList()
		{
			List<JPChar> kanjiList = new List<JPChar>();

			foreach (JPChar kanji in _currentKanjiSelection.OrderBy(
					item =>	item.Strokes).ThenBy(item => item.Id)) {
				kanjiList.Add(kanji);
			}

			return kanjiList;
		}

		
	}
}
