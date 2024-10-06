/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [NEW] created
 */

using Idmr.Common;
using System;
using System.Collections.Generic;

namespace Idmr.Platform
{
	public partial class BaseBriefing
	{
		/// <summary>Container for all Briefing events.</summary>
		public class EventCollection : ResizableCollection<Event>
		{
			readonly int _ticksPerSecond = Tie.Briefing.TicksPerSecond;
			
			/// <summary>Initializes a new collection.</summary>
			/// <param name="platform">The platform the collection is used for.</param>
			/// <remarks>Not used for X-wing.</remarks>
			public EventCollection(MissionFile.Platform platform)
			{
				if (platform == MissionFile.Platform.XWA) _itemLimit = Xwa.Briefing.EventQuantityLimit;
				else _itemLimit = Tie.Briefing.EventQuantityLimit;
				if (platform == MissionFile.Platform.XvT || platform == MissionFile.Platform.BoP) _ticksPerSecond = Xvt.Briefing.TicksPerSecond;
				else if (platform == MissionFile.Platform.XWA) _ticksPerSecond = Xwa.Briefing.TicksPerSecond;
				_items = new List<Event>(_itemLimit)
				{
					new Event(EventType.EndBriefing) { _parent = this }
				};
			}
			/// <summary>Initializes a new collection from raw data.</summary>
			/// <param name="platform">The platform the collection is used for.</param>
			/// <param name="raw">The raw byte data read from file.</param>
			/// <remarks>Not used for X-wing.</remarks>
			public EventCollection(MissionFile.Platform platform, byte[] raw) : this(platform)
			{
				short[] shorts = new short[_itemLimit * 2];
				Buffer.BlockCopy(raw, 0, shorts, 0, raw.Length);

				for (int i = 0, offset = 0; i < _itemLimit; i++)
				{
					Add(new Event(shorts, offset) { _parent = this });
					offset += _items[i].Length;
					
					if (_items[i].IsEndEvent) break;
				}
			}

			/// <summary>Populate or truncate the collection.</summary>
			/// <param name="value">The new size of the Collection.</param>
			/// <param name="allowTruncate">Controls if the Collection is allowed to get smaller.</param>
			/// <remarks>Adds blank events or removes the event before <see cref="EventType.EndBriefing"/>.</remarks>
			public override void SetCount(int value, bool allowTruncate)
			{
				if (value == _items.Count) return;
				if (value < _items.Count && !allowTruncate) return;
				if (value < 0) return;

				int err = 0;
				if (value > _items.Count) for (int i = _items.Count; i < value; i++) err = Add(new Event());
				else if (value > 1) for (int i = _items.Count; i > value; i--) err = RemoveAt(_items.Count - 2);
				else Clear();	// fires for 0 or 1
				if (err == -1)
				{
					// Add/Remove failed
				}
			}

			/// <summary>Adds the given item to the end of the Collection before <see cref="EventType.EndBriefing"/>.</summary>
			/// <param name="item">The item to be added.</param>
			/// <returns>The index of the added item if successfull, otherwise <b>-1</b>.</returns>
			/// <remarks>Cannot add if it causes <see cref="EventType.EndBriefing"/> to overflow.<br/>
			/// Cannot add <see cref="EventType.EndBriefing"/> events, automatically managed.</remarks>
			public override int Add(Event item) => Insert(_items.Count - 1, item);

			/// <summary>Inserts the event at the specified index.</summary>
			/// <param name="index">Location of the event in the collection.</param>
			/// <param name="item">The event to be added.</param>
			/// <returns>The index of the added item if successfull, otherwise <b>-1</b>.</returns>
			/// <remarks>Cannot insert if it causes <see cref="EventType.EndBriefing"/> to overflow.<br/>
			/// Cannot add <see cref="EventType.EndBriefing"/> events, automatically managed.<br/>
			/// If the new <see cref="Event.Time"/> is after the one it's inserting before, will use the existing's time. If it's before the previous' time, will use that.<br/>
			/// If the new time is <b>9999</b>, will use the previous event's time. If there is no previous event, time will be <b>zero</b>.</remarks>
			public override int Insert(int index, Event item)
			{
				if (!HasSpaceForEvent(item)) return -1;
				if (index < 0 || index  >= _items.Count) return -1;
				if (item.Type == EventType.EndBriefing) return -1;

				if (item.Time > _items[index].Time) item.Time = _items[index].Time;
				else if (index > 0 && item.Time < _items[index - 1].Time) item.Time = _items[index - 1].Time;
				if (item.Time == 9999) item.Time = (short)(index > 0 ? _items[index - 1].Time : 0);

				item._parent = this;
				return base.Insert(index, item);
			}

