/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] auto-properties
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implementation
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual Team settings</summary>
	[Serializable]
	public partial class Team
	{
		/// <summary>IFF relations to other Teams</summary>
		public enum Allegeance : byte
		{
			/// <summary>Enemy craft</summary>
			Hostile,
			/// <summary>Friendly craft</summary>
			Friendly,
			/// <summary>Third-party craft</summary>
			Neutral
		}

		/// <summary>Indexes for <see cref="EndOfMissionMessages"/></summary>
		public enum EomMessageIndex : byte
		{
			/// <summary>First message displayed after Primary mission is completed</summary>
			PrimaryComplete1,
			/// <summary>Second message displayed after Primary mission is completed</summary>
			PrimaryComplete2,
			/// <summary>First message displayed after Primary mission is failed</summary>
			PrimaryFailed1,
			/// <summary>Second message displayed after Primary mission is failed</summary>
			PrimaryFailed2,
			/// <summary>First message displayed after mission is completed with exceptional results</summary>
			OutstandingComplete1,
			/// <summary>Second message displayed after mission is completed with exceptional results</summary>
			OutstandingComplete2
		}

		string _name;
		readonly string[] _endOfMissionMessages = new string[6];
		readonly string[] _eomNotes = new string[3];
		readonly string[] _voiceIDs = new string[3];

		/// <summary>Initializes a new team</summary>
		/// <remarks><see cref="Name"/> initializes according to <paramref name="teamNumber"/>; <b>0</b> = "Imperial", <b>1</b> = "Rebel", other = "Team #"</remarks>
		/// <param name="teamNumber">Team index being initialized</param>
		public Team(int teamNumber)
		{
			if (teamNumber <= 0) { _name = "Imperial"; Allies[0] = Allegeance.Friendly; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber > 8 ? 10 : teamNumber + 1);
			for (int i = 0; i < 6; i++) _endOfMissionMessages[i] = "";
			for (int i = 0; i < 3; i++)
			{
				_voiceIDs[i] = "";
				_eomNotes[i] = "";
			}
			EndOfMissionMessages = new EomMessageIndexer(_endOfMissionMessages);
			VoiceIDs = new VoiceIDIndexer(_voiceIDs);
			EomNotes = new EomMessageNoteIndexer(_eomNotes);
		}
		/// <summary>Gets or sets the Team name</summary>
		/// <remarks>17 character limit</remarks>
		public string Name
		{
			get => _name;
			set => _name = StringFunctions.GetTrimmed(value, 0x11);
		}

		/// <summary>Gets the array accessor for the EoM Messages</summary>
		public EomMessageIndexer EndOfMissionMessages { get; private set; }

		/// <summary>Gets the array accessor for the EoM Message Notes</summary>
		public EomMessageNoteIndexer EomNotes { get; private set; }

		/// <summary>Gets the allegiance array with other teams</summary>
		/// <remarks>Array is Length = 10</remarks>
		public Allegeance[] Allies { get; } = new Allegeance[10];

		/// <summary>Gets the unknown values</summary>
		/// <remarks>Array is Length = 6</remarks>
		public byte[] Unknowns { get; } = new byte[6];

		/// <summary>Gets the array accessor for the short EoM Message notes</summary>
		public VoiceIDIndexer VoiceIDs { get; private set; }
	}
}