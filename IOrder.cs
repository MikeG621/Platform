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
	/// <summary>Interface for FlightGroup orders</sumamry>
	public interface IOrder
	{
		int Length { get; }
		
		byte Command { get; set; }
		byte Throttle { get; set; }
		byte Variable1 { get; set; }
		byte Variable2 { get; set; }
		// skip
		// skip
		byte Target3Type { get; set; }
		byte Target4Type { get; set; }
		byte Target3 { get; set; }
		byte Target4 { get; set; }
		bool T3AndOrT4 { get; set; }
		// skip
		byte Target1Type { get; set; }
		byte Target1 { get; set; }
		byte Target2Type { get; set; }
		byte Target2 { get; set; }
		bool T1AndOrT2 { get; set; }
		
		byte this[int index] { get; set; }
	}
}
