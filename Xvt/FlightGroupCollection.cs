/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 * [NEW] IsModified implementation
 * [NEW] SetCount
 * v2.0, 120525
 * [NEW] GetList()
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain mission FG list</summary>
	/// <remarks><see cref="Common.ResizableCollection{T}.ItemLimit"/> is set to <see cref="Mission.FlightGroupLimit"/> (48)</remarks>
	public class FlightGroupCollection : Common.ResizableCollection<FlightGroup>
	{
		/// <summary>Create a new Collection with one FlightGroup</summary>
		public FlightGroupCollection()
		{
			_itemLimit = Mission.FlightGroupLimit;
			_items = new List<FlightGroup>(_itemLimit)
			{
				new FlightGroup()
			};
		}

		/// <summary>Creates a new Collection with multiple initial FlightGroups</summary>
		/// <param name="quantity">Number of FlightGroups to start with</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>quantity</i> is less than 1 or greater than <see cref="Common.ResizableCollection{T}.ItemLimit"/></exception>
		public FlightGroupCollection(int quantity)
		{
			_itemLimit = Mission.FlightGroupLimit;
			if (quantity < 1 || quantity > _itemLimit) throw new ArgumentOutOfRangeException("quantity", "Invalid quantity, must be 1-" + _itemLimit);
			_items = new List<FlightGroup>(_itemLimit);
			for (int i = 0; i < quantity; i++) _items.Add(new FlightGroup());
		}

		/// <summary>Adds a new FlightGroup to the end of the Collection</summary>
		/// <returns>The index of the added FlightGroup if successfull, otherwise <b>-1</b></returns>
		public int Add()
		{
			int result = _add(new FlightGroup());
			if (result != -1 && !_isLoading) _isModified = true;
			return result;
		}

		/// <summary>Inserts a new FlightGroup at the specified index</summary>
		/// <param name="index">Location of the FlightGroup</param>
		/// <returns>The index of the added FlightGroup if successfull, otherwise <b>-1</b></returns>
		public int Insert(int index)
		{
			int result = _insert(index, new FlightGroup());
			if (result != -1) _isModified = true;
			return result;
		}

		/// <summary>Removes all existing entries in the Collection, creates a single new FlightGroup</summary>
		/// <remarks>All existing FlightGroups are lost, first FG is re-initialized</remarks>
		public override void Clear()
		{
			_items.Clear();
			_items.Add(new FlightGroup());
			if (!_isLoading) _isModified = true;
		}

		/// <summary>Deletes the FlightGroup at the specified index</summary>
		/// <remarks>If the first and only FlightGroup is selected, initializes it to a new FlightGroup</remarks>
		/// <param name="index">The index of the FlightGroup to be deleted</param>
		/// <returns>The index of the next available FlightGroup if successfull, otherwise <b>-1</b></returns>
		public int RemoveAt(int index)
		{
			int result = -1;
			if (index >= 0 && index < Count && Count > 1) { result = _removeAt(index); }
			else if (index == 0 && Count == 1)
			{
				_items[0] = new FlightGroup();
				result = 0;
			}
			if (result != -1) _isModified = true;
			return result;
		}

		/// <summary>Expands or contracts the Collection, populating as necessary</summary>
		/// <param name="value">The new size of the Collection. Must be greater than <b>0</b>.</param>
		/// <param name="allowTruncate">Controls if the Collection is allowed to get smaller</param>
		/// <exception cref="InvalidOperationException"><i>value</i> is smaller than <see cref="Common.FixedSizeCollection{T}.Count"/> and <i>allowTruncate</i> is <b>false</b>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><i>value</i> must be greater than 0.</exception>
		/// <remarks>If the Collection expands, the new items will be a new <see cref="FlightGroup"/>. When truncating, items will be removed starting from the last index.</remarks>
		public override void SetCount(int value, bool allowTruncate)
		{
			if (value == Count) return;
			else if (value < 1 || value > _itemLimit) throw new ArgumentOutOfRangeException("value", "Invalid quantity, must be 1-" + _itemLimit);
			else if (value < Count)
			{
				if (!allowTruncate) throw new InvalidOperationException("Reducing 'value' will cause data loss");
				else while (Count > value) RemoveAt(Count - 1);
			}
			else while (Count < value) Add();
		}
		
		/// <summary>Provides quick access to an array of FlightGroup names</summary>
		/// <returns>An array of of short-form string representations</returns>
		public string[] GetList()
		{
			string[] list = new string[Count];
			for (int i = 0; i < Count; i++) list[i] = _items[i].ToString(false);
			return list;
		}
	}
}
