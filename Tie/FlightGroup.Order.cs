/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0, 120525
 * [NEW] value checks, exceptions, conversions
 * [NEW] ToString() override
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	public partial class FlightGroup : BaseFlightGroup
	{
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
		
	}
}
