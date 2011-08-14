using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object to maintain mission in-flight messages</summary>
	/// <remarks>ItemLimit is set to Mission.MessageLimit (16)</remarks>
	public class MessageCollection : ResizableCollection
	{
		/// <summary>Create a new empty Collection</summary>
		public MessageCollection()
		{
			_itemLimit = Mission.MessageLimit;
		}

		/// <summary>Create a new Collection with multiple initial Messages</summary>
		/// <param name="quantity">Number of Messages to start with</param>
		public MessageCollection(int quantity)
		{
			_itemLimit = Mission.MessageLimit;
			if (quantity <= _itemLimit && quantity > 0) _count = quantity;
			else if (quantity <= 0) _count = 1;
			else _count = _itemLimit;
			_items = new Message[_count];
			for (int i=0;i<_count;i++) _items[i] = new Message();
		}

		/// <value>A single Message within the Collection</value>
		/// <example>MessageCollection Msgs = new MessageCollection(3);<br>
		/// Msgs[2].MessageString = "This is a message";<br>
		/// Msgs[1] = new Message();<br>
		/// Msgs[0].MessageString = Msgs[2].MessageString // Msgs[0].MessageString = "This is a message"</example>
		public Message this[int index]
		{
			get { return (Message)_getItem(index); }
			set { _setItem(index, value); }
		}

		/// <summary>Add a new Message to the end of the Collection</summary>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Add() { return _add(new Message()); }

		/// <summary>Adds the given Message to the end of the Collection</summary>
		/// <param name="message">The Message to be added</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Add(Message message) { return _add(message); }

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
		/// <remarks>All existing messages are lost, Count is set to zero</remarks>
		public void Clear()
		{
			_count = 0;
			_items = null;
		}

		/// <summary>Inserts a new Message at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Insert(int index) { return _insert(index, new Message()); }

		/// <summary>Inserts the given Message at the specified index</summary>
		/// <param name="index">Location of the Message</param>
		/// <param name="message">The Message to be added</param>
		/// <returns>The index of the added Message if successfull, otherwise -1</returns>
		public int Insert(int index, Message message) { return _insert(index, message); }

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