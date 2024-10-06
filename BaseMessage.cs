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
 * [UPD] LengthLimit changed to 63 to account for null-term
 * v5.7.5, 230116
 * [UPD] LengthLimit now public
 * v5.7.3, 220619
 * [UPD] length limit pulled out to a variable
 * v4.0, 200809
 * [UPD] Message length increased to 64 [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 */

using System;

namespace Idmr.Platform
{
	/// <summary>Base class for in-flight messages.</summary>
	/// <remarks>Serializable to allow copy-paste functionality.</remarks>
	[Serializable]
	public abstract class BaseMessage
	{
		/// <summary>The message shown in-flight.</summary>
		private protected string _messageString = "New Message";

		/// <summary>The length of <see cref="MessageString"/>.</summary>
		/// <remarks>Defaults to <b>63</b>.</remarks>
		public static int LengthLimit { get; internal set; } = 0x3F;

		/// <summary>Gets or sets the in-flight message string.</summary>
		/// <remarks>Restricted to 63 characters for TIE/XvT, 68 for XWA, defaults to <b>"New Message"</b>.</remarks>
		public string MessageString
		{
			get => _messageString;
			set => _messageString = Common.StringFunctions.GetTrimmed(value, LengthLimit);
		}
		/// <summary>Gets or sets the message color index.</summary>
		public byte Color { get; set; }
	}
}