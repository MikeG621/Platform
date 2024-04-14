/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System.Collections.Generic;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object to maintain all Team Global Goals.</summary>
	public class GlobalsCollection : Common.FixedSizeCollection<Globals>
	{
		/// <summary>Creates a new Collection of Global Goals for each team (10).</summary>
		public GlobalsCollection()
		{
			_items = new List<Globals>(10);
			for (int i = 0; i < _items.Capacity; i++) _items.Add(new Globals());
		}

		/// <summary>Resets Globals Goals for selected Team to defaults.</summary>
		/// <param name="team">Team index.</param>
		public void Clear(int team) => _setItem(team, new Globals());

		/// <summary>Resets all Global Goals to defaults.</summary>
		public void ClearAll() { for (int i = 0; i < Count; i++) _setItem(i, new Globals()); }
	}
}