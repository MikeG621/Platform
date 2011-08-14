using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object to maintain mission FG list</summary>
	/// <remarks>ItemLimit is set to Mission.FlightGroupLimit (48)</remarks>
	public class FlightGroupCollection : ResizableCollection
	{
		/// <summary>Create a new Collection with one FlightGroup</summary>
		public FlightGroupCollection()
		{
			_itemLimit = Mission.FlightGroupLimit;
			_count = 1;
			_items = new FlightGroup[1];
			_items[0] = new FlightGroup();
		}

		/// <summary>Create a new Collection with multiple initial FlightGroups</summary>
		/// <param name="quantity">Number of FlightGroups to start with</param>
		public FlightGroupCollection(int quantity)
		{
			_itemLimit = Mission.FlightGroupLimit;
			if (quantity <= _itemLimit && quantity > 0) _count = quantity;
			else if (quantity < 0) _count = 1;
			else _count = _itemLimit;
			_items = new FlightGroup[_count];
			for (int i=0;i<_count;i++) _items[i] = new FlightGroup();
		}

		/// <value>A single FlightGroup within the Collection</value>
		/// <example>FlightGroupCollection Flights = new FlightGroupCollection(3);<br>
		/// Flights[2].Name = "Last FG";<br>
		/// Flights[1] = new FlightGroup();<br>
		/// Flights[0].Name = Flights[2].Name</example>
		public FlightGroup this[int index]
		{
			get { return (FlightGroup)_getItem(index); }
			set { _setItem(index, value); }
		}

		/// <summary>Add a new FlightGroup to the end of the Collection</summary>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Add() { return _add(new FlightGroup()); }

		/// <summary>Adds the given FlightGroup to the end of the Collection</summary>
		/// <param name="flightGroup">The FlightGroup to be added</param>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Add(FlightGroup flightGroup) { return _add(flightGroup); }

		/// <summary>Inserts a new FlightGroup at the specified index</summary>
		/// <param name="index">Location of the FlightGroup</param>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Insert(int index) { return _insert(index, new FlightGroup()); }

		/// <summary>Inserts the given FlightGroup at the specified index</summary>
		/// <param name="index">Location of the FlightGroup</param>
		/// <param name="flightGroup">The FlightGroup to add</param>
		/// <returns>The index of the added FlightGroup if successfull, otherwise -1</returns>
		public int Insert(int index, FlightGroup flightGroup) { return _insert(index, flightGroup); }

		/// <summary>Removes all existing entries in the Collection, creates a single new FlightGroup</summary>
		/// <remarks>All existing FlightGroups are lost, first FG is re-initialized</remarks>
		public void Clear()
		{
			_count = 1;
			_items = new FlightGroup[1];
			_items[0] = new FlightGroup();
		}

		/// <summary>Deletes the FlightGroup at the specified index</summary>
		/// <remarks>If the first and only FlightGroup is selected, initializes it to a new FlightGroup</remarks>
		/// <param name="index">The index of the FlightGroup to be deleted</param>
		/// <returns>The index of the next available FlightGroup if successfull, otherwise -1</returns>
		public int RemoveAt(int index)
		{
			if (index >= 0 && index < _count && _count > 1) { return _removeAt(index); }
			else if (index == 0 && _count == 1)
			{
				_items[0] = new FlightGroup();
				return 0;
			}
			else return -1;
		}
	}
}
