using System;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for Briefings</summary>
	/// <remarks>Contains values that are shared among all briefing types. Class is Serializable to allow copy/paste functionality.</remarks>
	public abstract class BaseBriefing
	{
		protected short _length;
		protected byte[] _events;
		protected short _unknown1 = 0;
		protected string[] _briefingTags;
		protected string[] _briefingStrings;
        protected MissionFile.Platform _platform;

		public enum EventType { None, PageBreak = 3, TitleText, CaptionText, MoveMap, ZoomMap, ClearFGTags, FGTag1, FGTag2, FGTag3, FGTag4, FGTag5, 
			FGTag6, FGTag7, FGTag8, ClearTextTags, TextTag1, TextTag2, TextTag3, TextTag4, TextTag5, TextTag6, TextTag7, TextTag8,
			XwaNewIcon, XwaShipInfo, XwaMoveIcon, XwaRotateIcon, XwaChangeRegion, EndBriefing = 0x22};
		
		/// <value>Text Tags to be used on the map</value>
		/// <remarks>Length is determined by derivative class</remarks>
		public string[] BriefingTag { get { return _briefingTags; } }
		/// <value>Title and Caption strings</value>
		/// <remarks>Length is determined by derivative class</remarks>
		public string[] BriefingString { get { return _briefingStrings; } }
		/// <value>Raw briefing event data</value>
		/// <remarks>Although Events is a byte array, briefing values are actually shorts. Length is determined by the derivative class</remarks>
		public byte[] Events { get { return _events; } }
		/// <value>Number of values in Events[]</value>
		/// <remarks>Gets the number of shorts (int16) that make up the raw event data</remarks>
		public short EventsLength
		{
			get
			{
				int i;
				for (i=0;i<(_events.Length-3);i++) if (_events[i]==0xF && _events[i+1]==0x27 && _events[i+2]==0x22) break;	// find STOP @ 9999
				return (short)(i/2 + 2);
			}
		}
		/// <value>Length of Briefing, in Ticks</value>
		/// <remarks>The X-wing series uses Ticks instead of seconds, TicksPerSecond is defined by the derivative class</remarks>
		public short Length
		{
			get { return _length; }
			set { _length = value; }
		}
		/// <value>Number of values in Events[] at time = 0</value>
		/// <remarks>Gets the number of shorts (int16) that are required at Time = 0</remarks>
		public short StartLength
		{
			get
			{
				short evnt = 0;
				short offset;
				for(offset=0; offset<(_events.Length-3); offset+=4)
				{
					if (BitConverter.ToInt16(_events, offset) != 0) break;
					evnt = BitConverter.ToInt16(_events, offset+2);
					if (evnt == (int)EventType.TitleText || evnt == (int)EventType.CaptionText
						|| (evnt >= (int)EventType.FGTag1 && evnt <= (int)EventType.FGTag8)
						|| (evnt == (int)EventType.XwaChangeRegion && _platform == MissionFile.Platform.XWA)) offset += 2;
					else if (evnt == (int)EventType.MoveMap || evnt == (int)EventType.ZoomMap
						|| (_platform == MissionFile.Platform.XWA
						&& (evnt == (int)EventType.XwaShipInfo || evnt == (int)EventType.XwaRotateIcon))) offset += 4;
					else if (evnt >= (int)EventType.TextTag1 && evnt <= (int)EventType.TextTag8) offset += 8;
					else if ((evnt == (int)EventType.XwaNewIcon || evnt == (int)EventType.XwaMoveIcon) && _platform == MissionFile.Platform.XWA)
						offset += 6;
				}
				return (short)(offset/2);
			}
		}
		/// <value>Unknown value</value>
		/// <remarks>Briefing offset 0x02, in between Length and StartLength</remarks>
		public short Unknown1
		{
			get { return _unknown1; }
			set { _unknown1 = value; }
		}
	}
}
