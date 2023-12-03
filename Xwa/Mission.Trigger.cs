/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.8+
 */

/* CHANGELOG
 * [NEW] TypeList, AmountList, ConditionList enums
 * v5.8, 230804
 * [NEW] Region references prepped for string replacement
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v5.5, 2108001
 * [FIX] CraftType errors in strings [JB]
 * v5.0, 201004
 * [UPD] GG references prepped for string replacement [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * [FIX] ToString now prevents "of of" [JB]
 * [UPD] IFF substitution setup
 * v3.0, 180903
 * [UPD] added SafeString calls [JB]
 * [FIX] delay calculation [JB]
 * [NEW] helper functions for delete/swap FG/Mess [JB]
 * v2.7, 180509
 * [ADD] Proximity in ToString
 * [ADD #1] TriggerType unknowns (via JeremyAnsel)
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] conversions, _craftDowngrade(), exceptions, ToString() override
 * [UPD] ctor(byte[]) accepts Length 4
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class Mission : MissionFile
	{
		/// <summary>Object for a single Trigger</summary>
		[Serializable] public class Trigger : BaseTrigger
		{
			/// <summary>Available <see cref="BaseTrigger.VariableType"/> values</summary>
			public enum TypeList : byte
			{
				None,
				FlightGroup,
				ShipType,
				ShipClass,
				ObjectType,
				IFF,
				ShipOrders,
				CraftWhen,
				GlobalGroup,
				AILevel,
				Status,
				AllCraft,
				Team,
				PlayerNum,
				BeforeTime,
				NotFG,
				NotShipType,
				NotShipClass,
				NotObjectType,
				NotIFF,
				NotGlobalGroup,
				NotTeam,
				NotPlayerNum,
				GlobalUnit,
				NotGlobalUnit,
				GlobalCargo,
				NotGlobalCargo,
				MessageNum
			}
			/// <summary>Available <see cref="BaseTrigger.Amount"/> values</summary>
			public enum AmountList : byte
			{
				Percent100,
				Percent75,
				Percent50,
				Percent25,
				AtLeast1,
				AllBut1,
				AllSpecial,
				AllNonSpecial,
				AllNonPlayers,
				PlayersCraft,
				Percent100FirstWave,
				Percent75FirstWave,
				Percent50FirstWave,
				Percent25FirstWave,
				AnyFirstWave,
				AllBut1FirstWave,
				Percent66,
				Percent33,
				EachCraft,
				EachSpecialCraft,
				Unknown1,
				Unknown2,
				Unknown3
			}
			/// <summary>Available <see cref="BaseTrigger.Condition"/> values</summary>
			public enum ConditionList : byte
			{
				True,
				Created,
				Destroyed,
				Attacked,
				Captured,
				Inspected,
				Boarded,
				Docked,
				Disabled,
				Exist,
				False,
				Unused11,
				CompletedMission,
				CompletedPrimary,
				FailedPrimary,
				CompletedSecondary,
				FailedSecondary,
				CompletedBonus,
				FailedBonus,
				DroppedOff,
				Reinforced,
				Shields0Percent,
				Hull50Percent,
				NoWarheads,
				Unknown24,
				BeDroppedOff,
				Broken26,
				NotDisabled,
				NotCaptured,
				NotInspected,
				IsDocking,
				NotDocking,
				IsBoarding,
				NotBoarding,
				Shields50Percent,
				Shields25Percent,
				Hull75Percent,
				Hull25Percent,
				Failed,
				TeamModifier,
				Unknown40,
				BeAllPlayer,
				BeAllAI,
				ComeAndGo,
				BeBagged,
				Withdraw,
				BeCarried,
				ArrivedInRegion,
				DepartedRegion,
				InProximity,
				NotInProximity,
				AllCaptured,
				Defect,
				InConvoy,
				Delivered,
				AllDisabled,
				MessageShown,
				Identified,
				NotIdentified,
				Exist59
			}

			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() : base(new byte[6]) { }

			/// <summary>Initializes a new Trigger from an existing Trigger. If null, a blank Trigger is created.</summary>
			/// <param name="other">Existing Trigger to clone. If <b>null</b>, Trigger will be blank.</param>
			public Trigger(Trigger other) : this()
			{
				if (other != null)
					Array.Copy(other._items, _items, _items.Length);
			}

			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length</exception>
			public Trigger(byte[] raw)
			{
				_items = new byte[6];
				if (raw.Length >= 6) ArrayFunctions.TrimArray(raw, 0, _items);
				else if (raw.Length >= 4) for (int i = 0; i < 4; i++) _items[i] = raw[i];
				else throw new ArgumentException("Minimum length of raw is 4", "raw");
			}

			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <remarks>If <paramref name="raw"/>.Length is 6 or greater, reads six bytes. If the length is 4 or 5, reads only four bytes</remarks>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/></exception>
			public Trigger(byte[] raw, int startIndex)
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				_items = new byte[6];
				if (raw.Length >= 6)
				{
					if (raw.Length - startIndex < 6 || startIndex < 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 6));
					ArrayFunctions.TrimArray(raw, startIndex, _items);
				}
				else
				{
					if (raw.Length - startIndex < 4 || startIndex < 0)
						throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 4));
					for (int i = 0; i < 4; i++) _items[i] = raw[startIndex + i];
				}
			}
			
			static byte[] craftDowngrade(Trigger t)
			{
				byte[] b = new byte[4];
				ArrayFunctions.TrimArray((byte[])t, 0, b);
				if (b[1] == 2)
				{
					if (b[2] == 227) b[2] = 48;	// MC80 Liberty to MC80a
					else if (b[2] == 228) b[2] = 51;	// VSD II to VSD
					else if (b[2] == 229) b[2] = 52;	// ISD II to ISD
					else if (b[2] == 10 || b[2] == 11 || b[2] == 31 || b[2] > 91)
						throw new ArgumentException("Invalid CraftType Variable detected (" + b[2] + ")");
					else if (b[2] == 39) b[2] = 38;	// Falcon to CORT
					else if (b[2] == 71) b[2] = 69;	// SAT3 to SAT1
					else if (b[2] == 84 || b[2] == 87) b[2] = 82;	// HypBuoy/RDVBuoy to NavBuoy1
					else if (b[2] == 88) b[2] = 59;	// Cargo Canister to CN/A
				}
				return b;
			}
			
			/// <summary>Returns a representative string of the Trigger</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, IFFs are <b>"IFF:#" and </b> Teams are <b>"TM:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the trigger and targets if applicable</returns>
			public override string ToString()
			{
				string trig = "";
				if (Condition != (byte)ConditionList.True && Condition != (byte)ConditionList.False && VariableType != (byte)TypeList.BeforeTime)
				{
					if (Condition == (byte)ConditionList.InProximity || Condition == (byte)ConditionList.NotInProximity) trig = "Any of ";
					else
					{
						if (Amount > Strings.Amount.Length) Amount = 0; //can occur switching away from high-distance prox triggers
						trig = BaseStrings.SafeString(Strings.Amount, Amount);
						trig += (trig.IndexOf(" of") >= 0 || trig.IndexOf(" in") >= 0) ? " " : " of ";
					}
					switch ((TypeList)VariableType)
					{
						case TypeList.FlightGroup:
							trig += "FG:" + Variable;
							break;
						case TypeList.ShipType:
							trig += "Ship type " + BaseStrings.SafeString(Strings.CraftType, Variable);
							break;
						case TypeList.ShipClass:
							trig += "Ship class " + BaseStrings.SafeString(Strings.ShipClass, Variable);
							break;
						case TypeList.ObjectType:
							trig += "Object type " + BaseStrings.SafeString(Strings.ObjectType, Variable);
							break;
						case TypeList.IFF:
							trig += "IFF:" + Variable;
							break;
						case TypeList.ShipOrders:
							trig += "Ship orders " + BaseStrings.SafeString(Strings.Orders, Variable);
							break;
						case TypeList.CraftWhen:
							trig += "Craft When " + BaseStrings.SafeString(Strings.CraftWhen, Variable);
							break;
						case TypeList.GlobalGroup:
							trig += "GG:" + Variable;
							break;
                        case TypeList.AILevel:
                            trig += "Rating " + BaseStrings.SafeString(Strings.Rating, Variable);
                            break;
                        case TypeList.Status:
                            trig += "Craft with status: " + BaseStrings.SafeString(Strings.Status, Variable);
                            break;
                        case TypeList.AllCraft:
                            trig += "All craft";
                            break;
                        case TypeList.Team:
                            trig += "TM:" + Variable;
                            break;
                        case TypeList.PlayerNum:
                            trig += "Player #" + Variable;
                            break;
                        //case 14: omitted, special case after the switch
                        case TypeList.NotFG:
                            trig += "all except FG:" + Variable;
                            break;
                        case TypeList.NotShipType:
                            trig += "all except " + BaseStrings.SafeString(Strings.CraftType, Variable) + "s";
                            break;
                        case TypeList.NotShipClass:
                            trig += "all except " + BaseStrings.SafeString(Strings.ShipClass, Variable);
                            break;
                        case TypeList.NotObjectType:
                            trig += "all except " + BaseStrings.SafeString(Strings.ObjectType, Variable);
                            break;
                        case TypeList.NotIFF:
                            trig += "all except IFF:" + Variable;
							break;
                        case TypeList.NotGlobalGroup:
                            trig += "all except GG:" + Variable;
                            break;
                        case TypeList.NotTeam:
                            trig += "all except TM:" + Variable;
                            break;
                        case TypeList.NotPlayerNum:
                            trig += "all except Player #" + Variable;
                            break;
                        case TypeList.GlobalUnit:
                            trig += "Global Unit " + Variable;
                            break;
                        case TypeList.NotGlobalUnit:
                            trig += "all except Global Unit " + Variable;
                            break;
                        case TypeList.GlobalCargo:
                            trig += "Global Cargo " + Variable;
                            break;
                        case TypeList.NotGlobalCargo:
                            trig += "all except Global Cargo " + Variable;
                            break;
                        case TypeList.MessageNum:
							trig += "Message #" + (Variable + 1);  //[JB] One-based for display purposes.
							break;
						default:
							trig += VariableType + " " + Variable;
							break;
					}
					trig += " must ";
				}
				if (VariableType == (byte)TypeList.BeforeTime) trig = "After " + string.Format("{0}:{1:00}", GetDelaySeconds(Variable) / 60, GetDelaySeconds(Variable) % 60) + " delay";
				else if (Condition == (byte)ConditionList.InProximity || Condition == (byte)ConditionList.NotInProximity)
				{
					trig += (Condition == (byte)ConditionList.NotInProximity ? "NOT " : "") + "be within ";
					double dist;
					if (Amount == 0) dist = 0.05;
					else if (Amount <= 10) dist = 0.1 * Amount;
					else dist = Amount * 0.5 - 4;
					trig += dist + " km of FG2:" + Parameter1;
				}
				else trig += BaseStrings.SafeString(Strings.Trigger, Condition);
				if (trig.Contains("Region")) trig += " REG:" + Parameter1;
				return trig;
			}
			
			/// <summary>Converts a Trigger to a byte array</summary>
			/// <param name="trigger">The Trigger to convert</param>
			/// <returns>A byte array with Length 6 contianing the Trigger data</returns>
			public static explicit operator byte[](Trigger trigger)
			{
				byte[] b = new byte[6];
				for (int i = 0; i < 6; i++) b[i] = trigger[i];
				return b;
			}
			/// <summary>Converts a Trigger for use in TIE</summary>
			/// <remarks>Parameters are lost in the conversion</remarks>
			/// <param name="trig">The Trigger to convert</param>
			/// <exception cref="ArgumentException">Invalid values detected</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in TIE95</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) => new Tie.Mission.Trigger(craftDowngrade(trig));     // Parameters lost
			/// <summary>Converts a Trigger for use in XvT</summary>
			/// <remarks>Parameters are lost in the conversion</remarks>
			/// <param name="trig">The Trigger to convert</param>
			/// <exception cref="ArgumentException">Invalid values detected</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in XvT</returns>
			public static explicit operator Xvt.Mission.Trigger(Trigger trig) => new Xvt.Mission.Trigger(craftDowngrade(trig));     // Parameters lost

			/// <summary>Gets or sets the first additional setting</summary>
			public byte Parameter1
			{
				get => _items[4];
				set => _items[4] = value;
			}
			/// <summary>Gets or sets the second additional setting</summary>
			public byte Parameter2
			{
				get => _items[5];
				set => _items[5] = value;
			}

			/// <summary>This allows checking XWA's new properties.</summary>
			/// <param name="srcIndex">The original FG index location</param>
			/// <param name="dstIndex">The new FG index location</param>
			/// <param name="delete">Whether or not to delete the FG</param>
			/// <param name="delCond">The condition to reset references to after a delete operation</param>
			/// <returns><b>true</b> on a successful change</returns>
			/// <remarks>Extension for BaseTrigger to process additional properties found only in XWA</remarks>
			protected override bool TransformFGReferencesExtended(int srcIndex, int dstIndex, bool delete, bool delCond)
            {
                bool change = false;
                if ((Parameter1 == 5 + srcIndex) || (Parameter1 == srcIndex && Parameter1 == 255)) //255 is used as temp while swapping.  Ensure if we're swapping temp back to normal.
                {
                    change = true;
                    Parameter1 = (byte)dstIndex;
                    if (Parameter1 != 255)  //Don't modify if temp
                        Parameter1 += 5;    //Add the offset back in.
                    if (delete) Parameter1 = 0;
                }
                else if (Parameter1 > 5 + srcIndex && delete == true)
                {
                    change = true;
                    Parameter1--;
                }

                if (VariableType == (byte)TypeList.NotFG && Variable == srcIndex)
                {
                    change = true;
                    Variable = (byte)dstIndex;
                    if (delete)
                    {
                        Amount = 0;
                        VariableType = 0;
                        Variable = 0;
                        Condition = (byte)(delCond ? ConditionList.True : ConditionList.False);
                    }
                }
                else if (VariableType == (byte)TypeList.NotFG && Variable > srcIndex && delete == true)
                {
                    change = true;
                    Variable--;  //If deleting, decrement FG index to maintain
                }
                
                return change;
            }

            /// <summary>Helper function that changes Message indexes during a Move (index swap) operation.</summary>
			/// <param name="srcIndex">The original Message index location</param>
			/// <param name="dstIndex">The new Message index location</param>
			/// <returns><b>true</b> on successful change</returns>
            /// <remarks>Should not be called directly unless part of a larger Message Move operation.  Message references may exist in other mission properties, ensure those properties are adjusted when applicable.</remarks>
            public bool SwapMessageReferences(int srcIndex, int dstIndex)
            {
                bool change = false;
                change |= TransformMessageRef(dstIndex, 255);
                change |= TransformMessageRef(srcIndex, dstIndex);
                change |= TransformMessageRef(255, srcIndex);
                return change;
            }

			/// <summary>Helper function that changes Message indexes during a Message Move or Delete operation.</summary>
			/// <remarks>Should not be called directly unless part of a larger Message Move or Delete operation.  FG references may exist in other mission properties, ensure those properties are adjusted when applicable.</remarks>
			/// <param name="srcIndex">The Message index to match and replace (Move), or match and Delete.</param>
			/// <param name="dstIndex">The Message index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="dstIndex"/> is <b>256</b> or more.</exception>
			/// <returns><b>true</b> on a successful change</returns>
			public bool TransformMessageRef(int srcIndex, int dstIndex)
            {
                if (VariableType != (byte)TypeList.MessageNum) return false;

                bool change = false;
                bool delete = false;
                if (dstIndex < 0)
                {
                    dstIndex = 0;
                    delete = true;
                }
                else if (dstIndex > 255) throw new ArgumentOutOfRangeException("TransformMessageRef: dstIndex out of range.");

                if (delete)
                {
                    if (Variable == srcIndex)
                    {
                        change = true; Amount = 0; Condition = 0; VariableType = 0; Variable = 0;
                    }
                    else if (Variable > srcIndex) { change = true; Variable--; }
                }
                else
                {
                    if (Variable == (byte)srcIndex)
                    {
                        change = true;
                        Variable = (byte)dstIndex;
                    }
                }
                return change;
            }
        }
	}
}
