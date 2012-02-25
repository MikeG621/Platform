/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG
 * 120212 - T[] to List<T> conversion
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain mission FG list</summary>
	/// <remarks>ItemLimit is set to Mission.FlightGroupLimit (46)</remarks>
	public class FlightGroupCollection : Idmr.Common.ResizableCollection<FlightGroup>
	{
		/// <summary>Create a new Collection with one FlightGroup</summary>
		public FlightGroupCollection()
		{
			_itemLimit = Mission.FlightGroupLimit;
			_items = new List<FlightGroup>(_itemLimit);
			_items.Add(new FlightGroup());
		}

		/// <summary>Creates a new Collection with multiple initial FlightGroups</summary>
		/// <param name="quantity">Number of FlightGroups to start with</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>quantity</i> is less than 1 or greater than Mission.FlightGroupLimit</exception>
		public FlightGroupCollection(int quantity)
		{
			_itemLimit = Mission.FlightGroupLimit;
			if (quantity < 1 || quantity > _itemLimit) throw new ArgumentOutOfRangeException("quantity", "Invalid quantity, must be 1-" + _itemLimit);
			_items = new List<FlightGroup>(_itemLimit);
			for (int i = 0; i < quantity; i++) _items.Add(new FlightGroup());
		}

		/// <summary>Adds a new FlightGroup to the end of the Collection</summary>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Add() { return _add(new FlightGroup()); }

		/// <summary>Inserts a new FlightGroup at the specified index</summary>
		/// <param name="index">Location of the FlightGroup</param>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Insert(int index) { return _insert(index, new FlightGroup()); }

		/// <summary>Removes all existing entries in the Collection, creates a single new FlightGroup</summary>
		/// <remarks>All existing FlightGroups are lost, first FG is re-initialized</remarks>
		public void Clear()
		{
			_items.Clear();
			_items.Add(new FlightGroup());
		}

		/// <summary>Deletes the FlightGroup at the specified index</summary>
		/// <remarks>If the first and only FlightGroup is selected, initializes it to a new FlightGroup</remarks>
		/// <param name="index">The index of the FlightGroup to be deleted</param>
		/// <returns>The index of the next available FlightGroup if successfull, otherwise -1</returns>
		public int RemoveAt(int index)
		{
			if (index >= 0 && index < Count && Count > 1) { return _removeAt(index); }
			else if (index == 0 && Count == 1)
			{
				_items[0] = new FlightGroup();
				return 0;
			}
			else return -1;
		}
	}
}
