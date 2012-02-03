/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain Teams</summary>
	public class TeamCollection : Idmr.Common.FixedSizeCollection<Team>
	{
		/// <summary>Creates a new Collection of Teams (10)</summary>
		public TeamCollection()
		{
			_count = 10;
			_items = new Team[_count];
			for (int i=0;i<_count;i++) _items[i] = new Team(i);
		}

		/// <summary>Resets selected Team to defaults</summary>
		/// <param name="team">Team index</param>
		public void Clear(int team) { _setItem(team, new Team(team)); }

		/// <summary>Resets all Teams to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Team(i)); }
	}
}