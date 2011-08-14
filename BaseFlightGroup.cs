using System;

namespace Idmr.Platform
{
	[Serializable]
	public abstract class BaseFlightGroup
	{
		// private/protected
		#region Craft
		protected int _stringLength = 0x13;	// lengths for name, cargo, spec
		protected string _name = "New Ship";
		protected string _cargo = "";
		protected string _specialCargo = "";
		protected byte _specialCargoCraft = 0;	// binary is 1
		protected bool _randSpecCargo = false;
		protected byte _craftType = 1;	//default is X-W
		protected byte _numberOfCraft = 1;
		protected byte _status1 = 0;
		protected byte _missile = 0;
		protected byte _beam = 0;
		protected byte _iff = 0;
		protected byte _ai = 3;
		protected byte _markings = 0;
		protected byte _formation = 0;
		protected byte _formDistance = 2;
		protected byte _globalGroup = 0;
		protected byte _formLeaderDist = 0;
		protected byte _numberOfWaves = 1;	// binary is 0
		protected byte _playerCraft = 0;
		protected short _yaw = 0; // degrees, converted in Load/Save
		protected short _pitch = 0; // corrected to 0x40 in Load/Save
		protected short _roll = 0;
		#endregion
		#region ArrDep
		protected byte _difficulty = 0;
		protected byte[,] _arrDepTriggers;
		protected byte _arrivalDelayMinutes = 0;
		protected byte _arrivalDelaySeconds = 0;
		protected byte _departureTimerMinutes = 0;
		protected byte _departureTimerSeconds = 0;
		protected byte _abortTrigger = 0;
		protected byte _arrivalCraft1 = 0;
		protected bool _arrivalMethod1 = false;
		protected byte _arrivalCraft2 = 0;
		protected bool _arrivalMethod2 = false;
		protected byte _departureCraft1 = 0;
		protected bool _departureMethod1 = false;
		protected byte _departureCraft2 = 0;
		protected bool _departureMethod2 = false;
		#endregion
		protected byte[,] _orders;
		protected short[,] _waypoints;

