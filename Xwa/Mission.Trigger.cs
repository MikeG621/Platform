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
 * [NEW] conversions, _craftDowngrade(), exceptions, ToString() override
 * [UPD] ctor(byte[]) accepts Length 4
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class Mission : MissionFile
	{
		/// <summary>Object for a single Trigger</summary>
		[Serializable] public class Trigger : BaseTrigger
		{
			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() : base(new byte[6]) { }
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i> Length</exception>
			public Trigger(byte[] raw)
			{
				_items = new byte[6];
				if (raw.Length >= 6) ArrayFunctions.TrimArray(raw, 0, _items);
				else if (raw.Length >= 4) for (int i = 0; i < 4; i++) _items[i] = raw[i];
				else throw new ArgumentException("Minimum length of raw is 4", "raw");
			}
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <remarks>If <i>raw</i>.Length is 6 or greater, reads six bytes. If the length is 4 or 5, reads only four bytes</remarks>
			/// <param name="raw">Raw data, minimum Length of 4</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i> Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
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
			
			static byte[] _craftDowngrade(Trigger t)
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
							trig += "Ship type " + Strings.CraftType[Variable + 1];
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
						case 0xD:
							trig += "Player of GG " + Variable;
							break;
						case 0x15:
							trig += "Not TM:" + Variable;
							break;
						case 0x17:
							trig += "Global Unit " + Variable;
							break;
						case 0x19:
							trig += "Global Cargo " + Variable;
							break;
						case 0x1B:
							trig += "Message #" + Variable;
							break;
						default:
							trig += VariableType + " " + Variable;
							break;
					}
					trig += " must ";
				}
				trig += Strings.Trigger[Condition];
				if (trig.Contains("Region")) trig += " " + Parameter1;
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
			/// <returns>A copy of <i>trig</i> for use in TIE95</returns>
			public static explicit operator Tie.Mission.Trigger(Trigger trig) { return new Tie.Mission.Trigger(_craftDowngrade(trig)); }	// Parameters lost
			/// <summary>Converts a Trigger for use in XvT</summary>
			/// <remarks>Parameters are lost in the conversion</remarks>
			/// <param name="trig">The Trigger to convert</param>
			/// <exception cref="ArgumentException">Invalid values detected</exception>
			/// <returns>A copy of <i>trig</i> for use in XvT</returns>
			public static explicit operator Xvt.Mission.Trigger(Trigger trig) { return new Xvt.Mission.Trigger(_craftDowngrade(trig)); }	// Parameters lost
			
			/// <summary>Gets or sets the first additional setting</summary>
			public byte Parameter1
			{
				get { return _items[4]; }
				set { _items[4] = value; }
			}
			/// <summary>Gets or sets the second additional setting</summary>
			public byte Parameter2
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
		}
	}
}
