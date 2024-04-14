/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0+
 */

/* CHANGELOG
 * [NEW] Format spec implemented
 * [NEW] GetBytes
 * v3.0, 180309
 * [NEW] helper functions for FG move/delete [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * - Release
 */

using System;
using Idmr.Common;

namespace Idmr.Platform
{
	public abstract partial class BaseFlightGroup
	{
		/// <summary>Base class for FlightGroup orders.</summary>
		[Serializable]
		public abstract class BaseOrder : Indexer<byte>
		{
			// Doesn't need protected constructors, all handling done in derived classes
			#region public properties
			/// <summary>Gets or sets the command for the FlightGroup.</summary>
			public byte Command
			{
				get => _items[0];
				set => _items[0] = value;
			}
			/// <summary>Gets or sets the throttle setting.</summary>
			/// <remarks>Multiply value by 10 to get Throttle percent.</remarks>
			public byte Throttle
			{
				get => _items[1];
				set => _items[1] = value;
			}
			/// <summary>Gets or sets the first order-specific setting.</summary>
			public byte Variable1
			{
				get => _items[2];
				set => _items[2] = value;
			}
			/// <summary>Gets or sets the second order-specific setting.</summary>
			public byte Variable2
			{
				get => _items[3];
				set => _items[3] = value;
			}
			/// <summary>Gets or sets the third order-specific setting.</summary>
			/// <remarks>Only known to be used in XWA.</remarks>
			public byte Variable3
			{
				get => _items[4];
				set => _items[4] = value;
			}
			/// <summary>Gets or sets the fourth order-specific setting.</summary>
			/// <remarks>Never known to be used.</remarks>
			public byte Variable4
			{
				get => _items[5];
				set => _items[5] = value;
			}
			/// <summary>Gets or sets the Type for <see cref="Target3"/>.</summary>
			public byte Target3Type
			{
				get => _items[6];
				set => _items[6] = value;
			}
			/// <summary>Gets or sets the Type for <see cref="Target4"/>.</summary>
			public byte Target4Type
			{
				get => _items[7];
				set => _items[7] = value;
			}
			/// <summary>Gets or sets the third target.</summary>
			public byte Target3
			{
				get => _items[8];
				set => _items[8] = value;
			}
			/// <summary>Gets or sets the fourth target.</summary>
			public byte Target4
			{
				get => _items[9];
				set => _items[9] = value;
			}
			/// <summary>Gets or sets if the T3 and T4 settings are mutually exclusive.</summary>
			/// <remarks><b>false</b> is And/If and <b>true</b> is Or.</remarks>
			public bool T3AndOrT4
			{
				get => Convert.ToBoolean(_items[10]);
				set => _items[10] = Convert.ToByte(value);
			}
			/// <summary>Gets or sets the Type for <see cref="Target1"/>.</summary>
			public byte Target1Type
			{
				get => _items[12];
				set => _items[12] = value;
			}
			/// <summary>Gets or sets the first target.</summary>
			public byte Target1
			{
				get => _items[13];
				set => _items[13] = value;
			}
			/// <summary>Gets or sets the Type for <see cref="Target2"/>.</summary>
			public byte Target2Type
			{
				get => _items[14];
				set => _items[14] = value;
			}
			/// <summary>Gets or sets the second target.</summary>
			public byte Target2
			{
				get => _items[15];
				set => _items[15] = value;
			}
			/// <summary>Gets or sets if the T1 and T2 settings are mutually exclusive.</summary>
			/// <remarks><b>false</b> is And/If and <b>true</b> is Or.</remarks>
			public bool T1AndOrT2
			{
				get => Convert.ToBoolean(_items[16]);
				set => _items[16] = Convert.ToByte(value);
			}
			#endregion public properties

			#region public methods
			/// <summary>Transforms an Order that targets a specific Flight Group index.</summary>
			/// <remarks>If <paramref name="dstIndex"/> is negative, it will delete the Order.  If the Order is deleted, it will reset the trigger condition to either TRUE or FALSE depending on the state of <b>delCond</b>.</remarks>
			/// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
			/// <param name="dstIndex">The FG index to replace with.  Specify <b>-1</b> to Delete, or zero or above to Move.</param>
			/// <returns>Returns <b>true</b> if anything was changed.</returns>
			public bool TransformFGReferences(int srcIndex, int dstIndex)
			{
				bool change = TransformFGReferencesExtended(srcIndex, dstIndex);

				byte dst = (byte)dstIndex;
				bool delete = false;
				if (dstIndex < 0)
				{
					dst = 0;
					delete = true;
				}

				//[JB] This looks ugly but it's compact.  If we match, change target (or delete by nullifying Target and Type).  If we don't match, check if deleting and decrement indexes if necessary.
				if (Target1Type == 1 && Target1 == srcIndex) { change = true; Target1 = dst; if (delete) Target1Type = 0; } else if (Target1Type == 1 && Target1 > srcIndex && delete) { change = true; Target1--; }
				if (Target2Type == 1 && Target2 == srcIndex) { change = true; Target2 = dst; if (delete) Target2Type = 0; } else if (Target2Type == 1 && Target2 > srcIndex && delete) { change = true; Target2--; }
				if (Target3Type == 1 && Target3 == srcIndex) { change = true; Target3 = dst; if (delete) Target3Type = 0; } else if (Target3Type == 1 && Target3 > srcIndex && delete) { change = true; Target3--; }
				if (Target4Type == 1 && Target4 == srcIndex) { change = true; Target4 = dst; if (delete) Target4Type = 0; } else if (Target4Type == 1 && Target4 > srcIndex && delete) { change = true; Target4--; }

				if (Command == 0x12) //The "Drop Off" Order targets a Flight Group number.
				{
					if ((Variable2 == 1 + srcIndex) || (Variable2 == srcIndex && Variable2 == 255)) //255 is used as temp while swapping.  Ensure if we're swapping temp back to normal.
					{
						change = true;
						Variable2 = dst;
						if (Variable2 != 255) //Don't modify if temp
							Variable2++;      //Increment so it's one-based.
						if (delete) Variable2 = 0;
					}
					else if (Variable2 > 1 + srcIndex && delete)
					{
						change = true;
						Variable2--;
					}
				}
				return change;
			}
			/// <summary>This stub function allows overrides to check and modify additional properties without needing to override the base function.</summary>
			/// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
			/// <param name="dstIndex">The FG index to replace with.  Specify <b>-1</b> to Delete, or zero or above to Move.</param>
			/// <returns>Always returns <b>false</b>.</returns>
			protected virtual bool TransformFGReferencesExtended(int srcIndex, int dstIndex) => false;  /* do nothing */

			/// <summary>Gets a copy of the Order as a byte array.</summary>
			/// <remarks>Gets only the values, not any text or Waypoints. Length is <b>18</b> for TIE, <b>19</b> for XvT/BoP, and <b>20</b> for XWA.</remarks>
			/// <returns>The byte array equivalent.</returns>
			public byte[] GetBytes() => (byte[])_items.Clone();
			#endregion public methods
		}
	}
}
