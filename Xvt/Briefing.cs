/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2023 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.7.4+
 */

/* CHANGELOG
 * [FIX] EventQuantityLimit
 * v5.7.4, 220827
 * [FIX] TicksPerSecond from 0x14 to 0x15
 * v4.0, 200809
 * [UPD] Teams now auto-property
 * v3.0, 180903
 * [UPD] Moved Team init to BriefingCollection [JB]
 * v2.5, 170107
 * [ADD] Team functionality [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform.Xvt
{
	/// <summary>Briefing object for XvT and BoP</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48. Class is serializable for copy/paste functionality</remarks>
	[Serializable]
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is <b>21 (0x15)</b></remarks>
		public const int TicksPerSecond = 0x15;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is <b>200 (0xC8)</b></remarks>
		public const int EventQuantityLimit = 0xC8;

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{	//initialize
			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.XvT;
			Length = 0x3B1;	// default to 45 seconds
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
            //[JB] Moved team visibility to BriefingCollection to fix a bug where the team[0] would see "every" briefing (the last team to be flagged?)
		}

		/// <summary>Gets or sets the unknown setting</summary>
		/// <remarks>Briefing offset 0x08</remarks>
		public short Unknown3 { get; set; }

		/// <summary>Gets the briefing team visibility</summary>  
		/// <remarks>Determines which teams view the briefing. Array length = 10</remarks>  
		public bool[] Team { get; } = new bool[10];
	}
}
