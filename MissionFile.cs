/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using System.IO;

namespace Idmr.Platform
{
	/// <summary>Function class for mission files</summary>
	/// <remarks>Contains functions to determine platform</remarks>
	public static class MissionFile
	{
		/// <summary>Types of mission files</summary>
		public enum Platform : byte { TIE, XvT, BoP, XWA, Invalid }

		/// <summary>Returns the Platform of the given file</summary>
		/// <remarks>Returns Platform.Invalid on error</remarks>
		/// <param name="fileMission">Full path to un-opened mission file</param>
		/// <returns>Enumerated Platform</returns>
		public static Platform GetPlatform(string fileMission)
		{
			if (!fileMission.ToLower().EndsWith(".tie")) return Platform.Invalid;
			FileStream fs = File.OpenRead(fileMission);
			Platform p = GetPlatform(fs);
			fs.Close();
			return p;
		}
		/// <summary>Returns the Platform of the given file</summary>
		/// <remarks>Returns Platform.Invalid on error</remarks>
		/// <param name="stream">Stream to opened mission file</param>
		/// <returns>Enumerated Platform</returns>
		public static Platform GetPlatform(FileStream stream)
		{
			if (!stream.Name.ToLower().EndsWith(".tie")) return Platform.Invalid;
			stream.Position = 0;
			short p = new BinaryReader(stream).ReadInt16();
			if (p == -1) return Platform.TIE;
			else if (p == 0xC) return Platform.XvT;
			else if (p == 0xE) return Platform.BoP;
			else if (p == 0x12) return Platform.XWA;
			else return Platform.Invalid;
		}
	}
}
