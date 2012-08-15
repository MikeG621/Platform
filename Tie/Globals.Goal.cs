/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * v2.0, 120525
 */

using System;

namespace Idmr.Platform.Tie
{
	public partial class Globals
	{
		/// <summary>Object for a single Global Goal</summary>
		public class Goal
		{
			Mission.Trigger[] _triggers = new Mission.Trigger[2];
			
			/// <summary>Initialize a new Goal</summary>
			/// <remarks>If <i>condition</i> is set to <b>10</b> ("never (FALSE)"), <see cref="T1AndOrT2"/> is set to <b>true</b> ("OR")</remarks>
			/// <param name="condition">Default Trigger.Condition</param>
			public Goal(byte condition)
			{
				for (int i = 0; i < 2; i++)
				{
					_triggers[i] = new Mission.Trigger();
					_triggers[i].Condition = condition;
				}
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
