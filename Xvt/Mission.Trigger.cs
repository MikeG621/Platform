/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.1+
 */

/* CHANGELOG
 * v6.1, 231208
 * [FIX] byte[] ctor now trims properly
 * [NEW] TypeList, AmountList, ConditionList enums
 * [FIX] Converting to XWA adjusts craft type properly
 * v5.7, 220127
 * [UPD] added ctor now calls base [JB]
 * v5.6, 220103
 * [NEW] cloning ctor [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * [FIX] ToString now prevents "of of" [JB]
 * [UPD] TriggerType expanded [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] conversions, _checkValues(), exceptions
 * [NEW] ToString() override
 */

using Idmr.Common;
using System;

namespace Idmr.Platform.Xvt
{
	public partial class Mission : MissionFile
	{
		/// <summary>Object for a single Trigger.</summary>
		[Serializable] public class Trigger	: BaseTrigger
		{
			/// <summary>Available <see cref="BaseTrigger.VariableType"/> values.</summary>
			public enum TypeList : byte
			{
				/// <summary>No Target</summary>
				None,
				/// <summary>Target is FlightGroup</summary>
				FlightGroup,
				/// <summary>Target is a specific craft type</summary>
				ShipType,
				/// <summary>Target is a specific class (starfighter, transport, etc)</summary>
				ShipClass,
				/// <summary>Target is a specific type of object (craft, weapon, object)</summary>
				ObjectType,
				/// <summary>Target is a specific IFF</summary>
				IFF,
				/// <summary>Target has a specific order</summary>
				ShipOrders,
				/// <summary>Target has a special condition or state</summary>
				CraftWhen,
				/// <summary>Target is a member of a GG</summary>
				GlobalGroup,
				/// <summary>Target has a specific AI rating</summary>
				AILevel,
				/// <summary>Target has a specific Status</summary>
				Status,
				/// <summary>All craft</summary>
				AllCraft,
				/// <summary>Target is Team</summary>
				Team,
				/// <summary>Target is specific player</summary>
				PlayerNum,
				/// <summary>Trigger completed before mission time</summary>
				BeforeTime,
				/// <summary>All FGs except target</summary>
				NotFG,
				/// <summary>All craft types except target</summary>
				NotShipType,
				/// <summary>All ship classes except target</summary>
				NotShipClass,
				/// <summary>All object tpyes except target</summary>
				NotObjectType,
				/// <summary>All IFFs except target</summary>
				NotIFF,
				/// <summary>All GGs except target</summary>
				NotGlobalGroup,
				/// <summary>All Teams except target</summary>
				NotTeam,
				/// <summary>All players except target</summary>
				NotPlayerNum,
				/// <summary>Target is a GU</summary>
				GlobalUnit,
				/// <summary>All GUs except target</summary>
				NotGlobalUnit
			}
			/// <summary>Available <see cref="BaseTrigger.Amount"/> values.</summary>
			public enum AmountList : byte
			{
				/// <summary>100% of target</summary>
				Percent100,
				/// <summary>75% of target</summary>
				Percent75,
				/// <summary>50% of target</summary>
				Percent50,
				/// <summary>25% of target</summary>
				Percent25,
				/// <summary>At least 1 of target</summary>
				AtLeast1,
				/// <summary>All but 1 of target</summary>
				AllBut1,
				/// <summary>All Special Craft of the target</summary>
				AllSpecial,
				/// <summary>All non-Special Craft of the target</summary>
				AllNonSpecial,
				/// <summary>All AI craft of the target</summary>
				AllNonPlayers,
				/// <summary>The Player's craft of the target</summary>
				PlayersCraft,
				/// <summary>100% of the first wave of target</summary>
				Percent100FirstWave,
				/// <summary>75% of the first wave of target</summary>
				Percent75FirstWave,
				/// <summary>50% of the first wave of target</summary>
				Percent50FirstWave,
				/// <summary>25% of the first wave of target</summary>
				Percent25FirstWave,
				/// <summary>At least 1 of the first wave of target</summary>
				AnyFirstWave,
				/// <summary>All but 1 of the first wave of target</summary>
				AllBut1FirstWave,
				/// <summary>66% of target</summary>
				Percent66,
				/// <summary>33% of target</summary>
				Percent33,
				/// <summary>Each craft of target, individually</summary>
				EachCraft,
				/// <summary>Each Special Craft of target, individually</summary>
				EachSpecialCraft
			}
			/// <summary>Available <see cref="BaseTrigger.Condition"/> values.</summary>
			public enum ConditionList : byte
			{
				/// <summary>Always true</summary>
				True,
				/// <summary>Target arrived</summary>
				Arrived,
				/// <summary>Target destroyed</summary>
				Destroyed,
				/// <summary>Target damaged</summary>
				Attacked,
				/// <summary>Target captured</summary>
				Captured,
				/// <summary>Target inspected</summary>
				Inspected,
				/// <summary>Target is boarded</summary>
				Boarded,
				/// <summary>Target is docked</summary>
				Docked,
				/// <summary>Target disabled</summary>
				Disabled,
				/// <summary>Target is present, not destroyed</summary>
				Exist,
				/// <summary>Never true</summary>
				False,
				/// <summary>Unknown</summary>
				Unused11,
				/// <summary>Mission complete</summary>
				CompletedMission,
				/// <summary>Primary goals complete</summary>
				CompletedPrimary,
				/// <summary>Primary goals failed</summary>
				FailedPrimary,
				/// <summary>Secondary goals complete</summary>
				CompletedSecondary,
				/// <summary>Secondary goals failed</summary>
				FailedSecondary,
				/// <summary>Bonus goals complete</summary>
				CompletedBonus,
				/// <summary>Bonus goals failed</summary>
				FailedBonus,
				/// <summary>Dropped off from mothership</summary>
				DroppedOff,
				/// <summary>Called for reinforcements</summary>
				Reinforced,
				/// <summary>Shields down</summary>
				Shields0Percent,
				/// <summary>Hull damaged to 50%</summary>
				Hull50Percent,
				/// <summary>Out of warheads</summary>
				NoWarheads,
				/// <summary>Cannon subsystem disabled or missing</summary>
				CannonsDisabled,
				/// <summary>Broken</summary>
				BeDroppedOff,
				/// <summary>Come and Go without being attacked. For Triggers, player action is ignored.</summary>
				/// <remarks>If the player manages to destroy the target in a single hit, the engine incorrectly registers "Come and Go" instead of "Attacked"</remarks>
				NotAttacked,
				/// <summary>For Triggers, simple Come and Go. For Goals, Come and Go without being disabled.</summary>
				NotDisabled,
				/// <summary>Come and Go without being captured</summary>
				NotCaptured,
				/// <summary>Come and Go without inspection</summary>
				/// <remarks>If used as FG Goal, or paired with <see cref="TeamModifier"/>, then craft must arrive, then leave or be destroyed.<br/>
				/// Otherwise, "Come and Go" is determined by proxy such as mothership destruction</remarks>
				NotInspected,
				/// <summary>Start boarding process</summary>
				BeginBoarding,
				/// <summary>Come and Go without completely being boarded</summary>
				NotBeingBoarded,
				/// <summary>Start docking process</summary>
				BeginDocking,
				/// <summary>Come and go without completing the boarding of another craft</summary>
				NotBeginDocking,
				/// <summary>Shields down to 50%</summary>
				Shields50Percent,
				/// <summary>Shields down to 25%</summary>
				Shields25Percent,
				/// <summary>Hull down to 75%</summary>
				Hull75Percent,
				/// <summary>Hull down to 25%</summary>
				Hull25Percent,
				/// <summary>Explicit failure, not just "false"</summary>
				Failed,
				/// <summary>Modifies specific triggers to use Team statistics instead of Global</summary>
				/// <remarks>To be used as Trigger2 or 4. <see cref="BaseTrigger.VariableType"/> must be <b>Team</b>, <see cref="BaseTrigger.Amount"/> ignored.</remarks>
				TeamModifier,
				/// <summary>Unused</summary>
				Unused40,
				/// <summary>Target is all players</summary>
				BeAllPlayer,
				/// <summary>Target is all non-players</summary>
				BeAllAI,
				/// <summary>Arrive and then depart</summary>
				ComeAndGo,
				/// <summary>Captured and successfully depart</summary>
				BeBagged,
				/// <summary>Target aborts mission</summary>
				Withdraw,
				/// <summary>Picked up by friendly craft</summary>
				BeCarried
			}

			/// <summary>Initializes a blank Trigger.</summary>
			public Trigger() : base(new byte[4]) { }

			/// <summary>Initializes a new Trigger from an existing Trigger. If null, a blank Trigger is created.</summary>
			/// <param name="other">Existing Trigger to clone. If <b>null</b>, Trigger will be blank.</param>
			public Trigger(Trigger other) : this()
			{
				if (other != null)
					Array.Copy(other._items, _items, _items.Length);
			}

			/// <summary>Initializes a new Trigger from raw data.</summary>
			/// <param name="raw">Raw data, minimum Length of 4.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values.</exception>
			public Trigger(byte[] raw) : this()
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				Array.Copy(raw, _items, _items.Length);
				checkValues(this);
			}

