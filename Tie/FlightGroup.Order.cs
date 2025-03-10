﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [UPD] Format spec implemented
 * v6.1, 231208
 * [NEW] CommandList enum
 * [FIX] Convert times for XWA
 * v5.7.5, 230116
 * [UPD #12] Importing order values past Board to Repair (0x20) will reset to zero to match TIE behavior
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v5.5, 2108001
 * [UPD] SS Patrol and SS Await Return order strings now show target info [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * v2.5, 170107
 * [FIX] hack _checkValues [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * [FIX] _checkValues reverts TargetType 0A to 00 (LA sloppiness, caused load fail on certain missions)
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
		/// <summary>Object for a single Order.</summary>
		[Serializable] public class Order : BaseOrder
		{
			/// <summary>Available orders.</summary>
			public enum CommandList : byte
			{
				/// <summary>Stationary. Go Home if not first order.</summary>
				HoldSteady,
				/// <summary>Return to Mothership or hyperspace.</summary>
				GoHome,
				/// <summary>Loop through waypoints.</summary>
				Circle,
				/// <summary>Loop through waypoints and evade.</summary>
				CircleEvade,
				/// <summary>Fly to RDV and await docking.</summary>
				Rendezvous,
				/// <summary>Disabled.</summary>
				Disabled,
				/// <summary>Disabled, awaiting boarding.</summary>
				AwaitBoarding,
				/// <summary>Attack targets.</summary>
				AttackTargets,
				/// <summary>Attack target's escorts.</summary>
				AttackEscorts,
				/// <summary>Attack target's attackers.</summary>
				Protect,
				/// <summary>Attack target's attackers and boarding craft.</summary>
				Escort,
				/// <summary>Attack to disable targets.</summary>
				DisableTargets,
				/// <summary>Board targets to give cargo.</summary>
				BoardGiveCargo,
				/// <summary>Board targets to take cargo.</summary>
				BoardTakeCargo,
				/// <summary>Board targets to exchange cargo.</summary>
				BoardExchangeCargo,
				/// <summary>Board to capture targets.</summary>
				BoardToCapture,
				/// <summary>Board targets to destroy.</summary>
				BoardDestroy,
				/// <summary>Pickup and carry target.</summary>
				PickUp,
				/// <summary>Drops off designated FG.</summary>
				DropOff,
				/// <summary>Wait for a time.</summary>
				Wait,
				/// <summary>Wait for a time (Starship).</summary>
				SSWait,
				/// <summary>Loop through waypoints (Starship).</summary>
				SSPatrol,
				/// <summary>Wait for return of all craft that use FG as Mothership.</summary>
				SSAwaitReturn,
				/// <summary>Wait for launch of all craft that use FG as Mothership.</summary>
				SSLaunch,
				/// <summary>Loop through waypoints and attack target's attackers.</summary>
				SSProtect,
				/// <summary>Wait and attack target's attackers.</summary>
				SSWaitProtect,
				/// <summary>Loop through waypoints and attack targets.</summary>
				SSPatrolAttack,
				/// <summary>Loop through waypoints and attack to disable targets.</summary>
				SSPatrolDisable,
				/// <summary>Stationary (Starship).</summary>
				SSHold,
				/// <summary>Return to Mothership or hyperspace (Starship).</summary>
				SSGoHome,
				/// <summary>Wait for a time (Starship).</summary>
				SSWait2,
				/// <summary>Boards target (Starship).</summary>
				SSBoard,
				/// <summary>Boards target to repair systems.</summary>
				BoardRepair
			}

			#region constructors
			/// <summary>Initializes a blank order.</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to 100%, AndOr values to <b>true</b>.</remarks>
			public Order()
			{
				_items = new byte[18];
				Throttle = 10;
				T1OrT2 = T3OrT4 = true;
			}
			/// <summary>Initializes a new Order from an existing Order.</summary>
			/// <param name="other">Existing Order to clone. If <b>null</b>, Order will be blank.</param>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b> if <paramref name="other"/> is <b>null</b>.</remarks>
			public Order(Order other) : this()
			{
				if (other != null)
					Array.Copy(other._items, _items, _items.Length);
			}
			
			/// <summary>Initializes the order from raw data.</summary>
			/// <param name="raw">Raw byte data, minimum Length of 18.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length value<br/><b>-or-</b><br/>Invalid member values.</exception>
			public Order(byte[] raw) : this()
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				Array.Copy(raw, _items, _items.Length);
				checkValues(this);
			}
			
			/// <summary>Initializes the order from raw data.</summary>
			/// <param name="raw">Raw byte data, minimum Length of 18.</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length value<br/><b>-or-</b><br/>Invalid member values.</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/>.</exception>
			public Order(byte[] raw, int startIndex) : this()
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				if (raw.Length - startIndex < 18 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 18));
				ArrayFunctions.TrimArray(raw, startIndex, _items);
				checkValues(this);
			}
			#endregion
			
			static void checkValues(Order o)
			{
				string error = "";
				byte tempVar;
				if (o.Command > (byte)CommandList.BoardRepair) o.Command = 0;	// How TIE does it, also bypasses error in HI1W
				if (o.Target1Type == (byte)Mission.Trigger.TypeList.Status) o.Target1Type = o.Target1 = 0;
				if (o.Target2Type == (byte)Mission.Trigger.TypeList.Status) o.Target2Type = o.Target2 = 0;
				if (o.Target3Type == (byte)Mission.Trigger.TypeList.Status) o.Target3Type = o.Target3 = 0;
				if (o.Target4Type == (byte)Mission.Trigger.TypeList.Status) o.Target4Type = o.Target4 = 0;
				if (o.Target3Type == (byte)Mission.Trigger.TypeList.ShipClass && o.Target3 > 6) { o.Target3Type = 0; o.Target3 = 0; }; //HACK: [JB] fix TIE DOS B2M3IW.TIE
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
						s += "IFF:" + target;
						break;
					case Mission.Trigger.TypeList.ShipOrders:
						s += "Craft with " + BaseStrings.SafeString(Strings.Orders, target) + " orders";
						break;
					case Mission.Trigger.TypeList.CraftWhen:
						s += "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, target);
						break;
					case Mission.Trigger.TypeList.GlobalGroup:
						s += "Global Group " + target;
						break;
					default:
						s += type + " " + target;
						break;
				}
				return s;
			}
			
			/// <summary>Returns a representative string of the Order.</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> for later substitution if required.</remarks>
			/// <returns>Description of the order and targets if applicable, otherwise <b>"None"</b>.</returns>
			public override string ToString()
			{
				if (Command == 0) return "None";
				string order = BaseStrings.SafeString(Strings.Orders, Command);
				if ((Command >= (byte)CommandList.AttackTargets && Command <= (byte)CommandList.DropOff)
					|| (Command >= (byte)CommandList.SSPatrol && Command <= (byte)CommandList.SSPatrolDisable)
					|| Command == (byte)CommandList.SSBoard || Command == (byte)CommandList.BoardRepair)	//all orders where targets are important
				{
					string s = orderTargetString(Target1, Target1Type);
					string s2 = orderTargetString(Target2, Target2Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T1OrT2) order += " or " + s2;
						else order += " if " + s2;
					}
					s = orderTargetString(Target3, Target3Type);
					s2 = orderTargetString(Target4, Target4Type);
					if (s != "") order += ", " + s;
					if (s != "" && s2 != "")
					{
						if (T3OrT4) order += " or " + s2;
						else order += " if " + s2;
					}
				}
				return order;
			}

			/// <summary>Converts an Order into a byte array.</summary>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A byte array of length 18 containing the Order data.</returns>
			public static explicit operator byte[](Order ord) => ord.GetBytes();
			/// <summary>Converts an Order for XvT.</summary>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A copy of <paramref name="ord"/> for XvT.</returns>
			public static implicit operator Xvt.FlightGroup.Order(Order ord) => new Xvt.FlightGroup.Order(ord.GetBytes());
			/// <summary>Converts an Order for XWA.</summary>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A copy of <paramref name="ord"/> for XWA.</returns>
			public static implicit operator Xwa.FlightGroup.Order(Order ord)
			{
				var newOrder = new Xwa.FlightGroup.Order(ord.GetBytes());
				//TIE time is value*5=sec
				//XWA time value, if 20 (decimal) or under is exact seconds, then +5 up to 15:00, then +10.
				//21 = 25 sec, 22 = 30 sec, etc.
				switch ((CommandList)newOrder.Command)
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
					case CommandList.SSHold:
					case CommandList.SSWait2:
					case CommandList.SSBoard:
					case CommandList.BoardRepair:
						if (newOrder.Variable1 < 4) newOrder.Variable1 = (byte)(newOrder.Variable1 * 5);
						else if (newOrder.Variable1 < 180) newOrder.Variable1 = (byte)(newOrder.Variable1 + 16);
						else newOrder.Variable1 = (byte)(newOrder.Variable1 / 2 + 106);
						break;
					default: break;
				}
				if (newOrder.Target1Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target1 != 255) newOrder.Target1++;
				if (newOrder.Target2Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target2 != 255) newOrder.Target2++;
				if (newOrder.Target3Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target3 != 255) newOrder.Target3++;
				if (newOrder.Target4Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target4 != 255) newOrder.Target4++;
				return newOrder;
			}
		}
	}
}