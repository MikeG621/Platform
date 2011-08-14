using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object to hold all Briefings</summary>
	/// <remarks>Two briefings in total, first being used for single-player</remarks>
	public class BriefingCollection : FixedSizeCollection
	{
		/// <summary>Create a new Collection with 2 Briefings</summary>
		public BriefingCollection()
		{
			_count = 2;
			_items = new Briefing[_count];
			for (int i=0;i<_count;i++) _items[i] = new Briefing();
		}
		
		/// <value>A single Briefing within the Collection</value>
		public Briefing this[int index]
		{
			get { return (Briefing)_getItem(index); }
			set { _setItem(index, value); }
		}

		/// <summary>Resets selected Briefing to defaults</summary>
		/// <param name="index">Briefing index</param>
		public void Clear(int index) { _setItem(index, new Briefing()); }

		/// <summary>Resets all Briefings to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Briefing()); }
	}
}