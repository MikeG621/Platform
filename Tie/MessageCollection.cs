﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object to maintain mission in-flight messages</summary>
	/// <remarks>ItemLimit is set to Mission.MessageLimit (16)</remarks>
	public class MessageCollection : Idmr.Common.ResizableCollection<Message>
	{
		/// <summary>Creates a new empty Collection</summary>
		public MessageCollection()
		{
			_itemLimit = Mission.MessageLimit;
		}

		/// <summary>Creates a new Collection with multiple initial Messages</summary>
		/// <param name="quantity">Number of Messages to start with</param>
		public MessageCollection(int quantity)
		{
			_itemLimit = Mission.MessageLimit;
			if (quantity <= _itemLimit && quantity > 0) _count = quantity;
			else if (quantity < 0) _count = 1;
			else if (quantity == 0) return;
			else _count = _itemLimit;
			_items = new Message[_count];
			for (int i=0;i<_count;i++) _items[i] = new Message();
		}

		/// <summary>Add a new Message to the end of the Collection</summary>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Add() { return _add(new Message()); }

		/// <summary>Add a new Message with the given string to the end of the Collection</summary>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Add(string message)
		{
			int index = Add();
			if (index != -1) this[index].MessageString = message;
			return index;
		}

		/// <summary>Empties the Collection of entries</summary>
		/// <remarks>All existing messages are lost, <i>Count</i> is set to zero</remarks>
		public void Clear()
		{
			_count = 0;
			_items = null;
		}

		/// <summary>Inserts a new Message at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Insert(int index) { return _insert(index, new Message()); }

		/// <summary>Inserts a new Message with the given string at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <param name="message">The message string</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Insert(int index, string message)
		{
			int newIndex = Insert(index);
			if (newIndex != -1) this[newIndex].MessageString = message;
			return newIndex;
		}

		/// <summary>Deletes the Message at the specified index</summary>
		/// <remarks>If first and only Message is specified, executes Clear()</remarks>
		/// <param name="index">The index of the Message to be deleted</param>
		/// <returns>The index of the next available Message if successfull, otherwise -1</returns>
		public int RemoveAt(int index)
		{
			if (index >= 0 && index < _count && _count > 1) { return _removeAt(index); }
			else if (index == 0 && _count == 1) Clear();
			return -1;
		}
	}
}