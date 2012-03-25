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
 * 120213 - rewrote RolesIndexer to test array ref instead of class, rewrote for IOrder removal
 * 120220 - Indexer<T> implementation
 * 120227 - ToString() overrides
 * 120321 - Goal and Order exceptions
 * *** v2.0 ***
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable] public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[6];
		string[] _roles = new string[4];
		bool[] _arrDepAO = new bool[4];
		Order[] _orders = new Order[4];
		Mission.Trigger[] _skipToOrder4Trigger = new Mission.Trigger[2];
		Goal[] _goals = new Goal[8];
		bool[] _optLoad = new bool[15];
		OptionalCraft[] _optCraft = new OptionalCraft[10];
		Waypoint[] _waypoints = new Waypoint[22];
		Indexer<string> _rolesIndexer;
		LoadoutIndexer _loadoutIndexer;

		/// <summary>Indexes for <see cref="ArrDepTriggers"/></summary>
		public enum ArrDepTriggerIndex : byte {
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
		/// <summary>Indexes for <see cref="Waypoints"/></summary>
		public enum WaypointIndex : byte {
			/// <summary>Primary starting coordinate</summary>
			Start1,
			/// <summary>Optional starting coordinate</summary>
			Start2,
			/// <summary>Optional starting coordinate</summary>
			Start3,
			/// <summary>Optional starting coordinate</summary>
			Start4,
			/// <summary>First coordinate for orders and initial trajectory</summary>
			WP1,
			/// <summary>Second order coordinate</summary>
			WP2,
			/// <summary>Third order coordinate</summary>
			WP3,
			/// <summary>Fourth order coordinate</summary>
			WP4,
			/// <summary>Fifth order coordinate</summary>
			WP5,
			/// <summary>Sixth order coordinate</summary>
			WP6,
			/// <summary>Seventh order coordinate</summary>
			WP7,
			/// <summary>Eigth order coordinate</summary>
			WP8,
			/// <summary>Coordinate for Rendezvous orders</summary>
			Rendezvous,
			/// <summary>Arrival and Departure coordinate</summary>
			Hyperspace,
			/// <summary>Primary briefing coordinate</summary>
			Briefing1,
			/// <summary>Team 2 briefing coordinate</summary>
			Briefing2,
			/// <summary>Team 3 briefing coordinate</summary>
			Briefing3,
			/// <summary>Team 4 briefing coordinate</summary>
			Briefing4,
			/// <summary>Team 5 briefing coordinate</summary>
			Briefing5,
			/// <summary>Team 6 briefing coordinate</summary>
			Briefing6,
			/// <summary>Team 7 briefing coordinate</summary>
			Briefing7,
			/// <summary>Team 8 briefing coordinate</summary>
			Briefing8
		}
		/// <summary>Values for <see cref="OptCraftCategory"/></summary>
		public enum OptionalCraftCategory : byte {
			/// <summary>No other available craft</summary>
			None,
			/// <summary>All craft set to 'Flyable' are avai;able</summary>
			AllFlyable,
			/// <summary>All Rebel craft set to 'Flyable' are available</summary>
			AllRebelFlyable,
			/// <summary>All Imperial craft set tp 'Flyable' are available</summary>
			AllImperialFlyable,
			/// <summary>Available craft are determined by <see cref="OptCraft"/></summary>
			Custom 
		}
		
		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All <see cref="Orders"/> set to <b>100%</b> <see cref="BaseFlightGroup.BaseOrder.Throttle"/>, <see cref="Goals"/> are all set to <b>NONE</b>, SP1 <b>Enabled</b>, <see cref="Unknowns"/> are <b>0/false</b></remarks>
		public FlightGroup()
		{
			_stringLength = 0x14;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _roles.Length; i++) _roles[i] = "";
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < _skipToOrder4Trigger.Length; i++) { _skipToOrder4Trigger[i] = new Mission.Trigger(); _skipToOrder4Trigger[i].Condition = 10; }
			for (int i = 0; i < _goals.Length; i++) _goals[i] = new Goal();
			_optLoad[0] = true;
			_optLoad[8] = true;
			_optLoad[12] = true;
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
			_rolesIndexer = new Indexer<string>(_roles, 4);
			_loadoutIndexer = new LoadoutIndexer(_optLoad);
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Short form is <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b><br/>
		/// Long form is <b>"<see cref="Team"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see> - IsPlayer
		/// <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/>.
		/// <see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/> (<see cref="GlobalUnit"/>)"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
			if (!verbose) return longName;
			return Team + " - " + GlobalGroup + " - " + (PlayerNumber != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName + (GlobalUnit != 0 ? " (" + GlobalUnit + ")" : "");
		}
		
		#region craft
		/// <summary>Gets the craft roles, such as Command Ship or Strike Craft</summary>
		/// <remarks>This value has been seen as an AI string with unknown results. Restricted to 4 characters.</remarks>
		public Indexer<string> Roles { get { return _rolesIndexer; } }
		
		/// <summary>The allegiance value that controls goals and IFF behaviour</summary>
		public byte Team { get; set; }
		
		/// <summary>The team or player number to which the craft communicates with</summary>
		public byte Radio { get; set; }
		
		/// <summary>Determines if the craft has a human or AI pilot</summary>
		/// <remarks>Value of <b>zero</b> defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present.</remarks>
		public byte PlayerNumber { get; set; }
		
		/// <summary>Determines if craft is required to be player-controlled</summary>
		/// <remarks>When <b>true</b>, craft with PlayerNumber set will not appear without a human player.</remarks>
		public bool ArriveOnlyIfHuman { get; set; }
		#endregion
		#region arr/dep
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds { get { return (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && _arrDepTriggers[2].Condition == 0 && _arrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure triggers</summary>
		/// <remarks>Use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets which <see cref="ArrDepTriggers"/> must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}; effectively <see cref="ArrDepTriggerIndex"/> / 2</remarks>
		public bool[] ArrDepAO { get { return _arrDepAO; } }
		#endregion
		/// <summary>Gets the Orders used to control FlightGroup behaviour</summary>
		public Order[] Orders { get { return _orders; } }
		/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to <see cref="Orders">Order[3]</see></summary>
		/// <remarks>Array length is 2</remarks>
		public Mission.Trigger[] SkipToOrder4Trigger { get { return _skipToOrder4Trigger; } }
		/// <summary>Determines if both <see cref="SkipToOrder4Trigger">Skip triggers</see> must be completed</summary>
		/// <remarks><b>true</b> is AND, <b>false</b> is OR</remarks>
		public bool SkipToO4T1AndOrT2 { get; set; }
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 8</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Gets the FlightGroup location markers</summary>
		/// <remarks>Use <see cref="WaypointIndex"/> for indexes</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }
		
		#region Unks and Options
		/// <summary>The defenses available to the FG</summary>
		public byte Countermeasures { get; set; }
		/// <summary>The duration of death animation</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime { get; set; }
		/// <summary>The second condition of the FlightGroup upon creation</summary>
		public byte Status2 { get; set; }
		/// <summary>The additional grouping assignment, can share craft numbering</summary>
		public byte GlobalUnit { get; set; }
		
		/// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use <see cref="LoadoutIndexer.Indexes"/> for indexes</remarks>
		public LoadoutIndexer OptLoadout { get { return _loadoutIndexer; } }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get { return _optCraft; } }
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory { get; set; }
		/// <summary>The unknown values container</summary>
		/// <remarks>All values initialize to <b>0</b> or <b>false</b>. <see cref="Orders"/> contain Unknown6-9, <see cref="Goals"/> contain Unknown10-16</remarks>
		public UnknownValues Unknowns;
		#endregion
		
		/// <summary>Object to provide array access to the Optional Craft Loadout values</summary>
		[Serializable] public class LoadoutIndexer : Indexer<bool>
		{
			/// <summary>Indexes for <see cref="this[Indexes]"/></summary>
			public enum Indexes : byte {
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
			/// <param name="loadout">The Loadout array</param>
			public LoadoutIndexer(bool[] loadout) : base(loadout) { }
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value, <b>0-15</b></param>
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
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value</param>
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
		/// <summary>Object for a single Waypoint</summary>
		[Serializable] public class Waypoint : BaseWaypoint
		{
			/// <summary>Default constructor</summary>
			public Waypoint() : base(new short[4]) { }
			
			/// <summary>Initialize a new Waypoint using raw data</summary>
			/// <remarks><i>raw</i> must have Length of 4</remarks>
			/// <param name="raw">Raw data</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Waypoint(short[] raw) : base(raw) { if (raw.Length != 4) throw new ArgumentException("raw does not have the correct size"); }

			/// <summary>Converts a Waypoint for TIE</summary>
			/// <param name="wp">The Waypoint to convert</param>
			/// <returns>A copy of <i>wp</i> for TIE95</returns>
			public static implicit operator Tie.FlightGroup.Waypoint(Waypoint wp) { return new Tie.FlightGroup.Waypoint((short[])wp); }
			/// <summary>Converts a Waypoint for XWA</summary>
			/// <param name="wp">The Waypoint to convert</param>
			/// <returns>A copy of <i>wp</i> for XWA</returns>
			public static implicit operator Xwa.FlightGroup.Waypoint(Waypoint wp) { return new Xwa.FlightGroup.Waypoint((short[])wp); }
		}
		/// <summary>Object for a single FlightGroup-specific Goal</summary>
		[Serializable] public class Goal : Indexer<byte>
		{
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks><see cref="Condition"/> is set to <b>10</b> ("never (FALSE)")</remarks>
			public Goal()
			{
				_items = new byte[15];
				_items[1] = 10;
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length < 15) throw new ArgumentException("Minimum length of raw is 15", "raw");
				_items = new byte[15];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Goal(byte[] raw, int startIndex)
			{
				if (raw.Length < 15) throw new ArgumentException("Minimum length of raw is 15", "raw");
				if (raw.Length - startIndex < 15 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 15));
				_items = new byte[15];
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
					goal += " " + Strings.Trigger[Condition] + " (" + Points + " points)";
				}
				else goal = "None";
				return goal;
			}

			#region public properties
			/// <summary>Gets or sets the goal behaviour</summary>
			/// <remarks>Values are <b>0-3</b>; must, must not (prevent), BONUS must, BONUS must not (bonus prevent)</remarks>
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
			/// <remarks>Equals <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b></remarks>
			public short Points
			{
				get { return (short)(_items[3] * 250); }
				set { _items[3] = Convert.ToByte((value > 31750 ? 31750 : (value < -32000 ? -32000 : value)) / 250); }
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
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x06</remarks>
			public bool Unknown10
			{
				get { return Convert.ToBoolean(_items[6]); }
				set { _items[6] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x07</remarks>
			public bool Unknown11
			{
				get { return Convert.ToBoolean(_items[7]); }
				set { _items[7] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x08</remarks>
			public bool Unknown12
			{
				get { return Convert.ToBoolean(_items[8]); }
				set { _items[8] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0B</remarks>
			public byte Unknown13
			{
				get { return _items[11]; }
				set { _items[11] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0C</remarks>
			public bool Unknown14
			{
				get { return Convert.ToBoolean(_items[12]); }
				set { _items[12] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0E</remarks>
			public byte Unknown16
			{
				get { return _items[14]; }
				set { _items[14] = value; }
			}
			
			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary goals</remarks>
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
			/// <remarks>String is limited to 63 char. Not used for Secondary or Prevent goals</remarks>
			public string FailedText
			{
				get { return _failedText; }
				set { _failedText = StringFunctions.GetTrimmed(value, 63); }
			}
			#endregion public properties
		}
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0062, in Craft section</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0063, in Craft section</remarks>
			public bool Unknown2 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0085, in Arr/Dep section, may be ArrDelay30Sec</remarks>
			public byte Unknown3 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0096, in Arr/Dep section</remarks>
			public byte Unknown4 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0098, in Arr/Dep section</remarks>
			public byte Unknown5 { get; set; }

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0516, in Unknowns/Options section</remarks>
			public bool Unknown17 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0518, in Unknowns/Options section</remarks>
			public bool Unknown18 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0520, in Unknowns/Options section</remarks>
			public bool Unknown19 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0521, in Unknowns/Options section</remarks>
			public byte Unknown20 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0522, in Unknowns/Options section</remarks>
			public byte Unknown21 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0527, in Unknowns/Options section</remarks>
			public bool Unknown22 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0528, in Unknowns/Options section</remarks>
			public bool Unknown23 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0529, in Unknowns/Options section</remarks>
			public bool Unknown24 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052A, in Unknowns/Options section</remarks>
			public bool Unknown25 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052B, in Unknowns/Options section</remarks>
			public bool Unknown26 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052C, in Unknowns/Options section</remarks>
			public bool Unknown27 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052D, in Unknowns/Options section</remarks>
			public bool Unknown28 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052E, in Unknowns/Options section</remarks>
			public bool Unknown29 { get; set; }
		}
		/// <summary>Object for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _designation = "";
			
			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b></remarks>
			public Order()
			{
				_items = new byte[19];
				_items[1] = 10;	// Throttle
				_items[10] = _items[16] = 1;	// AndOrs
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks>If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19) ArrayFunctions.TrimArray(raw, 0, _items);
				else ArrayFunctions.WriteToArray(raw, _items, 0);
				_checkValues(this);
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks>If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
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
				_checkValues(this);
			}
			#endregion
			
			static void _checkValues(Order o)
			{
				string error = "";
				string msg;
				byte tempVar;
				if (o.Command > 39) error += "Command (" + o.Command + ")";
				tempVar = o.Target1;
				Mission.CheckTarget(o.Target1Type, ref tempVar, out msg);
				o.Target1 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T1 " + msg;
				tempVar = o.Target2;
				Mission.CheckTarget(o.Target2Type, ref tempVar, out msg);
				o.Target2 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T2 " + msg;
				tempVar = o.Target3;
				Mission.CheckTarget(o.Target3Type, ref tempVar, out msg);
				o.Target3 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T3 " + msg;
				tempVar = o.Target4;
				Mission.CheckTarget(o.Target4Type, ref tempVar, out msg);
				o.Target4 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T4 " + msg;
				if (error != "") throw new ArgumentException("Invalid values detected: " + error);
			}

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
					case 0x15:
						s += "All Teams except TM:" + target;
						break;
					case 0x17:
						s += "Global Unit " + target;
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

			/// <summary>Converts an Order into a byte array</summary>
			/// <remarks><see cref="Designation"/> is lost in the conversion</remarks>
			/// <param name="order">The Order to convert</param>
			/// <returns>A byte array of Length 19 containing the order data</returns>
			public static explicit operator byte[](Order order)
			{
				// Designation is lost
				byte[] b = new byte[19];
				for (int i = 0; i < 19; i++) b[i] = order[i];
				return b;
			}
			/// <summary>Converts an Order for TIE</summary>
			/// <remarks><see cref="Speed"/> and <see cref="Designation"/> are lost</remarks>
			/// <param name="order">The Order to convert</param>
			/// <returns>A copy of <i>order</i> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order order)
			{
				// Speed, Designtation are lost
				return new Tie.FlightGroup.Order((byte[])order, 0);
			}
			/// <summary>Converts an Order for XWA</summary>
			/// <param name="order">The Order to convert</param>
			/// <returns>A copy of <i>order</i> for use in XWA</returns>
			public static implicit operator Xwa.FlightGroup.Order(Order order)
			{
				Xwa.FlightGroup.Order ord = new Xwa.FlightGroup.Order((byte[])order);
				ord.CustomText = order.Designation;
				return ord;
			}
			
			#region public properties
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown6
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown7
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x0B</remarks>
			public byte Unknown8
			{
				get { return _items[11]; }
				set { _items[11] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x11</remarks>
			public byte Unknown9
			{
				get { return _items[17]; }
				set { _items[17] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _items[18]; }
				set { _items[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 16 characters</remarks>
			public string Designation
			{
				get { return _designation; }
				set { _designation = StringFunctions.GetTrimmed(value, 16); }
			}
			#endregion public properties
		}
	}
}
