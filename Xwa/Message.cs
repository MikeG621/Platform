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
 * [NEW] Format spec implemented
 * [UPD] Delay renamed to RawDelay
 * v5.7.5, 230116
 * [UPD] LengthLimit name
 * v5.7.3, 220619
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
	/// <summary>Object for individual in-flight messages.</summary>
	/// <remarks>Class is serializable to allow copy and paste functionality.</remarks>
	[Serializable]
	public class Message : BaseMessage
	{
		string _voiceID = "";
		string _note = "";

		/// <summary>Values for <see cref="CancelMeaning"/>.</summary>
		public enum CancelValues : byte
		{
			/// <summary>Ignores Message entirely, will not show.</summary>
			Cancel,
			/// <summary>Unknown.</summary>
			Stop,
			/// <summary>Unknown.</summary>
			Finished,
			/// <summary>Unknown.</summary>
			Both
		}

		/// <summary>Creates a new Message object.</summary>
		/// <remarks>Sent to Team 1 by default.</remarks>
		public Message()
		{
			LengthLimit = 0x44;
			for (int i = 0; i < 6; i++) Triggers[i] = new Mission.Trigger();
			SentTo[0] = true;
		}

		/// <summary>Gets or sets if the triggers are mutually exclusive.</summary>
		public bool T1AndOrT2
		{
			get => TrigAndOr[0];
			set => TrigAndOr[0] = value;
		}
		/// <summary>Gets or sets if the triggers are mutually exclusive.</summary>
		public bool T3AndOrT4
		{
			get => TrigAndOr[1];
			set => TrigAndOr[1] = value;
		}
		/// <summary>Gets or sets if the triggers pairs are mutually exclusive.</summary>
		public bool T12AndOrT34
		{
			get => TrigAndOr[2];
			set => TrigAndOr[2] = value;
		}
		/// <summary>Gets or sets if the triggers are mutually exclusive.</summary>
		public bool CancelT1AndOrT2
		{
			get => TrigAndOr[3];
			set => TrigAndOr[3] = value;
		}
		/// <summary>Gets the Triggers that control the Message behaviour.</summary>
		/// <remarks>Array length is 6. Four normal Triggers, followed by two Cancel Triggers.</remarks>
		public Mission.Trigger[] Triggers { get; } = new Mission.Trigger[6];
		/// <summary>Gets which teams can receive the message.</summary>
		/// <remarks>Array length = 10.</remarks>
		public bool[] SentTo { get; } = new bool[10];
		/// <summary>Gets the array for the trigger AndOr values.</summary>
		/// <remarks>Array is {1AO2, 3AO4, 12AO34, Cancel1AO2}. <b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool[] TrigAndOr { get; } = new bool[4];
		/// <summary>Gets or sets the editor note typically used to signify the speaker.</summary>
		/// <remarks>Value is restricted to 8 characters.</remarks>
		public string VoiceID
		{
			get => _voiceID;
			set => _voiceID = StringFunctions.GetTrimmed(value, 8);
		}
		/// <summary>Gets or sets the editor note typically used to signify the FlightGroup index of the speaker.</summary>
		/// <remarks>Defaults to <b>zero</b>, there appears to be no game mechanic or consequence for using this value.</remarks>
		public byte OriginatingFG { get; set; }	// TODO XWA: test this in-game for highlighting
		/// <summary>Unknown use.</summary>
		public int Type {  get; set; }
		/// <summary>Gets or sets the delay after trigger is fired.</summary>
		/// <remarks>Default is <b>zero</b>. Unlike TIE and XvT, it isn't a simple multiplier.  Use <see cref="Mission.GetDelaySeconds"/> to convert the delay into total seconds.</remarks>
		public byte RawDelay { get; set; }
		/// <summary>Gets the number of seconds after trigger is fired.</summary>
		public ushort DelaySeconds => (ushort)Mission.GetDelaySeconds(RawDelay);
		/// <summary>Unknown use.</summary>
		public bool SpeakerHeader { get; set; }
		/// <summary>Unknown use.</summary>
		public byte CancelMeaning { get; set; }
		/// <summary>Gets or sets the editor note.</summary>
		/// <remarks>Value is restricted to 63 characters.</remarks>
		public string Note
		{
			get => _note;
			set => _note = StringFunctions.GetTrimmed(value, 0x53);
		}
	}
}
