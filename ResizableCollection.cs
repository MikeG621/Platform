using System;

namespace Idmr.Platform
{
	/// <summary>Collection class for variable-sized arrays such as FlightGroups and In-Flight Messages</summary>
	/// <remarks>There's a few more private members that are used in the child classes</remarks>
	public abstract class ResizableCollection
	{
		protected object[] _items;
		protected int _count = 0;
		protected int _itemLimit;

		/// <value>Gets the number of objects in the Collection</value>
		public int Count { get { return _count; } }
		/// <value>Gets the maximum number of objects allowed in the Collection</value>
		/// <remarks>Used to control Add and Insert methods</remarks>
		public int ItemLimit { get { return _itemLimit; } }

		protected int _add(object item)
		{
			if (_count < _itemLimit)
			{
				object[] tempItems = _items;
				_items = new object[_count+1];
				for (int i=0;i<(_count);i++) _items[i] = tempItems[i];
				_items[_count] = item;
				_count++;
				return (short)(_count-1);
			}
			else return -1;
		}
		protected int _insert(int index, object item)
		{
			if (_count < ItemLimit && index >= 0 && index <= _count)
			{
				object[] tempItems = _items;
				_items = new object[_count+1];
				for (int i=0;i<index;i++) _items[i] = tempItems[i];
				_items[index] = item;
				for (int i=index;i<_count;i++) _items[i+1] = tempItems[i];
				_count++;
				return index;
			}
			else return -1;
		}
		protected int _removeAt(int index)
		{
			_count--;
			object[] tempItems = _items;
			_items = new object[_count];
			for (int i=0;i<index;i++) _items[i] = tempItems[i];
			for (int i=index;i<_count;i++) _items[i] = tempItems[i+1];
			return (index == _count ? index-1 : index);
		}

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