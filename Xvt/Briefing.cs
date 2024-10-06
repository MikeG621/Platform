/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [NEW] ConvertTicksToSeconds and ConvertSecondsToTicks
 * [DEL] Unknown
 * [UPD] Events changed to collection
 * v6.1, 231208
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
	/// <summary>Briefing object for XvT and BoP.</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48.</remarks>
	[Serializable]
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation.</summary>
		public const int TicksPerSecond = 21;
		/// <summary>Maximum number of events that can be held.</summary>
		public const int EventQuantityLimit = 200;

		/// <summary>Initializes a blank Briefing.</summary>
		public Briefing()
		{
            _platform = MissionFile.Platform.XvT;
			Length = 45 * TicksPerSecond;
			_events = new EventCollection(_platform);
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			for (int i = 0; i < 0x20; i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events.Add(new Event(EventType.MoveMap));
			_events.Add(new Event(EventType.ZoomMap) { Variables = new short[] { 0x30, 0x30 } });
		}

		/// <summary>Converts the time value into seconds.</summary>
		/// <param name="ticks">Raw time value.</param>
		/// <returns>The time per the platform-specific tick rate.</returns>
		public override float ConvertTicksToSeconds(short ticks) => (float)ticks / TicksPerSecond;
		/// <summary>Converts the time to the platform-specific tick count.</summary>
		/// <param name="seconds">Time in seconds.</param>
		/// <returns>The raw time value.</returns>
		public override short ConvertSecondsToTicks(float seconds) => (short)(seconds * TicksPerSecond);

		/// <summary>Gets the briefing team visibility.</summary>  
		/// <remarks>Determines which teams view the briefing. Array length = 10.</remarks>  
		public bool[] Team { get; } = new bool[10];
	}
}