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
	/// <summary>Object to maintain all Team Global Goals</summary>
	public class GlobalsCollection : Idmr.Common.FixedSizeCollection<Globals>
	{
		/// <summary>Creates a new Collection of Global Goals for each team (10)</summary>
		public GlobalsCollection()
		{
			_count = 10;
			_items = new Globals[_count];
			for (int i=0;i<_count;i++) _items[i] = new Globals();
		}

		/// <summary>Resets Globals Goals for selected Team to defaults</summary>
		/// <param name="team">Team index</param>
		public void Clear(int team) { _setItem(team, new Globals()); }

		/// <summary>Resets all Global Goals to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Globals()); }
	}
}