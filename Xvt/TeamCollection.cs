using System;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain Teams</summary>
	public class TeamCollection : FixedSizeCollection
	{
		/// <summary>Create a new Collection of Teams (10)</summary>
		public TeamCollection()
		{
			_count = 10;
			_items = new Team[_count];
			for (int i=0;i<_count;i++) _items[i] = new Team(i);
		}

		/// <value>A single Team object within the Collection</value>
		public Team this[int index]
		{
			get { return (Team)_getItem(index); }
			set { _setItem(index, value); }
		}

		/// <summary>Resets selected Team to defaults</summary>
		/// <param name="team">Team index</param>
		public void Clear(int team) { _setItem(team, new Team(team)); }

		/// <summary>Resets all Teams to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Team(i)); }
	}
}