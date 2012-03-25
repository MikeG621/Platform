/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 120207 - added Waypoint conversions
 * 120208 - added Order conversions
 * 120210 - rewrote Waypoint
 * 120213 - rewrote for IOrder removal
 * 120221 - Indexer<T> implementation
 * 120227 - ToString() overrides
 * 120321 - Goal and Order exceptions
 * *** v2.0 ***
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable]
	public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[6];
		string _role = "";
		bool[] _arrDepAndOr = new bool[4];
		Order[,] _orders = new Order[4,4];
		Goal[] _goals = new Goal[8];
		bool[] _optLoadout = new bool[15];
		OptionalCraft[] _optCraft = new OptionalCraft[10];
		string _pilotID = "";
		Waypoint[] _waypoints = new Waypoint[4];
		LoadoutIndexer _loadoutIndexer;

		/// <summary>Indexes for <see cref="ArrDepTriggers"/></summary>
		public enum ArrDepTriggerIndex : byte
		{
			/// <summary>First Arrival trigger</summary>
			Arrival1,
			/// <summary>Second Arrival trigger</summary>
			Arrival2,
			/// <summary>Third Arrival trigger</summary>
			Arrival3,
			/// <summary>Fourth Arrival trigger</summary>
			Arrival4,
			/// <summary>First Departure trigger</summary>
			Departure1,
			/// <summary>Second Departure trigger</summary>
			Departure2
		}
		/// <summary>Values for <see cref="OptCraftCategory"/></summary>
		public enum OptionalCraftCategory : byte
		{
			/// <summary>No other available craft</summary>
			None,
			/// <summary>All craft set to 'Flyable' are available</summary>
			AllFlyable,
			/// <summary>All Rebel craft set to 'Flyable' are available</summary>
			AllRebelFlyable,
			/// <summary>All Imperial craft set tp 'Flyable' are available</summary>
			AllImperialFlyable,
			/// <summary>Available craft are determined by <see cref="OptCraft"/></summary>
			Custom
		}

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All <see cref="Orders"/> set to <b>100%</b> <see cref="BaseFlightGroup.BaseOrder.Throttle"/> (16 total, 4 per Region), <see cref="Goals"/> set to <b>NONE</b>, SP1 <b>Enabled</b>, most <see cref="Unknowns"/> <b>0/false</b>, <see cref="UnknownValues.Unknown1"/> to <b>2</b></remarks>
		public FlightGroup()
		{
			for (int i = 0; i < 6; i++) _arrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < 4; i++) _waypoints[i] = new Waypoint();
			for (int i = 0; i < 16; i++) _orders[i / 4, i % 4] = new Order();
			for (int i = 0; i < 8; i++) _goals[i] = new Goal();
			_optLoadout[0] = true;
			_optLoadout[8] = true;
			_optLoadout[12] = true;
			_waypoints[0].Enabled = true;
			Unknowns.Unknown1 = 2;
			_loadoutIndexer = new LoadoutIndexer(_optLoadout);
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Short form is <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b><br/>
		/// Long form is <b>"<see cref="Team"/> - <see cref="BaseFlightGroup.GlobalGroup"/> -
		/// IsPlayer <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/>.
		/// <see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/> (<see cref="GlobalUnit"/>)"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
			if (!verbose) return longName;
			return (Team+1) + " - " + GlobalGroup + " - " + (PlayerNumber != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName + (GlobalUnit != 0 ? " (" + GlobalUnit + ")" : "");
		}
		
		#region craft
		/// <summary>Determines if <see cref="Designation1"/> is used</summary>
		public bool EnableDesignation1 { get; set; }
		/// <summary>Determines if <see cref="Designation2"/> is used</summary>
		public bool EnableDesignation2 { get; set; }
		/// <summary>Primary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation1 { get; set; }
		/// <summary>Secondary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation2 { get; set; }
		/// <summary>The <see cref="Mission.GlobalCargo"/> index to be used instead of <see cref="BaseFlightGroup.Cargo"/></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalCargo { get; set; }
		/// <summary>The <see cref="Mission.GlobalCargo"/> index to be used instead of <see cref="BaseFlightGroup.SpecialCargo"/></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalSpecialCargo { get; set; }
		/// <summary>Gets or sets a text form of the craft role</summary>
		/// <remarks>19 character limit, appears to be an editor note</remarks>
		public string Role
		{
			get { return _role; }
			set { _role = StringFunctions.GetTrimmed(value, _stringLength); }
		}
		/// <summary>Gets or sets the allegiance value that controls goals and IFF behaviour</summary>
		public byte Team { get; set; }
		/// <summary>Gets or sets team or player number to which the craft communicates with</summary>
		public byte Radio { get; set; }
		/// <summary>Gets or sets if the craft has a human or AI pilot</summary>
		/// <remarks>Value of <b>zero</b> defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present</remarks>
		public byte PlayerNumber { get; set; }
		/// <summary>Gets or sets if craft is required to be player-controlled</summary>
		/// <remarks>When <b>true</b>, craft with PlayerNumber set will not appear without a human player</remarks>
		public bool ArriveOnlyIfHuman { get; set; }
		#endregion
		#region arrdep
		/// <summary>Returns <b>true</b> if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds { get { return (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && _arrDepTriggers[2].Condition == 0 && _arrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets which <see cref="ArrDepTriggers"/> must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}</remarks>
		public bool[] ArrDepAndOr { get { return _arrDepAndOr; } }
		#endregion
		/// <summary>Gets the FlightGroup objective commands</summary>
		/// <remarks>Array is [Region, OrderIndex]</remarks>
		public Order[,] Orders { get { return _orders; } }
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 8</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Gets the FlightGroup start location markers</summary>
		/// <remarks>Array is length 4</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }
		#region Unks and Option
		/// <summary>Determines if the FlightGroup should share numbering across the <see cref="GlobalUnit"/></summary>
		public bool GlobalNumbering { get; set; }
		/// <summary>Gets or sets the defenses available to the FG</summary>
		public byte Countermeasures { get; set; }
		/// <summary>Gets or sets the duration of death animation</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime { get; set; }
		/// <summary>Gets or sets the second condition of FlightGroup upon creation</summary>
		public byte Status2 { get; set; }
		/// <summary>Gets or sets the additional grouping assignment, can share craft numbering</summary>
		public byte GlobalUnit { get; set; }
		/// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use the <see cref="LoadoutIndexer.Indexes"/> enumeration for indexes</remarks>
		public LoadoutIndexer OptLoadout { get { return _loadoutIndexer; } }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get { return _optCraft; } }
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory = OptionalCraftCategory.None;

		/// <summary>Gets or sets the editor note primarily used to signifiy the pilot voice</summary>
		/// <remarks>15 character limit</remarks>
		public string PilotID
		{
			get { return _pilotID; }
			set { _pilotID = StringFunctions.GetTrimmed(value, 15); }
		}
		/// <summary>For Backdrop <see cref="BaseFlightGroup.CraftType">CraftTypes</see>, is the backdrop image</summary>
		/// <remarks>Zero denotes a light source</remarks>
		public byte Backdrop { get; set; }
		/// <summary>The unknown values container</summary>
		/// <remarks>All values except <see cref="UnknownValues.Unknown1"/> initialize to <b>0</b> or <b>false</b>, Unknown1 initializes to <b>2</b>. <see cref="Orders"/> contain Unknown9-14, <see cref="Goals"/> contain Unknown15</remarks>
		public UnknownValues Unknowns;
		#endregion
		
		/// <summary>Container for the optional craft settings</summary>
		[Serializable] public struct OptionalCraft
		{
			/// <summary>The Craft Type</summary>
			public byte CraftType { get; set; }
			/// <summary>The number of ships per Wave</summary>
			public byte NumberOfCraft { get; set; }
			/// <summary>The number of waves available</summary>
			public byte NumberOfWaves { get; set; }
		}
		
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0018, in Craft section</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x007B, in Craft section</remarks>
			public byte Unknown3 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0084, in Craft section</remarks>
			public byte Unknown4 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0087, in Arr/Dep section</remarks>
			public byte Unknown5 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0097, in Arr/Dep section</remarks>
			public bool Unknown6 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00BF, in Arr/Dep section</remarks>
			public byte Unknown7 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00C0, in Arr/Dep section</remarks>
			public byte Unknown8 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAE, in Unks/Options section</remarks>
			public byte Unknown16 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAF, in Unks/Options section</remarks>
			public byte Unknown17 { get; set; }
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB0, in Unks/Options section</remarks>
			public byte Unknown18 { get; set; }
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB1, in Unks/Options section</remarks>
			public byte Unknown19 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB2, in Unks/Options section</remarks>
			public byte Unknown20 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB3, in Unks/Options section</remarks>
			public byte Unknown21 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB4, in Unks/Options section</remarks>
			public bool Unknown22 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB6, in Unks/Options section</remarks>
			public byte Unknown23 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB7, in Unks/Options section</remarks>
			public byte Unknown24 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB8, in Unks/Options section</remarks>
			public byte Unknown25 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB9, in Unks/Options section</remarks>
			public byte Unknown26 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBA, in Unks/Options section</remarks>
			public byte Unknown27 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBB, in Unks/Options section</remarks>
			public byte Unknown28 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBC, in Unks/Options section</remarks>
			public bool Unknown29 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC0, in Unks/Options section</remarks>
			public bool Unknown30 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC1, in Unks/Options section</remarks>
			public bool Unknown31 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC5, in Unks/Options section</remarks>
			public byte Unknown32 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC6, in Unks/Options section</remarks>
			public byte Unknown33 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E29, in Unks/Options section</remarks>
			public bool Unknown34 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2B, in Unks/Options section</remarks>
			public bool Unknown35 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2D, in Unks/Options section</remarks>
			public bool Unknown36 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2F, in Unks/Options section</remarks>
			public bool Unknown37 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E31, in Unks/Options section</remarks>
			public bool Unknown38 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E33, in Unks/Options section</remarks>
			public bool Unknown39 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E35, in Unks/Options section</remarks>
			public bool Unknown40 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E37, in Unks/Options section</remarks>
			public bool Unknown41 { get; set; }
		}
		
		/// <summary>Object to provide array access to the Optional Craft Loadout values</summary>
		[Serializable] public class LoadoutIndexer : Indexer<bool>
		{
			/// <summary>Indexes for <see cref="this[Indexes]"/></summary>
			public enum Indexes : byte
			{
				/// <summary>All warheads unavailable</summary>
				NoWarheads,
				/// <summary>Space Bomb warheads</summary>
				SpaceBomb,
				/// <summary>Heavy Rocket warhesds</summary>
				HeavyRocket,
				/// <summary>Missile warheads</summary>
				Missile,
				/// <summary>Torpedo warheads</summary>
				Torpedo,
				/// <summary>Advanced Missile warheads</summary>
				AdvMissile,
				/// <summary>Advanced Torpedo warheads</summary>
				AdvTorpedo,
				/// <summary>MagPulse distruption warheads</summary>
				MagPulse,
				/// <summary>All beams unavailable</summary>
				NoBeam,
				/// <summary>Tractor beam</summary>
				TractorBeam,
				/// <summary>Targeting Jamming beam</summary>
				JammingBeam,
				/// <summary>Decoy beam</summary>
				DecoyBeam,
				/// <summary>All countermeasures unavailable</summary>
				NoCountermeasures,
				/// <summary>Chaff countermeasures</summary>
				Chaff,
				/// <summary>Flare countermeasures</summary>
				Flare
			}			
			/// <summary>Initializes the indexer</summary>
			/// <param name="optLoadout">The loadout array</param>
			public LoadoutIndexer(bool[] optLoadout) : base(optLoadout) { }
			
			/// <summary>Gets or sets the Role values</summary>
			/// <param name="index">Indexes enumerated value, 0-15</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br/>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br/>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br/>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
			public override bool this[int index]
			{
				get { return _items[index]; }
				set
				{
					if ((index == 0 || index == 8 || index == 12) && !value) return;
					_items[index] = value;
					if (index == 0 && value) for (int i = 1; i < 8; i++) _items[i] = false;	// set NoWarheads, clear warheads
					else if (index == 8 && value) for (int i = 9; i < 12; i++) _items[i] = false;	// set NoBeam, clear beams
					else if (index == 12 && value) for (int i = 13; i < 15; i++) _items[i] = false;	// set NoCMs, clear CMs
					else if (index < 8 && value) _items[0] = false;	// set a warhead, clear NoWarheads
					else if (index < 12 && value) _items[8] = false;	// set a beam, clear NoBeam
					else if (index < 15 && value) _items[12] = false;	// set a CM, clear NoCMs
					else if (index < 8)
					{
						bool used = false;
						for (int i = 1; i < 8; i++) used |= _items[i];
						if (!used) _items[0] = true;	// cleared last warhead, set NoWarheads
					}
					else if (index < 12)
					{
						bool used = false;
						for (int i = 9; i < 12; i++) used |= _items[i];
						if (!used) _items[8] = true;	// cleared last beam, set NoBeam
					}
					else
					{
						bool used = false;
						for (int i = 13; i < 15; i++) used |= _items[i];
						if (!used) _items[12] = true;	// cleared last CM, set NoCMs
					}
				}
			}
			
			/// <summary>Gets or sets the Role values</summary>
			/// <param name="index">Indexes enumerated value</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br/>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br/>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br/>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
			public bool this[Indexes index]
			{
				get { return _items[(int)index]; }
				set { this[(int)index] = value; }	// make the other indexer do the work
			}
		}
		
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
			/// <returns>Waypoint coordinates in the form of <b>"(<see cref="X"/>, <see cref="Y"/>, <see cref="Z"/>) <see cref="Region"/> #"</b>if enabled, otherwise <b>"Disabled"</b></returns>
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
		
		/// <summary>Object for a single FlightGroup-specific Goal</summary>
		[Serializable] public class Goal : Indexer<byte>
		{
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks><see cref="Condition"/> is set to <b>"never (FALSE)"</b></remarks>
			public Goal()
			{
				_items = new byte[16];
				_items[1] = 10;
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 16</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length < 16) throw new ArgumentException("Minimum length of raw is 16", "raw");
				_items = new byte[16];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 16</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Goal(byte[] raw, int startIndex)
			{
				if (raw.Length < 16) throw new ArgumentException("Minimum length of raw is 16", "raw");
				if (raw.Length - startIndex < 16 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 16));
				_items = new byte[16];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
			}

			/// <summary>Gets a representative string of the Goal</summary>
			/// <returns>Description of the goal if enabled, otherwise <b>"None"</b></returns>
			public override string ToString()
			{
				string goal = "";
				if (Condition != 0 && Condition != 10 && Enabled)
				{
					goal = Strings.Amount[Amount] + " of Flight Group ";
					goal += (Argument == 0 ? "must" : (Argument == 1 ? "must NOT" : (Argument == 2 ? "BONUS must" : "BONUS must NOT")));
					goal += " " + Strings.Trigger[Condition];
					if (Condition == 0x2F || Condition == 0x30) goal += " " + Parameter;
					goal += " (" + Points + " points)";
				}
				else goal = "None";
				return goal;
			}

			#region public properties
			/// <summary>Gets or sets the goal behaviour</summary>
			/// <remarks>Values are <b>0-3</b>; must, must not (Prevent), BONUS must, BONUS must not (bonus prevent)</remarks>
			public byte Argument
			{
				get { return _items[0]; }
				set { _items[0] = value; }
			}
			/// <summary>Gets or sets the Goal trigger</summary>
			public byte Condition
			{
				get { return _items[1]; }
				set { _items[1] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="Condition"/></summary>
			public byte Amount
			{
				get { return _items[2]; }
				set { _items[2] = value; }
			}
			/// <summary>Gets or sets the points value stored in the file</summary>
			public sbyte RawPoints
			{
				get { return (sbyte)_items[3]; }
				set { _items[3] = (byte)value; }
			}
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equals <see cref="RawPoints"/> * 25, limited from <b>-3200</b> to <b>+3175</b></remarks>
			public short Points
			{
				get { return (short)(RawPoints * 25); }
				set { RawPoints = (sbyte)((value > 3175 ? 3175 : (value < -3200 ? -3200 : value)) / 25); }
			}
			/// <summary>Gets or sets if the Goal is active</summary>
			public bool Enabled
			{
				get { return Convert.ToBoolean(_items[4]); }
				set { _items[4] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets which Team the Goal applies to</summary>
			public byte Team
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Gets or sets the additional Goal setting</summary>
			public byte Parameter
			{
				get { return _items[14]; }
				set { _items[14] = value; }
			}
			/// <summary>Gets or sets the location within the Active Sequence</summary>
			public byte ActiveSequence
			{
				get { return _items[15]; }
				set { _items[15] = value; }
			}
			
			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Bonus goals</remarks>
			public string IncompleteText
			{
				get { return _incompleteText; }
				set { _incompleteText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after completion</summary>
			/// <remarks>String is limited to 63 char</remarks>
			public string CompleteText
			{
				get { return _completeText; }
				set { _completeText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after failure</summary>
			/// <remarks>String is limited to 63 char. Not used for Bonus or Prevent goals</remarks>
			public string FailedText
			{
				get { return _failedText; }
				set { _failedText = StringFunctions.GetTrimmed(value, 63); }
			}

			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x4F</remarks>
			public bool Unknown15 { get; set; }
			#endregion public properties
		}
		
		/// <summary>Container for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _customText = "";
			Waypoint[] _waypoints = new Waypoint[8];
			Mission.Trigger[] _skipTriggers = new Mission.Trigger[2];
			
			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b>, <see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b></remarks>
			public Order()
			{
				_items = new byte[19];
				_items[1] = 10;	// Throttle
				_items[10] = _items[16] = 1;	// AndOrs
				initialize();
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b><br/>
			/// If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19) ArrayFunctions.TrimArray(raw, 0, _items);
				else ArrayFunctions.WriteToArray(raw, _items, 0);
				initialize();
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b><br/>
			/// If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Order(byte[] raw, int startIndex)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19)
				{
					if (raw.Length - startIndex < 19 || startIndex < 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 19));
					ArrayFunctions.TrimArray(raw, startIndex, _items);
				}
				else
				{
					if (startIndex != 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0");
					ArrayFunctions.WriteToArray(raw, _items, 0);
				}
				initialize();
			}
			
			void initialize()
			{
				for (int i = 0; i < 8; i++) _waypoints[i] = new Waypoint();
				_skipTriggers[0] = new Mission.Trigger();
				_skipTriggers[1] = new Mission.Trigger();
				_skipTriggers[0].Condition = 10;
				_skipTriggers[1].Condition = 10;
				SkipT1AndOrT2 = true;
			}
			#endregion constructors

			static string _orderTargetString(byte target, byte type)
			{
				string s = "";
				switch (type)
				{
					case 0:
						break;
					case 1:
						s += "FG:" + target;
						break;
					case 2:
						s += Strings.CraftType[target + 1] + "s";
						break;
					case 3:
						s += Strings.ShipClass[target];
						break;
					case 4:
						s += Strings.ObjectType[target];
						break;
					case 5:
						s += Strings.IFF[target] + "s";
						break;
					case 6:
						s += "Craft with " + Strings.Orders[target] + " orders";
						break;
					case 7:
						s += "Craft when " + Strings.CraftWhen[target];
						break;
					case 8:
						s += "Global Group " + target;
						break;
					case 0xC:
						s += "TM:" + target;
						break;
					case 0xD:
						s += "Player of GG " + target;
						break;
					case 0x15:
						s += "All Teams except TM:" + target;
						break;
					case 0x17:
						s += "Global Unit " + target;
						break;
					case 0x19:
						s += "Global Cargo " + target;
						break;
					case 0x1B:
						s += "Message #" + target;
						break;
					default:
						s += type + " " + target;
						break;
				}
				return s;
			}

			/// <summary>Returns a representative string of the Order</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> and Teams are identified as <b>"TM:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the order and targets if applicable, otherwise <b>"None"</b></returns>
			public override string ToString()
			{
				if (Command == 0) return "None";
				string order = Strings.Orders[Command];
				if ((Command >= 7 && Command <= 0x12) || (Command >= 0x17 && Command <= 0x1B) || Command == 0x1F || Command == 0x20 || Command == 0x25)	//all orders where targets are important
				{
					string s = _orderTargetString(Target1, Target1Type);
					string s2 = _orderTargetString(Target2, Target2Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T1AndOrT2) order += " or " + s2;
						else order += " if " + s2;
					}
					s = _orderTargetString(Target3, Target3Type);
					s2 = _orderTargetString(Target4, Target4Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T3AndOrT4) order += " or " + s2;
						else order += " if " + s2;
					}
				}
				return order;
			}

			/// <summary>Converts an order to a byte array</summary>
			/// <remarks><see cref="CustomText"/>, <see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A byte array of Length 19 containing the Order data</returns>
			public static explicit operator byte[](Order ord)
			{
				// CustomText, WPs, Skips lost
				byte[] b = new byte[19];
				for (int i = 0; i < 19; i++) b[i] = ord[i];
				return b;
			}
			/// <summary>Converts an Order for TIE</summary>
			/// <remarks><see cref="Speed"/>, <see cref="CustomText"/>, <see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order ord)
			{
				// Speed, CustomText, WPs, Skips lost
				return new Tie.FlightGroup.Order((byte[])ord, 0);
			}
			/// <summary>Converts an Order for XvT</summary>
			/// <remarks><see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion. <see cref="CustomText"/> trimmed to 16 characters</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for use in XvT</returns>
			public static explicit operator Xvt.FlightGroup.Order(Order ord)
			{
				// WPs, Skips lost
				// CustomText trimmed to 16 char
				Xvt.FlightGroup.Order newOrder = new Xvt.FlightGroup.Order((byte[])ord);
				newOrder.Designation = ord.CustomText;
				return newOrder;
			}
			
			#region public properties
			/// <summary>Gets or sets the third order-specific setting</summary>
			public byte Variable3
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown9
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _items[18]; }
				set { _items[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 63 characters</remarks>
			public string CustomText
			{
				get { return _customText; }
				set { _customText = StringFunctions.GetTrimmed(value, 63); }
			}

			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x72</remarks>
			public byte Unknown10 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x73</remarks>
			public bool Unknown11 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x74</remarks>
			public bool Unknown12 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x7B</remarks>
			public bool Unknown13 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x81</remarks>
			public bool Unknown14 { get; set; }
			/// <summary>Whether or not the Skip Triggers are exclusive</summary>
			/// <remarks>Default is <b>"Or" (true)</b> due to default "never (FALSE)" Trigger</remarks>
			public bool SkipT1AndOrT2 { get; set; }

			/// <summary>Gets the order-specific location markers</summary>
			/// <remarks>Array is length 8</remarks>
			public Waypoint[] Waypoints { get { return _waypoints; } }
			
			/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to the order</summary>
			/// <remarks>Array is length 2</remarks>
			public Mission.Trigger[] SkipTriggers { get { return _skipTriggers; } }
			#endregion public properties
		}
	}
}
