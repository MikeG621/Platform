/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.7
 */

/* CHANGELOG
 * v2.7, 180509
 * [ADD #1] TriggerType unknowns
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] conversions, exceptions
 * [NEW] ToString() override
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Container for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _customText = "";
			Waypoint[] _waypoints = new Waypoint[8];
			Mission.Trigger[] _skipTriggers = new Mission.Trigger[2];
			
			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b>, <see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b></remarks>
			public Order()
			{
				_items = new byte[19];
				_items[1] = 10;	// Throttle
				_items[10] = _items[16] = 1;	// AndOrs
				initialize();
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b><br/>
			/// If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19) ArrayFunctions.TrimArray(raw, 0, _items);
				else ArrayFunctions.WriteToArray(raw, _items, 0);
				initialize();
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"never (FALSE)"</b><br/>
			/// If <i>raw</i>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value</exception>
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
				initialize();
			}
			
			void initialize()
			{
				for (int i = 0; i < 8; i++) _waypoints[i] = new Waypoint();
				_skipTriggers[0] = new Mission.Trigger();
				_skipTriggers[1] = new Mission.Trigger();
				_skipTriggers[0].Condition = 10;
				_skipTriggers[1].Condition = 10;
				SkipT1AndOrT2 = true;
			}
			#endregion constructors

			static string _orderTargetString(byte target, byte type)
			{
				string s = "";
				switch (type)
				{
					case 0:
						break;
					case 1:
						s = "FG:" + target;
						break;
					case 2:
						s = Strings.CraftType[target + 1] + "s";
						break;
					case 3:
						s = Strings.ShipClass[target];
						break;
					case 4:
						s = Strings.ObjectType[target];
						break;
					case 5:
						s = Strings.IFF[target] + "s";
						break;
					case 6:
						s = "Craft with " + Strings.Orders[target] + " orders";
						break;
					case 7:
						s = "Craft when " + Strings.CraftWhen[target];
						break;
					case 8:
						s = "Global Group " + target;
						break;
					case 9:
						s = "Rating " + Strings.Rating[target];
						break;
					case 10:
						s = "Craft with status: " + Strings.Status[target];
						break;
					case 11:
						s = "All";
						break;
					case 12:
						s = "TM:" + target;
						break;
					case 13:
						s = "Player #" + target;
						break;
					case 15:
						s = "Not FG:" + target;
						break;
					case 16:
						s = "Not ship type " + Strings.CraftType[target + 1];
						break;
					case 17:
						s = "Not ship class " + Strings.ShipClass[target];
						break;
					case 18:
						s = "Not object type " + Strings.ObjectType[target];
						break;
					case 19:
						s = "Not IFF " + Strings.IFF[target];
						break;
					case 20:
						s = "Not GG " + target;
						break;
					case 21:
						s = "All Teams except TM:" + target;
						break;
					case 22:
						s = "Not player " + target;
						break;
					case 23:
						s = "Global Unit " + target;
						break;
					case 24:
						s = "Not Global Unit " + target;
						break;
					case 25:
						s = "Global Cargo " + target;
						break;
					case 26:
						s = "Not Global Cargo " + target;
						break;
					case 27:
						s = "Message #" + target;
						break;
					default:
						s = type + " " + target;
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
				if ((Command >= 7 && Command <= 18) || (Command >= 23 && Command <= 27) || Command == 31 || Command == 32 || Command == 37)	//all orders where targets are important
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

			/// <summary>Converts an order to a byte array</summary>
			/// <remarks><see cref="CustomText"/>, <see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A byte array of Length 19 containing the Order data</returns>
			public static explicit operator byte[](Order ord)
			{
				// CustomText, WPs, Skips lost
				byte[] b = new byte[19];
				for (int i = 0; i < 19; i++) b[i] = ord[i];
				return b;
			}
			/// <summary>Converts an Order for TIE</summary>
			/// <remarks><see cref="Speed"/>, <see cref="CustomText"/>, <see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order ord)
			{
				// Speed, CustomText, WPs, Skips lost
				return new Tie.FlightGroup.Order((byte[])ord, 0);
			}
			/// <summary>Converts an Order for XvT</summary>
			/// <remarks><see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion. <see cref="CustomText"/> trimmed to 16 characters</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <i>ord</i> for use in XvT</returns>
			public static explicit operator Xvt.FlightGroup.Order(Order ord)
			{
				// WPs, Skips lost
				// CustomText trimmed to 16 char
				Xvt.FlightGroup.Order newOrder = new Xvt.FlightGroup.Order((byte[])ord);
				newOrder.Designation = ord.CustomText;
				return newOrder;
			}
			
			#region public properties
			/// <summary>Gets or sets the third order-specific setting</summary>
			public byte Variable3
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown9
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _items[18]; }
				set { _items[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 63 characters</remarks>
			public string CustomText
			{
				get { return _customText; }
				set { _customText = StringFunctions.GetTrimmed(value, 63); }
			}

			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x72</remarks>
			public byte Unknown10 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x73</remarks>
			public bool Unknown11 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x74</remarks>
			public bool Unknown12 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x7B</remarks>
			public bool Unknown13 { get; set; }
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x81</remarks>
			public bool Unknown14 { get; set; }
			/// <summary>Whether or not the Skip Triggers are exclusive</summary>
			/// <remarks>Default is <b>"Or" (true)</b> due to default "never (FALSE)" Trigger</remarks>
			public bool SkipT1AndOrT2 { get; set; }

			/// <summary>Gets the order-specific location markers</summary>
			/// <remarks>Array is length 8</remarks>
			public Waypoint[] Waypoints { get { return _waypoints; } }
			
			/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to the order</summary>
			/// <remarks>Array is length 2</remarks>
			public Mission.Trigger[] SkipTriggers { get { return _skipTriggers; } }
			#endregion public properties
		}
	}
}
