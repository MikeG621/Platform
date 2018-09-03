/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * [UPD] added EditorCraftNumber and Difficulty to ToString() [JB]
 * [NEW] helper functions for move/delete FG [JB]
 * [NEW] Campaign perma-death, formerly Unk9 and Unk10 [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0.1, 120814
 * [NEW] UnknownValues.19-21
 * v2.0, 120525
 * [NEW] ToString() overrides
 * [UPD] Radio renamed to FollowsOrders
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable] public partial class FlightGroup : BaseFlightGroup
	{
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[3];
		string _pilot = "";
		Order[] _orders = new Order[3];
		Waypoint[] _waypoints = new Waypoint[15];
		
		/// <summary>Indexes for <see cref="ArrDepTriggers"/></summary>
		public enum ArrDepTriggerIndex : byte {
			/// <summary>First arrival trigger</summary>
			Arrival1,
			/// <summary>Second arrival trigger</summary>
			Arrival2,
			/// <summary>Departure trigger</summary>
			Departure
		}
		/// <summary>Indexes for <see cref="Waypoints"/></summary>
		public enum WaypointIndex : byte
		{
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
			/// <summary>Briefing coordinate</summary>
			Briefing
		}

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set Throttle to <b>100%</b>, all Goals set to <b>FALSE</b>, SP1 <b>Enabled</b></remarks>
		public FlightGroup()
		{
			_stringLength = 0xC;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			Goals = new FGGoals();
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"CraftAbbrv Name"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Parenthesis indicate "if applicable" fields, doubled (( )) being "if applicable" and include literal parenthesis.<br/>
		/// Short form is <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;)"</b><br/><br/>
		/// Long form is <b>"<see cref="BaseFlightGroup.IFF"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see>
		/// - (IsPlayer * indicator) <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/> 
		/// <see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;) ([<see cref="BaseStrings.DifficultyAbbrv"/>])"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
            if (EditorCraftNumber > 0) //[JB] Added numbering information.
                longName += EditorCraftExplicit ? " " + EditorCraftNumber : " <" + EditorCraftNumber + ">";
			if (!verbose) return longName;

            //[JB] Added difficulty tag, by feature request.
            string ret = IFF + " - " + GlobalGroup + " - " + (PlayerCraft != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName;
            if (Difficulty >= 1 && Difficulty <= 6)
                ret += " [" + BaseStrings.DifficultyAbbrv[Difficulty] + "]";

            return ret;
		}
		
        /// <summary>Changes all Flight Group indexes during a FG Move or Delete operation.</summary>
        /// <remarks>Should not be called directly unless part of a larger FG Move or Delete operation.  FG references may exist in other mission properties, ensure those properties are adjusted when applicable.</remarks>
        /// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
        /// <param name="dstIndex">The FG index to replace with.  Specify -1 to Delete, or zero or above to Move.</param>
        /// <returns>Returns true if anything was changed.</returns>
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
            if (ArrivalCraft1 == srcIndex) { ArrivalCraft1 = dst; change = true; if (delete) { ArrivalMethod1 = false; } } else if (ArrivalCraft1 > srcIndex && delete) { ArrivalCraft1--; change = true; }
            if (ArrivalCraft2 == srcIndex) { ArrivalCraft2 = dst; change = true; if (delete) { ArrivalMethod2 = false; } } else if (ArrivalCraft2 > srcIndex && delete) { ArrivalCraft2--; change = true; }
            if (DepartureCraft1 == srcIndex) { DepartureCraft1 = dst; change = true; if (delete) { DepartureMethod1 = false; } } else if (DepartureCraft1 > srcIndex && delete) { DepartureCraft1--; change = true; }
            if (DepartureCraft2 == srcIndex) { DepartureCraft2 = dst; change = true; if (delete) { DepartureMethod2 = false; } } else if (DepartureCraft2 > srcIndex && delete) { DepartureCraft2--; change = true; }
            for (int i = 0; i < ArrDepTriggers.Length; i++)
            {
                Mission.Trigger adt = ArrDepTriggers[i];
                change |= adt.TransformFGReferences(srcIndex, dstIndex, (i < 2));  //[0] and [1] are are arrival.  Set those to true.
            }
            foreach (FlightGroup.Order order in Orders)
                change |= order.TransformFGReferences(srcIndex, dstIndex);
            return change;
        }

		/// <summary>Gets or sets the Pilot name, used as short note</summary>
		/// <remarks>Restricted to 12 characters</remarks>
		public string Pilot
		{
			get { return _pilot; }
			set { _pilot = StringFunctions.GetTrimmed(value, _stringLength); }
		}
        /// <summary>Gets or sets if the FlightGroup responds to player's orders</summary>
		public bool FollowsOrders { get; set; }
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for blank Arrival triggers and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds { get { return (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Array length is 3, use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets or sets if both triggers must be completed</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b></remarks>
		public bool AT1AndOrAT2 { get; set; }
		/// <summary>Gets or sets the FlightGroup-specific mission goals</summary>
		public FGGoals Goals { get; set; }
		/// <summary>Gets the Orders array used to control FlightGroup behaviour</summary>
		public Order[] Orders { get { return _orders; } }
		/// <summary>Gets or sets the unknown values container</summary>
		/// <remarks>All values initialize to <b>0</b> or <b>false</b></remarks>
		public UnknownValues Unknowns;
		/// <summary>Gets the Waypoint array used to determine starting location and AI pathing</summary>
		/// <remarks>Array length is 15, use <see cref="WaypointIndex"/> for indexes</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }

        /// <summary>If enabled, Flight Groups that die in campaign mode will not appear in later missions.  Formerly Unknown9</summary>
        public byte PermaDeathEnabled { get; set; }	//TODO: I think this is really bool, need to get MOA running to check
        /// <summary>If enabled, Flight Groups matching this ID that have died in previous missions will not appear.  Formerly Unknown10</summary>
        public byte PermaDeathID { get; set; }

		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x003B, in Craft section, Reserved(0)</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0041, in Craft section</remarks>
			public byte Unknown5 { get; set; }

			// Unknown9 is campaign perma-death flag.  Offset 0x0046, in Craft section.

			// Unknown10 is campaign perma-death ID.  Offset 0x0047, in Craft section.
            
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0048, in Craft section, Reserved(0)</remarks>
			public byte Unknown11 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0053, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown12 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005D, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown15 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005E, in Arr/Dep section</remarks>
			public byte Unknown16 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005F, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown17 { get; set; }

			// Unknown18 is in Orders

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0120</remarks>
			public bool Unknown19 { get; set; }

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0122</remarks>
			public byte Unknown20 { get; set; }

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0123</remarks>
			public bool Unknown21 { get; set; }

			/// <summary>Array form of the Unknowns</summary>
			/// <param name="index">Valid indexes are 0-9</param>
			/// <exception cref="ArgumentOutOfRangeException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get
				{
					if (index == 0) return Unknown1;
					else if (index == 1) return Unknown5;
					else if (index == 2) return Unknown11;
					else if (index == 3) return Unknown12;
					else if (index == 4) return Unknown15;
					else if (index == 5) return Unknown16;
					else if (index == 6) return Unknown17;
					else if (index == 7) return Convert.ToByte(Unknown19);
					else if (index == 8) return Unknown20;
					else if (index == 9) return Convert.ToByte(Unknown21);
					else throw new ArgumentOutOfRangeException("index must be 0-9", "index");
				}
				set
				{
					if (index == 0) Unknown1 = value;
					else if (index == 1) Unknown5 = value;
					else if (index == 2) Unknown11 = value;
					else if (index == 3) Unknown12 = value;
					else if (index == 4) Unknown15 = value;
					else if (index == 5) Unknown16 = value;
					else if (index == 6) Unknown17 = value;
					else if (index == 7) Unknown19 = Convert.ToBoolean(value);
					else if (index == 8) Unknown20 = value;
					else if (index == 9) Unknown21 = Convert.ToBoolean(value);
				}
			}
		}
	}
}