		// public
		#region Craft
		/// <value>Name of FlightGroup</value>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE</remarks>
		public string Name
		{
			get { return _name; }
			set
			{
				if (value.Length > _stringLength) _name = value.Substring(0, _stringLength);
				else _name = value;
			}
		}
		/// <value>Default cargo assigned to entire FlightGroup</value>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE</remarks>
		public string Cargo
		{
			get { return _cargo; }
			set
			{
				if (value.Length > _stringLength) _cargo = value.Substring(0, _stringLength);
				else _cargo = value;
			}
		}
		/// <value>Cargo string for Special Craft</value>
		/// <remarks>Length is restricted to 0x13 for XvT and XWA, 0xC for TIE</remarks>
		public string SpecialCargo
		{
			get { return _specialCargo; }
			set
			{
				if (value.Length > _stringLength) _specialCargo = value.Substring(0, _stringLength);
				else _specialCargo = value;
			}
		}
		/// <value>When non-zero, indicated craft will use SpecialCargo instead of Cargo</value>
		/// <remarks>File stores as zero-indexed, when not used value is NumberOfCraft</remarks>
		public byte SpecialCargoCraft
		{
			get { return _specialCargoCraft; }
			set { _specialCargoCraft = value; }
		}
		/// <value>When <i>true</i> a craft within the FlightGroup will be picked at random to use SpecialCargo</value>
		public bool RandSpecCargo
		{
			get { return _randSpecCargo; }
			set { _randSpecCargo = value; }
		}
		/// <value>Determines ship or object type</value>
		/// <remarks>Defaults to X-wing (value=1)</remarks>
		public byte CraftType
		{
			get { return _craftType; }
			set { _craftType = value; }
		}
		/// <value>Number of craft per wave</value>
		public byte NumberOfCraft
		{
			get { return _numberOfCraft; }
			set { _numberOfCraft = value; }
		}
		/// <value>Condition of FlightGroup upon creation</value>
		public byte Status1
		{
			get { return _status1; }
			set { _status1 = value; }
		}
		/// <value>Warhead available to the FlightGroup</value>
		public byte Missile
		{
			get { return _missile; }
			set { _missile = value; }
		}
		/// <value>Determines Beam weapon used in the FlightGroup</value>
		public byte Beam
		{
			get { return _beam; }
			set { _beam = value; }
		}
		/// <value>Friend or Foe index</value>
		/// <remarks>Determines color of FlightGroup name and allegiance</remarks>
		public byte IFF
		{
			get { return _iff; }
			set { _iff = value; }
		}
		/// <value>AI setting for NPC craft</value>
		/// <remarks>Default index is 3</remarks>
		public byte AI
		{
			get { return _ai; }
			set { _ai = value; }
		}
		/// <value>Determines FlightGroup colors if applicable</value>
		public byte Markings
		{
			get { return _markings; }
			set { _markings = value; }
		}
		/// <value>FlightGroup Formation index</value>
		/// <remarks>Default is Vic (value=0)</remarks>
		public byte Formation
		{
			get { return _formation; }
			set { _formation = value; }
		}
		/// <value>FlightGroup spacing while in formation</value>
		/// <remarks>Default value=2</remarks>
		public byte FormDistance
		{
			get { return _formDistance; }
			set { _formDistance = value; }
		}
		/// <value>Grouping assignment</value>
		/// <remarks>Used to group multiple FlightGroups, used for triggers</remarks>
		public byte GlobalGroup
		{
			get { return _globalGroup; }
			set { _globalGroup = value; }
		}
		/// <value>Controls how far in front of the FlightGroup the Leader is</value>
		public byte FormLeaderDist
		{
			get { return _formLeaderDist; }
			set { _formLeaderDist = value; }
		}
		/// <value>Number of times FlightGroup will arrive</value>
		/// <remarks>Really zero-indexed, converted to one-indexed for ease of use</remarks>
		public byte NumberOfWaves
		{
			get { return _numberOfWaves; }
			set { _numberOfWaves = value; }
		}
		/// <value>When used, indicates craft index the Player occupies</value>
		public byte PlayerCraft
		{
			get { return _playerCraft; }
			set { _playerCraft = value; }
		}
		/// <value>Craft z-axis orientation at start in degrees, -180 to 179</value>
		/// <remarks>Calculated before Pitch and Roll, in TIE only applies to SpaceObjects
		/// File stores value as sbyte</remarks>
		public short Yaw
		{
			get { return _yaw; }
			set { if (value >= -180 && value < 180) _yaw = value; }
		}
		/// <value>Craft y-axis orientation at start in degrees, -180 to 179</value>
		/// <remarks>Calculated after Yaw and before Roll, in TIE only applies to SpaceObjects.<br>
		/// Flight engine adds -90 (nose down) pitch angle, automatically adjusted during Load/Save<br>
		/// File stores value as sbyte.</remarks>
		public short Pitch
		{
			get { return _pitch; }
			set { if (value >= -180 && value < 180) _pitch = value; }
		}
		/// <value>Craft x-axis orientation at start in degrees, -180 to 179</value>
		/// <remarks>Calculated after Yaw and Pitch, in TIE only applies to SpaceObjects
		/// File stores value as sbyte</remarks>
		public short Roll
		{
			get { return _roll; }
			set { if (value >= -180 && value < 180) _roll = value; }
		}
		#endregion
		#region ArrDep
		/// <value>Determines which mission difficulty setting is required for FlightGroup to arrive</value>
		/// <remarks>Defaults to All (value=0)</remarks>
		public byte Difficulty
		{
			get { return _difficulty; }
			set { _difficulty = value; }
		}
		/// <value>Arrival and Departure trigger array</value>
		public byte[,] ArrDepTrigger { get { return _arrDepTriggers; } }
		/// <value>Delay in minutes after the Arrival Trigger conditions have been met</value>
		/// <remarks>Can be used in conjunction with ArrivalDelaySeconds</remarks>
		public byte ArrivalDelayMinutes
		{
			get { return _arrivalDelayMinutes; }
			set { _arrivalDelayMinutes = value; }
		}
		/// <value>Delay in seconds after the Arrival Trigger conditions have been met</value>
		/// <remarks>Can be used in conjunction with ArrivalDelayMinutes</remarks>
		public byte ArrivalDelaySeconds
		{
			get { return _arrivalDelaySeconds; }
			set { _arrivalDelaySeconds = value; }
		}
		/// <value>Delay in minutes after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</value>
		/// <remarks>Can be used in conjunction with DepartureDelaySeconds</remarks>
		public byte DepartureTimerMinutes
		{
			get { return _departureTimerMinutes; }
			set { _departureTimerMinutes = value; }
		}
		/// <value>Delay in seconds after the Departure Trigger conditions have been met in XvT and XWA, after mission start in TIE</value>
		/// <remarks>Can be used in conjunction with DepartureDelayMinutes</remarks>
		public byte DepartureTimerSeconds
		{
			get { return _departureTimerSeconds; }
			set { _departureTimerSeconds = value; }
		}
		/// <value>Controls individual craft departure requirement</value>
		public byte AbortTrigger
		{
			get { return _abortTrigger; }
			set { _abortTrigger = value; }
		}
		/// <value>Primary arrival mothership FlightGroup index</value>
		public byte ArrivalCraft1
		{
			get { return _arrivalCraft1; }
			set { _arrivalCraft1 = value; }
		}
		/// <value>Primary method of arrival</value>
		/// <remarks>When true FlightGroup will attempt arrive via mothership, hyperspace when <i>false</i></remarks>
		public bool ArrivalMethod1
		{
			get { return _arrivalMethod1; }
			set { _arrivalMethod1 = value; }
		}
		/// <value>Secondary arrival mothership FlightGroup index</value>
		public byte ArrivalCraft2
		{
			get { return _arrivalCraft2; }
			set { _arrivalCraft2 = value; }
		}
		/// <value>Secondary method of arrival</value>
		/// <remarks>When <i>true</i> FlightGroup will attempt arrive via mothership, hyperspace when <i>false</i></remarks>
		public bool ArrivalMethod2
		{
			get { return _arrivalMethod2; }
			set { _arrivalMethod2 = value; }
		}
		/// <value>Primary departure mothership FlightGroup index</value>
		public byte DepartureCraft1
		{
			get { return _departureCraft1; }
			set { _departureCraft1 = value; }
		}
		/// <value>Primary method of departure</value>
		/// <remarks>When <i>true</i> Flightgroup will attempt to depart via mothership, hyperspace when <i>false</i></remarks>
		public bool DepartureMethod1
		{
			get { return _departureMethod1; }
			set { _departureMethod1 = value; }
		}
		/// <value>Secondary departure mothership FlightGroup index</value>
		public byte DepartureCraft2
		{
			get { return _departureCraft2; }
			set { _departureCraft2 = value; }
		}
		/// <value>Secondary method of departure</value>
		/// <remarks>When <i>true</i> Flightgroup will attempt to depart via mothership, hyperspace when <i>false</i></remarks>
		public bool DepartureMethod2
		{
			get { return _departureMethod2; }
			set { _departureMethod2 = value; }
		}
		#endregion
		/// <value>Controls FlightGroup actions during the mission</value>
		/// <remarks>See specific platforms for array declaration</remarks>
		public byte[,] Orders { get { return _orders; } }
		/// <value>Controls FlightGroup movement and locations</value>
		/// <remarks>Value stored is Coordinate * 160</remarks>
		public short[,] Waypoints { get { return _waypoints; } }
	}
}
