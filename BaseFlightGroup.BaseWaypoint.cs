/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
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
		/// <summary>Base class for FlightGroup waypoints.</summary>
		[Serializable] public abstract class BaseWaypoint : Indexer<short>
		{
			/// <summary>Default constructor for derived classes.</summary>
			protected BaseWaypoint() { /* do nothing */ }

			/// <summary>Default constructor for derived classes.</summary>
			/// <param name="raw">Raw data.</param>
			protected BaseWaypoint(short[] raw) : base(raw) { /* do nothing */ }

			/// <summary>Returns a representative string of the Waypoint.</summary>
			/// <returns>Waypoint coordinates in the form of <b>"(X, Y, Z)"</b>if enabled, otherwise <b>"Disabled"</b>.</returns>
			public override string ToString() => (Enabled ? "(" + X + ", " + Y + ", " + Z + ") " : "Disabled");

			#region public properties
			/// <summary>Array form of the waypoint.</summary>
			/// <remarks><see cref="Enabled"/> restricted to <b>0</b> and <b>1</b>, <see cref="Xwa.FlightGroup.Waypoint.Region"/> restricted to <b>0-3</b>. No effect for invalid values.</remarks>
			/// <param name="index">X, Y, Z, Enabled, Region (XWA only).</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <paramref name="index"/> value.</exception>
			public override short this[int index]
			{
				get => _items[index];
				set
				{
					if (index == 3 && value != 0) _items[index] = 1;
					else if (Length == 5 && index == 4 && (value < 0 || value > 3)) return;
					else _items[index] = value;
				}
			}

			/// <summary>Gets or sets if the Waypoint is active for use.</summary>
			public bool Enabled
			{
				get => Convert.ToBoolean(_items[3]);
				set => _items[3] = Convert.ToInt16(value);
			}
			/// <summary>Gets or sets the stored X value.</summary>
			public short RawX
			{
				get => _items[0];
				set => _items[0] = value;
			}
			/// <summary>Gets or sets the stored Y value.</summary>
			/// <remarks>Is multipled by -1 during read/write to plot as positive-Y-Up in map and briefing.</remarks>
			public short RawY
			{
				get => _items[1];
				set => _items[1] = value;
			}
			/// <summary>Gets or sets the stored Z value.</summary>
			public short RawZ
			{
				get => _items[2];
				set => _items[2] = value;
			}
			/// <summary>Gets or sets the X value in kilometers.</summary>
			/// <remarks>Equals <see cref="RawX"/> divided by 160.</remarks>
			public double X
			{
				get => (double)RawX / 160;
				set => RawX = (short)(value * 160);
			}
			/// <summary>Gets or sets the Y value in kilometers.</summary>
			/// <remarks>Equals <see cref="RawY"/> divided by 160.</remarks>
			public double Y
			{
				get => (double)RawY / 160;
				set => RawY = (short)(value * 160);
			}
			/// <summary>Gets or sets the Z value in kilometers.</summary>
			/// <remarks>Equals <see cref="RawZ"/> divided by 160.</remarks>
			public double Z
			{
				get => (double)RawZ / 160;
				set => RawZ = (short)(value * 160);
			}
			#endregion public properties

			/// <summary>Converts a waypoint to a short array.</summary>
			/// <remarks>Always returns Length 4 array, even for XWA, due to how values are stored in the file.</remarks>
			/// <param name="wp">The waypoint to convert.</param>
			/// <returns>An array of shorts containing <see cref="X"/>, <see cref="Y"/>, <see cref="Z"/>, and <see cref="Enabled"/>.</returns>
			public static explicit operator short[](BaseWaypoint wp)
			{
				short[] s = new short[4];
				for (int i = 0; i < 4; i++) s[i] = wp[i];
				return s;
			}
		}
	}
}
