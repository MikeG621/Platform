/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 120213 - added BaseOrder, BaseWaypoint
 * 120220 - Indexer<T> implementation in BaseOrder/BaseWaypoint
 * *** v2.0 ***
 */
using System;
using Idmr.Common;

namespace Idmr.Platform
{
	/// <summary>Base class for FlightGroups</summary>
	/// <remarks>Contains values that are common to all FlightGroup types. Class is Serializable for copy/paste functionality</remarks>
	[Serializable]
	public abstract class BaseFlightGroup
	{
		/// <summary>Maximum string length for <see cref="Name"/>, <see cref="Cargo"/> and <see cref="SpecialCargo"/></summary>
		/// <remarks>Defaults to 0x13, may be redefined by derivative classes</remarks>
		protected int _stringLength = 0x13;
		/// <summary>Displayed name of the FG</summary>
		protected string _name = "New Ship";
		/// <summary>Displayed cargo</summary>
		protected string _cargo = "";
		/// <summary>Displayed special cargo</summary>
		protected string _specialCargo = "";
		/// <summary>Special craft index</summary>
		protected byte _specialCargoCraft = 0;
		/// <summary>Whether or not the Special Craft is decided at random</summary>
		protected bool _randSpecCargo = false;
		/// <summary>Raw X rotation in degrees</summary>
		protected short _yaw = 0;
		/// <summary>Raw Y rotation in degrees</summary>
		protected short _pitch = 0;
		/// <summary>Raw Z rotation in degrees</summary>
		protected short _roll = 0;

		/// <summary>Default constructor</summary>
		protected BaseFlightGroup()
		{
			CraftType = 1;
			NumberOfCraft = 1;
			NumberOfWaves = 1;
			AI = 3;
			FormDistance = 2;
		}

