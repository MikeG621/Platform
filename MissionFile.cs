using System;
using System.IO;

namespace Idmr.Platform
{
	/// <summary>Function class for mission files</summary>
	/// <remarks>Contains functions to determine platform and to single out triggers.</remarks>
	public static class MissionFile
	{
		/// <summary>Types of mission files</summary>
		/// <remarks>0 = TIE<br>1 = XvT<br>2 = BoP<br>3 = XWA<br>4 = Invalid
		public enum Platform { TIE, XvT, BoP, XWA, Invalid }

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
		/// <summary>Returns a single Trigger from the full array</summary>
		/// <remarks>XWA will return an array with Length=6, the other platforms will return Length=4</remarks>
		/// <param name="fullTriggerArray">Complete 2-D Trigger array</param>
		/// <param name="triggerIndex">Selected Trigger</param>
		/// <returns>1-D Byte array of selected trigger</returns>
		public static byte[] Trigger(byte[,] fullTriggerArray, int triggerIndex)
		{
			byte[] b = new byte[6];
			try { b[0] = fullTriggerArray[0, 4]; }	// doesn't throw on XWA 6-byte triggers
			catch { b = new byte[4]; }
			for (int i=0;i<b.Length;i++) b[i] = fullTriggerArray[triggerIndex, i];
			return b;
		}
	}
}
