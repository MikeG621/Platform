﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [UPD] OrderDesc fixed
 * v5.7, 220127
 * [FIX] Formation [JB]
 * v4.0, 200809
 * [UPD] FormationObject updated [JB]
 * [NEW] Ability to reset _craftType and _craftAbbrv to defaults [JB]
 * v3.1, 200703
 * [NEW] OverrideShipList
 * v3.0, 180309
 * [NEW] created [JB]
 * [UPD] "at least 1" amount changed to "any"
 */

using System;

namespace Idmr.Platform.Xwing
{
	/// <summary>Object for string lists used in XWING95.</summary>
	/// <remarks>All arrays return Clones to prevent editing.</remarks>
	public abstract class Strings
	{
		#region array definitions
		static readonly string[] _iff = { "Default",
								  "Rebel",
								  "Imperial",
								  "Neutral (Blue)",
								  "4 (Unknown/Unused)"
							  };

        static readonly string[] _color = { "Red (TIE - none)",
									"Gold (TIE - Red)",
									"Blue (TIE - Gold)",
									"Green (TIE - Blue)"
								};
        
        static string[] _craftType = { "None",
										"X-Wing",
										"Y-Wing",
										"A-Wing",
										"TIE Fighter",
										"TIE Interceptor",
										"TIE Bomber",
										"Assault Gunboat",
										"Transport",
										"Shuttle",
										"Tug",
										"Container",
										"Freighter",
										"Calamari Cruiser",
										"Nebulon-B Frigate",
										"Corellian Corvette",
										"Star Destroyer",
										"TIE Advanced",
										"B-Wing"
									 };
		static readonly string[] _defaultCraftType = (string[])_craftType.Clone();  //Backup to preserve original when overriding.
		static string[] _craftAbbrv = { "None",
										"X-W",
										"Y-W",
										"A-W",
										"T/F",
										"T/I",
										"T/B",
										"GUN",
										"TRN",
										"SHU",
										"TUG",
										"CON",
										"FRT",
										"CRS",
										"FRG",
										"CRV",
										"STD",
										"T/A",
										"B-W"
									  };
		static readonly string[] _defaultCraftAbbrv = (string[])_craftAbbrv.Clone();  //Backup to preserve original when overriding.
		static readonly string[] _objectType = {   "None",
										  "Mine1",  //[JB] IMPORTANT! Objects begin at index 18 in game.
										  "Mine2",
										  "Mine3",
										  "Mine4",
										  "Satellite",
										  "Nav Buoy",
										  "Probe",
										  "B-wing (BRF)",
										  "Asteroid1",
										  "Asteroid2",
										  "Asteroid3",
										  "Asteroid4",
										  "Asteroid5",
										  "Asteroid6",
										  "Asteroid7",
										  "Asteroid8",
										  "Rock World",
										  "Gray Ring World",
										  "Gray World",
										  "Brown World",
										  "Gray World II",
										  "Planet & Moon",
										  "Gray Crescent",
										  "Orange Crescent 1",
										  "Orange Crescent 2",
										  "Orange Crescent 3",
										  "Orange Crescent 4",
										  "Orange Crescent 5",
										  "Orange Crescent 6",
										  "Orange Crescent 7",
										  "Orange Crescent 8",
										  "Death Star",
										  "50",
										  "51",
										  "52",
										  "53",
										  "54",
										  "55",
										  "56",
										  "57",
										  "*Training Platform 1",  //58 (0x3A)
										  "*Training Platform 2",
										  "*Training Platform 3",
										  "*Training Platform 4",
										  "*Training Platform 5",
										  "*Training Platform 6",
										  "*Training Platform 7",
										  "*Training Platform 8",
										  "*Training Platform 9",
										  "*Training Platform 10",
										  "*Training Platform 11",
										  "*Training Platform 12",
										  "70",
										  "71",
										  "72",
										  "73",
										  "74",
										  "75"
									  };


