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
	/// <sumamry>Interface for FlightGroup waypoints</summary>
	public interface IWaypoint
	{
		int Length { get; }
		
		short RawX { get; set; }
		short RawY { get; set; }
		short RawZ { get; set; }
		bool Enabled { get; set; }
		double X { get; set; }
		double Y { get; set; }
		double Z { get; set; }

		short this[int index] { get; set; }
	}
}
