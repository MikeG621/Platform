/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 120207 - added Waypoint conversions
 * 120208 - added Order conversions
 * 120210 - rewrote Waypoint
 * 120213 - rewrote for IOrder removal, added Order value checks
 * 120215 - Indexer<T> implmentation
 * 120227 - ToString() overrides
 * 120321 - Goal and Order exceptions, rename Radio to FollowsOrders
 * *** v2.0 ***
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
		
		/// <summary>Indexes for <see cref="ArrDepTriggers"/></summary>
		public enum ArrDepTriggerIndex : byte {
			/// <summary>First arrival trigger</summary>
			Arrival1,
			/// <summary>Second arrival trigger</summary>
			Arrival2,
			/// <summary>Departure trigger</summary>
			Departure
		}
		/// <summary>Indexes for <see cref="Waypoints"/></summary>
		public enum WaypointIndex : byte
		{
			/// <summary>Primary starting coordinate</summary>
			Start1,
			/// <summary>Optional starting coordinate</summary>
			Start2,
			/// <summary>Optional starting coordinate</summary>
			Start3,
			/// <summary>Optional starting coordinate</summary>
			Start4,
			/// <summary>First coordinate for orders and initial trajectory</summary>
			WP1,
			/// <summary>Second order coordinate</summary>
			WP2,
			/// <summary>Third order coordinate</summary>
			WP3,
			/// <summary>Fourth order coordinate</summary>
			WP4,
			/// <summary>Fifth order coordinate</summary>
			WP5,
			/// <summary>Sixth order coordinate</summary>
			WP6,
			/// <summary>Seventh order coordinate</summary>
			WP7,
			/// <summary>Eigth order coordinate</summary>
			WP8,
			/// <summary>Coordinate for Rendezvous orders</summary>
			Rendezvous,
			/// <summary>Arrival and Departure coordinate</summary>
			Hyperspace,
			/// <summary>Briefing coordinate</summary>
			Briefing
		}

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set Throttle to <b>100%</b>, all Goals set to <b>FALSE</b>, SP1 <b>Enabled</b></remarks>
		public FlightGroup()
		{
			_stringLength = 0xC;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			Goals = new FGGoals();
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"CraftAbbrv Name"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Short form is <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/></b><br/>Long form is <b>"<see cref="BaseFlightGroup.IFF"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see>
		/// - IsPlayer <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/>.
		/// <see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
			if (!verbose) return longName;
			return IFF + " - " + GlobalGroup + " - " + (PlayerCraft != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName;
		}
		
		/// <summary>Gets or sets the Pilot name, used as short note</summary>
		/// <remarks>Restricted to 12 characters</remarks>
		public string Pilot
		{
			get { return _pilot; }
			set { _pilot = StringFunctions.GetTrimmed(value, _stringLength); }
		}
		/// <summary>Gets or sets if the FlightGroup responds to player's orders</summary>
		public bool FollowsOrders { get; set; }
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for blank Arrival triggers and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds { get { return (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Array length is 3, use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets or sets if both triggers must be completed</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b></remarks>
		public bool AT1AndOrAT2 { get; set; }
		/// <summary>Gets or sets the FlightGroup-specific mission goals</summary>
		public FGGoals Goals { get; set; }
		/// <summary>Gets the Orders array used to control FlightGroup behaviour</summary>
		public Order[] Orders { get { return _orders; } }
		/// <summary>Gets or sets the unknown values container</summary>
		/// <remarks>All values initialize to <b>0</b> or <b>false</b></remarks>
		public UnknownValues Unknowns;
		/// <summary>Gets the Waypoint array used to determine starting location and AI pathing</summary>
		/// <remarks>Array length is 15, use <see cref="WaypointIndex"/> for indexes</remarks>
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
			/// <returns>A copy of <i>wp</i> for XvT</returns>
			public static implicit operator Xvt.FlightGroup.Waypoint(Waypoint wp) { return new Xvt.FlightGroup.Waypoint((short[])wp); }
			/// <summary>Converts a Waypoint for XWA</summary>
			/// <param name="wp">The Waypoint to convert</param>
			/// <returns>A copy of <i>wp</i> for XWA. The Region will be <b>zero</b></returns>
			public static implicit operator Xwa.FlightGroup.Waypoint(Waypoint wp) { return new Xwa.FlightGroup.Waypoint((short[])wp); }
		}
		/// <summary>Object for the Flightgroup-specific mission goals</summary>
		[Serializable] public class FGGoals : Indexer<byte>
		{
			/// <summary>Initializes a new instance with all goals set to "never (FALSE)" (10)</summary>
			public FGGoals()
			{
				_items = new byte[9];
				for (int i = 0; i < _items.Length; i += 2) _items[i] = 10;
			}
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 9</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public FGGoals(byte[] raw)
			{
				if (raw.Length < 9) throw new ArgumentException("Minimum length of raw is 9", "raw");
				_items = new byte[9];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}
			
			/// <summary>Initializes the Goals from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public FGGoals(byte[] raw, int startIndex)
			{
				if (raw.Length < 9) throw new ArgumentException("Minimum length of raw is 9", "raw");
				if (raw.Length - startIndex < 9 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 9));
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
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="PrimaryCondition"/></summary>
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
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="SecondaryCondition"/></summary>
			public byte SecondaryAmount
			{
				get { return _items[3]; }
				set { _items[3] = value; }
			}
			/// <summary>Gets or sets the hidden goal</summary>
			/// <remarks>Use of this goal is unknown, entirely hidden</remarks>
			public byte SecretCondition
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="SecretCondition"/></summary>
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
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="BonusCondition"/></summary>
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
			/// <remarks>Equals <see cref="RawBonusPoints"/> * 50, limited from <b>-6400</b> to <b>+6350</b></remarks>
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
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to 100%, AndOr values to <b>true</b></remarks>
			public Order()
			{
				_items = new byte[18];
				_items[1] = 10;
				_items[10] = 1;
				_items[16] = 1;
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[18];
				ArrayFunctions.TrimArray(raw, 0, _items);
				_checkValues(this);
			}
			
			/// <summary>Initializes the order from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Order(byte[] raw, int startIndex)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 4", "raw");
				if (raw.Length - startIndex < 18 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 18));
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
			
			static string _orderTargetString(byte target, byte type)
			{
				string s = "";
				switch (type)
				{
					case 0:
						break;
					case 1:
						s += "FG:" + target;
						break;
					case 2:
						s += Strings.CraftType[target + 1] + "s";
						break;
					case 3:
						s += Strings.ShipClass[target];
						break;
					case 4:
						s += Strings.ObjectType[target];
						break;
					case 5:
						s += Strings.IFF[target] + "s";
						break;
					case 6:
						s += "Craft with " + Strings.Orders[target] + " orders";
						break;
					case 7:
						s += "Craft when " + Strings.CraftWhen[target];
						break;
					case 8:
						s += "Global Group " + target;
						break;
					default:
						s += type + " " + target;
						break;
				}
				return s;
			}
			
			/// <summary>Returns a representative string of the Order</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the order and targets if applicable, otherwise <b>"None"</b></returns>
			public override string ToString()
			{
				if (Command == 0) return "None";
				string order = Strings.Orders[Command];
				if ((Command >= 7 && Command <= 0x12) || (Command >= 0x17 && Command <= 0x1B) || Command == 0x1F || Command == 0x20 || Command == 0x25)	//all orders where targets are important
				{
					string s = _orderTargetString(Target1, Target1Type);
					string s2 = _orderTargetString(Target2, Target2Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T1AndOrT2) order += " or " + s2;
						else order += " if " + s2;
					}
					s = _orderTargetString(Target3, Target3Type);
					s2 = _orderTargetString(Target4, Target4Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T3AndOrT4) order += " or " + s2;
						else order += " if " + s2;
					}
				}
				return order;
			}
			
			/// <summary>Converts an Order into a byte array</summary>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A byte array of length 18 containing the Order data</returns>
			public static explicit operator byte[](Order ord)
			{
				byte[] b = new byte[18];
				for (int i = 0; i < 18; i++) b[i] = ord[i];
				return b;
			}
			/// <summary>Converts an Order for XvT</summary>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for XvT</returns>
			public static implicit operator Xvt.FlightGroup.Order(Order ord) { return new Xvt.FlightGroup.Order((byte[])ord); }
			/// <summary>Converts an Order for XWA</summary>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for XWA</returns>
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
			/// <exception cref="ArgumentOutOfRangeException">Invalid <i>index</i> value</exception>
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
					else throw new ArgumentOutOfRangeException("index must be 0-8", "index");
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
