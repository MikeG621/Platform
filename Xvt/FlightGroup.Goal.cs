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
 * [UPD] cloning ctor now calls base [JB]
 * v5.6, 220103
 * [NEW] cloning ctor [JB]
 * v5.0, 201004
 * [UPD] Enabled, Team, Unknowns10-15 replaced with Get/SetEnabledForTeam()
 * v3.0, 180903
 * [NEW] TimeLimit, formerly Unk16 [JB]
 * v2.5, 170107
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
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length < 15) throw new ArgumentException("Minimum length of raw is 15", "raw");
				_items = new byte[15];
				ArrayFunctions.TrimArray(raw, 0, _items);
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, minimum Length of 15</param>
			/// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length</exception>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/></exception>
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
				string goal;
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
			/// <remarks>Equals <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b></remarks>
			public short Points
			{
				get => (short)((sbyte)_items[3] * 250);
				set => _items[3] = (byte)((value > 31750 ? 31750 : (value < -32000 ? -32000 : value)) / 250);
			}

			/// <summary>Gets whether the goal is enabled for the specified team.</summary>
			/// <param name="index">Team index</param>
			/// <returns><b>true</b> if the goal applies to the given team.</returns>
			/// <remarks>The EnabledForTeam array encompasses 10 elements ranging from offsets 0x4 to 0xD, which formerly contained Unknown10 through Unknown 15.</remarks>
			/// <exception cref="ArgumentOutOfRangeException">Team <paramref name="index"/> is not 0-9.</exception>
			public bool GetEnabledForTeam(int index)
			{
				if(index < 0 || index >= 10)
					throw new ArgumentOutOfRangeException("Team index must be 0 to 9, inclusive.");
				return (_items[4 + index] != 0);
			}
			/// <summary>Sets whether the goal is enabled for the specified team.</summary>
			/// <param name="index">Team index.</param>
			/// <param name="state">The value to apply.</param>
			/// <exception cref="ArgumentOutOfRangeException">Team <paramref name="index"/> is not 0-9.</exception>
			public void SetEnabledForTeam(int index, bool state)
			{
				if(index < 0 || index >= 10)
					throw new ArgumentOutOfRangeException("Team index must be 0 to 9, inclusive.");
				_items[4 + index] = Convert.ToByte(state);
			}
			/// <summary>Time limit</summary>
			/// <remarks>Time limit that goal must be finished within (seconds*5).  Previously Unknown16. Goal offset 0x0E</remarks>
			public byte TimeLimit
			{
				get => _items[14];
				set => _items[14] = value;
			}

			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary goals</remarks>
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
			/// <remarks>String is limited to 63 char. Not used for Secondary or Prevent goals</remarks>
			public string FailedText
			{
				get => _failedText;
				set => _failedText = StringFunctions.GetTrimmed(value, 63);
			}
			#endregion public properties
		}
	}
}
