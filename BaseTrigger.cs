﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [NEW] GetBytes, this[TriggerIndex], TriggerIndex.XwaParameter
 * v3.0, 180309
 * [NEW] helper functions for FG move/delete [JB]
 * v2.3, 150405
 * [NEW] Added TriggerIndex enum
 * v2.2, 150205
 * [FIX #5] marked Serializable
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implemented
 */
using System;

namespace Idmr.Platform
{
	/// <summary>Base class for mission Triggers.</summary>
	[Serializable]
	public abstract class BaseTrigger : Common.Indexer<byte>
	{
		/// <summary>Indexes within the Trigger array.</summary>
		public enum TriggerIndex : byte
		{
			/// <summary>The Trigger event.</summary>
			Condition,
			/// <summary>The category <see cref="Variable"/> belongs to.</summary>
			VariableType,
			/// <summary>The Trigger subject.</summary>
			Variable,
			/// <summary>The amount required to fire the event.</summary>
			Amount,
			/// <summary>The Region or FG paramater.</summary>
			/// <remarks>XWA only, should only be called from <see cref="Xwa.Mission.Trigger.Parameter"/> since it's really a short.</remarks>
			XwaParameter
		};

		/// <summary>Default constructor for derived classes.</summary>
		protected BaseTrigger() { /* do nothing */ }

		/// <summary>Default constructor for derived classes.</summary>
		/// <param name="raw">Raw data.</param>
		protected BaseTrigger(byte[] raw) : base(raw) { /* do nothing */ }

		/// <summary>Gets or sets the appropriate value.</summary>
		/// <param name="index">The selected index.</param>
		/// <returns>The selected value, or <b>255</b> if invalid selection.</returns>
		public byte this[TriggerIndex index]
		{
			get => (_items.Length == 4 && index == TriggerIndex.XwaParameter) ? (byte)255 : _items[(byte)index];
			set { if (_items.Length != 4 || index != TriggerIndex.XwaParameter) _items[(byte)index] = value; }
		}

		/// <summary>Gets or sets the Trigger itself.</summary>
		public byte Condition
		{
			get => _items[0];
			set => _items[0] = value;
		}
		/// <summary>Gets or sets the category <see cref="Variable"/> belongs to.</summary>
		public byte VariableType
		{
			get => _items[1];
			set => _items[1] = value;
		}
		/// <summary>Gets or sets the Trigger subject.</summary>
		public byte Variable
		{
			get => _items[2];
			set => _items[2] = value;
		}
		/// <summary>Gets or sets the amount required to fire the Trigger.</summary>
		public byte Amount
		{
			get => _items[3];
			set => _items[3] = value;
		}

		/// <summary>Helper function that changes Flight Group indexes during a FG Move or Delete operation.</summary>
		/// <remarks>Should not be called directly unless part of a larger FG Move or Delete operation.  FG references may exist in other mission properties, ensure those properties are adjusted when applicable.</remarks>
		/// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
		/// <param name="dstIndex">The FG index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
		/// <param name="delCond">Ignored unless FG is deleted.  If <b>true</b>, condition is set to "Always (true)" otherwise "Never (false)".</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="dstIndex"/> is greater than <b>255</b>.</exception>
		/// <returns>Returns <b>true</b> if anything was changed.</returns>
		public bool TransformFGReferences(int srcIndex, int dstIndex, bool delCond)
		{
			bool change = false;
			bool delete = false;
			if (dstIndex < 0)
			{
				dstIndex = 0;
				delete = true;
			}
			else if (dstIndex > 255) throw new ArgumentOutOfRangeException("TransformFGRef: dstIndex out of range.");

			change |= TransformFGReferencesExtended(srcIndex, dstIndex, delete, delCond);

			//In TIE/XvT/XWA, VariableType==1 is targeting a specific Flight Group.  Condition==0 is TRUE, Condition==10 is FALSE.
			if (VariableType == 1 && Variable == srcIndex)
			{
				change = true;
				Variable = (byte)dstIndex;
				if (delete)
				{
					Amount = 0;
					VariableType = 0;
					Variable = 0;
					Condition = (byte)(delCond ? 0 : 10); //this will expand the bool true/false into the condition true/false
				}
			}
			else if (VariableType == 1 && Variable > srcIndex && delete)
			{
				change = true;
				Variable--;  //If deleting, decrement FG index to maintain
			}
			return change;
		}
		/// <summary>This allows overrides to check additional properties without needing to override the base function.</summary>
		/// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
		/// <param name="dstIndex">The FG index to replace with.  Specify <b>-1</b> to Delete, or <b>zero</b> or above to Move.</param>
		/// <param name="delete">Whether or not to delete the FG.</param>
		/// <param name="delCond">Ignored unless FG is deleted.  If <b>true</b>, condition is set to "Always (true)" otherwise "Never (false)".</param>
		/// <returns>Always returns <b>false</b>.</returns>
		protected virtual bool TransformFGReferencesExtended(int srcIndex, int dstIndex, bool delete, bool delCond) => false;  /* do nothing */

		/// <summary>Helper function that changes Flight Group indexes during a Move (index swap) operation.</summary>
		/// <remarks>Should not be called directly unless part of a larger FG Move operation.  FG references may exist in other mission properties, ensure those properties are adjusted when applicable.</remarks>
		/// <param name="srcIndex">The FG index to match and replace.</param>
		/// <param name="dstIndex">The FG index to replace with.</param>
		/// <returns>Returns true if anything was changed.</returns>
		public bool SwapFGReferences(int srcIndex, int dstIndex)
		{
			bool change = false;
			change |= TransformFGReferences(dstIndex, 255, false);
			change |= TransformFGReferences(srcIndex, dstIndex, false);
			change |= TransformFGReferences(255, srcIndex, false);
			return change;
		}

		/// <summary>Gets a copy of the Trigger as a byte array.</summary>
		/// <remarks>Length is <b>4</b> for TIE and XvT/BoP, <b>6</b> for XWA.</remarks>
		/// <returns>The byte array equivalent.</returns>
		public byte[] GetBytes() => (byte[])_items.Clone();
	}
}