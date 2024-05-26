/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.1+
 */

/* CHANGELOG
 * [NEW] Full format spec implemented
 * [FIX] EoM notes R/W for other teams
 * [UPD] Briefing events I/O
 * v6.1, 231208
 * [UPD] GetDelaySeconds is now static
 * v6.0, 231027
 * [UPD] Changes due to XWA Arr/Dep Method1
 * v5.7.5, 230116
 * [FIX] Message read now uses LengthLimit
 * v5.7.2, 220225
 * [FIX] Missing squadron values from LogoEnum
 * v5.7.1, 220208
 * [FIX] Message Trigger And/Or read now checks for 1 instead of any odd value (JB)
 * v5.7, 220127
 * [FIX] Message.OriginatingFG error during delete [JB]
 * v5.1, 210315
 * [UPD] Trigger And/Or values now read XWA's method of (value & 1) = TRUE. Still only writes 0/1 [Related to YOGEME#48]
 * v5.0, 201004
 * [FIX] Changed Trim to TrimEnd for craft Name and Cargos, as there's the potential for leading \0 which would show the rest of the string
 * v4.0, 200809
 * [UPD] auto-properties
 * [UPD] Better Save backup [JB]
 * [UPD] Message load null term fixed [JB]
 * [UPD] Iffs renamed to IFFs
 * v3.1, 200703
 * [UPD] added backup during save
 * v3.0.1, 180919
 * [FIX] Pitch value check during write
 * v3.0, 180903
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
	/// <summary>Framework for XWA.</summary>
	/// <remarks>This is the primary container object for XWA mission files.</remarks>
	public partial class Mission : MissionFile
	{
		readonly string[] _iff = Strings.IFF;
		string _missionDescription = "#";
		string _missionFailed = "#";
		string _missionSuccessful = "";
		string _missionNote = "";
		string _descriptionNote = "";
		string _failedNote = "";
		string _successfulNote = "";

		/// <summary>Briefing <see cref="Logo"/> values.</summary>
		public enum LogoEnum : byte {
			/// <summary>No logo shown.</summary>
			None,
			/// <summary>Defiance logo.</summary>
			Defiance = 4,
			/// <summary>Liberty logo.</summary>
			Liberty,
			/// <summary>Independance logo.</summary>
			Independance,
			/// <summary>Family logo.</summary>
			AzzameenBase,
			/// <summary>Squadron 1 logo.</summary>
			PhantomSquadron,
			/// <summary>Squadron 2 logo.</summary>
			VectorSquadron,
			/// <summary>Squadron 3 logo.</summary>
			RogueSquadron,
			/// <summary>Squadron 4 logo.</summary>
			FamilyTransport,
			/// <summary>Unknown logo.</summary>
			Unknown_C
		}
		/// <summary>Mission starting <see cref="MissionType"/> (aka Hangar).</summary>
		public enum HangarEnum : byte {
			/// <summary>Junkyard starting location.</summary>
			Junkyard,
			/// <summary>Instant start, only through the Simulator.</summary>
			Simulator1,
			/// <summary>Instant start.</summary>
			QuickStart,
			/// <summary>Instant start, only through the Simulator.</summary>
			Simulator2,
			/// <summary>Skirmish mission type.</summary>
			Skirmish,
			/// <summary>Death Star mission type.</summary>
			DeathStar,
			/// <summary>Standard Rebel mission.</summary>
			MonCalCruiser,
			/// <summary>Standard Family mission.</summary>
			FamilyTransport
		}

		#region constructors
		/// <summary>Default constructor, creates a blank mission.</summary>
		public Mission()
		{
			initialize();
			for (int i = 0; i < 16; i++) { GlobalCargos[i].Cargo = ""; GlobalCargos[i].Count = 1; }
			for (int i = 0; i < 32; i++) { GlobalGroups[i].Name = ""; GlobalGroups[i].SpecialCargo = ""; }
			for (int i = 0; i < 40; i++) { GlobalUnits[i].Name = ""; GlobalUnits[i].SpecialCargo = ""; }
			for (int i = 0; i < 4; i++) { Regions[i].Name = "Region " + (i + 1).ToString(); _iff[i + 2] = ""; }
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
			_invalidError = _invalidError.Replace("{0}", "XWA");
			IFFs = new Indexer<string>(_iff, 19, new bool[] { true, true, false, false, false, false });
			FlightGroups = new FlightGroupCollection();
			Messages = new MessageCollection();
			Globals = new GlobalsCollection();
			Teams = new TeamCollection();
			Briefings = new BriefingCollection();
		}
		#endregion constructors

		#region public methods
		/// <summary>Load a mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		/// <exception cref="FileNotFoundException"><paramref name="filePath"/> cannot be located.</exception>
		/// <exception cref="InvalidDataException"><paramref name="filePath"/> is not a XWA mission file.</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid XWA mission file.</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (GetPlatform(stream) != Platform.XWA) throw new InvalidDataException(_invalidError);
			BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(1252));  //[JB] Changed encoding to windows-1252 (ANSI Latin 1) to ensure proper loading of 8-bit ANSI regardless of the operating system's default code page.
			int i, j;
			long p;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			#region Platform
			LegacyTimeLimitMin = br.ReadByte();
			LegacyTimeLimitSec = br.ReadByte();
			LegacyWinType = br.ReadByte();
			LegacyBackdrop = br.ReadByte();
			LegacyRescue = br.ReadByte();
			LegacyAllWayShown = br.ReadByte();
			LegacyVars = br.ReadBytes(LegacyVars.Length);
			for (i = 2; i < 6; i++) _iff[i] = new string(br.ReadChars(0x14)).Trim('\0');
			for (i = 0; i < 4; i++)
			{
				Regions[i].Name = new string(br.ReadChars(0x40)).Trim('\0');
				Regions[i].ID = br.ReadInt32();
				stream.Position += 0x40;
			}
			for (i = 0; i < 16; i++)
			{
				GlobalCargos[i].Cargo = new string(br.ReadChars(0x40)).Trim('\0');
				GlobalCargos[i].ID = br.ReadInt32();
				GlobalCargos[i].Count = br.ReadInt32();
				GlobalCargos[i].Type = (GlobCarg.CargoTypes)br.ReadByte();
				GlobalCargos[i].Volume = br.ReadByte();
				GlobalCargos[i].Value = br.ReadByte();
				GlobalCargos[i].Volatility = (GlobCarg.VolatilityLevels)br.ReadByte();
				stream.Position += 0x40;
			}
			for (i = 0; i < 32; i++)
			{
				GlobalGroups[i].Name = new string(br.ReadChars(0x40)).Trim('\0');
				GlobalGroups[i].Leader = br.ReadByte();
				GlobalGroups[i].SpecialCraft = br.ReadByte();
				GlobalGroups[i].SpecialCargo = new string(br.ReadChars(0x14)).Trim('\0');
				GlobalGroups[i].RandomSpecialCraft = br.ReadBoolean();
			}
			for (i = 0; i < 40; i++)
			{
				GlobalUnits[i].Name = new string(br.ReadChars(0x40)).Trim('\0');
				GlobalUnits[i].Leader = br.ReadByte();
				GlobalUnits[i].SpecialCraft = br.ReadByte();
				GlobalUnits[i].SpecialCargo = new string(br.ReadChars(0x14)).Trim('\0');
				GlobalUnits[i].RandomSpecialCraft = br.ReadBoolean();
			}
			MissionType = (HangarEnum)br.ReadByte();
			GoalsUnimportant = br.ReadBoolean();
			TimeLimitMin = br.ReadByte();
			EndWhenComplete = br.ReadBoolean();
			Officer = br.ReadByte();
			Logo = (LogoEnum)br.ReadByte();
			BriefingOfficerEntryLine = br.ReadByte();
			SecondaryVersion = br.ReadByte();
			WinOfficer = br.ReadByte();
			FailOfficer = br.ReadByte();
			stream.Position = 0x23F0;
			#endregion
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			byte[] buffer = new byte[64];
			for (i = 0; i < numFlightGroups; i++)
			{
				#region Craft
				p = stream.Position;
				FlightGroups[i].Name = new string(br.ReadChars(0x14)).Trim('\0');
				stream.Read(buffer, 0, 7);
				FlightGroups[i].EnableDesignation1 = buffer[0];
				FlightGroups[i].EnableDesignation2 = buffer[1];
				FlightGroups[i].Designation1 = buffer[2];
				FlightGroups[i].Designation2 = buffer[3];
				FlightGroups[i].Comms = (FlightGroup.CommsVerbosity)buffer[4];
				try { FlightGroups[i].GlobalCargo = (byte)(buffer[5] + 1); }
				catch { FlightGroups[i].GlobalCargo = 0; }
				try { FlightGroups[i].GlobalSpecialCargo = (byte)(buffer[6] + 1); }
				catch { FlightGroups[i].GlobalSpecialCargo = 0; }
				stream.Position += 0xD;
				FlightGroups[i].Cargo = new string(br.ReadChars(0x14)).TrimEnd('\0');
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(0x14)).TrimEnd('\0');
				FlightGroups[i].Role = new string(br.ReadChars(0x14)).TrimEnd('\0');
				stream.Position += 5;
				stream.Read(buffer, 0, 0x1B);
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
				FlightGroups[i].LegacyPermaDeathID = br.ReadInt16();
				#endregion
				#region Arr/Dep
				stream.Read(buffer, 0, 0x3C);
				FlightGroups[i].Difficulty = buffer[1];
				for (j = 0; j < 6; j++)
				{
					FlightGroups[i].ArrDepTriggers[0][j] = buffer[2 + j];   // Arr1...
					FlightGroups[i].ArrDepTriggers[1][j] = buffer[8 + j];
					FlightGroups[i].ArrDepTriggers[2][j] = buffer[0x12 + j];
					FlightGroups[i].ArrDepTriggers[3][j] = buffer[0x18 + j];
					FlightGroups[i].ArrDepTriggers[4][j] = buffer[0x26 + j];    // Dep1...
					FlightGroups[i].ArrDepTriggers[5][j] = buffer[0x2C + j];
				}
				FlightGroups[i].ArrDepAndOr[0] = Convert.ToBoolean(buffer[0x10] & 1);
				FlightGroups[i].ArrDepAndOr[1] = Convert.ToBoolean(buffer[0x20] & 1);
				FlightGroups[i].ArrDepAndOr[2] = Convert.ToBoolean(buffer[0x22] & 1);
				FlightGroups[i].RandomArrivalDelayMinutes = buffer[0x23];
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				FlightGroups[i].ArrDepAndOr[3] = Convert.ToBoolean(buffer[0x34] & 1);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x36];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x37];
				FlightGroups[i].AbortTrigger = buffer[0x38];
				FlightGroups[i].RandomArrivalDelaySeconds = buffer[0x39];
				stream.Read(buffer, 0, 8);
				FlightGroups[i].ArrivalMothership = buffer[0];
				FlightGroups[i].ArriveViaMothership = buffer[1];
				FlightGroups[i].DepartureMothership = buffer[2];
				FlightGroups[i].DepartViaMothership = buffer[3];
				FlightGroups[i].AlternateMothership = buffer[4];
				FlightGroups[i].AlternateMothershipUsed = Convert.ToBoolean(buffer[5]);
				FlightGroups[i].CapturedDepartureMothership = buffer[6];
				FlightGroups[i].CapturedDepartViaMothership = Convert.ToBoolean(buffer[7]);
				#endregion
				#region Orders
				for (j = 0; j < 16; j++)
				{
					FlightGroups[i].Orders[j / 4, j % 4] = new FlightGroup.Order();
					stream.Read(buffer, 0, 0x14);
					for (int h = 0; h < 0x13; h++) FlightGroups[i].Orders[j / 4, j % 4][h] = buffer[h];
					for (int h = 0; h < 8; h++)
						for (int k = 0; k < 4; k++) FlightGroups[i].Orders[j / 4, j % 4].Waypoints[h][k] = (short)(br.ReadInt16() * (k == 1 ? -1 : 1));
					stream.Position += 0x40;
				}
				for (j = 0; j < 16; j++)
				{
					stream.Read(buffer, 0, 0x10);
					for (int h = 0; h < 6; h++)
					{
						FlightGroups[i].Orders[j / 4, j % 4].SkipTriggers[0][h] = buffer[h];
						FlightGroups[i].Orders[j / 4, j % 4].SkipTriggers[1][h] = buffer[h + 6];
					}
					FlightGroups[i].Orders[j / 4, j % 4].SkipT1AndOrT2 = Convert.ToBoolean(buffer[0xE] & 1);
				}
				#endregion
				#region Goals
				for (j = 0; j < 8; j++)
				{
					byte[] temp = new byte[0x10];
					stream.Read(temp, 0, 0x10);
					FlightGroups[i].Goals[j] = new FlightGroup.Goal(temp);
					stream.Position += 0x40;
				}
				#endregion
				for (j = 0; j < 4; j++) for (int k = 0; k < 4; k++) FlightGroups[i].Waypoints[j][k] = (short)(br.ReadInt16() * (k == 1 ? -1 : 1));
				for (j = 0; j < 4; j++) FlightGroups[i].Waypoints[j].Region = br.ReadByte();
				#region Options/other
				stream.Read(buffer, 0, 0x1E);
				FlightGroups[i].GlobalNumbering = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].DepartureClockMin = buffer[0x17];
				FlightGroups[i].DepartureClockSec = buffer[0x18];
				FlightGroups[i].Countermeasures = (FlightGroup.CounterTypes)buffer[0x19];
				FlightGroups[i].ExplosionTime = buffer[0x1A];
				FlightGroups[i].Status2 = buffer[0x1B];
				FlightGroups[i].GlobalUnit = buffer[0x1C];
				FlightGroups[i].Handicap = buffer[0x1D];

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
				stream.Position = p + 0xE3E;
				#endregion
			}
			#endregion
			#region Messages
			if (numMessages != 0)
			{
				Messages = new MessageCollection(numMessages);
				for (i = 0; i < numMessages; i++)
				{
					stream.Position += 2;
					Messages[i].MessageString = new string(br.ReadChars(Message.LengthLimit));
					if (Messages[i].MessageString.IndexOf('\0') != -1) Messages[i].MessageString = Messages[i].MessageString.Substring(0, Messages[i].MessageString.IndexOf('\0'));
					stream.Position += 0x0C;
					stream.Read(buffer, 0, 0xA);
					for (j = 0; j < 10; j++) Messages[i].SentTo[j] = Convert.ToBoolean(buffer[j]);
					stream.Read(buffer, 0, 0x20);
					for (j = 0; j < 6; j++)
					{
						Messages[i].Triggers[0][j] = buffer[j]; // T1...
						Messages[i].Triggers[1][j] = buffer[6 + j];
						Messages[i].Triggers[2][j] = buffer[0x10 + j];
						Messages[i].Triggers[3][j] = buffer[0x16 + j];
					}
					Messages[i].TrigAndOr[0] = (buffer[0xE] == 1);
					Messages[i].TrigAndOr[1] = (buffer[0x1E] == 1);
					Messages[i].VoiceID = new string(br.ReadChars(8)).Trim('\0');
					Messages[i].OriginatingFG = br.ReadByte();
					stream.Position += 3;
					Messages[i].Type = br.ReadInt32();
					stream.Read(buffer, 0, 0x16);
					Messages[i].RawDelay = buffer[0];
					Messages[i].TrigAndOr[2] = (buffer[1] == 1);
					Messages[i].Color = buffer[2];
					Messages[i].SpeakerHeader = Convert.ToBoolean(buffer[3]);

					for (j = 0; j < 6; j++)
					{
						Messages[i].Triggers[4][j] = buffer[4 + j]; // CancelT1...
						Messages[i].Triggers[5][j] = buffer[0xA + j];
					}
					Messages[i].TrigAndOr[3] = Convert.ToBoolean(buffer[0x12] & 1);
					Messages[i].CancelMeaning = buffer[0x14];
				}
			}
			else Messages.Clear();
			#endregion
			#region Globals
			Globals.ClearAll();
			for (i = 0; i < Globals.Count; i++)
			{
				var readCount = br.ReadInt16();
				for (int k = 0; k < 3; k++)
				{
					p = stream.Position;
					stream.Read(buffer, 0, 0xE);
					for (j = 0; j < 6; j++)
					{
						Globals[i].Goals[k].Triggers[0][j] = buffer[j];
						Globals[i].Goals[k].Triggers[1][j] = buffer[j + 6];
					}
					Globals[i].Goals[k].T1AndOrT2 = Convert.ToBoolean(br.ReadByte() & 1);
					stream.Position++;
					stream.Read(buffer, 0, 0xE);
					for (j = 0; j < 6; j++)
					{
						Globals[i].Goals[k].Triggers[2][j] = buffer[j];
						Globals[i].Goals[k].Triggers[3][j] = buffer[j + 6];
					}
					Globals[i].Goals[k].T3AndOrT4 = Convert.ToBoolean(br.ReadByte() & 1);
					stream.Position++;
					Globals[i].Goals[k].Name = new string(br.ReadChars(16)).Trim('\0');
					Globals[i].Goals[k].Version = br.ReadByte();
					Globals[i].Goals[k].T12AndOrT34 = Convert.ToBoolean(br.ReadByte() & 1);
					stream.Read(buffer, 0, 7);
					Globals[i].Goals[k].RawDelay = buffer[0];
					Globals[i].Goals[k].RawPoints = (sbyte)buffer[1];
					for (int n = 0; n < 4; n++) Globals[i].Goals[k].RawPointsPerTrigger[n] = (sbyte)buffer[2 + n];
					Globals[i].Goals[k].ActiveSequence = buffer[6];
					stream.Position = p + 0x7A;
				}
				if (readCount > 3) for (int k = 3; k < readCount; k++) stream.Position += 0x7A; // account for the extras, but ignore entirely
			}
			#endregion
			#region Teams
			Teams.ClearAll();
			for (i = 0; i < Teams.Count; i++)
			{
				stream.Position += 2;
				Teams[i].Name = new string(br.ReadChars(0x10)).Trim('\0');  // null-termed
				stream.Position += 8;
				for (j = 0; j < 10; j++) Teams[i].Allies[j] = (Team.Allegeance)br.ReadByte();
				for (j = 0; j < 6; j++) Teams[i].EndOfMissionMessages[j] = new string(br.ReadChars(0x40)).Trim('\0');
				stream.Read(Teams[i].EomRawDelay, 0, 3);
				stream.Read(Teams[i].EomSourceFG, 0, 3);
				for (j = 0; j < 3; j++) Teams[i].VoiceIDs[j] = new string(br.ReadChars(0x14)).Trim('\0');
				stream.Position++;
			}
			#endregion
			#region Briefing
			Briefings.ClearAll();
			for (i = 0; i < 2; i++)
			{
				Briefings[i].Length = br.ReadInt16();
				stream.Position += 6;   // CurrentTime, StartLength, EventsLength
				Briefings[i].Tile = br.ReadInt16();
				byte[] rawEvents = br.ReadBytes(Briefing.EventQuantityLimit * 4);
				Briefings[i].Events = new BaseBriefing.EventCollection(Platform.XWA, rawEvents);
				for (j = 0; j < 192; j++)
				{
					Briefings[i].Icons[j].Species = br.ReadByte();
					Briefings[i].Icons[j].IFF = br.ReadByte();
					Briefings[i].Icons[j].X = br.ReadInt16();
					Briefings[i].Icons[j].Y = br.ReadInt16();
					Briefings[i].Icons[j].Orientation = br.ReadInt16();
					stream.Position += 0x10;
				}
				stream.Read(buffer, 0, 0xA);
				for (j = 0; j < 10; j++) Briefings[i].Team[j] = Convert.ToBoolean(buffer[j]);
				for (j = 0; j < 128; j++)
				{
					int k = br.ReadInt16();
					if (k > 0) Briefings[i].BriefingTag[j] = new string(br.ReadChars(k)).Trim('\0');    // shouldn't need the trim
				}
				for (j = 0; j < 128; j++)
				{
					int k = br.ReadInt16();
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
			for (i = 0; i < 10; i++)
				for (j = 0; j < 7; j++)
					if (j < 3) Teams[i].EomNotes[j] = new string(br.ReadChars(0x64)).Trim('\0');
					else stream.Position += 0x64;
			_descriptionNote = new string(br.ReadChars(0x64)).Trim('\0');
			_successfulNote = new string(br.ReadChars(0x64)).Trim('\0');
			_failedNote = new string(br.ReadChars(0x64)).Trim('\0');
			#endregion
			#region FG goal strings
			for (i = 0; i < FlightGroups.Count; i++)
				for (j = 0; j < 8; j++) // per goal
					for (int k = 0; k < 3; k++) // per string
						if (br.ReadByte() != 0)
						{
							stream.Position--;
							if (k == 0) FlightGroups[i].Goals[j].IncompleteText = new string(br.ReadChars(0x40)).Trim('\0');
							else if (k == 1) FlightGroups[i].Goals[j].CompleteText = new string(br.ReadChars(0x40)).Trim('\0');
							else FlightGroups[i].Goals[j].FailedText = new string(br.ReadChars(0x40)).Trim('\0');
						}
			#endregion
			#region Globals strings
			for (i = 0; i < 10; i++)    // Team
				for (j = 0; j < 28; j++)    // Goal * Trigger
					for (int k = 0; k < 3; k++) // State
						if (br.ReadByte() != 0)
						{
							if (j >= 12) { stream.Position += 0x3F; continue; } // skip the extras, unlikely
							if (j >= 8 && k == 0) { stream.Position += 0x3F; continue; }    // skip Sec Inc
							if (j >= 4 && k == 2) { stream.Position += 0x3F; continue; }    // skip Prev & Sec Fail
							stream.Position--;
							Globals[i].Goals[j / 4].GoalStrings[j % 4, k] = new string(br.ReadChars(0x40));
						}
			#endregion
			#region Order strings
			for (i = 0; i < 192; i++) // per FG (and then some)
				for (j = 0; j < 16; j++) // per order
					if (br.ReadByte() != 0)
					{
						if (i >= FlightGroups.Count) { stream.Position += 0x3F; continue; } // skip if FG doesn't exist
						stream.Position--;
						FlightGroups[i].Orders[j / 4, j % 4].CustomText = new string(br.ReadChars(0x40)).Trim('\0');
					}
			#endregion
			_missionSuccessful = new string(br.ReadChars(0x1000)).TrimEnd('\0');
			_missionFailed = new string(br.ReadChars(0x1000)).TrimEnd('\0');
			_missionDescription = new string(br.ReadChars(0x1000)).TrimEnd('\0');
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
				bw.Write((short)0x12);
				bw.Write((short)FlightGroups.Count);
				bw.Write((short)Messages.Count);
				bw.Write(LegacyTimeLimitMin);
				bw.Write(LegacyTimeLimitSec);
				bw.Write(LegacyWinType);
				bw.Write(LegacyBackdrop);
				bw.Write(LegacyRescue);
				bw.Write(LegacyAllWayShown);
				bw.Write(LegacyVars);
				for (i = 2; i < 6; i++)
				{
					p = fs.Position;
					bw.Write(_iff[i].ToCharArray());
					fs.Position = p + 0x14;
				}
				for (i = 0; i < 4; i++)
				{
					p = fs.Position;
					bw.Write(Regions[i].Name.ToCharArray());
					fs.Position = p + 0x40;
					bw.Write(Regions[i].ID);
					fs.Position += 0x40;
				}
				for (i = 0; i < 16; i++)
				{
					p = fs.Position;
					bw.Write(GlobalCargos[i].Cargo.ToCharArray());
					fs.Position = p + 0x40;
					bw.Write(GlobalCargos[i].ID);
					bw.Write(GlobalCargos[i].Count);
					bw.Write((byte)GlobalCargos[i].Type);
					bw.Write(GlobalCargos[i].Volume);
					bw.Write(GlobalCargos[i].Value);
					bw.Write((byte)GlobalCargos[i].Volatility);
					fs.Position += 0x40;
				}
				for (i = 0; i < 32; i++)
				{
					p = fs.Position;
					bw.Write(GlobalGroups[i].Name.ToCharArray());
					fs.Position = p + 0x40;
					bw.Write(GlobalGroups[i].Leader);
					bw.Write(GlobalGroups[i].SpecialCraft);
					bw.Write(GlobalGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x56;
					bw.Write(GlobalGroups[i].RandomSpecialCraft);
				}
				for (i = 0; i < 40; i++)
				{
					p = fs.Position;
					bw.Write(GlobalUnits[i].Name.ToCharArray());
					fs.Position = p + 0x40;
					bw.Write(GlobalUnits[i].Leader);
					bw.Write(GlobalUnits[i].SpecialCraft);
					bw.Write(GlobalUnits[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x56;
					bw.Write(GlobalUnits[i].RandomSpecialCraft);
				}
				bw.Write((byte)MissionType);
				bw.Write(GoalsUnimportant);
				bw.Write(TimeLimitMin);
				bw.Write(EndWhenComplete);
				bw.Write(Officer);
				bw.Write((byte)Logo);
				bw.Write(BriefingOfficerEntryLine);
				bw.Write(SecondaryVersion);
				bw.Write(WinOfficer);
				bw.Write(FailOfficer);
				fs.Position = 0x23F0;
				#endregion
				#region FlightGroups
				for (i = 0; i < FlightGroups.Count; i++)
				{
					p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0x14;
					bw.Write(FlightGroups[i].EnableDesignation1);
					bw.Write(FlightGroups[i].EnableDesignation2);
					bw.Write(FlightGroups[i].Designation1);
					bw.Write(FlightGroups[i].Designation2);
					bw.Write((byte)FlightGroups[i].Comms);
					bw.Write((byte)(FlightGroups[i].GlobalCargo == 0 ? 255 : FlightGroups[i].GlobalCargo - 1));
					bw.Write((byte)(FlightGroups[i].GlobalSpecialCargo == 0 ? 255 : FlightGroups[i].GlobalSpecialCargo - 1));
					fs.Position = p + 0x28;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x3C;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x50;
					bw.Write(FlightGroups[i].Role.ToCharArray());
					fs.Position = p + 0x69;
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
					fs.Position++;
					bw.Write(FlightGroups[i].LegacyPermaDeathID);
					#endregion
					#region Arr/Dep
					fs.Position++;
					bw.Write(FlightGroups[i].Difficulty);
					bw.Write(FlightGroups[i].ArrDepTriggers[0].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[1].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[0]);
					fs.Position++;
					bw.Write(FlightGroups[i].ArrDepTriggers[2].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[3].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[1]);
					fs.Position++;
					bw.Write(FlightGroups[i].ArrDepAndOr[2]);
					bw.Write(FlightGroups[i].RandomArrivalDelayMinutes);
					bw.Write(FlightGroups[i].ArrivalDelayMinutes);
					bw.Write(FlightGroups[i].ArrivalDelaySeconds);
					bw.Write(FlightGroups[i].ArrDepTriggers[4].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[5].GetBytes());
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[3]);
					fs.Position++;
					bw.Write(FlightGroups[i].DepartureTimerMinutes);
					bw.Write(FlightGroups[i].DepartureTimerSeconds);
					bw.Write(FlightGroups[i].AbortTrigger);
					bw.Write(FlightGroups[i].RandomArrivalDelaySeconds);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrivalMothership);
					bw.Write(FlightGroups[i].ArriveViaMothership);
					bw.Write(FlightGroups[i].DepartureMothership);
					bw.Write(FlightGroups[i].DepartViaMothership);
					bw.Write(FlightGroups[i].AlternateMothership);
					bw.Write(FlightGroups[i].AlternateMothershipUsed);
					bw.Write(FlightGroups[i].CapturedDepartureMothership);
					bw.Write(FlightGroups[i].CapturedDepartViaMothership);
					#endregion
					#region Orders
					for (j = 0; j < 16; j++)
					{
						bw.Write(FlightGroups[i].Orders[j / 4, j % 4].GetBytes());
						for (int h = 0; h < 8; h++)
							for (int k = 0; k < 4; k++) bw.Write((short)(FlightGroups[i].Orders[j / 4, j % 4].Waypoints[h][k] * (k == 1 ? -1 : 1)));
						fs.Position += 0x40;
					}
					for (j = 0; j < 16; j++)
					{
						bw.Write(FlightGroups[i].Orders[j / 4, j % 4].SkipTriggers[0].GetBytes());
						bw.Write(FlightGroups[i].Orders[j / 4, j % 4].SkipTriggers[1].GetBytes());
						fs.Position += 2;
						bw.Write(FlightGroups[i].Orders[j / 4, j % 4].SkipT1AndOrT2);
						fs.Position++;
					}
					#endregion
					#region Goals
					for (j = 0; j < 8; j++)
					{
						bw.Write(FlightGroups[i].Goals[j].GetBytes());
						fs.Position += 0x40;
					}
					#endregion
					// SP1 0,0,0 check for backdrops
					if (FlightGroups[i].CraftType == 0xB7 && FlightGroups[i].Waypoints[0].RawX == 0 && FlightGroups[i].Waypoints[0].RawY == 0 && FlightGroups[i].Waypoints[0].RawZ == 0)
						FlightGroups[i].Waypoints[0].RawY = 10;
					for (j = 0; j < 4; j++) for (int k = 0; k < 4; k++) bw.Write((short)(FlightGroups[i].Waypoints[j][k] * (k == 1 ? -1 : 1)));
					for (j = 0; j < 4; j++) bw.Write(FlightGroups[i].Waypoints[j].Region);
					#region Options/other
					fs.Position = p + 0xDC4;
					bw.Write(FlightGroups[i].GlobalNumbering);
					bw.Write(FlightGroups[i].DepartureClockMin);
					bw.Write(FlightGroups[i].DepartureClockSec);
					bw.Write((byte)FlightGroups[i].Countermeasures);
					bw.Write(FlightGroups[i].ExplosionTime);
					bw.Write(FlightGroups[i].Status2);
					bw.Write(FlightGroups[i].GlobalUnit);
					bw.Write(FlightGroups[i].Handicap);

					for (j = 1; j < 9; j++) if (FlightGroups[i].OptLoadout[j]) bw.Write((byte)j); else fs.Position++;   // warheads
					for (j = 1; j < 5; j++) if (FlightGroups[i].OptLoadout[j + 9]) bw.Write((byte)j); else fs.Position++;   // CMs
					fs.Position += 2;   // only writing 4, 2 bytes remain 
					for (j = 1; j < 4; j++) if (FlightGroups[i].OptLoadout[j + 14]) bw.Write((byte)j); else fs.Position++;  // beam
					fs.Position += 1;   // only writing 3, 1 byte remains

					bw.Write((byte)FlightGroups[i].OptCraftCategory);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].CraftType);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].NumberOfCraft);
					for (int k = 0; k < 10; k++) bw.Write(FlightGroups[i].OptCraft[k].NumberOfWaves);
					bw.Write(FlightGroups[i].PilotID.ToCharArray());
					fs.Position = p + 0xE12;
					bw.Write(FlightGroups[i].Backdrop);
					fs.Position = p + 0xE3E;
					#endregion
				}
				#endregion
				#region Messages
				for (i = 0; i < Messages.Count; i++)
				{
					p = fs.Position;
					bw.Write((short)i);
					bw.Write(Messages[i].MessageString.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x52;
					for (int j = 0; j < 10; j++) bw.Write(Messages[i].SentTo[j]);
					bw.Write(Messages[i].Triggers[0].GetBytes());
					bw.Write(Messages[i].Triggers[1].GetBytes());
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[0]);
					fs.Position++;
					bw.Write(Messages[i].Triggers[2].GetBytes());
					bw.Write(Messages[i].Triggers[3].GetBytes());
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[1]);
					fs.Position++;
					bw.Write(Messages[i].VoiceID.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x84;
					bw.Write(Messages[i].OriginatingFG);
					fs.Position += 3;
					bw.Write(Messages[i].Type);
					bw.Write(Messages[i].RawDelay);
					bw.Write(Messages[i].TrigAndOr[2]);
					bw.Write(Messages[i].Color);
					bw.Write(Messages[i].SpeakerHeader);
					bw.Write(Messages[i].Triggers[4].GetBytes());
					bw.Write(Messages[i].Triggers[5].GetBytes());
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[3]);
					fs.Position++;
					bw.Write(Messages[i].CancelMeaning);
					fs.Position++;
				}
				#endregion
				#region Globals
				for (i = 0; i < Globals.Count; i++)
				{
					bw.Write((short)3);
					int j;
					for (int k = 0; k < 3; k++)
					{
						p = fs.Position;
						bw.Write(Globals[i].Goals[k].Triggers[0].GetBytes());
						bw.Write(Globals[i].Goals[k].Triggers[1].GetBytes());
						fs.Position += 2;
						bw.Write(Globals[i].Goals[k].T1AndOrT2);
						fs.Position++;
						bw.Write(Globals[i].Goals[k].Triggers[2].GetBytes());
						bw.Write(Globals[i].Goals[k].Triggers[3].GetBytes());
						fs.Position += 2;
						bw.Write(Globals[i].Goals[k].T3AndOrT4);
						fs.Position++;
						bw.Write(Globals[i].Goals[k].Name.ToCharArray()); bw.Write('\0');
						fs.Position = p + 0x30;
						bw.Write(Globals[i].Goals[k].T12AndOrT34);
						bw.Write(Globals[i].Goals[k].RawDelay);
						bw.Write((byte)Globals[i].Goals[k].RawPoints);
						for (j = 0; j < 4; j++) bw.Write((byte)Globals[i].Goals[k].RawPointsPerTrigger[j]);
						bw.Write(Globals[i].Goals[k].ActiveSequence);
						fs.Position = p + 0x7A;
					}
				}
				#endregion
				#region Teams
				for (i = 0; i < Teams.Count; i++)
				{
					p = fs.Position;
					bw.Write((short)1);
					bw.Write(Teams[i].Name.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x1A;
					for (int j = 0; j < 10; j++) bw.Write((byte)Teams[i].Allies[j]);
					for (int j = 0; j < 6; j++)
					{
						bw.Write(Teams[i].EndOfMissionMessages[j].ToCharArray()); bw.Write('\0');
						fs.Position = p + 0x24 + (j + 1) * 0x40;
					}
					fs.Write(Teams[i].EomRawDelay, 0, 3);
					fs.Write(Teams[i].EomSourceFG, 0, 3);
					for (int j = 0; j < 3; j++)
					{
						bw.Write(Teams[i].VoiceIDs[j].ToCharArray()); bw.Write('\0');
						fs.Position = p + 0x1AA + (j + 1) * 0x14;
					}
					fs.Position = p + 0x1E7;
				}
				#endregion
				#region Briefing
				for (i = 0; i < 2; i++)
				{
					bw.Write(Briefings[i].Length);
					bw.Write(Briefings[i].CurrentTime);
					bw.Write(Briefings[i].StartLength);
					bw.Write(Briefings[i].EventsLength);
					bw.Write(Briefings[i].Tile);
					byte[] briefBuffer = new byte[Briefing.EventQuantityLimit * 2];
					Buffer.BlockCopy(Briefings[i].Events.GetArray(), 0, briefBuffer, 0, Briefings[i].Events.Length * 2);
					bw.Write(briefBuffer);
					for (int j = 0; j < 192; j++)
					{
						bw.Write(Briefings[i].Icons[j].Species);
						bw.Write(Briefings[i].Icons[j].IFF);
						bw.Write(Briefings[i].Icons[j].X);
						bw.Write(Briefings[i].Icons[j].Y);
						bw.Write(Briefings[i].Icons[j].Orientation);
						fs.Position += 0x10;
					}
					for (int j = 0; j < 10; j++) bw.Write(Briefings[i].Team[j]);
					for (int j = 0; j < 128; j++)
					{
						bw.Write((short)Briefings[i].BriefingTag[j].Length);
						if (Briefings[i].BriefingTag[j].Length != 0) bw.Write(Briefings[i].BriefingTag[j].ToCharArray());
					}
					for (int j = 0; j < 128; j++)
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
				for (i = 0; i < 64; i++)
				{
					p = fs.Position;
					if (i < Messages.Count) bw.Write(Messages[i].Note.ToCharArray());
					fs.Position = p + 0x64;
				}
				for (i = 0; i < 10; i++)
					for (int j = 0; j < 7; j++)
					{
						p = fs.Position;
						if (j < 3) bw.Write(Teams[i].EomNotes[j].ToCharArray());
						fs.Position = p + 0x64;
					}
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
				for (i = 0; i < FlightGroups.Count; i++)
					for (int j = 0; j < 8; j++) // per goal
						for (int k = 0; k < 3; k++) // per string
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
				for (i = 0; i < 10; i++)    // Team
					for (int j = 0; j < 28; j++)    // Goal * Trigger, remember there's 7 even though we only use 3
					{
						if (j >= 12) { fs.Position += 3; continue; }    // make space
						for (int k = 0; k < 3; k++) // State
							if (Globals[i].Goals[j / 4].GoalStrings[j % 4, k] != "")
							{
								p = fs.Position;
								bw.Write(Globals[i].Goals[j / 4].GoalStrings[j % 4, k].ToCharArray());
								fs.Position = p + 0x40;
							}
							else fs.Position++;
					}
				#endregion
				#region Order strings
				for (i = 0; i < 192; i++) // per FG (and then some)
					for (int j = 0; j < 16; j++) // per order
						if (i < FlightGroups.Count && FlightGroups[i].Orders[j / 4, j % 4].CustomText != "")
						{
							p = fs.Position;
							bw.Write(FlightGroups[i].Orders[j / 4, j % 4].CustomText.ToCharArray()); bw.Write('\0');
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
				if (_missionDescription.Length < 0x1000 - 3)
				{
					fs.Position -= 2;
					bw.Write((short)0x2106);
				}

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

        /// <summary>Deletes a Flight Group, performing all necessary cleanup to avoid broken indexes.</summary>
		/// <param name="fgIndex">The FG index to remove.</param>
        /// <remarks>Propagates throughout all members which may reference Flight Group indexes.</remarks>
        /// <returns>Index of the next available Flight Group.</returns>
        public int DeleteFG(int fgIndex)
        {
            if (fgIndex < 0 || fgIndex >= FlightGroups.Count) return 0;  //If out of range, do nothing and return selection to first item.

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Trigger trig in goal.Triggers)
                        trig.TransformFGReferences(fgIndex, -1, false);

			foreach (Message msg in Messages)
			{
				foreach (Trigger trig in msg.Triggers)
					trig.TransformFGReferences(fgIndex, -1, true);
				if (msg.OriginatingFG == fgIndex)
					msg.OriginatingFG = 0;
				else if (msg.OriginatingFG > fgIndex)
					msg.OriginatingFG--;
			}

            //XWA Briefing does not use FG indexes.

            //Skip triggers are processed by the Orders, which are processed by the FlightGroup.
            foreach (FlightGroup fg in FlightGroups)
                fg.TransformFGReferences(fgIndex, -1);

            return FlightGroups.RemoveAt(fgIndex);
        }

        /// <summary>Swaps two FlightGroups, used to move FGs up or down in the list.</summary>
		/// <param name="srcIndex">The original FG index location.</param>
		/// <param name="dstIndex">The new FG index location.</param>
        /// <remarks>Automatically performs bounds checking and adjusts all Flight Group indexes to prevent broken indexes in triggers, orders, etc.</remarks>
        /// <returns>Returns <b>true</b> if an adjustment was performed, <b>false</b> if index validation failed.</returns>
        public bool SwapFG(int srcIndex, int dstIndex)
        {
            if ((srcIndex < 0 || srcIndex >= FlightGroups.Count) || (dstIndex < 0 || dstIndex >= FlightGroups.Count) || (srcIndex == dstIndex)) return false;

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Trigger trig in goal.Triggers)
                        trig.SwapFGReferences(srcIndex, dstIndex);

            foreach (Message msg in Messages)
                foreach (Trigger trig in msg.Triggers)
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
		/// <param name="msgIndex">The Message index to remove.</param>
        /// <remarks>Iterates throughout all members which may reference Message indexes.</remarks>
        /// <returns>Index of the next available Message.</returns>
        public int DeleteMessage(int msgIndex)
        {
            if (msgIndex < 0 || msgIndex >= Messages.Count) return 0;  //If out of range, do nothing and return selection to first item.

            foreach (Message msg in Messages)
                foreach (Trigger trig in msg.Triggers)
                    trig.TransformMessageRef(msgIndex, -1);

            foreach (FlightGroup fg in FlightGroups)
            {
                foreach (Trigger trig in fg.ArrDepTriggers)
                    trig.TransformMessageRef(msgIndex, -1);

                foreach (FlightGroup.Order order in fg.Orders)
                {
                    order.TransformMessageReferences(msgIndex, -1);
                    foreach (Trigger trig in order.SkipTriggers)
                        trig.TransformMessageRef(msgIndex, -1);
                }
            }

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Trigger trig in goal.Triggers)
                        trig.TransformMessageRef(msgIndex, -1);
            
            return Messages.RemoveAt(msgIndex);
        }

        /// <summary>Swaps two Messages, used to move Messages up or down in the list.</summary>
		/// <param name="srcIndex">The original Message index location.</param>
		/// <param name="dstIndex">The new Message index location.</param>
        /// <remarks>Automatically performs bounds checking and adjusts all Flight Group indexes to prevent broken indexes in triggers, orders, etc.</remarks>
        /// <returns>Returns <b>true</b> if an adjustment was performed, <b>false</b> if any index or bounds errors occurred.</returns>
        public bool SwapMessage(int srcIndex, int dstIndex)
        {
            if ((srcIndex < 0 || srcIndex >= Messages.Count) || (dstIndex < 0 || dstIndex >= Messages.Count) || (srcIndex == dstIndex)) return false;

            foreach (Message msg in Messages)
                foreach (Trigger trig in msg.Triggers)
                    trig.SwapMessageReferences(srcIndex, dstIndex);

            foreach (FlightGroup fg in FlightGroups)
            {
                foreach (Trigger trig in fg.ArrDepTriggers)
                    trig.SwapMessageReferences(srcIndex, dstIndex);

                foreach (FlightGroup.Order order in fg.Orders)
                {
                    order.SwapMessageReferences(srcIndex, dstIndex);
                    foreach (Trigger trig in order.SkipTriggers)
                        trig.SwapMessageReferences(srcIndex, dstIndex);
                }
            }

            foreach (Globals global in Globals)
                foreach (Globals.Goal goal in global.Goals)
                    foreach (Trigger trig in goal.Triggers)
                        trig.SwapMessageReferences(srcIndex, dstIndex);

            Message temp = Messages[srcIndex];
            Messages[srcIndex] = Messages[dstIndex];
            Messages[dstIndex] = temp;

            return true;
        }

        /// <summary>Converts a raw time delay into number of seconds.</summary>
        /// <param name="value">The raw value of the time delay.</param>
        /// <remarks>The raw value is used to encode both minutes and seconds.  Maximum range of delay times is 0:00 to 24:50.</remarks>
        /// <returns>Number of seconds.</returns>
        public static int GetDelaySeconds(byte value)
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
		/// <summary>Maximum number of craft that can exist at one time in a single region.</summary>
		public const int CraftLimit = 96;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file.</summary>
		public const int FlightGroupLimit = 192;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file.</summary>
		public const int MessageLimit = 64;
		#region header
		/// <summary>No effect.</summary>
		public byte LegacyTimeLimitMin { get; set; }
		/// <summary>No effect.</summary>
		public byte LegacyTimeLimitSec { get; set; }
		/// <summary>Possibly no effect.</summary>
		/// <remarks>Defaults to <b>1</b>.</remarks>
		public byte LegacyWinType { get; set; } = 1;
		/// <summary>Probably no effect.</summary>
		public byte LegacyBackdrop { get; set; }
		/// <summary>Probably no effect.</summary>
		public byte LegacyRescue { get; set; }
		/// <summary>Probably no effect.</summary>
		/// <remarks>Defaults to <b>1</b>.</remarks>
		public byte LegacyAllWayShown { get; set; } = 1;
		/// <summary>Probably no effect.</summary>
		public byte[] LegacyVars { get; private set; } = new byte[8];
		/// <summary>Gets the array accessor for the IFF names.</summary>
		/// <remarks>No custom Indexer for XWA due to no special processing of values.</remarks>
		public Indexer<string> IFFs { get; private set; }
		/// <summary>Gets the Regions for the mission.</summary>
		public Region[] Regions { get; } = new Region[4];
		/// <summary>Gets the Global Cargos for the mission.</summary>
		public GlobCarg[] GlobalCargos { get; } = new GlobCarg[16];
		/// <summary>Gets the Global Group details for the mission.</summary>
		public GlobalUnit[] GlobalGroups { get; } = new GlobalUnit[32];
		/// <summary>Gets the Globa Unit details for the mission.</summary>
		public GlobalUnit[] GlobalUnits { get; } = new GlobalUnit[40];
		/// <summary>Gets or sets the start mode of the player.</summary>
		/// <remarks>Defaults to <see cref="HangarEnum.MonCalCruiser"/>.</remarks>
		public HangarEnum MissionType { get; set; } = HangarEnum.MonCalCruiser;
		/// <summary>Gets or sets if Goals impact Mission Complete.</summary>
		public bool GoalsUnimportant { get; set; }
		/// <summary>Gets or sets the minutes value of the time limit.</summary>
		public byte TimeLimitMin { get; set; }
		/// <summary>Gets or sets if the mission will automatically end when Primary goals are complete.</summary>
		public bool EndWhenComplete { get; set; }
		/// <summary>Gets or sets the voice of in-game mission update messages.</summary>
		public byte Officer { get; set; }
		/// <summary>Gets or sets the Briefing image.</summary>
		public LogoEnum Logo { get; set; } = LogoEnum.None;
		/// <summary>Gets or sets the Briefing String index used while walking into the briefing.</summary>
		public byte BriefingOfficerEntryLine { get; set; }
		/// <summary>Used in conjunction with <see cref="PlatformID"/>.</summary>
		/// <remarks>Default is <b>0x62</b>, <b>'b'</b>, might not be used in-game.</remarks>
		public byte SecondaryVersion { get; set; } = 0x62;
		/// <summary>Gets or sets the voice at mission complete debrief.</summary>
		public byte WinOfficer { get; set; }
		/// <summary>Gets or sets the voice at the mission failed debrief.</summary>
		public byte FailOfficer { get; set; }
		#endregion header

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

		/// <summary>Editor-only notes for the mission.</summary>
		/// <remarks>6268 char limit.</remarks>
		public string MissionNotes
		{
			get => _missionNote.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionNote = StringFunctions.GetTrimmed(s, 0x187C);
			}
		}
		/// <summary>Gets or sets the summary of the mission.</summary>
		/// <remarks>4096 char limit.</remarks>
		public string MissionDescription
		{
			get => _missionDescription.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				_missionDescription = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionDescription"/>.</summary>
		/// <remarks>100 char limit. Used as voice actor instructions.</remarks>
		public string DescriptionNotes
		{
			get => _descriptionNote;
			set => _descriptionNote = StringFunctions.GetTrimmed(value, 100);
		}
		/// <summary>Gets or sets the debriefing text.</summary>
		/// <remarks>4096 char limit.</remarks>
		public string MissionFailed
		{
			get => _missionFailed.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				_missionFailed = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionFailed"/>.</summary>
		/// <remarks>100 char limit. Used as voice actor instructions.</remarks>
		public string FailedNotes
		{
			get => _failedNote;
			set => _failedNote = StringFunctions.GetTrimmed(value, 100);
		}
		/// <summary>Gets or sets the debriefing text.</summary>
		/// <remarks>4096 char limit.</remarks>
		public string MissionSuccessful
		{
			get => _missionSuccessful.Replace("$", "\r\n");
			set
			{
				string s = value.Replace("\r\n", "$");
				_missionSuccessful = StringFunctions.GetTrimmed(s, 4096);
			}
		}
		/// <summary>Gets or sets the notes attributed to <see cref="MissionSuccessful"/>.</summary>
		/// <remarks>100 char limit. Used as voice actor instructions.</remarks>
		public string SuccessfulNotes
		{
			get => _successfulNote;
			set => _successfulNote = StringFunctions.GetTrimmed(value, 100);
		}
		#endregion public properties

		/// <summary>Container for Global Cargo data.</summary>
		[Serializable]
		public struct GlobCarg
		{
			string _cargo;

			/// <summary>Material type values.</summary>
			public enum CargoTypes : byte {
				/// <summary>Solid state.</summary>
				Solid,
				/// <summary>Liquid state.</summary>
				Liquid,
				/// <summary>Gas or plasma state.</summary>
				Gas
			}

			/// <summary>Explosivness values.</summary>
			public enum VolatilityLevels : byte {
				/// <summary>Low to no effect.</summary>
				Low,
				/// <summary>Normal.</summary>
				Medium,
				/// <summary>More explosive.</summary>
				High,
				/// <summary>Very explosive.</summary>
				Kaboom
			}

			/// <summary>Get the description of the cargo.</summary>
			/// <remarks>Format is "[Type] [Cargo], [Volatility] explosiveness".</remarks>
			/// <returns>Fromatted string if <see cref="Cargo"/> is defined, otherwise "<b>None</b>".</returns>
			public override string ToString() => _cargo != "" ? Enum.GetName(typeof(CargoTypes), Type) + " " + _cargo + ", " + Enum.GetName(typeof(VolatilityLevels), Volatility) + " explosiveness" : "None";

			/// <summary>Gets or sets the Cargo string.</summary>
			/// <remarks>63 character limit.</remarks>
			public string Cargo
			{
				get => _cargo;
				set => _cargo = StringFunctions.GetTrimmed(value, 63);
			}
			/// <summary>ID values for the cargo.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public int ID { get; set; }
			/// <summary>Quantity of the cargo contents.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public int Count { get; set; }
			/// <summary>Physical state of the cargo material.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public CargoTypes Type { get; set; }
			/// <summary>Amount of space occupied by the cargo.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public byte Volume { get; set; }
			/// <summary>Monetary value of the cargo.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public byte Value { get; set; }
			/// <summary>The explosive properties of the cargo.</summary>
			/// <remarks>This should affect the explosiveness of the craft.</remarks>
			public VolatilityLevels Volatility { get; set; }
		}

		/// <summary>Container for Region data.</summary>
		[Serializable]
		public struct Region
		{
			string _name;

			/// <summary>Name of the region.</summary>
			/// <remarks>63 character limit.</remarks>
			public string Name
			{
				get => _name;
				set => _name = StringFunctions.GetTrimmed(value, 63);
			}

			/// <summary>ID number of the region.</summary>
			/// <remarks>Unknown if there's a real use.</remarks>
			public int ID { get; set; }
		}

		/// <summary>Container for Global Group and Global Unit data.</summary>
		[Serializable]
		public struct GlobalUnit
		{
			string _name;
			string _cargo;

			/// <summary>Unit name.</summary>
			/// <remarks>63 character limit.</remarks>
			public string Name
			{
				get => _name;
				set => _name = StringFunctions.GetTrimmed(value, 63);
			}

			/// <summary>FlightGroup index of the group leader.</summary>
			public byte Leader { get; set; }
			/// <summary>Craft index of the special craft.</summary>
			/// <remarks>Unsure if this is used or not, or how it would calculate indexes.</remarks>
			public byte SpecialCraft { get; set; }
			/// <summary>Group special cargo.</summary>
			/// <remarks>19 character limit.</remarks>
			public string SpecialCargo
			{
				get => _cargo;
				set => _cargo = StringFunctions.GetTrimmed(value, 19);
			}
			/// <summary>Random special craft index.</summary>
			public bool RandomSpecialCraft { get; set; }
		}
	}
}