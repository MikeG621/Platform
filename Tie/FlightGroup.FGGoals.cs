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
 * [NEW] GetBytes
 * v5.7.5, 230116
 * [DEL #12] SecretCondition and Secret amount, still accessible via the Indexer
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implementation
 * [NEW] exceptions
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object for the Flightgroup-specific mission goals.</summary>
		[Serializable] public class FGGoals : Indexer<byte>
		{
			/// <summary>Initializes a new instance with all goals set to "never (FALSE)" (10).</summary>
			public FGGoals()
			{
				_items = new byte[9];
				for (int i = 0; i < _items.Length; i += 2) _items[i] = 10;
			}
			
			/// <summary>Initializes the Goals from raw data.</summary>
			/// <param name="raw">Raw byte data, must have Length of 9.</param>
			/// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length.</exception>
			public FGGoals(byte[] raw) : this()
			{
				if (raw.Length < 9) throw new ArgumentException("Minimum length of raw is 9", "raw");
				Array.Copy(raw, _items, _items.Length);
			}

            /// <summary>Initializes the Goals from raw data.</summary>
            /// <param name="raw">Raw byte data.</param>
            /// <param name="startIndex">Offset within <paramref name="raw"/> to begin reading.</param>
            /// <exception cref="ArgumentException">Invalid <paramref name="raw"/>.Length.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> results in reading outside the bounds of <paramref name="raw"/>.</exception>
            public FGGoals(byte[] raw, int startIndex) : this()
			{
				if (raw.Length < 9) throw new ArgumentException("Minimum length of raw is 9", "raw");
				if (raw.Length - startIndex < 9 || startIndex < 0)
					throw new ArgumentOutOfRangeException("For provided value of raw, startIndex must be 0-" + (raw.Length - 9));
				ArrayFunctions.TrimArray(raw, startIndex, _items);
			}
			
			#region public properties
			/// <summary>Gets or sets the Primary goal.</summary>
			public byte PrimaryCondition
			{
				get => _items[0];
				set => _items[0] = value;
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="PrimaryCondition"/>.</summary>
			public byte PrimaryAmount
			{
				get => _items[1];
				set => _items[1] = value;
			}
			/// <summary>Gets or sets the Secondary goal.</summary>
			public byte SecondaryCondition
			{
				get => _items[2];
				set => _items[2] = value;
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="SecondaryCondition"/>.</summary>
			public byte SecondaryAmount
			{
				get => _items[3];
				set => _items[3] = value;
			}
			/// <summary>Gets or sets the Bonus goal.</summary>
			public byte BonusCondition
			{
				get => _items[6];
				set => _items[6] = value;
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <see cref="BonusCondition"/>.</summary>
			public byte BonusAmount
			{
				get => _items[7];
				set => _items[7] = value;
			}
			/// <summary>Gets or sets the raw points value stored in the file.</summary>
			public sbyte RawBonusPoints
			{
				get => (sbyte)_items[8];
				set => _items[8] = (byte)value;
			}
			/// <summary>Gets or sets the points awarded or subtracted upon Bonus goal completion.</summary>
			/// <remarks>Equals <see cref="RawBonusPoints"/> * 50, limited from <b>-6400</b> to <b>+6350</b>.</remarks>
			public short BonusPoints
			{
				get => (short)(RawBonusPoints * 50);
				set => RawBonusPoints = (sbyte)((value > 6350 ? 6350 : (value < -6400 ? -6400 : value)) / 50);
			}
			#endregion public properties

			/// <summary>Gets a copy of the Goals as a byte array.</summary>
			/// <remarks>Length is <b>9</b>.</remarks>
			/// <returns>The byte array equivalent.</returns>
			public byte[] GetBytes() => (byte[])_items.Clone();
		}		
	}
}