/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
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
			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() : base(new byte[4]) { }

			/// <summary>Constructs a new Trigger from an existing Trigger. If null, a blank Trigger is created.</summary>
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
				_items = raw;
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
				string msg;
				if (t.Condition > 46) error = "Condition (" + t.Condition + ")";
				byte tempVar = t.Variable;
				CheckTarget(t.VariableType, ref tempVar, out msg);
				t.Variable = tempVar;
				if (msg != "") error += (error != "" ? ", " : "") + msg;
				if (error != "") throw new ArgumentException("Invalid values detected: " + error +  ".");
				if (t.Amount == 19) t.Amount = 6;	// "each special" to "100% special"
			}

			/// <summary>Returns a representative string of the Trigger</summary>
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b>, IFFs are <b>"IFF:#" and </b> Teams are <b>"TM:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the trigger and targets if applicable</returns>
			public override string ToString()
			{
				string trig = "";
				if (Condition != 0 /*TRUE*/ && Condition != 10 /*FALSE*/)
				{
					trig = BaseStrings.SafeString(Strings.Amount, Amount);
					trig += (trig.IndexOf(" of") >= 0 || trig.IndexOf(" in") >= 0) ? " " : " of ";
					switch (VariableType)
					{
						case 1:
							trig += "FG:" + Variable;
							break;
						case 2:
							trig += "Ship type " + BaseStrings.SafeString(Strings.CraftType, Variable + 1);
							break;
						case 3:
							trig += "Ship class " + BaseStrings.SafeString(Strings.ShipClass, Variable);
							break;
						case 4:
							trig += "Object type " + BaseStrings.SafeString(Strings.ObjectType, Variable);
							break;
						case 5:
							trig += "IFF:" + Variable;
							break;
						case 6:
							trig += "Craft with " + BaseStrings.SafeString(Strings.Orders, Variable) + " starting orders";
							break;
						case 7:
							trig += "Craft when " + BaseStrings.SafeString(Strings.CraftWhen, Variable);
							break;
						case 8:
							trig += "Global Group " + Variable;
							break;
						case 9:
							trig += "Craft with " + BaseStrings.SafeString(Strings.Rating, Variable) + " adjusted skill";
							break;
						case 0xA:
							trig += "Craft with primary status: " + BaseStrings.SafeString(Strings.Status, Variable);
							break;
						case 0xB:
							trig += "All craft";
							break;
						case 0xC:
							trig += "TM:" + Variable;
							break;
						case 0xD:
							trig += "(Player #" + Variable + ")";
							break;
						case 0xE:
							trig += "(Before elapsed time " + string.Format("{0}:{1:00}", Variable * 5 / 60, Variable * 5 % 60) + ")";
							break;
						case 0xF:
							trig += "All except FG:" + Variable;
							break;
						case 0x10:
							trig += "All except " + BaseStrings.SafeString(Strings.CraftType, Variable + 1) + "s";
							break;
						case 0x11:
							trig += "All except " + BaseStrings.SafeString(Strings.ShipClass, Variable);
							break;
						case 0x12:
							trig += "All except " + BaseStrings.SafeString(Strings.ObjectType, Variable);
							break;
						case 0x13:
							trig += "All except IFF:" + Variable;
							break;
						case 0x14:
							trig += "All except GG " + Variable;
							break;
						case 0x15:
							trig += "All except TM:" + Variable;
							break;
						case 0x16:
							trig += "(All except Player #" + (Variable + 1) + ")";
							break;
						case 0x17:
							trig += "Global Unit " + Variable;
							break;
						case 0x18:
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
			public static implicit operator Xwa.Mission.Trigger(Trigger trig) { return new Xwa.Mission.Trigger((byte[])trig); }
		}
	}
}
