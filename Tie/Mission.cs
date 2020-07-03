/*
 * Idmr.Platform.dll, X2020wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2019 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.1
 */

/* CHANGELOG
 * v3.1, 200703
 * [UPD] added backup during save
 * v3.0.1, 180919
 * [FIX] Pitch value check during write
 * v3.0, 180903
 * [UPD] updated string encodings [JB]
 * [NEW] EndOfMissionMessageColor [JB]
 * [UPD] PermaDeath (Unk9 and 10) [JB]
 * [FIX] Encoding highlight brackets in briefing questions [JB]
 * [FIX] added char(160) check during Officer read
 * [UPD] cleaned up Officer saving
 * [FIX] fs null check to prevent exception during save [JB]
 * [NEW] DeleteFG(), SwapFG() [JB]
 * v2.5, 170107
 * [UPD] Enforced string encodings during read/write[JB]
 * [FIX] Message loading length check [JB]
 * [FIX] Global Goal loading [JB]
 * v2.4, 160606
 * [FIX] Invert WP.Y at read/write
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0.1, 120814
 * [FIX] Officer questions save [higlighting] properly
 * [FIX] FlightGroup.SpecialCargoCraft load/save
 * v2.0, 120525
 * [NEW] CraftCheck(), CheckTarget()
 * [UPD] IffHostile and EndOfMissionMessages inherit Indexer<T>
 * [UPD] class inherits MissionFile
 * [DEL] NumFlightGroups and NumMessages
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	/// <summary>Framework for TIE95</summary>
	/// <remarks>This is the primary container object for a TIE95 mission file</remarks>
	public partial class Mission : MissionFile
	{
		string[] _endOfMissionMessages = new string[6];	//Mission Comp/Fail messages
		string[] _iff = Strings.IFF;
		bool[] _iffHostile = new bool[6];
		IffNameIndexer _iffNameIndexer;
		Indexer<bool> _iffHostileIndexer;
        byte[] _endOfMissionMessageColor = new byte[6];
        Indexer<string> _endOfMissionIndexer;
		
		/// <summary>Pre- and Post-mission officers</summary>
		public enum BriefingOfficers : byte {
			/// <summary>No officers present</summary>
			None,
			/// <summary>Both officers are present</summary>
			Both,
			/// <summary>Only the Flight Officer is present</summary>
			FlightOfficer,
			/// <summary>Only the Secret Order member is present</summary>
			SecretOrder
		}

		#region constructors
		/// <summary>Default constructor, creates a blank mission</summary>
		public Mission()
		{
			_initialize();
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
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
			OfficersPresent = BriefingOfficers.FlightOfficer;
			_invalidError = _invalidError.Replace("{0}", "TIE");
			_iffHostile[0] = true;
			_iffNameIndexer = new IffNameIndexer(this);
			_iffHostileIndexer = new Indexer<bool>(_iffHostile, new bool[]{true, true, false, false, false, false});
			_endOfMissionIndexer = new Indexer<string>(_endOfMissionMessages, 63);
			FlightGroups = new FlightGroupCollection();
			Messages = new MessageCollection();
			GlobalGoals = new Globals();
			Briefing = new Briefing();
			BriefingQuestions = new Questions();
		}
		#endregion constructors
		
		#region public methods
		/// <summary>Loads a mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		/// <exception cref="FileNotFoundException"><i>filePath</i> does not exist</exception>
		/// <exception cref="InvalidDataException"><i>filePath</i> is not a TIE mission file</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Loads a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="InvalidDataException"><i>stream</i> is not a valid TIE mission file</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (GetPlatform(stream) != Platform.TIE) throw new InvalidDataException(_invalidError);
            BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
			int i;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			stream.Position = 0xA;
			try { OfficersPresent = (BriefingOfficers)br.ReadByte(); }
			catch { OfficersPresent = BriefingOfficers.Both; }
			stream.Position = 0xD;
			CapturedOnEjection = br.ReadBoolean();
			stream.Position = 0x18;
            for (i = 0; i < 6; i++)
            {
                EndOfMissionMessages[i] = new string(br.ReadChars(64));
                if (EndOfMissionMessages[i] != "")
                {
                    char c = EndOfMissionMessages[i][0];
                    if (c == '1' || c == '2' || c == '3')
                    {
                        EndOfMissionMessageColor[i] = byte.Parse(c.ToString());
                        EndOfMissionMessages[i] = EndOfMissionMessages[i].Substring(1);
                    }
                }
            }
			stream.Position += 2;
			byte[] buffer = new byte[64];
			for (i=2;i<6;i++) IFFs[i] = new string(br.ReadChars(12));
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			for (i=0;i<FlightGroups.Count;i++)
			{
				#region Craft
				int j;
				FlightGroups[i].Name = new string(br.ReadChars(12));
				FlightGroups[i].Pilot = new string(br.ReadChars(12));	//not used by TIE
				FlightGroups[i].Cargo = new string(br.ReadChars(12));
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(12));
				stream.Read(buffer, 0, 0x38);
				FlightGroups[i].SpecialCargoCraft = buffer[0];
				FlightGroups[i].RandSpecCargo = Convert.ToBoolean(buffer[1]);
				FlightGroups[i].CraftType = buffer[2];
				FlightGroups[i].NumberOfCraft = buffer[3];
				// the Rand part of the check below is because if true, don't need to mess with it
				if (!FlightGroups[i].RandSpecCargo)
				{
					if (FlightGroups[i].SpecialCargoCraft >= FlightGroups[i].NumberOfCraft) FlightGroups[i].SpecialCargoCraft = 0;
					else FlightGroups[i].SpecialCargoCraft++;
				}
				FlightGroups[i].Status1 = buffer[4];
				FlightGroups[i].Missile = buffer[5];
				FlightGroups[i].Beam = buffer[6];
				FlightGroups[i].IFF = buffer[7];
				FlightGroups[i].AI = buffer[8];
				FlightGroups[i].Markings = buffer[9];
				FlightGroups[i].FollowsOrders = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Unknowns.Unknown1 = buffer[0xB];
				FlightGroups[i].Formation = buffer[0xC];
				FlightGroups[i].FormDistance = buffer[0xD];
				FlightGroups[i].GlobalGroup = buffer[0xE];
				FlightGroups[i].FormLeaderDist = buffer[0xF];
				FlightGroups[i].NumberOfWaves = (byte)(buffer[0x10]+1);
				FlightGroups[i].Unknowns.Unknown5 = buffer[0x11];
				FlightGroups[i].PlayerCraft = buffer[0x12];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x13] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x14] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x15] * 360 / 0x100);
				FlightGroups[i].PermaDeathEnabled = buffer[0x16];
				FlightGroups[i].PermaDeathID = buffer[0x17];
				FlightGroups[i].Unknowns.Unknown11 = buffer[0x18];
				#endregion
				#region Arr/Dep
				FlightGroups[i].Difficulty = buffer[0x19];
				FlightGroups[i].ArrDepTriggers[0] = new Trigger(buffer, 0x1A);
				FlightGroups[i].ArrDepTriggers[1] = new Trigger(buffer, 0x1E);
				FlightGroups[i].AT1AndOrAT2 = Convert.ToBoolean(buffer[0x22]);
				FlightGroups[i].Unknowns.Unknown12 = buffer[0x23];
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				FlightGroups[i].ArrDepTriggers[2] = new Trigger(buffer, 0x26);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x2A];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x2B];
				FlightGroups[i].AbortTrigger = buffer[0x2C];
				FlightGroups[i].Unknowns.Unknown15 = buffer[0x2D];
				FlightGroups[i].Unknowns.Unknown16 = buffer[0x2E];
				FlightGroups[i].Unknowns.Unknown17 = buffer[0x2F];
				FlightGroups[i].ArrivalCraft1 = buffer[0x30];
				FlightGroups[i].ArrivalMethod1 = Convert.ToBoolean(buffer[0x31]);
				FlightGroups[i].DepartureCraft1 = buffer[0x32];
				FlightGroups[i].DepartureMethod1 = Convert.ToBoolean(buffer[0x33]);
				FlightGroups[i].ArrivalCraft2 = buffer[0x34];
				FlightGroups[i].ArrivalMethod2 = Convert.ToBoolean(buffer[0x35]);
				FlightGroups[i].DepartureCraft2 = buffer[0x36];
				FlightGroups[i].DepartureMethod2 = Convert.ToBoolean(buffer[0x37]);
				#endregion
				stream.Read(buffer, 0, 0x36);
				for (j = 0; j < 3; j++) FlightGroups[i].Orders[j] = new FlightGroup.Order(buffer, j * 18);
				stream.Read(buffer, 0, 0xA);
				FlightGroups[i].Goals = new FlightGroup.FGGoals(buffer, 0);
				for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) FlightGroups[i].Waypoints[k][j] = (short)(br.ReadInt16() * (j == 1 ? -1 : 1));
				FlightGroups[i].Unknowns.Unknown19 = br.ReadBoolean();
				stream.Position++;
				FlightGroups[i].Unknowns.Unknown20 = br.ReadByte();
				FlightGroups[i].Unknowns.Unknown21 = br.ReadBoolean();
			}
			#endregion
			#region Messages
			if (numMessages != 0)
			{
				Messages = new MessageCollection(numMessages);
				for (i=0;i<Messages.Count;i++)
				{
					Messages[i].MessageString = new string(br.ReadChars(64)).Trim('\0');
					if (Messages[i].MessageString.IndexOf('\0') != -1) Messages[i].MessageString = Messages[i].MessageString.Substring(0, Messages[i].MessageString.IndexOf('\0'));
					Messages[i].Color = 0;
					if (Messages[i].MessageString.Length > 0)  //[JB] Added length check, otherwise empty strings would crash the load process and make the mission unrecoverable to anything but hex editing.  
					{
						char c = Messages[i].MessageString[0];
						if (c >= '1' && c <= '3')
						{
							Messages[i].Color = (byte)(c - '0');
							Messages[i].MessageString = Messages[i].MessageString.Substring(1);
						}
					}
					stream.Read(buffer, 0, 8);
					Messages[i].Triggers[0] = new Trigger(buffer, 0);
					Messages[i].Triggers[1] = new Trigger(buffer, 4);
					Messages[i].Short = new string(br.ReadChars(12));
					stream.Position += 4;
					Messages[i].Delay = br.ReadByte();
					Messages[i].Trig1AndOrTrig2 = br.ReadBoolean();
				}
			}
			else { Messages.Clear(); }
			#endregion
			#region Globals
			for (i=0;i<3;i++)
			{
				stream.Read(buffer, 0, 8);
				GlobalGoals.Goals[i].Triggers[0] = new Trigger(buffer, 0);
				GlobalGoals.Goals[i].Triggers[1] = new Trigger(buffer, 4);
				stream.Position += 0x11;
				//for some reason, there's triggers with Var set with no Type
				if (GlobalGoals.Goals[i].Triggers[0].VariableType == 0) GlobalGoals.Goals[i].Triggers[0].Variable = 0;
				if (GlobalGoals.Goals[i].Triggers[1].VariableType == 0) GlobalGoals.Goals[i].Triggers[1].Variable = 0;
				GlobalGoals.Goals[i].T1AndOrT2 = br.ReadBoolean();
				stream.Position += 2;
			}
			#endregion
			#region Briefing
			Briefing.Length = br.ReadInt16();
			Briefing.Unknown1 = br.ReadInt16();
			stream.Position += 6;	// StartLength, EventsLength, Res(0)
			for (i=0;i<12;i++)
			{
				stream.Read(buffer, 0, 0x40);
				Buffer.BlockCopy(buffer, 0, Briefing.Events, 0x40 * i, 0x40);
			}
			stream.Read(buffer, 0, 0x20);
			Buffer.BlockCopy(buffer, 0, Briefing.Events, 0x300, 0x20);
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				if (j > 0) Briefing.BriefingTag[i] = new string(br.ReadChars(j));
			}
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				if (j > 0) Briefing.BriefingString[i] = new string(br.ReadChars(j));
			}
			#endregion
			#region Questions
			for (i=0;i<10;i++)
			{
				int j, k, l = 0;
				j = br.ReadInt16();	//read the question length, we're just not going to save it
				if (j == 1)
				{
					stream.Position++;	//if it's just the return, get rid of it
					continue;
				}
				if (j == 0) continue;
				for (k=0;k<j;k++)
				{
					BriefingQuestions.PreMissQuestions[i] += br.ReadChar().ToString();
					l++;
					if (stream.ReadByte() == 10) break;
					else stream.Position--;
				}
				l++;
				for (k=l;k<j;k++)
				{
                    int b = br.ReadChar(); //[JB] Must honor stream encoding for strings, can't use ReadByte
                    switch (b)	//TIE uses char(2) and char(1) for bolding in this section
					{
						case 1:
							BriefingQuestions.PreMissAnswers[i] += "]";
							break;
						case 2:
							BriefingQuestions.PreMissAnswers[i] += "[";
							break;
						case 10:
							BriefingQuestions.PreMissAnswers[i] += "\r\n";	//because txt doesn't like \n by itself
							break;
						default:
							BriefingQuestions.PreMissAnswers[i] += Convert.ToChar(b).ToString();
							break;
					}
				}
			}
			for (i=0;i<10;i++)
			{
				int j, k, l = 2;
				j = br.ReadInt16();	//also got rid of saving here, calc'ing on the fly
				if (j == 3)
				{
					stream.Position += 3;	// stupid TFW-isms
					continue;
				}
				if (j == 0) continue;
				BriefingQuestions.PostTrigger[i] = br.ReadByte();
				BriefingQuestions.PostTrigType[i] = br.ReadByte();
				for (k=0;k<j;k++)
				{
					BriefingQuestions.PostMissQuestions[i] += br.ReadChar().ToString();
					l++;
					if (stream.ReadByte() == 10) break;
					else stream.Position--;
				}
				l++;
				for (k=l;k<j;k++)
				{
                    int b = br.ReadChar(); //[JB] Must honor stream encoding for strings, can't use ReadByte
                    switch (b)
					{
						case 0:
							k = j;
							break;
						case 1:
							BriefingQuestions.PostMissAnswers[i] += "]";
							break;
						case 2:
							BriefingQuestions.PostMissAnswers[i] += "[";
							break;
						case 10:
							BriefingQuestions.PostMissAnswers[i] += "\r\n";
							break;
						case 160:
						case 255:
							k = j;
							break;
						default:
							BriefingQuestions.PostMissAnswers[i] += Convert.ToChar(b).ToString();
							break;
					}
				}
			}
			#endregion
			MissionPath = stream.Name;
		}

		/// <summary>Saves the mission to <see cref="MissionFile.MissionPath"/></summary>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <see cref="MissionFile.MissionPath"/> are denied</exception>
		public void Save()
		{
			FileStream fs = null;
			string backup = MissionPath.Replace(".tie", "_tie.bak");
			if (File.Exists(MissionPath))
			{
				File.Copy(MissionPath, backup);
				File.Delete(MissionPath);
			}
			try
			{

				fs = File.OpenWrite(MissionPath);
                BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
				bw.Write((short)-1);
				bw.Write((short)FlightGroups.Count);
				bw.Write((short)Messages.Count);
				bw.Write((short)3);
				fs.Position = 0xA;
				fs.WriteByte((byte)OfficersPresent);
				fs.Position = 0xD;
				bw.Write(CapturedOnEjection);
				fs.Position = 0x18;
				for (int i=0;i<6;i++)
				{
					long p = fs.Position;
                    if (EndOfMissionMessageColor[i] != 0) bw.Write(Convert.ToByte(EndOfMissionMessageColor[i] + 48));
                    bw.Write(_endOfMissionMessages[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x40;
				}
				fs.Position += 2;
				for (int i=2;i<6;i++)
				{
					long p = fs.Position;
					if (_iffHostile[i]) bw.Write('1');
					bw.Write(_iff[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0xC;
				}
				#region Flightgroups
				for (int i=0;i<FlightGroups.Count;i++)
				{
					long p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0xC;
					bw.Write(FlightGroups[i].Pilot.ToCharArray());
					fs.Position = p + 0x18;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x24;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x30;
					if (FlightGroups[i].SpecialCargoCraft == 0) fs.WriteByte(FlightGroups[i].NumberOfCraft);
					else fs.WriteByte((byte)(FlightGroups[i].SpecialCargoCraft-1));
					bw.Write(FlightGroups[i].RandSpecCargo);
					fs.WriteByte(FlightGroups[i].CraftType);
					fs.WriteByte(FlightGroups[i].NumberOfCraft);
					fs.WriteByte(FlightGroups[i].Status1);
					fs.WriteByte(FlightGroups[i].Missile);
					fs.WriteByte(FlightGroups[i].Beam);
					fs.WriteByte(FlightGroups[i].IFF);
					fs.WriteByte(FlightGroups[i].AI);
					fs.WriteByte(FlightGroups[i].Markings);
					bw.Write(FlightGroups[i].FollowsOrders);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown1);
					fs.WriteByte(FlightGroups[i].Formation);
					fs.WriteByte(FlightGroups[i].FormDistance);
					fs.WriteByte(FlightGroups[i].GlobalGroup);
					fs.WriteByte(FlightGroups[i].FormLeaderDist);
					fs.WriteByte((byte)(FlightGroups[i].NumberOfWaves-1));
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown5);
					fs.WriteByte(FlightGroups[i].PlayerCraft);
					fs.WriteByte((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					fs.WriteByte((byte)((FlightGroups[i].Pitch >= 90 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					fs.WriteByte((byte)(FlightGroups[i].Roll * 0x100 / 360));
					bw.Write(FlightGroups[i].PermaDeathEnabled);
                    fs.WriteByte(FlightGroups[i].PermaDeathID);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown11);
					#endregion
					#region Arr/Dep
					fs.WriteByte(FlightGroups[i].Difficulty);
					for (j = 0; j < 4; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[0][j]);
					for (j = 0; j < 4; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[1][j]);
					bw.Write(FlightGroups[i].AT1AndOrAT2);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown12);
					fs.WriteByte(FlightGroups[i].ArrivalDelayMinutes);
					fs.WriteByte(FlightGroups[i].ArrivalDelaySeconds);
					for (j = 0; j < 4; j++) fs.WriteByte(FlightGroups[i].ArrDepTriggers[2][j]);
					fs.WriteByte(FlightGroups[i].DepartureTimerMinutes);
					fs.WriteByte(FlightGroups[i].DepartureTimerSeconds);
					fs.WriteByte(FlightGroups[i].AbortTrigger);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown15);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown16);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown17);
					fs.WriteByte(FlightGroups[i].ArrivalCraft1);
					bw.Write(FlightGroups[i].ArrivalMethod1);
					fs.WriteByte(FlightGroups[i].DepartureCraft1);
					bw.Write(FlightGroups[i].DepartureMethod1);
					fs.WriteByte(FlightGroups[i].ArrivalCraft2);
					bw.Write(FlightGroups[i].ArrivalMethod2);
					fs.WriteByte(FlightGroups[i].DepartureCraft2);
					bw.Write(FlightGroups[i].DepartureMethod2);
					#endregion
					for (j = 0; j < 3; j++)
						for (int k = 0; k < 18; k++) fs.WriteByte(FlightGroups[i].Orders[j][k]);
					for (j = 0; j < 9; j++) fs.WriteByte(FlightGroups[i].Goals[j]);
					fs.Position++;
					for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) bw.Write((short)(FlightGroups[i].Waypoints[k][j] * (j == 1 ? -1 : 1)));
					bw.Write(FlightGroups[i].Unknowns.Unknown19);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown20);
					bw.Write(FlightGroups[i].Unknowns.Unknown21);
					fs.Position = p + 0x124;
				}
				#endregion
				#region Messages
				for (int i=0;i<Messages.Count;i++)
				{
					long p = fs.Position;
					if (Messages[i].Color != 0) bw.Write((byte)(Messages[i].Color + 0x30));
					bw.Write(Messages[i].MessageString.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x3F;
					fs.WriteByte(0);
					for (int j = 0; j < 4; j++) fs.WriteByte(Messages[i].Triggers[0][j]);
					for (int j = 0; j < 4; j++) fs.WriteByte(Messages[i].Triggers[1][j]);
					bw.Write(Messages[i].Short.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x57;
					fs.WriteByte(0);
					fs.WriteByte(Messages[i].Delay);
					bw.Write(Messages[i].Trig1AndOrTrig2);
				}
				#endregion
				#region Globals
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 4; j++) fs.WriteByte(GlobalGoals.Goals[i].Triggers[0][j]);
					for (int j = 0; j < 4; j++) fs.WriteByte(GlobalGoals.Goals[i].Triggers[1][j]);
					fs.Position += 0x11;
					bw.Write(GlobalGoals.Goals[i].T1AndOrT2);
					fs.Position += 2;
				}
				#endregion
				#region Briefing
				bw.Write(Briefing.Length);
				bw.Write(Briefing.Unknown1);
				bw.Write(Briefing.StartLength);
				bw.Write(Briefing.EventsLength);
				fs.Position += 2;
				byte[] briefBuffer = new byte[Briefing.Events.Length * 2];
				Buffer.BlockCopy(Briefing.Events, 0, briefBuffer, 0, briefBuffer.Length);
				bw.Write(briefBuffer);
				for (int i=0;i<32;i++)
				{
					string str_t = Briefing.BriefingTag[i].Replace("\r", "");
					int j = Briefing.BriefingTag[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				for (int i=0;i<32;i++)
				{
					string str_t = Briefing.BriefingString[i].Replace("\r", "");
					int j = Briefing.BriefingString[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				#endregion
				#region Questions
				for (int i=0;i<10;i++)
				{
					int j;
					string str_q = BriefingQuestions.PreMissQuestions[i];
					string str_a = BriefingQuestions.PreMissAnswers[i];
					str_a = str_a.Replace("\r", "");
					str_a = str_a.Replace(']', Convert.ToChar(1));
					str_a = str_a.Replace('[', Convert.ToChar(2));
					j = str_q.Length + str_a.Length;
					if (j == 0)
					{
						bw.Write((short)0);
						continue;	//if it doesn't exist, don't even bother with the rest of this
					}
					j++;	//takes into account the q/a spacer
					bw.Write((short)j);
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
				}
				for (int i=0;i<10;i++)
				{
					int j;
					string str_q = BriefingQuestions.PostMissQuestions[i];
					string str_a = BriefingQuestions.PostMissAnswers[i];
                    str_a = str_a.Replace("\r", "");
                    str_a = str_a.Replace(']', Convert.ToChar(1));  //[JB] Debrief questions use the same highlight scheme.  Added character conversions.
					str_a = str_a.Replace('[', Convert.ToChar(2));
                    j = str_q.Length + str_a.Length;
					if (j == 0)
					{
						bw.Write((short)0);
						continue;
					}
					j += 3;
					long p = fs.Position;
					bw.Write((short)j);
					fs.WriteByte(BriefingQuestions.PostTrigger[i]);
					fs.WriteByte(BriefingQuestions.PostTrigType[i]);
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
				}
				#endregion
				bw.Write((short)0x2106); fs.WriteByte(0xFF);
				fs.SetLength(fs.Position);
				fs.Close();
			}
			catch
			{
                if (fs != null) fs.Close(); //[JB] Fixed to prevent object instance error.
				if (File.Exists(backup))
				{
					File.Delete(MissionPath);
					File.Copy(backup, MissionPath);
					File.Delete(backup);
				}
				throw;
			}
			File.Delete(backup);
		}

		/// <summary>Saves the mission to a new <see cref="MissionFile.MissionPath"/></summary>
		/// <param name="filePath">Full path to the new <see cref="MissionFile.MissionPath"/></param>
		/// <exception cref="System.UnauthorizedAccessException">Write permissions for <i>filePath</i> are denied</exception>
		public void Save(string filePath)
		{
			MissionPath = filePath;
			Save();
		}
		
		/// <summary>Checks a CraftType for valid values and adjusts if necessary</summary>
		/// <param name="craftType">The craft index to check</param>
		/// <returns>Resultant CraftType index, or <b>255</b> if invalid</returns>
		public static byte CraftCheck(byte craftType)
		{
			if (craftType > 91) return 255;
			else if (craftType == 77) return 31;	// G/PLT
			else if (craftType == 89) return 10;	// SHPYD
			else if (craftType == 90) return 11;	// REPYD
			else if (craftType == 91) return 39;	// M/SC
			else return craftType;
		}
		
		/// <summary>Checks Trigger.Type/Variable or Order.TargetType/Target pairs for values compatible with TIE</summary>
		/// <remarks>First checks for invalid Types, then runs through allowed values for each Type. Does not verify FlightGroup, CraftWhen, GlobalGroup or Misc</remarks>
		/// <param name="type">Trigger.Type or Order.TargetType</param>
		/// <param name="variable">Trigger.Variable or Order.Target, may be updated</param>
		/// <param name="errorMessage">Error description if found, otherwise <b>""</b></param>
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

		/// <summary>Deletes a Flight Group, performing all necessary cleanup to avoid broken indexes.</summary>
		/// <param name="fgIndex">The FG index to delete</param>
		/// <returns>The index of the next available FlightGroup if successfull, otherwise <b>-1</b></returns>
		/// <remarks>Propagates throughout all members which may reference Flight Group indexes.</remarks>
		public int DeleteFG(int fgIndex)
        {
            if (fgIndex < 0 || fgIndex >= FlightGroups.Count)
                return 0;  //If for some reason this is out of range, don't do anything and return selection to first item.

            foreach (Globals.Goal goal in GlobalGoals.Goals)
                foreach (Mission.Trigger trig in goal.Triggers)
                    trig.TransformFGReferences(fgIndex, -1, false);

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.TransformFGReferences(fgIndex, -1, true);

            Briefing.TransformFGReferences(fgIndex, -1);

            foreach (FlightGroup fg in FlightGroups)
                fg.TransformFGReferences(fgIndex, -1);
            
            return FlightGroups.RemoveAt(fgIndex);  //This handles all the cleanup within the FlightGroupCollection itself.
        }

        /// <summary>Swaps two FlightGroups.</summary>
		/// <param name="srcIndex">The original index</param>
		/// <param name="dstIndex">The new index</param>
        /// <remarks>Automatically performs bounds checking and adjusts all references in the mission to prevent breaking any indexes for triggers, orders, etc.</remarks>
        /// <returns>Returns <b>true</b> if an adjustment was performed, <b>false</b> if index validation failed.</returns>
        public bool SwapFG(int srcIndex, int dstIndex)
        {
            if((srcIndex < 0 || srcIndex >= FlightGroups.Count) || (dstIndex < 0 || dstIndex >= FlightGroups.Count) || (srcIndex == dstIndex)) return false;

            foreach (Globals.Goal goal in GlobalGoals.Goals)
                foreach (Mission.Trigger trig in goal.Triggers)
                    trig.SwapFGReferences(srcIndex, dstIndex);

            foreach (Message msg in Messages)
                foreach (Mission.Trigger trig in msg.Triggers)
                    trig.SwapFGReferences(srcIndex, dstIndex);

            Briefing.SwapFGReferences(srcIndex, dstIndex);

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
		/// <summary>Gets the array accessor for the IFF names</summary>
		public IffNameIndexer IFFs { get { return _iffNameIndexer; } }
		/// <summary>Gets the array accessor for the IFF behaviour</summary>
		public Indexer<bool> IffHostile { get { return _iffHostileIndexer; } }

        /// <summary>Gets or sets the color of the specified EoM Message</summary>
        public byte[] EndOfMissionMessageColor { get { return _endOfMissionMessageColor; } }
        /// <summary>Gets the array accessor for the EoM Messages</summary>
		public Indexer<string> EndOfMissionMessages { get { return _endOfMissionIndexer; } }

		/// <summary>Maximum number of craft that can exist at one time in-game</summary>
		/// <remarks>Value is <b>28</b></remarks>
		public const int CraftLimit = 28;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file</summary>
		/// <remarks>Value is <b>48</b></remarks>
		public const int FlightGroupLimit = 48;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file</summary>
		/// <remarks>Value is <b>16</b></remarks>
		public const int MessageLimit = 16;
		
		/// <summary>Gets or sets the officers present before and after the mission</summary>
		/// <remarks>Defaults to <b>FlightOfficer</b></remarks>
		public BriefingOfficers OfficersPresent { get; set; }
		/// <summary>Gets or sets if the pilot is captured upon ejection or destruction</summary>
		/// <remarks><b>true</b> results in capture, <b>false</b> results in rescue (default)</remarks>
		public bool CapturedOnEjection { get; set; }
		
		/// <summary>Gets or sets the FlightGroups for the mission</summary>
		/// <remarks>Defaults to one FlightGroup</remarks>
		public FlightGroupCollection FlightGroups { get; set; }
		/// <summary>Gets or sets the In-Flight Messages for the mission</summary>
		/// <remarks>Defaults to zero messages</remarks>
		public MessageCollection Messages { get; set; }
		/// <summary>Gets or sets the Global Goals for the mission</summary>
		public Globals GlobalGoals { get; set; }
		/// <summary>Gets or sets the Briefing for the mission</summary>
		public Briefing Briefing { get; set; }
		/// <summary>Gets or sets the questions for the Briefing Officers</summary>
		public Questions BriefingQuestions { get; set; }
		#endregion public properties
	}
}
