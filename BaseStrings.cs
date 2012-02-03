/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform
{
	/// <summary>Contains strings shared between all platforms</summary>
	public abstract class BaseStrings
	{
		#region array declarations
		private static string[] _warheads = { "None",
										"Space Bomb",
										"Heavy Rocket",
										"Concussion Missile",
										"Torpedo",
										"Adv. Concussion Missile",
										"Adv. Torpedo",
										"Mag Pulse"
								   };
		private static string[] _color = { "Red (TIE - none)",
									"Gold (TIE - Red)",
									"Blue (TIE - Gold)",
									"Green (TIE - Blue)"
								};
		private static string[] _difficulty = { "All",
										"Easy",
										"Medium",
										"Hard",
										"Better than Easy",
										"Less than Hard",
										"Never",
									 };
		private static string[] _formation = { "Vic",
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
		private static string[] _shipClass = { "Starfighters",
										"Transports",
										"Freighters/Containers",
										"Starships",
										"Utility craft",
										"Platforms",
										"Mines"
									};
		private static string[] _objectType = { "Craft",
										 "Weapons",
										 "Satelites/Mines"
									 };
		#endregion
		/// <summary>Gets a copy of FlightGroup craft markings</summary>
		/// <remarks>Array has a Length of 4</remarks>
		public static string[] Color { get { return (string[])_color.Clone(); } }
		/// <summary>Gets a copy of Arrival difficulty settings</summary>
		/// <remarks>Array has a Length of 7</remarks>
		public static string[] Difficulty { get { return (string[])_difficulty.Clone(); } }
		/// <summary>Gets a copy of FlightGroup formations</summary>
		/// <remarks>Array has a Length of 34</remarks>
		public static string[] Formation { get { return (string[])_formation.Clone(); } }
		/// <summary>Gets a copy of Trigger Types for solid objects</summary>
		/// <remarks>Array has a Length of 3</remarks>
		public static string[] ObjectType { get { return (string[])_objectType.Clone(); } }
		/// <summary>Gets a copy of Trigger Types for ships</summary>
		/// <remarks>Array has a Length of 7</remarks>
		public static string[] ShipClass { get { return (string[])_shipClass.Clone(); } }
		/// <summary>Gets a copy of Warhead types for FlightGroup usage</summary>
		/// <remarks>Array has a Length of 8</remarks>
		public static string[] Warheads { get { return (string[])_warheads.Clone(); } }
	}
}
