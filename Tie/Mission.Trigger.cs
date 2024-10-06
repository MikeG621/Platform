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
 * [UPD] use of GetBytes
 * v6.1, 231208
 * [NEW] TypeList, AmountList, ConditionList enums
 * v5.7.5, 230116
 * [UPD #12] ToString update for AI Rating, Status and All Craft
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v4.0, 200809
 * [UPD] SafeString implemented [JB]
 * [FIX] ToString now prevents "of of" [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * [FIX] _checkValues reverts TargetType 0A to 00 (LA sloppiness, caused load fail on certain missions)
 * v2.0, 120525
 * [NEW] conversions, validation, exceptions, ToString() override
 */

using Idmr.Common;
using System;

namespace Idmr.Platform.Tie
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
				ShipOrders,
				/// <summary>Target has a special condition or state.</summary>
				CraftWhen,
				/// <summary>Target is a member of a GG.</summary>
				GlobalGroup,
				/// <summary>Target has a specific AI rating.</summary>
				AILevel,
				/// <summary>Target has a specific Status.</summary>
				Status,
				/// <summary>All craft.</summary>
				AllCraft
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
				Percent100FirstWave,
				/// <summary>75% of the first wave of target.</summary>
				Percent75FirstWave,
				/// <summary>50% of the first wave of target.</summary>
				Percent50FirstWave,
				/// <summary>25% of the first wave of target.</summary>
				Percent25FirstWave,
				/// <summary>At least 1 of the first wave of target.</summary>
				AnyFirstWave,
				/// <summary>All but 1 of the first wave of target.</summary>
				AllBut1FirstWave
			}

			/// <summary>Available <see cref="BaseTrigger.Condition"/> values.</summary>
			public enum ConditionList : byte
			{
				/// <summary>Always true.</summary>
				True,
				/// <summary>Target arrived.</summary>
				Created,
				/// <summary>Target destroyed.</summary>
				Destroyed,
				/// <summary>Target damaged.</summary>
				Attacked,
				/// <summary>Target captured.</summary>
				Captured,
				/// <summary>Target inspected.</summary>
				Inspected,
				/// <summary>Target is boarded.</summary>
				Boarded,
				/// <summary>Target is docked.</summary>
				Docked,
				/// <summary>Target disabled.</summary>
				Disabled,
				/// <summary>Target is present, not destroyed.</summary>
				Exist,
				/// <summary>Never true.</summary>
				False,
				/// <summary>Unknown.</summary>
				Unknown11,
				/// <summary>Mission complete.</summary>
				CompletedMission,
				/// <summary>Primary goals complete.</summary>
				CompletedPrimary,
				/// <summary>Primary goals failed.</summary>
				FailedPrimary,
				/// <summary>Secondary goals complete.</summary>
				CompletedSecondary,
				/// <summary>Secondary goals failed.</summary>
				FailedSecondary,
				/// <summary>Bonus goals complete.</summary>
				CompletedBonus,
				/// <summary>Bonus goals failed.</summary>
				FailedBonus,
				/// <summary>Dropped off from mothership.</summary>
				DroppedOff,
				/// <summary>Called for reinforcements.</summary>
				Reinforced,
				/// <summary>Shields down.</summary>
				NoShields,
				/// <summary>Hull damaged to 50%.</summary>
				HalfHull,
				/// <summary>Out of warheads.</summary>
				NoWarheads,
				/// <summary>Cannon subsystem disabled or missing.</summary>
				CannonsDisabled
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
				if (t.Condition > (byte)ConditionList.CannonsDisabled) error = "Condition (" + t.Condition + ")";
				byte tempVar = t.Variable;
				if (t.VariableType == (byte)TypeList.Status)
				{
					t.VariableType = 0;
					t.Variable = 0;
				}
				CheckTarget(t.VariableType, ref tempVar, out string msg);
				t.Variable = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + msg;
				if (error != "") throw new ArgumentException("Invalid values detected: " + error + ".");
				t.Amount = (t.Amount == (byte)Xvt.Mission.Trigger.AmountList.Percent66 ? (byte)AmountList.Percent75 :
					(t.Amount == (byte)Xvt.Mission.Trigger.AmountList.Percent33 ? (byte)AmountList.Percent50 :
					(t.Amount == (byte)Xvt.Mission.Trigger.AmountList.EachCraft ? (byte)AmountList.Percent100 :
					(t.Amount == (byte)Xvt.Mission.Trigger.AmountList.EachSpecialCraft ? (byte)AmountList.AllSpecial :
					t.Amount))));
			}
			
			static byte[] craftUpgrade(Trigger t)
			{
				byte[] b = t.GetBytes();
				if (t.VariableType == (byte)TypeList.ShipType)
				{
					if (t.Variable == 10) b[(byte)TriggerIndex.Variable] = 89;	// SHPYD
					else if (t.Variable == 11) b[(byte)TriggerIndex.Variable] = 90;	// REPYD
					else if (t.Variable == 31) b[(byte)TriggerIndex.Variable] = 77;	// G/PLT
					else if (t.Variable == 39) b[(byte)TriggerIndex.Variable] = 91;	// M/SC
				}
				return b;
			}
			
			/// <summary>Returns a representative string of the Trigger.</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> for later substitution if required.</remarks>
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
							trig += "Ship orders " + BaseStrings.SafeString(Strings.Orders, Variable);
							break;
						case TypeList.CraftWhen:
							trig += "Craft When " + BaseStrings.SafeString(Strings.CraftWhen, Variable);
							break;
						case TypeList.GlobalGroup:
							trig += "Global Group " + Variable;
							break;
						case TypeList.AILevel:
							trig += "AI Rating " + BaseStrings.SafeString(Strings.Rating, Variable);
							break;
                        case TypeList.Status:
                            trig += "Craft with status: " + BaseStrings.SafeString(Strings.Status, Variable);
                            break;
                        case TypeList.AllCraft:
                            trig += "All craft";
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
			/// <returns>A byte array with a length of 4 containing the trigger data.</returns>
			public static explicit operator byte[](Trigger trig) => trig.GetBytes();
			/// <summary>Converts a Trigger for use in XvT.</summary>
			/// <remarks>CraftType indexes for SHPYD, REPYD, G/PLT and M/SC will be updated.</remarks>
			/// <param name="trig">The Trigger to convert.</param>
			/// <returns>A copy of <paramref name="trig"/> for XvT.</returns>
			public static implicit operator Xvt.Mission.Trigger(Trigger trig) => new Xvt.Mission.Trigger(craftUpgrade(trig));
			/// <summary>Converts a Trigger for use in XWA.</summary>
			/// <remarks>CraftType indexes for SHPYD, REPYD, G/PLT and M/SC will be updated.</remarks>
			/// <param name="trig">The Trigger to convert.</param>
			/// <returns>A copy of <paramref name="trig"/> for XWA.</returns>
			public static implicit operator Xwa.Mission.Trigger(Trigger trig) => new Xwa.Mission.Trigger(craftUpgrade(trig));
		}
	}
}