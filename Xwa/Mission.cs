using System;
using System.IO;

namespace Idmr.Platform.Xwa
{
	/// <summary>Framework for XWA</summary>
	/// <remarks>This is the primary container object for XWA mission files</remarks>
	public class Mission
	{
		private string _missionPath = "\\NewMission.tie";
		private bool _unknown1 = true;	// 0x0008
		private bool _unknown2 = true;	// 0x000B
		private string[] _iff = new string[6];
		private string[] _region = new string[4];
		private GlobCarg[] _globalCargo = new GlobCarg[16];
		private string[] _globalGroup = new string[16];
		private byte _hangar = 6;
		private byte _timeLimitMin = 0;
		private bool _endWhenComplete = false;
		private byte _officer = 0;
		private byte _logo = 4;
		private byte _unknown3 = 62;	// 0x23B3
		private byte _unknown4 = 0;	// 0x23B4
		private byte _unknown5 = 0;	// 0x23B5
		private string _missionDescription = "#";	// 4096 CHAR
		private string _missionFailed = "#";
		private string _missionSuccessful = "";
		private FlightGroupCollection _flightGroups = new FlightGroupCollection();
		private MessageCollection _messages = new MessageCollection();
		private BriefingCollection _briefings = new BriefingCollection();
		private GlobalsCollection _globals = new GlobalsCollection();
		private TeamCollection _teams = new TeamCollection();
		private string _notes = "";

		/// <summary>Parts of a Trigger array</summary>
		/// <remarks>0 = Trigger<br>1 = TrigType<br>2 = Variable<br>3 = Amount<br>4 = Parameter1<br>5 = Parameter2</remarks>
		public enum TriggerIndexes : byte { Trigger, TrigType, Variable, Amount, Parameter1, Parameter2 };

		/// <summary>Default constructor, create a blank mission</summary>
		public Mission()
		{
			for (int i=0;i<16;i++) { _globalCargo[i].Cargo = ""; _globalCargo[i].Unknown1 = true;  _globalGroup[i] = ""; }
			for (int i=0;i<4;i++) { _region[i] = "Region " + (i+1).ToString(); _iff[i+2] = ""; }
			_iff[0] = "Rebel";
			_iff[1] = "Imperial";
		}

		/// <summary>Create a new mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		public Mission(string filePath) { LoadFromFile(filePath); }

		/// <summary>Create a new mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		public Mission(FileStream stream) { LoadFromStream(stream); }

