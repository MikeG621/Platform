/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7+
 */

/* CHANGELOG
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
		/// <summary>Object for a single Trigger</summary>
		[Serializable] public class Trigger	: BaseTrigger
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
				NotGlobalUnit
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
				EachSpecialCraft
			}
			/// <summary>Available <see cref="BaseTrigger.Condition"/> values</summary>
			public enum ConditionList : byte
			{
				True,
				Arrived,
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
				CannonsDisabled,
				BeDroppedOff,
				Broken26,
				NotDisabled,
				NotCaptured,
				NotInspected,
				BeginBoarding,
				NotBeingBoarded,
				BeginDocking,
				NotBeginDocking,
				Shields50Percent,
				Shields25Percent,
				Hull75Percent,
				Hull25Percent,
				Failed,
				TeamModifier,
				Unused40,
				BeAllPlayer,
				BeAllAI,
				ComeAndGo,
				BeBagged,
				Withdraw,
				BeCarried
			}

			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() : base(new byte[4]) { }

			/// <summary>Initializes a new Trigger from an existing Trigger. If null, a blank Trigger is created.</summary>
			/// <param name="other">Existing Trigger to clone. If <b>null</b>, Trigger will be blank.</param>
			public Trigger(Trigger other) : this()
			{
				if (other != null)
					Array.Copy(other._items, _items, _items.Length);
			}

			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Trigger(byte[] raw)
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				_items = new byte[4];
				ArrayFunctions.TrimArray(raw, 0, _items);
				checkValues(this);
			}

			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/></exception>
			public Trigger(byte[] raw, int startIndex)
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				if (raw.Length - startIndex < 4 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 4));
				_items = new byte[4];
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

			/// <summary>Returns a representative string of the Trigger</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, IFFs are <b>"IFF:#" and </b> Teams are <b>"TM:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the trigger and targets if applicable</returns>
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
			
			/// <summary>Converts a Trigger to a byte array</summary>
			/// <param name="trig">The Trigger to convert</param>
			/// <returns>A byte array of Length 4 containing the Trigger data</returns>
			public static explicit operator byte[](Trigger trig)
			{
				byte[] b = new byte[4];
				for (int i = 0; i < 4; i++) b[i] = trig[i];
				return b;
			}
			/// <summary>Converts a Trigger for use in TIE</summary>
			/// <param name="trig">The Trigger to convert</param>
			/// <exception cref="ArgumentException">Invalid values detected</exception>
			/// <returns>A copy of <paramref name="trig"/> for use in TIE95</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) { return new Tie.Mission.Trigger((byte[])trig); }
			/// <summary>Converts a Trigger for use in XWA</summary>
			/// <param name="trig">The Trigger to convert</param>
			/// <returns>A copy of <paramref name="trig"/> for use in XWA</returns>
			public static implicit operator Xwa.Mission.Trigger(Trigger trig)
			{
				Xwa.Mission.Trigger t = new Xwa.Mission.Trigger((byte[])trig);
				if (t.VariableType == (byte)TypeList.ShipType && t.Variable != 255) t.Variable++;
				return t;
			}
		}
	}
}
