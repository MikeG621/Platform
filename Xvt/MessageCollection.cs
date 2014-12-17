/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 * [NEW] IsModified implementation
 * [NEW] SetCount
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object to maintain mission in-flight messages</summary>
	/// <remarks><see cref="Idmr.Common.ResizableCollection{T}.ItemLimit"/> is set to <see cref="Mission.MessageLimit"/> (64)</remarks>
	public class MessageCollection : Idmr.Common.ResizableCollection<Message>
	{
		/// <summary>Creates a new empty Collection</summary>
		public MessageCollection()
		{
			_itemLimit = Mission.MessageLimit;
			_items = new List<Message>(_itemLimit);
		}

		/// <summary>Creates a new Collection with multiple initial Messages</summary>
		/// <param name="quantity">Number of Messages to start with</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>quantity</i> is less than 0 or greater than <see cref="Idmr.Common.ResizableCollection{T}.ItemLimit"/></exception>
		public MessageCollection(int quantity)
		{
			_itemLimit = Mission.MessageLimit;
			if (quantity < 0 || quantity > _itemLimit) throw new ArgumentOutOfRangeException("quantity", "Invalid quantity, must be 0-" + _itemLimit);
			_items = new List<Message>(_itemLimit);
			for (int i = 0; i < quantity; i++) _items.Add(new Message());
		}

		/// <summary>Adds a new Message to the end of the Collection</summary>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Add()
		{
			int result = _add(new Message());
			if (result != -1 && !_isLoading) _isModified = true;
			return result;
		}

		/// <summary>Adds a new Message with the given string to the end of the Collection</summary>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Add(string message)
		{
			int index = Add();
			if (index != -1)
			{
				this[index].MessageString = message;
				if (!_isLoading) _isModified = true;
			}
			return index;
		}

		/// <summary>Inserts a new Message at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Insert(int index)
		{
			int result = _insert(index, new Message());
			if (result != -1) _isModified = true;
			return result;
		}

		/// <summary>Inserts a new Message with the given string at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Insert(int index, string message)
		{
			int newIndex = Insert(index);
			if (newIndex != -1)
			{
				this[newIndex].MessageString = message;
				_isModified = true;
			}
			return newIndex;
		}

		/// <summary>Deletes the Message at the specified index</summary>
		/// <param name="index">The index of the Message to be deleted</param>
		/// <returns>The index of the next available Message if successfull, otherwise <b>-1</b></returns>
		public int RemoveAt(int index)
		{
			int result = -1;
			if (index >= 0 && index < Count && Count > 1) { result = _removeAt(index); }
			else if (index == 0 && Count == 1)
			{
				Clear();
				result = 0;
			}
			if (result != -1) _isModified = true;
			return result;
		}

		/// <summary>Expands or contracts the Collection, populating as necessary</summary>
		/// <param name="value">The new size of the Collection. Must not be negative.</param>
		/// <param name="allowTruncate">Controls if the Collection is allowed to get smaller</param>
		/// <exception cref="InvalidOperationException"><i>value</i> is smaller than <see cref="Count"/> and <i>allowTruncate</i> is <b>false</b>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><i>value</i> must not be negative.</exception>
		/// <remarks>If the Collection expands, the new items will be a blank <see cref="Message"/>. When truncating, items will be removed starting from the last index.</remarks>
		public override void SetCount(int value, bool allowTruncate)
		{
			if (value == Count) return;
			else if (value < 0 || value > _itemLimit) throw new ArgumentOutOfRangeException("value", "Invalid quantity, must be 0-" + _itemLimit);
			else if (value < Count)
			{
				if (!allowTruncate) throw new InvalidOperationException("Reducing 'value' will cause data loss");
				if (Count == 0) Clear();
				else while (Count > value) RemoveAt(Count - 1);
			}
			else while (Count < value) Add();
		}
	}
}