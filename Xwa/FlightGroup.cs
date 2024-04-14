/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.0+
 */

/* CHANGELOG
 * [NEW] Full format spec implemented, CounterTypes
 * [UPD] Radio, Countermeasures changed to enum
 * v6.0, 231027
 * [FIX] Arr/Dep Method1 to byte
 * v4.0, 200809
 * [UPD] auto-properties
 * v3.0, 180903
 * [UPD] Energy Beam and Cluster Mine added [JB]
 * [UPD] disable Designations on init [JB]
 * [UPD] added info to ToString() [JB]
 * [NEW] helper functions for dete/swap FGs [JB]
 * v2.6.1, 171118
 * [ADD] backdrop-specific properties
 * v2.5, 170107
 * [ADD] Ion Pulse
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] ToString() overrides
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual FlightGroups.</summary>
	[Serializable]
	public partial class FlightGroup : BaseFlightGroup
	{
		string _role = "";
		readonly bool[] _optLoadout = new bool[18];
		string _pilotID = "";

		/// <summary>Indexes for <see cref="ArrDepTriggers"/>.</summary>
		public enum ArrDepTriggerIndex : byte
		{
			/// <summary>First Arrival trigger.</summary>
			Arrival1,
			/// <summary>Second Arrival trigger.</summary>
			Arrival2,
			/// <summary>Third Arrival trigger.</summary>
			Arrival3,
			/// <summary>Fourth Arrival trigger.</summary>
			Arrival4,
			/// <summary>First Departure trigger.</summary>
			Departure1,
			/// <summary>Second Departure trigger.</summary>
			Departure2
		}
		/// <summary>Values for <see cref="OptCraftCategory"/>.</summary>
		public enum OptionalCraftCategory : byte
		{
			/// <summary>No other available craft.</summary>
			None,
			/// <summary>All craft set to 'Flyable' are available.</summary>
			AllFlyable,
			/// <summary>All Rebel craft set to 'Flyable' are available.</summary>
			AllRebelFlyable,
			/// <summary>All Imperial craft set to 'Flyable' are available.</summary>
			AllImperialFlyable,
			/// <summary>Available craft are determined by <see cref="OptCraft"/>.</summary>
			Custom
		}
		/// <summary>Values for <see cref="Comms"/>.</summary>
		public enum CommsVerbosity : byte
		{
			/// <summary>No additional content.</summary>
			None,
			/// <summary>Short craft reference</summary>
			Minimal,
			/// <summary>Standard operation.</summary>
			/// <remarks>Verbosity is semi-randomized, depending on use.</remarks>
			Normal,
			/// <summary>Full craft reference.</summary>
			Verbose
		}
		/// <summary>Values for <see cref="Radio"/>.</summary>
		public enum RadioChannel : byte
		{
			/// <summary>No radio chatter.</summary>
			None,
			/// <summary>Team 1, usually the Player's.</summary>
			Team1,
			/// <summary>Team 2.</summary>
			Team2,
			/// <summary>Team 3.</summary>
			Team3,
			/// <summary>Team 4.</summary>
			Team4,
			/// <summary>Team 5.</summary>
			Team5,
			/// <summary>Team 6.</summary>
			Team6,
			/// <summary>Team 7.</summary>
			Team7,
			/// <summary>Team 8.</summary>
			Team8,
			/// <summary>Player 1 only, mainly for multiplayer.</summary>
			Player1,
			/// <summary>Player 2.</summary>
			Player2,
			/// <summary>Player 3.</summary>
			Player3,
			/// <summary>Player 4.</summary>
			Player4,
			/// <summary>Player 5.</summary>
			Player5,
			/// <summary>Player 6.</summary>
			Player6,
			/// <summary>Player 7.</summary>
			Player7,
			/// <summary>Player 8.</summary>
			Player8,
			/// <summary>Player 9.</summary>
			Player9,
			/// <summary>Player 10.</summary>
			Player10,
			/// <summary>Player 11.</summary>
			Player11,
			/// <summary>Player 12.</summary>
			Player12,
			/// <summary>Player 13.</summary>
			Player13,
			/// <summary>Player 14.</summary>
			Player14,
			/// <summary>Player 15.</summary>
			Player15,
			/// <summary>Player 16.</summary>
			Player16
		}
		/// <summary>Values for <see cref="StopArrivingWhen"/>.</summary>
		public enum WavesEnd
		{
			/// <summary>Waves continue until spent.</summary>
			Never,
			/// <summary>Waves end when primary goals are completed.</summary>
			MissionComplete,
			/// <summary>Waves end when FG's Team has won.</summary>
			TeamWon,
			/// <summary>Waves end when FG's Team has lost.</summary>
			TeamLost
		}
		/// <summary>Values for <see cref="Countermeasures"/>.</summary>
		public enum CounterTypes
		{
			/// <summary>None.</summary>
			None,
			/// <summary>Acts as obstacle for incoming warheads.</summary>
			Chaff,
			/// <summary>Acts as distraction for incoming warheads and targeting.</summary>
			Flare,
			/// <summary>Acts as explosives against nearby entities.</summary>
			ClusterMine
		}

		/// <summary>Initializes a new FlightGroup.</summary>
		/// <remarks>All <see cref="Orders"/> set to <b>100%</b> <see cref="BaseFlightGroup.BaseOrder.Throttle">Throttle</see> (16 total, 4 per Region), <see cref="Goals"/> set to <b>NONE</b>, SP1 <b>Enabled</b>.</remarks>
		public FlightGroup()
		{
			for (int i = 0; i < 6; i++) ArrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < 4; i++) Waypoints[i] = new Waypoint();
			for (int i = 0; i < 16; i++) Orders[i / 4, i % 4] = new Order();
			for (int i = 0; i < 8; i++) Goals[i] = new Goal();
			_optLoadout[(int)LoadoutIndexer.Indexes.NoWarheads] = true;
			_optLoadout[(int)LoadoutIndexer.Indexes.NoBeam] = true;
			_optLoadout[(int)LoadoutIndexer.Indexes.NoCountermeasures] = true;
			Waypoints[0].Enabled = true;
			OptLoadout = new LoadoutIndexer(_optLoadout);
		}

		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <returns>Short representation of the FlightGroup as <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (<see cref="BaseFlightGroup.EditorCraftNumber"/>)"</b>.</returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <remarks>Parenthesis indicate "if applicable" fields, doubled (( )) being "if applicable" and include literal parenthesis.<br/>
		/// Short form is <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (<see cref="BaseFlightGroup.EditorCraftNumber"/>)"</b>.<br/><br/>
		/// Long form is <b>"<see cref="Team"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see> - (IsPlayer * indicator)
		/// <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/> 
		/// <see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;) ((<see cref="GlobalUnit"/>))
		/// ([(Plr: <see cref="PlayerNumber"/>) ("hu" if <see cref="ArriveOnlyIfHuman"/>)])"</b>.</remarks>
		/// <param name="verbose">When <b>true</b> returns long form.</param>
		/// <returns>Representation of the FlightGroup.</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
			if (EditorCraftNumber > 0)
				longName += EditorCraftExplicit ? " " + EditorCraftNumber : " <" + EditorCraftNumber + ">";
			if (!verbose) return longName;

			string plr = "";
			if (PlayerNumber > 0 || ArriveOnlyIfHuman == true)
			{
				if (PlayerNumber > 0) plr = "Plr:" + PlayerNumber;
				if (ArriveOnlyIfHuman) plr += (plr.Length > 0 ? " " : "") + "hu";
				if (plr.Length > 0) plr = " [" + plr + "]";
			}
			string ret = (Team + 1) + " - " + GlobalGroup + " - " + (PlayerNumber != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName + (GlobalUnit != 0 ? " (" + GlobalUnit + ")" : "") + plr;
			if (Difficulty >= 1 && Difficulty <= 6)
				ret += " [" + Strings.DifficultyAbbrv[Difficulty] + "]";

			return ret;
		}
		/// <summary>Changes all Flight Group indexes, to be used during a FG Swap (Move) or Delete operation.</summary>
		/// <remarks>FG references may exist in other mission properties, ensure those properties are adjusted too.</remarks>
		/// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
		/// <param name="dstIndex">The FG index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
		/// <returns><b>True</b> if something was changed.</returns>
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
			if (ArrivalMothership == srcIndex) { change = true; ArrivalMothership = dst; if (delete) { ArrivalMethod = 0; } } else if (ArrivalMothership > srcIndex && delete == true) { change = true; ArrivalMothership--; }
			if (AlternateMothership == srcIndex) { change = true; AlternateMothership = dst; if (delete) { AlternateMothershipUsed = false; } } else if (AlternateMothership > srcIndex && delete == true) { change = true; AlternateMothership--; }
			if (DepartureMothership == srcIndex) { change = true; DepartureMothership = dst; if (delete) { DepartureMethod = 0; } } else if (DepartureMothership > srcIndex && delete == true) { change = true; DepartureMothership--; }
			if (CapturedDepartureMothership == srcIndex) { change = true; CapturedDepartureMothership = dst; if (delete) { CapturedDepartViaMothership = false; } } else if (CapturedDepartureMothership > srcIndex && delete == true) { change = true; CapturedDepartureMothership--; }
			for (int i = 0; i < ArrDepTriggers.Length; i++)
			{
				Mission.Trigger adt = ArrDepTriggers[i];
				change |= adt.TransformFGReferences(srcIndex, dstIndex, true);  //First 2 are arrival.  Set those to true.
			}

			//Skip triggers are part of the Order in XWA, don't need to handle them here.

			foreach (Order order in Orders)
				change |= order.TransformFGReferences(srcIndex, dstIndex);
			return change;
		}
		/// <summary>Changes all Message indexes, to be used during a Message Swap (Move) or Delete operation.</summary>
		/// <remarks>Same concept as for Flight Groups.  Triggers may depend on Message indexes, and this function helps ensure indexes are not broken.</remarks>
		/// <param name="srcIndex">The Message index to match and replace (Move), or match and Delete.</param>
		/// <param name="dstIndex">The Message index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="dstIndex"/> is <b>256</b> or more.</exception>
		/// <returns><b>True</b> if something was changed.</returns>
		public bool TransformMessageReferences(int srcIndex, int dstIndex)
		{
			bool change = false;

			for (int i = 0; i < ArrDepTriggers.Length; i++)
			{
				Mission.Trigger adt = ArrDepTriggers[i];
				change |= adt.TransformMessageRef(srcIndex, dstIndex);
			}

			foreach (Order order in Orders)
			{
				foreach (Mission.Trigger trig in order.SkipTriggers)
					change |= trig.TransformMessageRef(srcIndex, dstIndex);
			}

			return change;
		}

		#region craft
		/// <summary>Determines if <see cref="Designation1"/> is used, and which teams it applies to.</summary>
		public byte EnableDesignation1 { get; set; } = 255;
		/// <summary>Determines if <see cref="Designation2"/> is used, and which teams it applies to.</summary>
		public byte EnableDesignation2 { get; set; } = 255;
		/// <summary>Primary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys.</summary>
		public byte Designation1 { get; set; }
		/// <summary>Secondary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys.</summary>
		public byte Designation2 { get; set; }
		/// <summary>How verbose the craft radio callout is.</summary>
		/// <remarks>Defaults to <b>Normal</b>.</remarks>
		public CommsVerbosity Comms { get; set; } = CommsVerbosity.Normal;
		/// <summary>The <see cref="Mission.GlobalCargos"/> index to be used instead of <see cref="BaseFlightGroup.Cargo"/>.</summary>
		/// <remarks>One-indexed, zero is "N/A". Adjusted from/to zero-indexed during Load/Save.</remarks>
		public byte GlobalCargo { get; set; }
		/// <summary>The <see cref="Mission.GlobalCargos"/> index to be used instead of <see cref="BaseFlightGroup.SpecialCargo"/>.</summary>
		/// <remarks>One-indexed, zero is "N/A". Adjusted from/to zero-indexed during Load/Save.</remarks>
		public byte GlobalSpecialCargo { get; set; }
		/// <summary>Gets or sets a text form of the craft role.</summary>
		/// <remarks>19 character limit, appears to be an editor note, use Designation to drive mission effects.</remarks>
		public string Role
		{
			get => _role;
			set => _role = StringFunctions.GetTrimmed(value, _stringLength);
		}
		/// <summary>Gets or sets the allegiance value that controls goals and IFF behaviour.</summary>
		public byte Team { get; set; }
		/// <summary>Gets or sets the channel to which the craft communicates with.</summary>
		public RadioChannel Radio { get; set; }
		/// <summary>Gets or sets when waves stop arriving, even if there are waves remaining.</summary>
		public WavesEnd StopArrivingWhen { get; set; }
		/// <summary>Gets or sets if the craft has a human or AI pilot.</summary>
		/// <remarks>Value of <b>zero</b> defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present.</remarks>
		public byte PlayerNumber { get; set; }
		/// <summary>Gets or sets if craft is required to be player-controlled.</summary>
		/// <remarks>When <b>true</b>, craft with <see cref="PlayerNumber"/> set will not appear without a human player.</remarks>
		public bool ArriveOnlyIfHuman { get; set; }

		/// <summary>Gets or sets the RGB values of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.Name"/>.</remarks>
		public string LightRGB
		{
			get => Name;
			set => Name = value;
		}
		/// <summary>Gets or sets the Shadow value of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="GlobalCargo"/>, does not have error-checking.</remarks>
		public byte Shadow
		{
			get => GlobalCargo;
			set => GlobalCargo = value;
		}
		/// <summary>Gets or sets the numerical Brightness value of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.Cargo"/>.</remarks>
		public string Brightness
		{
			get => Cargo;
			set => Cargo = value;
		}
		/// <summary>Gets or sets the numerical Size of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.SpecialCargo"/>.</remarks>
		public string BackdropSize
		{
			get => SpecialCargo;
			set => SpecialCargo = value;
		}
		/// <summary>Legacy value, function not available in XWA.</summary>
		/// <remarks>Still tracked in case future hook enables functionality.</remarks>
		public short LegacyPermaDeathID { get; set; }
		#endregion
		#region arrdep
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start.</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less.</remarks>
		public bool ArrivesIn30Seconds => (ArrDepTriggers[0].Condition == (byte)Mission.Trigger.ConditionList.True 
			&& ArrDepTriggers[1].Condition == (byte)Mission.Trigger.ConditionList.True
			&& ArrDepTriggers[2].Condition == (byte)Mission.Trigger.ConditionList.True
			&& ArrDepTriggers[3].Condition == (byte)Mission.Trigger.ConditionList.True
			&& ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30);
		/// <summary>Gets the Arrival and Departure trigger array.</summary>
		/// <remarks>Use <see cref="ArrDepTriggerIndex"/> for indexes.</remarks>
		public Mission.Trigger[] ArrDepTriggers { get; } = new Mission.Trigger[6];
		/// <summary>Gets which <see cref="ArrDepTriggers"/> must be completed.</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}.</remarks>
		public bool[] ArrDepAndOr { get; } = new bool[4];
		/// <summary>Gets or sets the maximum number of minutes that can be added to the Arrival delay.</summary>
		public byte RandomArrivalDelayMinutes { get; set; }
		/// <summary>Gets or sets the maximum number of seconds that can be added to the Arrival delay.</summary>
		public byte RandomArrivalDelaySeconds { get; set; }
		/// <summary>Gets or sets the primary method of arrival.</summary>
		/// <remarks>When <b>1</b>, FlightGroup will attempt arrive via mothership, hyperspace when <b>0</b>. When <b>2</b>, hypers in to same region as mothership.</remarks>
		new public byte ArrivalMethod { get; set; }
		/// <summary>Gets or sets the primary method of departure.</summary>
		/// <remarks>When <b>1</b>, Flightgroup will attempt to depart via mothership, hyperspace when <b>0</b>. Unknown effect if <b>2</b>.</remarks>
		new public byte DepartureMethod { get; set; }
		#endregion
		/// <summary>Gets the FlightGroup objective commands.</summary>
		/// <remarks>Array is [Region, OrderIndex].</remarks>
		public Order[,] Orders { get; } = new Order[4, 4];
		/// <summary>Gets the FlightGroup-specific mission goals.</summary>
		/// <remarks>Array is Length = 8.</remarks>
		public Goal[] Goals { get; } = new Goal[8];

		/// <summary>Gets the FlightGroup start locations.</summary>
		/// <remarks>Array is Length = 4.</remarks>
		public Waypoint[] Waypoints { get; } = new Waypoint[4];
		#region Options
		/// <summary>Gets or sets the departure time in minutes from mission start.</summary>
		public byte DepartureClockMin { get; set; }
		/// <summary>Gets or sets the departure time in seconds from mission start.</summary>
		public byte DepartureClockSec { get; set; }
		/// <summary>Determines if the FlightGroup should share numbering across the <see cref="GlobalUnit"/>.</summary>
		public bool GlobalNumbering { get; set; }
		/// <summary>Gets or sets the defenses available to the FG.</summary>
		public CounterTypes Countermeasures { get; set; }
		/// <summary>Gets or sets the duration of death animation.</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class.</remarks>
		public byte ExplosionTime { get; set; }
		/// <summary>Gets or sets the second condition of FlightGroup upon creation.</summary>
		public byte Status2 { get; set; }
		/// <summary>Gets or sets the additional grouping assignment, can share craft numbering.</summary>
		public byte GlobalUnit { get; set; }
		/// <summary>Unknown effect.</summary>
		/// <remarks>Values are None, FavorRebels, EvenOnly, FavorImps, FavorRebsEven, FavorImpsEven</remarks>
		public byte Handicap { get; set; }
		/// <summary>Gets the array of alternate weapons the player can select.</summary>
		/// <remarks>Use the <see cref="LoadoutIndexer.Indexes"/> enumeration for indexes.</remarks>
		public LoadoutIndexer OptLoadout { get; private set; }
		/// <summary>Gets the array of alternate craft types the player can select.</summary>
		/// <remarks>Array is Length = 10.</remarks>
		public OptionalCraft[] OptCraft { get; } = new OptionalCraft[10];
		/// <summary>The alternate craft types the player can select by list.</summary>
		public OptionalCraftCategory OptCraftCategory = OptionalCraftCategory.None;

		/// <summary>Gets or sets the editor note primarily used to signifiy the pilot voice.</summary>
		/// <remarks>15 character limit.</remarks>
		public string PilotID
		{
			get => _pilotID;
			set => _pilotID = StringFunctions.GetTrimmed(value, 15);
		}
		/// <summary>For Backdrop <see cref="BaseFlightGroup.CraftType">CraftTypes</see>, is the backdrop image.</summary>
		/// <remarks>Zero denotes a light source.</remarks>
		public byte Backdrop { get; set; }
		#endregion

		/// <summary>Container for the optional craft settings.</summary>
		[Serializable]
		public struct OptionalCraft
		{
			/// <summary>The Craft Type.</summary>
			public byte CraftType { get; set; }
			/// <summary>The number of ships per Wave.</summary>
			public byte NumberOfCraft { get; set; }
			/// <summary>The number of waves available.</summary>
			public byte NumberOfWaves { get; set; }
		}
	}
}
