using System;

namespace Idmr.Platform
{
	/// <summary>Collection class for fixed-size arrays such as Goals</summary>
	/// <remarks>There's a few more private members that are used in the child classes</remarks>
	public abstract class FixedSizeCollection
	{
		protected object[] _items;
		protected int _count = 0;

		/// <value>Gets the number of objects in the collection</value>
		/// <remarks>May not necessarily be the size of the internal array</remarks>
		public int Count { get { return _count; } }

		protected object _getItem(int index)
		{
			if (index >= 0 && index < _count) return _items[index];
			else return null;
		}
		protected void _setItem(int index, object item)
		{
			if (index >= 0 && index < _items.Length) _items[index] = item;
		}
	}
}