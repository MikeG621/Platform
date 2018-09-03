/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0
 */

/* CHANGELOG
 * v3.0, 180903
 * [UPD] added "All IFFs except" target type [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] conversions, exceptions
 * [NEW] ToString() override
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _designation = "";
			
			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b></remarks>
			public Order()
			{
				_items = new byte[19];
				_items[1] = 10;	// Throttle
				_items[10] = _items[16] = 1;	// AndOrs
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks>If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19) ArrayFunctions.TrimArray(raw, 0, _items);
				else ArrayFunctions.WriteToArray(raw, _items, 0);
				_checkValues(this);
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks>If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Order(byte[] raw, int startIndex)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19)
				{
					if (raw.Length - startIndex < 19 || startIndex < 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 19));
					ArrayFunctions.TrimArray(raw, startIndex, _items);
				}
				else
				{
					if (startIndex != 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0");
					ArrayFunctions.WriteToArray(raw, _items, 0);
				}
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
					case 0xC:
						s += "TM:" + target;
						break;
					case 0x13:
						s += "All IFFs except " + Strings.IFF[target];  //[JB] Previously unknown order.
						break;
					case 0x15:
						s += "All Teams except TM:" + target;
						break;
					case 0x17:
						s += "Global Unit " + target;
						break;
					default:
						s += type + " " + target;
						break;
				}
				return s;
			}

			/// <summary>Returns a representative string of the Order</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> and Teams are identified as <b>"TM:#"</b> for later substitution if required</remarks>
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
			/// <remarks><see cref="Designation"/> is lost in the conversion</remarks>
			/// <param name="order">The Order to convert</param>
			/// <returns>A byte array of Length 19 containing the order data</returns>
			public static explicit operator byte[](Order order)
			{
				// Designation is lost
				byte[] b = new byte[19];
				for (int i = 0; i < 19; i++) b[i] = order[i];
				return b;
			}
			/// <summary>Converts an Order for TIE</summary>
			/// <remarks><see cref="Speed"/> and <see cref="Designation"/> are lost</remarks>
			/// <param name="order">The Order to convert</param>
			/// <returns>A copy of <i>order</i> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order order)
			{
				// Speed, Designtation are lost
				return new Tie.FlightGroup.Order((byte[])order, 0);
			}
			/// <summary>Converts an Order for XWA</summary>
			/// <param name="order">The Order to convert</param>
			/// <returns>A copy of <i>order</i> for use in XWA</returns>
			public static implicit operator Xwa.FlightGroup.Order(Order order)
			{
				Xwa.FlightGroup.Order ord = new Xwa.FlightGroup.Order((byte[])order);
				ord.CustomText = order.Designation;
				return ord;
			}
			
			#region public properties
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown6
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown7
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x0B</remarks>
			public byte Unknown8
			{
				get { return _items[11]; }
				set { _items[11] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x11</remarks>
			public byte Unknown9
			{
				get { return _items[17]; }
				set { _items[17] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _items[18]; }
				set { _items[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 16 characters</remarks>
			public string Designation
			{
				get { return _designation; }
				set { _designation = StringFunctions.GetTrimmed(value, 16); }
			}
			#endregion public properties
		}
	}
}
