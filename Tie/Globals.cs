/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the mission-wide goals</summary>
	public partial class Globals
	{
		Goal[] _goals = new Goal[3];

		/// <summary>Initializes a blank set of Global Goals</summary>
		/// <remarks>Three Goals with two Triggers each, all set to <b>"never (FALSE)"</b></remarks>
		public Globals() { for (int i = 0; i < 3; i++) _goals[i] = new Goal(10); }

		/// <summary>Gets the Global Goals array</summary>
		/// <remarks>Array is Primary, Secondary and Bonus</remarks>
		public Goal[] Goals { get { return _goals; } }
	}
}
