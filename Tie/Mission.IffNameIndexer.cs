/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implementation
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Tie
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
			/// <remarks>11 character limit, Rebel and Imperial are read-only</remarks>
			/// <param name="index">IFF index</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
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
						_owner._iffHostile[index] = true;
						value = value.Substring(1);
					}
					_items[index] = StringFunctions.GetTrimmed(value, 11);
				}
			}
		}
	}
}
