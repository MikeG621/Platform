/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Object for individual in-flight messages</summary>
	/// <remarks>Class is serializable to allow copy & paste functionality</remarks>
	public class Message : BaseMessage
	{
		Mission.Trigger[] _triggers = new Mission.Trigger[4];
		bool[] _sentToTeam = new bool[10];
		string _note = "";

		/// <summary>Creates a new Message object</summary>
		public Message()
		{
			for (int i = 0; i < 4; i++) _triggers[i] = new Mission.Trigger();
			_sentToTeam[0] = true;
		}

		/// <summary>Gets the Triggers that control the Message behaviour</summary>
		/// <remarks>Array length is 4</remarks>
		public Mission.Trigger[] Triggers { get { return _triggers; } }
		/// <summary>Gets the array that control which teams can receive the message</summary>
		/// <remarks>Array length is 10</remarks>
		public bool[] SentToTeam { get { return _sentToTeam; } }
		/// <summary>Gets or sets if both triggers must be completed</summary>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T1AndOrT2 = false;
		/// <summary>Gets or sets if both triggers must be completed</summary>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T3AndOrT4 = false;
		/// <summary>Gets or sets if both trigger pairs must be completed</summary>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T12AndOrT34 = false;
		/// <summary>Gets or sets the string used as editor notes</summary>
		/// <remarks>Value is restricted to 15 characters</remarks>
		public string Note
		{
			get { return _note; }
			set { _note = Idmr.Common.StringFunctions.GetTrimmed(value, 0xF); }
		}
		/// <summary>Gets or sets the seconds after trigger is fired divded by five</summary>
		/// <remarks>Default is zero. Value of 1 is 5s, 2 is 10s, etc.</remarks>
		public byte Delay = 0;
	}
}
