/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.7+
 */

/* CHANGELOG
 * [UPD] changed string encoding [JB]
 * [UPD] appropriate updates to read/write due to format update [JB]
 * [UPD] mission strings changed to TrimEnd on read [JB]
 * [UPD] moved signature to end of description string [JB]
 * [FIX] added null check to fs.Close [JB]
 * [NEW] helper functions for delete/swap FG/Mess [JB]
 * [NEW] GetDelaySeconds [JB]
 * [UPD] FlightGroupLimit increased to 192 [JB]
 * v2.7, 180509
 * [UPD] FlightgroupLimit changed to 132, this is post-SBD install
 * v2.6.2, 180224
 * [FIX YOGEME\#16] added missing Y inversion on Order Waypoints
 * v2.5, 170107
 * [FIX] Unk3 init to hex [JB]
 * [UPD] enforce string encodings [JB]
 * [FIX] FG Options [JB]
 * v2.4, 160606
 * [FIX] Invert WP.Y at read/write
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0.1, 120814
 * [FIX] Bug in Save() that caused infinite loop and ballooning filesize
 * [FIX] FlightGroup.SpecialCargoCraft load/save
 * v2.0, 120525
 * [NEW] LogoEnum, _initialize()
 * [NEW] GlobalGroups, Regions, Iffs are now Indexer<string>
 * [UPD] class inherits MissionFile
 * [DEL] NumFlightGroups, NumMessages
 * [UPD] Hangar renamed to MissionType
 * [NEW] *Notes
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Framework for XWA</summary>
	/// <remarks>This is the primary container object for XWA mission files</remarks>
	public partial class Mission : MissionFile
	{
		string[] _iff = Strings.IFF;
		string[] _region = new string[4];
		GlobCarg[] _globalCargo = new GlobCarg[16];
		string[] _globalGroup = new string[16];
		string _missionDescription = "#";
		string _missionFailed = "#";
		string _missionSuccessful = "";
		string _missionNote = "";
		string _descriptionNote = "";
		string _failedNote = "";
		string _successfulNote = "";
		Indexer<string> _globalGroupNameIndexer;
		Indexer<string> _regionNameIndexer;
		Indexer<string> _iffNameIndexer;

		/// <summary>Briefing <see cref="Logo"/> values</summary>
		public enum LogoEnum : byte {
			/// <summary>Defiance logo</summary>
			Defiance = 4,
			/// <summary>Liberty logo</summary>
			Liberty,
			/// <summary>Independance logo</summary>
			Independance,
			/// <summary>Family logo</summary>
			Family,
			/// <summary>No logo shown</summary>
			None
		}
		/// <summary>Mission starting <see cref="MissionType"/> (aka Hangar)</summary>
		public enum HangarEnum : byte {
			/// <summary>Junkyard starting location</summary>
			Junkyard,
			/// <summary>Instant start, only through the Simulator</summary>
			Simulator1,
			/// <summary>Instant start</summary>
			QuickStart,
			/// <summary>Instant start, only through the Simulator</summary>
			Simulator2,
			/// <summary>Skirmish mission type</summary>
			Skirmish,
			/// <summary>Death Star mission type</summary>
			DeathStar,
			/// <summary>Standard Rebel mission</summary>
			MonCalCruiser,
			/// <summary>Standard Family mission</summary>
			FamilyTransport
		}

		#region constructors
		/// <summary>Default constructor, creates a blank mission</summary>
		public Mission()
		{
			_initialize();
			for (int i=0;i<16;i++) { _globalCargo[i].Cargo = ""; _globalCargo[i].Unknown1 = true;  _globalGroup[i] = ""; }
			for (int i=0;i<4;i++) { _region[i] = "Region " + (i+1).ToString(); _iff[i+2] = ""; }
			Unknown1 = Unknown2 = true;
			MissionType = HangarEnum.MonCalCruiser;
			Logo = LogoEnum.None;
			Unknown3 = 0x62;
		}

		/// <summary>Creates a new mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		public Mission(string filePath)
		{
			_initialize();
			LoadFromFile(filePath);
		}

		/// <summary>Creates a new mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		public Mission(FileStream stream)
		{
			_initialize();
			LoadFromStream(stream);
		}
		
		void _initialize()
		{
			_invalidError = _invalidError.Replace("{0}", "XWA");
			_globalGroupNameIndexer = new Indexer<string>(_globalGroup, 56);
			_regionNameIndexer = new Indexer<string>(_region, 0x83);
			_iffNameIndexer = new Indexer<string>(_iff, 19, new bool[]{true, true, false, false, false, false});
			FlightGroups = new FlightGroupCollection();
			Messages = new MessageCollection();
			Globals = new GlobalsCollection();
			Teams = new TeamCollection();
			Briefings = new BriefingCollection();
		}
		#endregion constructors

		#region public methods
		/// <summary>Load a mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		/// <exception cref="System.IO.FileNotFoundException"><i>filePath</i> cannot be lcoated</exception>
		/// <exception cref="System.IO.InvalidDataException"><i>filePath</i> is not a XWA mission file</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="InvalidDataException"><i>stream</i> is not a valid XWA mission file</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (MissionFile.GetPlatform(stream) != MissionFile.Platform.XWA) throw new InvalidDataException(_invalidError);
            BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(1252));  //[JB] Changed encoding to windows-1252 (ANSI Latin 1) to ensure proper loading of 8-bit ANSI regardless of the operating system's default code page.
			int i, j;
			long p;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			#region Platform
			stream.Position = 8;
			Unknown1 = br.ReadBoolean();
			stream.Position = 0xB;
			Unknown2 = br.ReadBoolean();
			stream.Position = 0x14;
			for (i=2;i<6;i++) _iff[i] = new string(br.ReadChars(0x14)).Trim('\0');
			for (i=0;i<4;i++) _region[i] = new string(br.ReadChars(0x84)).Trim('\0');
			for (i=0;i<16;i++)
			{
				p = stream.Position;
				GlobalCargo[i].Cargo = new string(br.ReadChars(0x40)).Trim('\0');
				stream.Position += 4;
				GlobalCargo[i].Unknown1 = br.ReadBoolean();
				stream.Position += 3;
				GlobalCargo[i].Unknown2 = br.ReadByte();
				GlobalCargo[i].Unknown3 = br.ReadByte();
				GlobalCargo[i].Unknown4 = br.ReadByte();
				GlobalCargo[i].Unknown5 = br.ReadByte();
				stream.Position = p + 0x8C;
			}
			for (i=0;i<16;i++) _globalGroup[i] = new string(br.ReadChars(0x57)).Trim('\0');
			stream.Position = 0x23AC;
			MissionType = (HangarEnum)br.ReadByte();
			stream.Position++;
			TimeLimitMin = br.ReadByte();
			EndWhenComplete = br.ReadBoolean();
			Officer = br.ReadByte();
			Logo = (LogoEnum)br.ReadByte();
			stream.Position++;
			Unknown3 = br.ReadByte();
			Unknown4 = br.ReadByte();
			Unknown5 = br.ReadByte();
			stream.Position = 0x23F0;
			#endregion
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			byte[] buffer = new byte[64];
			for(i=0;i<numFlightGroups;i++)
			{
				#region Craft
				FlightGroups[i].Name = new string(br.ReadChars(0x14)).Trim('\0');
				stream.Read(buffer, 0, 7);
				FlightGroups[i].EnableDesignation1 = buffer[0]; //[JB] Changed bool to byte since it handles multiple values.
				FlightGroups[i].EnableDesignation2 = buffer[1];
				FlightGroups[i].Designation1 = buffer[2];
				FlightGroups[i].Designation2 = buffer[3];
				FlightGroups[i].Unknowns.Unknown1 = buffer[4];
				try { FlightGroups[i].GlobalCargo = (byte)(buffer[5]+1); }
				catch { FlightGroups[i].GlobalCargo = 0; }
				try { FlightGroups[i].GlobalSpecialCargo = (byte)(buffer[6]+1); }
				catch { FlightGroups[i].GlobalSpecialCargo = 0; }
				stream.Position += 0xD;
				FlightGroups[i].Cargo = new string(br.ReadChars(0x14)).Trim('\0');
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(0x14)).Trim('\0');
				FlightGroups[i].Role = new string(br.ReadChars(0x14)).Trim('\0');
				stream.Position += 5;
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
				FlightGroups[i].Radio = buffer[0xB];
				FlightGroups[i].Formation = buffer[0xD];
				FlightGroups[i].FormDistance = buffer[0xE];
				FlightGroups[i].GlobalGroup = buffer[0xF];
				FlightGroups[i].FormLeaderDist = buffer[0x10];
				FlightGroups[i].NumberOfWaves = (byte)(buffer[0x11]+1);
				FlightGroups[i].Unknowns.Unknown3 = buffer[0x12];
				FlightGroups[i].PlayerNumber = buffer[0x14];
				FlightGroups[i].ArriveOnlyIfHuman = Convert.ToBoolean(buffer[0x15]);
				FlightGroups[i].PlayerCraft = buffer[0x16];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x17] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x18] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x19] * 360 / 0x100);
				FlightGroups[i].Unknowns.Unknown4 = buffer[0x1B];
				#endregion
				#region Arr/Dep
				stream.Read(buffer, 0, 0x3C);
                FlightGroups[i].Unknowns.Unknown5 = buffer[0];  //[JB] Swapped with difficulty, now in correct position.
                FlightGroups[i].Difficulty = buffer[1];
				for (j=0;j<6;j++)
				{
					FlightGroups[i].ArrDepTriggers[0][j] = buffer[2+j];	// Arr1...
					FlightGroups[i].ArrDepTriggers[1][j] = buffer[8+j];
					FlightGroups[i].ArrDepTriggers[2][j] = buffer[0x12+j];
					FlightGroups[i].ArrDepTriggers[3][j] = buffer[0x18+j];
					FlightGroups[i].ArrDepTriggers[4][j] = buffer[0x26+j];	// Dep1...
					FlightGroups[i].ArrDepTriggers[5][j] = buffer[0x2C+j];
				}
				FlightGroups[i].ArrDepAndOr[0] = Convert.ToBoolean(buffer[0x10]);
				FlightGroups[i].Unknowns.Unknown6 = Convert.ToBoolean(buffer[0x11]);
				FlightGroups[i].ArrDepAndOr[1] = Convert.ToBoolean(buffer[0x20]);
				FlightGroups[i].ArrDepAndOr[2] = Convert.ToBoolean(buffer[0x22]);
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				FlightGroups[i].ArrDepAndOr[3] = Convert.ToBoolean(buffer[0x34]);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x36];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x37];
				FlightGroups[i].AbortTrigger = buffer[0x38];
				FlightGroups[i].Unknowns.Unknown7 = buffer[0x39];
				FlightGroups[i].Unknowns.Unknown8 = buffer[0x3A];
				stream.Read(buffer, 0, 8);
				FlightGroups[i].ArrivalCraft1 = buffer[0];
				FlightGroups[i].ArrivalMethod1 = Convert.ToBoolean(buffer[1]);
                FlightGroups[i].DepartureCraft1 = buffer[2];   //[JB] Fixed this section
                FlightGroups[i].DepartureMethod1 = Convert.ToBoolean(buffer[3]);
                FlightGroups[i].ArrivalCraft2 = buffer[4];
				FlightGroups[i].ArrivalMethod2 = Convert.ToBoolean(buffer[5]);
				FlightGroups[i].DepartureCraft2 = buffer[6];
				FlightGroups[i].DepartureMethod2 = Convert.ToBoolean(buffer[7]);
				#endregion
				#region Orders
				for (j=0;j<16;j++)
				{
					FlightGroups[i].Orders[j/4, j%4] = new FlightGroup.Order();
					stream.Read(buffer, 0, 0x14);
					for (int h=0;h<0x13;h++) FlightGroups[i].Orders[j/4, j%4][h] = buffer[h];
					for (int h=0;h<8;h++)
						for (int k=0;k<4;k++) FlightGroups[i].Orders[j/4, j%4].Waypoints[h][k] = (short)(br.ReadInt16() * (k == 1 ? -1 : 1));
					stream.Position += 0x1E;
					FlightGroups[i].Orders[j/4, j%4].Unknown10 = br.ReadByte();
					FlightGroups[i].Orders[j/4, j%4].Unknown11 = br.ReadBoolean();
					FlightGroups[i].Orders[j/4, j%4].Unknown12 = br.ReadBoolean();
					stream.Position += 6;
					FlightGroups[i].Orders[j/4, j%4].Unknown13 = br.ReadBoolean();
					stream.Position += 5;
					FlightGroups[i].Orders[j/4, j%4].Unknown14 = br.ReadBoolean();
					stream.Position += 0x12;
				}
				for (j=0;j<16;j++)
				{
					stream.Read(buffer, 0, 0x10);
					for (int h=0;h<6;h++)
					{
						FlightGroups[i].Orders[j/4, j%4].SkipTriggers[0][h] = buffer[h];
						FlightGroups[i].Orders[j/4, j%4].SkipTriggers[1][h] = buffer[h+6];
					}
					FlightGroups[i].Orders[j/4, j%4].SkipT1AndOrT2 = Convert.ToBoolean(buffer[0xE]);
                }
				#endregion
				#region Goals
				for (j=0;j<8;j++)
				{
					byte[] temp = new byte[0x10];
					stream.Read(temp, 0, 0x10);
					FlightGroups[i].Goals[j] = new FlightGroup.Goal(temp);
					stream.Position += 0x3F;
					FlightGroups[i].Goals[j].Unknown15 = br.ReadBoolean();
				}
				#endregion
				for (j = 0; j < 4; j++) for (int k = 0; k < 4; k++) FlightGroups[i].Waypoints[j][k] = (short)(br.ReadInt16() * (k == 1 ? -1 : 1));
				for (j=0;j<4;j++) FlightGroups[i].Waypoints[j].Region = br.ReadByte();
				#region Options/other
				stream.Read(buffer, 0, 0x1E);
				FlightGroups[i].Unknowns.Unknown16 = buffer[0];
				FlightGroups[i].Unknowns.Unknown17 = buffer[1];
				FlightGroups[i].Unknowns.Unknown18 = buffer[2];
				FlightGroups[i].Unknowns.Unknown19 = buffer[3];
				FlightGroups[i].Unknowns.Unknown20 = buffer[4];
				FlightGroups[i].Unknowns.Unknown21 = buffer[5];
				FlightGroups[i].Unknowns.Unknown22 = Convert.ToBoolean(buffer[6]);
				FlightGroups[i].Unknowns.Unknown23 = buffer[8];
				FlightGroups[i].Unknowns.Unknown24 = buffer[9];
				FlightGroups[i].Unknowns.Unknown25 = buffer[0xA];
				FlightGroups[i].Unknowns.Unknown26 = buffer[0xB];
				FlightGroups[i].Unknowns.Unknown27 = buffer[0xC];
				FlightGroups[i].Unknowns.Unknown28 = buffer[0xD];
				FlightGroups[i].Unknowns.Unknown29 = Convert.ToBoolean(buffer[0xE]);
				FlightGroups[i].Unknowns.Unknown30 = Convert.ToBoolean(buffer[0x12]);
				FlightGroups[i].Unknowns.Unknown31 = Convert.ToBoolean(buffer[0x13]);
				FlightGroups[i].GlobalNumbering = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].Unknowns.Unknown32 = buffer[0x17];
				FlightGroups[i].Unknowns.Unknown33 = buffer[0x18];
				FlightGroups[i].Countermeasures = buffer[0x19];
				FlightGroups[i].ExplosionTime = buffer[0x1A];
				FlightGroups[i].Status2 = buffer[0x1B];
				FlightGroups[i].GlobalUnit = buffer[0x1C];

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
				FlightGroups[i].PilotID = new string(br.ReadChars(0x10)).Trim('\0');
				stream.Position += 5;
				FlightGroups[i].Backdrop = br.ReadByte();
				stream.Position += 0x16;
				stream.Read(buffer, 0, 0x15);
				FlightGroups[i].Unknowns.Unknown34 = Convert.ToBoolean(buffer[0]);
				FlightGroups[i].Unknowns.Unknown35 = Convert.ToBoolean(buffer[2]);
				FlightGroups[i].Unknowns.Unknown36 = Convert.ToBoolean(buffer[4]);
				FlightGroups[i].Unknowns.Unknown37 = Convert.ToBoolean(buffer[6]);
				FlightGroups[i].Unknowns.Unknown38 = Convert.ToBoolean(buffer[8]);
				FlightGroups[i].Unknowns.Unknown39 = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Unknowns.Unknown40 = Convert.ToBoolean(buffer[0xC]);
				FlightGroups[i].Unknowns.Unknown41 = Convert.ToBoolean(buffer[0xE]);
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
					Messages[i].MessageString = new string(br.ReadChars(0x40)).Trim('\0');
					stream.Position += 0x10;
					stream.Read(buffer, 0, 0xA);
					for (j=0;j<10;j++) Messages[i].SentTo[j] = Convert.ToBoolean(buffer[j]);
					stream.Read(buffer, 0, 0x20);
					for (j=0;j<6;j++)
					{
						Messages[i].Triggers[0][j] = buffer[j];	// T1...
						Messages[i].Triggers[1][j] = buffer[6+j];
						Messages[i].Triggers[2][j] = buffer[0x10+j];
						Messages[i].Triggers[3][j] = buffer[0x16+j];
					}
					Messages[i].Unknown1 = buffer[0xC];
					Messages[i].TrigAndOr[0] = Convert.ToBoolean(buffer[0xE]);
					Messages[i].TrigAndOr[1] = Convert.ToBoolean(buffer[0x1E]);
					Messages[i].VoiceID = new string(br.ReadChars(8)).Trim('\0');
					Messages[i].OriginatingFG = br.ReadByte();
					stream.Position += 7;
					stream.Read(buffer, 0, 0x16);
					Messages[i].Delay = buffer[0];
                    Messages[i].TrigAndOr[2] = Convert.ToBoolean(buffer[1]);
					Messages[i].Color = buffer[2];
                    Messages[i].Unknown2 = buffer[3];

                    for (j=0;j<6;j++)
					{
						Messages[i].Triggers[4][j] = buffer[4+j];	// CancelT1...
						Messages[i].Triggers[5][j] = buffer[0xA+j];
					}
					Messages[i].TrigAndOr[3] = Convert.ToBoolean(buffer[0x12]);
					Messages[i].Unknown3 = Convert.ToBoolean(buffer[0x14]);
				}
			}
			else Messages.Clear();
			#endregion
			#region Globals
			Globals.ClearAll();
			for(i=0;i<Globals.Count;i++)
			{
				stream.Position += 2;
				for (int k=0;k<3;k++)
				{
					stream.Read(buffer, 0, 0xE);
					for (j=0;j<6;j++)
					{
						Globals[i].Goals[k].Triggers[0][j] = buffer[j];
						Globals[i].Goals[k].Triggers[1][j] = buffer[j+6];
					}
					Globals[i].Goals[k].T1AndOrT2 = br.ReadBoolean();
					Globals[i].Goals[k].Unknown1 = br.ReadBoolean();
					stream.Read(buffer, 0, 0xE);
					for (j=0;j<6;j++)
					{
						Globals[i].Goals[k].Triggers[2][j] = buffer[j];
						Globals[i].Goals[k].Triggers[3][j] = buffer[j+6];
					}
					Globals[i].Goals[k].T3AndOrT4 = br.ReadBoolean();
					stream.Position += 8;
					Globals[i].Goals[k].Unknown2 = br.ReadBoolean();
					stream.Position += 9;
					Globals[i].Goals[k].T12AndOrT34 = br.ReadBoolean();
					stream.Read(buffer, 0, 7);
					Globals[i].Goals[k].Unknown3 = buffer[0];
					Globals[i].Goals[k].RawPoints=(sbyte)buffer[1];
					Globals[i].Goals[k].Unknown4 = buffer[2];
					Globals[i].Goals[k].Unknown5 = buffer[3];
					Globals[i].Goals[k].Unknown6 = buffer[4];
					Globals[i].Goals[k].ActiveSequence = buffer[6];
					stream.Position += 0x41;
				}
			}
			#endregion
			#region Teams
			Teams.ClearAll();
			for(i=0;i<Teams.Count;i++)
			{
				stream.Position += 2;
				Teams[i].Name = new string(br.ReadChars(0x12)).Trim('\0');	// null-termed
				stream.Position += 6;
				for (j=0;j<10;j++) Teams[i].Allies[j] = (Team.Allegeance)br.ReadByte();
				for (j=0;j<6;j++) Teams[i].EndOfMissionMessages[j] = new string(br.ReadChars(0x40)).Trim('\0');
				stream.Read(Teams[i].Unknowns, 0, 6);
				for (j=0;j<3;j++) Teams[i].VoiceIDs[j] = new string(br.ReadChars(0x14)).Trim('\0');
				stream.Position++;
			}
			#endregion
			#region Briefing
			Briefings.ClearAll();
			for (i=0;i<2;i++)
			{
				Briefings[i].Length = br.ReadInt16();
				Briefings[i].Unknown1 = br.ReadInt16();
				stream.Position += 6;	// StartLength, EventsLength, Res(0)
				byte[] briefBuffer = new byte[0x100];
				for (j=0;j<0x44;j++)
				{
					stream.Read(briefBuffer, 0, 0x100);
					Buffer.BlockCopy(briefBuffer, 0, Briefings[i].Events, 0x100 * j, 0x100);
				}
				stream.Read(buffer, 0, 0xA);
				for (j=0;j<10;j++) Briefings[i].Team[j] = Convert.ToBoolean(buffer[j]);
				for (j=0;j<128;j++)
				{
					int k = br.ReadInt16();
					Briefings[i].BriefingTag[j] = "";
					if (k > 0) Briefings[i].BriefingTag[j] = new string(br.ReadChars(k)).Trim('\0');	// shouldn't need the trim
				}
				for (j=0;j<128;j++)
				{
					int k = br.ReadInt16();
					Briefings[i].BriefingString[j] = "";
					if (k > 0) Briefings[i].BriefingString[j] = new string(br.ReadChars(k)).Trim('\0');
				}
			}
			#endregion
			#region notes
			_missionNote = new string(br.ReadChars(0x187C)).Trim('\0');
			for (i = 0; i < 128; i++) Briefings[0].BriefingStringsNotes[i] = new string(br.ReadChars(0x64)).Trim('\0');
			for (i = 0; i < 64; i++)
			{
				if (i < Messages.Count) Messages[i].Note = new string(br.ReadChars(0x64)).Trim('\0');
				else stream.Position += 0x64;
			}
			for (i = 0; i < 10; i++) for (j = 0; j < 3; j++) Teams[i].EomNotes[j] = new string(br.ReadChars(0x64)).Trim('\0');
			stream.Position += 0xFA0;	// unknown space, possibly message notes
			_descriptionNote = new string(br.ReadChars(0x64)).Trim('\0');
			_successfulNote = new string(br.ReadChars(0x64)).Trim('\0');
			_failedNote = new string(br.ReadChars(0x64)).Trim('\0');
			#endregion
			#region FG goal strings
			for (i=0;i<FlightGroups.Count;i++)
				for (j=0;j<8;j++)	// per goal
					for (int k=0;k<3;k++)	// per string
						if (br.ReadByte() != 0)
						{
							stream.Position--;
							if (k == 0) FlightGroups[i].Goals[j].IncompleteText = new string(br.ReadChars(0x40)).Trim('\0');
							else if (k == 1) FlightGroups[i].Goals[j].CompleteText = new string(br.ReadChars(0x40)).Trim('\0');
							else FlightGroups[i].Goals[j].FailedText = new string(br.ReadChars(0x40)).Trim('\0');
						}
			#endregion
			#region Globals strings
			for (i = 0; i < 10; i++)	// Team
				for (j = 0; j < 12; j++)	// Goal * Trigger
					for (int k = 0; k < 3; k++)	// State
						if (br.ReadByte() != 0)
						{
							if (j >= 8 && k == 0) { stream.Position += 0x3F; continue; }	// skip Sec Inc
							if (j >= 4 && k == 2) { stream.Position += 0x3F; continue; }	// skip Prev & Sec Fail
							stream.Position--;
							Globals[i].Goals[j / 4].GoalStrings[j % 4, k] = new string(br.ReadChars(0x40));
						}
			#endregion
			stream.Position += 0x1E0;	// unknown space
			#region Order strings
			for (i=0;i<192;i++) // per FG (and then some)
				for (j=0;j<16;j++) // per order
					if (br.ReadByte() != 0)
					{
						if (i >= FlightGroups.Count) { stream.Position += 0x3F; continue; }	// skip if FG doesn't exist
						stream.Position--;
						FlightGroups[i].Orders[j/4, j%4].CustomText = new string(br.ReadChars(0x40)).Trim('\0');
					}
			#endregion
            _missionSuccessful = new string(br.ReadChars(0x1000)).TrimEnd('\0');
            _missionFailed = new string(br.ReadChars(0x1000)).TrimEnd('\0');
            _missionDescription = new string(br.ReadChars(0x1000)).TrimEnd('\0');    //[JB] Only trim from end, because trimming from the start might return YOGEME's signature embedded into the end of the description, or any leftover garbage data that is normally ignored by a null terminator.
			MissionPath = stream.Name;
		}

		/// <summary>Save the mission with the default path</summary>
		/// <exception cref="System.UnauthorizedAccessException">Write permissions for <see cref="MissionFile.MissionPath"/> are denied</exception>
		public void Save()
		{
			FileStream fs = null;
			try
			{
				File.Delete(MissionPath);
				fs = File.OpenWrite(MissionPath);
                BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.GetEncoding(1252));  //[JB] Changed encoding to windows-1252 (ANSI Latin 1) to ensure proper loading of 8-bit ANSI regardless of the operating system's default code page.
				int i;
				long p;
				#region Platform
				bw.Write((short)0x12);
				bw.Write((short)FlightGroups.Count);
				bw.Write((short)Messages.Count);
				fs.Position = 8;
				bw.Write(Unknown1);
				fs.Position = 0xB;
				bw.Write(Unknown2);
				fs.Position = 0x14;
				for (i=2;i<6;i++)
				{
					p = fs.Position;
					bw.Write(_iff[i].ToCharArray());
					fs.Position = p + 0x14;
				}
				for (i=0;i<4;i++)
				{
					p = fs.Position;
					bw.Write(_region[i].ToCharArray());
					fs.Position = p + 0x84;
				}
				for (i=0;i<16;i++)
				{
					p = fs.Position;
					bw.Write(GlobalCargo[i].Cargo.ToCharArray());
					fs.Position = p + 0x44;
					bw.Write(GlobalCargo[i].Unknown1);
					fs.Position += 3;
					fs.WriteByte(GlobalCargo[i].Unknown2);
					fs.WriteByte(GlobalCargo[i].Unknown3);
					fs.WriteByte(GlobalCargo[i].Unknown4);
					fs.WriteByte(GlobalCargo[i].Unknown5);
					fs.Position = p + 0x8C;
				}
				for (i=0;i<16;i++)
				{
					p = fs.Position;
					bw.Write(_globalGroup[i].ToCharArray());
					fs.Position = p + 0x57;
				}
				fs.Position = 0x23AC;
				fs.WriteByte((byte)MissionType);
				fs.Position++;
				fs.WriteByte(TimeLimitMin);
				bw.Write(EndWhenComplete);
				fs.WriteByte(Officer);
				fs.WriteByte((byte)Logo);
				fs.Position++;
				fs.WriteByte(Unknown3);
				fs.WriteByte(Unknown4);
				fs.WriteByte(Unknown5);
				fs.Position = 0x23F0;
				#endregion
				#region FlightGroups
				for(i=0;i<FlightGroups.Count;i++)
				{
					p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0x14;
                    bw.Write(FlightGroups[i].EnableDesignation1);   //[JB]Changed bool to byte
					bw.Write(FlightGroups[i].EnableDesignation2);
					fs.WriteByte(FlightGroups[i].Designation1);
					fs.WriteByte(FlightGroups[i].Designation2);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown1);
					bw.Write((byte)(FlightGroups[i].GlobalCargo == 0 ? 255 : FlightGroups[i].GlobalCargo - 1));
					bw.Write((byte)(FlightGroups[i].GlobalSpecialCargo == 0 ? 255 : FlightGroups[i].GlobalSpecialCargo - 1));
					fs.Position = p + 0x28;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x3C;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x50;
					bw.Write(FlightGroups[i].Role.ToCharArray());
					fs.Position = p + 0x69;
					if (FlightGroups[i].SpecialCargoCraft == 0) fs.WriteByte(FlightGroups[i].NumberOfCraft);
					else fs.WriteByte((byte)(FlightGroups[i].SpecialCargoCraft-1));
					bw.Write(FlightGroups[i].RandSpecCargo);
					fs.WriteByte(FlightGroups[i].CraftType);
					fs.WriteByte(FlightGroups[i].NumberOfCraft);
					fs.WriteByte(FlightGroups[i].Status1);
					fs.WriteByte(FlightGroups[i].Missile);
					fs.WriteByte(FlightGroups[i].Beam);
					fs.WriteByte(FlightGroups[i].IFF);
					fs.WriteByte(FlightGroups[i].Team);
					fs.WriteByte(FlightGroups[i].AI);
					fs.WriteByte(FlightGroups[i].Markings);
					fs.WriteByte(FlightGroups[i].Radio);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Formation);
					fs.WriteByte(FlightGroups[i].FormDistance);
					fs.WriteByte(FlightGroups[i].GlobalGroup);
					fs.WriteByte(FlightGroups[i].FormLeaderDist);
					fs.WriteByte((byte)(FlightGroups[i].NumberOfWaves-1));
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown3);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].PlayerNumber);
					bw.Write(FlightGroups[i].ArriveOnlyIfHuman);
					fs.WriteByte(FlightGroups[i].PlayerCraft);
					fs.WriteByte((byte)Math.Round(((double)FlightGroups[i].Yaw * 0x100 / 360)));
					fs.WriteByte((byte)Math.Round(((double)(FlightGroups[i].Pitch >= 64 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360)));
					fs.WriteByte((byte)Math.Round(((double)FlightGroups[i].Roll * 0x100 / 360)));
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown4);
					fs.Position++;
					#endregion
					#region Arr/Dep
                    fs.WriteByte(FlightGroups[i].Unknowns.Unknown5);  //[JB] Swapped with difficulty, now in correct position
                    fs.WriteByte(FlightGroups[i].Difficulty);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[0][j]);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[1][j]);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[0]);
					bw.Write(FlightGroups[i].Unknowns.Unknown6);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[2][j]);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[3][j]);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[1]);
					fs.Position++;
					bw.Write(FlightGroups[i].ArrDepAndOr[2]);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].ArrivalDelayMinutes);
					fs.WriteByte(FlightGroups[i].ArrivalDelaySeconds);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[4][j]);
					for (j = 0; j < 6; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[5][j]);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[3]);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].DepartureTimerMinutes);
					fs.WriteByte(FlightGroups[i].DepartureTimerSeconds);
					fs.WriteByte(FlightGroups[i].AbortTrigger);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown7);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown8);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].ArrivalCraft1);
					bw.Write(FlightGroups[i].ArrivalMethod1);
                    fs.WriteByte(FlightGroups[i].DepartureCraft1); //[JB] Fixed order
                    bw.Write(FlightGroups[i].DepartureMethod1);
                    fs.WriteByte(FlightGroups[i].ArrivalCraft2);
					bw.Write(FlightGroups[i].ArrivalMethod2);
					fs.WriteByte(FlightGroups[i].DepartureCraft2);
					bw.Write(FlightGroups[i].DepartureMethod2);
					#endregion
					#region Orders
					for (j=0;j<16;j++)
					{
						for (int h=0;h<0x13;h++) fs.WriteByte(FlightGroups[i].Orders[j/4, j%4][h]);
						fs.Position++;
						for (int h=0;h<8;h++)
							for (int k=0;k<4;k++) bw.Write((short)(FlightGroups[i].Orders[j/4, j%4].Waypoints[h][k] * (k == 1 ? -1 : 1)));
						fs.Position += 0x1E;
						fs.WriteByte(FlightGroups[i].Orders[j/4, j%4].Unknown10);
						bw.Write(FlightGroups[i].Orders[j/4, j%4].Unknown11);
						bw.Write(FlightGroups[i].Orders[j/4, j%4].Unknown12);
						fs.Position += 6;
						bw.Write(FlightGroups[i].Orders[j/4, j%4].Unknown13);
						fs.Position += 5;
						bw.Write(FlightGroups[i].Orders[j/4, j%4].Unknown14);
						fs.Position += 0x12;
					}
					for (j=0;j<16;j++)
					{
						for (int k = 0; k < 6; k++) fs.WriteByte(FlightGroups[i].Orders[j/4, j%4].SkipTriggers[0][k]);
						for (int k = 0; k < 6; k++) fs.WriteByte(FlightGroups[i].Orders[j/4, j%4].SkipTriggers[1][k]);
						fs.Position += 2;
						bw.Write(FlightGroups[i].Orders[j/4, j%4].SkipT1AndOrT2);
						fs.Position++;
					}
					#endregion
					#region Goals
					for (j=0;j<8;j++)
					{
						for (int k=0;k<6;k++) fs.WriteByte(FlightGroups[i].Goals[j][k]);
						fs.Position += 7;  //Was 8
                        fs.WriteByte(FlightGroups[i].Goals[j].Unknown42);  //[JB] Added unknown.
						fs.WriteByte(FlightGroups[i].Goals[j].Parameter);
						fs.WriteByte(FlightGroups[i].Goals[j].ActiveSequence);
						fs.Position += 0x3F;
						bw.Write(FlightGroups[i].Goals[j].Unknown15);
					}
					#endregion
					// SP1 0,0,0 check for backdrops
					if (FlightGroups[i].CraftType == 0xB7 && FlightGroups[i].Waypoints[0].RawX == 0 && FlightGroups[i].Waypoints[0].RawY == 0 && FlightGroups[i].Waypoints[0].RawZ == 0)
						FlightGroups[i].Waypoints[0].RawY = 10;
					for (j = 0; j < 4; j++) for (int k = 0; k < 4; k++) bw.Write((short)(FlightGroups[i].Waypoints[j][k] * (k == 1 ? -1 : 1)));
					for (j = 0; j < 4; j++) fs.WriteByte(FlightGroups[i].Waypoints[j].Region);
					#region Options/other
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown16);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown17);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown18);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown19);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown20);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown21);
					bw.Write(FlightGroups[i].Unknowns.Unknown22);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown23);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown24);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown25);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown26);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown27);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown28);
					bw.Write(FlightGroups[i].Unknowns.Unknown29);
					fs.Position += 3;
					bw.Write(FlightGroups[i].Unknowns.Unknown30);
					bw.Write(FlightGroups[i].Unknowns.Unknown31);
					fs.Position += 2;
					bw.Write(FlightGroups[i].GlobalNumbering);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown32);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown33);
					fs.WriteByte(FlightGroups[i].Countermeasures);
					fs.WriteByte(FlightGroups[i].ExplosionTime);
					fs.WriteByte(FlightGroups[i].Status2);
					fs.WriteByte(FlightGroups[i].GlobalUnit);
					fs.Position++;

                    for (j=1;j<9;j++) if (FlightGroups[i].OptLoadout[j]) bw.Write((byte)j); else fs.Position++;	// warheads
					for (j=1;j<5;j++) if (FlightGroups[i].OptLoadout[j+9]) bw.Write((byte)j); else fs.Position++;	// CMs
					fs.Position += 2;	// only writing 4, 2 bytes remain 
					for (j=1;j<4;j++) if (FlightGroups[i].OptLoadout[j+14]) bw.Write((byte)j); else fs.Position++;	// beam
					fs.Position += 1;	// only writing 3, 1 byte remains

                    fs.WriteByte((byte)FlightGroups[i].OptCraftCategory);
					for (int k = 0; k < 10; k++) fs.WriteByte(FlightGroups[i].OptCraft[k].CraftType);
					for (int k = 0; k < 10; k++) fs.WriteByte(FlightGroups[i].OptCraft[k].NumberOfCraft);
					for (int k = 0; k < 10; k++) fs.WriteByte(FlightGroups[i].OptCraft[k].NumberOfWaves);
					bw.Write(FlightGroups[i].PilotID.ToCharArray());
					fs.Position = p + 0xE12;
					fs.WriteByte(FlightGroups[i].Backdrop);
					fs.Position += 0x16;
					bw.Write(FlightGroups[i].Unknowns.Unknown34); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown35); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown36); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown37); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown38); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown39); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown40); fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown41);
					fs.Position = p + 0xE3E;
					#endregion
				}
				#endregion
				#region Messages
				for (i=0;i<Messages.Count;i++)
				{
					p = fs.Position;
					bw.Write((short)i);
					bw.Write(Messages[i].MessageString.ToCharArray());
					fs.Position = p + 0x52;
					for (int j=0;j<10;j++) bw.Write(Messages[i].SentTo[j]);
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[0][j]);
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[1][j]);
					fs.WriteByte(Messages[i].Unknown1);
					fs.Position++;
					bw.Write(Messages[i].TrigAndOr[0]);
					fs.Position++;
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[2][j]);
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[3][j]);
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[1]);
					fs.Position++;
					bw.Write(Messages[i].VoiceID.ToCharArray());
					fs.Position = p + 0x84;
					fs.WriteByte(Messages[i].OriginatingFG);
					fs.Position += 7;
					fs.WriteByte(Messages[i].Delay);
                    bw.Write(Messages[i].TrigAndOr[2]);
					fs.WriteByte(Messages[i].Color);
                    fs.WriteByte(Messages[i].Unknown2);
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[4][j]);
					for (int j = 0; j < 6; j++) fs.WriteByte(Messages[i].Triggers[5][j]);
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[3]);
					fs.Position++;
					bw.Write(Messages[i].Unknown3);
					fs.Position++;
				}
				#endregion
				#region Globals
				for(i=0;i<Globals.Count;i++)
				{
					p = fs.Position;
					bw.Write((short)3);
					int j;
					for (int k=0;k<3;k++)
					{
						for (j = 0; j < 6; j++) fs.WriteByte(Globals[i].Goals[k].Triggers[0][j]);
						for (j = 0; j < 6; j++) fs.WriteByte(Globals[i].Goals[k].Triggers[1][j]);
						fs.Position += 2;
						bw.Write(Globals[i].Goals[k].T1AndOrT2);
						bw.Write(Globals[i].Goals[k].Unknown1);
						for (j = 0; j < 6; j++) fs.WriteByte(Globals[i].Goals[k].Triggers[2][j]);
						for (j = 0; j < 6; j++) fs.WriteByte(Globals[i].Goals[k].Triggers[3][j]);
						fs.Position += 2;
						bw.Write(Globals[i].Goals[k].T3AndOrT4);
						fs.Position += 8;
						bw.Write(Globals[i].Goals[k].Unknown2);
						fs.Position += 9;
						bw.Write(Globals[i].Goals[k].T12AndOrT34);
						fs.WriteByte(Globals[i].Goals[k].Unknown3);
						fs.WriteByte((byte)Globals[i].Goals[k].RawPoints);
						fs.WriteByte(Globals[i].Goals[k].Unknown4);
						fs.WriteByte(Globals[i].Goals[k].Unknown5);
						fs.WriteByte(Globals[i].Goals[k].Unknown6);
						fs.Position++;
						fs.WriteByte(Globals[i].Goals[k].ActiveSequence);
						fs.Position += 0x41;
					}
				}
				#endregion
				#region Teams
				for(i=0;i<Teams.Count;i++)
				{
					p = fs.Position;
					bw.Write((short)1);
					bw.Write(Teams[i].Name.ToCharArray()); fs.WriteByte(0);
					fs.Position = p + 0x1A;
					for (int j=0;j<10;j++) fs.WriteByte((byte)Teams[i].Allies[j]);
					for (int j=0;j<6;j++)
					{
						bw.Write(Teams[i].EndOfMissionMessages[j].ToCharArray());
						fs.WriteByte(0);
						fs.Position = p + 0x24 + (j+1)*0x40;
					}
					fs.Write(Teams[i].Unknowns, 0, 6);
					for (int j=0;j<3;j++)
					{
						bw.Write(Teams[i].VoiceIDs[j].ToCharArray());
						fs.Position = p + 0x1AA + (j+1)*0x14;
					}
					fs.Position = p + 0x1E7;
				}
				#endregion
				#region Briefing
				for (i=0;i<2;i++)
				{
					bw.Write(Briefings[i].Length);
					bw.Write(Briefings[i].Unknown1);
					bw.Write(Briefings[i].StartLength);
					bw.Write(Briefings[i].EventsLength);
					fs.Position += 2;
					byte[] briefBuffer = new byte[Briefings[i].Events.Length * 2];
					Buffer.BlockCopy(Briefings[i].Events, 0, briefBuffer, 0, briefBuffer.Length);
					bw.Write(briefBuffer);
					for (int j=0;j<10;j++) bw.Write(Briefings[i].Team[j]);
					for (int j=0;j<128;j++)
					{
						bw.Write((short)Briefings[i].BriefingTag[j].Length);
						if (Briefings[i].BriefingTag[j].Length != 0) bw.Write(Briefings[i].BriefingTag[j].ToCharArray());
					}
					for (int j=0;j<128;j++)
					{
						bw.Write((short)Briefings[i].BriefingString[j].Length);
						if (Briefings[i].BriefingString[j].Length != 0) bw.Write(Briefings[i].BriefingString[j].ToCharArray());
					}
				}
				#endregion
				#region notes
				p = fs.Position;
				bw.Write(_missionNote.ToCharArray());
				fs.Position = p + 0x187C;
				for (i = 0; i < 128; i++)
				{
					p = fs.Position;
					bw.Write(Briefings[0].BriefingStringsNotes[i].ToCharArray());
					fs.Position = p + 0x64;
				}
				for (i=0;i<64;i++)
				{
					p = fs.Position;
					if (i < Messages.Count) bw.Write(Messages[i].Note.ToCharArray());
					fs.Position = p + 0x64;
				}
				for (i = 0; i < 10; i++)
					for (int j = 0; j < 3; j++)
					{
						p = fs.Position;
						bw.Write(Teams[i].EomNotes[j].ToCharArray());
						fs.Position = p + 0x64;
					}
				fs.Position += 0xFA0;	// unknown space, possibly message notes
				p = fs.Position;
				bw.Write(_descriptionNote.ToCharArray());
				fs.Position = p + 0x64;
				p = fs.Position;
				bw.Write(_successfulNote.ToCharArray());
				fs.Position = p + 0x64;
				p = fs.Position;
				bw.Write(_failedNote.ToCharArray());
				fs.Position = p + 0x64;
				#endregion
				#region FG Goal Strings
				for (i=0;i<FlightGroups.Count;i++)
					for (int j=0;j<8;j++)	// per goal
						for (int k=0;k<3;k++)	// per string
						{
							string s = (k == 0 ? FlightGroups[i].Goals[j].IncompleteText : (k == 1 ? FlightGroups[i].Goals[j].CompleteText : FlightGroups[i].Goals[j].FailedText));
							if (s != "")
							{
								p = fs.Position;
								bw.Write(s.ToCharArray());
								fs.Position = p + 0x40;
							}
							else fs.Position++;
						}
				#endregion
				#region Globals strings
				for (i=0;i<10;i++)	// Team
					for (int j=0;j<12;j++)	// Goal * Trigger
						for (int k=0;k<3;k++)	// State
							if (Globals[i].Goals[j / 4].GoalStrings[j % 4, k] != "")
							{
								p = fs.Position;
								bw.Write(Globals[i].Goals[j / 4].GoalStrings[j % 4, k].ToCharArray());
								fs.Position = p + 0x40;
							}
							else fs.Position++;
				#endregion
				fs.Position += 0x1E0;	// unknown space
				#region Order strings
				for (i=0;i<192;i++) // per FG (and then some)
					for (int j=0;j<16;j++) // per order
						if (i < FlightGroups.Count && FlightGroups[i].Orders[j/4, j%4].CustomText != "")
						{
							p = fs.Position;
							bw.Write(FlightGroups[i].Orders[j/4, j%4].CustomText.ToCharArray());
							fs.Position = p + 0x40;
						}
						else fs.Position++;
				#endregion
				p = fs.Position;
				bw.Write(_missionSuccessful.ToCharArray());
				fs.Position = p + 0x1000;
				p = fs.Position;
				bw.Write(_missionFailed.ToCharArray());
				fs.Position = p + 0x1000;
				p = fs.Position;
				bw.Write(_missionDescription.ToCharArray());
				fs.Position = p + 0x1000;
				
				//[JB] Embed YOGEME signature into the empty description space (appending bytes would disrupt the text in game).  The description must have room for 3 bytes (null terminater for the briefing text, then the 2 byte signature)
				if(_missionDescription.Length < 0x1000 - 3)
				{
					fs.Position -= 2;
					bw.Write((short)0x2106);
				}

				fs.SetLength(fs.Position);
				fs.Close();
			}
			catch
			{
                if (fs != null) fs.Close(); //[JB] Fixed to prevent object instance error.
				throw;
			}
		}

		/// <summary>Save the mission to a new location</summary>
		/// <param name="filePath">Full path to the new file location</param>
		/// <exception cref="System.UnauthorizedAccessException">Write permissions for <i>filePath</i> are denied</exception>
		public void Save(string filePath)
		{
			MissionPath = filePath;
			Save();
		}

        /// <summary>Deletes a Flight Group, performing all necessary cleanup to avoid broken indexes.</summary>
        /// <remarks>Propagates throughout all members which may reference Flight Group indexes.</remarks>
        /// <returns>Index of the next available Flight Group.</returns>
        public int DeleteFG(int fgIndex)
        {
            if (fgIndex < 0 || fgIndex >= FlightGroups.Count) return 0;  //If out of range, do nothing and return selection to first item.

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Mission.Trigger trig in goal.Triggers)
                        trig.TransformFGReferences(fgIndex, -1, false);

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.TransformFGReferences(fgIndex, -1, true);

            //XWA Briefing does not use FG indexes.

            //Skip triggers are processed by the Orders, which are processed by the FlightGroup.
            foreach (FlightGroup fg in FlightGroups)
                fg.TransformFGReferences(fgIndex, -1);

            return FlightGroups.RemoveAt(fgIndex);
        }

        /// <summary>Swaps two FlightGroups, used to move FGs up or down in the list.</summary>
        /// <remarks>Automatically performs bounds checking and adjusts all Flight Group indexes to prevent broken indexes in triggers, orders, etc.</remarks>
        /// <returns>Returns true if an adjustment was performed, false if index validation failed.</returns>
        public bool SwapFG(int srcIndex, int dstIndex)
        {
            if ((srcIndex < 0 || srcIndex >= FlightGroups.Count) || (dstIndex < 0 || dstIndex >= FlightGroups.Count) || (srcIndex == dstIndex)) return false;

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Mission.Trigger trig in goal.Triggers)
                        trig.SwapFGReferences(srcIndex, dstIndex);

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.SwapFGReferences(srcIndex, dstIndex);

            foreach (Briefing b in Briefings)
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

        /// <summary>Deletes a Message, performing all necessary cleanup to avoid broken indexes.</summary>
        /// <remarks>Iterates throughout all members which may reference Message indexes.</remarks>
        /// <returns>Index of the next available Message.</returns>
        public int DeleteMessage(int msgIndex)
        {
            if (msgIndex < 0 || msgIndex >= Messages.Count) return 0;  //If out of range, do nothing and return selection to first item.

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.TransformMessageRef(msgIndex, -1);

            foreach (FlightGroup fg in FlightGroups)
            {
                foreach (Mission.Trigger trig in fg.ArrDepTriggers)
                    trig.TransformMessageRef(msgIndex, -1);

                foreach (FlightGroup.Order order in fg.Orders)
                {
                    order.TransformMessageReferences(msgIndex, -1);
                    foreach (Mission.Trigger trig in order.SkipTriggers)
                        trig.TransformMessageRef(msgIndex, -1);
                }
            }

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Mission.Trigger trig in goal.Triggers)
                        trig.TransformMessageRef(msgIndex, -1);
            
            return Messages.RemoveAt(msgIndex);
        }

        /// <summary>Swaps two Messages, used to move Messages up or down in the list.</summary>
        /// <remarks>Automatically performs bounds checking and adjusts all Flight Group indexes to prevent broken indexes in triggers, orders, etc.</remarks>
        /// <returns>Returns true if an adjustment was performed, false if any index or bounds errors occurred.</returns>
        public bool SwapMessage(int srcIndex, int dstIndex)
        {
            if ((srcIndex < 0 || srcIndex >= Messages.Count) || (dstIndex < 0 || dstIndex >= Messages.Count) || (srcIndex == dstIndex)) return false;

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.SwapMessageReferences(srcIndex, dstIndex);

            foreach (FlightGroup fg in FlightGroups)
            {
                foreach (Mission.Trigger trig in fg.ArrDepTriggers)
                    trig.SwapMessageReferences(srcIndex, dstIndex);

                foreach (FlightGroup.Order order in fg.Orders)
                {
                    order.SwapMessageReferences(srcIndex, dstIndex);
                    foreach (Mission.Trigger trig in order.SkipTriggers)
                        trig.SwapMessageReferences(srcIndex, dstIndex);
                }
            }

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Mission.Trigger trig in goal.Triggers)
                        trig.SwapMessageReferences(srcIndex, dstIndex);

            Message temp = Messages[srcIndex];
            Messages[srcIndex] = Messages[dstIndex];
            Messages[dstIndex] = temp;

            return true;
        }

        /// <summary>Converts a raw time delay into number of seconds.</summary>
        /// <param name="value">The raw value of the time delay.</param>
        /// <remarks>The raw value is used to encode both minutes and seconds.  Maximum range of delay times is 0:00 to 24:50</remarks>
        /// <returns>Number of seconds.</returns>
        public int GetDelaySeconds(byte value)
        {
            int sec = value;                     //XWA calculates wait times different than XvT.
            if (value > 20)
                sec = 20 + ((value - 20) * 5);   //5 seconds per increment
            if (value > 196)
                sec += (value - 196) * 5;        //Above 196 (15:00) it's 10 seconds per increment.  Since we already calculated 5 seconds above, add 5 seconds extra.

            return sec;
        }
        
        #endregion public methods

		#region public properties
		/// <summary>Maximum number of craft that can exist at one time in a single region</summary>
		/// <remarks>Value is <b>96</b></remarks>
		public const int CraftLimit = 96;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file</summary>
		/// <remarks>Value is <b>192</b></remarks>
		public const int FlightGroupLimit = 192;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file</summary>
		/// <remarks>Value is <b>64</b></remarks>
		public const int MessageLimit = 64;
		/// <summary>Unknown FileHeader value</summary>
		/// <remarks>Offset = 0x0B, defaults to <b>true</b></remarks>
		public bool Unknown2 { get; set; }
		/// <summary>Unknown FileHeader value</summary>
		/// <remarks>Offset = 0x08, defaults to <b>true</b></remarks>
		public bool Unknown1 { get; set; }

		/// <summary>Gets the Global Cargos for the mission</summary>
		public GlobCarg[] GlobalCargo { get { return _globalCargo; } }
		/// <summary>Gets or sets the start mode of the player)</summary>
		public HangarEnum MissionType { get; set; }
		/// <summary>Gets or sets the minutes value of the time limit</summary>
		public byte TimeLimitMin { get; set; }
		/// <summary>Gets or sets if the mission will automatically end when Primary goals are complete</summary>
		public bool EndWhenComplete { get; set; }
		/// <summary>Gets or sets the voice of in-game mission update messages</summary>
		public byte Officer { get; set; }
		/// <summary>Gets or sets the Briefing image</summary>
		public LogoEnum Logo { get; set; }
		/// <summary>Unknown FileHeader value</summary>
		/// <remarks>Offset = 0x23B3, default is 0x62, related to Briefings?</remarks>
		public byte Unknown3 { get; set; }
		/// <summary>Unknown FileHeader value</summary>
		/// <remarks>Offset = 0x23B4</remarks>
		public byte Unknown4 { get; set; }
		/// <summary>Unknown FileHeader value</summary>
		/// <remarks>Offset = 0x23B5</remarks>
		public byte Unknown5 { get; set; }
		/// <summary>Gets or sets the summary of the mission</summary>
		/// <remarks>4096 char limit</remarks>
		public string MissionDescription
		{
			get { return _missionDescription.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				_missionDescription = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionDescription"/></summary>
		/// <remarks>100 char limit. Used as voice actor instructions</remarks>
		public string DescriptionNotes
		{
			get { return _descriptionNote; }
			set { _descriptionNote = StringFunctions.GetTrimmed(value, 100); }
		}
		/// <summary>Gets or sets the debriefing text</summary>
		/// <remarks>4096 char limit</remarks>
		public string MissionFailed
		{
			get { return _missionFailed.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				_missionFailed = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionFailed"/></summary>
		/// <remarks>100 char limit. Used as voice actor instructions</remarks>
		public string FailedNotes
		{
			get { return _failedNote; }
			set { _failedNote = StringFunctions.GetTrimmed(value, 100); }
		}
		/// <summary>Gets or sets the debriefing text</summary>
		/// <remarks>4096 char limit</remarks>
		public string MissionSuccessful
		{
			get { return _missionSuccessful.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionSuccessful = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionSuccessful"/></summary>
		/// <remarks>100 char limit. Used as voice actor instructions</remarks>
		public string SuccessfulNotes
		{
			get { return _successfulNote; }
			set { _successfulNote = StringFunctions.GetTrimmed(value, 100); }
		}
		/// <summary>Editor-only notes for the mission</summary>
		/// <remarks>6268 char limit</remarks>
		public string MissionNotes
		{
			get { return _missionNote.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionNote = StringFunctions.GetTrimmed(s, 0x187C);
			}
		}
		/// <summary>Gets or sets the FlightGroups for the mission</summary>
		/// <remarks>Defaults to one FlightGroup</remarks>
		public FlightGroupCollection FlightGroups { get; set; }
		/// <summary>Gets or sets the In-Flight Messages for the mission</summary>
		/// <remarks>Defaults to zero messages</remarks>
		public MessageCollection Messages { get; set; }
		/// <summary>Gets or sets the Global Goals for the mission</summary>
		public GlobalsCollection Globals { get; set; }
		/// <summary>Gets or sets the Teams for the mission</summary>
		public TeamCollection Teams { get; set; }
		/// <summary>Gets or sets the Briefings for the mission</summary>
		public BriefingCollection Briefings { get; set; }
		/// <summary>Gets the array accessor for the GG names</summary>
		public Indexer<string> GlobalGroups { get { return _globalGroupNameIndexer; } }
		/// <summary>Gets the array accessor for the Region names</summary>
		public Indexer<string> Regions { get { return _regionNameIndexer; } }
		/// <summary>Gets the array accessor for the IFF names</summary>
		public Indexer<string> Iffs { get { return _iffNameIndexer; } }
		#endregion public properties
		
		/// <summary>Container for Global Cargo data</summary>
		[Serializable] public struct GlobCarg
		{
			string _cargo;

			/// <summary>Gets or sets the Cargo string</summary>
			/// <remarks>63 character limit</remarks>
			public string Cargo
			{
				get { return _cargo; }
				set { _cargo = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Unknown value, local 0x44</summary>
			public bool Unknown1 { get; set; }
			/// <summary>Unknown value, local 0x48</summary>
			public byte Unknown2 { get; set; }
			/// <summary>Unknown value, local 0x49</summary>
			public byte Unknown3 { get; set; }
			/// <summary>Unknown value, local 0x4A</summary>
			public byte Unknown4 { get; set; }
			/// <summary>Unknown value, local 0x4B</summary>
			public byte Unknown5 { get; set; }
		}
	}
}
