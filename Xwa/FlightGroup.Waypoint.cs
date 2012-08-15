/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0, 120525
 * [NEW] conversions
 * [NEW] ToString() override
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for a single Waypoint</summary>
		[Serializable] public class Waypoint : BaseWaypoint
		{
			/// <summary>Default constructor</summary>
			public Waypoint() : base(new short[5]) { /* do nothing */ }
			
			/// <summary>Initialize a new Waypoint using raw data</summary>
			/// <remarks><i>raw</i> must have Length of 4 or 5</remarks>
			/// <param name="raw">Raw data</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Waypoint(short[] raw)
			{
				if (raw.Length == 5) _items = raw;
				else if (raw.Length == 4)
				{
					_items = new short[5];
					ArrayFunctions.WriteToArray(raw, _items, 0);
				}
				else throw new ArgumentException("raw does not have the correct size");
			}

			/// <summary>Returns a representative string of the Waypoint</summary>
			/// <returns>Waypoint coordinates in the form of <b>"(<see cref="BaseFlightGroup.BaseWaypoint.X"/>, <see cref="BaseFlightGroup.BaseWaypoint.Y"/>, <see cref="BaseFlightGroup.BaseWaypoint.Z"/>) <see cref="Region"/> #"</b>if enabled, otherwise <b>"Disabled"</b></returns>
			public override string ToString() { return (Enabled ? "(" + X + ", " + Y + ", " + Z + ") Region " + Region : "Disabled"); }

			/// <summary>Converts a Waypoint for TIE</summary>
			/// <remarks><see cref="Region"/> is lost in the conversion</remarks>
			/// <param name="wp">The Waypoint to convert</param>
			/// <returns>A copy of <i>wp</i> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Waypoint(Waypoint wp) { return new Tie.FlightGroup.Waypoint((short[])wp); }
			/// <summary>Converts a Waypoint for XvT</summary>
			/// <remarks><see cref="Region"/> is lost in the conversion</remarks>
			/// <param name="wp">The Waypoint to convert</param>
			/// <returns>A copy of <i>wp</i> for use in XvT</returns>
			public static explicit operator Xvt.FlightGroup.Waypoint(Waypoint wp) { return new Xvt.FlightGroup.Waypoint((short[])wp); }
			
			/// <summary>Gets or sets the Region that the Waypoint is located in</summary>
			/// <remarks>Restricted to values <b>0-3</b></remarks>
			public byte Region
			{
				get { return (byte)_items[4]; }
				set { if (value < 4 && value >= 0) _items[4] = value; }
			}
		}
	}
}
