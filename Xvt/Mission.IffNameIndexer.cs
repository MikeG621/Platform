/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2020 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.1+
 */

/* CHANGELOG
 * v3.2, XXXXXX
 * [NEW] created [JB]
 */

using Idmr.Common;
using System;

namespace Idmr.Platform.Xvt
{
	public partial class Mission : MissionFile
	{
		/// <summary>Object to provide array access to the IFF names</summary>
		public class IffNameIndexer	: Indexer<string>
		{	
			Mission _owner;
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent Mission</param>
			public IffNameIndexer(Mission parent) : base(parent._iff) { _owner = parent; }

			/// <summary>Gets or sets the IFF Name</summary>
			/// <remarks>20 character limit, Rebel and Imperial are read-only</remarks>
			/// <param name="index">IFF index</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <paramref name="index"/> value</exception>
			/// <exception cref="InvalidOperationException">Index is read-only</exception>
			/// <returns>The IFF name</returns>
			public override string this[int index]
			{
				get { return _items[index]; }
				set
				{
					if (index < 2) throw new InvalidOperationException("Index (" + index + ") is read-only");
					if (value != "" && value[0] == '1')
					{
						value = value.Substring(1);
					}
					_items[index] = StringFunctions.GetTrimmed(value, 20);
				}
			}
		}
	}
}