			/// <summary>Remove all events except <see cref="EventType.EndBriefing"/>.</summary>
			public override void Clear()
			{
				base.Clear();
				_items.Add(new Event(EventType.EndBriefing) { _parent = this });
			}

			/// <summary>Remove the Event at the specified index.</summary>
			/// <param name="index">Location of the event in the collection.</param>
			/// <returns>Index of the next item after deletion.<br/>
			/// <b>-or-</b>
			/// <br/><b>-1</b> for invalid <paramref name="index"/> value.</returns>
			/// <remarks>Cannot remove <see cref="EventType.EndBriefing"/>.</remarks>
			public int RemoveAt(int index)
			{
				if (index ==  _items.Count - 1) return -1;

				return _removeAt(index);
			}

			/// <summary>Gets or sets a single event within the Collection.</summary>
			/// <param name="index">Location of the event in the collection.</param>
			/// <returns>A single event within the collection.<br/>
			/// -or-<br/>
			/// <b>null</b> for invalid values of <paramref name="index"/>.</returns>
			/// <remarks>No action is taken when attempting to set with invalid values of <paramref name="index"/> or <see cref="EventType.EndBriefing"/> would overflow.</remarks>
			public new Event this[int index]
			{
				get => base[index];
				set
				{
					if (Length - _items[index].Length + value.Length > _itemLimit * 2) return;	// should throw instead?

					value._parent = this;
					base[index] = value;
				}
			}

			/// <summary>Get the total number of <i>shorts</i> occupied by the events.</summary>
			public short Length
			{
				get
				{
					short len = 0;
					for (int i = 0; i < _items.Count; i++) len += _items[i].Length;
					return len;
				}
			}

			/// <summary>Gets whether or not the given event type can be added.</summary>
			/// <param name="type">Type of event.</param>
			/// <returns><b>true</b> if the event type can be added without overflowing <see cref="EventType.EndBriefing"/>.</returns>
			public bool HasSpaceForEvent(EventType type) => Length + EventParameters.GetCount(type) + 2 <= _itemLimit * 2;
			/// <summary>Gets whether or not the given event can be added.</summary>
			/// <param name="evt">The event.</param>
			/// <returns><b>true</b> if the event can be added without overflowing <see cref="EventType.EndBriefing"/>.</returns>
			public bool HasSpaceForEvent(Event evt) => Length + evt.Length <= _itemLimit * 2;

			/// <summary>Converts the event's time into seconds.</summary>
			/// <param name="evt">Briefing event.</param>
			/// <returns>The time per the platform-specific tick rate.</returns>
			public float GetTimeInSeconds(Event evt) => (float)evt.Time / _ticksPerSecond;

			/// <summary>Get the array equivalent of the collection.</summary>
			/// <returns>An array of raw values.</returns>
			public short[] GetArray()
			{
				short[] shorts = new short[Length];
				int pos = 0;
				for (int i = 0; i < _items.Count; i++)
				{
					Buffer.BlockCopy(_items[i].GetArray(), 0, shorts, pos, _items[i].Length * 2);
					pos += _items[i].Length * 2;
				}
				return shorts;
			}
		}

		/// <summary>Object for single briefing event.</summary>
		public class Event
		{
			EventType _type;
			internal EventCollection _parent;

