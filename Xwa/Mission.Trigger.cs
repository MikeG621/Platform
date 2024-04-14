/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.1+
 */

/* CHANGELOG
 * [UPD] Format spec implemented
 * v6.1, 231208
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
		/// <summary>Object for a single Trigger.</summary>
		[Serializable] public class Trigger : BaseTrigger
		{
			/// <summary>Available <see cref="BaseTrigger.VariableType"/> values.</summary>
			public enum TypeList : byte
			{
				/// <summary>No Target.</summary>
				None,
				/// <summary>Target is FlightGroup.</summary>
				FlightGroup,
				/// <summary>Target is a specific craft type.</summary>
				ShipType,
				/// <summary>Target is a specific class (starfighter, transport, etc).</summary>
				ShipClass,
				/// <summary>Target is a specific type of object (craft, weapon, object).</summary>
				ObjectType,
				/// <summary>Target is a specific IFF.</summary>
				IFF,
				/// <summary>Target has a specific order.</summary>
				/// <remarks>Spec has this mapped to None.</remarks>
				ShipOrders,
				/// <summary>Target has a special condition or state.</summary>
				CraftWhen,
				/// <summary>Target is a member of a GG.</summary>
				GlobalGroup,
				/// <summary>Target has a specific AI rating.</summary>
				/// <remarks>Spec has this mapped to None.</remarks>
				AILevel,
				/// <summary>Target has a specific Status.</summary>
				/// <remarks>Spec has this mapped to None.</remarks>
				Status,
				/// <summary>All craft.</summary>
				/// <remarks>Spec has this mapped to None.</remarks>
				AllCraft,
				/// <summary>Target is Team.</summary>
				Team,
				/// <summary>Target is specific player.</summary>
				PlayerNum,
				/// <summary>Trigger completed before mission time.</summary>
				BeforeTime,
				/// <summary>All FGs except target.</summary>
				NotFG,
				/// <summary>All craft types except target.</summary>
				NotShipType,
				/// <summary>All ship classes except target.</summary>
				NotShipClass,
				/// <summary>All object tpyes except target.</summary>
				NotObjectType,
				/// <summary>All IFFs except target.</summary>
				NotIFF,
				/// <summary>All GGs except target.</summary>
				NotGlobalGroup,
				/// <summary>All Teams except target.</summary>
				NotTeam,
				/// <summary>All players except target.</summary>
				NotPlayerNum,
				/// <summary>Target is a GU.</summary>
				GlobalUnit,
				/// <summary>All GUs except target.</summary>
				NotGlobalUnit,
				/// <summary>Target has a specific GC.</summary>
				GlobalCargo,
				/// <summary>Target does not have the GC.</summary>
				NotGlobalCargo,
				/// <summary>Message index.</summary>
				MessageNum
			}
			/// <summary>Available <see cref="BaseTrigger.Amount"/> values.</summary>
			public enum AmountList : byte
			{
				/// <summary>100% of target.</summary>
				Percent100,
				/// <summary>75% of target.</summary>
				Percent75,
				/// <summary>50% of target.</summary>
				Percent50,
				/// <summary>25% of target.</summary>
				Percent25,
				/// <summary>At least 1 of target.</summary>
				AtLeast1,
				/// <summary>All but 1 of target.</summary>
				AllBut1,
				/// <summary>All Special Craft of the target.</summary>
				AllSpecial,
				/// <summary>All non-Special Craft of the target.</summary>
				AllNonSpecial,
				/// <summary>All AI craft of the target.</summary>
				AllNonPlayers,
				/// <summary>The Player's craft of the target.</summary>
				PlayersCraft,
				/// <summary>100% of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to 100%.</remarks>
				Percent100FirstWave,
				/// <summary>75% of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to 75%<./remarks>
				Percent75FirstWave,
				/// <summary>50% of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to 50%.</remarks>
				Percent50FirstWave,
				/// <summary>25% of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to 25%.</remarks>
				Percent25FirstWave,
				/// <summary>At least 1 of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to AtLeast1.</remarks>
				AnyFirstWave,
				/// <summary>All but 1 of the first wave of target.</summary>
				/// <remarks>Spec has this mapped to AllBut1.</remarks>
				AllBut1FirstWave,
				/// <summary>66% of target.</summary>
				Percent66,
				/// <summary>33% of target.</summary>
				Percent33,
				/// <summary>Each craft of target, individually.</summary>
				EachCraft,
				/// <summary>Each Special Craft of target, individually.</summary>
				EachSpecialCraft,
				/// <summary>Unknown.</summary>
				Unknown1,
				/// <summary>Unknown.</summary>
				Unknown2,
				/// <summary>Unknown.</summary>
				Unknown3
			}
			/// <summary>Available <see cref="BaseTrigger.Condition"/> values.</summary>
			public enum ConditionList : byte
			{
				/// <summary>Always true.</summary>
				True,
				/// <summary>Target spawned.</summary>
				Created,
				/// <summary>Target destroyed.</summary>
				Destroyed,
				/// <summary>Target damaged.</summary>
				Attacked,
				/// <summary>Target captured.</summary>
				Captured,
				/// <summary>Target inspected.</summary>
				Inspected,
				/// <summary>Target has been boarded.</summary>
				WasBoarded,
				/// <summary>Target finished docking.</summary>
				WasDocked,
				/// <summary>Target disabled.</summary>
				Disabled,
				/// <summary>Target is present, not destroyed.</summary>
				Exist,
				/// <summary>Never true.</summary>
				False,
				/// <summary>False.</summary>
				UnusedA,
				/// <summary>Mission complete.</summary>
				CompletedMission,
				/// <summary>Primary goals complete.</summary>
				CompletedPrimary,
				/// <summary>Primary goals failed.</summary>
				FailedPrimary,
				/// <summary>False.</summary>
				UnusedF,
				/// <summary>False.</summary>
				Unused10,
				/// <summary>Bonus goals complete.</summary>
				CompletedBonus,
				/// <summary>Bonus goals failed.</summary>
				FailedBonus,
				/// <summary>Dropped off from mothership.</summary>
				DroppedOff,
				/// <summary>Called for reinforcements.</summary>
				Reinforced,
				/// <summary>Shields down.</summary>
				Shields0Percent,
				/// <summary>Hull damaged to 50%.</summary>
				Hull50Percent,
				/// <summary>Out of warheads.</summary>
				NoWarheads,
				/// <summary>Cannon subsystem disabled or missing.</summary>
				CannonsDisabled,
				/// <summary>Not yet arrived.</summary>
				NotCreated,
				/// <summary>Come and Go without being attacked. For Triggers, player action is ignored.</summary>
				/// <remarks>If the player manages to destroy the target in a single hit, the engine incorrectly registers "Come and Go" instead of "Attacked".</remarks>
				NotAttacked,
				/// <summary>For Triggers, simple Come and Go. For Goals, Come and Go without being disabled.</summary>
				NotDisabled,
				/// <summary>Come and Go without being captured.</summary>
				NotCaptured,
				/// <summary>Come and Go without inspection.</summary>
				/// <remarks>If used as FG Goal, or paired with <see cref="ByTeam"/>, then craft must arrive, then leave or be destroyed.<br/>
				/// Otherwise, "Come and Go" is determined by proxy such as mothership destruction.</remarks>
				NotInspected,
				/// <summary>Currently being boarded.</summary>
				IsBeingBoarded,
				/// <summary>Come and go without completely being boarded.</summary>
				NotBoarded,
				/// <summary>Started the docking process.</summary>
				BegunDocking,
				/// <summary>Come and Go without completing a docking operation.</summary>
				NotDocked,
				/// <summary>Shields down to 50%.</summary>
				Shields50Percent,
				/// <summary>Shields down to 25%.</summary>
				Shields25Percent,
				/// <summary>Hull down to 75%.</summary>
				Hull75Percent,
				/// <summary>Hull down to 25%.</summary>
				Hull25Percent,
				/// <summary>Time is before the value.</summary>
				BeforeTime,
				/// <summary>Modifies specific triggers to use Team statistics instead of Global.</summary>
				/// <remarks>To be used as Trigger2 or 4. <see cref="BaseTrigger.VariableType"/> must be <b>Team</b>, <see cref="BaseTrigger.Amount"/> ignored.</remarks>
				ByTeam,
				/// <summary>Inverse of <see cref="ByTeam"/>.</summary>
				NotByTeam,
				/// <summary>Target has player craft.</summary>
				HavePlayer,
				/// <summary>Target is all non-players.</summary>
				BeAllAI,
				/// <summary>Arrive and then depart.</summary>
				ComeAndGo,
				/// <summary>Captured and successfully depart.</summary>
				BeBagged,
				/// <summary>Target aborts mission.</summary>
				Withdraw,
				/// <summary>Captured and gone home.</summary>
				CapturedAndGone,
				/// <summary>Arrived in specific region.</summary>
				ArrivedInRegion,
				/// <summary>Departed the region.</summary>
				DepartedRegion,
				/// <summary>Within certain range of target.</summary>
				InProximity,
				/// <summary>Not within certain range of target.</summary>
				NotInProximity,
				/// <summary>Carried by firendly craft.</summary>
				BeCarried,
				/// <summary>Switch IFF.</summary>
				Defect,
				/// <summary>Part of a convy train.</summary>
				InConvoy,
				/// <summary>Carried item released.</summary>
				Delivered,
				/// <summary>Stationary at docking/landing port.</summary>
				Parked,
				/// <summary>Message fired and displayed.</summary>
				MessageShown,
				/// <summary>Target ID'ed.</summary>
				Identified,
				/// <summary>Come and GO without being ID'ed.</summary>
				NotIdentified,
				/// <summary>Unknown.</summary>
				Support
			}

			/// <summary>Initializes a blank Trigger.</summary>
			public Trigger() : base(new byte[6]) { }

			/// <summary>Initializes a new Trigger from an existing Trigger. If null, a blank Trigger is created.</summary>
			/// <param name="other">Existing Trigger to clone. If <b>null</b>, Trigger will be blank.</param>
			public Trigger(Trigger other) : this()
			{
				if (other != null)
					Array.Copy(other._items, _items, _items.Length);
			}

			/// <summary>Initializes a new Trigger from raw data.</summary>
			/// <param name="raw">Raw data, minimum Length of 4.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length.</exception>
			public Trigger(byte[] raw) : this()
			{
				if (raw.Length >= 6) Array.Copy(raw, _items, _items.Length);
				else if (raw.Length >= 4) Array.Copy(raw, _items, 4);
				else throw new ArgumentException("Minimum length of raw is 4", "raw");
			}

			/// <summary>Initializes a new Trigger from raw data.</summary>
			/// <remarks>If <paramref name="raw"/>.Length is 6 or greater, reads six bytes. If the length is 4 or 5, reads only four bytes.</remarks>
			/// <param name="raw">Raw data, minimum Length of 4.</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/> Length.</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/>.</exception>
			public Trigger(byte[] raw, int startIndex) : this()
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
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
				ArrayFunctions.TrimArray(t.GetBytes(), 0, b);
				if (t.VariableType == (byte)TypeList.ShipType)
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
			
			/// <summary>Returns a representative string of the Trigger.</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, IFFs are <b>"IFF:#" and </b> Teams are <b>"TM:#"</b> for later substitution if required.</remarks>
			/// <returns>Description of the trigger and targets if applicable.</returns>
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
					trig += dist + " km of FG2:" + Parameter;
				}
				else trig += BaseStrings.SafeString(Strings.Trigger, Condition);
				if (trig.Contains("Region")) trig += " REG:" + Parameter;
				return trig;
			}

			/// <summary>Converts a Trigger to a byte array.</summary>
			/// <param name="trigger">The Trigger to convert.</param>
			/// <returns>A byte array with Length 6 containing the Trigger data.</returns>
			public static explicit operator byte[](Trigger trigger) => trigger.GetBytes();
			/// <summary>Converts a Trigger for use in TIE.</summary>
			/// <remarks>Parameter is lost in the conversion.</remarks>
			/// <param name="trig">The Trigger to convert.</param>
			/// <exception cref="ArgumentException">Invalid values detected.</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in TIE95.</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) => new Tie.Mission.Trigger(craftDowngrade(trig));
			/// <summary>Converts a Trigger for use in XvT.</summary>
			/// <remarks>Parameter is lost in the conversion.</remarks>
			/// <param name="trig">The Trigger to convert.</param>
			/// <exception cref="ArgumentException">Invalid values detected.</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in XvT.</returns>
			public static explicit operator Xvt.Mission.Trigger(Trigger trig) => new Xvt.Mission.Trigger(craftDowngrade(trig));

			/// <summary>Gets or sets the additional trigger parameter.</summary>
			/// <remarks>This is typically a Region or FG Index.</remarks>
			public short Parameter
			{
				get => BitConverter.ToInt16(_items, 4);
				set => ArrayFunctions.WriteToArray(value, _items, 4);
			}

			/// <summary>This allows checking XWA's new properties.</summary>
			/// <param name="srcIndex">The original FG index location.</param>
			/// <param name="dstIndex">The new FG index location.</param>
			/// <param name="delete">Whether or not to delete the FG.</param>
			/// <param name="delCond">The condition to reset references to after a delete operation.</param>
			/// <returns><b>true</b> on a successful change.</returns>
			/// <remarks>Extension for BaseTrigger to process additional properties found only in XWA.</remarks>
			protected override bool TransformFGReferencesExtended(int srcIndex, int dstIndex, bool delete, bool delCond)
            {
                bool change = false;
                if ((Parameter == 5 + srcIndex) || (Parameter == srcIndex && Parameter == 255)) //255 is used as temp while swapping.  Ensure if we're swapping temp back to normal.
                {
                    change = true;
                    Parameter = (byte)dstIndex;
                    if (Parameter != 255)  //Don't modify if temp
                        Parameter += 5;    //Add the offset back in.
                    if (delete) Parameter = 0;
                }
                else if (Parameter > 5 + srcIndex && delete == true)
                {
                    change = true;
                    Parameter--;
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
			/// <param name="srcIndex">The original Message index location.</param>
			/// <param name="dstIndex">The new Message index location.</param>
			/// <returns><b>true</b> on successful change.</returns>
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