        /*
         * Mapping gun turrets around the gates on Training Platform.
         * (assuming a "front-facing" platform, player and platform guns facing each other)
         * This chart is mapped to the checkboxes displayed in the editor.
         *  Bits  5 6   Rear gate (left and right gun)
         *  Bits  3 4   Middle gate (left and right gun)
         *  Bits  1 2   Front gate (left and right gun)
         *
         * The strings here correspond to the bitfield (reading the string one character at a time, from left to right) indicating which checkboxes correspond to the bit values.  Note that each platform's bitfield mapping is different, some differences more subtle than others.  That's how the game is.
         * Example:
         *   563412
         *   Bit 1 maps to checkmark 5 (rear left), Bit 2 maps chk 6 (rear right)
         *   Bit 3 = chk 3 (mid left), Bit 4 = chk 4 (mid right)
         *   Bit 5 = chk 1 (front left), Bit 6 = chk 2 (front right)
         */
        static readonly string[] _platformBitfield = { "",  //Training Platform 1   //ID:58 (0x3A)  does not have guns
                                    "563412",  //0x3B
                                    "214365",  //0x3C
                                    "214365",  //0x3D
                                    "345612",  //0x3E
                                    "216543",  //0x3F
                                    "216543",  //0x40
                                    "435621",  //0x41
                                    "341265",  //0x42
                                    "341265",  //0x43
                                    "216534",  //0x44
                                    "216543"   //0x45	
                                            };

		static readonly string[] _rating = { "Novice",
									 "Officer",
									 "Veteran",
									 "Ace",
									 "Top Ace"
								 };
		static readonly string[] _status = { "Normal",
									 "No Warheads",
									 "1/2 Warheads",
									 "No Shields",
									 "1/2 Shields",
									 "5",
									 "6",
									 "7",
									 "8",
									 "9"
								 };
		static readonly string[] _trigger = { "always (TRUE)",
									  "have arrived",
									  "be destroyed",
									  "be attacked",
									  "be captured",
									  "be identified",
									  "be disabled",
								  };

		static readonly string[] _goalAmount = { "100%",  //[JB] Remove?
									  "50%",
									  "any of",
									  "all but 1 of",
									  "all special craft in"
								  };
		static readonly string[] _orders = { "Hold Steady",
									 "Go Home",
									 "Circle and Ignore",
									 "Fly Once and Ignore",
									 "Circle and Evade",
									 "Fly Once and Evade",
									 "Close Escort",
									 "Loose Escort",
									 "Attack Escorts",
									 "Attack Targets",
									 "Attack Enemies",
									 "Rendezvous",
									 "Disabled",
									 "Board to Give cargo",
									 "Board to Take cargo",
									 "Board to Exchange cargo",
									 "Board to Capture",
									 "Board to Destroy",
									 "Disable Targets",
									 "Disable Enemies",
									 "Attack Transports",
									 "Attack Freighters",
									 "Attack Starships",
									 "Disable Transports",
									 "Disable Freighters",
									 "Disable Starships",
									 "SS Hold Position",
									 "SS Fly Once",
									 "SS Circle",
									 "SS Await Return",
									 "SS Await Launch",
									 "SS Await Boarding",
									 "Wait for Arrival of Targets"
								 };

		static readonly string[] _objective = { "None",
										 "100% be destroyed",
										 "100% must complete mission",
										 "100% be captured",
										 "100% be boarded",
										 "special craft be destroyed",
										 "special craft complete mission",
										 "special craft be captured",
										 "special craft be boarded",
										 "50% be destroyed",
										 "50% complete mission",
										 "50% be captured",
										 "50% be boarded",
										 "100% be identified",
										 "special craft be identified",
										 "50% be identified",
										 "Arrive"
									 };

        static readonly string[] _objectObjective = { "None",
										 "100% Destroyed",
										 "100% Survived"
									 };
        

