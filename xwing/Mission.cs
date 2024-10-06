/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [UPD] Briefing events I/O
 * [UPD] ArrDep bool-short conversion
 * v4.0, 200809
 * [UPD] Unknown1 renamed to RndSeed [JB]
 * [UPD] Better Save backup [JB]
 * [FIX] Yaw/Pitch flipped during save [JB]
 * [UPD] FlightGroupLimit increased to 255 [JB]
 * [UPD] MessageLimit decreased to 0 [JB]
 * v3.0.1, 180919
 * [FIX] Object angle degress conversion
 * v3.0, 180903
 * [NEW] created [JB]
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Xwing
{
	/// <summary>Framework for XWING95.</summary>
	/// <remarks>This is the primary container object for a XWING95 mission file.</remarks>
	public partial class Mission : MissionFile
	{
		readonly string[] _endOfMissionMessages = new string[3]; //Mission Completion messages.
		Indexer<string> _endOfMissionIndexer;

		#region constructors
		/// <summary>Default constructor, creates a blank mission.</summary>
		public Mission()
		{
			initialize();
			for (int i = 0; i < 3; i++) _endOfMissionMessages[i] = "";
		}

		/// <summary>Creates a new mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		public Mission(string filePath)
		{
			initialize();
			LoadFromFile(filePath);
		}

		/// <summary>Creates a new mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		public Mission(FileStream stream)
		{
			initialize();
			LoadFromStream(stream);
		}

		void initialize()
		{
			_invalidError = _invalidError.Replace("{0}", "TIE");
			_endOfMissionIndexer = new Indexer<string>(_endOfMissionMessages, 63);
			FlightGroups = new FlightGroupCollection();
			FlightGroupsBriefing = new FlightGroupCollection();
			Briefing = new Briefing();
		}
		#endregion constructors

		#region public methods
		/// <summary>Loads a mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		/// <exception cref="FileNotFoundException"><paramref name="filePath"/> does not exist.</exception>
		/// <exception cref="InvalidDataException"><paramref name="filePath"/> is not a X-wing mission file.</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();

			string briefpath = filePath.ToLower().Replace(".xwi", ".brf");
			if (!File.Exists(briefpath)) return;  //May not have briefing file.
			FileStream bfs = File.OpenRead(briefpath);
			LoadBriefingFromStream(bfs);
			bfs.Close();
		}

		/// <summary>Loads a mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid X-wing mission file.</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (GetPlatform(stream) != Platform.Xwing) throw new InvalidDataException(_invalidError);
			BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
			int i;
			//Position 0 = PlatformID (2 bytes)
			stream.Position = 2;
			TimeLimitMinutes = br.ReadInt16();
			EndEvent = br.ReadInt16();
			RndSeed = br.ReadInt16();
			Location = br.ReadInt16();
			for (i = 0; i < 3; i++) EndOfMissionMessages[i] = new string(br.ReadChars(64));
			short numFlightGroups = br.ReadInt16();
			short numObjectGroups = br.ReadInt16();

			//Header is finished.  Begin Flight Groups.
			FlightGroups = new FlightGroupCollection(numFlightGroups + numObjectGroups);
			for (i = 0; i < numFlightGroups; i++)
			{
				#region Craft
				int j;
				FlightGroups[i].Name = new string(br.ReadChars(16));
				FlightGroups[i].Cargo = new string(br.ReadChars(16));
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(16));
				FlightGroups[i].SpecialCargoCraft = (byte)br.ReadInt16();
				FlightGroups[i].CraftType = (byte)br.ReadInt16();
				FlightGroups[i].IFF = (byte)br.ReadInt16();
				FlightGroups[i].Status1 = (byte)br.ReadInt16();
				FlightGroups[i].NumberOfCraft = (byte)br.ReadInt16();
				FlightGroups[i].NumberOfWaves = (byte)br.ReadInt16();
				#endregion
				#region Arr/Dep
				FlightGroups[i].ArrivalEvent = br.ReadInt16();
				FlightGroups[i].ArrivalDelay = br.ReadInt16();
				FlightGroups[i].ArrivalFG = br.ReadInt16();
				FlightGroups[i].Mothership = br.ReadInt16();
				FlightGroups[i].ArriveViaHyperspace = Convert.ToBoolean(br.ReadInt16());
				FlightGroups[i].DepartViaHyperspace = Convert.ToBoolean(br.ReadInt16());
				#endregion

				//Waypoints (7 real waypoints, rest are virtualized BRF coordinate sets)
				for (j = 0; j < 4; j++)
					for (int k = (byte)FlightGroup.WaypointIndex.Start1; k <= (byte)FlightGroup.WaypointIndex.Hyperspace; k++)
						FlightGroups[i].Waypoints[k][j] = br.ReadInt16();

				//More craft info
				FlightGroups[i].Formation = (byte)br.ReadInt16();
				FlightGroups[i].PlayerCraft = (byte)br.ReadInt16();
				FlightGroups[i].AI = (byte)br.ReadInt16();
				FlightGroups[i].Order = br.ReadInt16();
				FlightGroups[i].DockTimeThrottle = br.ReadInt16();
				FlightGroups[i].Markings = (byte)br.ReadInt16();
				stream.Position += 2; //Second marking.
				FlightGroups[i].Objective = br.ReadInt16();
				FlightGroups[i].TargetPrimary = br.ReadInt16();
				FlightGroups[i].TargetSecondary = br.ReadInt16();
			}
			//Read the Object Groups into the FG list.  Although Flight Groups and Object Groups are separate in X-wing, YOGEME abstracts them to appear as flight groups for editing purposes.
			for (i = numFlightGroups; i < FlightGroups.Count; i++)
			{
				FlightGroups[i].Name = new string(br.ReadChars(16));  //Only the name is used, but the format appears to resemble FGs 
				FlightGroups[i].Cargo = new string(br.ReadChars(16));
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(16));
				FlightGroups[i].SpecialCargoCraft = (byte)br.ReadInt16();
				FlightGroups[i].CraftType = 0;
				FlightGroups[i].ObjectType = br.ReadInt16();
				FlightGroups[i].IFF = (byte)br.ReadInt16();
				//The format begins to deviate here
				FlightGroups[i].Formation = (byte)br.ReadInt16();
				FlightGroups[i].NumberOfWaves = 0; //Doesn't exist in file, but setting it here just in case.
				FlightGroups[i].NumberOfCraft = (byte)br.ReadInt16();
				FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawX = br.ReadInt16();
				FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawY = br.ReadInt16();
				FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawZ = br.ReadInt16();
				FlightGroups[i].Yaw = br.ReadInt16();  //Conversion to/from degrees handled in the editor. This helps preserve the exact values used by pilot proving ground platforms.
				FlightGroups[i].Pitch = br.ReadInt16();
				FlightGroups[i].Roll = br.ReadInt16();
			}
			MissionPath = stream.Name;
		}

		/// <summary>Loads a mission briefing (.BRF) file from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission briefing file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid X-wing mission briefing file.</exception>
		public void LoadBriefingFromStream(FileStream stream)
		{
			BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.

			string str;
			short s;
			s = br.ReadInt16(); //PlatformID
			if (s != 2) throw new InvalidDataException("Not a valid X-wing briefing file.");
			short shipCount = br.ReadInt16();
			short coordCount = br.ReadInt16();
			FlightGroupsBriefing = new FlightGroupCollection(shipCount);
			int wp;
			for (int i = 0; i < coordCount; i++)
			{
				//Just in case there too many coord sets than what the editor allows, read them but only load them if the indexes are valid.
				if (i == 0) wp = (byte)FlightGroup.WaypointIndex.Start1;
				else wp = (byte)FlightGroup.WaypointIndex.CS1 + i - 1;  //at this point i==1 so subtract to compensate
				if (wp >= (byte)FlightGroup.WaypointIndex.CS4)
					wp = -1;
				for (int j = 0; j < shipCount; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						short dat = br.ReadInt16();
						if (wp >= 0)
							FlightGroupsBriefing[j].Waypoints[wp][k] = dat;
					}
					if (wp == (byte)FlightGroup.WaypointIndex.Start1)	// ~MG: was a range here, but only =0 was possible
						FlightGroupsBriefing[j].Waypoints[wp].Enabled = true;
				}
			}
			if (coordCount < 2) coordCount = 2;  //Sanity check for editor purposes.  All LEC missions have 2 sets.
			else if (coordCount > 4) coordCount = 4;
			Briefing.MaxCoordSet = coordCount;

			for (int i = 0; i < shipCount; i++)
			{
				s = br.ReadInt16(); //craft type
				if (s < 18)
				{
					FlightGroupsBriefing[i].CraftType = (byte)s;
					FlightGroupsBriefing[i].ObjectType = 0;
				}
				else
				{
					FlightGroupsBriefing[i].CraftType = 0;
					FlightGroupsBriefing[i].ObjectType = s;
				}

				FlightGroupsBriefing[i].IFF = (byte)br.ReadInt16();
				FlightGroupsBriefing[i].NumberOfCraft = (byte)br.ReadInt16();
				FlightGroupsBriefing[i].NumberOfWaves = (byte)br.ReadInt16();
				FlightGroupsBriefing[i].Name = new string(br.ReadChars(16));
				FlightGroupsBriefing[i].Cargo = new string(br.ReadChars(16));
				FlightGroupsBriefing[i].SpecialCargo = new string(br.ReadChars(16));
				FlightGroupsBriefing[i].SpecialCargoCraft = br.ReadInt16();
				FlightGroupsBriefing[i].Yaw = br.ReadInt16();
				FlightGroupsBriefing[i].Pitch = br.ReadInt16();
				FlightGroupsBriefing[i].Roll = br.ReadInt16();
			}

			#region WindowUISettings
			short count = br.ReadInt16();  //Setting count.  Usually 2, but not always.
			Briefing.ResetUISettings(count);
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					BriefingUIItem item = Briefing.WindowSettings[i].Items[j];
					item.Top = br.ReadInt16();
					item.Left = br.ReadInt16();
					item.Bottom = br.ReadInt16();
					item.Right = br.ReadInt16();
					item.IsVisible = Convert.ToBoolean(br.ReadInt16());
				}
			}
			#endregion WindowUISettings

			#region Pages
			count = br.ReadInt16();
			Briefing.ResetPages(count);
			for (int i = 0; i < count; i++)
			{
				BriefingPage pg = Briefing.Pages[i];
				pg.Length = br.ReadInt16(); //total ticks
				short len = br.ReadInt16();  //EventsLength
				pg.CoordSet = br.ReadInt16();
				pg.PageType = br.ReadInt16();

				byte[] briefBuffer = br.ReadBytes(len * 2);
				pg.Events = new Briefing.EventCollection(briefBuffer);
			}

			#endregion Pages

			stream.Position += 6;
			/*s = br.ReadInt16();  //TimeLimitMinutes?
			s = br.ReadInt16();  //EndEvent?
			s = br.ReadInt16();  //Unknown1?*/
			Briefing.MissionLocation = br.ReadInt16();
			stream.Position += 3 * 64;
			/*for (int i = 0; i < 3; i++)  //EndOfMissionMessages
				str = new string(br.ReadChars(64));*/
			stream.Position += 90 * shipCount;
			/*for (int i = 0; i < shipCount; i++)
			{
				//No idea what data this is, or even what order.
				br.ReadBytes(68);
				s = br.ReadInt16(); //PlayerCraft?
				br.ReadBytes(20);
			}*/

			#region Text/Tags
			count = br.ReadInt16();  //Tags
			Briefing.ResizeTagList(count);
			for (int i = 0; i < count; i++)
			{
				short len = br.ReadInt16();
				Briefing.BriefingTag[i] = new string(br.ReadChars(len));
			}
			count = br.ReadInt16();  //Text
			Briefing.ResizeStringList(count);
			for (int i = 0; i < count; i++)
			{
				short len = br.ReadInt16();
				str = new string(br.ReadChars(len));
				byte[] highlight = new byte[len];
				br.Read(highlight, 0, len);
				Briefing.BriefingString[i] = Briefing.TranslateHighlightToString(str, highlight);
			}
			#endregion Text/Tags

		}

		/// <summary>Saves the mission to <see cref="MissionFile.MissionPath"/>.</summary>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <see cref="MissionFile.MissionPath"/> are denied.</exception>
		public void Save()
		{
			//[JB] Added backup logic.  See the TIE Save() function for comments.
			if (File.Exists(MissionPath) && (File.GetAttributes(MissionPath) & FileAttributes.ReadOnly) != 0) throw new UnauthorizedAccessException("Cannot save, existing file is read-only.");

			FileStream fs = null;
			string backup = MissionPath.ToLower().Replace(".xwi", "_xwi.bak");
			bool backupCreated = false, writerCreated = false;

			if (File.Exists(MissionPath) && MissionPath.ToLower() != backup)
			{
				try
				{
					if (File.Exists(backup))
						File.Delete(backup);
					File.Copy(MissionPath, backup);
					backupCreated = true;
				}
				catch { }
			}
			try
			{
				if (File.Exists(MissionPath)) File.Delete(MissionPath);
				fs = File.OpenWrite(MissionPath);
				BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
				writerCreated = true;
				bw.Write((short)0x2);  //Platform
				bw.Write(TimeLimitMinutes);
				bw.Write(EndEvent);
				bw.Write(RndSeed);
				bw.Write(Location);
				for (int i = 0; i < 3; i++)
				{
					long p = fs.Position;
					bw.Write(_endOfMissionMessages[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x40;
				}
				int numFG = 0;
				int numOG = 0;
				for (int i = 0; i < FlightGroups.Count; i++)
				{
					if (FlightGroups[i].CraftType > 0) numFG++;
					if (FlightGroups[i].ObjectType > 0) numOG++;
				}
				bw.Write((short)numFG);
				bw.Write((short)numOG);


				#region Flightgroups
				for (int i = 0; i < FlightGroups.Count; i++)
				{
					if (FlightGroups[i].CraftType == 0) continue;

					long p = fs.Position;

					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0x10;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x20;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x30;
					bw.Write((short)FlightGroups[i].SpecialCargoCraft);
					bw.Write((short)FlightGroups[i].CraftType);
					bw.Write((short)FlightGroups[i].IFF);
					bw.Write((short)FlightGroups[i].Status1);
					bw.Write((short)FlightGroups[i].NumberOfCraft);
					bw.Write((short)FlightGroups[i].NumberOfWaves);
					#endregion
					#region Arr/Dep
					bw.Write(FlightGroups[i].ArrivalEvent);
					bw.Write(FlightGroups[i].ArrivalDelay);
					bw.Write(FlightGroups[i].ArrivalFG);
					bw.Write(FlightGroups[i].Mothership);
					bw.Write((short)(FlightGroups[i].ArriveViaHyperspace ? 1 : 0));
					bw.Write((short)(FlightGroups[i].DepartViaHyperspace ? 1 : 0));
					#endregion

					//Waypoints (7 real waypoints, rest are virtualized BRF coordinate sets)
					for (int j = 0; j < 4; j++)
						for (int k = (byte)FlightGroup.WaypointIndex.Start1; k <= (byte)FlightGroup.WaypointIndex.Hyperspace; k++)
							bw.Write(FlightGroups[i].Waypoints[k][j]);

					//More craft info
					bw.Write((short)FlightGroups[i].Formation);
					bw.Write((short)FlightGroups[i].PlayerCraft);
					bw.Write((short)FlightGroups[i].AI);
					bw.Write(FlightGroups[i].Order);
					bw.Write(FlightGroups[i].DockTimeThrottle);
					bw.Write((short)FlightGroups[i].Markings);
					bw.Write((short)FlightGroups[i].Markings);  //Was Unknown1. Another color value. Official missions always have the same color.
					bw.Write(FlightGroups[i].Objective);
					bw.Write(FlightGroups[i].TargetPrimary);
					bw.Write(FlightGroups[i].TargetSecondary);
				}
				// OBJECT GROUPS
				for (int i = 0; i < FlightGroups.Count; i++)
				{
					if (FlightGroups[i].ObjectType == 0) continue;

					long p = fs.Position;

					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0x10;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x20;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x30;
					bw.Write(FlightGroups[i].SpecialCargoCraft);

					bw.Write(FlightGroups[i].ObjectType);
					bw.Write((short)FlightGroups[i].IFF);
					//The format begins to deviate here
					bw.Write((short)FlightGroups[i].Formation);
					bw.Write((short)FlightGroups[i].NumberOfCraft);
					bw.Write(FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawX);
					bw.Write(FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawY);
					bw.Write(FlightGroups[i].Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawZ);
					bw.Write(FlightGroups[i].Yaw); //Conversion to/from degrees handled in the editor. This helps preserve the exact values used by pilot proving ground platforms.
					bw.Write(FlightGroups[i].Pitch);
					bw.Write(FlightGroups[i].Roll);
				}
				#endregion
				fs.SetLength(fs.Position);
				fs.Close();
			}
			catch
			{
				fs?.Close();
				if (writerCreated && backupCreated)
				{
					File.Delete(MissionPath);
					File.Copy(backup, MissionPath);
					File.Delete(backup);
				}
				throw;
			}
			if (backupCreated) File.Delete(backup);

			//Finished saving XWI file.  Now save the BRF file.
			string briefingPath = MissionPath;
			string ext = Path.GetExtension(MissionPath);
			bool upper;                       //This stuff is merely to try and make the BRF extension match the case of the XWI, so the file names look nice and tidy.
			if (ext != null && ext.Length > 1) upper = char.IsUpper(ext[1]);  //Detect case from the first character of the extension.
			else upper = char.IsUpper(briefingPath[briefingPath.Length - 1]);   // or from the last character of the name.
			briefingPath = Path.ChangeExtension(briefingPath, upper ? "BRF" : "brf");

			if (File.Exists(briefingPath) && (File.GetAttributes(briefingPath) & FileAttributes.ReadOnly) != 0) throw new UnauthorizedAccessException("Cannot save briefing, existing file is read-only.");

			fs = null;
			backup = briefingPath.ToLower().Replace(".brf", "_brf.bak");
			backupCreated = false; writerCreated = false;

			if (File.Exists(briefingPath) && briefingPath.ToLower() != backup)
			{
				try
				{
					if (File.Exists(backup)) File.Delete(backup);
					File.Copy(briefingPath, backup);
					backupCreated = true;
				}
				catch { }
			}
			try
			{
				if (File.Exists(briefingPath)) File.Delete(briefingPath);
				fs = File.OpenWrite(briefingPath);
				BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
				writerCreated = true;
				bw.Write((short)2);   //Version
				bw.Write((short)FlightGroupsBriefing.Count);
				bw.Write(Briefing.MaxCoordSet);  //Coordinate count;
				long p = 0;
				int wp = 0;
				for (int i = 0; i < Briefing.MaxCoordSet; i++)  //Coordinate count
				{
					//Just in case there too many coord sets than what the editor allows, read them but only load them if the indexes are valid.
					if (i == 0) wp = 0;  //SP1
					else
						wp = 7 + i - 1;  //CS1 starts at [7], but at this point i==1 so subtract to compensate
					if (wp >= 10)
						wp = -1;

					for (int j = 0; j < FlightGroupsBriefing.Count; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							short dat = 0;
							if (wp >= 0)
								dat = FlightGroupsBriefing[j].Waypoints[wp][k];

							bw.Write(dat);
						}
					}
				}
				for (int i = 0; i < FlightGroupsBriefing.Count; i++)
				{
					if (FlightGroupsBriefing[i].IsFlightGroup())
						bw.Write((short)FlightGroupsBriefing[i].CraftType);
					else
						bw.Write(FlightGroupsBriefing[i].ObjectType);

					bw.Write((short)FlightGroupsBriefing[i].IFF);
					bw.Write((short)FlightGroupsBriefing[i].NumberOfCraft);
					bw.Write((short)FlightGroupsBriefing[i].NumberOfWaves);

					p = fs.Position;
					bw.Write(FlightGroupsBriefing[i].Name.ToCharArray());
					fs.Position = p + 0x10;
					bw.Write(FlightGroupsBriefing[i].Cargo.ToCharArray());
					fs.Position = p + 0x20;
					bw.Write(FlightGroupsBriefing[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x30;

					bw.Write(FlightGroupsBriefing[i].SpecialCargoCraft);
					bw.Write(FlightGroupsBriefing[i].Yaw);
					bw.Write(FlightGroupsBriefing[i].Pitch);
					bw.Write(FlightGroupsBriefing[i].Roll);
				}

				#region WindowUISettings
				short count = (short)Briefing.WindowSettings.Count;
				bw.Write(count);
				for (int i = 0; i < count; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						BriefingUIItem item = Briefing.WindowSettings[i].Items[j];
						bw.Write(item.Top);
						bw.Write(item.Left);
						bw.Write(item.Bottom);
						bw.Write(item.Right);
						bw.Write(Convert.ToInt16(item.IsVisible));
					}
				}
				#endregion WindowUISettings

				#region Pages
				bw.Write((short)Briefing.Pages.Count);
				for (int i = 0; i < Briefing.Pages.Count; i++)
				{
					BriefingPage pg = Briefing.GetBriefingPage(i);
					bw.Write(pg.Length);
					bw.Write(pg.EventsLength);
					bw.Write(pg.CoordSet);
					bw.Write(pg.PageType);

					byte[] briefBuffer = new byte[pg.Events.Length * 2];	// X-wing is unique that this is dynamic, other platforms use EventQuantityLimit
					Buffer.BlockCopy(pg.Events.GetArray(), 0, briefBuffer, 0, briefBuffer.Length);
					bw.Write(briefBuffer);
				}
				#endregion Pages

				bw.Write(TimeLimitMinutes);
				bw.Write(EndEvent);
				bw.Write(RndSeed);
				bw.Write(Briefing.MissionLocation);

				p = fs.Position;
				for (int i = 0; i < 3; i++)
				{
					bw.Write(EndOfMissionMessages[i].ToCharArray());
					fs.Position = p + ((i + 1) * 64);
				}

				p = fs.Position;
				for (int i = 0; i < FlightGroupsBriefing.Count; i++)
				{
					fs.Position += 68;
					bw.Write((short)FlightGroupsBriefing[i].PlayerCraft);
					fs.Position = p + ((i + 1) * 90);
				}

				#region Text/Tags
				bw.Write((short)32); //Count
				for (int i = 0; i < 32; i++)
				{
					bw.Write((short)Briefing.BriefingTag[i].Length);
					bw.Write(Briefing.BriefingTag[i].ToCharArray());
				}
				bw.Write((short)32);
				for (int i = 0; i < 32; i++)
				{
					string t = Briefing.RemoveBrackets(Briefing.BriefingString[i]);
					int len = t.Length;
					bw.Write((short)len);
					bw.Write(t.ToCharArray());
					byte[] highlight = Briefing.TranslateStringToHighlight(Briefing.BriefingString[i]);
					bw.Write(highlight);
				}
				#endregion Text/Tags
				fs.SetLength(fs.Position);
				fs.Close();
			}
			catch
			{
				fs?.Close();
				if (writerCreated && backupCreated)
				{
					File.Delete(briefingPath);
					File.Copy(backup, briefingPath);
					File.Delete(backup);
				}
				throw;
			}
			if (backupCreated) File.Delete(backup);
		}

		/// <summary>Saves the mission to a new <see cref="MissionFile.MissionPath"/>.</summary>
		/// <param name="filePath">Full path to the new <see cref="MissionFile.MissionPath"/>.</param>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <paramref name="filePath"/> are denied.</exception>
		public void Save(string filePath)
		{
			MissionPath = filePath;
			Save();
		}

		/// <summary>Checks a CraftType for valid values and adjusts if necessary.</summary>
		/// <param name="craftType">The craft index to check.</param>
		/// <returns>Resultant CraftType index, or <b>255</b> if invalid.</returns>
		public static byte CraftCheck(byte craftType)
		{
			if (craftType > 91) return 255;
			else if (craftType == 77) return 31;    // G/PLT
			else if (craftType == 89) return 10;    // SHPYD
			else if (craftType == 90) return 11;    // REPYD
			else if (craftType == 91) return 39;    // M/SC
			else return craftType;
		}

		/// <summary>Attempts to retrieve the XWI flightgroup from the given BRF flightgroup.</summary>
		/// <param name="BrfIndex">The FG index within the Briefing.</param>
		/// <remarks>In order to convert briefings from X-wing to other platforms, it must grab the necessary coordinate set waypoints.  However, the briefing has its own set of flightgroups.  While the list is usually a match of the XWI, the BRF file-format specs do not guarantee this.  The lists may not be the same size, or items at the same index.</remarks>
		/// <returns>Resultant XWI Flight Group index, or <b>-1</b> if not found.</returns>
		public int GetMatchingXWIFlightGroup(int BrfIndex)
		{
			if (BrfIndex < 0 || BrfIndex >= FlightGroupsBriefing.Count) return -1;

			FlightGroup bfg = FlightGroupsBriefing[BrfIndex];
			FlightGroup fg;
			//If the lists are the same size, check to see if the slots are equal.
			if (FlightGroupsBriefing.Count == FlightGroups.Count)
			{
				fg = FlightGroups[BrfIndex];
				if (bfg.Name == fg.Name)
				{
					if (bfg.IsObjectGroup())
					{
						if (bfg.ObjectType == 25 && fg.CraftType == 2 && fg.Status1 >= 10)  //Compare B-W icon and B-W craft 
							return BrfIndex;

						if (bfg.ObjectType == FlightGroups[BrfIndex].ObjectType)  //Normal object
							return BrfIndex;
					}
					else if (bfg.CraftType == fg.CraftType)
						return BrfIndex;
				}
			}

			//If the direct slot compare didn't match, iterate though all the FGs and compare.
			//Check for name and type match, but also check for matching SP1/CS1 too.  If both exist, prioritize a matching waypoint over a matching name, since it's possible duplicate names/types might exists.
			int nameMatch = -1;
			int waypointMatch = -1;
			for (int i = 0; i < FlightGroups.Count; i++)
			{
				fg = FlightGroups[i];
				bool typeMatch = false;
				if (bfg.Name == fg.Name)  //Check names
				{
					if (bfg.IsObjectGroup())
					{
						if (bfg.ObjectType == 25 && fg.CraftType == 2 && fg.Status1 >= 10)  //Compare B-W icon and B-W craft 
							typeMatch = true;

						if (bfg.ObjectType == fg.ObjectType)  //Normal object
							typeMatch = true;
					}
					else if (bfg.CraftType == fg.CraftType)
						typeMatch = true;

					//For unknown reasons the BRF's CS1 Y-axis is usually inverted from XWI SP1
					if (typeMatch)
					{
						if (nameMatch == -1)
							nameMatch = i;
						if ((fg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawX == bfg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawX)
							&& (fg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawY == -bfg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawY)
							&& (fg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawZ == bfg.Waypoints[(byte)FlightGroup.WaypointIndex.Start1].RawZ))
							waypointMatch = i;
					}
				}
				if (nameMatch >= 0 && waypointMatch >= 0)
					break;
			}
			if (waypointMatch >= 0) return waypointMatch;
			return nameMatch;
		}

		/// <summary>Checks Trigger.Type/Variable or Order.TargetType/Target pairs for values compatible with TIE.</summary>
		/// <remarks>First checks for invalid Types, then runs through allowed values for each Type. Does not verify FlightGroup, CraftWhen, GlobalGroup or Misc.</remarks>
		/// <param name="type">Trigger.Type or Order.TargetType.</param>
		/// <param name="variable">Trigger.Variable or Order.Target, may be updated.</param>
		/// <param name="errorMessage">Error description if found, otherwise empty.</param>
		public static void CheckTarget(byte type, ref byte variable, out string errorMessage)
		{
			errorMessage = "";
			if (type > 9)
			{
				errorMessage = "Type (" + type + ")";
				return;
			}
			// can't check FG
			else if (type == 2)
			{
				byte newCraft = CraftCheck(variable);
				if (newCraft == 255) errorMessage = "CraftType";
				else variable = newCraft;
			}
			else if (type == 3) if (variable > 6) errorMessage = "CraftCategory";
				else if (type == 4) if (variable > 2) errorMessage = "ObjectCategory";
					else if (type == 5) if (variable > 5) errorMessage = "IFF";
						else if (type == 6) if (variable > 39) errorMessage = "Order";
			// don't want to check CraftWhen
			// can't check GlobalGroup
			// don't want to check Misc
			if (errorMessage != "") errorMessage += " (" + variable + ")";
		}
		#endregion public methods

		#region public properties
		/// <summary>Gets or sets the allowed mission duration.</summary>
		public short TimeLimitMinutes = 0;
		/// <summary>Gets or sets the event that occurs when the player dies and Death Star outcomes.</summary>
		/// <remarks>For player destruction, <b>00</b> is Rescued and <b>01</b> is Captured.<br/>
		/// For Death Star outcomes, <b>01</b> is Clear Laser Tower, and <b>05</b> is Hit Exhaust Port.</remarks>
		public short EndEvent = 0;
		/// <summary>RndSeed value.</summary>
		/// <remarks>RndSeed is supposed to be an initializer to the pseudo-random number generator, but is not actually used.</remarks>
		public short RndSeed = 0;
		/// <summary>Gets or sets where the mission takes place.</summary>
		/// <remarks>Value is <b>00</b> for normal space missions, <b>01</b> for the Death Star surface.</remarks>
		public short Location = 0;
		/// <summary>Gets the array accessor for the EoM Messages.</summary>
		public Indexer<string> EndOfMissionMessages => _endOfMissionIndexer;

		/// <summary>Maximum number of craft that can exist at one time in-game.</summary>
		/// <remarks>Value is <b>28</b>.</remarks>
		public const int CraftLimit = 28;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file.</summary>
		/// <remarks>XWING95 is unique in the series in how it uses separate arrays for FlightGroups and ObjectGroups. The native <b>FlightGroup</b> limit is <b>16</b>, ObjectGroup limit is <b>64</b>. For the sake of editing in YOGEME, these groups are abstracted into a single container while at the same time extending these limits for third party support.</remarks>
		public const int FlightGroupLimit = 255;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file.</summary>
		/// <remarks>XWING95 does not have this feature.</remarks>
		public const int MessageLimit = 0;

		/// <summary>Gets or sets the FlightGroups for the mission.</summary>
		/// <remarks>Defaults to one FlightGroup.</remarks>
		public FlightGroupCollection FlightGroups { get; set; }
		/// <summary>Gets or sets the Briefing FlightGroups for the mission.</summary>
		/// <remarks>Defaults to one FlightGroup.</remarks>
		public FlightGroupCollection FlightGroupsBriefing { get; set; }
		/// <summary>Gets or sets the Briefing for the mission.</summary>
		public Briefing Briefing { get; set; }
		#endregion public properties
	}
}