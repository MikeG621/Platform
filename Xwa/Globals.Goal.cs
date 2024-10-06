/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [NEW] Format spec implemented
 * [UPD] Delay renamed to RawDelay
 * v4.0, 200809
 * [UPD] auto-properties
 * v2.7, 180509
 * [UPD] just tweaked the Trigger declaration
 * v2.1, 141214
 * [UPD] change to MPL
 */

namespace Idmr.Platform.Xwa
{
	public partial class Globals
	{
		/// <summary>Class for a single Global Goal.</summary>
		public partial class Goal
		{
			readonly string[,] _strings = new string[4,3];  // No PreventFailed, SecondaryIncomplete, SecondaryFailed
			string _name = "";
			
			/// <summary>Initializes a blank Goal.</summary>
			/// <remarks>All <see cref="Triggers"/> set to <b>"never (FALSE)"</b>, <see cref="AndOr"/> values set to <b>true</b> (OR).</remarks>
			public Goal()
			{
				for (int i = 0; i < 12; i++) _strings[i / 3, i % 3] = "";
				for (int i = 0; i < 4; i++) Triggers[i] = new Mission.Trigger() { Condition = (byte)Mission.Trigger.ConditionList.False };
				for (int i = 0; i < 3; i++) AndOr[i] = true;
				GoalStrings = new StringsIndexer(this);
			}

			#region properties
			/// <summary>The Triggers that define the Goal.</summary>
			/// <remarks>Array length is 4.</remarks>
			public Mission.Trigger[] Triggers { get; } = new Mission.Trigger[4];
			/// <summary>Gets the array for the AndOr values.</summary>
			public bool[] AndOr { get; } = new bool[3];
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
			/// <summary>Editor note for the goal.</summary>
			/// <remarks>Limited to 15 char.</remarks>
			public string Name
			{
				get => _name;
				set => _name = Common.StringFunctions.GetTrimmed(value, 15);
			}
			/// <summary>Unknown.</summary>
			public byte Version { get; set; }
			/// <summary>Gets or sets if both Trigger pairs must be met.</summary>
			public bool T12AndOrT34
			{
				get => AndOr[2];
				set => AndOr[2] = value;
			}
			/// <summary>Delay value between trigger condition and goal award.</summary>
			public byte RawDelay { get; set; }
			/// <summary>Raw value stored in file.</summary>
			public sbyte RawPoints { get; set; }
			/// <summary>Raw points awarded per specific trigger completion.</summary>
			public sbyte[] RawPointsPerTrigger { get; } = new sbyte[4];
			/// <summary>The Goal's location within the mission's Active Sequence.</summary>
			public byte ActiveSequence { get; set; }
			
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion.</summary>
			/// <remarks>Equal to <see cref="RawPoints"/> * 25, limited from <b>-3200</b> to <b>+3175</b>.</remarks>
			public short Points
			{
				get => (short)(RawPoints * 25);
				set => RawPoints = pointsToRaw(value);
			}
			#endregion

			/// <summary>Get the points awarded or subtracted after Trigger completion.</summary>
			/// <param name="index">The <see cref="Triggers"/> index.</param>
			/// <returns>Equal to <see cref="RawPointsPerTrigger"/>[index] * 25.</returns>
			public short GetPointsPerTrigger(int index) => index >= 0 && index < 4 ? (short)(RawPointsPerTrigger[index] * 25) : (short)0;
			/// <summary>Set the points awarded or subtracted after Trigger completion.</summary>
			/// <param name="index">The <see cref="Triggers"/> index.</param>
			/// <param name="points">The point total, limited from <b>-3200</b> to <b>+3175</b>.</param>
			public void SetPointsPerTrigger(int index, short points) { if (index >= 0 && index < 4) RawPointsPerTrigger[index] = pointsToRaw(points); }

			/// <summary>Gets the array accessor for the GoalString values.</summary>
			public StringsIndexer GoalStrings { get; private set; }

			private sbyte pointsToRaw(short value) => (sbyte)(value > 3175 ? 3175 : (value < -3200 ? -3200 : value) / 25);
		}
	}
}
