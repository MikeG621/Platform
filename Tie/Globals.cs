/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] Goals now uses auto-property, private set
 * v2.1, 141214
 * [UPD] change to MPL
 */

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the mission-wide goals.</summary>
	public partial class Globals
	{
		/// <summary>Initializes a blank set of Global Goals.</summary>
		/// <remarks>Three Goals with two Triggers each, all set to <b>"never (FALSE)"</b>.</remarks>
		public Globals() { for (int i = 0; i < 3; i++) Goals[i] = new Goal(10); }

		/// <summary>Gets the Global Goals array.</summary>
		/// <remarks>Array is Primary, Secondary and Bonus.</remarks>
		public Goal[] Goals { get; private set; } = new Goal[3];
	}
}