/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0+
 */

/* CHANGELOG
 * [NEW] Updates per XWA format spec
 * [UPD] Renamed ArrDep mothership properties
 * [NEW] Difficulties enum
 * v3.0, 180309
 * [NEW] EditorCraftNumber [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] BaseOrder/BaseWaypoint
 */

using System;
using Idmr.Common;

namespace Idmr.Platform
{
	/// <summary>Base class for FlightGroups.</summary>
	/// <remarks>Contains values that are common to all FlightGroup types. Class is Serializable for copy/paste functionality.</remarks>
	[Serializable]
	public abstract partial class BaseFlightGroup
	{
		/// <summary>Maximum string length for <see cref="Name"/>, <see cref="Cargo"/>, and <see cref="SpecialCargo"/>.</summary>
		/// <remarks>Defaults to 0x13, may be redefined by derivative classes.</remarks>
		private protected int _stringLength = 0x13;
		/// <summary>Displayed name of the FG.</summary>
		private protected string _name = "New Ship";
		/// <summary>Displayed cargo.</summary>
		private protected string _cargo = "";
		/// <summary>Displayed special cargo.</summary>
		private protected string _specialCargo = "";
		/// <summary>Special craft index.</summary>
		private protected byte _specialCargoCraft = 0;
		/// <summary>Whether or not the Special Craft is decided at random.</summary>
		private protected bool _randSpecCargo = false;
		/// <summary>Raw X rotation in degrees.</summary>
		private protected short _yaw = 0;
		/// <summary>Raw Y rotation in degrees.</summary>
		private protected short _pitch = 0;
		/// <summary>Raw Z rotation in degrees.</summary>
		private protected short _roll = 0;

		/// <summary>Values for <see cref="Difficulty"/>.</summary>
		public enum Difficulties
		{
			/// <summary>All difficulty settings.</summary>
			All,
			/// <summary>Easy settings.</summary>
			Easy,
			/// <summary>Normal settings.</summary>
			Medium,
			/// <summary>Hard settings.</summary>
			Hard,
			/// <summary>Normal or Hard settings.</summary>
			NotEasy,
			/// <summary>Easy or Normal settings.</summary>
			NotHard,
			/// <summary>Does not arrive.</summary>
			/// <remarks>Not originally a selectable value.</remarks>
			Never
		}

		/// <summary>Default constructor.</summary>
		protected BaseFlightGroup()
		{
			CraftType = 1;
			NumberOfCraft = 1;
			NumberOfWaves = 1;
			AI = 3;
			FormDistance = 2;
		}

