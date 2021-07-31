/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.0
 */

/* CHANGELOG
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
		/// <summary>Container for a single Order</summary>
		[Serializable] public class Order : BaseOrder
		{
			string _customText = "";
			readonly Waypoint[] _waypoints = new Waypoint[8];
			readonly Mission.Trigger[] _skipTriggers = new Mission.Trigger[2];
			
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
                //[JB] This modified code is redundant since everything is initialized to zero by default.
				//_skipTriggers[0].Condition = 0;  //[JB] For some reason these must be set to always(TRUE) for the orders to function properly in game, otherwise it will skip over orders and behave unexpectedly.
				//_skipTriggers[1].Condition = 0;  
				SkipT1AndOrT2 = false;  //[JB] Set to AND
			}
			#endregion constructors

			static string orderTargetString(byte target, byte type)
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
						s = BaseStrings.SafeString(Strings.CraftType, target) + "s";
						break;
					case 3:
						s = BaseStrings.SafeString(Strings.ShipClass, target);
						break;
					case 4:
						s = BaseStrings.SafeString(Strings.ObjectType, target);
						break;
					case 5:
						s = BaseStrings.SafeString(Strings.IFF, target) + "s";
						break;
					case 6:
						s = "Craft with " + BaseStrings.SafeString(Strings.Orders, target) + " orders";
						break;
					case 7:
						s = "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, target);
						break;
					case 8:
						s = "GG:" + target;
						break;
                    case 9:
                        s = "Rating " + BaseStrings.SafeString(Strings.Rating, target);
                        break;
                    case 10:
                        s = "Craft with status: " + BaseStrings.SafeString(Strings.Status, target);
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
                    case 14:
                        s = "before time " + string.Format("{0}:{1:00}", target * 5 / 60, target * 5 % 60);
                        break;
                    case 15:
                        s = "Not FG:" + target;
                        break;
                    case 16:
                        s = "Not ship type " + BaseStrings.SafeString(Strings.CraftType, target);
                        break;
                    case 17:
                        s = "Not ship class " + BaseStrings.SafeString(Strings.ShipClass, target);
                        break;
                    case 18:
                        s = "Not object type " + BaseStrings.SafeString(Strings.ObjectType, target);
                        break;
                    case 19:
                        s = "Not IFF " + BaseStrings.SafeString(Strings.IFF, target);
                        break;
                    case 20:
                        s = "Not GG:" + target;
                        break;
                    case 21:
                        s = "All Teams except TM:" + target;
                        break;
                    case 22:
                        s = "Not player #" + target;
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
                        s = "Message #" + (target + 1);  //[JB] Now one-based for consistency with other message displays
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
				string order = BaseStrings.SafeString(Strings.Orders, Command);
				if ((Command >= 7 && Command <= 18) || (Command >= 21 && Command <= 27) || Command == 31 || Command == 32 || Command == 37) //all orders where targets are important
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
				else if (Command == 50)  // Hyper to Region
				{
					order += " REG:" + this.Variable1;
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
			/// <returns>A copy of <paramref name="ord"/> for use in TIE95</returns>
			public static explicit operator Tie.FlightGroup.Order(Order ord)
			{
				// Speed, CustomText, WPs, Skips lost
				return new Tie.FlightGroup.Order((byte[])ord, 0);
			}
			/// <summary>Converts an Order for XvT</summary>
			/// <remarks><see cref="Waypoints"/> and <see cref="SkipTriggers"/> are lost in the conversion. <see cref="CustomText"/> trimmed to 16 characters</remarks>
			/// <param name="ord">The Order to convert</param>
			/// <returns>A copy of <paramref name="ord"/> for use in XvT</returns>
			public static explicit operator Xvt.FlightGroup.Order(Order ord)
			{
				// WPs, Skips lost
				// CustomText trimmed to 16 char
				Xvt.FlightGroup.Order newOrder = new Xvt.FlightGroup.Order((byte[])ord)
				{
					Designation = ord.CustomText
				};
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

            /// <summary>Helper function that updates FG indexes during move/delete operations.</summary>
			/// <param name="srcIndex">The original FG index</param>
			/// <param name="dstIndex">The new FG index</param>
			/// <returns><b>true</b> on successful change</returns>
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
                //All FGs except
                if (Target1Type == 15 && Target1 == srcIndex) { change = true; Target1 = dst; if (delete) Target1Type = 0; } else if (Target1Type == 15 && Target1 > srcIndex && delete == true) { change = true; Target1--; }
                if (Target2Type == 15 && Target2 == srcIndex) { change = true; Target2 = dst; if (delete) Target2Type = 0; } else if (Target2Type == 15 && Target2 > srcIndex && delete == true) { change = true; Target2--; }
                if (Target3Type == 15 && Target3 == srcIndex) { change = true; Target3 = dst; if (delete) Target3Type = 0; } else if (Target3Type == 15 && Target3 > srcIndex && delete == true) { change = true; Target3--; }
                if (Target4Type == 15 && Target4 == srcIndex) { change = true; Target4 = dst; if (delete) Target4Type = 0; } else if (Target4Type == 15 && Target4 > srcIndex && delete == true) { change = true; Target4--; }

                foreach (Mission.Trigger trig in SkipTriggers)
                    change |= trig.TransformFGReferences(srcIndex, dstIndex, true);
                return change;
            }

            /// <summary>Changes all Message indexes, to be used during a Message Swap (Move) or Delete operation.</summary>
            /// <remarks>Same concept as for Flight Groups.  Triggers may depend on Message indexes, and this function helps ensure indexes are not broken.</remarks>
            /// <param name="srcIndex">The Message index to match and replace (Move), or match and Delete.</param>
            /// <param name="dstIndex">The Message index to replace with.  Specify -1 to Delete, or zero or above to Move.</param>
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

                if (Target1Type == 27 && Target1 == srcIndex) { change = true; Target1 = (byte)dstIndex; if (delete) { Target1Type = 0; } } else if (Target1Type == 27 && Target1 > srcIndex && delete == true) { change = true; Target1--; }
                if (Target2Type == 27 && Target2 == srcIndex) { change = true; Target2 = (byte)dstIndex; if (delete) { Target2Type = 0; } } else if (Target2Type == 27 && Target2 > srcIndex && delete == true) { change = true; Target2--; }
                if (Target3Type == 27 && Target3 == srcIndex) { change = true; Target3 = (byte)dstIndex; if (delete) { Target3Type = 0; } } else if (Target3Type == 27 && Target3 > srcIndex && delete == true) { change = true; Target3--; }
                if (Target4Type == 27 && Target4 == srcIndex) { change = true; Target4 = (byte)dstIndex; if (delete) { Target4Type = 0; } } else if (Target4Type == 27 && Target4 > srcIndex && delete == true) { change = true; Target4--; }

                return change;
            }
			/// <summary>Change references</summary>
			/// <param name="srcIndex">First index</param>
			/// <param name="dstIndex">Second index</param>
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
			/// <returns><b>true</b> if the Skip trigger can be used</returns>
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
			/// <returns><b>true</b> if the Skip is impossible</returns>
			/// <remarks>Checks to make sure a trigger does not use a FALSE condition paired (AND) with True.</remarks>
            public bool IsSkipTriggerBroken()
            {
                return ((SkipTriggers[0].Condition == 10 || SkipTriggers[1].Condition == 10) && SkipT1AndOrT2 == false);
            }

			/// <summary>Check if the Order is used.</summary>
			/// <returns><b>true</b> if the Order is used</returns>
			/// <remarks>An order will not be processed AT ALL if both Skip Triggers are set to <b>false</b>.</remarks>
            public bool IsOrderUsed()
            {
                return (SkipTriggers[0].Condition == 10 && SkipTriggers[1].Condition == 10);
            }
        }
	}
}
