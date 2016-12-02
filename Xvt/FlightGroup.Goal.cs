/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2016 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1+
 */

/* CHANGELOG
 * [UPD] Changed goal visibility due to teams [JB]
 * [FIX] points casting [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implementation
 * [NEW] ToString() override
 * [NEW] exceptions
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for a single FlightGroup-specific Goal</summary>
		[Serializable] public class Goal : Indexer<byte>
		{
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks><see cref="Condition"/> is set to <b>10</b> ("never (FALSE)")</remarks>
			public Goal()
			{
				_items = new byte[15];
				_items[1] = 10;
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length < 15) throw new ArgumentException("Minimum length of raw is 15", "raw");
				_items = new byte[15];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Goal(byte[] raw, int startIndex)
			{
				if (raw.Length < 15) throw new ArgumentException("Minimum length of raw is 15", "raw");
				if (raw.Length - startIndex < 15 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 15));
				_items = new byte[15];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
			}

			/// <summary>Gets a representative string of the Goal</summary>
			/// <returns>Description of the goal if enabled, otherwise <b>"None"</b></returns>
			public override string ToString()
			{
				string goal = "";
				if (Condition != 0 && Condition != 10)
				{
					goal = Strings.Amount[Amount] + " of Flight Group ";
					goal += (Argument == 0 ? "must" : (Argument == 1 ? "must NOT" : (Argument == 2 ? "BONUS must" : "BONUS must NOT")));
					goal += " " + Strings.Trigger[Condition] + " (" + Points + " points)";
				}
				else goal = "None";
				return goal;
			}

			#region public properties
			/// <summary>Gets or sets the goal behaviour</summary>
			/// <remarks>Values are <b>0-3</b>; must, must not (prevent), BONUS must, BONUS must not (bonus prevent)</remarks>
			public byte Argument
			{
				get { return _items[0]; }
				set { _items[0] = value; }
			}
			/// <summary>Gets or sets the Goal trigger</summary>
			public byte Condition
			{
				get { return _items[1]; }
				set { _items[1] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="Condition"/></summary>
			public byte Amount
			{
				get { return _items[2]; }
				set { _items[2] = value; }
			}
			/// <summary>Gets or sets the points value stored in the file</summary>
			public sbyte RawPoints
			{
				get { return (sbyte)_items[3]; }
				set { _items[3] = (byte)value; }
			}
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equals <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b></remarks>
			public short Points
			{
				get { return (short)((sbyte)_items[3] * 250); }
				set { _items[3] = (byte)((value > 31750 ? 31750 : (value < -32000 ? -32000 : value)) / 250); }
			}
			/// <summary>Gets or sets if the Goal is active</summary>
			public bool Enabled
			{
				get { return Convert.ToBoolean(_items[4]); }
				set { _items[4] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets which Team the Goal applies to</summary>
			public byte Team
			{
				get { return _items[5]; }
				set { _items[5] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x06</remarks>
			public bool Unknown10
			{
				get { return Convert.ToBoolean(_items[6]); }
				set { _items[6] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x07</remarks>
			public bool Unknown11
			{
				get { return Convert.ToBoolean(_items[7]); }
				set { _items[7] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x08</remarks>
			public bool Unknown12
			{
				get { return Convert.ToBoolean(_items[8]); }
				set { _items[8] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0B</remarks>
			public byte Unknown13
			{
				get { return _items[11]; }
				set { _items[11] = value; }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0C</remarks>
			public bool Unknown14
			{
				get { return Convert.ToBoolean(_items[12]); }
				set { _items[12] = Convert.ToByte(value); }
			}
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x0E</remarks>
			public byte Unknown16
			{
				get { return _items[14]; }
				set { _items[14] = value; }
			}
			
			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary goals</remarks>
			public string IncompleteText
			{
				get { return _incompleteText; }
				set { _incompleteText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after completion</summary>
			/// <remarks>String is limited to 63 char</remarks>
			public string CompleteText
			{
				get { return _completeText; }
				set { _completeText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after failure</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary or Prevent goals</remarks>
			public string FailedText
			{
				get { return _failedText; }
				set { _failedText = StringFunctions.GetTrimmed(value, 63); }
			}
			#endregion public properties
		}
	}
}
