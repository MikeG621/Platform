﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.0
 */

/* CHANGELOG
 * v6.0, 231027
 * [UPD] Role unk uncovered [YOGEME#91]
 * v5.8.1, 231014
 * [UPD] Added missing CraftWhen [YOGEME#90]
 * [UPD] Defined unknown Triggers [YOGEME#89]
 * v4.0, 200809
 * [UPD] RoleTeams updated [JB]
 * [NEW] Ability to reset _craftType and _craftAbbrv to defaults [JB]
 * v3.1, 200703
 * [UPD] More details to OverrideShipList length exception
 * [UPD] Orbit details
 * [UPD] Added missing craft entry to shiplist
 * v3.0, 180903
 * [NEW] RoleTeams [JB]
 * [UPD] more Radio players, numbered unused Officers, Energy Beam, various renames [JB]
 * [UPD] order docking updates [JB]
 * [UPD] "at least 1" amounts changed to "any"
 * [UPD] removed ? from "be delivered"
 * v2.7, 180509
 * [UPD] label for Escort Position
 * [ADD #1] TriggerType unknowns (via JeremyAnsel)
 * v2.6, 151017
 * [NEW YOGEME #10] ability to replace craft list
 * [UPD] missing * in abbrvs
 * v2.5, 170107
 * [UPD] added Unks [JB]
 * [UPD] Amounts [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [DEL] Logo, hangar
 */

