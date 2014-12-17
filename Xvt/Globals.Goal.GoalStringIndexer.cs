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
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class Globals
	{
		public partial class Goal
		{
			/// <summary>Object to provide array access to the GoalString values</summary>
			public class GoalStringIndexer
			{
				Goal _owner;
				
				/// <summary>Initializes the indexer</summary>
				/// <param name="parent">Parent Global Goal</param>
				public GoalStringIndexer(Goal parent) { _owner = parent; }
				
				/// <summary>Gets the size of the array</summary>
				public int Length { get { return _owner._goalStrings.Length; } }
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks>Limited to 63 characters</remarks>
				/// <param name="trigger">Trigger index, 0-3</param>
				/// <param name="state">GoalState value, 0-2</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> or <i>state</i> value</exception>
				public string this[int trigger, int state]
				{
					get { return _owner._goalStrings[trigger, state]; }
					set { _owner._goalStrings[trigger, state] = StringFunctions.GetTrimmed(value, 63); }
				}
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks>Limited to 63 characters</remarks>
				/// <param name="trigger">Trigger index, 0-3</param>
				/// <param name="state">GoalState value</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> value</exception>
				public string this[int trigger, GoalState state]
				{
					get { return _owner._goalStrings[trigger, (int)state]; }
					set { _owner._goalStrings[trigger, (int)state] = StringFunctions.GetTrimmed(value, 63); }
				}
			}
		}
	}
}
