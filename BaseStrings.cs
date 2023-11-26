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
 * [UPD] ShipClass and ObjectType updated [JB]
 * [NEW] FormationMine [JB]
 * v3.0, 180309
 * [UPD] reworded >Easy [JB]
 * [NEW] DifficultyAbbrv [JB]
 * [FIX] remarks for Warheads count [JB]
 * [NEW] SafeString() [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * [NEW] Warheads.(Ion Pulse)
 */

namespace Idmr.Platform
{
    /// <summary>Contains strings shared between all platforms</summary>
    public abstract class BaseStrings
	{
		#region array declarations
		static readonly string[] _warheads = { "None",
										"Space Bomb",
										"Heavy Rocket",
										"Concussion Missile",
										"Torpedo",
										"Adv. Concussion Missile",
										"Adv. Torpedo",
										"Mag Pulse",
										"(Ion Pulse)"
								   };
		static readonly string[] _color = { "Red (TIE - none)",
									"Gold (TIE - Red)",
									"Blue (TIE - Gold)",
									"Green (TIE - Blue)"
								};
		static readonly string[] _difficulty = { "All",
										"Easy",
										"Medium",
										"Hard",
										"Greater than Easy",
										"Less than Hard",
										"Never",
									 };
        static readonly string[] _difficultyAbbrv = { "",
										"E",
										"M",
										"H",
										">E",
										"<H",
										"X",
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
										"Reverse High Vic",
										"Reverse Line Astern",
										"Stacked Low",
										"Abreast Right",
										"Abreast Left",
										"Wing Forward",
										"Wing Back",
										"Line Astern Up",
										"Line Astern Down",
										"Abreast V",
										"Abreast Inverted V",
										"Double Astern Mirror",
										"Double Stacked Astern",
										"Double Stacked High",
										"Diamond 1",
										"Diamond 2",
										"Flat Pentagon",
										"Side Pentagon",
										"Front Pentagon",
										"Flat Hexagon",
										"Side Hexagon",
										"Front Hexagon"
									};
		static readonly string[] _formationMine = {"Floor (X-Y)",
										"Side (Y-Z)",
										"Front (X-Z)",
										"Overlap on point (bad)"
									};
		static readonly string[] _shipClass = { "Starfighters",
										"Transports",
										"Freighters/Containers",
										"Starships",
										"Utility craft",
										"Platforms",
										"Mines",
										"Probes/Sats/Navs",
									};
		static readonly string[] _objectType = { "Craft",
										 "Mines",
										 "Probes/Sats/Navs"
									 };
		#endregion
		/// <summary>Gets a copy of FlightGroup craft markings</summary>
		/// <remarks>Array has a Length of 4</remarks>
		public static string[] Color => (string[])_color.Clone();
		/// <summary>Gets a copy of Arrival difficulty settings</summary>
		/// <remarks>Array has a Length of 7</remarks>
		public static string[] Difficulty => (string[])_difficulty.Clone();
		/// <summary>Gets a copy of Arrival Difficulty abbreviations</summary>
		/// <remarks>Array has a Length of 7</remarks>
		public static string[] DifficultyAbbrv => (string[])_difficultyAbbrv.Clone();
		/// <summary>Gets a copy of FlightGroup formations</summary>
		/// <remarks>Array has a Length of 34</remarks>
		public static string[] Formation => (string[])_formation.Clone();
		/// <summary>Gets a copy of formations only used by mine space objects</summary>
		/// <remarks>Array has a Length of 4</remarks>
		public static string[] FormationMine => (string[])_formationMine.Clone();
		/// <summary>Gets a copy of Trigger Types for solid objects</summary>
		/// <remarks>Array has a Length of 3</remarks>
		public static string[] ObjectType => (string[])_objectType.Clone();
		/// <summary>Gets a copy of Trigger Types for ships</summary>
		/// <remarks>Array has a Length of 7</remarks>
		public static string[] ShipClass => (string[])_shipClass.Clone();
		/// <summary>Gets a copy of Warhead types for FlightGroup usage</summary>
		/// <remarks>Array has a Length of 9</remarks>
		public static string[] Warheads => (string[])_warheads.Clone();

		/// <summary>Checks whether an index into a given string array is valid before accessing that element.</summary>
		/// <param name="array">The strings to pull from</param>
		/// <param name="index">The desired entry</param>
		/// <remarks>Avoids exceptions when requesting strings of associated values.</remarks>
		/// <returns>A string from the index, or a string indicating an unknown value.</returns>
		public static string SafeString(string[] array, int index)
		{
            if(array == null) return "Invalid Array";
			if(index < 0 || index >= array.Length)
				return "Unknown:" + index;
			return array[index];
		}    
    }
}
