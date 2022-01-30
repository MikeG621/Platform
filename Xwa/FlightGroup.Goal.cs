/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2022 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7
 */

/* CHANGELOG
 * v5.7, 220127
 * [NEW] cloning ctor [JB]
 * v3.0, 180903
 * [NEW] Unknown 43 [JB]
 * [UPD] added remarks for Parameter [JB]
 * v2.7, 180509
 * [NEW] Prox condition in ToString
 * [UPD] ToString update [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implementation
 * [NEW] ToString() override
 * [NEW] exceptions
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for a single FlightGroup-specific Goal</summary>
		[Serializable]
		public class Goal : Indexer<byte>
		{
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";

			/// <summary>Initializes a blank Goal</summary>
			/// <remarks><see cref="Condition"/> is set to <b>"never (FALSE)"</b></remarks>
			public Goal()
			{
				_items = new byte[16];
				_items[1] = 10;
			}
			/// <summary>Initializes a new Goal from an existing Goal.</summary>
			/// <param name="other">Existing Goal to clone. If <b>null</b>, Goal will be blank.</param>
			/// <remarks><see cref="Condition"/> is set to <b>10</b> ("never (FALSE)") if <paramref name="other"/> is <b>null</b>.</remarks>
			public Goal(Goal other) : this()
			{
				if (other != null)
				{
					Array.Copy(other._items, _items, _items.Length);
					_incompleteText = other._incompleteText;
					_completeText = other._completeText;
					_failedText = other._failedText;
				}
			}

			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 16</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length < 16) throw new ArgumentException("Minimum length of raw is 16", "raw");
				_items = new byte[16];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}

			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 16</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> results in reading outside the bounds of <i>raw</i></exception>
			public Goal(byte[] raw, int startIndex)
			{
				if (raw.Length < 16) throw new ArgumentException("Minimum length of raw is 16", "raw");
				if (raw.Length - startIndex < 16 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 16));
				_items = new byte[16];
				ArrayFunctions.TrimArray(raw, startIndex, _items);
			}

			/// <summary>Gets a representative string of the Goal</summary>
			/// <returns>Description of the goal if enabled, otherwise <b>"None"</b></returns>
			public override string ToString()
			{
				string goal;
				if (Condition != 0 && Condition != 10)
				{
					goal = (Condition == 0x31 || Condition == 0x32 ? "Any" : Strings.Amount[Amount]) + " of Flight Group ";
					goal += (Argument == 0 ? "must" : (Argument == 1 ? "must NOT" : (Argument == 2 ? "BONUS must" : "BONUS must NOT")));
					if (Condition == 0x31 || Condition == 0x32)
					{
						goal += (Condition == 0x32 ? " NOT" : "") + " be within ";
						double dist;
						if (Amount == 0) dist = 0.05;
						else if (Amount <= 10) dist = 0.1 * Amount;
						else dist = Amount * 0.5 - 4;
						goal += dist + " km of FG2:" + Parameter;
					}
					else goal += " " + Strings.Trigger[Condition];
					if (goal.Contains("Region")) goal += " " + Parameter;
					goal += " (" + Points + " points)";
				}
				else goal = "None";
				return goal;
			}

			#region public properties
			/// <summary>Gets or sets the goal behaviour</summary>
			/// <remarks>Values are <b>0-3</b>; must, must not (Prevent), BONUS must, BONUS must not (bonus prevent)</remarks>
			public byte Argument
			{
				get => _items[0];
				set => _items[0] = value;
			}
			/// <summary>Gets or sets the Goal trigger</summary>
			public byte Condition
			{
				get => _items[1];
				set => _items[1] = value;
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="Condition"/></summary>
			public byte Amount
			{
				get => _items[2];
				set => _items[2] = value;
			}
			/// <summary>Gets or sets the points value stored in the file</summary>
			public sbyte RawPoints
			{
				get => (sbyte)_items[3];
				set => _items[3] = (byte)value;
			}
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equals <see cref="RawPoints"/> * 25, limited from <b>-3200</b> to <b>+3175</b></remarks>
			public short Points
			{
				get => (short)(RawPoints * 25);
				set => RawPoints = (sbyte)((value > 3175 ? 3175 : (value < -3200 ? -3200 : value)) / 25);
			}
			/// <summary>Gets or sets if the Goal is active</summary>
			public bool Enabled
			{
				get => Convert.ToBoolean(_items[4]);
				set => _items[4] = Convert.ToByte(value);
			}
			/// <summary>Gets or sets which Team the Goal applies to</summary>
			public byte Team
			{
				get => _items[5];
				set => _items[5] = value;
			}
			/// <summary>Gets or sets the unknown</summary>
			public byte Unknown42
			{
				get => _items[13];
				set => _items[13] = value;
			}
			/// <summary>Gets or sets the additional Goal setting</summary>
			/// <remarks>Shared as both an extra parameter for certain orders, or goal time limit for anything else.  Zero for no time limit.  Standard delay format used by Message and Order waiting times.</remarks>
			public byte Parameter
			{
				get => _items[14];
				set => _items[14] = value;
			}
			/// <summary>Gets or sets the location within the Active Sequence</summary>
			public byte ActiveSequence
			{
				get => _items[15];
				set => _items[15] = value;
			}

			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Bonus goals</remarks>
			public string IncompleteText
			{
				get => _incompleteText;
				set => _incompleteText = StringFunctions.GetTrimmed(value, 63);
			}
			/// <summary>Gets or sets the goal text shown after completion</summary>
			/// <remarks>String is limited to 63 char</remarks>
			public string CompleteText
			{
				get => _completeText;
				set => _completeText = StringFunctions.GetTrimmed(value, 63);
			}
			/// <summary>Gets or sets the goal text shown after failure</summary>
			/// <remarks>String is limited to 63 char. Not used for Bonus or Prevent goals</remarks>
			public string FailedText
			{
				get => _failedText;
				set => _failedText = StringFunctions.GetTrimmed(value, 63);
			}

			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x4F</remarks>
			public bool Unknown15 { get; set; }
			#endregion public properties
		}
	}
}