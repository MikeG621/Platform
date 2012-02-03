/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable] public class FlightGroup : BaseFlightGroup
	{
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[3];
		string _pilot = "";
		Order[] _orders = new Order[3];
		Waypoint[] _waypoints = new Waypoint[15];
		
		/// <summary>Indexes for the ArrDep Trigger array</summary>
		public enum ArrDepTriggerIndex : byte { Arrival1, Arrival2, Departure };
		/// <summary>Indexes for the Waypoint array</summary>
		public enum WaypointIndex : byte { Start1, Start2, Start3, Start4, WP1, WP2, WP3, WP4, WP5, WP6, WP7, WP8, Rendezvous, Hyperspace, Briefing };

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set Throttle to 100%, all Goals set to FALSE, SP1 Enabled</remarks>
		public FlightGroup()
		{
			_stringLength = 0xC;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			Goals = new FGGoals();
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
			//Radio = false;
			//AT1AndOrAT2 = false;
			//Unknowns = new UnknownValues();
		}

		/// <summary>Gets or sets the Pilot name, used as short note</summary>
		/// <remarks>Restricted to 12 characters</remarks>
		public string Pilot
		{
			get { return _pilot; }
			set { _pilot = StringFunctions.GetTrimmed(value, _stringLength); }
		}
		/// <summary>Gets or sets if the FlightGroup responds to player's orders</summary>
		public bool Radio { get; set; }
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30) return true;
				else return false;
			}
		}
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Array length is 3, use <i>ArrDepTriggerIndex</i> enumeration</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets or sets if both triggers must be completed</summary>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool AT1AndOrAT2 { get; set; }
		/// <summary>Gets or sets the FlightGroup-specific mission goals</summary>
		public FGGoals Goals { get; set; }
		/// <summary>Gets the Orders array used to control FlightGroup behaviour</summary>
		public Order[] Orders { get { return _orders; } }
		/// <summary>Gets or sets the unknown values container</summary>
		/// <remarks>All values initialize to 0 or <i>false</i></remarks>
		public UnknownValues Unknowns;
		/// <summary>Gets the Waypoint array used to determine starting location and AI pathing</summary>
		/// <remarks>Array length is 15, use the <i>WaypointIndex</i> enumeration</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }
		
		/// <summary>Object for a single Waypoint</summary>
		[Serializable] public class Waypoint : IWaypoint
		{
			short _rawX = 0;
			short _rawY = 0;
			short _rawZ = 0;
			bool _enabled = false;
			
			#region IWaypoint Members
			/// <summary>Array form of the Waypoint</summary>
			/// <param name="index">X, Y, Z, Enabled</param>
			/// <exception cref="ArgumentException">Invalid <i>index</i> value</exception>
			public short this[int index]
			{
				get
				{
					if (index == 0) return _rawX;
					else if (index == 1) return _rawY;
					else if (index == 2) return _rawZ;
					else if (index == 3) return Convert.ToInt16(_enabled);
					else throw new ArgumentException("index must be 0-3", "index");
					// TODO: throw InvalidOperation?
				}
				set
				{
					if (index == 0) _rawX = value;
					else if (index == 1) _rawY = value;
					else if (index == 2) _rawZ = value;
					else if (index == 3) _enabled = Convert.ToBoolean(value);
				}
			}
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return 4; } }
			
			/// <summary>Gets or sets if the Waypoint is active for use</summary>
			public bool Enabled
			{
				get { return _enabled; }
				set { _enabled = value; }
			}
			/// <summary>Gets or sets the stored X value</summary>
			public short RawX
			{
				get { return _rawX; }
				set { _rawX = value; }
			}
			/// <summary>Gets or sets the stored Y value</summary>
			public short RawY
			{
				get { return _rawY; }
				set { _rawY = value; }
			}
			/// <summary>Gets or sets the stored Z value</summary>
			public short RawZ
			{
				get { return _rawZ; }
				set { _rawZ = value; }
			}
			/// <summary>Gets or sets the X value in kilometers</summary>
			public double X
			{
				get { return (double)RawX / 160; }
				set { RawX = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Y value in kilometers</summary>
			public double Y
			{
				get { return (double)RawY / 160; }
				set { RawY = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Z value in kilometers</summary>
			public double Z
			{
				get { return (double)RawZ / 160; }
				set { RawZ = (short)(value * 160); }
			}
			#endregion
		}
		/// <summary>Object for the Flightgroup-specific mission goals</summary>
		[Serializable] public class FGGoals
		{
			byte[] _raw = new byte[9];
			
			/// <summary>Initializes a new instance with all goals set to "never (FALSE)" (10)</summary>
			public FGGoals() { for (int i = 0; i < _raw.Length; i += 2) _raw[i] = 10; }
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 9</param>
			/// <exception cref="ArgumentException"><i>raw</i> does not have the correct Length</exception>
			public FGGoals(byte[] raw)
			{
				if (raw.Length != 9) throw new ArgumentException("raw does not have the correct length", "raw");
				_raw = raw;
			}
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public FGGoals(byte[] raw, int startIndex) { ArrayFunctions.TrimArray(raw, startIndex, _raw); }
			
			/// <summary>Array form of the Goals</summary>
			/// <param name="index">Valid indexes are 0-8</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set { _raw[index] = value; }
			}
			
			#region public properties
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _raw.Length; } }
			
			/// <summary>Gets or sets the Primary goal</summary>
			public byte PrimaryCondition
			{
				get { return _raw[0]; }
				set { _raw[0] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>PrimaryCondition</i></summary>
			public byte PrimaryAmount
			{
				get { return _raw[1]; }
				set { _raw[1] = value; }
			}
			/// <summary>Gets or sets the Secondary goal</summary>
			public byte SecondaryCondition
			{
				get { return _raw[2]; }
				set { _raw[2] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>SecondaryCondition</i></summary>
			public byte SecondaryAmount
			{
				get { return _raw[3]; }
				set { _raw[3] = value; }
			}
			/// <summary>Gets or sets the hidden goal</summary>
			/// <remarks>Use of this goal is unknown, entirely hidden</summary>
			public byte SecretCondition
			{
				get { return _raw[4]; }
				set { _raw[4] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>SecretCondition</i></summary>
			public byte SecretAmount
			{
				get { return _raw[5]; }
				set { _raw[5] = value; }
			}
			/// <summary>Gets or sets the Bonus goal</summary>
			public byte BonusCondition
			{
				get { return _raw[6]; }
				set { _raw[6] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>BonusCondition</i></summary>
			public byte BonusAmount
			{
				get { return _raw[7]; }
				set { _raw[7] = value; }
			}
			/// <summary>Gets or sets the raw points value stored in the file</summary>
			public sbyte RawBonusPoints
			{
				get { return (sbyte)_raw[8]; }
				set { _raw[8] = (byte)value; }
			}
			/// <summary>Gets or sets the points awarded or subtracted upon Bonus goal completion</summary>
			/// <remarks>Equals <i>RawBonusPoints</i> * 50, limited from -6400 to +6350</remarks>
			public short BonusPoints
			{
				get { return (short)(_raw[8] * 50); }
				set { _raw[8] = (byte)((value > 6350 ? 6350 : (value < -6400 ? -6400 : value)) / 50); }
			}
			#endregion public properties
		}
		/// <summary>Object for a single Order</summary>
		[Serializable] public class Order : IOrder
		{
			byte[] _raw = new byte[18];

			/// <summary>Initializes a blank order</summary>
			/// <remarks><i>Throttle</i> set to 100%, AndOr values to <i>true</i></remarks>
			public Order()
			{
				_raw[1] = 10;
				_raw[10] = 1;
				_raw[16] = 1;
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 18</param>
			public Order(byte[] raw)
			{
				if (raw.Length != 18) throw new ArgumentException("raw does not have the correct length", "raw");
				_raw = raw;
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public Order(byte[] raw, int startIndex) { ArrayFunctions.TrimArray(raw, startIndex, _raw); }
			
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown18
			{
				get { return _raw[4]; }
				set { _raw[4] = value; }
			}
			
			#region IOrder Members
			/// <summary>Array form of the Order</summary>
			/// <param name="index">Valid indexes are 0-17. 5, 11 and 17 are always zero and cannot be set</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set
				{
					if (index == 5 || index == 11 || index == 17) return;
					_raw[index] = value;
					// TODO: catch IndexOutOfBounds and throw InvalidOperation?
				}
			}
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _raw.Length; } }

			/// <summary>Gets or sets the command for the FlightGroup</summary>
			public byte Command
			{
				get { return _raw[0]; }
				set { _raw[0] = value; }
			}
			/// <summary>Gets or sets the throttle setting</summary>
			/// <remarks>Multiply value by 10 to get Throttle percent</remarks>
			public byte Throttle
			{
				get { return _raw[1]; }
				set { _raw[1] = value; }
			}
			/// <summary>Gets or sets the first order-specific setting</summary>
			public byte Variable1
			{
				get { return _raw[2]; }
				set { _raw[2] = value; }
			}
			/// <summary>Gets or sets the second order-specific setting</summary>
			public byte Variable2
			{
				get { return _raw[3]; }
				set { _raw[3] = value; }
			}
			/// <summary>Gets or sets the type for Target 3</summary>
			public byte Target3Type
			{
				get { return _raw[6]; }
				set { _raw[6] = value; }
			}
			/// <summary>Gets or sets the type for target 4</summary>
			public byte Target4Type
			{
				get { return _raw[7]; }
				set { _raw[7] = value; }
			}
			/// <summary>Gets or sets the third target</summary>
			public byte Target3
			{
				get { return _raw[8]; }
				set { _raw[8] = value; }
			}
			/// <summary>Fourth target</summary>
			public byte Target4
			{
				get { return _raw[9]; }
				set { _raw[9] = value; }
			}
			/// <summary>Gets or sets if the T3 and T4 settings are mutually exclusive</summary>
			/// <remarks><i>false</i> is And/If and <i>true</i> is Or</remarks>
			public bool T3AndOrT4
			{
				get { return Convert.ToBoolean(_raw[10]); }
				set { _raw[10] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the type for Target 1</summary>
			public byte Target1Type
			{
				get { return _raw[12]; }
				set { _raw[12] = value; }
			}
			/// <summary>Gets or sets the first target</summary>
			public byte Target1
			{
				get { return _raw[13]; }
				set { _raw[13] = value; }
			}
			/// <summary>Gets or sets the type for Target 2</summary>
			public byte Target2Type
			{
				get { return _raw[14]; }
				set { _raw[14] = value; }
			}
			/// <summary>Gets or sets the second target</summary>
			public byte Target2
			{
				get { return _raw[15]; }
				set { _raw[15] = value; }
			}
			/// <summary>Gets or sets if the T1 and T2 settings are mutually exclusive</summary>
			/// <remarks><i>false</i> is And/If and <i>true</i> is Or</remarks>
			public bool T1AndOrT2
			{
				get { return Convert.ToBoolean(_raw[16]); }
				set { _raw[16] = Convert.ToByte(value); }
			}
			#endregion IOrder Members
		}
		
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x003B, in Craft section, Reserved(0)</remarks>
			public byte Unknown1;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0041, in Craft section</remarks>
			public byte Unknown5;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0046, in Craft section</remarks>
			public bool Unknown9;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0047, in Craft section</remarks>
			public byte Unknown10;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0048, in Craft section, Reserved(0)</remarks>
			public byte Unknown11;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0053, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown12;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005D, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown15;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005E, in Arr/Dep section</remarks>
			public byte Unknown16;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005F, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown17;

			/// <summary>Array form of the Unknowns</summary>
			/// <param name="index">Valid indexes are 0-8</param>
			/// <exception cref="ArgumentException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get
				{
					if (index == 0) return Unknown1;
					else if (index == 1) return Unknown5;
					else if (index == 2) return Convert.ToByte(Unknown9);
					else if (index == 3) return Unknown10;
					else if (index == 4) return Unknown11;
					else if (index == 5) return Unknown12;
					else if (index == 6) return Unknown15;
					else if (index == 7) return Unknown16;
					else if (index == 8) return Unknown17;
					else throw new ArgumentException("index must be 0-8", "index");
				}
				set
				{
					if (index == 0) Unknown1 = value;
					else if (index == 1) Unknown5 = value;
					else if (index == 2) Unknown9 = Convert.ToBoolean(value);
					else if (index == 3) Unknown10 = value;
					else if (index == 4) Unknown11 = value;
					else if (index == 5) Unknown12 = value;
					else if (index == 6) Unknown15 = value;
					else if (index == 7) Unknown16 = value;
					else if (index == 8) Unknown17 = value;
				}
			}
		}
	}
}
