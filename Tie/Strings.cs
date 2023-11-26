/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7.5
 */

/* CHANGELOG
 * v5.7.5, 230116
 * [UPD #12] Trigger 0x18 now "have cannon subsystem disabled"
 * [UPD #12] TriggerType Misc now "Adjusted AI Skill"
 * [DEL #12] Misc array
 * [NEW #12] TriggerType Status and "All Craft"
 * [UPD #12] "No Lasers" to "No Turrets", everything after "Hyperdrive added" deleted
 * [DEL #12] All orders past Board to Repair
 * v4.0, 200809
 * [UPD] IFF numbers removed
 * [NEW] Ability to reset _craftType and _craftAbbrv to defaults [JB]
 * v3.1, 200703
 * [UPD] More details to OverrideShipList length exception
 * v3.0, 180903
 * [UPD] changed TIE to T/F [JB]
 * [UPD] updated Ratings [JB]
 * [UPD] changed Disabled status to No Shields [JB]
 * [UPD] changed a couple Trigger definitions [JB]
 * [NEW] added missing CraftWhen [JB]
 * [UPD] updated order descriptions [JB]
 * [UPD] "at least 1" amounts changed to "any"
 * v2.6, 151017
 * [FIX] Missing * from Medium Transport abbrv
 * [NEW YOGEME #10] ability to replace craft list
 * v2.5, 170107
 * [UPD] Added Decoy Beam [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for string lists used in TIE</summary>
	/// <remarks>All arrays return Clones to prevent editing</remarks>
	public abstract class Strings : BaseStrings
	{
		#region array definitions
		static readonly string[] _iff = { "Rebel",
								  "Imperial",
								  "Blue",
								  "Purple",
								  "Red",
								  "Purple"
							  };
		static readonly string[] _beam = { "None",
								   "Tractor beam",
								   "Jamming beam",
								   "(Decoy Beam)"
							   };
		static string[] _craftType = { "None",
										"X-Wing",
										"Y-Wing",
										"A-Wing",
										"B-Wing",
										"TIE Fighter",
										"TIE Interceptor",
										"TIE Bomber",
										"TIE Advanced",
										"TIE Defender",
										"*Shipyard",
										"*Repair Yard",
										"Missile Boat",
										"T-Wing",
										"Z-95 Headhunter",
										"R-41 Starchaser",
										"Assault Gunboat",
										"Lambda Shuttle",
										"Delta Escort Shuttle",
										"IPV Patrol Craft",
										"Scout Craft",
										"Delta Transport",
										"Gamma Assault Transport",
										"Beta Escort Transport",
										"Tug",
										"Combat Utility Vehicle",
										"Container A",
										"Container B",
										"Container C",
										"Container D",
										"Heavy Lifter",
										"*Gun Emplacement",
										"Bulk Freighter",
										"Cargo Ferry",
										"Modular Conveyor",
										"Container Transport",
										"*Medium Transport",
										"Murrian Transport",
										"Corellian Transport",
										"*Modified Strike Cruiser",
										"Corellian Corvette",
										"Modified Corvette",
										"Nebulon-B Frigate",
										"Modified Frigate",
										"C-3 Passenger Liner",
										"Carrack Cruiser",
										"Strike Cruiser",
										"Escort Carrier",
										"Dreadnaught",
										"MC80a Cruiser",
										"MC40a Light Cruiser",
										"Interdictor",
										"Victory Star Destroyer",
										"Imperator Star Destroyer",
										"*Executor Star Destroyer",
										"Container E",
										"Container F",
										"Container G",
										"Container H",
										"Container I",
										"Platform A",
										"Platform B",
										"Platform C",
										"Platform D",
										"Platform E",
										"Platform F",
										"Asteroid R&D Facility",
										"Ast. Laser Battery",
										"Ast. Warhead Battery",
										"X/7 Factory",
										"Satellite 1",
										"Satellite 2",
										"71",
										"72",
										"73",
										"Mine A",
										"Mine B",
										"Mine C",
										"77",
										"78",
										"Probe A",
										"Probe B",
										"81",
										"Nav Buoy 1",
										"Nav Buoy 2",
										"Asteroid Field 1",
										"Asteroid Field 2",
										"Planet"
									};
		static readonly string[] _defaultCraftType = (string[])_craftType.Clone();  //Backup to preserve original when overriding.
		static string[] _craftAbbrv = { "",
										 "X-W",
										 "Y-W",
										 "A-W",
										 "B-W",
										 "T/F",
										 "T/I",
										 "T/B",
										 "T/A",
										 "T/D",
										 "*SHPYD",
										 "*REPYD",
										 "MIS",
										 "T-W",
										 "Z-95",
										 "R-41",
										 "GUN",
										 "SHU",
										 "E/S",
										 "SPC",
										 "SCT",
										 "TRN",
										 "ATR",
										 "ETR",
										 "TUG",
										 "CUV",
										 "CN/A",
										 "CN/B",
										 "CN/C",
										 "CN/D",
										 "HLF",
										 "*GPLT",
										 "FRT",
										 "CARG",
										 "CNVYR",
										 "CTRNS",
										 "*MDTRN",
										 "MUTR",
										 "CORT",
										 "*M/SC",
										 "CRV",
										 "M/CRV",
										 "FRG",
										 "M/FRG",
										 "LINER",
										 "CRCK",
										 "STRKC",
										 "ESC",
										 "DREAD",
										 "CRS",
										 "CRL",
										 "INT",
										 "VSD",
										 "ISD",
										 "*SSD",
										 "CN/E",
										 "CN/F",
										 "CN/G",
										 "CN/H",
										 "CN/I",
										 "PLT/1",
										 "PLT/2",
										 "PLT/3",
										 "PLT/4",
										 "PLT/5",
										 "PLT/6",
										 "R&D FC",
										 "LAS BAT",
										 "W LNCHR",
										 "FAC/1",
										 "SAT 1",
										 "SAT/2",
										 "",
										 "",
										 "",
										 "MINE A",
										 "MINE B",
										 "MINE C",
										 "",
										 "",
										 "PROBE A",
										 "PROBE B",
										 "",
										 "NAV 1",
										 "NAV 2",
										 "Asteroid",
										 "Asteroid",
										 "Planet"
									 };
		static readonly string[] _defaultCraftAbbrv = (string[])_craftAbbrv.Clone();  //Backup to preserve original when overriding.
		static readonly string[] _rating = { "Novice",    //[JB] Fixed to mirror names in HD1W.TIE
									 "Officer",
									 "Veteran",
									 "Ace",
									 "Top Ace",
									 "Jedi (invul)"
								 };
		static readonly string[] _status = { "Normal",
									 "2x Warheads",
									 "1/2 Warheads",
									 "No Shields",
									 "1/2 Shields",
									 "No Turrets",
									 "No Hyperdrive",
									 "Shields 0%, charging",
									 "Shields added",
									 "Hyperdrive added"
								 };
		static readonly string[] _trigger = { "always (TRUE)",
									  "have arrived",
									  "be destroyed",
									  "be attacked",
									  "be captured",
									  "be inspected",
									  "finish being boarded",
									  "finish docking",
									  "be disabled",
									  "have survived (exist)",
									  "none (FALSE)",
									  "---",
									  "complete mission",
									  "complete primary mission",
									  "fail primary mission",
									  "complete secondary mission",
									  "fail secondary mission",
									  "complete bonus mission",
									  "fail bonus mission",
									  "be dropped off",
									  "be reinforced",
									  "have 0% shields",
									  "have 50% hull",
									  "run out of missiles",
									  "have cannon subsystem disabled"
								  };
		static readonly string[] _triggerType = { "none",
										  "Flight Group",
										  "Ship type",
										  "Ship class",
										  "Object type",
										  "IFF",
										  "Ship orders",
										  "Craft when",
										  "Global Group",
										  "Adjusted AI Skill",
										  "Status",
										  "All Craft"
									  };
		static readonly string[] _amount = { "100%",
									 "75%",
									 "50%",
									 "25%",
									 "any of",
									 "all but 1 of",
									 "all special craft in",
									 "all non-special craft in",
									 "all non-player craft in",
									 "player's craft in",
									 "100% of first wave",
									 "75% of first wave",
									 "50% of first wave",
									 "25% of first wave",
									 "any of first wave",
									 "all but 1 of first wave"
								 };
		static readonly string[] _goalAmount = { "100%",
									  "50%",
									  "at least 1 of",
									  "all but 1 of",
									  "all special craft in"
								  };
		static readonly string[] _orders = { "Hold Steady",
									 "Go Home",
									 "Circle",
									 "Circle and Evade",
									 "Rendezvous",
									 "Disabled",
									 "Awaiting Boarding",
									 "Attack targets",
									 "Attack escorts",
									 "Protect",
									 "Escort",
									 "Disable targets",
									 "Board to Give cargo",
									 "Board to Take cargo",
									 "Board to Exchange cargo",
									 "Board to Capture",
									 "Board to Destroy cargo",
									 "Pick up / Bag",
									 "Drop off",
									 "Wait",
									 "SS Wait",
									 "SS Patrol waypoints",
									 "SS Await Return",
									 "SS Launch",
									 "SS Protect",
									 "SS Wait / Protect",
									 "SS Patrol and Attack",
									 "SS Patrol and Disable",
									 "SS Hold Station",
									 "SS Go Home",
									 "SS Wait",
									 "SS Board",
									 "Board to Repair"
								 };
		static readonly string[] _craftWhen = { "Captured",
										"Inspected",
										"Finished being boarded",
										"Finished docking",
										"Disabled",
										"Attacked",
										"Any hull damage",
										"Special craft",
										"Non-special craft",
										"Player's craft",
										"Non-player craft"
									};
		static readonly string[] _abort = { "never",
									"0% shields",
									"75% systems (not SS)",
									"out of warheads",
									"50% hull",
									"attacked"
								};
		static readonly string[] _orderDesc = { "Stationary, 100% Systems, does not return fire. If not first order, craft flies home|Meaningless|Meaningless",
										"Fly to Mothership, or Hyperspace|Meaningless|Meaningless",
										"Circle through Waypoints.  Ignores targets, returns fire|# of loops|Meaningless",
										"Circles through Waypoints, evading attackers.  Ignores targets, returns fire|# of loops|Meaningless",
										"Fly to Rendezvous point and await docking. Ignores targets, returns fire|# of dockings|Meaningless",
										"Disabled|Meaningless|Meaningless",
										"Disabled, awaiting boarding|# of dockings|Meaningless",
										"Attacks targets (not for starships)|Component?|Meaningless",
										"Attacks escorts of targets|Meaningless|Meaningless",
										"Attacks craft that attack targets, ignores boarding craft|Meaningless|Meaningless",
										"Attacks craft that attack targets, including boarding craft|Meaningless|Attack Player",
										"Attacks to disable.  Warheads used to lower shields|Meaningless|Meaningless",
										"Boards targets (if stationary) to give cargo|Docking time (x5 sec)|# of dockings",
										"Boards targets (if stationary) to take cargo|Docking time (x5 sec)|# of dockings",
										"Boards targets (if stationary) to exchange cargo|Docking time (x5 sec)|# of dockings",
										"Boards targets (if stationary) to capture|Docking time (x5 sec)|# of dockings",
										"Boards targets (if stationary) to plant explosives. Target will explode when complete|Docking time (x5 sec)|# of dockings",
										"Dock or pickup target, carry for remainder of mission or until dropped|Docking time (x5 sec)|# of dockings",
										"Drops off designated Flight Group (disregards targets)|Deploy time? (x5 sec)|Flight Group #",
										"Waits for designated time before continuing. Returns fire|Wait time (x5 sec)|Meaningless",
										"Waits for designated time before continuing. Returns fire|Wait time (x5 sec)|Meaningless",
										"Circles through Waypoints. Attacks targets, returns fire|# of loops|Meaningless",
										"Wait for the return of all FGs with it as their Mothership. Attacks targets, returns fire|Meaningless|Meaningless",
										"Waits for the launch of all FGs with it as their Mothership. Attacks targets, returns fire|Meaningless|Meaningless",
										"Circles through Waypoints attacking craft that attack targets. Returns fire|Meaningless|Meaningless",
										"Circles through Waypoints attacking craft that attack targets. Returns fire|Meaningless|Meaningless",
										"Circles through Waypoints attacking targets. Returns fire|Meaningless|Meaningless",
										"Circles through Waypoints attacking targets to disable. Returns fire|Meaningless|Meaningless",
										"Waits for designated time before continuing. Does not return fire|Meaningless|Meaningless",
										"Fly to Mothership, or Hyperspace. Attacks targets, returns fire|Meaningless|Meaningless",
										"Waits for designated time before continuing. Does not return fire|Wait time (x5 sec)|Meaningless",
										"Boards targets (if stationary)|Docking time (x5 sec)|# of dockings",
										"Boards targets (if stationary) to repair systems|Docking time (x5 sec)|# of dockings"
									};
		static readonly string[] _formation = { "Vic",
										"Finger Four",
										"Line Astern",
										"Line Abreast",
										"Echelon Right",
										"Echelon Left",
										"Double Astern",
										"Diamond",
										"Stack",
										"High X",
										"Vic Abreast",
										"High Vic",
										"Reverse High Vic"
									};
		#endregion

		/// <summary>Replaces <see cref="CraftType"/> and <see cref="CraftAbbrv"/> with custom arrays, or restores defaults.</summary>
		/// <param name="craftTypes">Array of new craft types, or null to restore both defaults.</param>
		/// <param name="craftAbbrv">Array of new craft abbreviations, or null to restore both defaults.</param>
		/// <exception cref="ArgumentException">The <see cref="Array.Length"/> of the arrays do match the originals.</exception>
		public static void OverrideShipList(string[] craftTypes, string[] craftAbbrv)
		{
			if (craftTypes != null && craftAbbrv != null)
			{
				if (craftTypes.Length != _defaultCraftType.Length || craftAbbrv.Length != _defaultCraftAbbrv.Length)
					throw new ArgumentException("New arrays (Types " + craftTypes.Length + ", Abbrv " + craftAbbrv.Length + ") must match original length (" + _craftAbbrv.Length + ").");
				_craftType = craftTypes;
				_craftAbbrv = craftAbbrv;
			}
			else
			{
				_craftType = _defaultCraftType;
				_craftAbbrv = _defaultCraftAbbrv;
			}
		}

		/// <summary>Gets a copy of the default IFF Names</summary>
		/// <remarks>Array is Length = 6</remarks>
		public static string[] IFF => (string[])_iff.Clone();
		/// <summary>Gets a copy of the beam weapons for craft use</summary>
		/// <remarks>Array is Length = 3</remarks>
		public static string[] Beam => (string[])_beam.Clone();
		/// <summary>Gets a copy of the long names for ship types</summary>
		/// <remarks>Array is Length = 88</remarks>
		public static string[] CraftType => (string[])_craftType.Clone();
		/// <summary>Gets a copy of the short names for ship types</summary>
		/// <remarks>Array is Length = 88</remarks>
		public static string[] CraftAbbrv => (string[])_craftAbbrv.Clone();
		/// <summary>Gets a copy of the craft AI settings</summary>
		/// <remarks>Array is Length = 6</remarks>
		public static string[] Rating => (string[])_rating.Clone();
		/// <summary>Gets a copy of the Flight Group initial state parameters</summary>
		/// <remarks>Array is Length = 11</remarks>
		public static string[] Status => (string[])_status.Clone();
		/// <summary>Gets a copy of the Conditions required to complete trigger</summary>
		/// <remarks>Array is Length = 25</remarks>
		public static string[] Trigger => (string[])_trigger.Clone();
		/// <summary>Gets a copy of the Categories that the Trigger Parameter belongs to</summary>
		/// <remarks>Array is Length = 12</remarks>
		public static string[] VariableType => (string[])_triggerType.Clone();
		/// <summary>Gets a copy of the quantities of applicable conditions that must be met</summary>
		/// <remarks>Array is Length = 16</remarks>
		public static string[] Amount => (string[])_amount.Clone();
		/// <summary>Gets a copy of the quantities of applicable conditions that must be met for FlightGroup Goals</summary>
		/// <remarks>Array is Length = 5</remarks>
		public static string[] GoalAmount => (string[])_goalAmount.Clone();
		/// <summary>Gets a copy of the FlightGroup orders</summary>
		/// <remarks>Array is Length = 33</remarks>
		public static string[] Orders => (string[])_orders.Clone();
		/// <summary>Gets a copy of the craft behaviour to be used in triggers</summary>
		/// <remarks>Array is Length = 12</remarks>
		public static string[] CraftWhen => (string[])_craftWhen.Clone();
		/// <summary>Gets a copy of the individual craft abort conditions</summary>
		/// <remarks>Array is Length = 6</remarks>
		public static string[] Abort => (string[])_abort.Clone();
		/// <summary>Gets a copy of the descriptions of orders and variables</summary>
		/// <remarks>Array is Length = 33</remarks>
		public static string[] OrderDesc => (string[])_orderDesc.Clone();
		/// <summary>Gets a copy of the FlightGroup formations</summary>
		/// <remarks>Array is Length = 13, replaces BaseStrings.Formation</remarks>
		new public static string[] Formation => (string[])_formation.Clone();
	}
}
