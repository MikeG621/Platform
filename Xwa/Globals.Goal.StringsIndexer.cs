/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 */

using System;

namespace Idmr.Platform.Xwa
{
	public partial class Globals
	{
		public partial class Goal
		{
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
				/// <param name="state"><see cref="GoalState"/> index</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> or <i>state</i> value</exception>
				public string this[int trigger, int state]
				{
					get { return _owner._strings[trigger, state]; }
					set { _owner._strings[trigger, state] = Idmr.Common.StringFunctions.GetTrimmed(value, 63); }
				}
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks><i>value</i> is limited to 63 characters</remarks>
				/// <param name="trigger">The Trigger index, 0-3</param>
				/// <param name="state"><see cref="GoalState"/> value</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> value</exception>
				public string this[int trigger, GoalState state]
				{
					get { return _owner._strings[trigger, (int)state]; }
					set { _owner._strings[trigger, (int)state] = Idmr.Common.StringFunctions.GetTrimmed(value, 63); }
				}
			}
		}
	}
}
