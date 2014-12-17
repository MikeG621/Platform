/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * - Release
 */

using System;
using Idmr.Common;

namespace Idmr.Platform
{
	public abstract partial class BaseFlightGroup
	{
		/// <summary>Base class for FlightGroup orders</summary>
		[Serializable] public abstract class BaseOrder : Indexer<byte>
		{
			// Doesn't need protected constructors, all handling done in derived classes
			#region public properties
			/// <summary>Gets or sets the command for the FlightGroup</summary>
			public byte Command
			{
				get { return _items[0]; }
				set { _items[0] = value; }
			}
			/// <summary>Gets or sets the throttle setting</summary>
			/// <remarks>Multiply value by 10 to get Throttle percent</remarks>
			public byte Throttle
			{
				get { return _items[1]; }
				set { _items[1] = value; }
			}
			/// <summary>Gets or sets the first order-specific setting</summary>
			public byte Variable1
			{
				get { return _items[2]; }
				set { _items[2] = value; }
			}
			/// <summary>Gets or sets the second order-specific setting</summary>
			public byte Variable2
			{
				get { return _items[3]; }
				set { _items[3] = value; }
			}
			/// <summary>Gets or sets the Type for <see cref="Target3"/></summary>
			public byte Target3Type
			{
				get { return _items[6]; }
				set { _items[6] = value; }
			}
			/// <summary>Gets or sets the Type for <see cref="Target4"/></summary>
			public byte Target4Type
			{
				get { return _items[7]; }
				set { _items[7] = value; }
			}
			/// <summary>Gets or sets the third target</summary>
			public byte Target3
			{
				get { return _items[8]; }
				set { _items[8] = value; }
			}
			/// <summary>Gets or sets the fourth target</summary>
			public byte Target4
			{
				get { return _items[9]; }
				set { _items[9] = value; }
			}
			/// <summary>Gets or sets if the T3 and T4 settings are mutually exclusive</summary>
			/// <remarks><b>false</b> is And/If and <b>true</b> is Or</remarks>
			public bool T3AndOrT4
			{
				get { return Convert.ToBoolean(_items[10]); }
				set { _items[10] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the Type for <see cref="Target1"/></summary>
			public byte Target1Type
			{
				get { return _items[12]; }
				set { _items[12] = value; }
			}
			/// <summary>Gets or sets the first target</summary>
			public byte Target1
			{
				get { return _items[13]; }
				set { _items[13] = value; }
			}
			/// <summary>Gets or sets the Type for <see cref="Target2"/></summary>
			public byte Target2Type
			{
				get { return _items[14]; }
				set { _items[14] = value; }
			}
			/// <summary>Gets or sets the second target</summary>
			public byte Target2
			{
				get { return _items[15]; }
				set { _items[15] = value; }
			}
			/// <summary>Gets or sets if the T1 and T2 settings are mutually exclusive</summary>
			/// <remarks><b>false</b> is And/If and <b>true</b> is Or</remarks>
			public bool T1AndOrT2
			{
				get { return Convert.ToBoolean(_items[16]); }
				set { _items[16] = Convert.ToByte(value); }
			}
			#endregion public properties
		}
	}
}
