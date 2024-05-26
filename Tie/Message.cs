/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0+
 */

/* CHANGELOG
 * [UPD] Format spec
 * v4.0, 200809
 * [UPD] Triggers to auto-property
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for individual in-flight messages.</summary>
	/// <remarks>Class is serializable to allow copy and paste functionality.</remarks>
	[Serializable] public class Message : BaseMessage
	{
		string _short = "";

		/// <summary>Creates a new Message object.</summary>
		public Message()
		{
			Triggers[0] = new Mission.Trigger();
			Triggers[1] = new Mission.Trigger();
		}

		/// <summary>Gets or sets the string used as editor notes.</summary>
		/// <remarks>Value is restricted to 15 characters.</remarks>
		public string Short
		{
			get => _short;
			set => _short = Common.StringFunctions.GetTrimmed(value, 0xF);
		}
		/// <summary>Gets or sets the seconds after trigger is fired divided by five.</summary>
		/// <remarks>Default is <b>zero</b>. Value of <b>1</b> is 5s, <b>2</b> is 10s, etc.</remarks>
		public byte RawDelay { get; set; }
		/// <summary>Gets or sets the number of seconds after trigger is fired.</summary>
		/// <remarks>Rounds down to the nearest multiple of 5, maximum of <b>1275</b> or <b>21:15</b>.</remarks>
		public ushort DelaySeconds
		{
			get => (ushort)(RawDelay * 5);
			set => RawDelay = value < 1275 ? (byte)(value / 5) : (byte)255;
		}

		/// <summary>Gets or sets if both triggers must be completed.</summary>
		/// <remarks><b>false</b> is "And", <b>true</b> is "Or", defaults to <b>false</b>.</remarks>
		public bool Trig1AndOrTrig2 { get; set; }

		/// <summary>Gets the Triggers that control the Message behaviour.</summary>
		/// <remarks>Array length is 2.</remarks>
		public Mission.Trigger[] Triggers { get; private set; } = new Mission.Trigger[2];
	}
}