		/// <summary>Load a mission from a file</summary>
		/// <remarks>Calls LoadFromStream()</remarks>
		/// <param name="filePath">Full path to the file</param>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		/// <exception cref="System.IO.InvalidDataException">Throws if file is not a XWA mission file</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			if (MissionFile.GetPlatform(filePath) != MissionFile.Platform.XWA) throw new InvalidDataException("File is not a valid XWA mission file");
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="InvalidDataException">Thrown when stream is not a valid XWA mission file</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (MissionFile.GetPlatform(stream) != MissionFile.Platform.XWA) throw new InvalidDataException("File is not a valid XWA mission file");
			BinaryReader br = new BinaryReader(stream);
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
			for (i=2;i<6;i++) SetIff(i, new string(br.ReadChars(0x14)).Trim('\0'));
			for (i=0;i<4;i++) SetRegion(i, new string(br.ReadChars(0x84)).Trim('\0'));
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
			for (i=0;i<16;i++) SetGlobalGroupName(i, new string(br.ReadChars(0x57)).Trim('\0'));
			stream.Position = 0x23AC;
			Hangar = br.ReadByte();
			stream.Position++;
			TimeLimitMin = br.ReadByte();
			EndWhenComplete = br.ReadBoolean();
			Officer = br.ReadByte();
			Logo = (byte)(br.ReadByte()-4);
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
				FlightGroups[i].EnableDesignation1 = !Convert.ToBoolean(buffer[0]);		// 0=yes, 255=true
				FlightGroups[i].EnableDesignation2 = !Convert.ToBoolean(buffer[1]);
				FlightGroups[i].Designation1 = buffer[2];
				FlightGroups[i].Designation2 = buffer[3];
				FlightGroups[i].Unknown1 = buffer[4];
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
				// normally I'd check numCraft, but because objects are mixed all over the place, not going to bother.
				// that's more of a mission design problem anyway, I don't *have* to proofread their crap :P
				FlightGroups[i].NumberOfCraft = buffer[3];
				if (FlightGroups[i].SpecialCargoCraft >= FlightGroups[i].NumberOfCraft) FlightGroups[i].SpecialCargoCraft = 0;
				else FlightGroups[i].SpecialCargoCraft++;
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
				FlightGroups[i].Unknown3 = buffer[0x12];
				FlightGroups[i].PlayerNumber = buffer[0x14];
				FlightGroups[i].ArriveOnlyIfHuman = Convert.ToBoolean(buffer[0x15]);
				FlightGroups[i].PlayerCraft = buffer[0x16];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x17] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x18] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x19] * 360 / 0x100);
				FlightGroups[i].Unknown4 = buffer[0x1B];
				#endregion
				#region Arr/Dep
				stream.Read(buffer, 0, 0x3C);
				FlightGroups[i].Difficulty = buffer[0];
				FlightGroups[i].Unknown5 = buffer[1];
				for (j=0;j<6;j++)
				{
					FlightGroups[i].ArrDepTrigger[0, j] = buffer[2+j];	// Arr1...
					FlightGroups[i].ArrDepTrigger[1, j] = buffer[8+j];
					FlightGroups[i].ArrDepTrigger[2, j] = buffer[0x12+j];
					FlightGroups[i].ArrDepTrigger[3, j] = buffer[0x18+j];
					FlightGroups[i].ArrDepTrigger[4, j] = buffer[0x26+j];	// Dep1...
					FlightGroups[i].ArrDepTrigger[5, j] = buffer[0x2C+j];
				}
				FlightGroups[i].ArrDepAndOr[0] = Convert.ToBoolean(buffer[0x10]);
				FlightGroups[i].Unknown6 = Convert.ToBoolean(buffer[0x11]);
				FlightGroups[i].ArrDepAndOr[1] = Convert.ToBoolean(buffer[0x20]);
				FlightGroups[i].ArrDepAndOr[2] = Convert.ToBoolean(buffer[0x22]);
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				FlightGroups[i].ArrDepAndOr[3] = Convert.ToBoolean(buffer[0x34]);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x36];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x37];
				FlightGroups[i].AbortTrigger = buffer[0x38];
				FlightGroups[i].Unknown7 = buffer[0x39];
				FlightGroups[i].Unknown8 = buffer[0x3A];
				stream.Read(buffer, 0, 8);
				FlightGroups[i].ArrivalCraft1 = buffer[0];
				FlightGroups[i].ArrivalMethod1 = Convert.ToBoolean(buffer[1]);
				FlightGroups[i].ArrivalCraft2 = buffer[2];
				FlightGroups[i].ArrivalMethod2 = Convert.ToBoolean(buffer[3]);
				FlightGroups[i].DepartureCraft1 = buffer[4];
				FlightGroups[i].DepartureMethod1 = Convert.ToBoolean(buffer[5]);
				FlightGroups[i].DepartureCraft2 = buffer[6];
				FlightGroups[i].DepartureMethod2 = Convert.ToBoolean(buffer[7]);
				#endregion
				#region Orders
				for (j=0;j<16;j++)
				{
					stream.Read(buffer, 0, 0x14);
					for (int h=0;h<0x13;h++) FlightGroups[i].Orders[j, h] = buffer[h];	// Unk9 @ 0x5
					for (int h=0;h<8;h++)
						for (int k=0;k<4;k++) FlightGroups[i].Waypoints[j*8+h+4, k] = br.ReadInt16();
					stream.Position += 0x1E;
					FlightGroups[i].Unknown10[j] = br.ReadByte();
					FlightGroups[i].Unknown11[j] = br.ReadBoolean();
					FlightGroups[i].Unknown12[j] = br.ReadBoolean();
					stream.Position += 6;
					FlightGroups[i].Unknown13[j] = br.ReadBoolean();
					stream.Position += 5;
					FlightGroups[i].Unknown14[j] = br.ReadBoolean();
					stream.Position += 0x12;
				}
				for (j=0;j<16;j++)
				{
					stream.Read(buffer, 0, 0x10);
					for (int h=0;h<6;h++)
					{
						FlightGroups[i].SkipToOrder[j*2, h] = buffer[h];
						FlightGroups[i].SkipToOrder[j*2+1, h] = buffer[h+6];
					}
					FlightGroups[i].SkipAndOr[j] = Convert.ToBoolean(buffer[0xE]);
				}
				#endregion
				#region Goals
				for (j=0;j<8;j++)
				{
					stream.Read(buffer, 0, 0x10);
					for (int h=0;h<6;h++) FlightGroups[i].Goals[j, h] = buffer[h];
					FlightGroups[i].Goals[j, 6] = buffer[0xE];
					FlightGroups[i].Goals[j, 7] = buffer[0xF];
					stream.Position += 0x3F;
					FlightGroups[i].Goals[j, 8] = br.ReadByte();	// Unk15
				}
				#endregion
				for (j=0;j<4;j++) for (int k=0;k<4;k++) FlightGroups[i].Waypoints[j, k] = br.ReadInt16();
				for (j=0;j<4;j++) FlightGroups[i].Waypoints[j, 4] = br.ReadByte();
				#region Options/other
				stream.Read(buffer, 0, 0x1E);
				FlightGroups[i].Unknown16 = buffer[0];
				FlightGroups[i].Unknown17 = buffer[1];
				FlightGroups[i].Unknown18 = buffer[2];
				FlightGroups[i].Unknown19 = buffer[3];
				FlightGroups[i].Unknown20 = buffer[4];
				FlightGroups[i].Unknown21 = buffer[5];
				FlightGroups[i].Unknown22 = Convert.ToBoolean(buffer[6]);
				FlightGroups[i].Unknown23 = buffer[8];
				FlightGroups[i].Unknown24 = buffer[9];
				FlightGroups[i].Unknown25 = buffer[0xA];
				FlightGroups[i].Unknown26 = buffer[0xB];
				FlightGroups[i].Unknown27 = buffer[0xC];
				FlightGroups[i].Unknown28 = buffer[0xD];
				FlightGroups[i].Unknown29 = Convert.ToBoolean(buffer[0xE]);
				FlightGroups[i].Unknown30 = Convert.ToBoolean(buffer[0x12]);
				FlightGroups[i].Unknown31 = Convert.ToBoolean(buffer[0x13]);
				FlightGroups[i].GlobalNumbering = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].Unknown32 = buffer[0x17];
				FlightGroups[i].Unknown33 = buffer[0x18];
				FlightGroups[i].Countermeasures = buffer[0x19];
				FlightGroups[i].ExplosionTime = buffer[0x1A];
				FlightGroups[i].Status2 = buffer[0x1B];
				FlightGroups[i].GlobalUnit = buffer[0x1C];
				for (j=0;j<8;j++)
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 8) { FlightGroups[i].OptLoadout[x] = true; FlightGroups[i].OptLoadout[0] = false; }
				}
				for (j=8;j<12;j++)
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 4) { FlightGroups[i].OptLoadout[x] = true; FlightGroups[i].OptLoadout[8] = false; }
				}
				stream.Position += 2;
				for (j=12;j<15;j++)
				{
					byte x = br.ReadByte();
					if (x != 0 && x < 3) { FlightGroups[i].OptLoadout[x] = true; FlightGroups[i].OptLoadout[12] = false; }
				}
				stream.Position++;
				FlightGroups[i].OptCraftCategory = br.ReadByte();
				stream.Read(buffer, 0, 0x1E);
				for (j=0;j<3;j++) for (int k=0;k<10;k++) FlightGroups[i].OptCraft[k, j] = buffer[j*3+k];
				FlightGroups[i].PilotID = new string(br.ReadChars(0x10)).Trim('\0');
				stream.Position += 5;
				FlightGroups[i].Backdrop = br.ReadByte();
				stream.Position += 0x16;
				stream.Read(buffer, 0, 0x15);
				FlightGroups[i].Unknown34 = Convert.ToBoolean(buffer[0]);
				FlightGroups[i].Unknown35 = Convert.ToBoolean(buffer[2]);
				FlightGroups[i].Unknown36 = Convert.ToBoolean(buffer[4]);
				FlightGroups[i].Unknown37 = Convert.ToBoolean(buffer[6]);
				FlightGroups[i].Unknown38 = Convert.ToBoolean(buffer[8]);
				FlightGroups[i].Unknown39 = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Unknown40 = Convert.ToBoolean(buffer[0xC]);
				FlightGroups[i].Unknown41 = Convert.ToBoolean(buffer[0xE]);
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
						Messages[i].Triggers[0, j] = buffer[j];	// T1...
						Messages[i].Triggers[1, j] = buffer[6+j];
						Messages[i].Triggers[2, j] = buffer[0x10+j];
						Messages[i].Triggers[3, j] = buffer[0x16+j];
					}
					Messages[i].Unknown1 = buffer[0xC];
					Messages[i].TrigAndOr[0] = Convert.ToBoolean(buffer[0xE]);
					Messages[i].TrigAndOr[1] = Convert.ToBoolean(buffer[0x1E]);
					Messages[i].VoiceID = new string(br.ReadChars(8)).Trim('\0');
					Messages[i].OriginatingFG = br.ReadByte();
					stream.Position += 7;
					stream.Read(buffer, 0, 0x16);
					Messages[i].DelaySeconds = buffer[0];
					Messages[i].DelayMinutes = buffer[1];
					Messages[i].Color = buffer[2];
					Messages[i].TrigAndOr[2] = Convert.ToBoolean(buffer[3]);
					for (j=0;j<6;j++)
					{
						Messages[i].Triggers[4, j] = buffer[4+j];	// CancelT1...
						Messages[i].Triggers[5, j] = buffer[0xA+j];
					}
					Messages[i].TrigAndOr[3] = Convert.ToBoolean(buffer[0x12]);
					Messages[i].Unknown2 = Convert.ToBoolean(buffer[0x14]);
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
						Globals[i].Triggers[k*4, j] = buffer[j];
						Globals[i].Triggers[k*4+1, j] = buffer[j+6];
					}
					Globals[i].AndOr[k*3] = br.ReadBoolean();
					Globals[i].Unknown1[k] = br.ReadBoolean();
					stream.Read(buffer, 0, 0xE);
					for (j=0;j<6;j++)
					{
						Globals[i].Triggers[k*4+2, j] = buffer[j];
						Globals[i].Triggers[k*4+3, j] = buffer[j+6];
					}
					Globals[i].AndOr[k*3+1] = br.ReadBoolean();
					stream.Position += 8;
					Globals[i].Unknown2[k] = br.ReadBoolean();
					stream.Position += 9;
					Globals[i].AndOr[k*3+2] = br.ReadBoolean();
					stream.Read(buffer, 0, 7);
					Globals[i].Unknown3[k] = buffer[0];
					Globals[i].SetGoalPoints(k, (short)((sbyte)buffer[1] * 25));
					Globals[i].Unknown4[k] = buffer[2];
					Globals[i].Unknown5[k] = buffer[3];
					Globals[i].Unknown6[k] = buffer[4];
					Globals[i].ActiveSequence[k] = buffer[6];
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
				for (j=0;j<6;j++) Teams[i].SetEndOfMissionMessage(j, new string(br.ReadChars(0x40)).Trim('\0'));
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
				for (j=0;j<0x44;j++) stream.Read(Briefings[i].Events, j*0x100, 0x100);
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
			_notes = new string(br.ReadChars(0x18E0)).Trim('\0');
			stream.Position += 0x319C;	// TODO: unknown message notes (L 100, qty 127)
			for (i=0;i<64;i++)
			{
				if (i < Messages.Count) Messages[i].Note = new string(br.ReadChars(0x64)).Trim('\0');
				else stream.Position += 0x64;
			}
			stream.Position += 0x1B58;	// TODO: unknown message notes (L 100, qty 70)
			stream.Position += 0x12C;	// TODO: EoM notes? (L 100, qty 3)
			#endregion
			#region FG goal strings
			for (i=0;i<NumFlightGroups;i++)
				for (j=0;j<8;j++)	// per goal
					for (int k=0;k<3;k++)	// per string
						if (br.ReadByte() != 0)
						{
							stream.Position--;
							FlightGroups[i].GoalStrings[j, k] = new string(br.ReadChars(0x40)).Trim('\0');
						}
			#endregion
			#region Globals strings
			for (i=0;i<10;i++)	// per team
				for (j=0;j<12;j++)	// per goal
					for (int k=0;k<3;k++)	// per string
						if (br.ReadByte() != 0)
						{
							if (j>=8 && k==0) { stream.Position += 0x3F; continue; }	// skip Sec Inc
							if (j>=4 && k==2) { stream.Position += 0x3F; continue; }	// skip Prev & Sec Fail
							stream.Position--;
							Globals[i].SetGoalString(j, k, new string(br.ReadChars(0x40)).Trim('\0'));
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
						FlightGroups[i].OrderStrings[j] = new string(br.ReadChars(0x40)).Trim('\0');
					}
			#endregion
			_missionSuccessful = new string(br.ReadChars(0x1000)).Trim('\0');
			_missionFailed = new string(br.ReadChars(0x1000)).Trim('\0');
			_missionDescription = new string(br.ReadChars(0x1000)).Trim('\0');
			_missionPath = stream.Name;
		}

		/// <summary>Save the mission with the default path</summary>
		public void Save()
		{
			FileStream fs = null;
			try
			{
				File.Delete(_missionPath);
				fs = File.OpenWrite(_missionPath);
				BinaryWriter bw = new BinaryWriter(fs);
				int i;
				long p;
				#region Platform
				bw.Write((short)0x12);
				bw.Write(NumFlightGroups);
				bw.Write(NumMessages);
				fs.Position = 8;
				bw.Write(Unknown1);
				fs.Position = 0xB;
				bw.Write(Unknown2);
				fs.Position = 0x14;
				for (i=2;i<6;i++)
				{
					p = fs.Position;
					bw.Write(GetIff(i).ToCharArray());
					fs.Position = p + 0x14;
				}
				for (i=0;i<4;i++)
				{
					p = fs.Position;
					bw.Write(GetRegion(i).ToCharArray());
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
					bw.Write(GetGlobalGroupName(i).ToCharArray());
					fs.Position = p + 0x57;
				}
				fs.Position = 0x23AC;
				fs.WriteByte(Hangar);
				fs.Position++;
				fs.WriteByte(TimeLimitMin);
				bw.Write(EndWhenComplete);
				fs.WriteByte(Officer);
				fs.WriteByte((byte)(Logo+4));
				fs.Position++;
				fs.WriteByte(Unknown3);
				fs.WriteByte(Unknown4);
				fs.WriteByte(Unknown5);
				fs.Position = 0x23F0;
				#endregion
				#region FlightGroups
				for(i=0;i<NumFlightGroups;i++)
				{
					p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.Position = p + 0x14;
					bw.Write((byte)(FlightGroups[i].EnableDesignation1 ? 0 : 255));
					bw.Write((byte)(FlightGroups[i].EnableDesignation2 ? 0 : 255));
					fs.WriteByte(FlightGroups[i].Designation1);
					fs.WriteByte(FlightGroups[i].Designation2);
					fs.WriteByte(FlightGroups[i].Unknown1);
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
					fs.WriteByte(FlightGroups[i].Unknown3);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].PlayerNumber);
					bw.Write(FlightGroups[i].ArriveOnlyIfHuman);
					fs.WriteByte(FlightGroups[i].PlayerCraft);
					fs.WriteByte((byte)Math.Round(((double)FlightGroups[i].Yaw * 0x100 / 360)));
					fs.WriteByte((byte)Math.Round(((double)(FlightGroups[i].Pitch >= 64 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360)));
					fs.WriteByte((byte)Math.Round(((double)FlightGroups[i].Roll * 0x100 / 360)));
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknown4);
					fs.Position++;
					#endregion
					#region Arr/Dep
					fs.WriteByte(FlightGroups[i].Difficulty);
					fs.WriteByte(FlightGroups[i].Unknown5);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 0), 0, 6);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 1), 0, 6);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[0]);
					bw.Write(FlightGroups[i].Unknown6);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 2), 0, 6);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 3), 0, 6);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[1]);
					fs.Position++;
					bw.Write(FlightGroups[i].ArrDepAndOr[2]);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].ArrivalDelayMinutes);
					fs.WriteByte(FlightGroups[i].ArrivalDelaySeconds);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 4), 0, 6);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 5), 0, 6);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAndOr[3]);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].DepartureTimerMinutes);
					fs.WriteByte(FlightGroups[i].DepartureTimerSeconds);
					fs.WriteByte(FlightGroups[i].AbortTrigger);
					fs.WriteByte(FlightGroups[i].Unknown7);
					fs.WriteByte(FlightGroups[i].Unknown8);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].ArrivalCraft1);
					bw.Write(FlightGroups[i].ArrivalMethod1);
					fs.WriteByte(FlightGroups[i].ArrivalCraft2);
					bw.Write(FlightGroups[i].ArrivalMethod2);
					fs.WriteByte(FlightGroups[i].DepartureCraft1);
					bw.Write(FlightGroups[i].DepartureMethod1);
					fs.WriteByte(FlightGroups[i].DepartureCraft2);
					bw.Write(FlightGroups[i].DepartureMethod2);
					#endregion
					#region Orders
					for (j=0;j<16;j++)
					{
						for (int h=0;h<0x13;h++) fs.WriteByte(FlightGroups[i].Orders[j, h]);
						fs.Position++;
						for (int h=0;h<8;h++)
							for (int k=0;k<4;k++) bw.Write(FlightGroups[i].Waypoints[j*8+h+4, k]);
						fs.Position += 0x1E;
						fs.WriteByte(FlightGroups[i].Unknown10[j]);
						bw.Write(FlightGroups[i].Unknown11[j]);
						bw.Write(FlightGroups[i].Unknown12[j]);
						fs.Position += 6;
						bw.Write(FlightGroups[i].Unknown13[j]);
						fs.Position += 5;
						bw.Write(FlightGroups[i].Unknown14[j]);
						fs.Position += 0x12;
					}
					for (j=0;j<16;j++)
					{
						fs.Write(MissionFile.Trigger(FlightGroups[i].SkipToOrder, j*2), 0, 6);
						fs.Write(MissionFile.Trigger(FlightGroups[i].SkipToOrder, j*2+1), 0, 6);
						fs.Position += 2;
						bw.Write(FlightGroups[i].SkipAndOr[j]);
						fs.Position++;
					}
					#endregion
					#region Goals
					for (j=0;j<8;j++)
					{
						for (int k=0;k<6;k++) fs.WriteByte(FlightGroups[i].Goals[j, k]);
						fs.Position += 8;
						fs.WriteByte(FlightGroups[i].Goals[j, 6]);
						fs.WriteByte(FlightGroups[i].Goals[j, 7]);
						fs.Position += 0x3F;
						fs.WriteByte(FlightGroups[i].Goals[j, 8]);
					}
					#endregion
					// SP1 0,0,0 check for backdrops
					if (FlightGroups[i].CraftType == 0xB7 && FlightGroups[i].Waypoints[0, 0] == 0 && FlightGroups[i].Waypoints[0, 1] == 0 && FlightGroups[i].Waypoints[0, 2] == 0)
						FlightGroups[i].Waypoints[0, 1] = 10;
					for (j=0;j<4;j++) for (int k=0;k<4;k++) bw.Write(FlightGroups[i].Waypoints[j, k]);
					for (j=0;j<4;j++) fs.WriteByte((byte)FlightGroups[i].Waypoints[j, 4]);
					#region Options/other
					fs.WriteByte(FlightGroups[i].Unknown16);
					fs.WriteByte(FlightGroups[i].Unknown17);
					fs.WriteByte(FlightGroups[i].Unknown18);
					fs.WriteByte(FlightGroups[i].Unknown19);
					fs.WriteByte(FlightGroups[i].Unknown20);
					fs.WriteByte(FlightGroups[i].Unknown21);
					bw.Write(FlightGroups[i].Unknown22);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknown23);
					fs.WriteByte(FlightGroups[i].Unknown24);
					fs.WriteByte(FlightGroups[i].Unknown25);
					fs.WriteByte(FlightGroups[i].Unknown26);
					fs.WriteByte(FlightGroups[i].Unknown27);
					fs.WriteByte(FlightGroups[i].Unknown28);
					bw.Write(FlightGroups[i].Unknown29);
					fs.Position += 3;
					bw.Write(FlightGroups[i].Unknown30);
					bw.Write(FlightGroups[i].Unknown31);
					fs.Position += 2;
					bw.Write(FlightGroups[i].GlobalNumbering);
					fs.WriteByte(FlightGroups[i].Unknown32);
					fs.WriteByte(FlightGroups[i].Unknown33);
					fs.WriteByte(FlightGroups[i].Countermeasures);
					fs.WriteByte(FlightGroups[i].ExplosionTime);
					fs.WriteByte(FlightGroups[i].Status2);
					fs.WriteByte(FlightGroups[i].GlobalUnit);
					fs.Position++;
					for (j=1;j<8;j++) if (FlightGroups[i].OptLoadout[j]) bw.Write((byte)j); else fs.Position++;	// warheads
					fs.Position++;	// only writing 7
					for (j=1;j<4;j++) if (FlightGroups[i].OptLoadout[j+8]) bw.Write((byte)j); else fs.Position++;	// CMs
					fs.Position += 3;	// only writing 3
					for (j=1;j<3;j++) if (FlightGroups[i].OptLoadout[j+12]) bw.Write((byte)j); else fs.Position++;	// beam
					fs.Position += 2;	// only writing 2
					fs.WriteByte(FlightGroups[i].OptCraftCategory);
					for (j=0;j<3;j++) for (int k=0;k<10;k++) fs.WriteByte(FlightGroups[i].OptCraft[k, j]);
					bw.Write(FlightGroups[i].PilotID.ToCharArray());
					fs.Position = p + 0xE12;
					fs.WriteByte(FlightGroups[i].Backdrop);
					fs.Position += 0x16;
					bw.Write(FlightGroups[i].Unknown34); fs.Position++;
					bw.Write(FlightGroups[i].Unknown35); fs.Position++;
					bw.Write(FlightGroups[i].Unknown36); fs.Position++;
					bw.Write(FlightGroups[i].Unknown37); fs.Position++;
					bw.Write(FlightGroups[i].Unknown38); fs.Position++;
					bw.Write(FlightGroups[i].Unknown39); fs.Position++;
					bw.Write(FlightGroups[i].Unknown40); fs.Position++;
					bw.Write(FlightGroups[i].Unknown41);
					fs.Position = p + 0xE3E;
					#endregion
				}
				#endregion
				#region Messages
				for (i=0;i<NumMessages;i++)
				{
					p = fs.Position;
					bw.Write((short)i);
					bw.Write(Messages[i].MessageString.ToCharArray());
					fs.Position = p + 0x52;
					for (int j=0;j<10;j++) bw.Write(Messages[i].SentTo[j]);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 0), 0, 6);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 1), 0, 6);
					fs.WriteByte(Messages[i].Unknown1);
					fs.Position++;
					bw.Write(Messages[i].TrigAndOr[0]);
					fs.Position++;
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 2), 0, 6);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 3), 0, 6);
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[1]);
					fs.Position++;
					bw.Write(Messages[i].VoiceID.ToCharArray());
					fs.Position = p + 0x84;
					fs.WriteByte(Messages[i].OriginatingFG);
					fs.Position += 7;
					fs.WriteByte(Messages[i].DelaySeconds);
					fs.WriteByte(Messages[i].DelayMinutes);
					fs.WriteByte(Messages[i].Color);
					bw.Write(Messages[i].TrigAndOr[2]);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 4), 0, 6);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 5), 0, 6);
					fs.Position += 2;
					bw.Write(Messages[i].TrigAndOr[3]);
					fs.Position++;
					bw.Write(Messages[i].Unknown2);
					fs.Position++;
				}
				#endregion
				#region Globals
				for(i=0;i<Globals.Count;i++)
				{
					p = fs.Position;
					bw.Write((short)3);
					for (int k=0;k<3;k++)
					{
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, k*4), 0, 6);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, k*4+1), 0, 6);
						fs.Position += 2;
						bw.Write(Globals[i].AndOr[k*3+0]);
						bw.Write(Globals[i].Unknown1[k]);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, k*4+2), 0, 6);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, k*4+3), 0, 6);
						fs.Position += 2;
						bw.Write(Globals[i].AndOr[k*3+1]);
						fs.Position += 8;
						bw.Write(Globals[i].Unknown2[k]);
						fs.Position += 9;
						bw.Write(Globals[i].AndOr[k*3+2]);
						fs.WriteByte(Globals[i].Unknown3[k]);
						fs.WriteByte((byte)(Globals[i].GetGoalPoints(k) / 25));
						fs.WriteByte(Globals[i].Unknown4[k]);
						fs.WriteByte(Globals[i].Unknown5[k]);
						fs.WriteByte(Globals[i].Unknown6[k]);
						fs.Position++;
						fs.WriteByte(Globals[i].ActiveSequence[k]);
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
						bw.Write(Teams[i].GetEndOfMissionMessage(j).ToCharArray());
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
					fs.Write(Briefings[i].Events, 0, Briefings[i].Events.Length);
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
				bw.Write(_notes.ToCharArray());
				fs.Position = p + 0x18E0;
				fs.Position += 0x319C;	// TODO: write unknown message notes
				for (i=0;i<64;i++)
				{
					p = fs.Position;
					if (i < Messages.Count) bw.Write(Messages[i].Note.ToCharArray());
					fs.Position = p + 0x64;
				}
				fs.Position += 0x1B58;	// TODO: write unkown message notes
				fs.Position += 0x12C;	// TODO: write EoM? notes
				#endregion
				#region FG Goal Strings
				for (i=0;i<NumFlightGroups;i++)
					for (int j=0;j<8;j++)	// per goal
						for (int k=0;k<3;k++)	// per string
							if (FlightGroups[i].GoalStrings[j, k] != "")
							{
								p = fs.Position;
								bw.Write(FlightGroups[i].GoalStrings[j, k].ToCharArray());
								fs.Position = p + 0x40;
							}
							else fs.Position++;
				#endregion
				#region Globals strings
				for (i=0;i<10;i++)	// per team
					for (int j=0;j<12;j++)	// per goal
						for (int k=0;k<3;k++)	// per string
							if (Globals[i].GetGoalString(j, k) != "")
							{
								p = fs.Position;
								bw.Write(Globals[i].GetGoalString(j, k).ToCharArray());
								fs.Position = p + 0x40;
							}
							else fs.Position++;
				#endregion
				fs.Position += 0x1E0;	// unknown space
				#region Order strings
				for (i=0;i<192;i++) // per FG (and then some)
					for (int j=0;j<16;j++) // per order
						if (i < FlightGroups.Count && FlightGroups[i].OrderStrings[j] != "")
						{
							p = fs.Position;
							bw.Write(FlightGroups[i].OrderStrings[j].ToCharArray());
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
				bw.Write((short)0x2106);
				fs.SetLength(fs.Position);
				fs.Close();
			}
			catch
			{
				fs.Close();
				throw;
			}
		}

		/// <summary>Save the mission to a new location</summary>
		/// <param name="filePath">Full path to the new file location</param>
		public void Save(string filePath)
		{
			_missionPath = filePath;
			Save();
		}

		/// <value>The full path to the mission file</value>
		/// <remarks>Defaults to "\\NewMission.tie"</remarks>
		public string MissionPath
		{
			get { return _missionPath; }
			set { _missionPath = value; }
		}
		/// <value>The file name of the mission file</value>
		/// <remarks>Defaults to "NewMission.tie"</remarks>
		public string MissionFileName { get { return _missionPath.Substring(_missionPath.LastIndexOf("\\")+1); } }
		/// <value>Gets the number of FlightGroups in the mission</value>
		public short NumFlightGroups { get { return (short)_flightGroups.Count; } }
		/// <value>Gets the number of In-Flight Messages in the mission</value>
		public short NumMessages { get { return (short)_messages.Count; } }
		/// <value>Maximum number of craft that can exist at one time in a single region</value>
		/// <remarks>Value is 96</value>
		public const int CraftLimit = 96;
		/// <value>Maximum number of FlightGroups that can exist in the mission file</value>
		/// <remarks>Value is 100</remarks>
		public const int FlightGroupLimit = 100;
		/// <value>Maximum number of In-Flight Messages that can exist in the mission file</value>
		/// <remarks>Value is 64</remarks>
		public const int MessageLimit = 64;
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x0B</remarks>
		public bool Unknown2
		{
			get { return _unknown2; }
			set { _unknown2 = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x08</remarks>
		public bool Unknown1
		{
			get { return _unknown1; }
			set { _unknown1 = value; }
		}

		/// <summary>Returns the name of the selected IFF</summary>
		/// <param name="index">Index of IFF, 0-5</param>
		/// <returns>Name of IFF</returns>
		public string GetIff(int index) { return _iff[index]; }

		/// <summary>Sets the name of the selected IFF</summary>
		/// <param name="index">Index of IFF, 2-5</param>
		/// <param name="iff">IFF name, 19 character limit</param>
		public void SetIff(int index, string iff)
		{
			if (index < 2) return;
			if (iff.Length > 19) _iff[index] = iff.Substring(0, 19);
			else _iff[index] = iff;
		}

		/// <summary>Returns the name of the selected Region</summary>
		/// <param name="index">Index of Region, 0-3</param>
		/// <returns>Region name</returns>
		public string GetRegion(int index) { return _region[index]; }

		/// <summary>Sets the name of the selected Region</summary>
		/// <param name="index">Index of Region, 0-3</param>
		/// <param name="region">Region name, 131 character limit</param>
		public void SetRegion(int index, string region)
		{
			if (region.Length > 0x83) _region[index] = region.Substring(0, 0x83);
			else _region[index] = region;
		}
		
		/// <summary>Returns the name of the selected Global Group</summary>
		/// <param name="index">Index of Global Group, 0-15</param>
		/// <returns>Global Group name</returns>
		public string GetGlobalGroupName(int index) { return _globalGroup[index]; }

		/// <summary>Sets the name of the selected Global Group</summary>
		/// <param name="index">Index of the Global Group, 0-15</param>
		/// <param name="name">Global Group name, 56 character limit</param>
		public void SetGlobalGroupName(int index, string name)
		{
			if (name.Length > 56) _globalGroup[index] = name.Substring(0, 56);
			else _globalGroup[index] = name;
		}

		/// <value>The Global Cargos for the mission</value>
		public GlobCarg[] GlobalCargo { get { return _globalCargo; } }
		/// <value>The start mode of the player (aka MissionType)</value>
		public byte Hangar
		{
			get { return _hangar; }
			set { _hangar = value; }
		}
		/// <value>Minutes value of the time limit</value>
		public byte TimeLimitMin
		{
			get { return _timeLimitMin; }
			set { _timeLimitMin = value; }
		}
		/// <value>Determines if the mission will automatically end when Primary goals are complete</value>
		public bool EndWhenComplete
		{
			get { return _endWhenComplete; }
			set { _endWhenComplete = value; }
		}
		/// <value>Voice of in-game mission update messages</value>
		public byte Officer
		{
			get { return _officer; }
			set { _officer = value; }
		}
		/// <value>Briefing image</value>
		public byte Logo
		{
			get { return _logo; }
			set { _logo = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x23B3</remarks>
		public byte Unknown3
		{
			get { return _unknown3; }
			set { _unknown3 = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x23B4</remarks>
		public byte Unknown4
		{
			get { return _unknown4; }
			set { _unknown4 = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x23B5</remarks>
		public byte Unknown5
		{
			get { return _unknown5; }
			set { _unknown5 = value; }
		}
		/// <value>Summary of the mission</value>
		/// <remarks>4095 char limit</remarks>
		public string MissionDescription
		{
			get { return _missionDescription.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				if (s.Length > 4095) _missionDescription = s.Substring(0, 4095);
				else _missionDescription = s;
			}
		}
		/// <value>Debriefing text</value>
		/// <remarks>4095 char limit</remarks>
		public string MissionFailed
		{
			get { return _missionFailed.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (!s.Contains("#")) s = "#" + s;
				if (s.Length > 4095) _missionFailed = s.Substring(0, 4095);
				else _missionFailed = s;
			}
		}
		/// <value>Debriefing text</value>
		/// <remarks>4096 char limit</remarks>
		public string MissionSuccessful
		{
			get { return _missionSuccessful.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (s.Length > 4096) _missionSuccessful = s.Substring(0, 4096);
				else _missionSuccessful = s;
			}
		}
		/// <value>The FlightGroups for the mission</value>
		/// <remarks>Defaults to one FlightGroup</remarks>
		public FlightGroupCollection FlightGroups
		{
			get { return _flightGroups; }
			set { _flightGroups = value; }
		}
		/// <value>The In-Flight Messages for the mission</value>
		/// <remarks>Defaults to zero messages</remarks>
		public MessageCollection Messages
		{
			get { return _messages; }
			set { _messages = value; }
		}
		/// <value>The Global Goals for the mission</value>
		public GlobalsCollection Globals { get { return _globals; } }
		/// <value>The Teams for the mission</value>
		public TeamCollection Teams { get { return _teams; } }
		/// <value>The Briefings for the mission</value>
		public BriefingCollection Briefings
		{
			get { return _briefings; }
			set { if (value.Count == _briefings.Count) _briefings = value; }
		}

		/// <summary>Container for Global Cargo data</summary>
		/// <remarks><i>Cargo</i> is restricted to 63 characters</remarks>
		public struct GlobCarg
		{
			private string _cargo;	// 63 CHAR

			/// <value>Cargo string</value>
			public string Cargo
			{
				get { return _cargo; }
				set
				{
					if (value.Length > 63) _cargo = value.Substring(0, 63);
					else _cargo = value;
				}
			}
			/// <value>Unknown value, local 0x44</value>
			public bool Unknown1;
			/// <value>Unknown value, local 0x48</value>
			public byte Unknown2;
			/// <value>Unknown value, local 0x49</value>
			public byte Unknown3;
			/// <value>Unknown value, local 0x4A</value>
			public byte Unknown4;
			/// <value>Unknown value, local 0x4B</value>
			public byte Unknown5;
		}
	}
}
