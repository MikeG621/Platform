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
	/// <summary>Interface for Triggers</summary>
	public interface ITrigger
	{
		int Length { get; }
		
		byte Condition { get; set; }
		byte VariableType { get; set; }
		byte Variable { get; set; }
		byte Amount { get; set; }

		byte this[int index] { get; set; }
	}
}
