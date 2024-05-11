/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1+
 */

/* CHANGELOG
 * [NEW] ConvertTicksToSeconds and ConvertSecondsToTicks
 * [UPD] Events changed to collection
 * v2.1, 141214
 * [UPD] change to MPL
 */

namespace Idmr.Platform.Tie
{
	/// <summary>Briefing object for TIE95.</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48.</remarks>
	public class Briefing : BaseBriefing
	{
		/// <summary>Frames per second for briefing animation.</summary>
		public const int TicksPerSecond = 0xC;
		/// <summary>Maximum number of events that can be held.</summary>
		public const int EventQuantityLimit = 200;

		/// <summary>Initializes a blank Briefing.</summary>
		public Briefing()
		{
            _platform = MissionFile.Platform.TIE;
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
			_events.Add(new Event(EventType.ZoomMap) { Variables = new short[]{ 0x30, 0x30 } });
		}

		/// <summary>Converts the time to the platform-specific tick count.</summary>
		/// <param name="seconds">Time in seconds.</param>
		/// <returns>The raw time value.</returns>
		public override short ConvertSecondsToTicks(float seconds) => (short)(seconds * TicksPerSecond);

		/// <summary>Converts the time value into seconds.</summary>
		/// <param name="ticks">Raw time value.</param>
		/// <returns>The time per the platform-specific tick rate.</returns>
		public override float ConvertTicksToSeconds(short ticks) => (float)ticks / TicksPerSecond;
	}
}