using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for string lists used in XWA</summary>
	/// <remarks>All arrays return Clones to prevent editing</remarks>
	public abstract class Strings : BaseStrings
	{
		#region array declarations
		static readonly string[] _shadow = { "None",
											  "Right 3/4",
											  "Right Half", 
											  "Right Quarter",
											  "Left Quarter",
											  "Left Half",
											  "Left 3/4"
										  };
        static readonly string[] _roleTeams = { "Role Disabled",
                                       "Team 1",
                                       "Team 2",
                                       "Team 3",
                                       "Team 4",
                                       "Team 5",
                                       "Team 6",
                                       "Team 7",
                                       "Team 8",
                                       "All Teams",
                                       "All Other Teams",
                                       "All Enemy teams",
                                       "All Friendly teams",
                                       "IFF Rebel (hyper)",
                                       "IFF Imperial (hyper)"
                                    };

		static readonly string[] _roles = { "Command Ship",
									"Base",
									"Station",
									"Mission Critical Craft",
									"Convoy Craft",
									"Strike Craft",
									"Reload Craft",
									"Primary Target",
									"Secondary Target",
									"Tertiary Target",
									"Research Facility",  //[JB] Changed to correct string.
									"Facility",
									"HYP from Region 1",
									"HYP from Region 2",
									"HYP from Region 3",
									"HYP from Region 4",
									"HYP to Region 1",
									"HYP to Region 2",
									"HYP to Region 3",
									"HYP to Region 4",
									"HYP from Any Region",
									"Unknown"	//[JB] needed for 1B6M7FB, though the role is disabled
								};
		static readonly string[] _radio = { "None",
										"Team 1",
										"Team 2",
										"Team 3",
										"Team 4",
										"Team 5",
										"Team 6",
										"Team 7",
										"Team 8",
										"Player 1",
										"Player 2",
										"Player 3",
										"Player 4",
										"Player 5",
										"Player 6",
										"Player 7",
										"Player 8",
                                        "Player 9",   //[JB] Added up to Player 16
                                        "Player 10",
                                        "Player 11",
                                        "Player 12",
                                        "Player 13",
                                        "Player 14",
                                        "Player 15",
                                        "Player 16",
									};
		static readonly string[] _officer = { "Devers",
										"Kupalo",
										"Zaletta",
										"3",
										"4",
										"5",
										"6",
										"7",
										"Emkay"
									};
		static readonly string[] _iff = { "Rebel",
										"Imperial",
										"Blue",
										"Yellow",
										"Red",
										"Purple"
								  };
		static readonly string[] _beam = { "None",
									"Tractor beam",
									"Jamming beam",
									"Decoy beam",
                                    "(Energy beam)"
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
										"IRD Fighter",
										"Toscan Fighter",
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
										"Utility Tug",
										"Combat Utility Vehicle",
										"Container A",
										"Container B",
										"Container C",
										"Container D",
										"Heavy Lifter",
										"Mole Miner",
										"Bulk Freighter",
										"Cargo Ferry",
										"Modular Conveyor",
										"Container Transport",
										"Medium Transport",
										"Murrian Transport",
										"Corellian Transport",
										"Millenium Falcon",
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
										"Executor Star Destroyer",
										"Container E",
										"Container F",
										"Container G",
										"Container H",
										"Container I",
										"XQ1 Platform",
										"XQ2 Platform",
										"XQ3 Platform",
										"XQ4 Platform",
										"XQ5 Platform",
										"XQ6 Platform",
										"Asteroid R&D Facility",
										"Ast. Laser Battery",
										"Ast. Warhead Battery",
										"X/7 Factory",
										"Satellite 1",
										"Satellite 2",
										"Satellite 3",
										"*Satellite 4",
										"*Satellite 5",
										"Mine A",
										"Mine B",
										"Mine C",
										"Gun Emplacement",
										"*Mine 5",
										"Probe A",
										"Probe B",
										"*Probe 3",
										"Nav Buoy 1",
										"Nav Buoy 2",
										"Hyper Buoy",
										"Asteroid Field",
										"Planet",
										"Rendezvous Buoy",
										"Cargo Canister",
										"Shipyard",
										"Repair Yard",
										"*Modified Strike Cruiser",
										"Lancer Frigate",
										"Bulk Cruiser",
										"Assault Frigate",
										"Corellian Gunship",
										"Sentinel Landing Craft",
										"Gamma Assault Shuttle",
										"Maurader Corvette",
										"Star Galleon",
										"Imperial Research Ship",
										"Luxury Yacht 3000",
										"Ferryboat Liner",
										"Action VI Transport",
										"Mobquet Transport",
										"Xiytiar Transport",
										"Freight Transport/C",
										"Freight Transport/H",
										"Freight Transport/K",
										"YT-2000",
										"YT-2400",
										"Suprosa",
										"Skipray Blastboat",
										"T/e mk1",
										"T/e mk2",
										"T/e mk3",
										"T/e mk4",
										"T/e mk5",
										"Cloakshape Fighter",
										"Razor Fighter",
										"Planetary Fighter",
										"Supa Fighter",
										"Pinook Fighter",
										"Booster Pack",   //[JB] Removed asterisk, it will properly appear in missions.
										"Preybird Fighter",
										"*StarViper",
										"Firespray",
										"Pursuer",
										"Golan 1",
										"Golan 2",
										"Golan 3",
										"Derilyn Platform",
										"Sensor Array",
										"Comm Relay",
										"Space Colony 1",
										"Space Colony 2",
										"Space Colony 3",
										"Casino",
										"Cargo Facility 1",
										"Cargo Facility 2",
										"Asteroid Mining Plant",
										"Processing Plant",
										"Rebel Platform",
										"Imp. Research Center",
										"Family Base",
										"Family Repair Yard",
										"Pirate Shipyard",
										"Industrial Complex",
										"*Pirate Junkyard Base",
										"Escape Pod 1",
										"Pressure Tank",
										"Container J",
										"Container K",
										"Container L",
										"Container Hangar",
										"Large Emplacement",
										"Warhead Emplacement",
										"Prox Mine A",
										"Prox Mine B",
										"*Homing Mine",
										"Homing Mine B",
										"New Laser Battery",
										"New Ion Battery",
										"Cargo Freighter",
										"*Cargo Freighter 2",
										"*Cargo Freighter 3",
										"*Cargo Freighter 4",
										"*Cargo Freighter 5",
										"Cargo Tanker",
										"*Cargo Tanker 2",
										"*Cargo Tanker 3",
										"*Cargo Tanker 4",
										"*Cargo Tanker 5",
										"Escape Pod 2",
										"Rebel Pilot",  //[JB] Removed asterisks, pilots will properly appear in missions.
										"Imperial Pilot",
										"Civilian Pilot",
										"Space Trooper",
										"Zero-G Utility Suit",
										"Emkay",
										"Astromech",
										"Worker Droid",
										"Backdrop",
										"*Forest Moon of Endor",
										"*Endor",
										"*Sullust",
										"*Bothuwai",
										"*Kothlis",
										"*Hoth",
										"*DS II backdrop",
										"*Nar Shadda",
										"*Planet",
										"*Planet",
										"*Planet",
										"*Planet",
										"*Planet",
										"*Planet",
										"*Moon",
										"*Moon",
										"*Moon",
										"*Moon",
										"*Moon",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Sun",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*Backdrop",
										"*DS II",
										"MC80 Liberty-class",
										"VSD II",
										"ISD II",
										"*Planet",
										"*Planet"
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
										"IRD",
										"TOS",
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
										"MINER",
										"FRT",
										"CARG",
										"CNVYR",
										"CTRNS",
										"MTRNS",
										"MUTR",
										"CORT",
										"FALC",
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
										"ESD",
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
										"L BAT",
										"W LNCHR",
										"FAC/1",
										"SAT/1",
										"SAT/2",
										"SAT/3",
										"*SAT/4",
										"*SAT/5",
										"MN/A",
										"MN/B",
										"MN/C",
										"GPLT",
										"*MN/5",
										"PRB/A",
										"PRB/B",
										"*PRB/C",
										"NAV/1",
										"NAV/2",
										"HYP",
										"AST",
										"PLNT",
										"RDV",
										"C/C",
										"SHPYD",
										"REPYD",
										"*M/SC",
										"L/FRG",
										"B/CRS",
										"A/FRG",
										"GSP",
										"L/C",
										"A/S",
										"M-CRV",
										"STARG",
										"SRS",
										"LY3K",
										"F/L",
										"ActVI",
										"MOB",
										"Xiy/T",
										"Frt/C",
										"Frt/H",
										"Frt/K",
										"YT2K",
										"YT24",
										"S/M",
										"S/B",
										"T/e1",
										"T/e2",
										"T/e3",
										"T/e4",
										"T/e5",
										"Clk/F",
										"Rzr/F",
										"Plt/F",
										"Sup/F",
										"Pnk/F",
										"B/R",
										"Pry/F",
										"*Xiz/F",
										"FRS",
										"PES",
										"G/1",
										"G/2",
										"G/3",
										"D/P",
										"S/ARY",
										"C/RLY",
										"SC/1",
										"SC/2",
										"SC/3",
										"CAS",
										"C/F1",
										"C/F2",
										"AMU",
										"PP",
										"BASE",
										"IRC",
										"AZZ",
										"RY/2",
										"P/SY",
										"FAC/2",
										"*JUNKYD",
										"E-POD",
										"Pr/Tk",
										"CN/J",
										"CN/K",
										"CN/L",
										"C/HGR",
										"G/P",
										"GW/P",
										"PM/1",
										"PM/2",
										"*H/MN1",
										"H/MN2",
										"L/B",
										"I/B",
										"CF",
										"*CF2",
										"*CF3",
										"*CF4",
										"*CF5",
										"CT",
										"*CT2",
										"*CT3",
										"*CT4",
										"*CT5",
										"E-POD/2",
										"Reb Pilot",
										"Imp Pilot",
										"Civ Pilot",
										"ZG/S",
										"ZG/U",
										"MK-09",
										"R2-D2",
										"Droid",
										"B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"*B/Drop",
										"WCRS",
										"VSDII",
										"ISDII",
										"*B/Drop",
										"*B/Drop"
									 };
		static readonly string[] _defaultCraftAbbrv = (string[])_craftAbbrv.Clone();  //Backup to preserve original when overriding.
		static readonly string[] _rating = { "Novice",
									"Officer",
									"Veteran",
									"Ace",
									"Top Ace",
									"Super Ace"
								 };
		static readonly string[] _status = { "Normal",
									"2x Warheads",
									"1/2 Warheads",
									"No Shields",
									"1/2 Shields",
									"No Lasers",
									"No Hyperdrive",
									"Shields 0%, charging",
									"Shields added (200%)",
									"Hyperdrive added",
									"2x Countermeasures",
									"1/2 Countermeasures",
									"(200% Shields)",
									"Shields 50%, charging",
									"(No Lasers)",
									"Engines Damaged",
									"Shields + Hyperdrive added",
									"All Systems Damaged",
									"200% Shields",
									"(50% Shields)",
									"Invincible",
									"Infinite Warheads",
									"No Escape Pods/Ejections",
									"No Cargo Pods",
									"Not Inspected",
									"Inspected",
									"Identified",
									"Limited Targetability"
								 };
		static readonly string[] _trigger = { "always (TRUE)",
										"have arrived",
										"be destroyed",
										"be attacked",
										"be captured",
										"be inspected",
										"finish being boarded",    //[JB] was "be boarded"
										"finish docking",          //[JB] was "be docked"
										"be disabled",
										"exist",
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
										"Unknown (arrive?)",
										"be dropped off",
										"be unharmed?",
										"NOT be disabled",
										"NOT be picked up",
										"be destroyed, not inspected",
										"begin being boarded",       //[JB] was "be docked with"
										"NOT being boarded",
										"begin docking",            //[JB] was "begin boarding"
										"NOT begin docking",
										"have 50% shields",
										"have 25% shields",
										"have 75% hull",
										"have 25% hull",
										"(always failed)",
										"be picked up for Team",
										"Unknown",
										"be all Player Craft",
										"reinforced by AI?",
										"come and go",
										"be picked up",
										"withdraw",
										"be carried away",
										"have arrived in Region",
										"have departed Region",
										"be nearby",
										"NOT be nearby",
										"all captured",
										"defect to",
										"be in convoy",
										"be delivered",
										"all disabled",
										"be shown (message)",
										"be identified",
										"NOT be identified",
										"exist?"
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
											"Rating",
											"Craft with status",
											"All",
											"Team",
											"Player #",
											"After delay",
											"All Flight Groups except",
											"All ship types except",
											"All ship classes except",
											"All object types except",
											"All IFFs except",
											"All Global Groups except",
											"All teams except",
											"All players except #",
											"Global Unit",
											"All Global Units except",
											"Global Cargo",
											"All Global Cargos except",
											"Message #",
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
									"all but 1 of first wave",
									"66%",
									"33%",
									"each craft",
									"each special craft",
									"???",
									"???",
									"???"
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
									"Board to Repair",
									"Hold Station",
									"Hold Steady",
									"SS Hold Station",
									"Self Destruct",
									"Kamikaze",
									"(Orbit)",
									"(Release Carried Cargo)",
									"Deliver/Drop Off",
									"Unknown",
									"(Attack)",
									"Load Objects",
									"Sit and Fire",
									"Repair Self",
									"Defect",
									"Self Capture/Surrender",
									"*Make",
									"Beacon",
									"Hyper to Region",
									"*Relaunch",
									"Transfer Cargo",
									"Inspect Targets",
									"*Await Assembly",
									"*Await Disassembly",
									"*Construct Train",
									"Park at",
									"*Board to Defuse",
									"Start Over",
									"*Take Apart Train",
									"Work On",
									"(Dock to Load)",
									"Follow Targets",
									"Home In"
								 };
		static readonly string[] _craftWhen = { "Captured",
										"Inspected",
										"Finished being boarded",
										"Finished docking",
										"Disabled",
										"Attacked",
										"Any hull damage",  //[JB] was "0% shields?", confirmed actual effect
										"Special craft",
										"Non-special craft",
										"Player's craft",
										"Non-player craft",
										"",
										"NOT Disabled"
									};
		static readonly string[] _abort = { "never",
									"0% shields",
									"",
									"out of warheads",
									"50% hull",
									"attacked",
									"50% shields",
									"25% shields",
									"75% hull",
									"25% hull"
								};
		static readonly string[] _orderDesc = { "Stationary, 100% Systems, does not return fire. If not first order, craft flies home|Meaningless|Meaningless|Meaningless",
										"Fly to Mothership, or Hyperspace|Meaningless|Meaningless|Meaningless",
										"Circle through Waypoints.  Ignores targets, returns fire|# of loops|Meaningless|Meaningless",
										"Circles through Waypoints, evading attackers.  Ignores targets, returns fire|# of loops|Meaningless|Meaningless",
										"Fly to Rendezvous point and await docking. Ignores targets, returns fire|# of dockings|Meaningless|Meaningless",
										"Disabled|Meaningless|Meaningless|Meaningless",
										"Disabled, awaiting boarding|# of dockings|Meaningless|Meaningless",
										"Attacks targets (not for starships)|Component|Meaningless|Meaningless",
										"Attacks escorts of targets|Meaningless|Meaningless|Meaningless",
										"Attacks craft that attack targets, ignores boarding craft|Meaningless|Meaningless|Meaningless",
										"Attacks craft that attack targets, including boarding craft|Position|Attack Player|Meaningless",
										"Attacks to disable.  Warheads used to lower shields|Meaningless|Meaningless|Meaningless",
										"Boards targets (if stationary) to give cargo|Docking time|# of dockings|Meaningless",
										"Boards targets (if stationary) to take cargo|Docking time|# of dockings|Meaningless",
										"Boards targets (if stationary) to exchange cargo|Docking time|# of dockings|Meaningless",
										"Boards targets (if stationary) to capture|Docking time|# of dockings|Meaningless",
										"Boards targets (if stationary) to plant explosives. Target will explode when complete|Docking time|# of dockings|Meaningless",
										"Dock or pickup target, carry for remainder of mission or until dropped|Docking time|# of dockings|Meaningless",  //[JB] Changed Var2, was Meaningless
										"Drops off designated Flight Group (disregards targets)|Deploy time|Flight Group #|Meaningless",
										"Waits for designated time before continuing. Returns fire|Wait time|Meaningless|Meaningless",
										"Waits for designated time before continuing. Returns fire|Wait time|Meaningless|Meaningless",
										"Circles through Waypoints. Attacks targets, returns fire|# of loops|Meaningless|Meaningless",
										"Wait for the return of all FGs with it as their Mothership. Attacks targets, returns fire|Meaningless|Meaningless|Meaningless",
										"Waits for the launch of all FGs with it as their Mothership. Attacks targets, returns fire|Meaningless|Meaningless|Meaningless",
										"Circles through Waypoints attacking craft that attack targets. Returns fire|Meaningless|Meaningless|Meaningless",
										"Circles through Waypoints attacking craft that attack targets. Returns fire|Meaningless|Meaningless|Meaningless",
										"Circles through Waypoints attacking targets. Returns fire|Meaningless|Meaningless|Meaningless",
										"Circles through Waypoints attacking targets to disable. Returns fire|Meaningless|Meaningless|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Fly to Mothership, or Hyperspace. Attacks targets, returns fire|Meaningless|Meaningless|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Boards targets (if stationary)|Docking time|# of dockings|Meaningless",
										"Boards targets (if stationary) to repair systems|Docking time|# of dockings|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Craft destroys self|Delay time|Meaningless|Meaningless",
										"Craft fires at and rams target|Meaningless|Meaningless|Meaningless",
										"Orbits around origin, 100% Systems, does not return fire|# of loops|Meaningless|Meaningless",
										"Stationary, 100% Systems, does not return fire|Meaningless|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Objects|Meaningless|Meaningless",
										"Unknown|Use Warheads|Meaningless|Meaningless",
										"Unknown|Repair Time|Meaningless|Meaningless",
										"Change ownership of craft|IFF|Team|Meaningless",
										"Unknown|IFF|Meaningless|Meaningless",
										"Unknown|Craft type|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Region|Wait|# of loops",
										"Unknown|Warhead Type|Meaningless|Meaningless",
										"Unknown|# of loops|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|starting FG|from Global Group|# of dockings",
										"Unknown|Wait Time|Waypoint #|Meaningless",
										"Unknown|Docking Time|Meaningless|Meaningless",
										"Unknown|# of loops|Meaningless|Meaningless",
										"Unknown|from FG|Meaningless|Meaningless",
										"Unknown|Work Time|Component|# of loops",
										"Unknown|Meaningless|Meaningless|Meaningless",
										"Unknown|# of loops|Meaningless|Meaningless",
										"Unknown|# of loops|Direction|Meaningless"
									};
		#endregion

		/// <summary>Replaces <see cref="CraftType"/> and <see cref="CraftAbbrv"/> with custom arrays, or restores defaults.</summary>
		/// <param name="craftTypes">Array of new craft types, or null to restore both defaults.</param>
		/// <param name="craftAbbrv">Array of new craft abbreviations, or null to restore both defaults.</param>
		/// <exception cref="ArgumentException">The <see cref="Array.Length"/> of the arrays are shorter than the originals.</exception>
		public static void OverrideShipList(string[] craftTypes, string[] craftAbbrv)
		{
			if (craftTypes != null && craftAbbrv != null)
			{
				//NOTE: Craft pack mods may extend the list beyond the original length, so only check shorter length instead of unequal.
				if (craftTypes.Length < _defaultCraftType.Length || craftAbbrv.Length < _defaultCraftAbbrv.Length)
					throw new ArgumentException("New arrays (Types " + craftTypes.Length + ", Abbrv " + craftAbbrv.Length + ") must not be shorter than original length (" + _craftAbbrv.Length + ").");
				_craftType = craftTypes;
				_craftAbbrv = craftAbbrv;
			}
			else
			{
				_craftType = _defaultCraftType;
				_craftAbbrv = _defaultCraftAbbrv;
			}
		}

		/// <summary>Gets a copy of the shadows to be applied to a backdrop</summary>
		/// <remarks>Array is Length = 7</remarks>
		public static string[] Shadow { get { return (string[])_shadow.Clone(); } }
        /// <summary>Gets a copy of the default team name entries that craft roles can be applied to.</summary>
        /// <remarks>Array is Length = 15</remarks>
        public static string[] RoleTeams { get { return (string[])_roleTeams.Clone(); } }
        /// <summary>Gets a copy of the craft roles used for specialized in-flight messages</summary>
		/// <remarks>Array is Length = 22</remarks>
		public static string[] Roles { get { return (string[])_roles.Clone(); } }
		/// <summary>Gets a copy of the radio channels the craft uses</summary>
		/// <remarks>Array is length = 25</remarks>
		public static string[] Radio { get { return (string[])_radio.Clone(); } }
		/// <summary>Gets a copy of the voices of the mission control officer</summary>
		/// <remarks>Array is Length = 9</remarks>
		public static string[] Officer { get { return (string[])_officer.Clone(); } }
		/// <summary>Gets a copy of the default IFF Names</summary>
		/// <remarks>Array is Length = 6</remarks>
		public static string[] IFF { get { return (string[])_iff.Clone(); } }
		/// <summary>Gets a copy of the beam weapons for craft use</summary>
		/// <remarks>Array is Length = 5</remarks>
		public static string[] Beam { get { return (string[])_beam.Clone(); } }
		/// <summary>Gets a copy of the long names for ship type</summary>
		/// <remarks>Array is Length = 232</remarks>
		public static string[] CraftType { get { return (string[])_craftType.Clone(); } }
		/// <summary>Gets a copy of the short names for ship type</summary>
		/// <remarks>Array is Length = 232</remarks>
		public static string[] CraftAbbrv { get { return (string[])_craftAbbrv.Clone(); } }
		/// <summary>Gets a copy of the craft AI settings</summary>
		/// <remarks>Array is Length = 6</remarks>
		public static string[] Rating { get { return (string[])_rating.Clone(); } }
		/// <summary>Gets a copy of the FlightGroup initial state parameters</summary>
		/// <remarks>Array is Length = 29</remarks>
		public static string[] Status { get { return (string[])_status.Clone(); } }
		/// <summary>Gets a copy of the conditions required to complete trigger</summary>
		/// <remarks>Array is Length = 60</remarks>
		public static string[] Trigger { get { return (string[])_trigger.Clone(); } }
		/// <summary>Gets a copy of the categories that the Trigger Parameter belongs to</summary>
		/// <remarks>Array is Length = 28</remarks>
		public static string[] VariableType { get { return (string[])_triggerType.Clone(); } }
		/// <summary>Gets a copy of the quantities of applicable conditions that must be met</summary>
		/// <remarks>Array is Length = 23</remarks>
		public static string[] Amount { get { return (string[])_amount.Clone(); } }
		/// <summary>Gets a copy of the FlightGroup orders</summary>
		/// <remarks>Array is Length = 65</remarks>
		public static string[] Orders { get { return (string[])_orders.Clone(); } }
		/// <summary>Gets a copy of the craft behaviours to be used in triggers</summary>
		/// <remarks>Array is Length = 13</remarks>
		public static string[] CraftWhen { get { return (string[])_craftWhen.Clone(); } }
		/// <summary>Gets a copy of the individual craft abort conditions</summary>
		/// <remarks>Array is Length = 10</remarks>
		public static string[] Abort { get { return (string[])_abort.Clone(); } }
		/// <summary>Gets a copy of the descriptions of orders and variables</summary>
		/// <remarks>Array is Length = 65</remarks>
		public static string[] OrderDesc { get { return (string[])_orderDesc.Clone(); } }
	}
}
