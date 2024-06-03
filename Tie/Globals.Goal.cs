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
 * [UPD] Triggers to auto-property
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 */

namespace Idmr.Platform.Tie
{
	public partial class Globals
	{
		/// <summary>Object for a single Global Goal.</summary>
		public class Goal
		{
			string _name = "";

			/// <summary>Initialize a new Goal.</summary>
			/// <param name="condition">Default Trigger.Condition.</param>
			/// <remarks>If <paramref name="condition"/> is set to <b>10</b> ("never (FALSE)"), <see cref="T1AndOrT2"/> is set to <b>true</b> ("OR").</remarks>
			public Goal(byte condition)
			{
				for (int i = 0; i < 2; i++)
				{
					Triggers[i] = new Mission.Trigger
					{
						Condition = condition
					};
				}
				T1AndOrT2 = (condition == 10);
			}
			
			/// <summary>Determines if both Triggers must be met.</summary>
			public bool T1AndOrT2 { get; set; }
			/// <summary>Gets the Triggers that define the Goal.</summary>
			/// <remarks>Array length is 2.</remarks>
			public Mission.Trigger[] Triggers { get; private set; } = new Mission.Trigger[2];

			/// <summary>Gets or sets the editor name of the goal.</summary>
			/// <remarks>Limited to 15 characters.</remarks>
			public string Name
			{
				get => _name;
				set => _name = Common.StringFunctions.GetTrimmed(value, 0xF);
			}
		}
	}
}