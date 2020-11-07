using System;

namespace Miharu.BackEnd.Data.KanjiByRad
{
	public class JPChar
	{
		public string Lit {
			get;
		}
		public int Id {
			get;
		}

		public int Strokes {
			get;
		}

		public JPChar(int id, string lit, int strokes, bool isRad)
		{
			Lit = lit;
			Id = id;
			Strokes = strokes;
			IsEnabled = true;
			IsSelected = false;
			IsRad  = isRad;
		}

		public static implicit operator string(JPChar value)
		{
			return value.Lit;
		}


		public static bool operator ==(JPChar c1, JPChar c2)
		{
			return c1.Id == c2.Id;
		}

		public static bool operator !=(JPChar c1, JPChar c2)
		{
			return c1.Id != c2.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is JPChar)
				return Id == ((JPChar)obj).Id;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Id;
		}




		public event EventHandler SelectedChanged;
		public event EventHandler EnabledChanged;

		private bool _isSelected;
		private bool _isEnabled;

		public bool IsSelected {
			get {
				return _isSelected;
			}
			set {
				_isSelected = value;
				SelectedChanged?.Invoke(this, new EventArgs());
			}
		}

		public bool IsEnabled {
			get {
				return _isEnabled;
			}
			set {
				_isEnabled = value;
				EnabledChanged?.Invoke(this, new EventArgs());
			}
		}

		public bool IsRad {
			get; set;
		}
	}
}
