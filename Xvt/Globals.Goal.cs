/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0+
 */

/* CHANGELOG
 * [NEW] Format spec implemented
 * v4.0, 200809
 * [UPD] auto-properties
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

namespace Idmr.Platform.Xvt
{
	public partial class Globals
	{
		/// <summary>Container for a single Global Goal.</summary>
		[Serializable]
		public partial class Goal
		{
			string _name = "";

			/// <summary>Initializes a new Goal</summary>
			/// <remarks>All <see cref="Triggers"/> set to <b>10</b>, "never (FALSE)". <see cref="AndOr"/> values set to <b>true</b>, "OR".</remarks>
			public Goal()
			{
				for (int i = 0; i < 4; i++) Triggers[i] = new Trigger();
				for (int i = 0; i < 3; i++) AndOr[i] = true;
			}
			
			/// <summary>Raw value stored in file.</summary>
			public sbyte RawPoints { get; set; }
			
			/// <summary>Gets or sets if both Triggers must be met.</summary>
			public bool T1AndOrT2
			{
				get => AndOr[0];
				set => AndOr[0] = value;
			}
			/// <summary>Gets or sets if both Triggers must be met.</summary>
			public bool T3AndOrT4
			{
				get => AndOr[1];
				set => AndOr[1] = value;
			}
			/// <summary>Gets or sets if both Trigger pairs must be met.</summary>
			public bool T12AndOrT34
			{
				get => AndOr[2];
				set => AndOr[2] = value;
			}

			/// <summary>Gets the array for the AndOr values.</summary>
			public bool[] AndOr { get; } = new bool[3];

			/// <summary>Gets or sets the editor name of the goal.</summary>
			/// <remarks>Limited to 15 characters.</remarks>
			public string Name
			{
				get => _name;
				set => _name = Common.StringFunctions.GetTrimmed(value, 0xF);
			}
			/// <summary>Probably unused.</summary>
			public byte Version { get; set; }
			/// <summary>Gets or sets the seconds after trigger is fired divided by five.</summary>
			/// <remarks>Default is <b>zero</b>. Value of <b>1</b> is 5s, <b>2</b> is 10s, etc.</remarks>
			public byte Delay { get; set; }
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion.</summary>
			/// <remarks>Equal to <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b>.</remarks>
			public short Points
			{
				get => (short)(RawPoints * 250);
				set => RawPoints = (sbyte)(value > 31750 ? 31750 : (value < -32000 ? -32000 : value) / 250);
			}

			/// <summary>The Triggers that define the Goal.</summary>
			/// <remarks>Array length is 4.</remarks>
			public Trigger[] Triggers { get; } = new Trigger[4];    // No PreventFailed, SecondaryIncomplete, SecondaryFailed
		}
	}
}