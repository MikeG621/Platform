/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

/* CHANGELOG
 * 111208 - removed Team.set
 */

using System;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Briefing object for XWA</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 32. Class is serializable for copy/paste functionality</remarks>
	public class Briefing : BaseBriefing
	{
		bool[] _team = new bool[10];
		/// <summary>Frames per second for briefing animation</summary>
		public const int TicksPerSecond = 0x19;
		/// <summary>Maximum number of events that can be held</summary>
		public const int EventQuantityLimit = 0x1100;

		/// <summary>Initializes a blank briefing</summary>
		public Briefing()
		{	//initialize
			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.XWA;
			Length = 0x465;		// default to 45 seconds
			_events = new byte[0x4400];
			_briefingTags = new string[0x80];
			_briefingStrings = new string[0x80];
			int i;
			for (i=0;i<0x80;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events[2] = 6;		// move map to (0,0)
			_events[10] = 7;	// zoom map
			_events[12] = 0x20;
			_events[14] = 0x20;
			_events[16] = 0xF;
			_events[17] = 0x27;
			_events[18] = 0x22;
			_team[0] = true;
		}

		/// <summary>Gets the briefing team visibility</summary>
		/// <remarks>Determines which teams view the briefing. Array length = 10</remarks>
		public bool[] Team { get { return _team; } }
	}
}
