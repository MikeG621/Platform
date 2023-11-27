/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7+
 */

/* CHANGELOG
 * [NEW] CommandList enum
 * [UPD] Convert times for XWA
 * v5.7, 220127
 * [UPD] cloning ctor now calls base [JB]
 * v5.6, 220103
 * [NEW] cloning ctor [JB]
 * v5.5, 2108001
 * [UPD] SS Patrol and SS Await Return order strings now show target info [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * [UPD] TriggerType expanded [JB]
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

			/// <summary>Available orders</summary>
			public enum CommandList : byte
			{
				/// <summary>Stationary. Go Home if not first order</summary>
				HoldSteady,
				/// <summary>Return to Mothership or hyperspace</summary>
				GoHome,
				/// <summary>Loop through waypoints</summary>
				Circle,
				/// <summary>Loop through waypoints and evade</summary>
				CircleEvade,
				/// <summary>Fly to RDV and await docking</summary>
				Rendezvous,
				/// <summary>Disabled</summary>
				Disabled,
				/// <summary>Disabled, awaiting boarding</summary>
				AwaitBoarding,
				/// <summary>Attack targets</summary>
				AttackTargets,
				/// <summary>Attack target's escorts</summary>
				AttackEscorts,
				/// <summary>Attack target's attackers</summary>
				Protect,
				/// <summary>Attack target's attackers and boarding craft</summary>
				Escort,
				/// <summary>Attack to disable targets</summary>
				DisableTargets,
				/// <summary>Board targets to give cargo</summary>
				BoardGiveCargo,
				/// <summary>Board targets to take cargo</summary>
				BoardTakeCargo,
				/// <summary>Board targets to exchange cargo</summary>
				BoardExchangeCargo,
				/// <summary>Board to capture targets</summary>
				BoardToCapture,
				/// <summary>Board targets to destroy</summary>
				BoardDestroy,
				/// <summary>Pickup and carry target</summary>
				PickUp,
				/// <summary>Drops off designated FG</summary>
				DropOff,
				/// <summary>Wait for a time</summary>
				Wait,
				/// <summary>Wait for a time (Starship)</summary>
				SSWait,
				/// <summary>Loop through waypoints (Starship)</summary>
				SSPatrol,
				/// <summary>Wait for return of all craft that use FG as Mothership</summary>
				SSAwaitReturn,
				/// <summary>Wait for launch of all craft that use FG as Mothership</summary>
				SSLaunch,
				/// <summary>Loop through waypoints and attack target's attackers</summary>
				SSProtect,
				/// <summary>Loop through waypoints and attack target's attackers</summary>
				SSProtect2,
				/// <summary>Loop through waypoints and attack targets</summary>
				SSPatrolAttack,
				/// <summary>Loop through waypoints and attack to disable targets</summary>
				SSPatrolDisable,
				/// <summary>Stationary</summary>
				Hold2,
				/// <summary>Return to Mothership or hyperspace (Starship)</summary>
				SSGoHome,
				/// <summary>Stationary</summary>
				Hold3,
				/// <summary>Boards target (Starship)</summary>
				SSBoard,
				/// <summary>Boards target to repair systems</summary>
				BoardRepair,
				/// <summary>Stationary</summary>
				Hold4,
				/// <summary>Stationary</summary>
				Hold5,
				/// <summary>Stationary</summary>
				Hold6,
				/// <summary>Destroys self</summary>
				SelfDestruct,
				/// <summary>Rams target</summary>
				Kamikaze,
				/// <summary>Stationary</summary>
				Hold7,
				/// <summary>Stationary</summary>
				Null
			}

			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b></remarks>
			public Order()
			{
				_items = new byte[19];
				_items[1] = 10;	// Throttle
				_items[10] = _items[16] = 1;	// AndOrs
			}
			/// <summary>Initializes a new Order from an existing Order.</summary>
			/// <param name="other">Existing Order to clone. If <b>null</b>, Order will be blank.</param>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b> if <paramref name="other"/> is <b>null</b>.</remarks>
			public Order(Order other) : this()
			{
				if (other != null)
				{
					Array.Copy(other._items, _items, _items.Length);
					_designation = other._designation;
				}
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks>If <paramref name="raw"/>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Order(byte[] raw)
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				_items = new byte[19];
				if (raw.Length >= 19) ArrayFunctions.TrimArray(raw, 0, _items);
				else ArrayFunctions.WriteToArray(raw, _items, 0);
				checkValues(this);
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks>If <paramref name="raw"/>.Length is 19 or greater, reads 19 bytes. Otherwise reads 18 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/></exception>
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
				checkValues(this);
			}
			#endregion
			
			static void checkValues(Order o)
			{
				string error = "";
				byte tempVar;
				if (o.Command > (byte)CommandList.Null) error += "Command (" + o.Command + ")";
				tempVar = o.Target1;
				Mission.CheckTarget(o.Target1Type, ref tempVar, out string msg);
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

			static string orderTargetString(byte target, byte type)
			{
				string s = "";
				switch ((Mission.Trigger.TypeList)type)
				{
					case Mission.Trigger.TypeList.None:
						break;
					case Mission.Trigger.TypeList.FlightGroup:
						s += "FG:" + target;
						break;
					case Mission.Trigger.TypeList.ShipType:
						s += BaseStrings.SafeString(Strings.CraftType, target + 1) + "s";
						break;
					case Mission.Trigger.TypeList.ShipClass:
						s += BaseStrings.SafeString(Strings.ShipClass, target);
						break;
					case Mission.Trigger.TypeList.ObjectType:
						s += BaseStrings.SafeString(Strings.ObjectType, target);
						break;
					case Mission.Trigger.TypeList.IFF:
						s += BaseStrings.SafeString(Strings.IFF, target) + "s";
						break;
					case Mission.Trigger.TypeList.ShipOrders:
						s += "Craft with " + BaseStrings.SafeString(Strings.Orders, target) + " starting orders";
						break;
					case Mission.Trigger.TypeList.CraftWhen:
						s += "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, target);
						break;
					case Mission.Trigger.TypeList.GlobalGroup:
						s += "Global Group " + target;
						break;
					case Mission.Trigger.TypeList.AILevel:
						s += "Craft with " + BaseStrings.SafeString(Strings.Rating, target) + " adjusted skill";
						break;
					case Mission.Trigger.TypeList.Status:
						s += "Craft with primary status: " + BaseStrings.SafeString(Strings.Status, target);
						break;
					case Mission.Trigger.TypeList.AllCraft:
						s += "All craft";
						break;
					case Mission.Trigger.TypeList.Team:
						s += "TM:" + target;
						break;
					case Mission.Trigger.TypeList.PlayerNum:
						s += "Player #" + (target + 1);
						break;
					case Mission.Trigger.TypeList.BeforeTime:
						s += "Before elapsed time " + string.Format("{0}:{1:00}", target * 5 / 60, target * 5 % 60);
						break;
					case Mission.Trigger.TypeList.NotFG:
						s += "Not FG:" + target;
						break;
					case Mission.Trigger.TypeList.NotShipType:
						s += "Not ship type " + BaseStrings.SafeString(Strings.CraftType, target + 1) + "s";
						break;
					case Mission.Trigger.TypeList.NotShipClass:
						s += "Not ship class " + BaseStrings.SafeString(Strings.ShipClass, target);
						break;
					case Mission.Trigger.TypeList.NotObjectType:
						s += "Not object type " + BaseStrings.SafeString(Strings.ObjectType, target);
						break;
					case Mission.Trigger.TypeList.NotIFF:
						s += "Not IFF " + BaseStrings.SafeString(Strings.IFF, target);
						break;
					case Mission.Trigger.TypeList.NotGlobalGroup:
						s += "Not GG " + target;
						break;
					case Mission.Trigger.TypeList.NotTeam:
						s += "All teams except TM:" + target;
						break;
					case Mission.Trigger.TypeList.NotPlayerNum:
						s += "Not player #" + (target + 1);
						break;
					case Mission.Trigger.TypeList.GlobalUnit:
						s += "Global Unit " + target;
						break;
					case Mission.Trigger.TypeList.NotGlobalUnit:
						s += "Not global unit " + target;
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
				string order = BaseStrings.SafeString(Strings.Orders, Command);
				if ((Command >= (byte)CommandList.AttackTargets && Command <= (byte)CommandList.DropOff)
					|| (Command >= (byte)CommandList.SSPatrol && Command <= (byte)CommandList.SSPatrolDisable)
					|| Command == (byte)CommandList.SSBoard || Command == (byte)CommandList.BoardRepair || Command == (byte)CommandList.Kamikaze)	//all orders where targets are important
				{
					string s = orderTargetString(Target1, Target1Type);
					string s2 = orderTargetString(Target2, Target2Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T1AndOrT2) order += " or " + s2;
						else order += " if " + s2;
					}
					s = orderTargetString(Target3, Target3Type);
					s2 = orderTargetString(Target4, Target4Type);
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
			/// <returns>A copy of <paramref name="order"/> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order order) => new Tie.FlightGroup.Order((byte[])order);
			/// <summary>Converts an Order for XWA</summary>
			/// <param name="order">The Order to convert</param>
			/// <returns>A copy of <paramref name="order"/> for use in XWA</returns>
			public static implicit operator Xwa.FlightGroup.Order(Order order)
			{
				var ord = new Xwa.FlightGroup.Order((byte[])order)
				{
					CustomText = order.Designation
				};
                //XvT time is value*5=sec
                //XWA time value, if 20 (decimal) or under is exact seconds, then +5 up to 15:00, then +10.
                //21 = 25 sec, 22 = 30 sec, etc.
                switch ((CommandList)ord.Command)
                {
                    case CommandList.BoardGiveCargo:
                    case CommandList.BoardTakeCargo:
                    case CommandList.BoardExchangeCargo:
                    case CommandList.BoardToCapture:
                    case CommandList.BoardDestroy:
                    case CommandList.PickUp:
                    case CommandList.DropOff:
                    case CommandList.Wait:
                    case CommandList.SSWait:
                    case CommandList.Hold2:
                    case CommandList.Hold3:
                    case CommandList.SSBoard:
                    case CommandList.BoardRepair:
                    case CommandList.SelfDestruct:
						if (ord.Variable1 < 4) ord.Variable1 = (byte)(ord.Variable1 * 5);
						else if (ord.Variable1 < 180) ord.Variable1 = (byte)(ord.Variable1 + 16);
						else ord.Variable1 = (byte)(ord.Variable1 / 2 + 106);
                        break;
                    default: break;
                }
				if (ord.Target1Type == (byte)Mission.Trigger.TypeList.ShipType && ord.Target1 != 255) ord.Target1++;
                if (ord.Target2Type == (byte)Mission.Trigger.TypeList.ShipType && ord.Target2 != 255) ord.Target2++;
                if (ord.Target3Type == (byte)Mission.Trigger.TypeList.ShipType && ord.Target3 != 255) ord.Target3++;
                if (ord.Target4Type == (byte)Mission.Trigger.TypeList.ShipType && ord.Target4 != 255) ord.Target4++;
                return ord;
			}
			
			#region public properties
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown6
			{
				get => _items[4];
				set => _items[4] = value;
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown7
			{
				get => _items[5];
				set => _items[5] = value;
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x0B</remarks>
			public byte Unknown8
			{
				get => _items[11];
				set => _items[11] = value;
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x11</remarks>
			public byte Unknown9
			{
				get => _items[17];
				set => _items[17] = value;
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get => _items[18];
				set => _items[18] = value;
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 16 characters</remarks>
			public string Designation
			{
				get => _designation;
				set => _designation = StringFunctions.GetTrimmed(value, 16);
			}
			#endregion public properties
		}
	}
}
