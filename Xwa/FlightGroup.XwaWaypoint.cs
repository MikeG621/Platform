/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7+
 */

/* CHANGELOG
 * [UPD] ToString formatted as literal
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] conversions
 * [NEW] ToString() override
 */

using System;

namespace Idmr.Platform.Xwa
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for a single Waypoint.</summary>
		[Serializable] public class XwaWaypoint : Waypoint
		{
			/// <summary>Default constructor.</summary>
			public XwaWaypoint() => _items = new short[5];

			/// <summary>Constructs a new Waypoint from an existing Waypoint. If null, a blank Waypoint is created.</summary>
			/// <param name="wp">Existing Waypoint to clone. If <b>null</b>, Waypoint will be blank.</param>
			public XwaWaypoint(XwaWaypoint wp) : this() { if (wp != null) Array.Copy(wp._items, _items, _items.Length); }

			/// <summary>Constructs a new Waypoint from an existing Waypoint. If null, a blank Waypoint is created.</summary>
			/// <param name="wp">Existing Waypoint to clone. If <b>null</b>, Waypoint will be blank.</param>
			public XwaWaypoint(Waypoint wp) : this()
			{
				if (wp != null)
				{
					short[] values = (short[])wp;
					Array.Copy(values, _items, values.Length);
				}
			}

			/// <summary>Initialize a new Waypoint using raw data.</summary>
			/// <remarks><paramref name="raw"/> must have Length of at least 4.</remarks>
			/// <param name="raw">Raw data.</param>
			/// <exception cref="ArgumentException">Incorrect <paramref name="raw"/> length.</exception>
			public XwaWaypoint(short[] raw) : this()
			{
				if (raw.Length == 5) _items = raw;
				else if (raw.Length >= 4) Array.Copy(raw, _items, raw.Length);
				else throw new ArgumentException("Array must have a Length of at least 4.");
			}

			/// <summary>Returns a representative string of the Waypoint.</summary>
			/// <returns>Waypoint coordinates in the form of <b>"(<see cref="BaseFlightGroup.Waypoint.X"/>, <see cref="BaseFlightGroup.Waypoint.Y"/>, <see cref="BaseFlightGroup.Waypoint.Z"/>) <see cref="Region"/> #"</b>if enabled, otherwise <b>"Disabled"</b>.</returns>
			public override string ToString() => (Enabled ? "$({X}, {Y}, {Z}) Region {Region}" : "Disabled");

			/// <summary>Gets a clean copy of the Waypoint.</summary>
			/// <returns>A new copy.</returns>
			new public XwaWaypoint Clone()
			{
				var wp = new XwaWaypoint();
				for (int i = 0; i < _items.Length; i++) wp[i] = _items[i];
				return wp;
			}

			/// <summary>Gets or sets the Region that the Waypoint is located in.</summary>
			/// <remarks>Restricted to values <b>0-3</b>.</remarks>
			public byte Region
			{
				get => (byte)_items[4];
				set { if (value < 4 && value >= 0) _items[4] = value; }
			}
		}
	}
}