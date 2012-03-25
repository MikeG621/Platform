/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG 
 * 120222 - Indexer<T> implementation
 * *** v2.0 ***
 */
using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual Team settings</summary>
	[Serializable]
	public class Team
	{
		/// <summary>IFF relations to other Teams</summary>
		public enum Allegeance : byte {
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
		string[] _endOfMissionMessages = new string[6];
		string[] _eomNotes = new string[3];
		Allegeance[] _allies = new Allegeance[10];
		byte[] _unknowns = new byte[6];
		string[] _voiceIDs = new string[3];
		EomMessageIndexer _eomMessageIndexer;
		VoiceIDIndexer _voiceIDIndexer;
		EomMessageNoteIndexer _notesIndexer;

		/// <summary>Initializes a new team</summary>
		/// <remarks><see cref="Name"/> initializes according to <i>teamNumber</i>; <b>0</b> = "Imperial", <b>1</b> = "Rebel", other = "Team #"</remarks>
		/// <param name="teamNumber">Team index being initialized</param>
		public Team(int teamNumber)
		{
			if (teamNumber <= 0) { _name = "Imperial"; _allies[0] = Allegeance.Friendly; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber > 8 ? 10 : teamNumber + 1);
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
			for (int i = 0; i < 3; i++)
			{
				_voiceIDs[i] = "";
				_eomNotes[i] = "";
			}
			_eomMessageIndexer = new EomMessageIndexer(_endOfMissionMessages);
			_voiceIDIndexer = new VoiceIDIndexer(_voiceIDs);
			_notesIndexer = new EomMessageNoteIndexer(_eomNotes);
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

		/// <summary>Gets the array accessor for the EoM Message Notes</summary>
		public EomMessageNoteIndexer EomNotes { get { return _notesIndexer; } }

		/// <summary>Gets the allegiance array with other teams</summary>
		/// <remarks>Array is Length = 10</remarks>
		public Allegeance[] Allies { get { return _allies; } }
		
		/// <summary>Gets the unknown values</summary>
		/// <remarks>Array is Length = 6</remarks>
		public byte[] Unknowns { get { return _unknowns; } }
		
		/// <summary>Gets the array accessor for the short EoM Message notes</summary>
		public VoiceIDIndexer VoiceIDs { get { return _voiceIDIndexer; } }
		
		/// <summary>Object to provide array access to the EoM Message values</summary>
		/// <remarks>Six EoM Messages, use <see cref="EomMessageIndex"/> for indexes</remarks>
		public class EomMessageIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <remarks>Character limit set to 64</remarks>
			/// <param name="eomMessages">The EoM Message array</param>
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
		/// <remarks>Used primarily to identify the speaker. Three strings, one per EoM Message pair.</remarks>
		public class VoiceIDIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <remarks>Character limit set to 20</remarks>
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

		/// <summary>Object to provide array access tot he EoM Message notes</summary>
		/// <remarks>Used primarily as instructions for the voice actors. Six notes, use <see cref="EomMessageIndex"/> for indexes</remarks>
		public class EomMessageNoteIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <remarks>Character limit set to 100</remarks>
			/// <param name="eomNotes">The EoM Message Notes array</param>
			public EomMessageNoteIndexer(string[] eomNotes) : base(eomNotes, 100) { }
			
			/// <summary>Gets or sets the EoM Message Note</summary>
			/// <remarks>100 character limit</remarks>
			/// <param name="index">EomMessageIndex</param>
			public string this[EomMessageIndex index]
			{
				get { return _items[(int)index / 2]; }
				set { this[(int)index / 2] = value; }
			}
		}
	}
}
