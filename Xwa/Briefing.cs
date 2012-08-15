/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0.1
 */

/* CHANGELOG
 * v2.0, 120525
 * [DEL] Team.set
 * [NEW] BriefingStringsNotes
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Briefing object for XWA</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 32. Class is serializable for copy/paste functionality</remarks>
	[Serializable]
	public class Briefing : BaseBriefing
	{
		Indexer<string> _stringsNotes = new Indexer<string>(new string[0x80], 0x64);

		bool[] _team = new bool[10];
		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is <b>25 (0x19)</b></remarks>
		public const int TicksPerSecond = 0x19;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is <b>4352 (0x1100)</b></remarks>
		public const int EventQuantityLimit = 0x1100;

		/// <summary>Initializes a blank briefing</summary>
		public Briefing()
		{	//initialize
			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.XWA;
			Length = 0x465;		// default to 45 seconds
			_events = new short[0x2200];
			_briefingTags = new string[0x80];
			_briefingStrings = new string[0x80];
			for (int i=0;i<0x80;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
				_stringsNotes[i] = "";
			}
			_events[1] = (short)EventType.MoveMap;
			_events[5] = (short)EventType.ZoomMap;
			_events[6] = 0x30;
			_events[7] = 0x30;
			_events[8] = 9999;
			_events[9] = (short)EventType.EndBriefing;
			_team[0] = true;
		}

		/// <summary>Gets the briefing team visibility</summary>
		/// <remarks>Determines which teams view the briefing. Array length = 10</remarks>
		public bool[] Team { get { return _team; } }

		/// <summary>Gets the Indexer for the notes attributed to <see cref="BaseBriefing.BriefingString"/></summary>
		/// <remarks>100 char limit. Used as voice actor instructions</remarks>
		public Indexer<string> BriefingStringsNotes { get { return _stringsNotes; } }
	}
}
