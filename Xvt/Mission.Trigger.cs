/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0, 120525
 * [NEW] conversions, _checkValues(), exceptions
 * [NEW] ToString() override
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class Mission : MissionFile
	{
		/// <summary>Object for a single Trigger</summary>
		[Serializable] public class Trigger	: BaseTrigger
		{
			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() : base(new byte[4]) { }
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			public Trigger(byte[] raw)
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				_items = new byte[4];
				_items = raw;
				_checkValues(this);
			}
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length value<br/><b>-or-</b><br/>Invalid member values</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Trigger(byte[] raw, int startIndex)
			{
				if (raw.Length < 4) throw new ArgumentException("Minimum length of raw is 4", "raw");
				if (raw.Length - startIndex < 4 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 4));
				_items = new byte[4];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
				_checkValues(this);
			}
			
			static void _checkValues(Trigger t)
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
			/// <remarks>Flightgroups are identified as <b>"FG:#"</b> and Teams are identified as <b>"TM:#"</b> for later substitution if required</remarks>
			/// <returns>Description of the trigger and targets if applicable</returns>
			public override string ToString()
			{
				string trig = "";
				if (Condition != 0 /*TRUE*/ && Condition != 10 /*FALSE*/)
				{
					trig = Strings.Amount[Amount] + " of ";
					switch (VariableType)
					{
						case 1:
							trig += "FG:" + Variable;
							break;
						case 2:
							trig += "Ship type " + Strings.CraftType[Variable+1];
							break;
						case 3:
							trig += "Ship class " + Strings.ShipClass[Variable];
							break;
						case 4:
							trig += "Object type " + Strings.ObjectType[Variable];
							break;
						case 5:
							trig += "IFF " + Strings.IFF[Variable];
							break;
						case 6:
							trig += "Ship orders " + Strings.Orders[Variable];
							break;
						case 7:
							trig += "Craft When " + Strings.CraftWhen[Variable];
							break;
						case 8:
							trig += "Global Group " + Variable;
							break;
						case 0xC:
							trig += "TM:" + Variable;
							break;
						case 0x15:
							trig += "Not TM:" + Variable;
							break;
						case 0x17:
							trig += "Global Unit " + Variable;
							break;
						default:
							trig += VariableType + " " + Variable;
							break;
					}
					trig += " must ";
				}
				trig += Strings.Trigger[Condition];
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
			/// <returns>A copy of <i>trig</i> for use in TIE95</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) { return new Tie.Mission.Trigger((byte[])trig); }
			/// <summary>Converts a Trigger for use in XWA</summary>
			/// <param name="trig">The Trigger to convert</param>
			/// <returns>A copy of <i>trig</i> for use in XWA</returns>
			public static implicit operator Xwa.Mission.Trigger(Trigger trig) { return new Xwa.Mission.Trigger((byte[])trig); }
		}
	}
}
