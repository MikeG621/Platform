/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG
 * 120223 - added EventParameterCount
 */

using System;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for Briefings</summary>
	/// <remarks>Contains values that are shared among all briefing types. Class is Serializable to allow copy/paste functionality.</remarks>
	public abstract class BaseBriefing
	{
		protected byte[] _events;
		protected string[] _briefingTags;
		protected string[] _briefingStrings;
        protected MissionFile.Platform _platform;
		protected EventParameters _eventParameters;

		/// <summary>Known briefing event types</summary>
		public enum EventType : byte { None, PageBreak = 3, TitleText, CaptionText, MoveMap, ZoomMap, ClearFGTags, FGTag1, FGTag2, FGTag3, FGTag4, FGTag5, 
			FGTag6, FGTag7, FGTag8, ClearTextTags, TextTag1, TextTag2, TextTag3, TextTag4, TextTag5, TextTag6, TextTag7, TextTag8,
			XwaNewIcon, XwaShipInfo, XwaMoveIcon, XwaRotateIcon, XwaChangeRegion, EndBriefing = 0x22};
		
		/// <summary>Gets the Text Tags to be used on the map</summary>
		/// <remarks>Length is determined by the derivative class</remarks>
		public string[] BriefingTag { get { return _briefingTags; } }
		/// <summary>Gets the Title and Caption strings</summary>
		/// <remarks>Length is determined by the derivative class</remarks>
		public string[] BriefingString { get { return _briefingStrings; } }
		/// <summary>Gets the raw briefing event data</summary>
		/// <remarks>Although Events is a byte array, briefing values are actually shorts. Length is determined by the derivative class</remarks>
		public byte[] Events { get { return _events; } }
		/// <summary>Gets the number of int16 values in Events[]</summary>
		public short EventsLength
		{
			get
			{
				int i;
				for (i = 0; i < (_events.Length - 3); i += 2) if (BitConverter.ToInt16(_events, i) == 9999 && _events[i + 2] == (int)EventType.EndBriefing) break;	// find STOP @ 9999
				return (short)(i/2 + 2);
			}
		}
		/// <summary>Gets or sets the duration of the Briefing, in Ticks</summary>
		/// <remarks>The X-wing series uses Ticks instead of seconds, TicksPerSecond is defined by the derivative class</remarks>
		public short Length { get; set; }
		/// <summary>Gets the number of int16 values in Events[] at time = 0</summary>
		public short StartLength
		{
			get
			{
				short evnt = 0;
				short offset;
				for(offset=0; offset<(_events.Length-3); offset+=4)
				{
					if (BitConverter.ToInt16(_events, offset) != 0) break;
					offset += (short)(4 + _eventParameters[BitConverter.ToInt16(_events, offset + 2)]);
				}
				return (short)(offset/2);
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
			public byte this[int eventType] { get { return _counts[eventType]; } }
			
			/// <summary>Gets a parameter count</summary>
			/// <param name="eventType">The briefing event</param>
			public byte this[EventType eventType] { get { return _counts[(int)eventType]; } }
		}
	}
}