		#region Craft
		/// <summary>Gets or sets the name of FlightGroup</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE. Defaults to <b>"New Ship"</b></remarks>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = StringFunctions.GetTrimmed(value, _stringLength);
				if (_name.IndexOf('\0') != -1) _name = _name.Substring(0, _name.IndexOf('\0'));	// mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the default cargo assigned to entire FlightGroup</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE</remarks>
		public string Cargo
		{
			get { return _cargo; }
			set
			{
				_cargo = StringFunctions.GetTrimmed(value, _stringLength);
				if (_cargo.IndexOf('\0') != -1) _cargo = _cargo.Substring(0, _cargo.IndexOf('\0'));	// mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the cargo string for Special Craft</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE</remarks>
		public string SpecialCargo
		{
			get { return _specialCargo; }
			set
			{
				_specialCargo = StringFunctions.GetTrimmed(value, _stringLength);
				if (_specialCargo.IndexOf('\0') != -1) _specialCargo = _specialCargo.Substring(0, _specialCargo.IndexOf('\0')); // mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the Special Craft number</summary>
		/// <remarks>One-indexed, zero is <b>"none"</b> (default). Raw Data is zero-indexed, "none" value is <see cref="NumberOfCraft"/>. Setting to a non-zero value sets <see cref="RandSpecCargo"/> to <b>false</b></remarks>
		public byte SpecialCargoCraft
		{
			get { return _specialCargoCraft; }
			set 
			{
				_specialCargoCraft = value;
				if (_specialCargoCraft != 0) _randSpecCargo = false;
			}
		}
		/// <summary>Gets or sets whether or not a craft within the FlightGroup will be picked at random to use <see cref="SpecialCargo"/></summary>
		/// <remarks>Defaults to <b>false</b>. Setting to <b>true</b> sets <see cref="SpecialCargoCraft"/> to 0</remarks>
		public bool RandSpecCargo
		{
			get { return _randSpecCargo; }
			set
			{
				_randSpecCargo = value;
				if (_randSpecCargo) _specialCargoCraft = 0;
			}
		}
		/// <summary>Gets or sets the ship or object type</summary>
		/// <remarks>Defaults to <b>X-wing (1)</b></remarks>
		public byte CraftType { get; set; }
		/// <summary>Gets or sets the number of craft per wave</summary>
		/// <remarks>One-indexed, default is <b>1</b></remarks>
		public byte NumberOfCraft { get; set; }
		/// <summary>Gets or sets the condition of the FlightGroup upon creation</summary>
		public byte Status1 { get; set; }
		/// <summary>Gets or sets the warhead available to the FlightGroup</summary>
		public byte Missile { get; set; }
		/// <summary>Gets or sets the Beam weapon used in the FlightGroup</summary>
		public byte Beam { get; set; }
		/// <summary>Gets or sets the Friend or Foe setting</summary>
		/// <remarks>Determines color of FlightGroup name and allegiance</remarks>
		public byte IFF { get; set; }
		/// <summary>Gets or sets AI setting for NPC craft</summary>
		/// <remarks>Default index is <b>3</b></remarks>
		public byte AI { get; set; }
		/// <summary>Gets or sets the FlightGroup colors if applicable</summary>
		public byte Markings { get; set; }
		/// <summary>Gets or sets the FlightGroup formation</summary>
		/// <remarks>Default is <b>Vic (0)</b></remarks>
		public byte Formation { get; set; }
		/// <summary>Gets or sets the FlightGroup spacing while in formation</summary>
		/// <remarks>Default value is <b>2</b></remarks>
		public byte FormDistance { get; set; }
		/// <summary>Gets or sets the grouping assignment</summary>
		/// <remarks>Used to group multiple FlightGroups, used for triggers</remarks>
		public byte GlobalGroup { get; set; }
		/// <summary>Gets or sets how far in front of the FlightGroup the Leader is</summary>
		public byte FormLeaderDist { get; set; }
		/// <summary>Gets or sets the number of times the FlightGroup will arrive</summary>
		/// <remarks>One-indexed</remarks>
		public byte NumberOfWaves { get; set; }
		/// <summary>Gets or sets the craft index the Player occupies</summary>
		/// <remarks>One-indexed, <b>0</b> for AI-controlled</remarks>
		public byte PlayerCraft { get; set; }
		/// <summary>Gets or sets the craft z-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated before <see cref="Pitch"/> and <see cref="Roll"/>, in TIE only applies to SpaceObjects</remarks>
		public short Yaw
		{
			get { return _yaw; }
			set { if (value >= -180 && value < 180) _yaw = value; }
		}
		/// <summary>Gets or sets the craft y-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated after <see cref="Yaw"/> and before <see cref="Roll"/>, in TIE only applies to SpaceObjects.<br/>
		/// Flight engine adds -90 (nose down) pitch angle, automatically adjusted during Load/Save</remarks>
		public short Pitch
		{
			get { return _pitch; }
			set { if (value >= -180 && value < 180) _pitch = value; }
		}
		/// <summary>Gets or sets the craft x-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated after <see cref="Yaw"/> and <see cref="Pitch"/>, in TIE only applies to SpaceObjects</remarks>
		public short Roll
		{
			get { return _roll; }
			set { if (value >= -180 && value < 180) _roll = value; }
		}
		#endregion
		#region ArrDep
		/// <summary>Gets or sets the mission difficulty setting required for the FlightGroup to arrive</summary>
		/// <remarks>Defaults to All (0)</remarks>
		public byte Difficulty { get; set; }
		/// <summary>Gets or sets the delay in minutes after the Arrival Trigger conditions have been met</summary>
		/// <remarks>Can be used in conjunction with <see cref="ArrivalDelaySeconds"/></remarks>
		public byte ArrivalDelayMinutes { get; set; }
		/// <summary>Gets or sets the delay in seconds after the Arrival Trigger conditions have been met</summary>
		/// <remarks>Can be used in conjunction with <see cref="ArrivalDelayMinutes"/></remarks>
		public byte ArrivalDelaySeconds { get; set; }
		/// <summary>Gets or sets the delay in minutes after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</summary>
		/// <remarks>Can be used in conjunction with <see cref="DepartureTimerSeconds"/></remarks>
		public byte DepartureTimerMinutes { get; set; }
		/// <summary>Gets or sets the delay in seconds after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</summary>
		/// <remarks>Can be used in conjunction with <see cref="DepartureTimerMinutes"/></remarks>
		public byte DepartureTimerSeconds { get; set; }
		/// <summary>Gets or sets the individual craft departure requirement</summary>
		public byte AbortTrigger { get; set; }
		/// <summary>Gets or sets the primary arrival mothership FlightGroup index</summary>
		public byte ArrivalCraft1 { get; set; }
		/// <summary>Gets or sets the primary method of arrival</summary>
		/// <remarks>When <b>true</b> FlightGroup will attempt arrive via mothership, hyperspace when <b>false</b></remarks>
		public bool ArrivalMethod1 { get; set; }
		/// <summary>Gets or sets the secondary arrival mothership FlightGroup index</summary>
		public byte ArrivalCraft2 { get; set; }
		/// <summary>Gets or sets the secondary method of arrival</summary>
		/// <remarks>When <b>true</b> FlightGroup will attempt arrive via mothership, hyperspace when <b>false</b></remarks>
		public bool ArrivalMethod2 { get; set; }
		/// <summary>Gets or sets the primary departure mothership FlightGroup index</summary>
		public byte DepartureCraft1 { get; set; }
		/// <summary>Gets or sets the primary method of departure</summary>
		/// <remarks>When <b>true</b> Flightgroup will attempt to depart via mothership, hyperspace when <b>false</b></remarks>
		public bool DepartureMethod1 { get; set; }
		/// <summary>Gets or sets the secondary departure mothership FlightGroup index</summary>
		public byte DepartureCraft2 { get; set; }
		/// <summary>Gets or sets the secondary method of departure</summary>
		/// <remarks>When <b>true</b> Flightgroup will attempt to depart via mothership, hyperspace when <b>false</b></remarks>
		public bool DepartureMethod2 { get; set; }
		#endregion
		
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
		
		/// <summary>Base class for FlightGroup waypoints</summary>
		[Serializable] public abstract class BaseWaypoint : Indexer<short>
		{
			/// <summary>Default constructor for derived classes</summary>
			protected BaseWaypoint() { /* do nothing */ }

			/// <summary>Default constructor for derived classes</summary>
			/// <param name="raw">Raw data</param>
			protected BaseWaypoint(short[] raw) : base(raw) { /* do nothing */ }

			/// <summary>Returns a representative string of the Waypoint</summary>
			/// <returns>Waypoint coordinates in the form of <b>"(X, Y, Z)"</b>if enabled, otherwise <b>"Disabled"</b></returns>
			public override string ToString() { return (Enabled ? "(" + X + ", " + Y + ", " + Z + ") " : "Disabled"); }

			#region public properties
			/// <summary>Array form of the waypoint</summary>
			/// <remarks><see cref="Enabled"/> restricted to <b>0</b> and <b>1</b>, <see cref="Idmr.Platform.Xwa.FlightGroup.Waypoint.Region"/> restricted to <b>0-3</b>. No effect for invalid values</remarks>
			/// <param name="index">X, Y, Z, Enabled, Region (XWA only)</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
			public override short this[int index]
			{
				get { return _items[index]; }
				set
				{
					if (index == 3 && value != 0) _items[index] = 1;
					else if (Length == 5 && index == 4 && (value < 0 || value > 3)) return;
					else _items[index] = value;
				}
			}
			
			/// <summary>Gets or sets if the Waypoint is active for use</summary>
			public bool Enabled
			{
				get { return Convert.ToBoolean(_items[3]); }
				set { _items[3] = Convert.ToInt16(value); }
			}
			/// <summary>Gets or sets the stored X value</summary>
			public short RawX
			{
				get { return _items[0]; }
				set { _items[0] = value; }
			}
			/// <summary>Gets or sets the stored Y value</summary>
			public short RawY
			{
				get { return _items[1]; }
				set { _items[1] = value; }
			}
			/// <summary>Gets or sets the stored Z value</summary>
			public short RawZ
			{
				get { return _items[2]; }
				set { _items[2] = value; }
			}
			/// <summary>Gets or sets the X value in kilometers</summary>
			/// <remarks>Equals <see cref="RawX"/> divided by 160</remarks>
			public double X
			{
				get { return (double)_items[0] / 160; }
				set { _items[0] = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Y value in kilometers</summary>
			/// <remarks>Equals <see cref="RawY"/> divided by 160</remarks>
			public double Y
			{
				get { return (double)_items[1] / 160; }
				set { _items[1] = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Z value in kilometers</summary>
			/// <remarks>Equals <see cref="RawZ"/> divided by 160</remarks>
			public double Z
			{
				get { return (double)_items[2] / 160; }
				set { _items[2] = (short)(value * 160); }
			}
			#endregion public properties

			/// <summary>Converts a waypoint to a short array</summary>
			/// <remarks>Always returns Length 4 array, even for XWA, due to how values are stored in the file</remarks>
			/// <param name="wp">The waypoint to convert</param>
			/// <returns>An array of shorts containing <see cref="X"/>, <see cref="Y"/>, <see cref="Z"/> and <see cref="Enabled"/></returns>
			public static explicit operator short[](BaseWaypoint wp)
			{
				short[] s = new short[4];
				for (int i = 0; i < 4; i++) s[i] = wp[i];
				return s;
			}
		}
	}
}
