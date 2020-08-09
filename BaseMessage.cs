/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] Message length increased to 64 [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 */
 
using System;

namespace Idmr.Platform
{
	/// <summary>Base class for in-flight messages</summary>
	/// <remarks>Serializable to allow copy-paste functionality</remarks>
	[Serializable]
	public abstract class BaseMessage
	{
		/// <summary>The message shown in-flight</summary>
		private protected string _messageString = "New Message";

		/// <summary>Gets or sets the in-flight message string</summary>
		/// <remarks>Restricted to 64 characters, defaults to <b>"New Message"</b></remarks>
		public string MessageString
		{
			get { return _messageString; }
			set { _messageString = Common.StringFunctions.GetTrimmed(value, 0x40); }
		}
		/// <summary>Gets or sets the message color index</summary>
		public byte Color { get; set; }
	}
}
