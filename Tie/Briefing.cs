/*
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
	/// <summary>Briefing object for TIE95</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48</remarks>
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is 12 (0xC)</remarks>
		public const int TicksPerSecond = 0xC;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is 200 (0xC8)</remarks>
		public const int EventQuantityLimit = 0xC8;

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{	//initialize
            _platform = MissionFile.Platform.TIE;
			Length = 0x21C;	//default 45 seconds
			_events = new byte[0x320];
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			for (int i=0;i<0x20;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events[2] = 6;		// move map to (0,0)
			_events[10] = 7;	// zoom map
			_events[12] = 0x30;
			_events[14] = 0x30;
			_events[16] = 0xF;
			_events[17] = 0x27;
			_events[18] = 0x22;
		}
	}
}
