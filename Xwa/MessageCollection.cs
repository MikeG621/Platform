/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 */

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xwa
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
		/// <exception cref="ArgumentOutOfRangeException"><i>quantity</i> is less than <b>0</b> or greater than <see cref="Idmr.Common.ResizableCollection{T}.ItemLimit"/></exception>
		public MessageCollection(int quantity)
		{
			_itemLimit = Mission.MessageLimit;
			if (quantity < 0 || quantity > _itemLimit) throw new ArgumentOutOfRangeException("quantity", "Invalid quantity, must be 0-" + _itemLimit);
			_items = new List<Message>(_itemLimit);
			for (int i = 0; i < quantity; i++) _items.Add(new Message());
		}

		/// <summary>Adds a new Message to the end of the Collection</summary>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Add() { return _add(new Message()); }

		/// <summary>Adds a new Message with the given string to the end of the Collection</summary>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Add(string message)
		{
			int index = Add();
			if (index != -1) this[index].MessageString = message;
			return index;
		}

		/// <summary>Empties the Collection of entries</summary>
		/// <remarks>All existing messages are lost, <see cref="Idmr.Common.FixedSizeCollection{T}.Count"/> is set to <b>zero</b></remarks>
		public void Clear() { _items.Clear(); }

		/// <summary>Inserts a new Message at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Insert(int index) { return _insert(index, new Message()); }

		/// <summary>Inserts a new Message with the given string at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise <b>-1</b></returns>
		public int Insert(int index, string message)
		{
			int newIndex = Insert(index);
			if (newIndex != -1) this[newIndex].MessageString = message;
			return newIndex;
		}

		/// <summary>Deletes the Message at the specified index</summary>
		/// <remarks>If first and only Message is specified, executes <see cref="Clear()"/></remarks>
		/// <param name="index">The index of the Message to be deleted</param>
		/// <returns>The index of the next available Message if successfull, otherwise <b>-1</b></returns>
		public int RemoveAt(int index)
		{
			if (index >= 0 && index < Count && Count > 1) { return _removeAt(index); }
			else if (index == 0 && Count == 1) Clear();
			return -1;
		}
	}
}