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
 * v4.0, 200809
 * [UPD] auto-properties
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual in-flight messages.</summary>
	/// <remarks>Class is serializable to allow copy and paste functionality.</remarks>
	[Serializable]
	public class Message : BaseMessage
	{
		string _note = "";

		/// <summary>Creates a new Message object.</summary>
		public Message()
		{
			for (int i = 0; i < 4; i++) Triggers[i] = new Mission.Trigger();
			SentToTeam[0] = true;
		}

		/// <summary>Gets the Triggers that control the Message behaviour.</summary>
		/// <remarks>Array length is 4.</remarks>
		public Mission.Trigger[] Triggers { get; } = new Mission.Trigger[4];
		/// <summary>Gets the array that control which teams can receive the message.</summary>
		/// <remarks>Array length is 10.</remarks>
		public bool[] SentToTeam { get; } = new bool[10];
		/// <summary>Gets or sets if both triggers must be completed.</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool T1OrT2 { get; set; }
		/// <summary>Gets or sets if both triggers must be completed.</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool T3OrT4 { get; set; }
		/// <summary>Gets or sets if both trigger pairs must be completed.</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool T12OrT34 { get; set; }
		/// <summary>Gets or sets the string used as editor notes.</summary>
		/// <remarks>Value is restricted to 15 characters.</remarks>
		public string Note
		{
			get => _note;
			set => _note = Common.StringFunctions.GetTrimmed(value, 0xF);
		}
		/// <summary>Gets or sets the delay after trigger is fired.</summary>
		/// <remarks>Multiply by <b>5</b> to get seconds. Default is <b>zero</b>. Value of <b>1</b> is 5s, <b>2</b> is 10s, etc.</remarks>
		public byte RawDelay { get; set; }
		/// <summary>Gets or sets the number of seconds after trigger is fired.</summary>
		/// <remarks>Rounds down to the nearest multiple of 5, maximum of <b>1275</b> or <b>21:15</b>.</remarks>
		public ushort DelaySeconds
		{
			get => (ushort)(RawDelay * 5);
			set => RawDelay = value < 1275 ? (byte)(value / 5) : (byte)255;
		}
	}
}