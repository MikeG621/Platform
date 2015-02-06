/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2015 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.2
 */

/* CHANGELOG
 * v2.2, 150205
 * [FIX] marked Serializable
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class Team
	{
		/// <summary>Object to provide array access to the EoM Message notes</summary>
		/// <remarks>Used primarily to identify the speaker. Three strings, one per EoM Message pair.</remarks>
		[Serializable]
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
	}
}
