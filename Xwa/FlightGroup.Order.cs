/*
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
 * [UPD] AndOr renamed
 * v6.1, 231208
 * [NEW] CommandList enum
 * [FIX] Time calculation for target string
 * [FIX] Convert times to TIE/XvT
 * v5.8, 230804
 * [UPD] Region references fixed
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v5.5, 2108001
 * [UPD] SS Patrol and SS Await Return order strings now show target info [JB]
 * [FIX] CraftType errors in strings [JB]
 * [UPD] Hyper to Region order text updated with token [JB]
 * v5.0, 201004
 * [UPD] GG references prepped for string replacement [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * v3.0, 180903
 * [UPD] now init to TRUE [JB]
 * [UPD] case 14 added [JB]
 * [UPD] Strings calls swapped to SafeString [JB]
 * [NEW] helper functions for swap/delete FG or Message [JB]
 * [NEW] helper functions for Skips [JB]
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
		/// <summary>Container for a single Order.</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _customText = "";
			readonly XwaWaypoint[] _waypoints = new XwaWaypoint[8];
			readonly Mission.Trigger[] _skipTriggers = new Mission.Trigger[2];

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
				/// <summary>Stationary.</summary>
				Hold19,
				/// <summary>Loop through waypoints and attack targets.</summary>
				SSPatrolAttack,
				/// <summary>Loop through waypoints and attack to disable targets.</summary>
				SSPatrolDisable,
				/// <summary>Stationary.</summary>
				Hold1C,
				/// <summary>Return to Mothership or hyperspace (Starship).</summary>
				SSGoHome,
				/// <summary>Stationary.</summary>
				Hold1E,
				/// <summary>Boards target (Starship).</summary>
				SSBoard,
				/// <summary>Boards target to repair systems.</summary>
				BoardRepair,
				/// <summary>Stationary.</summary>
				Hold21,
				/// <summary>Stationary.</summary>
				Hold22,
				/// <summary>Stationary.</summary>
				Hold23,
				/// <summary>Destroys self.</summary>
				SelfDestruct,
				/// <summary>Rams target.</summary>
				Kamikaze,
				/// <summary>Orbits origin.</summary>
				Orbit,
				/// <summary>Stationary.</summary>
				ReleaseCarried,
				/// <summary>Drop off specific FG.</summary>
				Deliver,
				/// <summary>Unknown.</summary>
				LoadObject,
				/// <summary>Attack from a distance.</summary>
				StandoffAttack,
				/// <summary>Unknown.</summary>
				Backup,
				/// <summary>Unknown.</summary>
				Shutdown,
				/// <summary>Repaired after an amount of time.</summary>
				RepairSelf,
				/// <summary>Changes IFF.</summary>
				Defect,
				/// <summary>Changes IFF and considered Captured.</summary>
				Surrender,
				/// <summary>Unknown.</summary>
				Make,
				/// <summary>Hyperbuoy order.</summary>
				Beacon,
				/// <summary>Hyper to region via appropriate hyperbuoy.</summary>
				HyperToRegion,
				/// <summary>Unknown.</summary>
				Relaunch,
				/// <summary>Unknown.</summary>
				TransferCargo,
				/// <summary>Unknown.</summary>
				InspectTargets,
				/// <summary>Unknown.</summary>
				AwaitAssembly,
				/// <summary>Unknown.</summary>
				AwaitDisassembly,
				/// <summary>Unknown.</summary>
				ConstructTrain,
				/// <summary>Unknown.</summary>
				Park,
				/// <summary>Unknown.</summary>
				BoardToDefuse,
				/// <summary>Unknown.</summary>
				StartOver,
				/// <summary>Unknown.</summary>
				TakeApartTrain,
				/// <summary>Unknown.</summary>
				WorkOn,
				/// <summary>Unknown.</summary>
				DockToLoad,
				/// <summary>Unknown.</summary>
				FollowTargets,
				/// <summary>Unknown.</summary>
				HomeIn
			}

			#region constructors
			/// <summary>Initializes a blank Order.</summary>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b>, <see cref="SkipTriggers"/> sets to <b>"Always AND Always"</b>.</remarks>
			public Order()
			{
				_items = new byte[20];
				Throttle = 10;
				T1OrT2 = T3OrT4 = true;
				initialize();
			}
			/// <summary>Initializes a new Order from an existing Order.</summary>
			/// <param name="other">Existing Order to clone. If <b>null</b>, Order will be blank.</param>
			/// <remarks><see cref="BaseFlightGroup.BaseOrder.Throttle"/> set to <b>100%</b>, AndOr values set to <b>"Or"</b>,<see cref="SkipTriggers"/> sets to <b>"Always AND Always"</b> if <paramref name="other"/> is <b>null</b>.</remarks>
			public Order(Order other) : this()
			{
				if (other != null)
				{
					Array.Copy(other._items, _items, _items.Length);
					_customText = other.CustomText;
					for (int i = 0; i < _waypoints.Length; i++)
						_waypoints[i] = new XwaWaypoint(other._waypoints[i]);
					for (int i = 0; i < _skipTriggers.Length; i++)
						_skipTriggers[i] = new Mission.Trigger(other._skipTriggers[i]);
				}
			}

			/// <summary>Initializes a new Order from raw data.</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"Always AND Always"</b>.<br/>
			/// If <paramref name="raw"/>.Length is 20 or greater, only reads first 20 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of 18.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length.</exception>
			public Order(byte[] raw) : this()
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				if (raw.Length >= 20) Array.Copy(raw, _items, _items.Length);
				else Array.Copy(raw, _items, raw.Length);
				initialize();
			}

			/// <summary>Initialize a new Order from raw data.</summary>
			/// <remarks><see cref="SkipTriggers"/> sets to <b>"Always AND Always"</b>.<br/>
			/// If <paramref name="raw"/>.Length is 20 or greater, only reads 20 bytes.</remarks>
			/// <param name="raw">Raw byte data, minimum Length of <b>18</b>.</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading. If raw.Length &lt; 20, must equal <b>zero</b>.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length value.</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/>.</exception>
			public Order(byte[] raw, int startIndex) : this()
			{
				if (raw.Length < 18) throw new ArgumentException("Minimum length of raw is 18", "raw");
				if (raw.Length >= 20)
				{
					if (raw.Length - startIndex < 20 || startIndex < 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 20));
					ArrayFunctions.TrimArray(raw, startIndex, _items);
				}
				else
				{
					if (startIndex != 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0");
					Array.Copy(raw, _items, raw.Length);
				}
				initialize();
			}
			
			void initialize()
			{
				for (int i = 0; i < 8; i++) _waypoints[i] = new XwaWaypoint();
				_skipTriggers[0] = new Mission.Trigger();
				_skipTriggers[1] = new Mission.Trigger();
                // SkipTriggers used to default to False OR False
				//[JB] For some reason these must be set to always(TRUE) for the orders to function properly in game, otherwise it will skip over orders and behave unexpectedly.
			}
			#endregion constructors

			static string orderTargetString(byte target, byte type)
			{
				string s = "";
				switch ((Mission.Trigger.TypeList)type)
				{
					case Mission.Trigger.TypeList.None:
						break;
					case Mission.Trigger.TypeList.FlightGroup:
						s = "FG:" + target;
						break;
					case Mission.Trigger.TypeList.ShipType:
						s = BaseStrings.SafeString(Strings.CraftType, target) + "s";
						break;
					case Mission.Trigger.TypeList.ShipClass:
						s = BaseStrings.SafeString(Strings.ShipClass, target);
						break;
					case Mission.Trigger.TypeList.ObjectType:
						s = BaseStrings.SafeString(Strings.ObjectType, target);
						break;
					case Mission.Trigger.TypeList.IFF:
						s = BaseStrings.SafeString(Strings.IFF, target) + "s";
						break;
					case Mission.Trigger.TypeList.ShipOrders:
						s = "Craft with " + BaseStrings.SafeString(Strings.Orders, target) + " orders";
						break;
					case Mission.Trigger.TypeList.CraftWhen:
						s = "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, target);
						break;
					case Mission.Trigger.TypeList.GlobalGroup:
						s = "GG:" + target;
						break;
                    case Mission.Trigger.TypeList.AILevel:
                        s = "Rating " + BaseStrings.SafeString(Strings.Rating, target);
                        break;
                    case Mission.Trigger.TypeList.Status:
                        s = "Craft with status: " + BaseStrings.SafeString(Strings.Status, target);
                        break;
                    case Mission.Trigger.TypeList.AllCraft:
                        s = "All";
                        break;
                    case Mission.Trigger.TypeList.Team:
                        s = "TM:" + target;
                        break;
                    case Mission.Trigger.TypeList.PlayerNum:
                        s = "Player #" + target;
                        break;
                    case Mission.Trigger.TypeList.BeforeTime:
                        s = "before time " + string.Format("{0}:{1:00}", Mission.GetDelaySeconds(target) / 60, Mission.GetDelaySeconds(target) % 60);
                        break;
                    case Mission.Trigger.TypeList.NotFG:
                        s = "Not FG:" + target;
                        break;
                    case Mission.Trigger.TypeList.NotShipType:
                        s = "Not ship type " + BaseStrings.SafeString(Strings.CraftType, target);
                        break;
                    case Mission.Trigger.TypeList.NotShipClass:
                        s = "Not ship class " + BaseStrings.SafeString(Strings.ShipClass, target);
                        break;
                    case Mission.Trigger.TypeList.NotObjectType:
                        s = "Not object type " + BaseStrings.SafeString(Strings.ObjectType, target);
                        break;
                    case Mission.Trigger.TypeList.NotIFF:
                        s = "Not IFF " + BaseStrings.SafeString(Strings.IFF, target);
                        break;
                    case Mission.Trigger.TypeList.NotGlobalGroup:
                        s = "Not GG:" + target;
                        break;
                    case Mission.Trigger.TypeList.NotTeam:
                        s = "All Teams except TM:" + target;
                        break;
                    case Mission.Trigger.TypeList.NotPlayerNum:
                        s = "Not player #" + target;
                        break;
                    case Mission.Trigger.TypeList.GlobalUnit:
                        s = "Global Unit " + target;
                        break;
                    case Mission.Trigger.TypeList.NotGlobalUnit:
                        s = "Not Global Unit " + target;
                        break;
                    case Mission.Trigger.TypeList.GlobalCargo:
                        s = "Global Cargo " + target;
                        break;
                    case Mission.Trigger.TypeList.NotGlobalCargo:
                        s = "Not Global Cargo " + target;
                        break;
                    case Mission.Trigger.TypeList.MessageNum:
                        s = "Message #" + (target + 1);  //[JB] Now one-based for consistency with other message displays
                        break;
                    default:
                        s = type + " " + target;
                        break;
   					}
				return s;
			}

			/// <summary>Returns a representative string of the Order.</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, Teams are identified as <b>"TM:#"</b> and Regions are <b>"REG:#"</b> for later substitution if required.</remarks>
			/// <returns>Description of the order and targets if applicable, otherwise <b>"None"</b>.</returns>
			public override string ToString()
			{
				if (Command == 0) return "None";
				string order = BaseStrings.SafeString(Strings.Orders, Command);
				if ((Command >= (byte)CommandList.AttackTargets && Command <= (byte)CommandList.DropOff)
					|| (Command >= (byte)CommandList.SSPatrol && Command <= (byte)CommandList.SSPatrolDisable)
					|| Command == (byte)CommandList.SSBoard || Command == (byte)CommandList.BoardRepair || Command == (byte)CommandList.Kamikaze) //all orders where targets are important
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
				else if (Command == (byte)CommandList.HyperToRegion)
				{
					order += " REG:" + (Variable1 + 1);
				}
				return order;
			}

			/// <summary>Converts an order to a byte array.</summary>
			/// <remarks><see cref="CustomText"/>, <see cref="Waypoints"/>, and <see cref="SkipTriggers"/> are lost in the conversion.</remarks>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A byte array of Length 20 containing the Order data.</returns>
			public static explicit operator byte[](Order ord) => ord.GetBytes();
			/// <summary>Converts an Order for TIE.</summary>
			/// <remarks><see cref="Speed"/>, <see cref="CustomText"/>, <see cref="Waypoints"/>, and <see cref="SkipTriggers"/> are lost in the conversion.</remarks>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A copy of <paramref name="ord"/> for use in TIE95.</returns>
			public static explicit operator Tie.FlightGroup.Order(Order ord)
			{
				var newOrder = new Tie.FlightGroup.Order(ord.GetBytes());
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
					case CommandList.Hold19:
					case CommandList.Hold1E:
					case CommandList.Hold21:
					case CommandList.Hold22:
					case CommandList.Hold23:
					case CommandList.SSBoard:
					case CommandList.BoardRepair:
					case CommandList.BoardToDefuse:
						int time = Mission.GetDelaySeconds(newOrder.Variable1) / 5;
						if (time > 255) newOrder.Variable1 = 255;
						else newOrder.Variable1 = (byte)time;
						break;
					default: break;
				}
				if (newOrder.Target1Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target1 != 0) newOrder.Target1--;
				if (newOrder.Target2Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target2 != 0) newOrder.Target2--;
				if (newOrder.Target3Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target3 != 0) newOrder.Target3--;
				if (newOrder.Target4Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target4 != 0) newOrder.Target4--;
				return newOrder;
			}
			/// <summary>Converts an Order for XvT.</summary>
			/// <remarks><see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion. <see cref="CustomText"/> trimmed to 16 characters.</remarks>
			/// <param name="ord">The Order to convert.</param>
			/// <returns>A copy of <paramref name="ord"/> for use in XvT.</returns>
			public static explicit operator Xvt.FlightGroup.Order(Order ord)
			{
				var newOrder = new Xvt.FlightGroup.Order(ord.GetBytes())
				{
					Designation = ord.CustomText
				};
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
					case CommandList.Hold19:
					case CommandList.Hold1E:
					case CommandList.Hold21:
					case CommandList.Hold22:
					case CommandList.Hold23:
					case CommandList.SSBoard:
					case CommandList.BoardRepair:
					case CommandList.SelfDestruct:
					case CommandList.BoardToDefuse:
						int time = Mission.GetDelaySeconds(newOrder.Variable1) / 5;
						if (time > 255) newOrder.Variable1 = 255;
						else newOrder.Variable1 = (byte)time;
						break;
					default: break;
				}
				if (newOrder.Target1Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target1 != 0) newOrder.Target1--;
				if (newOrder.Target2Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target2 != 0) newOrder.Target2--;
				if (newOrder.Target3Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target3 != 0) newOrder.Target3--;
				if (newOrder.Target4Type == (byte)Mission.Trigger.TypeList.ShipType && newOrder.Target4 != 0) newOrder.Target4--;
				return newOrder;
			}
			
			#region public properties
			/// <summary>Gets or sets the specific max velocity.</summary>
			public byte Speed
			{
				get => _items[18];
				set => _items[18] = value;
			}
			/// <summary>Gets or sets the order description.</summary>
			/// <remarks>Limited to 63 characters.</remarks>
			public string CustomText
			{
				get => _customText;
				set => _customText = StringFunctions.GetTrimmed(value, 63);
			}

			/// <summary>Whether or not the Skip Triggers are exclusive.</summary>
			public bool SkipT1OrT2 { get; set; }

			/// <summary>Gets the order-specific location markers.</summary>
			/// <remarks>Array is length 8.</remarks>
			public XwaWaypoint[] Waypoints => _waypoints;

			/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to the order.</summary>
			/// <remarks>Array is length 2.</remarks>
			public Mission.Trigger[] SkipTriggers => _skipTriggers;
			#endregion public properties

			/// <summary>Helper function that updates FG indexes during move/delete operations.</summary>
			/// <param name="srcIndex">The original FG index.</param>
			/// <param name="dstIndex">The new FG index.</param>
			/// <returns><b>true</b> on successful change.</returns>
			protected override bool TransformFGReferencesExtended(int srcIndex, int dstIndex)
            {
                bool change = false;

                byte dst = (byte)dstIndex;
                bool delete = false;
                if (dstIndex < 0)
                {
                    dst = 0;
                    delete = true;
                } 
                if (Target1Type == (byte)Mission.Trigger.TypeList.NotFG && Target1 == srcIndex) { change = true; Target1 = dst; if (delete) Target1Type = 0; }
				else if (Target1Type == (byte)Mission.Trigger.TypeList.NotFG && Target1 > srcIndex && delete == true) { change = true; Target1--; }
                if (Target2Type == (byte)Mission.Trigger.TypeList.NotFG && Target2 == srcIndex) { change = true; Target2 = dst; if (delete) Target2Type = 0; }
				else if (Target2Type == (byte)Mission.Trigger.TypeList.NotFG && Target2 > srcIndex && delete == true) { change = true; Target2--; }
                if (Target3Type == (byte)Mission.Trigger.TypeList.NotFG && Target3 == srcIndex) { change = true; Target3 = dst; if (delete) Target3Type = 0; }
				else if (Target3Type == (byte)Mission.Trigger.TypeList.NotFG && Target3 > srcIndex && delete == true) { change = true; Target3--; }
                if (Target4Type == (byte)Mission.Trigger.TypeList.NotFG && Target4 == srcIndex) { change = true; Target4 = dst; if (delete) Target4Type = 0; }
				else if (Target4Type == (byte)Mission.Trigger.TypeList.NotFG && Target4 > srcIndex && delete == true) { change = true; Target4--; }

                foreach (Mission.Trigger trig in SkipTriggers)
                    change |= trig.TransformFGReferences(srcIndex, dstIndex, true);
                return change;
            }

            /// <summary>Changes all Message indexes, to be used during a Message Swap (Move) or Delete operation.</summary>
            /// <remarks>Same concept as for Flight Groups.  Triggers may depend on Message indexes, and this function helps ensure indexes are not broken.</remarks>
            /// <param name="srcIndex">The Message index to match and replace (Move), or match and Delete.</param>
            /// <param name="dstIndex">The Message index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
            /// <returns><b>true</b> if something was changed.</returns>
            public bool TransformMessageReferences(int srcIndex, int dstIndex)
            {
                //[JB] I have no idea if message #s are actually used in Order triggers, but since I'm trying to keep any kind of indexes from being corrupted by move/delete operations, I thought I might as well do this too, just to be safe.
                bool change = false;
                bool delete = false;
                if (dstIndex < 0)
                {
                    dstIndex = 0;
                    delete = true;
                }
                else if (dstIndex > 255) throw new Exception("TransformMessagesReferences: dstIndex out of range.");

                if (Target1Type == (byte)Mission.Trigger.TypeList.MessageNum && Target1 == srcIndex) { change = true; Target1 = (byte)dstIndex; if (delete) Target1Type = 0; }
				else if (Target1Type == (byte)Mission.Trigger.TypeList.MessageNum && Target1 > srcIndex && delete == true) { change = true; Target1--; }
                if (Target2Type == (byte)Mission.Trigger.TypeList.MessageNum && Target2 == srcIndex) { change = true; Target2 = (byte)dstIndex; if (delete) Target2Type = 0; }
				else if (Target2Type == (byte)Mission.Trigger.TypeList.MessageNum && Target2 > srcIndex && delete == true) { change = true; Target2--; }
                if (Target3Type == (byte)Mission.Trigger.TypeList.MessageNum && Target3 == srcIndex) { change = true; Target3 = (byte)dstIndex; if (delete) Target3Type = 0; }
				else if (Target3Type == (byte)Mission.Trigger.TypeList.MessageNum && Target3 > srcIndex && delete == true) { change = true; Target3--; }
                if (Target4Type == (byte)Mission.Trigger.TypeList.MessageNum && Target4 == srcIndex) { change = true; Target4 = (byte)dstIndex; if (delete) Target4Type = 0; }
				else if (Target4Type == (byte)Mission.Trigger.TypeList.MessageNum && Target4 > srcIndex && delete == true) { change = true; Target4--; }

                return change;
            }
			/// <summary>Change references.</summary>
			/// <param name="srcIndex">First index.</param>
			/// <param name="dstIndex">Second index.</param>
			/// <returns><b>true</b> if anything is changed.</returns>
            public bool SwapMessageReferences(int srcIndex, int dstIndex)
            {
                bool change = false;
                change |= TransformMessageReferences(dstIndex, 255);
                change |= TransformMessageReferences(srcIndex, dstIndex);
                change |= TransformMessageReferences(255, srcIndex);
                return change;
            }

			/// <summary>Checks if the Skip Trigger is potentially used.</summary>
			/// <returns><b>true</b> if the Skip trigger can be used.</returns>
			/// <remarks>Null triggers (Condition, VariableType, Variable, Amount all zero) are usually ignored by the game.</remarks>
            public bool IsSkipTriggerActive()
            {
                bool active = false;
                foreach(Mission.Trigger trig in SkipTriggers)
                {
                    bool test = (trig.Condition != 0 || trig.VariableType != 0 || trig.Variable != 0 || trig.Amount != 0);
                    active |= test;
                }
                return active;
            }

			/// <summary>Checks if the Skip Trigger is in a state that will never fire.</summary>
			/// <returns><b>true</b> if the Skip is impossible.</returns>
			/// <remarks>Checks to make sure a trigger does not use a FALSE condition paired (AND) with True.</remarks>
			public bool IsSkipTriggerBroken() => ((SkipTriggers[0].Condition == (byte)Mission.Trigger.ConditionList.False || SkipTriggers[1].Condition == (byte)Mission.Trigger.ConditionList.False) && SkipT1OrT2 == false);

			/// <summary>Check if the Order is used.</summary>
			/// <returns><b>true</b> if the Order is used.</returns>
			/// <remarks>An order will not be processed AT ALL if both Skip Triggers are set to <b>false</b>.</remarks>
			public bool IsOrderUsed() => (SkipTriggers[0].Condition == (byte)Mission.Trigger.ConditionList.False && SkipTriggers[1].Condition == (byte)Mission.Trigger.ConditionList.False);
			//TODO XWA: Need to verify use in YOGEME to make sure this is done right
		}
	}
}