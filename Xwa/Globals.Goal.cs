/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, [UPD] auto-properties
 * v2.7, 180509
 * [UPD] just tweaked the Trigger declaration
 * v2.1, 141214
 * [UPD] change to MPL
 */

namespace Idmr.Platform.Xwa
{
	public partial class Globals
	{
		/// <summary>Class for a single Global Goal</summary>
		public partial class Goal
		{
			readonly string[,] _strings = new string[4,3];	// No PreventFailed, SecondaryIncomplete, SecondaryFailed
			
			/// <summary>Raw value stored in file</summary>
			public sbyte RawPoints { get; set; }
			/// <summary>The Goal's location within the mission's Active Sequence</summary>
			public byte ActiveSequence { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x11</remarks>
			public bool Unknown1 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x29</remarks>
			public bool Unknown2 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x34</remarks>
			public byte Unknown3 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x36</remarks>
			public byte Unknown4 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x37</remarks>
			public byte Unknown5 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x38</remarks>
			public byte Unknown6 { get; set; }
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks>All <see cref="Triggers"/> set to <b>"never (FALSE)"</b>, <see cref="AndOr"/> values set to <b>true</b> (OR)</remarks>
			public Goal()
			{
				for (int i = 0; i < 12; i++) _strings[i / 3, i % 3] = "";
				for (int i = 0; i < 4; i++) Triggers[i] = new Mission.Trigger() { Condition = 10 };
				for (int i = 0; i < 3; i++) AndOr[i] = true;
				GoalStrings = new StringsIndexer(this);
			}
			
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equal to <see cref="RawPoints"/> * 25, limited from <b>-3200</b> to <b>+3175</b></remarks>
			public short Points
			{
				get => (short)(RawPoints * 25);
				set => RawPoints = (sbyte)(value > 3175 ? 3175 : (value < -3200 ? -3200 : value) / 25);
			}

			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T1AndOrT2
			{
				get => AndOr[0];
				set => AndOr[0] = value;
			}
			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T3AndOrT4
			{
				get => AndOr[1];
				set => AndOr[1] = value;
			}
			/// <summary>Gets or sets if both Trigger pairs must be met</summary>
			public bool T12AndOrT34
			{
				get => AndOr[2];
				set => AndOr[2] = value;
			}

			/// <summary>Gets the array for the AndOr values</summary>
			public bool[] AndOr { get; } = new bool[3];
			
			/// <summary>Gets the array accessor for the GoalString values</summary>
			public StringsIndexer GoalStrings { get; private set; }

			/// <summary>The Triggers that define the Goal</summary>
			/// <remarks>Array length is 4</remarks>
			public Mission.Trigger[] Triggers { get; } = new Mission.Trigger[4];
		}
	}
}
