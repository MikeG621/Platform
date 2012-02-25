/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG 
 * 120222 - Indexer<T> implementation
 */
using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Object for individual Team settings</summary>
	public class Team
	{
		/// <summary>IFF relations to other Teams</summary>
		public enum Allegeance : byte { Hostile, Friendly, Neutral }
		
		/// <summary>Indexes for <i>EndOfMissionMessages</i></summary>
		public enum EomMessageIndex : byte { PrimaryComplete1, PrimaryComplete2, PrimaryFailed1, PriamryFailed2, OutstandingComplete1, OutstandingComplete2 }
		
		string _name;
		string[] _endOfMissionMessages = new string[6];
		Allegeance[] _allies = new Allegeance[10];
		byte[] _unknowns = new byte[6];
		string[] _voiceIDs = new string[3];
		EomMessageIndexer _eomMessageIndexer;
		VoiceIDIndexer _voiceIDIndexer;

		/// <summary>Initializes a new team</summary>
		/// <remarks><i>Name</i> initializes according to <i>teamNumber</i>; 0 = Imperial, 1 = Rebel, other = Team #</remarks>
		/// <param name="teamNumber">Team index being initialized</param>
		public Team(int teamNumber)
		{
			if (teamNumber <= 0) { _name = "Imperial"; _allies[0] = Allegeance.Friendly; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber > 8 ? 10 : teamNumber + 1);
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
			for (int i=0;i<3;i++) _voiceIDs[i] = "";
			_eomMessageIndexer = new EomMessageIndexer(_endOfMissionMessages);
			_voiceIDIndexer = new VoiceIDIndexer(_voiceIDs);
		}
		/// <summary>Gets or sets the Team name</summary>
		/// <remarks>17 character limit</remarks>
		public string Name
		{
			get { return _name; }
			set { _name = StringFunctions.GetTrimmed(value, 0x11); }
		}

		/// <summary>Gets the array accessor for the EoM Messages</summary>
		public EomMessageIndexer EndOfMissionMessages { get { return _eomMessageIndexer; } }

		/// <summary>Gets the allegiance array with other teams</summary>
		/// <remarks>Array is Length = 10</remarks>
		public Allegeance[] Allies { get { return _allies; } }
		
		/// <summary>Gets the unknown values</summary>
		/// <remarks>Array is Length = 6</remarks>
		public byte[] Unknowns { get { return _unknowns; } }
		
		/// <summary>Gets the array accessor for the short EoM Message notes</summary>
		public VoiceIDIndexer VoiceIDs { get { return _voiceIDIndexer; } }
		
		/// <summary>Object to provide array access to the EoM Message values</summary>
		public class EomMessageIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <param name="eommessages">The EoM Message array</param>
			public EomMessageIndexer(string[] eomMessages) : base(eomMessages, 64) { }
			
			/// <summary>Gets or sets the EoM Message</summary>
			/// <remarks>64 character limit</remarks>
			/// <param name="index">EomMessageIndex</param>
			public string this[EomMessageIndex index]
			{
				get { return _items[(int)index]; }
				set { this[(int)index] = value; }
			}
		}
		
		/// <summary>Object to provide array access to the EoM Message notes</summary>
		public class VoiceIDIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <param name="voiceIDs">The VoiceID array</param>
			public VoiceIDIndexer(string[] voiceIDs) : base(voiceIDs, 20) { }
			
			/// <summary>Gets or sets the EoM Message note</summary>
			/// <remarks>20 character limit. Index will be divided by 2, so PrimaryFailed1 and PrimaryFailed2 both point to the same string</remarks>
			/// <param name="index">EomMessage category</param>
			public string this[EomMessageIndex index]
			{
				get { return _items[(int)index / 2]; }
				set { this[(int)index / 2] = value; }
			}
		}
	}
}
