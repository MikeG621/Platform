/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the mission-wide goals</summary>
	public class Globals
	{
		Goal[] _goals = new Goal[3];

		/// <summary>Initializes a blank set of Global Goals</summary>
		/// <remarks>Three Goals with two Triggers each, all set to "never (FALSE)"</remarks>
		public Globals() { for (int i = 0; i < 3; i++) _goals[i] = new Goal(10); }

		/// <summary>Gets the Global Goals array</summary>
		/// <remarks>Array is Primary, Secondary and Bonus</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Object for a single Global Goal</summary>
		public class Goal
		{
			Mission.Trigger[] _triggers = new Mission.Trigger[2];
			
			/// <summary>Initialize a new Goal</summary>
			/// <remarks>If <i>condition</i> is set to 10, "never (FALSE)", AndOr values set to <i>true</i>, "OR"</remarks>
			/// <param name="condition">Default Trigger.Condition</param>
			public Goal(byte condition)
			{
				for (int i = 0; i < 2; i++) { _triggers[i] = new Mission.Trigger(); _triggers[i].Condition = condition; }
				T1AndOrT2 = (condition == 10);
			}
			
			/// <summary>Determines if both Triggers must be met</summary>
			public bool T1AndOrT2 { get; set; }
			/// <summary>Gets the Triggers that define the Goal</summary>
			/// <remarks>Array length is 2</remarks>
			public Mission.Trigger[] Triggers { get { return _triggers; } }
		}
	}
}
