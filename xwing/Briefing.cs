/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.5+
 */

/* CHANGELOG
* created [JB]
*/

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xwing
{

    //Thanks to the XWVM team for providing documentation on this block.
    public class BriefingUIItem
    {
        public short top;
        public short left;
        public short bottom;
        public short right;
        public short visible;
        public bool IsVisible()
        {
            return (visible != 0);
        }
    }
    public class BriefingUIPage
    {
        public BriefingUIItem[] items;
        public enum Elements
        {
            Title = 0,
            Text,
            Unused1,
            Unused2,
            Map
        }
        public BriefingUIPage()
        {
            items = new BriefingUIItem[5];
            for(int i = 0; i < 5; i++)
                items[i] = new BriefingUIItem();
        
        }
        public BriefingUIItem GetElement(Elements item)
        {
            return items[(int)item];
        }
        public string GetPageDesc()
        {
            return (items[(int)Elements.Map].visible == 1) ? "Map" : "Text";
        }
        public void SetDefaultsToMapPage()
        {
            short[,] defData = { {0, 0, 12, 212, 1},
                                 {115, 0, 138, 212, 1},
                                 {0, 0, 0, 0, 0},
                                 {0, 0, 0, 0, 0}, 
                                 {12, 0, 115, 212, 1} };
            SetRawData(defData);
        }
        public void SetDefaultsToTextPage()
        {
            short[,] defData = { {0, 0, 12, 212, 1}, 
                                 {12, 0, 138, 212, 1}, 
                                 {0, 0, 0, 0, 0}, 
                                 {0, 0, 0, 0, 0}, 
                                 {12, 0, 114, 212, 0} };
            SetRawData(defData);
        }
        private void SetRawData(short[,] data)
        {
            if (data.GetLength(0) < 5 || data.GetLength(0) < 5)
                throw new Exception("Not enough data elements.");
            for (int i = 0; i < 5; i++)
            {
                BriefingUIItem item = items[i];
                item.top = data[i, 0];
                item.left = data[i, 1];
                item.bottom = data[i, 2];
                item.right = data[i, 3];
                item.visible = data[i, 4];
            }
        }
    }

    [Serializable]
    public class BriefingPage
    {
        public short[] _events;
        public short Length = 0;
        public short EventsLength = 0;
        public short CoordSet = 0;
        public short PageType = 0;

        public void SetDefaultFirstPage()
        {
            CoordSet = 1;
            Length = 0x21C;	//default 45 seconds
			_events[1] = (short)15;  //Center map
			_events[5] = (short)16;  //Move
			_events[6] = 0x30;  
			_events[7] = 0x30;
			_events[8] = 9999;
			_events[9] = (short)41;  //End marker
        }
        public BriefingPage()
        {
            _events = new short[0x190];
        }
        public short[] Events { get { return _events; } }
    }

    /// <summary>Briefing object for XWING95</summary>
    /// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48</remarks>
    public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is <b>8 (0x8)</b></remarks>
		public const int TicksPerSecond = 0x8;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is <b>200 (0xC8)</b></remarks>
		public const int EventQuantityLimit = 0xC8;

        public List<BriefingPage> pages;
        public List<BriefingUIPage> windowSettings;

        public short MaxCoordSet = 2;
        public short MissionLocation = 0;

		/// <summary>X-wing uses a very similar event set to TIE, but the IDs are different and some parameters too.</summary>
		new public enum EventType : byte {
			/// <summary>No type defined</summary>
			None = 0,
			/// <summary>Waits for the user to click the page to proceed.  Used to hide the special hints text.</summary>
			WaitForClick = 1,
			/// <summary>Clears both the title and caption text.</summary>
			ClearText = 10, 
			/// <summary>Displays the specified text at the top of the briefing (param:textID)</summary>
			TitleText = 11,
			/// <summary>Displays the specified text at the bottom of the briefing (param:textID)</summary>
			CaptionText = 12,
            /// <summary>Alternate command used in some briefings.  If at tick 700, it's an end marker.  (param:textID)</summary>
            CaptionText2 = 14,
            /// <summary>Change the focal point of the map (param:x,y)</summary>
			MoveMap = 15,
			/// <summary>Change the zoom distance of the map (param:xFactor,yFactor)</summary>
			ZoomMap = 16,
			/// <summary>Erase all FlightGroup tags from view</summary>
			ClearFGTags = 21,
			/// <summary>Apply a FlightGroup tag using slot 1 (param:objectID)</summary>
			FGTag1 = 22,
			/// <summary>Apply a FlightGroup tag using slot 2 (param:objectID)</summary>
			FGTag2 = 23,
			/// <summary>Apply a FlightGroup tag using slot 3 (param:objectID)</summary>
			FGTag3 = 24,
			/// <summary>Apply a FlightGroup tag using slot 4 (param:objectID)</summary>
			FGTag4 = 25,
			/// <summary>Erase all text tags from view</summary>
			ClearTextTags = 26,
			/// <summary>Apply a text tag using slot 1 (param:tagID,x,y)</summary>
			TextTag1 = 27,
			/// <summary>Apply a text tag using slot 2 (param:tagID,x,y)</summary>
			TextTag2 = 28,
			/// <summary>Apply a text tag using slot 3 (param:tagID,x,y)</summary>
			TextTag3 = 29,
			/// <summary>Apply a text tag using slot 4 (param:tagID,x,y)</summary>
			TextTag4 = 30,
			/// <summary>End of briefing marker</summary>
			EndBriefing = 41
		};
        private Dictionary<EventType, string> _eventTypeStringMap = new Dictionary<EventType, string> {
            {EventType.None, "None"},
            {EventType.WaitForClick, "Wait For Click"},
            {EventType.ClearText, "Clear Text"},
            {EventType.TitleText, "Title Text"},
            {EventType.CaptionText, "Caption Text"},
            {EventType.CaptionText2, "* Caption Text 2"},
            {EventType.MoveMap, "Move Map"},
            {EventType.ZoomMap, "Zoom Map"},
            {EventType.ClearFGTags, "Clear FG Tags"},
            {EventType.FGTag1, "FG Tag 1"},
            {EventType.FGTag2, "FG Tag 2"},
            {EventType.FGTag3, "FG Tag 3"},
            {EventType.FGTag4, "FG Tag 4"},
            {EventType.ClearTextTags, "Clear Text Tags"},
            {EventType.TextTag1, "Text Tag 1"},
            {EventType.TextTag2, "Text Tag 2"},
            {EventType.TextTag3, "Text Tag 3"},
            {EventType.TextTag4, "Text Tag 4"},
            {EventType.EndBriefing, "End Briefing"}
        };

		/// <summary>Returns the string name of a particular <see cref="EventType"/>.</summary>
		/// <param name="eventCommand">The event</param>
		/// <returns>The event type, otherwise <b>"Unknown (<i>eventCommand</i>)"</b>.</returns>
        public string GetEventTypeAsString(EventType eventCommand)
        {
            if(_eventTypeStringMap.ContainsKey(eventCommand))
                return _eventTypeStringMap[eventCommand];
            return "Unknown(" + eventCommand + ")";
        }
		/// <summary>Gets the possible <see cref="EventType">EventTypes</see>, excluding the first entry (None).</summary>
		/// <returns>An array of the possible values.</returns>
		public string[] GetUsableEventTypeStrings()
        {
            string[] ret = new string[_eventTypeStringMap.Count - 1];
            int pos = -1;
            foreach(KeyValuePair<EventType, string> dat in _eventTypeStringMap)
            {
                if (pos == -1)
                {
                    pos = 0;
                    continue;
                }
                ret[pos++] = dat.Value;
            }
            return ret;
        }
		/// <summary>Gets the index from the string value of the <see cref="EventType"/></summary>
		/// <param name="name">The event's string value.</param>
		/// <returns>The raw index value.</returns>
        public int GetEventTypeByName(string name)
        {
            foreach (KeyValuePair<EventType, string> dat in _eventTypeStringMap)
                if (dat.Value == name)
                    return (int)dat.Key;
            return (int)EventType.None;
        }

        public bool IsMapPage(int page)
        {
            if (page < 0 || page >= pages.Count) return false;
            if (pages[page].PageType >= 0 && pages[page].PageType < windowSettings.Count)
                return (windowSettings[pages[page].PageType].GetElement(BriefingUIPage.Elements.Map).visible == 1);
            return false;
        }

        /// <summary>Enumerates a list of caption strings found in a briefing page.</summary>
		/// <param name="page">The index of <see cref="pages"/></param>
		/// <param name="captionText">The collection of captions</param>
        /// <remarks>This is a helper function for use in converting missions.</remarks>
        public void GetCaptionText(int page, out List<string> captionText)
        {
            captionText = new List<string>();
            if (page < 0 || page >= pages.Count) return;

            BriefingPage pg = pages[page];
            int length = pg.EventsLength;
            int rpos = 0;
            while (rpos < length)
            {
                short[] xwevt = ReadBriefingEvent(page, rpos);
                rpos += xwevt.Length;
                if (xwevt[1] == (short)EventType.CaptionText)
                    captionText.Add(BriefingString[xwevt[2]]);
                else if (xwevt[1] == 0 || xwevt[1] == (short)EventType.EndBriefing)
                    break;
            }
        }

		/// <summary>Determines if the given caption text is a potential hint page.</summary>
		/// <param name="captionText">The text to check</param>
		/// <returns><b>true</b> if <i>captionText</i> contains ">STRATEGY AND TACTICS".</returns>
		/// <remarks>This is a helper function for use in converting missions.  Intended for use with strings extracted via GetCaptionText().</remarks>
		public bool ContainsHintText(string captionText)
        {
            return (captionText.ToUpper().IndexOf(">STRATEGY AND TACTICS") >= 0);
        }

        public enum PageType : short
        {
            Map = 0,
            Text = 1
        };
        //This data table assists in converting X-wing briefing events to TIE Fighter briefing events for use in conversion.
		//Disp:  Index in the briefing editor events dropdown list.
		//XWID:  Event ID in Xwing
		//TIEID: Event ID in TIE Fighter if it exists (zero for no TIE equivalent)
		//PARAMS: Event parameter count in X-wing
		//-- Credits to the XWVM team for providing documentation of these events.
		public short[] EventMapper = {  //19 events, 4 columns each
		// DISP  XWID  TIEID  PARAMS       NOTES
			 0,    0,    0,   0,
			 1,    1,    0,   0,  //01: Wait For Click. (No params)   --> none (Doesn't exist in TIE)
			 2,   10,  0x11,   0,  //10: Clear Texts Boxes (No params) --> Clear Text Tags
			 3,   11,  0x04,   1,  //11: Display Title (textId)        --> Title Text (textId)
			 4,   12,  0x05,   1,  //12: Display Main Text (textId)    --> Caption Text (textId)
			 5,   14,  0x05,   1,  //14: Display Main Text 2 (textId)  --> Caption Text (textId)
			 6,   15,  0x06,   2,  //15: Center Map (x, y)             --> Move Map (X,Y)
			 7,   16,  0x07,   2,  //16: Zoom Map (xFactor, yFactor)   --> Zoom Map (X,Y)
			 8,   21,  0x08,   0,  //21: Clear Highlights (No params)  --> Clear FG Tags
			 9,   22,  0x09,   1,  //22: Set highlight 1 (objectId)    --> FG Tag 1 (FGIndex)
			10,   23,  0x0A,   1,  //23: Set highlight 2 (objectId)    --> FG Tag 2 (FGIndex)
			11,   24,  0x0B,   1,  //24: Set highlight 3 (objectId)    --> FG Tag 3 (FGIndex)
			12,   25,  0x0C,   1,  //25: Set highlight 4 (objectId)    --> FG Tag 4 (FGIndex)
			13,   26,  0x11,   0,  //26: Clear Tags (No params)        --> Clear Text Tags
			14,   27,  0x12,   3,  //27: Create Tag 1 (tagId, x, y)    --> Text Tag 1 (tag, color, x, y)
			15,   28,  0x13,   3,  //28: Create Tag 2 (tagId, x, y)    --> Text Tag 2 (tag, color, x, y)
			16,   29,  0x14,   3,  //29: Create Tag 3 (tagId, x, y)    --> Text Tag 3 (tag, color, x, y)
			17,   30,  0x15,   3,  //30: Create Tag 4 (tagId, x, y)    --> Text Tag 4 (tag, color, x, y)
			18,   41,  0x22,   0   //41: End marker                    --> End Briefing
		};
		public int EventCount(short eventCommand)
		{
			//byte convertCommand = EventMap(eventCommand);
 			//if(convertCommand == 1) return 0;
			//if(convertCommand == 2) return 2;
			return EventParameterCount(eventCommand);
		}

        /// <summary>The number of parameters per event type</summary>
        new protected EventParameters _eventParameters;

        /// <summary>Gets the array that contains the number of parameters per event type.  This is specific to X-wing only.</summary>
        override public byte EventParameterCount(int eventType)
        {
            return _eventParameters[eventType];
        }

        /// <summary>Object to maintain a read-only array</summary>
        new public class EventParameters
        {
                              //0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41
            byte[] _counts = {  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            /// <summary>Gets a parameter count</summary>
            /// <param name="eventType">The briefing event</param>
            /// <exception cref="IndexOutOfRangeException">Invalid <i>eventType</i> value</exception>
            public byte this[int eventType] { get { return _counts[eventType]; } }

            /// <summary>Gets a parameter count</summary>
            /// <param name="eventType">The briefing event</param>
            public byte this[EventType eventType] { get { return _counts[(int)eventType]; } }
        }

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{	//initialize
            pages = new List<BriefingPage>();
            pages.Add(new BriefingPage());
            pages[0].SetDefaultFirstPage();

            windowSettings = new List<BriefingUIPage>();
            ResetUISettings(2);

			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.Xwing;
            _events = new short[0x190];
			Length = 0x21C;	//default 45 seconds
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			for (int i=0;i<0x20;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
		}

		/// <summary>Re-initialize <see cref="BaseBriefing.BriefingTag"/> to the given size.</summary>
		/// <param name="count">The new size, max <b>32</b></param>
		/// <remarks>All tags initialized as empty.</remarks>
		public void ResizeTagList(int count)
		{
			if(count < 32) count = 32;
			_briefingTags = new string[count];
			for(int i = 0; i < count; i++)
				_briefingTags[i] = "";
		}
		/// <summary>Re-initialize <see cref="BaseBriefing.BriefingString"/> to the given size.</summary>
		/// <param name="count">The new size, max <b>32</b></param>
		/// <remarks>All strings initialized as empty.</remarks>
		public void ResizeStringList(int count)
		{
			if(count < 32) count = 32;
			_briefingStrings = new string[count];
			for(int i = 0; i < count; i++)
				_briefingStrings[i] = "";
		}

		/// <summary>DO NOT USE. Will always throw an exception</summary>
		/// <exception cref="InvalidOperationException">Throws on any get attempt.</exception>
		new public short EventsLength
        {
            get
            {
                throw new InvalidOperationException("Warning! EventsLength is not used for X-wing briefings. If you see this message, please file a bug report.");
            }
        }

        public override void TransformFGReferences(int fgIndex, int newIndex)
        {
            bool deleteCommand = false;
            if (newIndex < 0)
                deleteCommand = true;

            for (int page = 0; page < pages.Count; page++)
            {
                BriefingPage pg = pages[page];
                int p = 0, advance = 0;
                int paramCount = 0;
                int briefLen = GetEventsLength(page);
                while (p < briefLen)
                {
                    int evt = pg.Events[p + 1];
                    if (IsEndEvent(evt))
                        break;

                    advance = 2 + EventParameterCount(evt);
                    if (IsFGTag(evt))
                    {
                        if (pg.Events[p + 2] == fgIndex)
                        {
                            if (deleteCommand == false)
                            {
                                pg.Events[p + 2] = (short)newIndex;
                            }
                            else
                            {
                                int len = GetEventsLength(page); //get() walks the event list, so cache the current value as the modifications will temporarily corrupt it
                                paramCount = 2 + EventParameterCount(evt);
                                for (int i = p; i < len - paramCount; i++)
                                    pg.Events[i] = pg.Events[i + paramCount];  //Drop everything down
                                for (int i = len - paramCount; i < len; i++)
                                    pg.Events[i] = 0;  //Erase the final space
                                advance = 0;
                            }
                        }
                        else if (pg.Events[p + 2] > fgIndex && deleteCommand == true)
                        {
                            pg.Events[p + 2]--;
                        }
                    }
                    p += advance;
                }
            }
        }

		public string TranslateHighlightToString(string text, byte[] highlight)
		{
			if(highlight.Length != text.Length)
				throw new Exception("String highlight data does not match string length.");

			string process = "";
			bool state = false;
			int start = 0;
			int end = 0;
			
			for (int i = 0; i < highlight.Length; i++)
			{
				if (highlight[i] == 1)
				{
					if (state == false)
					{
						state = true;
						start = i;
						process += text.Substring(end, start - end) + "["; //from the old end position
					}
				}
				else
				{
					if (state == true)
					{
						state = false;
						end = i;
						process += text.Substring(start, end - start) + "]";
					}
				}
			}
			if (state == true) //Didn't terminate?
				process += text.Substring(start, text.Length - start) + "]";
			else
				process += text.Substring(end, text.Length - end); //Last known end position

			return process;
		}
        public byte[] TranslateStringToHighlight(string text)
        {
            int len = text.Length;
            byte[] temp = new byte[len];
            char[] tchar = text.ToCharArray();
            bool state = false;
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                if (tchar[i] == '[')
                    state = true;
                else if (tchar[i] == ']')
                    state = false;
                else
                    temp[pos++] = (byte)(state ? 1 : 0);
            }
            byte[] ret = new byte[pos];
            Array.Copy(temp, ret, pos);
            return ret;
        }
        public string RemoveBrackets(string text)
        {
            return text.Replace("[", String.Empty).Replace("]", String.Empty);
        }

		private short GetEventMapperIndex(int eventID)
		{
			for(short i = 0; i < EventMapper.Length / 4; i++)
				if(EventMapper[(i*4)+1] == eventID)  //+1 for Column[1]
					return (short)(i*4);
			return 0;
		}

		/// <summary>Reads an briefing event and returns an array with all the information for that event.</summary>
		/// <param name="page">The index in <see cref="pages"/></param>
		/// <param name="rawOffset">The offset within <see cref="BriefingPage.Events"/></param>
		/// <remarks>The returned array contains exactly as many shorts as used by the event: time, event command, and variable length parameter field.</remarks>
        /// <returns>A short[] array of equal size to the exact resulting command length.</returns>
		public short[] ReadBriefingEvent(int page, int rawOffset)
		{
			short evtTime = pages[page]._events[rawOffset++];
            short evtCommand = pages[page]._events[rawOffset++];
			short mapperOffset = GetEventMapperIndex(evtCommand);
			int paramCount = EventMapper[mapperOffset+3]; //Column[3] is param counts
			short[] retEvent = new short[2 + paramCount];
			int writePos = 0;
			retEvent[writePos++] = evtTime;
			retEvent[writePos++] = evtCommand;
			for(int i = 0; i < paramCount; i++)
                retEvent[writePos++] = pages[page]._events[rawOffset++];
			return retEvent;
		}
		/// <summary>Takes an event from ReadBriefingEvent and translates it into a TIE95 compatible format, adjusting event IDs and parameter count as necessary.</summary>
		/// <remarks>TextTag Color is a placeholder and may need to be modified by the calling platform.
        /// XvT and XWA viewports are larger than XWING95 and TIE95.  Zoom Map event parameters may need to be scaled.
        /// XWA Move Map event parameters may need to be scaled.</remarks>
        /// <returns>A short[] array of equal size to the exact resulting command length.</returns>
		public short[] TranslateBriefingEvent(short[] xwingEvent)
		{
			short rpos = 0, wpos = 0;
			short evtTime = xwingEvent[rpos++];
			short evtCommand = xwingEvent[rpos++];
			short tieCommand = 0;
			short xwParams = 0;
			short tieParams = 0;
			Tie.Briefing.EventParameters TIEEventParameters = new Tie.Briefing.EventParameters();
			short mapperOffset = GetEventMapperIndex(evtCommand);
			tieCommand = EventMapper[mapperOffset + 2];
			tieParams = TIEEventParameters[tieCommand];
			xwParams = EventMapper[mapperOffset + 3];

			short[] retEvent = new short[2 + tieParams];
			retEvent[wpos++] = evtTime;
			retEvent[wpos++] = tieCommand;
			if(xwParams == tieParams)
			{
				for(int i = 0; i < xwParams; i++)
					retEvent[wpos++] = xwingEvent[rpos++];
			}
			else
			{
                if (evtCommand >= (int)EventType.TextTag1 && evtCommand <= (int)EventType.TextTag4)
                {
                    retEvent[wpos++] = xwingEvent[rpos++];  //tagId
                    retEvent[wpos++] = xwingEvent[rpos++];  //x
                    retEvent[wpos++] = xwingEvent[rpos++];  //y
                    retEvent[wpos++] = 0;                   //color (placeholder).  Acceptable values are platform-dependent and should be set in the converting function.
                }
                else throw new Exception("Unhandled instruction");
			}
			return retEvent;
		}

        public BriefingPage GetBriefingPage(int index)
        {
            if(index < 0 || index >= pages.Count) throw new Exception("Briefing page index out of range.");
            return pages[index];
        }
        public void ResetPages(int count)
        {
            if(count < 0)
                count = 1;
            pages = new List<BriefingPage>();
            for(int i = 0; i < count; i++)
            {
                pages.Add(new BriefingPage());
                if(i == 0)
                    pages[i].SetDefaultFirstPage();
            }
        }
        public int GetEventsLength(int page)
        {
            BriefingPage pg = GetBriefingPage(page);
            int pos = 0;
            while(pos < pg._events.Length)
            {
                pos++; //skip event time
                int evt = pg._events[pos++];
                if(evt == (int)EventType.None)
                    return pos - 2;
                else if(evt == (int)EventType.EndBriefing)
                    return pos;
                else
                    pos += EventParameterCount(evt);
            }
            return pos;
        }

        public void ResetUISettings(int pageTypeCount)
        {
            /*
            short[,] defData = { {0, 0, 12, 212, 1},
                                   {115, 0, 138, 212, 1},
                                   {0, 0, 0, 0, 0},
                                   {0, 0, 0, 0, 0}, 
                                   {12, 0, 115, 212, 1}, 
                                   {0, 0, 12, 212, 1}, 
                                   {12, 0, 138, 212, 1}, 
                                   {0, 0, 0, 0, 0}, 
                                   {0, 0, 0, 0, 0}, 
                                   {12, 0, 114, 212, 0} };
             * */

            //Default to 2 page types, Map and Text.
            if (pageTypeCount < 2)
                pageTypeCount = 2;
            windowSettings.Clear();
            for (int i = 0; i < pageTypeCount; i++)
                windowSettings.Add(new BriefingUIPage());

            windowSettings[0].SetDefaultsToMapPage();
            windowSettings[1].SetDefaultsToTextPage();

            /*
                if (i < 2) //Only the first two pages are filled with defaults.  If extra pages are requested, they are set to empty.
                {
                    for (int j = 0; j < 5; j++)
                    {
                        BriefingUIItem item = page.items[j];
                        item.top = defData[(i * 5) + j, 0];
                        item.left = defData[(i * 5) + j, 1];
                        item.bottom = defData[(i * 5) + j, 2];
                        item.right = defData[(i * 5) + j, 3];
                        item.visible = defData[(i * 5) + j, 4];
                    }
                }
                windowSettings.Add(page);
            }
             * */
        }

        public int FindStringList(string str)
        {
            for (int i = 0; i < 32; i++)
                if (_briefingStrings[i] == str)
                    return i;
            return -1;
        }
        public int GetOrCreateString(string str)
        {
            int index = FindStringList(str);
            if (index >= 0)
                return index;

            index = FindStringList("");
            if (index >= 0)
               _briefingStrings[index] = str;

           return index;
        }

        public override bool IsEndEvent(int evt)
        {
            return (evt == (int)EventType.EndBriefing || evt == (int)EventType.None);
        }
        public override bool IsFGTag(int evt)
        {
            return (evt >= (int)EventType.FGTag1 && evt <= (int)EventType.FGTag4);
        }
    }
}
