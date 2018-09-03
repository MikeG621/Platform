/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0
 */

/* CHANGELOG
 * v3.0, 180309
 * [UPD] Added Xwing support [JB]
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * - class made abstract
 */

using System.IO;

namespace Idmr.Platform
{
	/// <summary>Function class for mission files</summary>
	/// <remarks>Contains functions to determine platform</remarks>
	public abstract class MissionFile
	{
		static string _extension = ".tie";
		
		/// <summary>Error message template</summary>
		protected string _invalidError = "File is not a valid {0} mission file";
		/// <summary>Default filename</summary>
		protected string _missionPath = "\\NewMission" + _extension;
		
		/// <summary>Types of mission files</summary>
		public enum Platform : byte {
            /// <summary>X-wing Win95 Collector's Edition</summary>
            Xwing,
            /// <summary>TIE Fighter Win95 Collector's Edition</summary>
			TIE,
			/// <summary>X-wing v. TIE Fighter</summary>
			XvT,
			/// <summary>XvT Balance of Power expansion</summary>
			BoP,
			/// <summary>X-wing Alliance</summary>
			XWA,
			/// <summary>Unsupported platform</summary>
			Invalid 
		}

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
			if (!stream.Name.ToLower().EndsWith(".tie") && !stream.Name.ToLower().EndsWith(".xwi")) return Platform.Invalid;  //[JB] Need to be able to load .xwi files.
			stream.Position = 0;
			short p = new BinaryReader(stream).ReadInt16();
			if (p == -1) return Platform.TIE;
			else if (p == 0x2) return Platform.Xwing;
			else if (p == 0xC) return Platform.XvT;
			else if (p == 0xE) return Platform.BoP;
			else if (p == 0x12) return Platform.XWA;
			else return Platform.Invalid;
		}
		
		/// <summary>Gets or sets the full path to the mission file</summary>
		/// <remarks>Defaults to <b>"\\NewMission.tie"</b>. Will automatically add the ".tie" extension if omitted</remarks>
		public string MissionPath
		{
			get { return _missionPath; }
			set
			{
				_missionPath = value;  //[JB] Modified.  Assumes TIE, but also checks for XWI in path name.
				if (!value.ToLower().EndsWith(".xwi") && !value.ToLower().EndsWith(_extension))
					_missionPath += _extension;
			}
		}
		/// <summary>Gets the file name of the mission file</summary>
		/// <remarks>Defaults to <b>"NewMission.tie"</b></remarks>
		public string MissionFileName { get { return Idmr.Common.StringFunctions.GetFileName(MissionPath, true); } }
	}
}
