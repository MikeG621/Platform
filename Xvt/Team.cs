/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG
 * 200212 - Indexer<T> implementation
 */
using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual Team settings</summary>
	[Serializable] public class Team
	{
		string _name = "";
		bool[] _alliedWithTeam = new bool[10];
		byte[] _endOfMissionMessageColor = new byte[6];
		string[] _endOfMissionMessages = new string[6];
		Indexer<string> _eomMessageIndexer;

		/// <summary>Indexes for <i>EndOfMissionMessageColor</i> and <i>EndOfMissionMessages</i></summary>
		public enum MessageIndex : byte { PrimaryComplete1, PrimaryComplete2, PrimaryFailed1, PrimaryFailed2, SecondaryComplete1, SecondaryComplete2 }
		
		/// <summary>Initializes a new team</summary>
		/// <remarks><i>Name</i> initializes according to <i>teamNumber</i>; 0 = Imperial, 1 = Rebel, other = Team (#+1)</remarks>
		/// <param name="teamNumber">Team index being initialized. Corrects to 0-9 as required</param>
		public Team(int teamNumber)
		{
			if (teamNumber <= 0) { _name = "Imperial"; _alliedWithTeam[0] = true; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber > 8 ? 10 : teamNumber + 1);
			for (int i = 0; i < 6; i++) _endOfMissionMessages[i] = "";
			_eomMessageIndexer = new Indexer<string>(_endOfMissionMessages, 63);
		}

		/// <summary>Gets or sets the Team name</summary>
		/// <remarks>15 character limit</remarks>
		public string Name
		{
			get { return _name; }
			set { _name = StringFunctions.GetTrimmed(value, 0xE); }
		}
		/// <summary>Gets the allegiance with other teams</summary>
		/// <remarks>Array is Length = 10</remarks>
		public bool[] AlliedWithTeam { get { return _alliedWithTeam; } }
		
		/// <summary>Gets or sets the color of the specified EoM Message</summary>
		/// <remarks>Use the MessageIndex enumeration for array indexes</remarks>
		public byte[] EndOfMissionMessageColor { get { return _endOfMissionMessageColor; } }
		
		/// <summary>Gets the array accessor for the EoM Messages</summary>
		public Indexer<string> EndOfMissionMessages { get { return _eomMessageIndexer; } }
	}
}
