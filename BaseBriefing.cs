/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0.1, 120814
 * [FIX] StartEvents calculation
 * v2.0, 120525
 * [UPD] Events changed to short[]
 * [NEW] EventParameterCount
 */

using System;

namespace Idmr.Platform
{
	/// <summary>Base class for Briefings</summary>
	/// <remarks>Contains values that are shared among all briefing types. Class is Serializable to allow copy/paste functionality.</remarks>
	[Serializable]
	public abstract class BaseBriefing
	{
		/// <summary>The raw event data</summary>
		protected short[] _events;
		/// <summary>The strings placed on the map</summary>
		protected string[] _briefingTags;
		/// <summary>The captions and titles</summary>
		protected string[] _briefingStrings;
		/// <summary>The briefing format</summary>
        protected MissionFile.Platform _platform;
		/// <summary>The number of parameters per event type</summary>
		protected EventParameters _eventParameters;

		/// <summary>Known briefing event types</summary>
		public enum EventType : byte {
			/// <summary>No type defined</summary>
			None,
			/// <summary>Clears Title and Caption text, creates breakpoint for briefing interface <b>Next</b> command</summary>
			PageBreak = 3,
			/// <summary>Displays the specified text at the top of the briefing</summary>
			TitleText,
			/// <summary>Displays the specified text at the bottom of the briefing</summary>
			CaptionText,
			/// <summary>Change the focal point of the map</summary>
			MoveMap,
			/// <summary>Change the zoom distance of the map</summary>
			ZoomMap,
			/// <summary>Erase all FlightGroup tags from view</summary>
			ClearFGTags,
			/// <summary>Apply a FlightGroup tag using slot 1</summary>
			FGTag1,
			/// <summary>Apply a FlightGroup tag using slot 2</summary>
			FGTag2,
			/// <summary>Apply a FlightGroup tag using slot 3</summary>
			FGTag3,
			/// <summary>Apply a FlightGroup tag using slot 4</summary>
			FGTag4,
			/// <summary>Apply a FlightGroup tag using slot 5</summary>
			FGTag5,
			/// <summary>Apply a FlightGroup tag using slot 6</summary>
			FGTag6,
			/// <summary>Apply a FlightGroup tag using slot 7</summary>
			FGTag7,
			/// <summary>Apply a FlightGroup tag using slot 8</summary>
			FGTag8,
			/// <summary>Erase all text tags from view</summary>
			ClearTextTags,
			/// <summary>Apply a text tag using slot 1</summary>
			TextTag1,
			/// <summary>Apply a text tag using slot 2</summary>
			TextTag2,
			/// <summary>Apply a text tag using slot 3</summary>
			TextTag3,
			/// <summary>Apply a text tag using slot 4</summary>
			TextTag4,
			/// <summary>Apply a text tag using slot 5</summary>
			TextTag5,
			/// <summary>Apply a text tag using slot 6</summary>
			TextTag6,
			/// <summary>Apply a text tag using slot 7</summary>
			TextTag7,
			/// <summary>Apply a text tag using slot 8</summary>
			TextTag8,
			/// <summary>Create a new ship icon (XWA only)</summary>
			XwaNewIcon,
			/// <summary>Display craft type information (XWA only)</summary>
			XwaShipInfo,
			/// <summary>Move a ship icon (XWA only)</summary>
			XwaMoveIcon,
			/// <summary>Rotate a ship icon (XWA only)</summary>
			XwaRotateIcon,
			/// <summary>Display the region name and transition (XWA only)</summary>
			XwaChangeRegion,
			/// <summary>End of briefing marker</summary>
			EndBriefing = 0x22};
		
		/// <summary>Gets the Text Tags to be used on the map</summary>
		/// <remarks>Length is determined by the derivative class</remarks>
		public string[] BriefingTag { get { return _briefingTags; } }
		/// <summary>Gets the Title and Caption strings</summary>
		/// <remarks>Length is determined by the derivative class</remarks>
		public string[] BriefingString { get { return _briefingStrings; } }
		/// <summary>Gets the raw briefing event data</summary>
		/// <remarks>Length is determined by the derivative class</remarks>
		public short[] Events { get { return _events; } }
		/// <summary>Gets the number of values in <see cref="Events"/></summary>
		public short EventsLength
		{
			get
			{
				int i;
				for (i = 0; i < (_events.Length - 1); i++) if (_events[i] == 9999 && _events[i+1] == (int)EventType.EndBriefing) break;	// find STOP @ 9999
				return (short)(i + 2);
			}
		}
		/// <summary>Gets or sets the duration of the Briefing, in Ticks</summary>
		/// <remarks>The X-wing series uses Ticks instead of seconds, TicksPerSecond is defined by the derivative class</remarks>
		public short Length { get; set; }
		/// <summary>Gets the number of int16 values in <see cref="Events"/> at time = 0</summary>
		public short StartLength
		{
			get
			{
				short offset;
				for (offset = 0; offset < (_events.Length - 1); offset += 2)
				{
					if (_events[offset] != 0) break;
					offset +=  _eventParameters[_events[offset + 1]];
				}
				return offset;
			}
		}
		/// <summary>Gets or sets the unknown value</summary>
		/// <remarks>Briefing offset 0x02, between Length and StartLength</remarks>
		public short Unknown1 { get; set; }

		/// <summary>Gets the array that contains the number of parameters per event type</summary>
		public EventParameters EventParameterCount { get { return _eventParameters; } }

		/// <summary>Object to maintain a read-only array</summary>
		public class EventParameters
		{
			byte[] _counts = { 0, 0, 0, 0, 1, 1, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 4, 4, 4, 4, 4, 4, 4, 4, 3, 2, 3, 2, 1, 0, 0, 0, 0 };
			
			/// <summary>Gets a parameter count</summary>
			/// <param name="eventType">The briefing event</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>eventType</i> value</exception>
			public byte this[int eventType] { get { return _counts[eventType]; } }
			
			/// <summary>Gets a parameter count</summary>
			/// <param name="eventType">The briefing event</param>
			public byte this[EventType eventType] { get { return _counts[(int)eventType]; } }
		}
	}
}
