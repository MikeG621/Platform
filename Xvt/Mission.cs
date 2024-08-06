/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 5.0+
 */

/* CHANGELOG
 * [NEW] Format spec implemented
 * [UPD] Briefing events I/O
 * v5.0, 201004
 * [NEW] RndSeed
 * v4.0, 200809
 * [UPD] Message load null term fixed [JB]
 * [UPD] Better Save backup [JB]
 * [UPD] Unknown4 and 5 removed, part of new IFF names [JB]
 * [UPD] Unknown6 renamed to PreventMissionOutcome [JB]
 * v3.1, 200703
 * [UPD] added backup during save
 * v3.0.1, 180919
 * [FIX] Pitch value check during write
 * v3.0, 180903
 * [UPD] Renamed MissionType.MPMelee [JB]
 * [UPD] updated string encodings [JB]
 * [FIX] Departure and Arrival2 R/W [JB]
 * [UPD] Appropriate R/W updates for format updates [JB]
 * [FIX] Mission Succ/Fail/Desc load changed to TrimEnd [JB]
 * [FIX] signature moved to within MissionDescription instead of "outside" format [JB]
 * [FIX] null check on fs.Close() [JB]
 * [NEW] Delete/swap FG helper functions [JB]
 * v2.5, 170107
 * [FIX] Enforced string encodings [JB]
 * [FIX] Craft options [JB]
 * [FIX] Message read length check [JB]
 * [FIX] FG Goal Unks write [JB]
 * [FIX] FG options writing [JB]
 * [FIX] write Message.Color [JB]
 * [FIX] Team write [JB]
 * [UPD] Briefing Teams R/W [JB]
 * v2.4, 160606
 * [FIX] Invert WP.Y at read/write
 * v2.1, 141214
 * [UPD] change to MPL
 * [FIX] Team.EndOfMissionMessageColor load/save
 * v2.0.1, 120814
 * [FIX] Critical load failure (located in briefing)
 * [FIX] Save failure (located in briefing)
 * [FIX] FlightGroup.SpecialCargoCraft load/save
 * v2.0, 120525
 * [NEW] CraftCheck(), CheckTarget()
 * - removed _briefings/_globals/_teams
 * [UPD] class inherits MissionFile
 * [DEL] removed NumFlightGroups, NumMessages
 * [UPD] BoP renamed to IsBop
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Framework for XvT and BoP</summary>
	/// <remarks>This is the primary container object for XvT and BoP mission files</remarks>
	public partial class Mission : MissionFile
	{
		readonly string[] _iff = Strings.IFF;
		string _missionDescription = "";
		string _missionFailed = "";
		string _missionSuccessful = "";

		/// <summary>The types of mission.</summary>
		public enum MissionTypeEnum : byte {
			/// <summary>Standard combat mission.</summary>
			Training,
			/// <summary>Unknown mission type.</summary>
			Unknown,
			/// <summary>Melee (Skirmish) mission.</summary>
			Melee,
			/// <summary>Multiplayer training/campaign mission.</summary>
			MPTraining,
			/// <summary>Multi-team combat engagement mission.</summary>
			MPCombat
		}

		#region constructors
		/// <summary>Default constructor, create a blank mission.</summary>
		public Mission()
		{
			MissionType = MissionTypeEnum.Training;
			initialize();
		}

		/// <summary>Create a new mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		/// <exception cref="FileNotFoundException"><paramref name="filePath"/> does not exist.</exception>
		/// <exception cref="InvalidDataException"><paramref name="filePath"/> is not a XvT or BoP mission file.</exception>
		public Mission(string filePath)
		{
			initialize();
			LoadFromFile(filePath);
		}

		/// <summary>Create a new mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid XvT or BoP mission file.</exception>
		public Mission(FileStream stream)
		{
			initialize();
			LoadFromStream(stream);
		}
		
		void initialize()
		{
			_invalidError = _invalidError.Replace("{0}", "XvT or BoP");
            IFFs = new IffNameIndexer(this);
			FlightGroups = new FlightGroupCollection();
			Messages = new MessageCollection();
			Globals = new GlobalsCollection();
			Teams = new TeamCollection();
			Briefings = new BriefingCollection();
		}
		#endregion

		#region public methods
		/// <summary>Load a mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		/// <exception cref="FileNotFoundException"><paramref name="filePath"/> does not exist.</exception>
		/// <exception cref="InvalidDataException"><paramref name="filePath"/> is not a XvT or BoP mission file.</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid XvT or BoP mission file.</exception>
		public void LoadFromStream(FileStream stream)
		{
			Platform p = GetPlatform(stream);
			if (p != Platform.XvT && p != Platform.BoP) throw new InvalidDataException(_invalidError);
			IsBop = (p == Platform.BoP);
            BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(1252));  //[JB] Changed encoding to windows-1252 (ANSI Latin 1) to ensure proper loading of 8-bit ANSI regardless of the operating system's default code page.
            int i, j;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			#region Platform
			LegacyTimeLimitMin = br.ReadByte();
			LegacyTimeLimitSec = br.ReadByte();
			WinType = br.ReadByte();
			RndSeed = br.ReadByte();
			LegacyRescue = br.ReadByte();
			LegacyAllWayShown = Convert.ToBoolean(br.ReadByte());	// doing this to capture &1 bools instead of >0
			LegacyVars = br.ReadBytes(8);
			for (i = 2; i < 6; i++) IFFs[i] = new string(br.ReadChars(0x14)).Trim('\0');
			stream.Position = 0x64;
			MissionType = (MissionTypeEnum)br.ReadByte();
			GoalsUnimportant = Convert.ToBoolean(br.ReadByte());
			TimeLimitMin = br.ReadByte();
			TimeLimitSec = br.ReadByte();
			stream.Position = 0xA4;
			#endregion
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			byte[] buffer = new byte[64];
			for (i = 0; i < numFlightGroups; i++)
			{
				#region Craft
				FlightGroups[i].Name = new string(br.ReadChars(0x14));  // null-termed
				for (j = 0; j < 4; j++) FlightGroups[i].Roles[j] = new string(br.ReadChars(4));
				stream.Position += 4;
				FlightGroups[i].Cargo = new string(br.ReadChars(0x14)); // null-termed
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(0x14));  // null-termed
				stream.Read(buffer, 0, 0x1D);
				FlightGroups[i].SpecialCargoCraft = buffer[0];
				FlightGroups[i].RandSpecCargo = Convert.ToBoolean(buffer[1]);
				FlightGroups[i].CraftType = buffer[2];
				FlightGroups[i].NumberOfCraft = buffer[3];
				if (!FlightGroups[i].RandSpecCargo)
				{
					if (FlightGroups[i].SpecialCargoCraft >= FlightGroups[i].NumberOfCraft) FlightGroups[i].SpecialCargoCraft = 0;
					else FlightGroups[i].SpecialCargoCraft++;
				}
				FlightGroups[i].Status1 = buffer[4];
				FlightGroups[i].Missile = buffer[5];
				FlightGroups[i].Beam = buffer[6];
				FlightGroups[i].IFF = buffer[7];
				FlightGroups[i].Team = buffer[8];
				FlightGroups[i].AI = buffer[9];
				FlightGroups[i].Markings = buffer[0xA];
				FlightGroups[i].Radio = (FlightGroup.RadioChannel)buffer[0xB];
				FlightGroups[i].Formation = buffer[0xD];
				FlightGroups[i].FormDistance = buffer[0xE];
				FlightGroups[i].GlobalGroup = buffer[0xF];
				FlightGroups[i].NumberOfWaves = (byte)(buffer[0x11] + 1);
				FlightGroups[i].WavesDelay = buffer[0x12];
				FlightGroups[i].StopArrivingWhen = (FlightGroup.WavesEnd)buffer[0x13];
				FlightGroups[i].PlayerNumber = buffer[0x14];
				FlightGroups[i].ArriveOnlyIfHuman = Convert.ToBoolean(buffer[0x15]);
				FlightGroups[i].PlayerCraft = buffer[0x16];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x17] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x18] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x19] * 360 / 0x100);
				FlightGroups[i].LegacyPermaDeathEnabled = Convert.ToBoolean(buffer[0x1A]);
				FlightGroups[i].LegacyPermaDeathID = buffer[0x1B];
				#endregion
				#region Arr/Dep
				stream.Read(buffer, 0, 0x35);
				// This handles bad values that XvtED allows
				if (buffer[0] == 7) buffer[0]--;
				if (buffer[0] > 6) buffer[0] -= 7;
				FlightGroups[i].Difficulty = (BaseFlightGroup.Difficulties)buffer[0];
				for (j = 0; j < 4; j++)
				{
					FlightGroups[i].ArrDepTriggers[0][j] = buffer[1 + j];   // Arr1...
					FlightGroups[i].ArrDepTriggers[1][j] = buffer[5 + j];
					FlightGroups[i].ArrDepTriggers[2][j] = buffer[0xC + j];
					FlightGroups[i].ArrDepTriggers[3][j] = buffer[0x10 + j];
					FlightGroups[i].ArrDepTriggers[4][j] = buffer[0x1B + j];    // Dep1...
					FlightGroups[i].ArrDepTriggers[5][j] = buffer[0x1F + j];
				}
				FlightGroups[i].ArrDepAO[0] = Convert.ToBoolean(buffer[0xB]);
				FlightGroups[i].ArrDepAO[1] = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].ArrDepAO[2] = Convert.ToBoolean(buffer[0x17]);
				FlightGroups[i].RandomArrivalDelayMinutes = buffer[0x18];
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x19];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x1A];
				FlightGroups[i].ArrDepAO[3] = Convert.ToBoolean(buffer[0x25]);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x26];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x27];
				FlightGroups[i].AbortTrigger = buffer[0x28];
				FlightGroups[i].RandomArrivalDelaySeconds = buffer[0x29];
				FlightGroups[i].ArrivalMothership = buffer[0x2D];
				FlightGroups[i].ArriveViaMothership = Convert.ToBoolean(buffer[0x2E]);    // false = hyper, true = mothership
				FlightGroups[i].DepartureMothership = buffer[0x2F];       //[JB] Fixed byte order.
				FlightGroups[i].DepartViaMothership = Convert.ToBoolean(buffer[0x30]);
				FlightGroups[i].AlternateMothership = buffer[0x31];
				FlightGroups[i].AlternateMothershipUsed = Convert.ToBoolean(buffer[0x32]);
				FlightGroups[i].CapturedDepartureMothership = buffer[0x33];
				FlightGroups[i].CapturedDepartViaMothership = Convert.ToBoolean(buffer[0x34]);
				#endregion
				#region Orders
				for (j = 0; j < 4; j++)
				{
					stream.Read(buffer, 0, 0x13);
					for (int h = 0; h < 0x13; h++) FlightGroups[i].Orders[j][h] = buffer[h];
					FlightGroups[i].Orders[j].Designation = new string(br.ReadChars(16));   // null-termed
					stream.Position += 0x2F;
				}
				stream.Read(buffer, 0, 0xB);
				for (j = 0; j < 4; j++)
				{
					FlightGroups[i].SkipToOrder4Trigger[0][j] = buffer[j];
					FlightGroups[i].SkipToOrder4Trigger[1][j] = buffer[4 + j];
				}
				FlightGroups[i].SkipToO4T1OrT2 = Convert.ToBoolean(buffer[0xA]);
				#endregion
				#region Goals
				for (j = 0; j < 8; j++)
				{
					FlightGroups[i].Goals[j] = new FlightGroup.Goal(br.ReadBytes(0xF));
					stream.Position += 0x3F;
				}
				stream.Position++;
				#endregion
				for (j = 0; j < 4; j++) for (int k = 0; k < 22; k++) FlightGroups[i].Waypoints[k][j] = (short)(br.ReadInt16() * (j == 1 ? -1 : 1));
				#region Options/Other
				stream.Position += 10;
				stream.Read(buffer, 0, 0x10);
				FlightGroups[i].PreventCraftNumbering = Convert.ToBoolean(buffer[0]);
				FlightGroups[i].DepartureClockMinutes = buffer[1];
				FlightGroups[i].DepartureClockSeconds = buffer[2];
				FlightGroups[i].Countermeasures = (FlightGroup.CounterTypes)buffer[3];
				FlightGroups[i].ExplosionTime = buffer[4];
				FlightGroups[i].Status2 = buffer[5];
				FlightGroups[i].GlobalUnit = buffer[6];
				FlightGroups[i].Handicap = buffer[0xF];
				//[JB] Revised loading optional weapons to support Ion Pulse, Energy Beam, Cluster Mine.
				for (j = 0; j < 8; j++)  //Warhead section, 8 bytes total.
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 9) { FlightGroups[i].OptLoadout[x] = true; FlightGroups[i].OptLoadout[0] = false; }
				}
				for (j = 0; j < 4; j++)  //Beam section, 6 bytes total (reading only 4)
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 5) { FlightGroups[i].OptLoadout[9 + x] = true; FlightGroups[i].OptLoadout[9] = false; }
				}
				stream.Position += 2;    //skip extra beams
				for (j = 0; j < 3; j++)  //Countermeasure section, 4 bytes total (reading only 3)
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 4) { FlightGroups[i].OptLoadout[14 + x] = true; FlightGroups[i].OptLoadout[14] = false; }
				}
				stream.Position++;       //skip extra countermeasure
				FlightGroups[i].OptCraftCategory = (FlightGroup.OptionalCraftCategory)br.ReadByte();
				stream.Read(buffer, 0, 0x1E);
				for (int k = 0; k < 10; k++)
				{
					FlightGroups[i].OptCraft[k].CraftType = buffer[k];
					FlightGroups[i].OptCraft[k].NumberOfCraft = buffer[k + 10];
					FlightGroups[i].OptCraft[k].NumberOfWaves = buffer[k + 20];
				}
				stream.Position++;
				#endregion
			}
			#endregion
			#region Messages
			if (numMessages != 0)
			{
				Messages = new MessageCollection(numMessages);
				for (i=0;i<numMessages;i++)
				{
					stream.Position += 2;
					Messages[i].MessageString = new string(br.ReadChars(64));
					if (Messages[i].MessageString.IndexOf('\0') != -1) Messages[i].MessageString = Messages[i].MessageString.Substring(0, Messages[i].MessageString.IndexOf('\0'));
					Messages[i].Color = 0;
                    if (Messages[i].MessageString.Length > 0)
					{
						char c = Messages[i].MessageString[0];
						if (c >= '1' && c <= '3')
						{
							Messages[i].Color = (byte)(c - '0');
							Messages[i].MessageString = Messages[i].MessageString.Substring(1);
						}
					}
					stream.Read(buffer, 0, 0x20);
					for (j=0;j<10;j++) Messages[i].SentToTeam[j] = Convert.ToBoolean(buffer[j]);
					for (j=0;j<4;j++)
					{
						Messages[i].Triggers[0][j] = buffer[0xA+j];
						Messages[i].Triggers[1][j] = buffer[0xE+j];
						Messages[i].Triggers[2][j] = buffer[0x15+j];
						Messages[i].Triggers[3][j] = buffer[0x19+j];
					}
					Messages[i].T1OrT2 = Convert.ToBoolean(buffer[0x14]);
					Messages[i].T3OrT4 = Convert.ToBoolean(buffer[0x1F]);
					Messages[i].Note = new string(br.ReadChars(16)).Trim('\0');	// null-termed
					Messages[i].Delay = br.ReadByte();
					Messages[i].T12OrT34 = Convert.ToBoolean(br.ReadByte());
				}
			}
			else Messages.Clear();
			#endregion
			#region Globals
			Globals.ClearAll();
			for (i = 0; i < 10; i++)
			{
				var count = br.ReadInt16(); // this should always be 3, but just in case...
				for (int k = 0; k < count; k++)
				{
					stream.Read(buffer, 0, 8);
					for (j = 0; j < 4; j++)
					{
						Globals[i].Goals[k].Triggers[0][j] = buffer[j];
						Globals[i].Goals[k].Triggers[1][j] = buffer[j + 4];
					}
					stream.Position += 2;
					Globals[i].Goals[k].T1OrT2 = br.ReadBoolean();
					stream.Read(buffer, 0, 8);
					for (j = 0; j < 4; j++)
					{
						Globals[i].Goals[k].Triggers[2][j] = buffer[j];
						Globals[i].Goals[k].Triggers[3][j] = buffer[j + 4];
					}
					stream.Position += 2;
					Globals[i].Goals[k].T3OrT4 = br.ReadBoolean();
					Globals[i].Goals[k].Name = new string(br.ReadChars(16));
					Globals[i].Goals[k].Version = br.ReadByte();
					Globals[i].Goals[k].T12OrT34 = br.ReadBoolean();
					Globals[i].Goals[k].Delay = br.ReadByte();
					Globals[i].Goals[k].RawPoints = br.ReadSByte();
				}
			}
			#endregion
			#region Teams
			Teams.ClearAll();
			for (i = 0; i < 10; i++)
			{
				stream.Position += 2;
				Teams[i].Name = new string(br.ReadChars(0x10)); // null-termed
				stream.Position += 8;
				for (j = 0; j < 10; j++) Teams[i].AlliedWithTeam[j] = br.ReadBoolean();
				for (j = 0; j < 6; j++)
				{
					Teams[i].EndOfMissionMessages[j] = new string(br.ReadChars(0x40));
					if (Teams[i].EndOfMissionMessages[j] != "")
					{
						char c = Teams[i].EndOfMissionMessages[j][0];
						if (c == '1' || c == '2' || c == '3')
						{
							Teams[i].EndOfMissionMessageColor[j] = byte.Parse(c.ToString());
							Teams[i].EndOfMissionMessages[j] = Teams[i].EndOfMissionMessages[j].Substring(1);
						}
					}
				}
				stream.Position += 0x43;
			}
			#endregion
			#region Briefing
			Briefings.ClearAll();
			for (i = 0; i < 8; i++)
			{
				Briefings[i].Length = br.ReadInt16();
				stream.Position += 6;   // CurrentTime, StartLength, EventsLength
				Briefings[i].Tile = br.ReadInt16();
				byte[] rawEvents = br.ReadBytes(Briefing.EventQuantityLimit * 4);
				Briefings[i].Events = new BaseBriefing.EventCollection(Platform.XvT, rawEvents);
				stream.Read(buffer, 0, 0xA);
				Buffer.BlockCopy(buffer, 0, Briefings[i].Team, 0, 0xA);
				for (j = 0; j < 32; j++)
				{
					int k = br.ReadInt16();
					if (k > 0) Briefings[i].BriefingTag[j] = new string(br.ReadChars(k)).Trim('\0');    // shouldn't need the trim
				}
				for (j = 0; j < 32; j++)
				{
					int k = br.ReadInt16();
					if (k > 0) Briefings[i].BriefingString[j] = new string(br.ReadChars(k)).Trim('\0');
				}
			}
			#endregion
			#region FG goal strings
			for (i=0;i<FlightGroups.Count;i++)
				for (j=0;j<8;j++)
				{
					FlightGroups[i].Goals[j].IncompleteText = new string(br.ReadChars(0x40));
					FlightGroups[i].Goals[j].CompleteText = new string(br.ReadChars(0x40));
					FlightGroups[i].Goals[j].FailedText = new string(br.ReadChars(0x40));
				}
			#endregion
			#region Globals strings
			for (i = 0; i < 10; i++)	// Team
			{
				for (j = 0; j < 12; j++)	// Goal * Trigger
				{
					for (int k = 0; k < 3; k++)	// State
					{
							if (j >= 8 && k == 0) { stream.Position += 0x40; continue; }	// skip Sec Inc
							if (j >= 4 && k == 2) { stream.Position += 0x40; continue; }	// skip Prev & Sec Fail
							Globals[i].Goals[j / 4].Triggers[j % 4].GoalStrings[k] = new string(br.ReadChars(0x40));
					}
				}
				stream.Position += 0xC00;
			}
			#endregion
			#region Debriefs
			if (IsBop)
			{
                _missionSuccessful = new string(br.ReadChars(0x1000)).TrimEnd('\0');
                _missionFailed = new string(br.ReadChars(0x1000)).TrimEnd('\0');
				_missionDescription = new string(br.ReadChars(0x1000)).TrimEnd('\0');  //[JB] Only trim from end, because trimming from the start might return YOGEME's signature embedded into the end of the description, or any leftover garbage data that is normally ignored by a null terminator.
			}
			else _missionDescription = new string(br.ReadChars(0x400)).TrimEnd('\0');
			#endregion
			MissionPath = stream.Name;
		}

		/// <summary>Save the mission with the default path.</summary>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <see cref="MissionFile.MissionPath"/> are denied.</exception>
		public void Save()
		{
			//[JB] Rewrote the backup logic.  See the TIE Save() function for comments.
			if (File.Exists(MissionPath) && (File.GetAttributes(MissionPath) & FileAttributes.ReadOnly) != 0) throw new UnauthorizedAccessException("Cannot save, existing file is read-only.");

			FileStream fs = null;
			string backup = MissionPath.ToLower().Replace(".tie", "_tie.bak");
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
				BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.GetEncoding(1252));  //[JB] Changed encoding to windows-1252 (ANSI Latin 1) to ensure proper loading of 8-bit ANSI regardless of the operating system's default code page.
				writerCreated = true;
				int i;
				long p;
				#region Platform
				if (IsBop) bw.Write((short)14);
				else bw.Write((short)12);
				bw.Write((short)FlightGroups.Count);
				bw.Write((short)Messages.Count);
				bw.Write(LegacyTimeLimitMin);
				bw.Write(LegacyTimeLimitSec);
				bw.Write(WinType);
				bw.Write(RndSeed);
				bw.Write(LegacyRescue);
				bw.Write(LegacyAllWayShown);
				bw.Write(LegacyVars);
				for (i = 2; i < 6; i++)
				{
					p = fs.Position;
					bw.Write(_iff[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x14;
				}
				fs.Position = 0x64;
				bw.Write((byte)MissionType);
				bw.Write(GoalsUnimportant);
				bw.Write(TimeLimitMin);
				bw.Write(TimeLimitSec);
				fs.Position = 0xA4;
				#endregion
				#region FlightGroups
				for (i = 0; i < FlightGroups.Count; i++)
				{
					p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x14;
					for (j = 0; j < 4; j++)
					{
						string s = FlightGroups[i].Roles[j];
						if (FlightGroups[i].Roles[j] != "") bw.Write(s.Length > 4 ? s.Substring(0, 4).ToCharArray() : s.ToCharArray());
						else bw.Write(0);
					}
					fs.Position = p + 0x28;
					bw.Write(FlightGroups[i].Cargo.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x3C;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x50;
					if (FlightGroups[i].SpecialCargoCraft == 0) bw.Write(FlightGroups[i].NumberOfCraft);
					else bw.Write((byte)(FlightGroups[i].SpecialCargoCraft - 1));
					bw.Write(FlightGroups[i].RandSpecCargo);
					bw.Write(FlightGroups[i].CraftType);
					bw.Write(FlightGroups[i].NumberOfCraft);
					bw.Write(FlightGroups[i].Status1);
					bw.Write(FlightGroups[i].Missile);
					bw.Write(FlightGroups[i].Beam);
					bw.Write(FlightGroups[i].IFF);
					bw.Write(FlightGroups[i].Team);
					bw.Write(FlightGroups[i].AI);
					bw.Write(FlightGroups[i].Markings);
					bw.Write((byte)FlightGroups[i].Radio);
					fs.Position++;
					bw.Write(FlightGroups[i].Formation);
					bw.Write(FlightGroups[i].FormDistance);
					bw.Write(FlightGroups[i].GlobalGroup);
					fs.Position++;
					bw.Write((byte)(FlightGroups[i].NumberOfWaves - 1));
					bw.Write(FlightGroups[i].WavesDelay);
					bw.Write((byte)FlightGroups[i].StopArrivingWhen);
					bw.Write(FlightGroups[i].PlayerNumber);
					bw.Write(FlightGroups[i].ArriveOnlyIfHuman);
					bw.Write(FlightGroups[i].PlayerCraft);
					bw.Write((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					bw.Write((byte)((FlightGroups[i].Pitch >= 90 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					bw.Write((byte)(FlightGroups[i].Roll * 0x100 / 360));
					bw.Write(FlightGroups[i].LegacyPermaDeathEnabled);
					bw.Write(FlightGroups[i].LegacyPermaDeathID);
					fs.Position = p + 0x6D;
					#endregion
					#region Arr/Dep
					bw.Write((byte)FlightGroups[i].Difficulty);
					bw.Write(FlightGroups[i].ArrDepTriggers[0].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[1].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[0]);
					bw.Write(FlightGroups[i].ArrDepTriggers[2].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[3].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[1]);
					bw.Write(FlightGroups[i].ArrDepAO[2]);
					bw.Write(FlightGroups[i].RandomArrivalDelayMinutes);
					bw.Write(FlightGroups[i].ArrivalDelayMinutes);
					bw.Write(FlightGroups[i].ArrivalDelaySeconds);
					bw.Write(FlightGroups[i].ArrDepTriggers[4].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[5].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[3]);
					bw.Write(FlightGroups[i].DepartureTimerMinutes);
					bw.Write(FlightGroups[i].DepartureTimerSeconds);
					bw.Write(FlightGroups[i].AbortTrigger);
					bw.Write(FlightGroups[i].RandomArrivalDelaySeconds);
					fs.Position += 3;
					fs.WriteByte(FlightGroups[i].ArrivalMothership);
					bw.Write(FlightGroups[i].ArriveViaMothership);
					fs.WriteByte(FlightGroups[i].DepartureMothership);
					bw.Write(FlightGroups[i].DepartViaMothership);
					fs.WriteByte(FlightGroups[i].AlternateMothership);
					bw.Write(FlightGroups[i].AlternateMothershipUsed);
					fs.WriteByte(FlightGroups[i].CapturedDepartureMothership);
					bw.Write(FlightGroups[i].CapturedDepartViaMothership);
					#endregion
					#region Orders
					for (j = 0; j < 4; j++)
					{
						bw.Write(FlightGroups[i].Orders[j].GetBytes());
						bw.Write(FlightGroups[i].Orders[j].Designation.ToCharArray()); bw.Write('\0');
						fs.Position = p + 0xA2 + ((j + 1) * 0x52);
					}
					bw.Write(FlightGroups[i].SkipToOrder4Trigger[0].GetBytes());
					bw.Write(FlightGroups[i].SkipToOrder4Trigger[1].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].SkipToO4T1OrT2);
					#endregion
					#region Goals
					for (j = 0; j < 8; j++)
					{
						bw.Write(FlightGroups[i].Goals[j].GetBytes());
						fs.Position = p + 0x1F5 + ((j + 1) * 0x4E);
					}
					fs.Position++;
					#endregion
					for (j = 0; j < 4; j++) for (int k = 0; k < 22; k++) bw.Write((short)(FlightGroups[i].Waypoints[k][j] * (j == 1 ? -1 : 1)));
					#region Options/Other
					fs.Position += 10;
					bw.Write(FlightGroups[i].PreventCraftNumbering);
					bw.Write(FlightGroups[i].DepartureClockMinutes);
					bw.Write(FlightGroups[i].DepartureClockSeconds);
					bw.Write((byte)FlightGroups[i].Countermeasures);
					bw.Write(FlightGroups[i].ExplosionTime);
					bw.Write(FlightGroups[i].Status2);
					bw.Write(FlightGroups[i].GlobalUnit);
					fs.Position += 8;
					bw.Write(FlightGroups[i].Handicap);
					//[JB] The old code iterated through the array and wrote the values, but it didn't work properly.  
					//The list of options must not contain any gaps of 00.  
					byte[] optBuff = new byte[18]; //One array to store everything so we don't need to zero anything between use  
					int oi = 0;
					for (j = 1; j < 9; j++) if (FlightGroups[i].OptLoadout[j]) optBuff[oi++] = (byte)j;
					bw.Write(optBuff, 0, 8);  //Warheads  
					oi = 9;
					for (j = 1; j < 5; j++) if (FlightGroups[i].OptLoadout[j + 9]) optBuff[oi++] = (byte)j;
					bw.Write(optBuff, 9, 4);  //Beams  
					fs.Position += 2; //Empty space  
					oi = 14;
					for (j = 1; j < 4; j++) if (FlightGroups[i].OptLoadout[j + 14]) optBuff[oi++] = (byte)j;
					bw.Write(optBuff, 14, 3); //Countermeasures  
					fs.Position++;    //Empty space 
					bw.Write((byte)FlightGroups[i].OptCraftCategory);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].CraftType);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].NumberOfCraft);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].NumberOfWaves);
					fs.Position++;
					#endregion
				}
				#endregion
				#region Messages
				for (i = 0; i < Messages.Count; i++)
				{
					p = fs.Position;
					bw.Write((short)i);
					if (Messages[i].Color != 0) bw.Write((byte)(Messages[i].Color + 0x30));
					bw.Write(Messages[i].MessageString.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x42;
					for (int j = 0; j < Messages[i].SentToTeam.Length; j++) bw.Write(Messages[i].SentToTeam[j]);
					bw.Write(Messages[i].Triggers[0].GetBytes());
					bw.Write(Messages[i].Triggers[1].GetBytes());
					fs.Position += 2;
					bw.Write(Messages[i].T1OrT2);
					bw.Write(Messages[i].Triggers[2].GetBytes());
					bw.Write(Messages[i].Triggers[3].GetBytes());
					fs.Position += 2;
					bw.Write(Messages[i].T3OrT4);
					bw.Write(Messages[i].Note.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x72;
					bw.Write(Messages[i].Delay);
					bw.Write(Messages[i].T12OrT34);
				}
				#endregion
				#region Globals
				for (i = 0; i < 10; i++)
				{
					bw.Write((short)3);
					for (int j = 0; j < 3; j++)
					{
						p = fs.Position;
						bw.Write(Globals[i].Goals[j].Triggers[0].GoalTrigger.GetBytes());
						bw.Write(Globals[i].Goals[j].Triggers[1].GoalTrigger.GetBytes());
						fs.Position += 2;
						bw.Write(Globals[i].Goals[j].T1OrT2);
						bw.Write(Globals[i].Goals[j].Triggers[2].GoalTrigger.GetBytes());
						bw.Write(Globals[i].Goals[j].Triggers[3].GoalTrigger.GetBytes());
						fs.Position += 2;
						bw.Write(Globals[i].Goals[j].T3OrT4);
						bw.Write(Globals[i].Goals[j].Name.ToCharArray()); bw.Write('\0');
						fs.Position = p + 0x26;
						bw.Write(Globals[i].Goals[j].Version);
						bw.Write(Globals[i].Goals[j].T12OrT34);
						bw.Write(Globals[i].Goals[j].Delay);
						bw.Write(Globals[i].Goals[j].RawPoints);
					}
				}
				#endregion
				#region Teams
				for (i = 0; i < 10; i++)
				{
					p = fs.Position;
					bw.Write((short)1);
					bw.Write(Teams[i].Name.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x1A;
					for (int j = 0; j < 10; j++) bw.Write(Teams[i].AlliedWithTeam[j]);
					for (int j = 0; j < 6; j++)
					{
						if (Teams[i].EndOfMissionMessageColor[j] != 0) bw.Write(Convert.ToByte(Teams[i].EndOfMissionMessageColor[j] + 48));
						bw.Write(Teams[i].EndOfMissionMessages[j].ToCharArray()); bw.Write('\0');
						fs.Position = p + 0x24 + (j + 1) * 0x40;
					}
					fs.Position = p + 0x1E7;
				}
				#endregion
				#region Briefing
				for (i = 0; i < 8; i++)
				{
					bw.Write(Briefings[i].Length);
					bw.Write(Briefings[i].CurrentTime);
					bw.Write(Briefings[i].StartLength);
					bw.Write(Briefings[i].EventsLength);
					bw.Write(Briefings[i].Tile);
					byte[] briefBuffer = new byte[Briefing.EventQuantityLimit * 2];
					Buffer.BlockCopy(Briefings[i].Events.GetArray(), 0, briefBuffer, 0, Briefings[i].Events.Length * 2);
					bw.Write(briefBuffer);
					for (int j = 0; j < 10; j++) bw.Write(Briefings[i].Team[j]);
					for (int j = 0; j < 32; j++)
					{
						bw.Write((short)Briefings[i].BriefingTag[j].Length);
						if (Briefings[i].BriefingTag[j].Length != 0) bw.Write(Briefings[i].BriefingTag[j].ToCharArray());
					}
					for (int j = 0; j < 32; j++)
					{
						bw.Write((short)Briefings[i].BriefingString[j].Length);
						if (Briefings[i].BriefingString[j].Length != 0) bw.Write(Briefings[i].BriefingString[j].ToCharArray());
					}
				}
				#endregion
				#region FG Goal Strings
				for (i = 0; i < FlightGroups.Count; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							p = fs.Position;
							if (k == 0) bw.Write(FlightGroups[i].Goals[j].IncompleteText.ToCharArray());
							else if (k == 1) bw.Write(FlightGroups[i].Goals[j].CompleteText.ToCharArray());
							else bw.Write(FlightGroups[i].Goals[j].FailedText.ToCharArray());
							bw.Write('\0');
							fs.Position = p + 0x40;
						}
					}
				}
				#endregion
				#region Global Goal Strings
				for (i = 0; i < 10; i++)
				{
					for (int j = 0; j < 12; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							if (j >= 8 && k == 0) { fs.Position += 0x40; continue; }    // skip Sec Inc
							if (j >= 4 && k == 2) { fs.Position += 0x40; continue; }    // skip Prev & Sec Fail
							p = fs.Position;
							bw.Write(Globals[i].Goals[j / 4].Triggers[j % 4].GoalStrings[k].ToCharArray()); bw.Write('\0');
							fs.Position = p + 0x40;
						}
					}
					fs.Position += 0xC00;
				}
				#endregion
				#region Debriefs
				p = fs.Position;
				if (IsBop)
				{
					bw.Write(_missionSuccessful.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x1000;
					bw.Write(_missionFailed.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x2000;
					bw.Write(_missionDescription.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x3000;
				}
				else
				{
					bw.Write(_missionDescription.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x400;
				}

				//[JB] Embed YOGEME signature into the empty description space (appending bytes would disrupt the text in game).  The description must have room for 3 bytes (null terminater for the briefing text, then the 2 byte signature)
				if ((IsBop && _missionDescription.Length < 0x1000 - 3) || (!IsBop && _missionDescription.Length < 0x400 - 3))
				{
					fs.Position -= 2;
					bw.Write((short)0x2106);
				}

				fs.SetLength(fs.Position);
				#endregion
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
			if (backupCreated)
				File.Delete(backup);
		}

		/// <summary>Save the mission to a new location.</summary>
		/// <param name="filePath">Full path to the new file location.</param>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <paramref name="filePath"/> are denied.</exception>
		public void Save(string filePath)
		{
			MissionPath = filePath;
			Save();
		}
		
		/// <summary>Checks a CraftType for valid values and adjusts if necessary.</summary>
		/// <param name="craftType">The craft index to check.</param>
		/// <returns>Validated craft type, otherwise <b>255</b>.</returns>
		public static byte CraftCheck(byte craftType)
		{
			if (craftType > 91) return 255;
			else return craftType;
		}
		
		/// <summary>Checks <see cref="Trigger"/>.Type/Variable or <see cref="FlightGroup.Order"/>.TargetType/Target pairs for values compatible with TIE.</summary>
		/// <remarks>First checks for invalid Types, then runs through allows values for each Type. Does not verify FlightGroup, CraftWhen, GlobalGroup or GlobalUnit.</remarks>
		/// <param name="type">Trigger.Type or Order.TargetType.</param>
		/// <param name="variable">Trigger.Variable or Order.Target, may be updated.</param>
		/// <param name="errorMessage">Error description if found, otherwise "".</param>
		public static void CheckTarget(byte type, ref byte variable, out string errorMessage)
		{
			errorMessage = "";
			if (type > (byte)Trigger.TypeList.AILevel)
			{
				errorMessage = "Type (" + type + ")";
				return;
			}
			// can't check FG
			else if (type == (byte)Trigger.TypeList.ShipType)
			{
				byte newCraft = CraftCheck(variable);
				if (newCraft == 255) errorMessage = "CraftType";
				else variable = newCraft;
			}
			else if (type == (byte)Trigger.TypeList.ShipClass && variable > 6) errorMessage = "CraftCategory";
			else if (type == (byte)Trigger.TypeList.ObjectType && variable > 2) errorMessage = "ObjectCategory";
			else if (type == (byte)Trigger.TypeList.IFF && variable > 5) errorMessage = "IFF";
			else if (type == (byte)Trigger.TypeList.ShipOrders && variable > 39) errorMessage = "Order";
			// don't want to check CraftWhen
			// can't check GG
			else if (type == (byte)Trigger.TypeList.Team || type == (byte)Trigger.TypeList.NotTeam) if (variable > 9) errorMessage = "Team";
			// can't check GU
			if (errorMessage != "") errorMessage += " (" + variable + ")";
		}

        /// <summary>Deletes a Flight Group, performing all necessary cleanup to avoid broken indexes.</summary>
		/// <param name="fgIndex">The FG index to remove.</param>
        /// <remarks>Propagates throughout all members which contain Flight Group indexes.</remarks>
        /// <returns>Index of the next available Flight Group.</returns>
        public int DeleteFG(int fgIndex)
        {
            if (fgIndex < 0 || fgIndex >= FlightGroups.Count)
                return 0;  //If for some reason this is out of range, don't do anything and return selection to first item.

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Globals.Goal.Trigger trig in goal.Triggers)
                        trig.GoalTrigger.TransformFGReferences(fgIndex, -1, false);

            foreach (Message msg in Messages)
                foreach (Trigger trig in msg.Triggers)
                    trig.TransformFGReferences(fgIndex, -1, true);

            foreach (Briefing b in Briefings)
                b.TransformFGReferences(fgIndex, -1);

            foreach(FlightGroup fg in FlightGroups)
                fg.TransformFGReferences(fgIndex, -1);

            return FlightGroups.RemoveAt(fgIndex);  //This handles all the cleanup within the FlightGroupCollection itself.
        }

        /// <summary>Swaps two FlightGroups.</summary>
		/// <param name="srcIndex">The original FG index.</param>
		/// <param name="dstIndex">The new FG index.</param>
        /// <remarks>Automatically performs bounds checking and adjusts all Flight Group indexes to prevent broken indexes in triggers, orders, etc.</remarks>
        /// <returns>Returns <b>true</b> if an adjustment was performed, <b>false</b> if index validation failed.</returns>
        public bool SwapFG(int srcIndex, int dstIndex)
        {
            if ((srcIndex < 0 || srcIndex >= FlightGroups.Count) || (dstIndex < 0 || dstIndex >= FlightGroups.Count) || (srcIndex == dstIndex)) return false;

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Globals.Goal.Trigger trig in goal.Triggers)
                        trig.GoalTrigger.SwapFGReferences(srcIndex, dstIndex);

            foreach (Message msg in Messages)
                foreach (Trigger trig in msg.Triggers)
                    trig.SwapFGReferences(srcIndex, dstIndex);

            foreach(Briefing b in Briefings)
                b.SwapFGReferences(srcIndex, dstIndex);

            foreach (FlightGroup fg in FlightGroups)
            {
                fg.TransformFGReferences(dstIndex, 255);
                fg.TransformFGReferences(srcIndex, dstIndex);
                fg.TransformFGReferences(255, srcIndex);
            }
            FlightGroup temp = FlightGroups[srcIndex];
            FlightGroups[srcIndex] = FlightGroups[dstIndex];
            FlightGroups[dstIndex] = temp;

            return true;
        }
		#endregion public methods

		#region public properties
		/// <summary>Gets the array accessor for the IFF names.</summary>
		public IffNameIndexer IFFs { get; private set; }
		/// <summary>Maximum number of craft that can exist at one time in-game.</summary>
		public const int CraftLimit = 32;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file.</summary>
		public const int FlightGroupLimit = 48;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file.</summary>
		public const int MessageLimit = 64;
		
		/// <summary>Gets or sets the mission platform.</summary>
		/// <remarks><b>true</b> for Balance of Power.</remarks>
		public bool IsBop { get; set; }
		/// <summary>No effect.</summary>
		public byte LegacyTimeLimitMin { get; set; }
		/// <summary>No effect.</summary>
		public byte LegacyTimeLimitSec { get; set; }
		/// <summary>Unknown if this is functional.</summary>
		/// <remarks>Defaults to <b>1</b>.</remarks>
		public byte WinType { get; set; } = 1;
		/// <summary>Seeds the random number generator that is responsible for deciding backdrops and asteroid positions.</summary>
		public byte RndSeed { get; set; }
		/// <summary>Probably no effect.</summary>
		public byte LegacyRescue { get; set; }
		/// <summary>Probably no effect.</summary>
		public bool LegacyAllWayShown { get; set; }
		/// <summary>Probably no effect.</summary>
		public byte[] LegacyVars { get; private set; } = new byte[8];
		/// <summary>Gets or sets the category the mission belongs to.</summary>
		public MissionTypeEnum MissionType { get; set; }
		/// <summary>Gets or sets whether a mission is not allowed to be completed.</summary>
		/// <remarks>If enabled, victory or failure is not possible, a special condition used in some melee scenarios.</remarks>
		public bool GoalsUnimportant { get; set; }
		/// <summary>Gets or sets the minutes value of the time limit.</summary>
		/// <remarks>Can be used in conjunction with <see cref="TimeLimitSec"/>.</remarks>
		public byte TimeLimitMin { get; set; }
		/// <summary>Gets or sets the seconds value of the time limit.</summary>
		/// <remarks>Can be used in conjunction with <see cref="TimeLimitMin"/>.</remarks>
		public byte TimeLimitSec { get; set; }
		/// <summary>Gets or sets the FlightGroups for the mission.</summary>
		/// <remarks>Defaults to one FlightGroup.</remarks>
		public FlightGroupCollection FlightGroups { get; set; }
		/// <summary>Gets or sets the In-Flight Messages for the mission.</summary>
		/// <remarks>Defaults to zero messages.</remarks>
		public MessageCollection Messages { get; set; }
		/// <summary>Gets or sets the Global Goals for the mission.</summary>
		public GlobalsCollection Globals { get; set; }
		/// <summary>Gets or sets the Teams for the mission.</summary>
		public TeamCollection Teams { get; set; }
		/// <summary>Gets or sets the Briefings for the mission.</summary>
		public BriefingCollection Briefings { get; set; }
		/// <summary>Gets or sets the summary of the mission.</summary>
		/// <remarks>1023 char limit for XvT, 4095 char limit for BoP.</remarks>
		public string MissionDescription
		{
			get => _missionDescription.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionDescription = StringFunctions.GetTrimmed(s, (IsBop ? 0x0FFF : 0x3FF));
			}
		}
		/// <summary>Gets or sets the BoP Debriefing text.</summary>
		/// <remarks>4095 char limit.</remarks>
		public string MissionFailed
		{
			get => _missionFailed.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionFailed = StringFunctions.GetTrimmed(s, 0x0FFF);
			}
		}
		/// <summary>Gets or sets the BoP Debriefing text.</summary>
		/// <remarks>4095 char limit.</remarks>
		public string MissionSuccessful
		{
			get => _missionSuccessful.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionSuccessful = StringFunctions.GetTrimmed(s, 0x0FFF);
			}
		}
		#endregion public properties
	}
}