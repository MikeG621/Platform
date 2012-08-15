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
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class Team
	{
		/// <summary>Object to provide array access to the EoM Message notes</summary>
		/// <remarks>Used primarily as instructions for the voice actors. Three strings, one per EoM Message pair.</remarks>
		public class EomMessageNoteIndexer : Indexer<string>
		{
			/// <summary>Initializes the indexer</summary>
			/// <remarks>Character limit set to 100</remarks>
			/// <param name="eomNotes">The EoM Message Notes array</param>
			public EomMessageNoteIndexer(string[] eomNotes) : base(eomNotes, 100) { }
			
			/// <summary>Gets or sets the EoM Message Note</summary>
			/// <remarks>100 character limit. Index will be divided by 2, so PrimaryFailed1 and PrimaryFailed2 both point to the same string</remarks>
			/// <param name="index">EomMessageIndex</param>
			public string this[EomMessageIndex index]
			{
				get { return _items[(int)index / 2]; }
				set { this[(int)index / 2] = value; }
			}
		}
	}
}
