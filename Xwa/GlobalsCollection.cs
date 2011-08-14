using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object to maintain all Team Global Goals</summary>
	public class GlobalsCollection : FixedSizeCollection
	{
		/// <summary>Create a new Collection of Global Goals for each team (10)</summary>
		public GlobalsCollection()
		{
			_count = 10;
			_items = new Globals[_count];
			for (int i=0;i<_count;i++) _items[i] = new Globals();
		}

		/// <value>A single Globals object within the Collection</value>
		public Globals this[int index]
		{
			get { return (Globals)_getItem(index); }
			set { _setItem(index, value); }
		}

		/// <summary>Resets Globals Goals for selected Team to defaults</summary>
		/// <param name="team">Team index</param>
		public void Clear(int team) { _setItem(team, new Globals()); }

		/// <summary>Resets all Global Goals to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Globals()); }
	}
}