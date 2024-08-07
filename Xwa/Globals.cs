﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] auto-property
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual Team's mission-wide goals.</summary>
	[Serializable] public partial class Globals
	{
		/// <summary>Creates a new Globals object.</summary>
		/// <remarks>Three <see cref="Goals"/>, each with four <see cref="Goal.Triggers"/> all set to "<b>never (FALSE)</b>".</remarks>
		public Globals() { for (int i = 0; i < 3; i++) Goals[i] = new Goal(); }

		/// <summary>The Global Goals.</summary>
		/// <remarks>Use the <see cref="GoalIndex"/> enumeration for indexes.</remarks>
		public Goal[] Goals { get; } = new Goal[3];
		
		/// <summary>Goal indexes.</summary>
		public enum GoalIndex : byte {
			/// <summary>Primary goals.</summary>
			Primary,
			/// <summary>Prevent goals.</summary>
			Prevent,
			/// <summary>Secondary goals.</summary>
			Secondary
		}
		/// <summary>Goal status.</summary>
		public enum GoalState : byte {
			/// <summary>Goals that have not yet been completed.</summary>
			/// <remarks>Does not apply to Secondary goals.</remarks>
			Incomplete,
			/// <summary>Goals that have been completed.</summary>
			Complete,
			/// <summary>Goals that cannot be completed due to gameplay (mission design may permit inability to complete goals without direct failure).</summary>
			/// <remarks>Does not apply to Prevent or Secondary goals.</remarks>
			Failed
		}

		// TODO XWA: in a separate code branch, explore the possibilty of using the extra globals, 3-6
		// maybe use the classic editor to test missions
	}
}
