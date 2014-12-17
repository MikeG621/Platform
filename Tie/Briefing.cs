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
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Briefing object for TIE95</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48</remarks>
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is <b>12 (0xC)</b></remarks>
		public const int TicksPerSecond = 0xC;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is <b>200 (0xC8)</b></remarks>
		public const int EventQuantityLimit = 0xC8;

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{	//initialize
			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.TIE;
			Length = 0x21C;	//default 45 seconds
			_events = new short[0x190];
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			for (int i=0;i<0x20;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events[1] = (short)EventType.MoveMap;
			_events[5] = (short)EventType.ZoomMap;
			_events[6] = 0x30;
			_events[7] = 0x30;
			_events[8] = 9999;
			_events[9] = (short)EventType.EndBriefing;
		}
	}
}
