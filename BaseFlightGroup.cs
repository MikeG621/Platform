/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using Idmr.Common;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for FlightGroups</summary>
	/// <remarks>Contains values that are common to all FlightGroup types. Class is Serializable for copy/paste functionality</remarks>
	public abstract class BaseFlightGroup
	{
		/// <summary>Maximum string length for <i>Name</i>, <i>Cargo</i> and <i>SpecialCargo</i></summary>
		/// <remarks>Defaults to 0x13, may be redefined by derivative classes</remarks>
		protected int _stringLength = 0x13;
		protected string _name = "New Ship";
		protected string _cargo = "";
		protected string _specialCargo = "";
		protected byte _specialCargoCraft = 0;
		protected bool _randSpecCargo = false;
		protected short _yaw = 0;
		protected short _pitch = 0;
		protected short _roll = 0;

		#region Craft
		/// <summary>Gets or sets the name of FlightGroup</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE. Defaults to "New Ship"</remarks>
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
		/// <remarks>One-indexed, zero is "none" (default). Raw Data is zero-indexed, "none" value is <i>NumberOfCraft</i>. Setting to a non-zero value sets <i>RandSpecCargo</i> to <i>false</i></remarks>
		public byte SpecialCargoCraft
		{
			get { return _specialCargoCraft; }
			set 
			{
				_specialCargoCraft = value;
				if (_specialCargoCraft != 0) _randSpecCargo = false;
			}
		}
		/// <summary>Gets or sets whether or not a craft within the FlightGroup will be picked at random to use SpecialCargo</summary>
		/// <remarks>Defaults to <i>false</i>. Setting to <i>true</i> sets <i>SpecialCargoCraft</i> to 0</remarks>
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
		/// <remarks>Defaults to X-wing (1)</remarks>
		public byte CraftType = 1;
		/// <summary>Gets or sets the number of craft per wave</summary>
		/// <remarks>One-indexed, default is 1</remarks>
		public byte NumberOfCraft = 1;
		/// <summary>Gets or sets the condition of the FlightGroup upon creation</summary>
		public byte Status1 = 0;
		/// <summary>Gets or sets the warhead available to the FlightGroup</summary>
		public byte Missile = 0;
		/// <summary>Gets or sets the Beam weapon used in the FlightGroup</summary>
		public byte Beam = 0;
		/// <summary>Gets or sets the Friend or Foe setting</summary>
		/// <remarks>Determines color of FlightGroup name and allegiance</remarks>
		public byte IFF = 0;
		/// <summary>Gets or sets AI setting for NPC craft</summary>
		/// <remarks>Default index is 3</remarks>
		public byte AI = 3;
		/// <summary>Gets or sets the FlightGroup colors if applicable</summary>
		public byte Markings = 0;
		/// <summary>Gets or sets the FlightGroup formation</summary>
		/// <remarks>Default is Vic (0)</remarks>
		public byte Formation = 0;
		/// <summary>Gets or sets the FlightGroup spacing while in formation</summary>
		/// <remarks>Default value is 2</remarks>
		public byte FormDistance = 2;
		/// <summary>Gets or sets the grouping assignment</summary>
		/// <remarks>Used to group multiple FlightGroups, used for triggers</remarks>
		public byte GlobalGroup = 0;
		/// <summary>Gets or sets how far in front of the FlightGroup the Leader is</summary>
		public byte FormLeaderDist = 0;
		/// <summary>Gets or sets the number of times the FlightGroup will arrive</summary>
		/// <remarks>Really zero-indexed, converted to one-indexed for ease of use</remarks>
		public byte NumberOfWaves = 1;
		/// <summary>Gets or sets the craft index the Player occupies</summary>
		/// <remarks>One-indexed, 0 for AI-controlled</remarks>
		public byte PlayerCraft = 0;
		/// <summary>Gets or sets the craft z-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated before Pitch and Roll, in TIE only applies to SpaceObjects</remarks>
		public short Yaw
		{
			get { return _yaw; }
			set { if (value >= -180 && value < 180) _yaw = value; }
		}
		/// <summary>Gets or sets the craft y-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated after Yaw and before Roll, in TIE only applies to SpaceObjects.
		/// Flight engine adds -90 (nose down) pitch angle, automatically adjusted during Load/Save</remarks>
		public short Pitch
		{
			get { return _pitch; }
			set { if (value >= -180 && value < 180) _pitch = value; }
		}
		/// <summary>Gets or sets the craft x-axis orientation at start in degrees, -180 to 179</summary>
		/// <remarks>Calculated after Yaw and Pitch, in TIE only applies to SpaceObjects</remarks>
		public short Roll
		{
			get { return _roll; }
			set { if (value >= -180 && value < 180) _roll = value; }
		}
		#endregion
		#region ArrDep
		/// <summary>Gets or sets the mission difficulty setting required for the FlightGroup to arrive</summary>
		/// <remarks>Defaults to All (0)</remarks>
		public byte Difficulty = 0;
		/// <summary>Gets or sets the delay in minutes after the Arrival Trigger conditions have been met</summary>
		/// <remarks>Can be used in conjunction with <i>ArrivalDelaySeconds</i></remarks>
		public byte ArrivalDelayMinutes = 0;
		/// <summary>Gets or sets the delay in seconds after the Arrival Trigger conditions have been met</summary>
		/// <remarks>Can be used in conjunction with <i>ArrivalDelayMinutes</i></remarks>
		public byte ArrivalDelaySeconds = 0;
		/// <summary>Gets or sets the delay in minutes after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</summary>
		/// <remarks>Can be used in conjunction with <i>DepartureDelaySeconds</i></remarks>
		public byte DepartureTimerMinutes = 0;
		/// <summary>Gets or sets the delay in seconds after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</summary>
		/// <remarks>Can be used in conjunction with <i>DepartureDelayMinutes</i></remarks>
		public byte DepartureTimerSeconds = 0;
		/// <summary>Gets or sets the individual craft departure requirement</summary>
		public byte AbortTrigger = 0;
		/// <summary>Gets or sets the primary arrival mothership FlightGroup index</summary>
		/// <remarks>Zero-indexed</remarks>
		public byte ArrivalCraft1 = 0;
		/// <summary>Gets or sets the primary method of arrival</summary>
		/// <remarks>When <i>true</i> FlightGroup will attempt arrive via mothership, hyperspace when <i>false</i></remarks>
		public bool ArrivalMethod1 = false;
		/// <summary>Gets or sets the secondary arrival mothership FlightGroup index</summary>
		/// <remarks>Zero-indexed</remarks>
		public byte ArrivalCraft2 = 0;
		/// <summary>Gets or sets the secondary method of arrival</summary>
		/// <remarks>When <i>true</i> FlightGroup will attempt arrive via mothership, hyperspace when <i>false</i></remarks>
		public bool ArrivalMethod2 = false;
		/// <summary>Gets or sets the primary departure mothership FlightGroup index</summary>
		/// <remarks>Zero-indexed</remarks>
		public byte DepartureCraft1 = 0;
		/// <summary>Gets or sets the primary method of departure</summary>
		/// <remarks>When <i>true</i> Flightgroup will attempt to depart via mothership, hyperspace when <i>false</i></remarks>
		public bool DepartureMethod1 = false;
		/// <summary>Gets or sets the secondary departure mothership FlightGroup index</summary>
		/// <remarks>Zero-indexed</remarks>
		public byte DepartureCraft2 = 0;
		/// <summary>Gets or sets the secondary method of departure</summary>
		/// <remarks>When <i>true</i> Flightgroup will attempt to depart via mothership, hyperspace when <i>false</i></remarks>
		public bool DepartureMethod2 = false;
		#endregion
	}
}
