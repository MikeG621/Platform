/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2022 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0+
 */

/* CHANGELOG
 * [UPD] length limit to 68
 * v4.0, 200809
 * [UPD] auto-properties
 * v3.0, 180903
 * [UPD] changes to Delay [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual in-flight messages</summary>
	/// <remarks>Class is serializable to allow copy and paste functionality</remarks>
	[Serializable]
	public class Message : BaseMessage
	{
		string _voiceID = "";
		string _note = "";

		/// <summary>Creates a new Message object</summary>
		/// <remarks>Sent to Team 1 by default</remarks>
		public Message()
		{
			_lengthLimit = 0x44;
			for (int i = 0; i < 6; i++) Triggers[i] = new Mission.Trigger();
			SentTo[0] = true;
		}

		/// <summary>Gets or sets if the triggers are mutually exclusive</summary>
		public bool T1AndOrT2
		{
			get => TrigAndOr[0];
			set => TrigAndOr[0] = value;
		}
		/// <summary>Gets or sets if the triggers are mutually exclusive</summary>
		public bool T3AndOrT4
		{
			get => TrigAndOr[1];
			set => TrigAndOr[1] = value;
		}
		/// <summary>Gets or sets if the triggers pairs are mutually exclusive</summary>
		public bool T12AndOrT34
		{
			get => TrigAndOr[2];
			set => TrigAndOr[2] = value;
		}
		/// <summary>Gets or sets if the triggers are mutually exclusive</summary>
		public bool CancelT1AndOrT2
		{
			get => TrigAndOr[3];
			set => TrigAndOr[3] = value;
		}
		/// <summary>Gets the Triggers that control the Message behaviour</summary>
		/// <remarks>Array length is 6. Four normal Triggers, followed by two Cancel Triggers</remarks>
		public Mission.Trigger[] Triggers { get; } = new Mission.Trigger[6];
		/// <summary>Gets which teams can receive the message</summary>
		/// <remarks>Array length = 10</remarks>
		public bool[] SentTo { get; } = new bool[10];
		/// <summary>Gets the array for the trigger AndOr values</summary>
		/// <remarks>Array is {1AO2, 3AO4, 12AO34, Cancel1AO2}. <b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b></remarks>
		public bool[] TrigAndOr { get; } = new bool[4];
		/// <summary>Unknown value</summary>
		/// <remarks>Offset 0x66</remarks>
		public byte Unknown1 { get; set; }
		/// <summary>Gets or sets the editor note typically used to signify the speaker</summary>
		/// <remarks>Value is restricted to 8 characters</remarks>
		public string VoiceID
		{
			get => _voiceID;
			set => _voiceID = StringFunctions.GetTrimmed(value, 8);
		}
		/// <summary>Gets or sets the editor note typically used to signify the FlightGroup index of the speaker</summary>
		/// <remarks>Defaults to <b>zero</b>, there appears to be no game mechanic or consequence for using this value</remarks>
		public byte OriginatingFG { get; set; }	// TODO: test this in-game for highlighting
		/// <summary>Gets or sets the delay after trigger is fired.</summary>
		/// <remarks>Default is <b>zero</b>. Unlike TIE and XvT, it contains unusual formatting and isn't in increments of 5 seconds.  Use <see cref="Mission.GetDelaySeconds"/> to convert the delay into total seconds.</remarks>
		public byte Delay { get; set; }
        /// <summary>Unknown value.  Formerly DelayMinutes.</summary>
        /// <remarks>Offset 0x8D</remarks>
        public byte Unknown2 { get; set; }
		/// <summary>Unknown value</summary>
		/// <remarks>Offset 0xA0</remarks>
		public bool Unknown3 { get; set; }
		/// <summary>Gets or sets the editor note</summary>
		/// <remarks>Value is restricted to 63 characters</remarks>
		public string Note
		{
			get => _note;
			set => _note = StringFunctions.GetTrimmed(value, 0x53);
		}
	}
}
