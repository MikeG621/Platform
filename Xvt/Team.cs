/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0, 120525
 * [UPD] EndOfMissionMessages is now Indexer<T>
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

		/// <summary>Indexes for <see cref="EndOfMissionMessageColor"/> and <see cref="EndOfMissionMessages"/></summary>
		public enum MessageIndex : byte {
			/// <summary>First message displayed after Primary mission is complete</summary>
			PrimaryComplete1,
			/// <summary>Second message displayed after Primary mission is complete</summary>
			PrimaryComplete2,
			/// <summary>First message displayed after Primary mission is failed</summary>
			PrimaryFailed1,
			/// <summary>Second message displayed after Primary mission is failed</summary>
			PrimaryFailed2,
			/// <summary>First message displayed after Secondary mission is complete</summary>
			SecondaryComplete1,
			/// <summary>Second message displayed after Secondary mission is complete</summary>
			SecondaryComplete2
		}
		
		/// <summary>Initializes a new team</summary>
		/// <remarks><see cref="Name"/> initializes according to <i>teamNumber</i>; <b>0</b> = "Imperial", <b>1</b> = "Rebel", other = "Team #"</remarks>
		/// <param name="teamNumber">Team index being initialized. Corrects to <b>0-9</b> as required</param>
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
		/// <remarks>Use the <see cref="MessageIndex"/> enumeration for array indexes</remarks>
		public byte[] EndOfMissionMessageColor { get { return _endOfMissionMessageColor; } }
		
		/// <summary>Gets the array accessor for the EoM Messages</summary>
		/// <remarks>Use the <see cref="MessageIndex"/> enumeration for array indexes</remarks>
		public Indexer<string> EndOfMissionMessages { get { return _eomMessageIndexer; } }
	}
}