		static readonly string[] _orderDesc = { "Stationary, 100% Systems, does not return fire. If not first order, craft flies home.",
										"Flies to Mothership, or Hyperspace.",
										"Circle through Waypoints.  Ignores targets, returns fire.",
										"Flies through Waypoints once.  Ignores targets, returns fire.",
										"Circles through Waypoints, evading attackers.  Ignores targets, returns fire.",
										"Flies through Waypoints once, evading attackers.  Ignores targets, returns fire.",
										"Attacks craft that attack targets, remaining close.",
										"Attacks craft that attack targets, ignores boarding craft.",
										"Attacks escorts of targets.",
										"Attacks targets.",
										"Attacks enemy Fighters, TRN, and SHU.",
										"Flies to Rendezvous point and await docking. Ignores targets, returns fire.",
										"Disabled.",
										"Boards targets (if stationary) to give cargo.",
										"Boards targets (if stationary) to take cargo.",
										"Boards targets (if stationary) to exchange cargo.",
										"Boards targets (if stationary) to capture.",
										"Boards targets (if stationary) to plant explosives. Target will explode when complete.",
										"Attacks to disable.  Warheads used to lower shields.",
										"Attacks enemy Fighters, TRN, and SHU to disable.",
										"Attacks enemy transport craft.",
										"Attacks enemy freighter craft and CRVs.",
										"Attacks enemy Starships.",
										"Attacks enemy transport craft to disable.",
										"Attacks enemy freighter craft and CRVs to disable.",
										"Attacks enemy Starships to disable.",
										"Stationary, 100% Systems, returns fire.",
										"Flies through Waypoints once attacking targets. Returns fire.",
										"Circles through Waypoints attacking targets. Returns fire.",
										"Waits for the return of all FGs with it as their Mothership. Attacks targets, returns fire.",
										"Waits for the launch of all FGs with it as their Mothership. Attacks targets, returns fire.",
										"Disabled, awaiting boarding.",
										"Waits for arrival of targets. Returns fire."
									};
		static readonly string[] _formation = { "Double Vic",
										"Finger Four",
										"Line Astern",
										"Line Abreast",
										"Echelon Right",
										"Echelon Left",
										"Double Astern",
										"Diamond",
										"Stacked",
										"Spread",
										"Undefined 1",	// Hi-Lo?
										"Undefined 2",	// Spiral?
									};
		static readonly string[] _formationObject = { "Floor (X-Y)",
										"Side (Y-Z)",
										"Front (X-Z)",
										"Scattered (undefined)"
									};


		static readonly string[] _missionLocation = { "Deep Space",
										"Death Star Surface"
									};

		static readonly string[] _endEvents = {"Capture Possible",
									"Rescued/Clear Laser Tower",
									"2",
									"3",
									"4",
									"Hit Exhaust Port"
									 };
        
        static readonly string[] _briefingUIElement = { "Title",
                                               "Text",
                                               "Unused1",
                                               "Unused2",
                                               "Map"
                                             };
        
        static readonly string[] _orderValue = {      "0   (100%)", //Interpreted as either docking time or throttle, depending on order.
                                             "1   (20%)",
                                             "2   (30%)",
                                             "3   (40%)",
                                             "4   (50%)",
                                             "5   (60%)",
                                             "6   (70%)",
                                             "7   (80%)",
                                             "8   (90%)",
                                             "9   (100%)",
                                             "10   (0%)"
                                          };
		#endregion

		/// <summary>Title used for briefing hint pages.</summary>
		public static readonly string BriefingPageHintTitle = ">MISSION COMPLETION HINTS";
		/// <summary>Caption used for briefing hint pages.</summary>
		public static readonly string BriefingPageHintCaption = "$>STRATEGY AND TACTICS$>FOR COMPLETING THIS MISSION$>ARE AVAILABLE.$$$>DO NOT READ THIS IF YOU WISH TO$>DISCOVER THESE FOR YOURSELF!$$$>CLICK ON THE BLACK PAGE NUMBER$>BOX TO SEE THE HINT(S).";

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