		#region Craft
		/// <summary>Gets or sets the name of FlightGroup.</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE. Defaults to <b>"New Ship"</b>.</remarks>
		public string Name
		{
			get => _name;
			set
			{
				_name = StringFunctions.GetTrimmed(value, _stringLength);
				if (_name.IndexOf('\0') != -1) _name = _name.Substring(0, _name.IndexOf('\0')); // mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the default cargo assigned to entire FlightGroup.</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE.</remarks>
		public string Cargo
		{
			get => _cargo;
			set
			{
				_cargo = StringFunctions.GetTrimmed(value, _stringLength);
				if (_cargo.IndexOf('\0') != -1) _cargo = _cargo.Substring(0, _cargo.IndexOf('\0')); // mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the cargo string for Special Craft.</summary>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE.</remarks>
		public string SpecialCargo
		{
			get => _specialCargo;
			set
			{
				_specialCargo = StringFunctions.GetTrimmed(value, _stringLength);
				if (_specialCargo.IndexOf('\0') != -1) _specialCargo = _specialCargo.Substring(0, _specialCargo.IndexOf('\0')); // mitigates partial overwrites
			}
		}
		/// <summary>Gets or sets the Special Craft number.</summary>
		/// <remarks>One-indexed, zero is <b>"none"</b> (default). Raw Data is zero-indexed, "none" value is <see cref="NumberOfCraft"/>. Setting to a non-zero value sets <see cref="RandSpecCargo"/> to <b>false</b>.</remarks>
		public byte SpecialCargoCraft
		{
			get => _specialCargoCraft;
			set
			{
				_specialCargoCraft = value;
				if (_specialCargoCraft != 0) _randSpecCargo = false;
			}
		}
		/// <summary>Gets or sets whether or not a craft within the FlightGroup will be picked at random to use <see cref="SpecialCargo"/>.</summary>
		/// <remarks>Defaults to <b>false</b>. Setting to <b>true</b> sets <see cref="SpecialCargoCraft"/> to 0.</remarks>
		public bool RandSpecCargo
		{
			get => _randSpecCargo;
			set
			{
				_randSpecCargo = value;
				if (_randSpecCargo) _specialCargoCraft = 0;
			}
		}
		/// <summary>Gets or sets the ship or object type.</summary>
		/// <remarks>Defaults to <b>X-wing (1)</b>.</remarks>
		public byte CraftType { get; set; }
		/// <summary>Gets or sets the number of craft per wave.</summary>
		/// <remarks>One-indexed, default is <b>1</b>.</remarks>
		public byte NumberOfCraft { get; set; }
		/// <summary>Gets or sets the condition of the FlightGroup upon creation.</summary>
		public byte Status1 { get; set; }
		/// <summary>Gets or sets the warhead available to the FlightGroup.</summary>
		public byte Missile { get; set; }
		/// <summary>Gets or sets the Beam weapon used in the FlightGroup.</summary>
		public byte Beam { get; set; }
		/// <summary>Gets or sets the Friend or Foe setting.</summary>
		/// <remarks>Determines color of FlightGroup name and allegiance.</remarks>
		public byte IFF { get; set; }
		/// <summary>Gets or sets AI setting for NPC craft.</summary>
		/// <remarks>Default index is <b>3</b>.</remarks>
		public byte AI { get; set; }
		/// <summary>Gets or sets the FlightGroup colors if applicable.</summary>
		public byte Markings { get; set; }
		/// <summary>Gets or sets the FlightGroup formation.</summary>
		/// <remarks>Default is <b>Vic (0)</b>.</remarks>
		public byte Formation { get; set; }
		/// <summary>Gets or sets the FlightGroup spacing while in formation.</summary>
		/// <remarks>Default value is <b>2</b>.</remarks>
		public byte FormDistance { get; set; }
		/// <summary>Gets or sets the grouping assignment.</summary>
		/// <remarks>Used to group multiple FlightGroups, used for triggers.</remarks>
		public byte GlobalGroup { get; set; }
		/// <summary>Gets or sets the number of times the FlightGroup will arrive.</summary>
		/// <remarks>One-indexed.</remarks>
		public byte NumberOfWaves { get; set; }
		/// <summary>Gets or sets the delay between wave arrivals.</summary>
		/// <remarks>Does not work in TIE.</remarks>
		public byte WavesDelay { get; set; }
		/// <summary>Gets or sets the craft index the Player occupies.</summary>
		/// <remarks>One-indexed, <b>0</b> for AI-controlled.</remarks>
		public byte PlayerCraft { get; set; }
		/// <summary>Gets or sets the craft z-axis orientation at start in degrees, -180 to 179.</summary>
		/// <remarks>Calculated before <see cref="Pitch"/> and <see cref="Roll"/>, in TIE only applies to SpaceObjects.</remarks>
		public short Yaw
		{
			get => _yaw;
			set { if (value >= -180 && value < 180) _yaw = value; }
		}
		/// <summary>Gets or sets the craft y-axis orientation at start in degrees, -180 to 179.</summary>
		/// <remarks>Calculated after <see cref="Yaw"/> and before <see cref="Roll"/>, in TIE only applies to SpaceObjects.<br/>
		/// Flight engine adds -90 (nose down) pitch angle, automatically adjusted during Load/Save.</remarks>
		public short Pitch
		{
			get => _pitch;
			set { if (value >= -180 && value < 180) _pitch = value; }
		}
		/// <summary>Gets or sets the craft x-axis orientation at start in degrees, -180 to 179.</summary>
		/// <remarks>Calculated after <see cref="Yaw"/> and <see cref="Pitch"/>, in TIE only applies to SpaceObjects.</remarks>
		public short Roll
		{
			get => _roll;
			set { if (value >= -180 && value < 180) _roll = value; }
		}
		#endregion
		#region ArrDep
		/// <summary>Gets or sets the mission difficulty setting required for the FlightGroup to arrive.</summary>
		/// <remarks>Defaults to All.</remarks>
		public Difficulties Difficulty { get; set; }
		/// <summary>Gets or sets the delay in minutes after the Arrival Trigger conditions have been met.</summary>
		/// <remarks>Can be used in conjunction with <see cref="ArrivalDelaySeconds"/>.</remarks>
		public byte ArrivalDelayMinutes { get; set; }
		/// <summary>Gets or sets the delay in seconds after the Arrival Trigger conditions have been met.</summary>
		/// <remarks>Can be used in conjunction with <see cref="ArrivalDelayMinutes"/>.</remarks>
		public byte ArrivalDelaySeconds { get; set; }
		/// <summary>Gets or sets the delay in minutes after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE.</summary>
		/// <remarks>Can be used in conjunction with <see cref="DepartureTimerSeconds"/>.</remarks>
		public byte DepartureTimerMinutes { get; set; }
		/// <summary>Gets or sets the delay in seconds after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE.</summary>
		/// <remarks>Can be used in conjunction with <see cref="DepartureTimerMinutes"/>.</remarks>
		public byte DepartureTimerSeconds { get; set; }
		/// <summary>Gets or sets the individual craft departure requirement.</summary>
		public byte AbortTrigger { get; set; }
		/// <summary>Gets or sets the primary arrival mothership FlightGroup index.</summary>
		public byte ArrivalMothership { get; set; }
		/// <summary>Gets or sets the primary method of arrival.</summary>
		/// <remarks>When <b>true</b> FlightGroup will attempt arrive via mothership, hyperspace when <b>false</b>.</remarks>
		public bool ArriveViaMothership { get; set; }
		/// <summary>Gets or sets the backup arrival/departure mothership FlightGroup index.</summary>
		public byte AlternateMothership { get; set; }
		/// <summary>Gets or sets the backup method of arrival/departure.</summary>
		/// <remarks>When <b>true</b> FlightGroup will attempt via mothership, hyperspace when <b>false</b>.</remarks>
		public bool AlternateMothershipUsed { get; set; }
		/// <summary>Gets or sets the primary departure mothership FlightGroup index.</summary>
		public byte DepartureMothership { get; set; }
		/// <summary>Gets or sets the primary method of departure.</summary>
		/// <remarks>When <b>true</b> Flightgroup will attempt to depart via mothership, hyperspace when <b>false</b>.</remarks>
		public bool DepartViaMothership { get; set; }
		/// <summary>Gets or sets the departure mothership FlightGroup index after capture.</summary>
		public byte CapturedDepartureMothership { get; set; }
		/// <summary>Gets or sets the method of departure after capture.</summary>
		/// <remarks>When <b>true</b> Flightgroup will attempt to depart via mothership, hyperspace when <b>false</b>.</remarks>
		public bool CapturedDepartViaMothership { get; set; }
		#endregion

		/// <summary>Gets or sets the descriptive craft number.</summary>
		/// <remarks>This is used to help tell multiple Flight Groups apart when they have multiple names.  It is specific to YOGEME and not part of the file format.</remarks>
		public int EditorCraftNumber { get; set; }
		/// <summary>Gets or sets whether the actual numbering (including craft per wave) is listed or just duplicate names.</summary>
		/// <remarks>This will be TRUE for XvT and XWA style Global Unit numbering, FALSE for TIE and X-wing or if XvT/XWA's GU numbering is disabled.  It is specific to YOGEME and not part of the file format.</remarks>
		public bool EditorCraftExplicit { get; set; }
	}
}