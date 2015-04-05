/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2015 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.3
 */

/* CHANGELOG
 * v2.3, 150405
 * [UPD] new subclass Trigger, GoalStringIndexer removed
 * [NEW] Serializable
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [UPD] AndOr to bool[]
 * - removed AndOrIndexer
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class Globals
	{
		/// <summary>Container for a single Global Goal</summary>
		[Serializable]
		public partial class Goal
		{
			Trigger[] _triggers = new Trigger[4];	// No PreventFailed, SecondaryIncomplete, SecondaryFailed
			bool[] _andOrs = new bool[3];

			/// <summary>Initializes a new Goal</summary>
			/// <remarks>All <see cref="Triggers"/> set to <b>10</b>, "never (FALSE)". <see cref="AndOr"/> values set to <b>true</b>, "OR"</remarks>
			public Goal()
			{
				for (int i = 0; i < 4; i++) _triggers[i] = new Trigger(this);
				for (int i = 0; i < 3; i++) _andOrs[i] = true;
			}
			
			/// <summary>Raw value stored in file</summary>
			public sbyte RawPoints { get; set; }
			
			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T1AndOrT2
			{
				get { return _andOrs[0]; }
				set { _andOrs[0] = value; }
			}
			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T3AndOrT4
			{
				get { return _andOrs[1]; }
				set { _andOrs[1] = value; }
			}
			/// <summary>Gets or sets if both Trigger pairs must be met</summary>
			public bool T12AndOrT34
			{
				get { return _andOrs[2]; }
				set { _andOrs[2] = value; }
			}

			/// <summary>Gets the array for the AndOr values</summary>
			public bool[] AndOr { get { return _andOrs; } }

			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equal to <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b></remarks>
			public short Points
			{
				get { return (short)(RawPoints * 250); }
				set { RawPoints = (sbyte)(value > 31750 ? 31750 : (value < -32000 ? -32000 : value) / 250); }
			}
			
			/// <summary>The Triggers that define the Goal</summary>
			/// <remarks>Array length is 4</remarks>
			public Trigger[] Triggers { get { return _triggers; } }
		}
	}
}
