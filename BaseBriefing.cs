/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.8+
 */

/* CHANGELOG
 * [NEW] Unknown1 renamed per format spec
 * [UPD] EventParameters changed to singleton, this[] made private in lieu of GetCount()
 * [NEW] ConvertTicksToSeconds and ConvertSecondsToTicks
 * [UPD] Events changed to collection
 * [DEL] EventParameterCount, redundant with GetCount()
 * v5.8, 230804
 * [NEW] SkipMarker command
 * v3.0, 180309
 * [UPD] EventParameterCount changed to function, made virtual [JB]
 * [NEW] virtual helper functions [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0.1, 120814
 * [FIX] StartEvents calculation
 * v2.0, 120525
 * [UPD] Events changed to short[]
 * [NEW] EventParameterCount
 */

using System;

namespace Idmr.Platform
{
	/// <summary>Base class for Briefings.</summary>
	/// <remarks>Contains values that are shared among all briefing types. Class is Serializable to allow copy/paste functionality.</remarks>
	[Serializable]
	public abstract partial class BaseBriefing
	{
		/// <summary>The raw event data.</summary>
		private protected EventCollection _events;
		/// <summary>The strings placed on the map.</summary>
		private protected string[] _briefingTags;
		/// <summary>The captions and titles.</summary>
		private protected string[] _briefingStrings;
		/// <summary>The briefing format.</summary>
		private protected MissionFile.Platform _platform;

		/// <summary>Known briefing event types.</summary>
		public enum EventType : byte
		{
			/// <summary>No type defined.</summary>
			None,
			/// <summary>Creates breakpoint for briefing interface <b>Next</b> command (TIE/XvT only).</summary>
			/// <remarks>Parameters:<br/>None</remarks>
			SkipMarker,
			/// <summary>Unknown.</summary>
			/// <remarks>Parameters:<br/>Unknown use</remarks>
			ShipIndex,
			/// <summary>Clears Title and Caption text, creates breakpoint for briefing interface <b>Next</b> command.</summary>
			/// <remarks>Parameters:<br/>None</remarks>
			PageBreak,
			/// <summary>Displays the specified text at the top of the briefing.</summary>
			/// <remarks>Parameters:<br/>String #</remarks>
			TitleText,
			/// <summary>Displays the specified text at the bottom of the briefing.</summary>
			/// <remarks>Parameters:<br/>String #</remarks>
			CaptionText,
			/// <summary>Change the focal point of the map.</summary>
			/// <remarks>Parameters:<br/>X coord, Y coord</remarks>
			MoveMap,
			/// <summary>Change the zoom distance of the map.</summary>
			/// <remarks>Parameters:<br/>X zoom, Y zoom</remarks>
			ZoomMap,
			/// <summary>Erase all FlightGroup tags from view.</summary>
			/// <remarks>Parameters:<br/>None</remarks>
			ClearFGTags,
			/// <summary>Apply a FlightGroup tag using slot 1.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag1,
			/// <summary>Apply a FlightGroup tag using slot 2.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag2,
			/// <summary>Apply a FlightGroup tag using slot 3.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag3,
			/// <summary>Apply a FlightGroup tag using slot 4.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag4,
			/// <summary>Apply a FlightGroup tag using slot 5.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag5,
			/// <summary>Apply a FlightGroup tag using slot 6.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag6,
			/// <summary>Apply a FlightGroup tag using slot 7.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag7,
			/// <summary>Apply a FlightGroup tag using slot 8.</summary>
			/// <remarks>Parameters:<br/>FG #</remarks>
			FGTag8,
			/// <summary>Erase all text tags from view.</summary>
			/// <remarks>Parameters:<br/>None</remarks>
			ClearTextTags,
			/// <summary>Apply a text tag using slot 1.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag1,
			/// <summary>Apply a text tag using slot 2.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag2,
			/// <summary>Apply a text tag using slot 3.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag3,
			/// <summary>Apply a text tag using slot 4.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag4,
			/// <summary>Apply a text tag using slot 5.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag5,
			/// <summary>Apply a text tag using slot 6.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag6,
			/// <summary>Apply a text tag using slot 7.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag7,
			/// <summary>Apply a text tag using slot 8.</summary>
			/// <remarks>Parameters:<br/>Tag #, X coord, Y coord, Color</remarks>
			TextTag8,
			/// <summary>Define a ship icon (XWA only)</summary>
			/// <remarks>Parameters:<br/>Icon #, CraftType, Color</remarks>
			XwaSetIcon,
			/// <summary>Display craft type information (XWA only).</summary>
			/// <remarks>Parameters:<br/>Show state, Icon #</remarks>
			XwaShipInfo,
			/// <summary>Move a ship icon (XWA only).</summary>
			/// <remarks>Parameters:<br/>Icon #, X coord, Y coord</remarks>
			XwaMoveIcon,
			/// <summary>Rotate a ship icon (XWA only).</summary>
			/// <remarks>Parameters:<br/>Icon #, Orientation</remarks>
			XwaRotateIcon,
			/// <summary>Display the region name and transition (XWA only).</summary>
			/// <remarks>Parameters:<br/>Region #</remarks>
			XwaChangeRegion,
			/// <summary>Unknown (probably XWA only).</summary>
			/// <remarks>Parameters:<br/>String #</remarks>
			XwaZoomParagraph,
			/// <summary>End of briefing marker.</summary>
			/// <remarks>Parameters:<br/>None</remarks>
			EndBriefing = 0x22
		};

