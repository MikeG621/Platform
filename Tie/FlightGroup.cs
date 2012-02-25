/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG
 * 120207 - added Waypoint conversions
 * 120208 - added Order conversions
 * 120210 - rewrote Waypoint
 * 120213 - rewrote for IOrder removal, added Order value checks
 * 120215 - Indexer<T> implmentation
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
		[Serializable] public class Waypoint : BaseWaypoint
		{
			/// <summary>Default constructor</summary>
			public Waypoint() : base(new short[4]) { }
			
			/// <summary>Initialize a new Waypoint using raw data</summary>
			/// <remarks><i>raw</i> must have Length of 4</remarks>
			/// <param name="raw">Raw data</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Waypoint(short[] raw) : base(raw) { if (raw.Length != 4) throw new ArgumentException("raw does not have the correct size"); }
			
			/// <summary>Converts a Waypoint for XvT</summary>
			/// <param name="wp">The Waypoint to convert</param>
			public static implicit operator Xvt.FlightGroup.Waypoint(Waypoint wp) { return new Xvt.FlightGroup.Waypoint((short[])wp); }
			/// <summary>Converts a Waypoint for XWA</summary>
			/// <param name="wp">The Waypoint to convert</param>
			public static implicit operator Xwa.FlightGroup.Waypoint(Waypoint wp) { return new Xwa.FlightGroup.Waypoint((short[])wp); }
		}
		/// <summary>Object for the Flightgroup-specific mission goals</summary>
		[Serializable] public class FGGoals : Indexer<byte>
		{
			// _items is the raw byte data from file
			
			/// <summary>Initializes a new instance with all goals set to "never (FALSE)" (10)</summary>
			public FGGoals()
			{
				_items = new byte[9];
				for (int i = 0; i < _items.Length; i += 2) _items[i] = 10;
			}
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 9</param>
			/// <exception cref="ArgumentException"><i>raw</i> does not have the correct Length</exception>
			public FGGoals(byte[] raw) : base(raw) { if (raw.Length != 9) throw new ArgumentException("raw does not have the correct length", "raw"); }
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public FGGoals(byte[] raw, int startIndex)
			{
				_items = new byte[9];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
			}
			
			#region public properties
			/// <summary>Gets or sets the Primary goal</summary>
			public byte PrimaryCondition
			{
				get { return _items[0]; }
				set { _items[0] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>PrimaryCondition</i></summary>
			public byte PrimaryAmount
			{
				get { return _items[1]; }
				set { _items[1] = value; }
			}
			/// <summary>Gets or sets the Secondary goal</summary>
			public byte SecondaryCondition
			{
				get { return _items[2]; }
				set { _items[2] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>SecondaryCondition</i></summary>
			public byte SecondaryAmount
			{
				get { return _items[3]; }
				set { _items[3] = value; }
			}
			/// <summary>Gets or sets the hidden goal</summary>
			/// <remarks>Use of this goal is unknown, entirely hidden</summary>
			public byte SecretCondition
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>SecretCondition</i></summary>
			public byte SecretAmount
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Gets or sets the Bonus goal</summary>
			public byte BonusCondition
			{
				get { return _items[6]; }
				set { _items[6] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>BonusCondition</i></summary>
			public byte BonusAmount
			{
				get { return _items[7]; }
				set { _items[7] = value; }
			}
			/// <summary>Gets or sets the raw points value stored in the file</summary>
			public sbyte RawBonusPoints
			{
				get { return (sbyte)_items[8]; }
				set { _items[8] = (byte)value; }
			}
			/// <summary>Gets or sets the points awarded or subtracted upon Bonus goal completion</summary>
			/// <remarks>Equals <i>RawBonusPoints</i> * 50, limited from -6400 to +6350</remarks>
			public short BonusPoints
			{
				get { return (short)(RawBonusPoints * 50); }
				set { RawBonusPoints = (sbyte)((value > 6350 ? 6350 : (value < -6400 ? -6400 : value)) / 50); }
			}
			#endregion public properties
		}
		/// <summary>Object for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			#region constructors
			/// <summary>Initializes a blank order</summary>
			/// <remarks><i>Throttle</i> set to 100%, AndOr values to <i>true</i></remarks>
			public Order()
			{
				_items = new byte[18];
				_items[1] = 10;
				_items[10] = 1;
				_items[16] = 1;
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 18</param>
			public Order(byte[] raw)
			{
				if (raw.Length != 18) throw new ArgumentException("raw does not have the correct length", "raw");
				_items = raw;
				_checkValues(this);
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public Order(byte[] raw, int startIndex)
			{
				_items = new byte[18];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
				_checkValues(this);
			}
			#endregion
			
			static void _checkValues(Order o)
			{
				string error = "";
				string msg;
				byte tempVar;
				if (o.Command > 39) error += "Command (" + o.Command + ")";
				tempVar = o.Target1;
				Mission.CheckTarget(o.Target1Type, ref tempVar, out msg);
				o.Target1 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T1 " + msg;
				tempVar = o.Target2;
				Mission.CheckTarget(o.Target2Type, ref tempVar, out msg);
				o.Target2 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T2 " + msg;
				tempVar = o.Target3;
				Mission.CheckTarget(o.Target3Type, ref tempVar, out msg);
				o.Target3 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T3 " + msg;
				tempVar = o.Target4;
				Mission.CheckTarget(o.Target4Type, ref tempVar, out msg);
				o.Target4 = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + "T4 " + msg;
				if (error != "") throw new ArgumentException("Invalid values detected: " + error);
			}
			/// <summary>Converts an Order into a byte array</summary>
			/// <remarks>Length will be 18</remarks>
			/// <param name="ord">The Order to convert</param>
			public static explicit operator byte[](Order ord)
			{
				byte[] b = new byte[18];
				for (int i = 0; i < 18; i++) b[i] = ord[i];
				return b;
			}
			/// <summary>Converts an Order for XvT</summary>
			/// <param name="ord">The Order to convert</param>
			public static implicit operator Xvt.FlightGroup.Order(Order ord) { return new Xvt.FlightGroup.Order((byte[])ord); }
			/// <summary>Converts an Order for XWA</summary>
			/// <param name="ord">The Order to convert</param>
			public static implicit operator Xwa.FlightGroup.Order(Order ord) { return new Xwa.FlightGroup.Order((byte[])ord); }
			
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown18
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
		}
		
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x003B, in Craft section, Reserved(0)</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0041, in Craft section</remarks>
			public byte Unknown5 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0046, in Craft section</remarks>
			public bool Unknown9 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0047, in Craft section</remarks>
			public byte Unknown10 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0048, in Craft section, Reserved(0)</remarks>
			public byte Unknown11 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0053, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown12 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005D, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown15 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005E, in Arr/Dep section</remarks>
			public byte Unknown16 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x005F, in Arr/Dep section, Reserved(0)</remarks>
			public byte Unknown17 { get; set; }

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
