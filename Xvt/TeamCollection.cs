/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 120212 - T[] to List<T> conversion
 * 120308 - GetList()
 * *** v2.0 ***
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain Teams</summary>
	public class TeamCollection : Idmr.Common.FixedSizeCollection<Team>
	{
		/// <summary>Creates a new Collection of Teams (10)</summary>
		public TeamCollection()
		{
			_items = new List<Team>(10);
			for (int i = 0; i < _items.Capacity; i++) _items.Add(new Team(i));
		}

		/// <summary>Resets selected Team to defaults</summary>
		/// <param name="team">Team index</param>
		public void Clear(int team) { _setItem(team, new Team(team)); }

		/// <summary>Resets all Teams to defaults</summary>
		public void ClearAll() { for (int i = 0; i < Count; i++) _setItem(i, new Team(i)); }

		/// <summary>Provides quick access to an array of Team <see cref="Team.Name">Names</see></summary>
		/// <returns>A new array with the Team <see cref="Team.Name">Names</see></returns>
		public string[] GetList()
		{
			string[] list = new string[Count];
			for (int i = 0; i < Count; i++) list[i] = _items[i].Name;
			return list;
		}
	}
}