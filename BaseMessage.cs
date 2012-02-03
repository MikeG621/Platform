/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for in-flight messages</summary>
	/// <remarks>Serializable to allow copy-paste functionality</remarks>
	public abstract class BaseMessage
	{
		protected string _messageString = "New Message";

		/// <summary>Gets or sets the in-flight message string</summary>
		/// <remarks>Restricted to 63 characters, defaults to "New Message"</remarks>
		public string MessageString
		{
			get { return _messageString; }
			set { _messageString = Idmr.Common.StringFunctions.GetTrimmed(value, 0x3F); }
		}
		/// <summary>Gets or sets the message color index</summary>
		/// <remarks>Default index of 0</remarks>
		public byte Color = 0;
	}
}
