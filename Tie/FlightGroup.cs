/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0+
 */

/* CHANGELOG
 * [UPD] Format spec implemented
 * [UPD] Renamed AndOr to Or for proper boolean naming
 * v4.0, 200809
 * [UPD] PermaDeath is bool
 * v3.0, 180309
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
	/// <summary>Object for individual FlightGroups.</summary>
	[Serializable]
	public partial class FlightGroup : BaseFlightGroup
	{
		readonly Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[3];
		string _pilot = "";
		readonly Order[] _orders = new Order[3];
		readonly Waypoint[] _waypoints = new Waypoint[15];

		/// <summary>Indexes for <see cref="ArrDepTriggers"/>.</summary>
		public enum ArrDepTriggerIndex : byte
		{
			/// <summary>First arrival trigger.</summary>
			Arrival1,
			/// <summary>Second arrival trigger.</summary>
			Arrival2,
			/// <summary>Departure trigger.</summary>
			Departure
		}
		/// <summary>Indexes for <see cref="Waypoints"/>.</summary>
		public enum WaypointIndex : byte
		{
			/// <summary>Primary starting coordinate.</summary>
			Start1,
			/// <summary>Optional starting coordinate.</summary>
			Start2,
			/// <summary>Optional starting coordinate.</summary>
			Start3,
			/// <summary>Optional starting coordinate.</summary>
			Start4,
			/// <summary>First coordinate for orders and initial trajectory.</summary>
			WP1,
			/// <summary>Second order coordinate.</summary>
			WP2,
			/// <summary>Third order coordinate.</summary>
			WP3,
			/// <summary>Fourth order coordinate.</summary>
			WP4,
			/// <summary>Fifth order coordinate.</summary>
			WP5,
			/// <summary>Sixth order coordinate.</summary>
			WP6,
			/// <summary>Seventh order coordinate.</summary>
			WP7,
			/// <summary>Eigth order coordinate.</summary>
			WP8,
			/// <summary>Coordinate for Rendezvous orders.</summary>
			Rendezvous,
			/// <summary>Arrival and Departure coordinate.</summary>
			Hyperspace,
			/// <summary>Briefing coordinate.</summary>
			Briefing
		}

		/// <summary>Initializes a new FlightGroup.</summary>
		/// <remarks>All Orders set Throttle to <b>100%</b>, all Goals set to <b>FALSE</b>, SP1 <b>Enabled</b>.</remarks>
		public FlightGroup()
		{
			_stringLength = 0xC;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			Goals = new FGGoals();
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
		}

		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <returns>Short representation of the FlightGroup as <b>"CraftAbbrv Name"</b> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;)"</b>.</returns>
		public override string ToString() => ToString(false);
		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <remarks>Parenthesis indicate "if applicable" fields, doubled (( )) being "if applicable" and include literal parenthesis.<br/>
		/// Short form is <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;)"</b>.<br/><br/>
		/// Long form is <b>"<see cref="BaseFlightGroup.IFF"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see>
		/// - (IsPlayer * indicator) <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/> 
		/// <see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;) ([<see cref="BaseStrings.DifficultyAbbrv"/>])"</b>.</remarks>
		/// <param name="verbose">When <b>true</b> returns long form.</param>
		/// <returns>Representation of the FlightGroup.</returns>
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
			if (ArrivalMothership == srcIndex) { ArrivalMothership = dst; change = true; if (delete) { ArriveViaMothership = false; } } else if (ArrivalMothership > srcIndex && delete) { ArrivalMothership--; change = true; }
			if (AlternateMothership == srcIndex) { AlternateMothership = dst; change = true; if (delete) { AlternateMothershipUsed = false; } } else if (AlternateMothership > srcIndex && delete) { AlternateMothership--; change = true; }
			if (DepartureMothership == srcIndex) { DepartureMothership = dst; change = true; if (delete) { DepartViaMothership = false; } } else if (DepartureMothership > srcIndex && delete) { DepartureMothership--; change = true; }
			if (CapturedDepartureMothership == srcIndex) { CapturedDepartureMothership = dst; change = true; if (delete) { CapturedDepartViaMothership = false; } } else if (CapturedDepartureMothership > srcIndex && delete) { CapturedDepartureMothership--; change = true; }
			for (int i = 0; i < ArrDepTriggers.Length; i++)
			{
				Mission.Trigger adt = ArrDepTriggers[i];
				change |= adt.TransformFGReferences(srcIndex, dstIndex, (i < 2));  //[0] and [1] are are arrival.  Set those to true.
			}
			foreach (Order order in Orders)
				change |= order.TransformFGReferences(srcIndex, dstIndex);
			return change;
		}

		/// <summary>Gets or sets the Pilot name, used as short note.</summary>
		/// <remarks>Restricted to 12 characters.</remarks>
		public string Pilot
		{
			get => _pilot;
			set => _pilot = StringFunctions.GetTrimmed(value, _stringLength);
		}
		/// <summary>Gets or sets if the FlightGroup responds to player's orders.</summary>
		public bool FollowsOrders { get; set; }
		/// <summary>If enabled, Flight Groups that die in campaign mode will not appear in later missions.</summary>
		public bool PermaDeathEnabled { get; set; }
		/// <summary>If enabled, Flight Groups matching this ID that have died in previous missions will not appear.</summary>
		public byte PermaDeathID { get; set; }
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start.</summary>
		/// <remarks>Looks for blank Arrival triggers and a delay of 30 seconds or less.</remarks>
		public bool ArrivesIn30Seconds => (_arrDepTriggers[0].Condition == (byte)Mission.Trigger.ConditionList.True
			&& _arrDepTriggers[1].Condition == (byte)Mission.Trigger.ConditionList.True
			&& ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30);
		/// <summary>Gets the Arrival and Departure trigger array.</summary>
		/// <remarks>Array length is 3, use <see cref="ArrDepTriggerIndex"/> for indexes.</remarks>
		public Mission.Trigger[] ArrDepTriggers => _arrDepTriggers;
		/// <summary>Gets or sets if both triggers must be completed.</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool AT1OrAT2 { get; set; }
		/// <summary>Gets or sets the FlightGroup-specific mission goals.</summary>
		public FGGoals Goals { get; set; }
		/// <summary>Gets the Orders array used to control FlightGroup behaviour.</summary>
		public Order[] Orders => _orders;
		/// <summary>Gets the Waypoint array used to determine starting location and AI pathing.</summary>
		/// <remarks>Array length is 15, use <see cref="WaypointIndex"/> for indexes.</remarks>
		public Waypoint[] Waypoints => _waypoints;
	}
}