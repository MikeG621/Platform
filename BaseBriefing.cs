/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1+
 */

/* CHANGELOG
 * [UPD] EventParameterCount changed to function, made virtual [JB]
 * [NEW] virtual helper functions [JB]
 * v2.1, 141214
 * [UPD] change to MPL
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
		virtual public byte EventParameterCount(int eventType) { return _eventParameters[eventType]; }

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

		/// <summary>Gets if the specified event denotes the end of the briefing.</summary>
		/// <param name="evt">The event index</param>
		/// <returns><b>true</b> if <i>evt</i> is <see cref="EventType.EndBriefing"/> or <see cref="EventType.None"/>.</returns>
        public virtual bool IsEndEvent(int evt)
        {
            return (evt == (int)EventType.EndBriefing || evt == (int)EventType.None);
        }
		/// <summary>Gets if the specified event denotes one of the FlightGroup Tag events.</summary>
		/// <param name="evt">The event index</param>
		/// <returns><b>true</b> if <i>evt</i> is <see cref="EventType.FGTag1"/> through <see cref="EventType.FGTag8"/>.</returns>
        public virtual bool IsFGTag(int evt)
        {
            return (evt >= (int)EventType.FGTag1 && evt <= (int)EventType.FGTag8);
        }
		/// <summary>Adjust FG references as necessary.</summary>
		/// <param name="fgIndex">Original index</param>
		/// <param name="newIndex">Replacement index</param>
        public virtual void TransformFGReferences(int fgIndex, int newIndex)
        {
            bool deleteCommand = false;
            if (newIndex < 0)
                deleteCommand = true;

            int p = 0, advance = 0;
            int paramCount = 0;
            while (p < EventsLength)
            {
                int evt = Events[p + 1];
                if (IsEndEvent(evt))
                    break;

                advance = 2 + EventParameterCount(evt);
                if (IsFGTag(evt))
                {
                    if (Events[p + 2] == fgIndex)
                    {
                        if (deleteCommand == false)
                        {
                            Events[p + 2] = (short)newIndex;
                        }
                        else
                        {
                            int len = EventsLength; //get() walks the event list, so cache the current value as the modifications will temporarily corrupt it
                            paramCount = 2 + EventParameterCount(evt);
                            for (int i = p; i < len - paramCount; i++)
                                Events[i] = Events[i + paramCount];  //Drop everything down
                            for (int i = len - paramCount; i < len; i++)
                                Events[i] = 0;  //Erase the final space
                            advance = 0;
                        }
                    }
                    else if (Events[p + 2] > fgIndex && deleteCommand == true)
                    {
                        Events[p + 2]--;
                    }
                }
                p += advance;
            }
        }
		/// <summary>Swap FG indexes</summary>
		/// <param name="srcIndex">First index</param>
		/// <param name="dstIndex">Second index</param>
		/// <remarks>Calls <see cref="TransformFGReferences(int, int)"/> with an interim value.</remarks>
        public virtual void SwapFGReferences(int srcIndex, int dstIndex)
        {
            TransformFGReferences(dstIndex, 255);
            TransformFGReferences(srcIndex, dstIndex);
            TransformFGReferences(255, srcIndex);
        }
    }
}