		/// <summary>Gets a copy of the default IFF Names.</summary>
		/// <remarks>Array is Length = 5.</remarks>
		public static string[] IFF => (string[])_iff.Clone();
		/// <summary>Gets a copy of FlightGroup craft markings.</summary>
		/// <remarks>Array has a Length of 4.</remarks>
		public static string[] Color => (string[])_color.Clone();
		/// <summary>Gets a copy of the long names for ship types.</summary>
		/// <remarks>Array is Length = 88.</remarks>
		public static string[] CraftType => (string[])_craftType.Clone();
		/// <summary>Gets a copy of the long names for object types.  NOTE: X-wing distinguishes between craft FGs and object FGs as separate entities.</summary>
		/// <remarks>Array is Length = 33.</remarks>
		public static string[] ObjectType => (string[])_objectType.Clone();
		/// <summary>Gets a copy of the short names for ship types.</summary>
		/// <remarks>Array is Length = 88.</remarks>
		public static string[] CraftAbbrv => (string[])_craftAbbrv.Clone();
		/// <summary>Gets a copy of the craft AI settings.</summary>
		/// <remarks>Array is Length = 6.</remarks>
		public static string[] Rating => (string[])_rating.Clone();
		/// <summary>Gets a copy of the Flight Group initial state parameters.</summary>
		/// <remarks>Array is Length = 21.</remarks>
		public static string[] Status => (string[])_status.Clone();
		/// <summary>Gets a copy of the Conditions required to complete trigger.</summary>
		/// <remarks>Array is Length = 7.</remarks>
		public static string[] Trigger => (string[])_trigger.Clone();
		/// <summary>Gets a copy of the quantities of applicable conditions that must be met for FlightGroup Goals.</summary>
		/// <remarks>Array is Length = 5.</remarks>
		public static string[] GoalAmount => (string[])_goalAmount.Clone();
		/// <summary>Gets a copy of the FlightGroup orders.</summary>
		/// <remarks>Array is Length = 40.</remarks>
		public static string[] Orders => (string[])_orders.Clone();
		/// <summary>Gets a copy of the descriptions of orders and variables.</summary>
		/// <remarks>Array is Length = 40.</remarks>
		public static string[] OrderDesc => (string[])_orderDesc.Clone();
		/// <summary>Gets a copy of the FlightGroup formations.</summary>
		/// <remarks>Array is Length = 13, replaces BaseStrings.Formation.</remarks>
		public static string[] Formation => (string[])_formation.Clone();
		/// <summary>Gets a copy of the Object formations.</summary>
		/// <remarks>Array is Length = 4.</remarks>
		public static string[] FormationObject => (string[])_formationObject.Clone();
		/// <summary>Gets a copy of the Mission Location list.</summary>
		/// <remarks>Array is Length = 2.</remarks>
		public static string[] MissionLocation => (string[])_missionLocation.Clone();
		/// <summary>Gets a copy of the craft Objective list.</summary>
		/// <remarks>Array is Length = 16.</remarks>
		public static string[] Objective => (string[])_objective.Clone();
		/// <summary>Gets a copy of the object Objective list.</summary>
		/// <remarks>Array is Length = 3.</remarks>
		public static string[] ObjectObjective => (string[])_objectObjective.Clone();
		/// <summary>Gets a copy of the EndEvents list.</summary>
		/// <remarks>Array is Length = 6.</remarks>
		public static string[] EndEvents => (string[])_endEvents.Clone();
		/// <summary>Gets a copy of the PlatformBitField list.</summary>
		/// <remarks>Array is Length = 12.</remarks>
		public static string[] PlatformBitField => (string[])_platformBitfield.Clone();

		/// <summary>Gets a copy of the briefing UI element names.</summary>
		/// <remarks>Array is Length = 5.</remarks>
		public static string[] BriefingUIElement => (string[])_briefingUIElement.Clone();

		/// <summary>Gets a copy of the order parameter (docktime/throttle) Throttle percentage description.</summary>
		/// <remarks>Array is Length = 11.</remarks>
		public static string[] OrderValue => (string[])_orderValue.Clone();

	}
}