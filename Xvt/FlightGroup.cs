/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] cleaned arrays
 * v3.0, 180903
 * [UPD] added Ion Pulse, Energy Beam, Cluster Mine [JB]
 * [UPD] changed default Skip triggers to True [JB]
 * [UPD] added info to to ToString() [JB]
 * [NEW] helper functions for move/delete FG [JB]
 * [NEW] PreventCraftNumbering, DepartureClockMinutes, DepartureClockSeconds, formerly Unk 19, 20, 21 [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] ToString() overrides
 * [UPD] RolesIndexer converted to Indexer<string>
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable] public partial class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		readonly string[] _roles = new string[4];
		readonly bool[] _optLoad = new bool[18];  //[JB] Added Ion Pulse, Energy Beam, Cluster Mine

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
			for (int i = 0; i < Orders.Length; i++) Orders[i] = new Order();
			for (int i = 0; i < _roles.Length; i++) _roles[i] = "";
			for (int i = 0; i < ArrDepTriggers.Length; i++) ArrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < SkipToOrder4Trigger.Length; i++) { SkipToOrder4Trigger[i] = new Mission.Trigger(); }  //[JB] Removed setting skip trigger condition to False by default. LEC missions use True for empty conditions.
			for (int i = 0; i < Goals.Length; i++) Goals[i] = new Goal();
            _optLoad[0] = true;
            _optLoad[9] = true;  //[JB] Adjusted these indexes for added Ion Pulse, Energy Beam, Cluster Mine
            _optLoad[14] = true;
            for (int i = 0; i < Waypoints.Length; i++) Waypoints[i] = new Waypoint();
			Waypoints[(int)WaypointIndex.Start1].Enabled = true;
			Roles = new Indexer<string>(_roles, 4);
			OptLoadout = new LoadoutIndexer(_optLoad);
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Parenthesis indicate "if applicable" fields, doubled (( )) being "if applicable" and include literal parenthesis.<br/>
		/// Short form is <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;)"</b><br/><br/>
		/// Long form is <b>"<see cref="Team"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see> - (IsPlayer * indicator)
		/// <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/> 
		/// <see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;) ((<see cref="GlobalUnit"/>))
		/// ([(Plr: <see cref="PlayerNumber"/>) ("hu" if <see cref="ArriveOnlyIfHuman"/>)])"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
            if (EditorCraftNumber > 0) //[JB] Added numbering information.
                longName += EditorCraftExplicit ? " " + EditorCraftNumber : " <" + EditorCraftNumber + ">";
            if (!verbose) return longName;

			//[JB] Added player position and difficulty tags, by feature request.
            string plr = "";
            if (PlayerNumber > 0 || ArriveOnlyIfHuman)
            {
                if (PlayerNumber > 0) plr = "Plr:" + PlayerNumber;
                if (ArriveOnlyIfHuman) plr += (plr.Length > 0 ? " " : "") + "hu";
                plr = " [" + plr + "]";
            }
            string ret = Team + " - " + GlobalGroup + " - " + (PlayerNumber != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName + (GlobalUnit != 0 ? " (" + GlobalUnit + ")" : "") + plr;
            if(Difficulty >= 1 && Difficulty < Strings.DifficultyAbbrv.Length)
                ret += " [" + Strings.DifficultyAbbrv[Difficulty] + "]";

            return ret;
		}

        /// <summary>Changes all Flight Group indexes, to be used during a FG Swap (Move) or Delete operation.</summary>
        /// <remarks>FG references may exist in other mission properties, ensure those properties are adjusted too.</remarks>
        /// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
        /// <param name="dstIndex">The FG index to replace with.  Specify -1 to Delete, or zero or above to Move.</param>
        /// <returns>True if something was changed.</returns>
        public bool TransformFGReferences(int srcIndex, int dstIndex)
        {
            bool change = false;
            bool delete = false;
            byte dst = (byte)dstIndex;
            if (dstIndex < 0)
            {
                dst = 0;
                delete = true;
            }

            //If the FG matches, replace (and delete if necessary).  Else if our index is higher and we're supposed to delete, decrement index.
            if (ArrivalCraft1 == srcIndex) { change = true; ArrivalCraft1 = dst; if (delete) { ArrivalMethod1 = false; } } else if (ArrivalCraft1 > srcIndex && delete == true) { change = true; ArrivalCraft1--; }
            if (ArrivalCraft2 == srcIndex) { change = true; ArrivalCraft2 = dst; if (delete) { ArrivalMethod2 = false; } } else if (ArrivalCraft2 > srcIndex && delete == true) { change = true; ArrivalCraft2--; }
            if (DepartureCraft1 == srcIndex) { change = true; DepartureCraft1 = dst; if (delete) { DepartureMethod1 = false; } } else if (DepartureCraft1 > srcIndex && delete == true) { change = true; DepartureCraft1--; }
            if (DepartureCraft2 == srcIndex) { change = true; DepartureCraft2 = dst; if (delete) { DepartureMethod2 = false; } } else if (DepartureCraft2 > srcIndex && delete == true) { change = true; DepartureCraft2--; }
            for (int i = 0; i < ArrDepTriggers.Length; i++)
            {
                Mission.Trigger adt = ArrDepTriggers[i];
                change |= adt.TransformFGReferences(srcIndex, dstIndex, true);  //First 2 are arrival.  Set those to true.
            }
            foreach(Mission.Trigger trig in SkipToOrder4Trigger)
                change |= trig.TransformFGReferences(srcIndex, dstIndex, true);

            foreach (FlightGroup.Order order in Orders)
                change |= order.TransformFGReferences(srcIndex, dstIndex);

            return change;
        }

     
        #region craft
		/// <summary>Gets the craft roles, such as Command Ship or Strike Craft</summary>
		/// <remarks>This value has been seen as an AI string with unknown results. Restricted to 4 characters.</remarks>
		public Indexer<string> Roles { get; private set; }
		
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
		public bool ArrivesIn30Seconds { get { return (ArrDepTriggers[0].Condition == 0 && ArrDepTriggers[1].Condition == 0 && ArrDepTriggers[2].Condition == 0 && ArrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure triggers</summary>
		/// <remarks>Use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get; } = new Mission.Trigger[6];
		/// <summary>Gets which <see cref="ArrDepTriggers"/> must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}; effectively <see cref="ArrDepTriggerIndex"/> / 2</remarks>
		public bool[] ArrDepAO { get; } = new bool[4];
		#endregion
		/// <summary>Gets the Orders used to control FlightGroup behaviour</summary>
		/// <remarks>Array is length = 4</remarks>
		public Order[] Orders { get; } = new Order[4];
		/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to <see cref="Orders">Order[3]</see></summary>
		/// <remarks>Array length is 2</remarks>
		public Mission.Trigger[] SkipToOrder4Trigger { get; } = new Mission.Trigger[2];
		/// <summary>Determines if both <see cref="SkipToOrder4Trigger">Skip triggers</see> must be completed</summary>
		/// <remarks><b>true</b> is AND, <b>false</b> is OR</remarks>
		public bool SkipToO4T1AndOrT2 { get; set; }
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 8</remarks>
		public Goal[] Goals { get; } = new Goal[8];

		/// <summary>Gets the FlightGroup location markers</summary>
		/// <remarks>Use <see cref="WaypointIndex"/> for indexes</remarks>
		public Waypoint[] Waypoints { get; } = new Waypoint[22];
		
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
		/// <summary>Determines a special condition when additional waves will stop arriving. This is not an abort condition.</summary>
		public byte StopArrivingWhen { get; set; }
		/// <summary>Prevents craft numbering (if multiple craft in each wave) from appearing in the CMD.</summary>
		public bool PreventCraftNumbering { get; set; }
		/// <summary>If nonzero, the craft will abort mission when the elapsed mission time (player's in-flight clock) reaches this time in minutes.  Formerly Unknown20 at Offset 0x0521.</summary>
		public byte DepartureClockMinutes { get; set; }
		/// <summary>If nonzero, the craft will abort mission when the elapsed mission time (player's in-flight clock) reaches this time in seconds.  Formerly Unknown21 at Offset 0x0522.</summary>
		public byte DepartureClockSeconds { get; set; }
		/// <summary>Determines additional arrival delay time (minutes) based on whether Randomize is enabled in-game.</summary>
		public byte RandomArrivalDelayMinutes { get; set; }
		/// <summary>Determines additional arrival delay time (seconds) based on whether Randomize is enabled in-game.</summary>
		public byte RandomArrivalDelaySeconds { get; set; }

        /// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use <see cref="LoadoutIndexer.Indexes"/> for indexes</remarks>
		public LoadoutIndexer OptLoadout { get; private set; }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get; } = new OptionalCraft[10];
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory { get; set; }
		/// <summary>The unknown values container</summary>
		/// <remarks>All values initialize to <b>0</b> or <b>false</b>. <see cref="Orders"/> contain Unknown6-9, <see cref="Goals"/> contain Unknown10-16</remarks>
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
			/// <remarks>Offset 0x0062, in Craft section</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0063, in Craft section. <see cref="StopArrivingWhen"/></remarks>
			//public bool Unknown2 { get; set; }
			
			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0085, in Arr/Dep section. <see cref="RandomArrivalDelayMinutes"/></remarks>
			//public byte Unknown3 { get; set; }
			
			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0096, in Arr/Dep section. <see cref="RandomArrivalDelaySeconds"/></remarks>
			//public byte Unknown4 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0098, in Arr/Dep section</remarks>
			public byte Unknown5 { get; set; }

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0516, in Unknowns/Options section</remarks>
			public bool Unknown17 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0518, in Unknowns/Options section</remarks>
			public bool Unknown18 { get; set; }
			
			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0520, in Unknowns/Options section. See <see cref="PreventCraftNumbering"/></remarks>
			//public bool Unknown19 { get; set; }

			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0521, in Unknowns/Options section. See <see cref="DepartureClockMinutes"/></remarks>
			//public byte Unknown20 { get; set; }

			/// <summary>Previously Unknown value</summary>
			/// <remarks>Offset 0x0522, in Unknowns/Options section. See <see cref="DepartureClockSeconds"/></remarks>
			//public byte Unknown21 { get; set; }
			
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
	}
}
