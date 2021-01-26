using System;

namespace Miharu2.Control
{

	public delegate void ListModificationEventHandler (object sender, ListModificationEventArgs e);
	public class ListModificationEventArgs : EventArgs
	{
		public int EventOldIndex {
			get;
			set;
		}

		public int EventNewIndex {
			get;
			set;
		} = -1;

		public object EventObject {
			get; 
			set;
		} = null;

		public ListModificationEventArgs (int oldIndex, object eventObject = null, int newIndex = -1) {
			EventOldIndex = oldIndex;
			EventObject = eventObject;
			EventNewIndex = newIndex;
		}

	}
}
