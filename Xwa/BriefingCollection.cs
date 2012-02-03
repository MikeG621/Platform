/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object to hold all Briefings</summary>
	/// <remarks>Two briefings in total, first being used for single-player</remarks>
	public class BriefingCollection : Idmr.Common.FixedSizeCollection<Briefing>
	{
		/// <summary>Create a new Collection with 2 Briefings</summary>
		public BriefingCollection()
		{
			_count = 2;
			_items = new Briefing[_count];
			for (int i=0;i<_count;i++) _items[i] = new Briefing();
		}

		/// <summary>Resets selected Briefing to defaults</summary>
		/// <param name="index">Briefing index</param>
		public void Clear(int index) { _setItem(index, new Briefing()); }

		/// <summary>Resets all Briefings to defaults</summary>
		public void ClearAll() { for (int i=0;i<_count;i++) _setItem(i, new Briefing()); }
	}
}