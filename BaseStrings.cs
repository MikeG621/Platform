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
		/// <value>FlightGroup craft markings</value>
		/// <remarks>Array is Length=4</remarks>
		public static string[] Color { get { return (string[])_color.Clone(); } }
		/// <value>Arrival difficulty settings</value>
		/// <remarks>Array is Length=7</remarks>
		public static string[] Difficulty { get { return (string[])_difficulty.Clone(); } }
		/// <value>FlightGroup formation</value>
		/// <remarks>Array is Length=34</remarks>
		public static string[] Formation { get { return (string[])_formation.Clone(); } }
		/// <value>Trigger Type for solid objects</value>
		/// <remarks>Array is Length=3</remarks>
		public static string[] ObjectType { get { return (string[])_objectType.Clone(); } }
		/// <value>Trigger Type for ships</value>
		/// <remarks>Array is Length=7</remarks>
		public static string[] ShipClass { get { return (string[])_shipClass.Clone(); } }
		/// <value>Warhead types for FlightGroup usage</value>
		/// <remarks>Array is Length=8</remarks>
		public static string[] Warheads { get { return (string[])_warheads.Clone(); } }
	}
}