		/// <summary>Gets the Text Tags to be used on the map.</summary>
		/// <remarks>Length is determined by the derivative class.</remarks>
		public string[] BriefingTag => _briefingTags;
		/// <summary>Gets the Title and Caption strings.</summary>
		/// <remarks>Length is determined by the derivative class.</remarks>
		public string[] BriefingString => _briefingStrings;
		/// <summary>Gets the raw briefing event data.</summary>
		/// <remarks>Length is determined by the derivative class.</remarks>
		public EventCollection Events
		{
			get => _events;
			internal set => _events = value;
		}

		/// <summary>Gets or sets the duration of the Briefing, in Ticks.</summary>
		/// <remarks>The X-wing series uses Ticks instead of seconds, TicksPerSecond is defined by the derivative class.</remarks>
		public short Length { get; set; }
		/// <summary>Gets or sets the current time.</summary>
		/// <remarks>Not important for design, used by the editor/platform to keep track of current time, stored value will be overwritten.</remarks>
		public short CurrentTime { get; set; }
		/// <summary>Gets the total number of <i>short</i> values in <see cref="Events"/> at time = 0.</summary>
		/// <remarks>Not important for design, used by the editor/platform as "CurrentEvent" offset within the raw short array, stored value will be overwritten.</remarks>
		public short StartLength
		{
			get
			{
				short len = 0;
				for (int i = 0; i < _events.Count; i++)
				{
					if (_events[i].Time != 0) break;
					len += _events[i].Length;
				}
				return len;
			}
		}
		/// <summary>Gets the number of occupied <i>shorts</i> in <see cref="Events"/>.</summary>
		/// <remarks>Not important for design, platform may completely ignore it.</remarks>
		public short EventsLength => _events.Length;
		/// <summary>Though named, doesn't appear to ever be used.</summary>
		public short Tile { get; set; }

		/// <summary>Converts the time value into seconds.</summary>
		/// <param name="ticks">Raw time value.</param>
		/// <returns>The time per the platform-specific tick rate.</returns>
		abstract public float ConvertTicksToSeconds(short ticks);
		/// <summary>Converts the time to the platform-specific tick count.</summary>
		/// <param name="seconds">Time in seconds.</param>
		/// <returns>The raw time value.</returns>
		abstract public short ConvertSecondsToTicks(float seconds);

		/// <summary>Adjust FG references as necessary.</summary>
		/// <param name="fgIndex">Original index.</param>
		/// <param name="newIndex">Replacement index.</param>
		public virtual void TransformFGReferences(int fgIndex, int newIndex)
		{
			bool deleteCommand = false;
			if (newIndex < 0) deleteCommand = true;
			for (int i = 0; i < Events.Count && !Events[i].IsEndEvent; i++)
			{
				if (Events[i].IsFGTag)
				{
					if (Events[i].Variables[0] == fgIndex)
					{
						if (!deleteCommand) Events[i].Variables[0] = (short)newIndex;
						else Events.RemoveAt(i);
					}
					else if (Events[i].Variables[0] > fgIndex && deleteCommand) Events[i].Variables[0]--;
				}
			}
		}
		/// <summary>Swap FG indexes.</summary>
		/// <param name="srcIndex">First index.</param>
		/// <param name="dstIndex">Second index.</param>
		/// <remarks>Calls <see cref="TransformFGReferences(int, int)"/> with an interim value.</remarks>
		public virtual void SwapFGReferences(int srcIndex, int dstIndex)
		{
			TransformFGReferences(dstIndex, 255);
			TransformFGReferences(srcIndex, dstIndex);
			TransformFGReferences(255, srcIndex);
		}

		/// <summary>Singleton object to maintain a read-only array.</summary>
		public class EventParameters
		{
			static readonly EventParameters _instance = new EventParameters();

			readonly byte[] _counts = { 0, 0, 1, 0, 1, 1, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 4, 4, 4, 4, 4, 4, 4, 4, 3, 2, 3, 2, 1, 1, 0, 0, 0 };

			private EventParameters() { }

			byte this[int eventType] => _counts[eventType];
			byte this[EventType eventType] => _counts[(int)eventType];

			/// <summary>Gets a parameter count.</summary>
			/// <param name="eventType">The briefing event.</param>
			/// <returns>The number of variables for the event.</returns>
			/// <exception cref="IndexOutOfRangeException">Invalid <paramref name="eventType"/> value.</exception>
			public static byte GetCount(int eventType) => _instance[eventType];
			/// <summary>Gets a parameter count.</summary>
			/// <param name="eventType">The briefing event.</param>
			/// <returns>The number of variables for the event.</returns>
			public static byte GetCount(EventType eventType) => _instance[eventType];
		}
	}
}