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
 * [FIX] Name now does 15 char instead of 14
 * v4.0, 200809
 * [UPD] auto-properties
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [UPD] EndOfMissionMessages is now Indexer<T>
 */
using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual Team settings.</summary>
	[Serializable] public class Team
	{
		string _name = "";
		readonly string[] _endOfMissionMessages = new string[6];

		/// <summary>Indexes for <see cref="EndOfMissionMessageColor"/> and <see cref="EndOfMissionMessages"/>.</summary>
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
		/// <remarks><see cref="Name"/> initializes according to <paramref name="teamNumber"/>; <b>0</b> = "Imperial", <b>1</b> = "Rebel", other = "Team #".</remarks>
		/// <param name="teamNumber">Team index being initialized. Corrects to <b>0-9</b> as required.</param>
		public Team(int teamNumber)
		{
			if (teamNumber <= 0) { _name = "Imperial"; AlliedWithTeam[0] = true; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber > 8 ? 10 : teamNumber + 1);
			for (int i = 0; i < 6; i++) _endOfMissionMessages[i] = "";
			EndOfMissionMessages = new Indexer<string>(_endOfMissionMessages, 63);
		}

		/// <summary>Gets or sets the Team name.</summary>
		/// <remarks>15 character limit.</remarks>
		public string Name
		{
			get => _name;
			set => _name = StringFunctions.GetTrimmed(value, 0xF);
		}
		/// <summary>Gets the allegiance with other teams.</summary>
		/// <remarks>Array is Length = 10.</remarks>
		public bool[] AlliedWithTeam { get; } = new bool[10];

		/// <summary>Gets or sets the color of the specified EoM Message.</summary>
		/// <remarks>Use the <see cref="MessageIndex"/> enumeration for array indexes.</remarks>
		public byte[] EndOfMissionMessageColor { get; } = new byte[6];
		
		/// <summary>Gets the array accessor for the EoM Messages.</summary>
		/// <remarks>Use the <see cref="MessageIndex"/> enumeration for array indexes.</remarks>
		public Indexer<string> EndOfMissionMessages { get; private set; }

		/// <summary>Gets the array for the delay between EoM condition and the messages being shown.</summary>
		/// <remarks>Array is Length = 3.</remarks>
		public byte[] EomRawDelay { get; } = new byte[3];
	}
}