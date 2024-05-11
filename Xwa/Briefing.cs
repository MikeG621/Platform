/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0+
 */

/* CHANGELOG
 * [NEW] Full format spec implemented
 * [FIX] Corrected EventQuantityLimit
 * v4.0, 200809
 * [UPD] auto-properties
 * v3.0, 180903
 * [UPD] removed team visiblity init [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [DEL] Team.set
 * [NEW] BriefingStringsNotes
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Briefing object for XWA.</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 32. Class is serializable for copy/paste functionality.</remarks>
	[Serializable]
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation.</summary>
		public const int TicksPerSecond = 25;
		/// <summary>Maximum number of events that can be held.</summary>
		public const int EventQuantityLimit = 3200;

		/// <summary>Initializes a blank briefing.</summary>
		public Briefing()
		{
			_platform = MissionFile.Platform.XWA;
			Length = 0x465;     // default to 45 seconds
			_events = new short[0x1900];
			_briefingTags = new string[0x80];
			_briefingStrings = new string[0x80];
			for (int i = 0; i < 0x80; i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
				BriefingStringsNotes[i] = "";
			}
			_events[1] = (short)EventType.MoveMap;
			_events[5] = (short)EventType.ZoomMap;
			_events[6] = 0x30;
			_events[7] = 0x30;
			_events[8] = 9999;
			_events[9] = (short)EventType.EndBriefing;
		}

		/// <summary>Converts the time value into seconds.</summary>
		/// <param name="time">Raw time value.</param>
		/// <returns>The time per the platform-specific tick rate.</returns>
		public override float GetTimeInSeconds(short time) => (float)time / TicksPerSecond;

		/// <summary>Gets the briefing team visibility.</summary>
		/// <remarks>Determines which teams view the briefing. Array length = 10.</remarks>
		public bool[] Team { get; } = new bool[10];

		/// <summary>Icon definitions per FlightGroup.</summary>
		/// <remarks>Unknown if this is used.</remarks>
		public Icon[] Icons { get; } = new Icon[192];

		/// <summary>Gets the Indexer for the notes attributed to <see cref="BaseBriefing.BriefingString"/>.</summary>
		/// <remarks>100 char limit. Used as voice actor instructions.</remarks>
		public Indexer<string> BriefingStringsNotes { get; } = new Indexer<string>(new string[0x80], 0x64);

		/// <summary>Container for FG Icon definitions.</summary>
		public struct Icon
		{
			/// <summary>Craft type.</summary>
			public byte Species { get; set; }
			/// <summary>Icon color.</summary>
			public byte IFF { get; set; }
			/// <summary>Initial? X location.</summary>
			public short X { get; set; }
			/// <summary>Initial? Y location.</summary>
			public short Y { get; set; }
			/// <summary>Initial? icon rotation.</summary>
			public short Orientation { get; set; }
		}
	}
}