			/// <summary>Initializes a new Trigger from raw data.</summary>
			/// <param name="raw">Raw data.</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values.</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/>.</exception>
			public Trigger(byte[] raw, int startIndex) : this()
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				if (raw.Length - startIndex < 4 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 4));
				ArrayFunctions.TrimArray(raw, startIndex, _items);
				checkValues(this);
			}
			
			static void checkValues(Trigger t)
			{
				string error = "";
				if (t.Condition > (byte)ConditionList.BeCarried) error = "Condition (" + t.Condition + ")";
				byte tempVar = t.Variable;
				CheckTarget(t.VariableType, ref tempVar, out string msg);
				t.Variable = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + msg;
				if (error != "") throw new ArgumentException("Invalid values detected: " + error +  ".");
				if (t.Amount == (byte)AmountList.EachSpecialCraft) t.Amount = (byte)AmountList.AllSpecial;
			}

			/// <summary>Returns a representative string of the Trigger.</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, IFFs are <b>"IFF:#" and </b> Teams are <b>"TM:#"</b> for later substitution if required.</remarks>
			/// <returns>Description of the trigger and targets if applicable.</returns>
			public override string ToString()
			{
				string trig = "";
				if (Condition != (byte)ConditionList.True && Condition != (byte)ConditionList.False)
				{
					trig = BaseStrings.SafeString(Strings.Amount, Amount);
					trig += (trig.IndexOf(" of") >= 0 || trig.IndexOf(" in") >= 0) ? " " : " of ";
					switch ((TypeList)VariableType)
					{
						case TypeList.FlightGroup:
							trig += "FG:" + Variable;
							break;
						case TypeList.ShipType:
							trig += "Ship type " + BaseStrings.SafeString(Strings.CraftType, Variable + 1);
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
							trig += "Craft with " + BaseStrings.SafeString(Strings.Orders, Variable) + " starting orders";
							break;
						case TypeList.CraftWhen:
							trig += "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, Variable);
							break;
						case TypeList.GlobalGroup:
							trig += "Global Group " + Variable;
							break;
						case TypeList.AILevel:
							trig += "Craft with " + BaseStrings.SafeString(Strings.Rating, Variable) + " adjusted skill";
							break;
						case TypeList.Status:
							trig += "Craft with primary status: " + BaseStrings.SafeString(Strings.Status, Variable);
							break;
						case TypeList.AllCraft:
							trig += "All craft";
							break;
						case TypeList.Team:
							trig += "TM:" + Variable;
							break;
						case TypeList.PlayerNum:
							trig += "(Player #" + Variable + ")";
							break;
						case TypeList.BeforeTime:
							trig += "(Before elapsed time " + string.Format("{0}:{1:00}", Variable * 5 / 60, Variable * 5 % 60) + ")";
							break;
						case TypeList.NotFG:
							trig += "All except FG:" + Variable;
							break;
						case TypeList.NotShipType:
							trig += "All except " + BaseStrings.SafeString(Strings.CraftType, Variable + 1) + "s";
							break;
						case TypeList.NotShipClass:
							trig += "All except " + BaseStrings.SafeString(Strings.ShipClass, Variable);
							break;
						case TypeList.NotObjectType:
							trig += "All except " + BaseStrings.SafeString(Strings.ObjectType, Variable);
							break;
						case TypeList.NotIFF:
							trig += "All except IFF:" + Variable;
							break;
						case TypeList.NotGlobalGroup:
							trig += "All except GG " + Variable;
							break;
						case TypeList.NotTeam:
							trig += "All except TM:" + Variable;
							break;
						case TypeList.NotPlayerNum:
							trig += "(All except Player #" + (Variable + 1) + ")";
							break;
						case TypeList.GlobalUnit:
							trig += "Global Unit " + Variable;
							break;
						case TypeList.NotGlobalUnit:
							trig += "All except Global Unit " + Variable;
							break;
						default:
							trig += VariableType + " " + Variable;
							break;
					}
					trig += " must ";
				}
				trig += BaseStrings.SafeString(Strings.Trigger, Condition);
				return trig;
			}

			/// <summary>Converts a Trigger to a byte array.</summary>
			/// <param name="trig">The Trigger to convert.</param>
			/// <returns>A byte array of Length 4 containing the Trigger data.</returns>
			public static explicit operator byte[](Trigger trig) => trig.GetBytes();
			/// <summary>Converts a Trigger for use in TIE.</summary>
			/// <param name="trig">The Trigger to convert.</param>
			/// <exception cref="ArgumentException">Invalid values detected.</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in TIE95.</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) => new Tie.Mission.Trigger(trig.GetBytes());
			/// <summary>Converts a Trigger for use in XWA.</summary>
			/// <param name="trig">The Trigger to convert.</param>
			/// <returns>A copy of <paramref name="trig"/> for use in XWA.</returns>
			public static implicit operator Xwa.Mission.Trigger(Trigger trig)
			{
				Xwa.Mission.Trigger t = new Xwa.Mission.Trigger(trig.GetBytes());
				if (t.VariableType == (byte)TypeList.ShipType && t.Variable != 255) t.Variable++;
				return t;
			}
		}
	}
}