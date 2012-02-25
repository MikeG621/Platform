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
	/// <summary>Object for individual Team's mission-wide goals</summary>
	[Serializable] public class Globals
	{
		Goal[] _goals = new Goal[3];

		/// <summary>Creates a new Globals object</summary>
		/// <remarks>Three Goals, each with four Triggers all set to "never (FALSE)"</remarks>
		public Globals() { for (int i = 0; i < 3; i++) _goals[i] = new Goal(); }
		
		/// <summary>The Global Goals</summary>
		/// <remarks>Use the <i>GoalIndex</i> enumeration for indexes</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Goal indexes</summary>
		/// <remarks>0 = Primary<br>1 = Prevent<br>2 = Secondary</remarks>
		public enum GoalIndex : byte { Primary, Prevent, Secondary };
		/// <summary>Goal status</summary>
		/// <remarks>0 = Incomplete<br>1 = Complete<br>2 = Failed</remarks>
		public enum GoalState : byte { Incomplete, Complete, Failed };
		
		/// <summary>Class for a single Global Goal</summary>
		public class Goal
		{
			Mission.Trigger[] _triggers = new Mission.Trigger[4];
			string[,] _strings = new string[4,3];	// No PreventFailed, SecondaryIncomplete, SecondaryFailed
			bool[] _andOrs = new bool[3];
			StringsIndexer _triggerStrings;
			
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
			/// <remarks>All <i>Trigger.Conditions</i>s set to "never (FALSE)", AndOr values set to <i>true</i> (OR)</remarks>
			public Goal()
			{
				for (int i = 0; i < 12; i++) _strings[i / 3, i % 3] = "";
				for (int i = 0; i < 4; i++) { _triggers[i] = new Mission.Trigger(); _triggers[i].Condition = 10; }
				for (int i = 0; i < 3; i++) _andOrs[i] = true;
				_triggerStrings = new StringsIndexer(this);
			}
			
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summ>
			/// <remarks>Equal to RawPoints * 25, limited from -3200 to +3175</remarks>
			public short Points
			{
				get { return (short)(RawPoints * 25); }
				set { RawPoints = (sbyte)(value > 3175 ? 3175 : (value < -3200 ? -3200 : value) / 25); }
			}
			
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
			
			/// <summary>Gets the array accessor for the GoalString values</summary>
			public StringsIndexer GoalStrings { get { return _triggerStrings; } }
			
			/// <summary>The Triggers that define the Goal</summary>
			/// <remarks>Array length is 4</remarks>
			public Mission.Trigger[] Triggers { get { return _triggers; } }

			/// <summary>Object to provide array access to the Trigger Strings</summary>
			public class StringsIndexer
			{
				Goal _owner;
				
				/// <summary>Initialize the indexer</summary>
				/// <param name="parent">Parent Global Goal</param>
				public StringsIndexer(Goal parent) { _owner = parent; }
				
				/// <summary>Length of the array</summary>
				public int Length { get { return _owner._strings.Length; } }
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks><i>value</i> is limited to 63 characters</remarks>
				/// <param name="trigger">The Trigger index, 0-3</param>
				/// <param name="state">GoalState value</param>
				/// <exception cref="IndexOutOfBoundsException">Invalid <i>trigger</i> or <i>state</i> value</exception>
				public string this[int trigger, int state]
				{
					get { return _owner._strings[trigger, state]; }
					set { _owner._strings[trigger, state] = Idmr.Common.StringFunctions.GetTrimmed(value, 63); }
				}
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks><i>value</i> is limited to 63 characters</remarks>
				/// <param name="trigger">The Trigger index, 0-3</param>
				/// <param name="state">GoalState value</param>
				/// <exception cref="IndexOutOfBoundsException">Invalid <i>trigger</i> value</exception>
				public string this[int trigger, GoalState state]
				{
					get { return _owner._strings[trigger, (int)state]; }
					set { _owner._strings[trigger, (int)state] = Idmr.Common.StringFunctions.GetTrimmed(value, 63); }
				}
			}
		}
	}
}