			/// <summary>Initialize a blank event.</summary>
			public Event() { }
			/// <summary>Initialize a new event.</summary>
			/// <param name="type">Event type.</param>
			/// <remarks>If creating an <see cref="EventType.EndBriefing"/>, <see cref="Time"/> is set to <b>9999</b>.</remarks>
			public Event(EventType type)
			{
				Type = type;
				if (type == EventType.EndBriefing) Time = 9999;
			}
			/// <summary>Creates an event from raw data.</summary>
			/// <param name="raw">The raw array.</param>
			/// <exception cref="ArgumentException"><paramref name="raw"/> is not long enough.</exception>
			public Event(short[] raw) : this(raw, 0) { }
			/// <summary>Creates an event from raw data.</summary>
			/// <param name="raw">The raw array.</param>
			/// <param name="offset">The starting location in <paramref name="raw"/>.</param>
			/// <exception cref="ArgumentException"><paramref name="raw"/> is not long enough.</exception>
			/// <exception cref="IndexOutOfRangeException"><paramref name="offset"/> is not within <paramref name="raw"/>.</exception>
			public Event(short[] raw, int offset)
			{
                if (offset < 0 || offset > raw.Length - 2) throw new IndexOutOfRangeException("offset must be within the raw data");
                if (raw.Length < 2) throw new ArgumentException("raw must have Time and Type.", "raw");

				Time = raw[offset];
				Type = (EventType)raw[offset + 1];
				if (raw.Length - offset < Length) throw new ArgumentException("raw not long enough for " + Enum.GetName(typeof(EventType), Type) + " event.", "raw");

				for (int i = 2; i < Length; i++) Variables[i - 2] = raw[i + offset];
			}

			/// <summary>Gets or sets the time in Ticks.</summary>
			public short Time { get; set; }
			/// <summary>Gets or sets the event type.</summary>
			/// <remarks>Resizes <see cref="Variables"/> accordingly, copying values if present.</remarks>
			/// <exception cref="OverflowException">Changing the type would cause the End marker to overflow raw array.</exception>
			public EventType Type
			{
				get => _type;
				set
				{
					if (_parent != null && _parent.Length - EventParameters.GetCount(_type) + EventParameters.GetCount(value) > _parent.ItemLimit * 2)
						throw new OverflowException("Cannot change type, End marker would overflow.");

					_type = value;
					var temp = Variables;
					Variables = new short[EventParameters.GetCount(_type)];
					for (int i = 0; i < temp.Length && i < Variables.Length; i++) Variables[i] = temp[i];
				}
			}
			/// <summary>Gets the variables for the event.</summary>
			/// <remarks>Length is per <see cref="EventParameters"/>.</remarks>
			public short[] Variables { get; internal set; } = new short[0];

			/// <summary>Returns <b>true</b> if <see cref="Type"/> is <see cref="EventType.EndBriefing"/> or <see cref="EventType.None"/>.</summary>
			public bool IsEndEvent => _type == EventType.EndBriefing || _type == EventType.None;
			/// <summary>Returns <b>true</b> if <see cref="Type"/> is within <see cref="EventType.FGTag1"/> and <see cref="EventType.FGTag8"/>.</summary>
			public bool IsFGTag => (int)_type >= (int)EventType.FGTag1 && (int)_type <= (int)EventType.FGTag8;
			/// <summary>Returns <b>true</b> if <see cref="Type"/> is within <see cref="EventType.TextTag1"/> and <see cref="EventType.TextTag8"/>.</summary>
			public bool IsTextTag => (int)_type >= (int)EventType.TextTag1 && (int)_type <= (int)EventType.TextTag8;

			/// <summary>Gets the total number of <i>shorts</i> in the raw data.</summary>
			public short Length { get => (short)(2 + Variables.Length); }

			/// <summary>Get the array equivalent of the event.</summary>
			/// <returns>An array of raw values.</returns>
			public short[] GetArray()
			{
				var array = new short[Length];
				array[0] = Time;
				array[1] = (short)Type;
				for (int i = 2; i < Length; i++) array[i] = Variables[i - 2];
				return array;
			}

			/// <summary>Gets a copy of the event.</summary>
			/// <returns>A new copy of the event.</returns>
			public Event Clone() => new Event(GetArray());
		}
	}
}