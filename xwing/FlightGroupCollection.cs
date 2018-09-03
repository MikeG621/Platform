/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0
 */

/* CHANGELOG
 * v3.0, 180309
 * [NEW] created [JB]
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xwing
{
	/// <summary>Object to maintain mission FG list</summary>
	/// <remarks><see cref="Common.ResizableCollection{T}.ItemLimit"/> is set to <see cref="Mission.FlightGroupLimit"/> (48)</remarks>
	public class FlightGroupCollection : Common.ResizableCollection<FlightGroup>
	{
		/// <summary>Creates a new Collection with one FlightGroup</summary>
		public FlightGroupCollection()
		{
			_itemLimit = Mission.FlightGroupLimit;
			_items = new List<FlightGroup>(_itemLimit);
			_items.Add(new FlightGroup());
		}

		/// <summary>Creates a new Collection with multiple initial FlightGroups</summary>
		/// <param name="quantity">Number of FlightGroups to start with</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>quantity</i> is less than <b>1</b> or greater than <see cref="Common.ResizableCollection{T}.ItemLimit"/></exception>
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
            if (index >= 0 && index < Count)   //[JB] Extra cleanup and reference adjustments here.
            {
                NullifyReferences(index);
                DropElementsAbove(index);
            }
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
		/// <returns>A new array of short-form string representations</returns>
		public string[] GetList()
		{
			string[] list = new string[Count];
			for (int i = 0; i < Count; i++) list[i] = _items[i].ToString(false);
			return list;
		}

        /// <summary>Retrieves the index of the first Flight Group.</summary>
        /// <remarks>Flight Groups and Object Groups are distinct in-game, but are stored in the same list for editing purposes.  Some editing functions like FG re-arrangement must take care not to mix them so that Flight Group indexes and Object Group indexes don't mix.</remarks>
        /// <returns>Returns an index, or -1 if not found.</returns>
        public int GetFirstOfFlightGroup()
        {
            for(int i = 0; i < Count; i++)
                if(_items[i].IsFlightGroup())
                    return i;
            return -1;
        }
		/// <summary>Retrieves the index of the last FlightGroup.</summary>
		/// /// <remarks>Flight Groups and Object Groups are distinct in-game, but are stored in the same list for editing purposes.  Some editing functions like FG re-arrangement must take care not to mix them so that Flight Group indexes and Object Group indexes don't mix.</remarks>
		/// <returns>Returns an index, or -1 if not found.</returns>
		public int GetLastOfFlightGroup()
        {
            for (int i = Count - 1; i >= 0; i--)
                if (_items[i].IsFlightGroup())
                    return i;
            return -1;
        }
		/// <summary>Retrieves the index of the first Object</summary>
		/// <remarks>Flight Groups and Object Groups are distinct in-game, but are stored in the same list for editing purposes.  Some editing functions like FG re-arrangement must take care not to mix them so that Flight Group indexes and Object Group indexes don't mix.</remarks>
		/// <returns>Returns an index, or -1 if not found.</returns>
		public int GetFirstOfObjectGroup()
        {
            for (int i = 0; i < Count; i++)
                if (!_items[i].IsFlightGroup())
                    return i;
            return -1;
        }
		/// <summary>Retrives the index of the last Object.</summary>
		/// <remarks>Flight Groups and Object Groups are distinct in-game, but are stored in the same list for editing purposes.  Some editing functions like FG re-arrangement must take care not to mix them so that Flight Group indexes and Object Group indexes don't mix.</remarks>
		/// <returns>Returns an index, or -1 if not found.</returns>
		public int GetLastOfObjectGroup()
        {
            for (int i = Count - 1; i >= 0; i--)
                if (!_items[i].IsFlightGroup())
                    return i;
            return -1;
        }
        /// <summary>Moves the FlightGroup at the specified index up one slot.</summary>
        /// <remarks>No effect if the first FG slot.  Automatically adjusts FG references to compensate for index changes.</remarks>
        /// <param name="index">The index of the FlightGroup to be moved</param>
        /// <returns>Returns true if successful (indexes were valid).</returns>
        public bool MoveUp(int index)
        {
            if (index <= 0 || index >= Count) return false;
            if (_items[index].IsFlightGroup() && index <= GetFirstOfFlightGroup()) return false;
            if (!_items[index].IsFlightGroup() && index <= GetFirstOfObjectGroup()) return false;

            Swap(index, index - 1);
            return true;
        }

        /// <summary>Moves the FlightGroup at the specified index down one slot.</summary>
        /// <remarks>No effect if the last FG slot.  Automatically adjusts FG references to compensate for index changes.</remarks>
        /// <param name="index">The index of the FlightGroup to be moved</param>
        /// <returns>Returns true if successful (indexes were valid).</returns>
        public bool MoveDown(int index)
        {
            if (index < 0 || index >= Count - 1) return false;
            if (_items[index].IsFlightGroup() && index >= GetLastOfFlightGroup()) return false;
            if (!_items[index].IsFlightGroup() && index >= GetLastOfObjectGroup()) return false;

            Swap(index, index + 1);
            return true;
        }

        /// <summary>Swaps two FlightGroup objects.</summary>
        /// <remarks>Indexes are NOT verified.</remarks>
		/// <param name="srcIndex">First FG index</param>
		/// <param name="dstIndex">Second FG index</param>
        public void Swap(int srcIndex, int dstIndex)
        {
            foreach (var fg in _items)
            {
                fg.ReplaceFGReference(srcIndex, -2); //-1 is reserved for null FG.
                fg.ReplaceFGReference(dstIndex, -3);
            }
            FlightGroup temp = _items[srcIndex];
            _items[srcIndex] = _items[dstIndex];
            _items[dstIndex] = temp;

            foreach (var fg in _items)
            {
                fg.ReplaceFGReference(-2, dstIndex); //-1 is reserved for null FG.
                fg.ReplaceFGReference(-3, srcIndex);
            }
        }

		/// <summary>Removes all references to the FlightGroup</summary>
		/// <param name="index">FG index</param>
        public void NullifyReferences(int index)
        {
            foreach(var fg in _items)
                fg.ReplaceFGReference(index, -1);
        }

        /// <summary>Helper function to erase all references and make corrective adjustments to array indexes in preparation for a graceful delete.</summary>
		/// <param name="index">The FG index to be removed</param>
        public void DropElementsAbove(int index)
        {
            for (int i = 0; i < Count; i++)
            {
                FlightGroup fg = _items[i];

                //Since the array element will be deleted, decrement indexes to accomodate the shift.
                if (fg.Mothership > index) fg.Mothership--;
                if (fg.ArrivalFG > index) fg.ArrivalFG--;
                if (fg.TargetPrimary > index) fg.TargetPrimary--;
                if (fg.TargetSecondary > index) fg.TargetSecondary--;
            }
        }
    }
}
