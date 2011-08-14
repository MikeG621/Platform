using System;
using System.IO;

namespace Idmr.Platform.Xvt
{
	/// <summary>Framework for XvT and BoP</summary>
	/// <remarks>This is the primary container object for XvT and BoP mission files</remarks>
	public class Mission
	{
		private string _missionPath = "\\NewMission.tie";
		private bool _bop = false;
		private byte _unknown1 = 0;	// 0x0006
		private byte _unknown2 = 0;	// 0x0008
		private bool _unknown3 = false;	// 0x000B
		private string _unknown4 = "";	// 0x0028, 16 char, BoP
		private string _unknown5 = "";	// 0x0050, 16 char, BoP
		private byte _missionType = 0;
		private bool _unknown6 = false;	// 0x0065
		private byte _timeLimitMin = 0;
		private byte _timeLimitSec = 0;
		private string _missionDescription = "";
		private string _missionFailed = "";	// BoP
		private string _missionSuccessful = "";	// BoP
		private FlightGroupCollection _flightGroups = new FlightGroupCollection();
		private MessageCollection _messages = new MessageCollection();
		private BriefingCollection _briefings = new BriefingCollection();
		private GlobalsCollection _globals = new GlobalsCollection();
		private TeamCollection _teams = new TeamCollection();

		/// <summary>Parts of a Trigger array</summary>
		/// <remarks>0 = Trigger<br>1 = TrigType<br>2 = Variable<br>3 = Amount</remarks>
		public enum TriggerIndexes : byte { Trigger, TrigType, Variable, Amount };

		/// <summary>Default constructor, create a blank mission</summary>
		public Mission() { }

		/// <summary>Create a new mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		public Mission(string filePath) { LoadFromFile(filePath); }

		/// <summary>Create a new mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		public Mission(FileStream stream) { LoadFromStream(stream); }

		/// <summary>Load a mission from a file</summary>
		/// <remarks>Calls LoadFromStream()</remarks>
		/// <param name="filePath">Full path to the file</param>
		/// <exception cref="System.IO.FileNotFoundException"><i>filePath</i> does not exist</exception>
		/// <exception cref="System.IO.InvalidDataException"><i>filePath</i> is not a XvT or BoP mission file</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			MissionFile.Platform p = MissionFile.GetPlatform(filePath);
			if (p != MissionFile.Platform.XvT && p != MissionFile.Platform.BoP) throw new InvalidDataException("File is not a valid XvT/BoP mission file");
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="InvalidDataException"><i>stream</i> is not a valid XvT or BoP mission file</exception>
		public void LoadFromStream(FileStream stream)
		{
			MissionFile.Platform p = MissionFile.GetPlatform(stream);
			if (p != MissionFile.Platform.XvT && p != MissionFile.Platform.BoP) throw new InvalidDataException("File is not a valid XvT/BoP mission file");
			_bop = (p == MissionFile.Platform.BoP ? true : false);
			BinaryReader br = new BinaryReader(stream);
			int i, j;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			#region Platform
			Unknown1 = br.ReadByte();
			stream.Position++;
			Unknown2 = br.ReadByte();
			stream.Position = 0xB;
			Unknown3 = Convert.ToBoolean(br.ReadByte());
			stream.Position = 0x28;
			Unknown4 = new string(br.ReadChars(0x10)).Trim('\0');
			stream.Position = 0x50;
			Unknown5 = new string(br.ReadChars(0x10)).Trim('\0');
			stream.Position = 0x64;
			MissionType = br.ReadByte();
			Unknown6 = Convert.ToBoolean(br.ReadByte());
			TimeLimitMin = br.ReadByte();
			TimeLimitSec = br.ReadByte();
			stream.Position = 0xA4;
			#endregion
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			byte[] buffer = new byte[64];
			for (i=0;i<numFlightGroups;i++)
			{
				#region Craft
				FlightGroups[i].Name = new string(br.ReadChars(0x14)).Trim('\0');	// null-termed
				for (j=0;j<4;j++) FlightGroups[i].Roles[j] = new string(br.ReadChars(4)).Trim('\0');
				stream.Position += 4;
				FlightGroups[i].Cargo = new string(br.ReadChars(0x14)).Trim('\0');	// null-termed
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(0x14)).Trim('\0');	// null-termed
				stream.Read(buffer, 0, 0x1A);
				FlightGroups[i].SpecialCargoCraft = buffer[0];
				FlightGroups[i].RandSpecCargo = Convert.ToBoolean(buffer[1]);
				FlightGroups[i].CraftType = buffer[2];
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
				FlightGroups[i].Unknowns.Unknown1 = buffer[0x12];
				FlightGroups[i].Unknowns.Unknown2 = Convert.ToBoolean(buffer[0x13]);
				FlightGroups[i].PlayerNumber = buffer[0x14];
				FlightGroups[i].ArriveOnlyIfHuman = Convert.ToBoolean(buffer[0x15]);
				FlightGroups[i].PlayerCraft = buffer[0x16];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x17] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x18] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x19] * 360 / 0x100);
				stream.Position += 3;
				#endregion
				#region Arr/Dep
				stream.Read(buffer, 0, 0x35);
				FlightGroups[i].Difficulty = buffer[0];
				for (j=0;j<4;j++)
				{
					FlightGroups[i].ArrDepTrigger[0, j] = buffer[1+j];	// Arr1...
					FlightGroups[i].ArrDepTrigger[1, j] = buffer[5+j];
					FlightGroups[i].ArrDepTrigger[2, j] = buffer[0xC+j];
					FlightGroups[i].ArrDepTrigger[3, j] = buffer[0x10+j];
					FlightGroups[i].ArrDepTrigger[4, j] = buffer[0x1B+j];	// Dep1...
					FlightGroups[i].ArrDepTrigger[5, j] = buffer[0x1F+j];
				}
				FlightGroups[i].ArrDepAO[0] = Convert.ToBoolean(buffer[0xB]);
				FlightGroups[i].ArrDepAO[1] = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].ArrDepAO[2] = Convert.ToBoolean(buffer[0x17]);
				FlightGroups[i].Unknowns.Unknown3 = buffer[0x18];
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x19];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x1A];
				FlightGroups[i].ArrDepAO[3] = Convert.ToBoolean(buffer[0x25]);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x26];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x27];
				FlightGroups[i].AbortTrigger = buffer[0x28];
				FlightGroups[i].Unknowns.Unknown4 = buffer[0x29];
				FlightGroups[i].Unknowns.Unknown5 = buffer[0x2B];
				FlightGroups[i].ArrivalCraft1 = buffer[0x2D];
				FlightGroups[i].ArrivalMethod1 = Convert.ToBoolean(buffer[0x2E]);	// false = hyper, true = mothership
				FlightGroups[i].ArrivalCraft2 = buffer[0x2F];
				FlightGroups[i].ArrivalMethod2 = Convert.ToBoolean(buffer[0x30]);
				FlightGroups[i].DepartureCraft1 = buffer[0x31];
				FlightGroups[i].DepartureMethod1 = Convert.ToBoolean(buffer[0x32]);
				FlightGroups[i].DepartureCraft2 = buffer[0x33];
				FlightGroups[i].DepartureMethod2 = Convert.ToBoolean(buffer[0x34]);
				#endregion
				#region Orders
				for (j=0;j<4;j++)
				{
					stream.Read(buffer, 0, 0x13);
					for (int h=0;h<0x13;h++) FlightGroups[i].Orders[j, h] = buffer[h];
					FlightGroups[i].OrderDesignations[j] = new string(br.ReadChars(16)).Trim('\0');	// null-termed
					stream.Position += 0x2F;
				}
				stream.Read(buffer, 0, 0xB);
				for (j=0;j<4;j++)
				{
					FlightGroups[i].SkipToOrder4Trigger[0, j] = buffer[j];
					FlightGroups[i].SkipToOrder4Trigger[1, j] = buffer[4+j];
				}
				FlightGroups[i].SkipToO4T1AndOrT2 = Convert.ToBoolean(buffer[0xA]);
				#endregion
				#region Goals
				for (j=0;j<8;j++)
				{
					stream.Read(buffer, 0, 0xF);
					for (int h=0;h<0xF;h++) FlightGroups[i].Goals[j, h] = buffer[h];
					stream.Position += 0x3F;
				}
				stream.Position++;
				#endregion
				for (j=0;j<4;j++) for (int k=0;k<22;k++) FlightGroups[i].Waypoints[k, j] = br.ReadInt16();
				#region Options/Other
				FlightGroups[i].Unknowns.Unknown17 = br.ReadBoolean();
				stream.Position++;
				FlightGroups[i].Unknowns.Unknown18 = br.ReadBoolean();
				stream.Position += 7;
				stream.Read(buffer, 0, 0xF);
				FlightGroups[i].Unknowns.Unknown19 = Convert.ToBoolean(buffer[0]);
				FlightGroups[i].Unknowns.Unknown20 = buffer[1];
				FlightGroups[i].Unknowns.Unknown21 = buffer[2];
				FlightGroups[i].Countermeasures = buffer[3];
				FlightGroups[i].ExplosionTime = buffer[4];
				FlightGroups[i].Status2 = buffer[5];
				FlightGroups[i].GlobalUnit = buffer[6];
				FlightGroups[i].Unknowns.Unknown22 = Convert.ToBoolean(buffer[7]);
				FlightGroups[i].Unknowns.Unknown23 = Convert.ToBoolean(buffer[8]);
				FlightGroups[i].Unknowns.Unknown24 = Convert.ToBoolean(buffer[9]);
				FlightGroups[i].Unknowns.Unknown25 = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Unknowns.Unknown26 = Convert.ToBoolean(buffer[0xB]);
				FlightGroups[i].Unknowns.Unknown27 = Convert.ToBoolean(buffer[0xC]);
				FlightGroups[i].Unknowns.Unknown28 = Convert.ToBoolean(buffer[0xD]);
				FlightGroups[i].Unknowns.Unknown29 = Convert.ToBoolean(buffer[0xE]);
				stream.Position++;
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
					Messages[i].MessageString = new string(br.ReadChars(64)).Trim('\0');		// null-termed
					Messages[i].Color = 0;
					if (Messages[i].MessageString[0] == '1')
					{
						Messages[i].Color = 1;
						Messages[i].MessageString = Messages[i].MessageString.Substring(1);
					}
					if (Messages[i].MessageString[0] == '2')
					{
						Messages[i].Color = 2;
						Messages[i].MessageString = Messages[i].MessageString.Substring(1);
					}
					if (Messages[i].MessageString[0] == '3')
					{
						Messages[i].Color = 3;
						Messages[i].MessageString = Messages[i].MessageString.Substring(1);
					}
					stream.Read(buffer, 0, 0x20);
					for (j=0;j<10;j++) Messages[i].SentToTeam[j] = Convert.ToBoolean(buffer[j]);
					for (j=0;j<4;j++)
					{
						Messages[i].Triggers[0, j] = buffer[0xA+j];
						Messages[i].Triggers[1, j] = buffer[0xE+j];
						Messages[i].Triggers[2, j] = buffer[0x15+j];
						Messages[i].Triggers[3, j] = buffer[0x19+j];
					}
					Messages[i].T1AndOrT2 = Convert.ToBoolean(buffer[0x14]);
					Messages[i].T3AndOrT4 = Convert.ToBoolean(buffer[0x1F]);
					Messages[i].Note = new string(br.ReadChars(16)).Trim('\0');	// null-termed
					Messages[i].Delay = br.ReadByte();
					Messages[i].T12AndOrT34 = Convert.ToBoolean(br.ReadByte());
				}
			}
			else Messages.Clear();
			#endregion
			#region Globals
			Globals.ClearAll();
			for (i=0;i<10;i++)
			{
				stream.Position += 2;
				for (int k=0;k<3;k++)
				{
					stream.Read(buffer, 0, 8);
					for (j=0;j<4;j++)
					{
						Globals[i].Triggers[k*4, j] = buffer[j];
						Globals[i].Triggers[k*4+1, j] = buffer[j+4];
					}
					stream.Position += 2;
					Globals[i].AndOr[k*3] = Convert.ToBoolean(br.ReadByte());
					stream.Read(buffer, 0, 8);
					for (j=0;j<4;j++)
					{
						Globals[i].Triggers[k*4+2, j] = buffer[j];
						Globals[i].Triggers[k*4+3, j] = buffer[j+4];
					}
					stream.Position += 2;
					Globals[i].AndOr[k*3+1] = Convert.ToBoolean(br.ReadByte());
					stream.Position += 0x11;
					Globals[i].AndOr[k*3+2] = Convert.ToBoolean(br.ReadByte());
					stream.Position++;
					Globals[i].SetGoalPoints(k, (short)(br.ReadSByte() * 250));
				}
			}
			#endregion
			#region Teams
			Teams.ClearAll();
			for (i=0;i<10;i++)
			{
				stream.Position += 2;
				Teams[i].Name = new string(br.ReadChars(0x10)).Trim('\0');	// null-termed
				stream.Position += 8;
				for (j=0;j<10;j++) Teams[i].AlliedWithTeam[j] = Convert.ToBoolean(br.ReadByte());
				for (j=0;j<6;j++)
				{
					Teams[i].SetEndOfMissionMessage(j, new string(br.ReadChars(0x40)).Trim('\0'));
					if (Teams[i].GetEndOfMissionMessage(j) != "")
					{
						string c = Teams[i].GetEndOfMissionMessage(j).Substring(0, 1);
						if (c == "1" || c == "2" || c == "3")
						{
							Teams[i].EndOfMissionMessageColor[j] = Convert.ToByte(c);
							Teams[i].SetEndOfMissionMessage(j, Teams[i].GetEndOfMissionMessage(j).Substring(1));
						}
					}
				}
				stream.Position += 0x43;
			}
			#endregion
			#region Briefing
			Briefings.ClearAll();
			for (i=0;i<8;i++)
			{
				Briefings[i].Length = br.ReadInt16();
				Briefings[i].Unknown1 = br.ReadInt16();
				stream.Position += 4;	// StartLength, EventsLength
				Briefings[i].Unknown3 = br.ReadInt16();
				for (j=0;j<12;j++) stream.Read(Briefings[i].Events, j*0x40, 0x40);
				stream.Read(Briefings[i].Events, 0x300, 0x2A);
				for (j=0;j<32;j++)
				{
					int k = br.ReadInt16();
					Briefings[i].BriefingTag[j] = "";
					if (k > 0) Briefings[i].BriefingTag[j] = new string(br.ReadChars(k)).Trim('\0');	// shouldn't need the trim
				}
				for (j=0;j<32;j++)
				{
					int k = br.ReadInt16();
					Briefings[i].BriefingString[j] = "";
					if (k > 0) Briefings[i].BriefingString[j] = new string(br.ReadChars(k)).Trim('\0');
				}
			}
			#endregion
			#region FG goal strings
			for (i=0;i<NumFlightGroups;i++)
			{
				for (j=0;j<8;j++)
				{
					for (int k=0;k<3;k++) FlightGroups[i].GoalStrings[j, k] = new string(br.ReadChars(0x40)).Trim('\0');
				}
			}
			#endregion
			#region Globals strings
			for (i=0;i<10;i++)
			{
				for (j=0;j<12;j++)
				{
					for (int k=0;k<3;k++)
					{
						if (j>=8 && k==0) { stream.Position += 0x40; continue; }	// skip Sec Inc
						if (j>=4 && k==2) { stream.Position += 0x40; continue; }	// skip Prev & Sec Fail
						Globals[i].SetGoalString(j, k, new string(br.ReadChars(0x40)).Trim('\0'));
					}
				}
				stream.Position += 0xC00;
			}
			#endregion
			#region Debriefs
			if (BoP)
			{
				_missionSuccessful = new string(br.ReadChars(0x1000)).Trim('\0');
				_missionFailed = new string(br.ReadChars(0x1000)).Trim('\0');
				_missionDescription = new string(br.ReadChars(0x1000)).Trim('\0');
			}
			else MissionDescription = new string(br.ReadChars(0x400)).Trim('\0');
			#endregion
			_missionPath = stream.Name;
		}

		/// <summary>Save the mission with the default path</summary>
		public void Save()
		{
			FileStream fs = null;
			try
			{
				if (File.Exists(_missionPath)) File.Delete(_missionPath);
				fs = File.OpenWrite(_missionPath);
				BinaryWriter bw = new BinaryWriter(fs);
				int i;
				long p;
				#region Platform
				if (BoP) bw.Write((short)14);
				else bw.Write((short)12);
				bw.Write(NumFlightGroups);
				bw.Write(NumMessages);
				bw.Write((short)Unknown1);
				bw.Write((short)Unknown2);
				fs.Position++;
				bw.Write(Unknown3);
				fs.Position = 0x28;
				bw.Write(Unknown4.ToCharArray());
				fs.WriteByte(0);	// just to ensure termination
				fs.Position = 0x50;
				bw.Write(Unknown5.ToCharArray());
				fs.WriteByte(0);	// just to ensure termination
				fs.Position = 0x64;
				bw.Write(MissionType);
				bw.Write(Unknown6);
				bw.Write(TimeLimitMin);
				bw.Write(TimeLimitSec);
				fs.Position = 0xA4;
				#endregion
				#region FlightGroups
				for (i = 0;i<NumFlightGroups;i++)
				{
					p = fs.Position;
					int j;
					#region Craft
					bw.Write(FlightGroups[i].Name.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x14;
					for (j=0;j<4;j++)
					{
						string s = FlightGroups[i].Roles[j];
						if (FlightGroups[i].Roles[j] != "") bw.Write((s.Length > 4 ? s.Substring(0, 4).ToCharArray() : s.ToCharArray()));
						else bw.Write((Int32)0);
					}
					fs.Position = p + 0x28;
					bw.Write(FlightGroups[i].Cargo.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x3C;
					bw.Write(FlightGroups[i].SpecialCargo.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x50;
					fs.WriteByte(FlightGroups[i].SpecialCargoCraft);
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
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown1);
					bw.Write(FlightGroups[i].Unknowns.Unknown2);
					fs.WriteByte(FlightGroups[i].PlayerNumber);
					bw.Write(FlightGroups[i].ArriveOnlyIfHuman);
					fs.WriteByte(FlightGroups[i].PlayerCraft);
					fs.WriteByte((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					fs.WriteByte((byte)((FlightGroups[i].Pitch >= 64 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					fs.WriteByte((byte)(FlightGroups[i].Roll * 0x100 / 360));
					fs.Position = p + 0x6D;
					#endregion
					#region Arr/Dep
					fs.WriteByte(FlightGroups[i].Difficulty);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 0), 0, 4);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 1), 0, 4);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[0]);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 2), 0, 4);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 3), 0, 4);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[1]);
					bw.Write(FlightGroups[i].ArrDepAO[2]);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown3);
					fs.WriteByte(FlightGroups[i].ArrivalDelayMinutes);
					fs.WriteByte(FlightGroups[i].ArrivalDelaySeconds);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 4), 0, 4);
					fs.Write(MissionFile.Trigger(FlightGroups[i].ArrDepTrigger, 5), 0, 4);
					fs.Position += 2;
					bw.Write(FlightGroups[i].ArrDepAO[3]);
					fs.WriteByte(FlightGroups[i].DepartureTimerMinutes);
					fs.WriteByte(FlightGroups[i].DepartureTimerSeconds);
					fs.WriteByte(FlightGroups[i].AbortTrigger);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown4);
					fs.Position++;
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown5);
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
					for (j=0;j<4;j++)
					{
						for (int k=0;k<0x13;k++) fs.WriteByte(FlightGroups[i].Orders[j, k]);
						string s = FlightGroups[i].OrderDesignations[j];
						bw.Write((s.Length > 16 ? s.Substring(0, 16).ToCharArray() : s.ToCharArray()));
						fs.WriteByte(0);
						fs.Position = p + 0xF4 + (j*0x52);
					}
					fs.Write(MissionFile.Trigger(FlightGroups[i].SkipToOrder4Trigger, 0), 0, 4);
					fs.Write(MissionFile.Trigger(FlightGroups[i].SkipToOrder4Trigger, 1), 0, 4);
					fs.Position += 2;
					bw.Write(FlightGroups[i].SkipToO4T1AndOrT2);
					#endregion
					#region Goals
					for (j=0;j<8;j++)
					{
						for (int k=0;k<0xF;k++) fs.WriteByte(FlightGroups[i].Goals[j, k]);
						fs.Position = p + 0x243 + (j*0x4E);
					}
					fs.Position++;
					#endregion
					for (j=0;j<4;j++)
						for (int k=0;k<22;k++) bw.Write(FlightGroups[i].Waypoints[k, j]);
					#region Options/Other
					bw.Write(FlightGroups[i].Unknowns.Unknown17);
					fs.Position++;
					bw.Write(FlightGroups[i].Unknowns.Unknown18);
					fs.Position += 7;
					bw.Write(FlightGroups[i].Unknowns.Unknown19);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown20);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown21);
					fs.WriteByte(FlightGroups[i].Countermeasures);
					fs.WriteByte(FlightGroups[i].ExplosionTime);
					fs.WriteByte(FlightGroups[i].Status2);
					fs.WriteByte(FlightGroups[i].GlobalUnit);
					bw.Write(FlightGroups[i].Unknowns.Unknown22);
					bw.Write(FlightGroups[i].Unknowns.Unknown23);
					bw.Write(FlightGroups[i].Unknowns.Unknown24);
					bw.Write(FlightGroups[i].Unknowns.Unknown25);
					bw.Write(FlightGroups[i].Unknowns.Unknown26);
					bw.Write(FlightGroups[i].Unknowns.Unknown27);
					bw.Write(FlightGroups[i].Unknowns.Unknown28);
					bw.Write(FlightGroups[i].Unknowns.Unknown29);
					fs.Position++;
					for (j=1;j<8;j++) if (FlightGroups[i].OptLoadout[j]) bw.Write((byte)j); else fs.WriteByte(0);	// warheads
					fs.Position++;	// only writing 7
					for (j=1;j<4;j++) if (FlightGroups[i].OptLoadout[j+8]) bw.Write((byte)j); else fs.WriteByte(0);	// CMs
					fs.Position += 3;	// only writing 3
					for (j=1;j<3;j++) if (FlightGroups[i].OptLoadout[j+12]) bw.Write((byte)j); else fs.WriteByte(0);	// beam
					fs.Position += 2;	// only writing 2
					fs.WriteByte(FlightGroups[i].OptCraftCategory);
					for (j=0;j<3;j++) for (int k=0;k<10;k++) fs.WriteByte(FlightGroups[i].OptCraft[k, j]);
					fs.Position++;
					#endregion
				}
				#endregion
				#region Messages
				for (i=0;i<NumMessages;i++)
				{
					p = fs.Position;
					bw.Write((short)i);
					bw.Write(Messages[i].MessageString.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x42;
					for (int j=0;j<Messages[i].SentToTeam.Length;j++) bw.Write(Messages[i].SentToTeam[j]);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 0), 0, 4);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 1), 0, 4);
					fs.Position += 2;
					bw.Write(Messages[i].T1AndOrT2);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 2), 0, 4);
					fs.Write(MissionFile.Trigger(Messages[i].Triggers, 3), 0, 4);
					fs.Position += 2;
					bw.Write(Messages[i].T3AndOrT4);
					bw.Write(Messages[i].Note.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x72;
					fs.WriteByte(Messages[i].Delay);
					bw.Write(Messages[i].T12AndOrT34);
				}
				#endregion
				#region Globals
				for (i=0;i<10;i++)
				{
					bw.Write((short)3);
					for (int j=0;j<3;j++)
					{
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, j*4), 0, 4);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, j*4+1), 0, 4);
						fs.Position += 2;
						bw.Write(Globals[i].AndOr[j*3]);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, j*4+2), 0, 4);
						fs.Write(MissionFile.Trigger(Globals[i].Triggers, j*4+3), 0, 4);
						fs.Position += 2;
						bw.Write(Globals[i].AndOr[j*3+1]);
						fs.Position += 0x11;
						bw.Write(Globals[i].AndOr[j*3+2]);
						fs.Position++;
						fs.WriteByte((byte)(Globals[i].GetGoalPoints(j) / 250));
					}
				}
				#endregion
				#region Teams
				for (i=0;i<10;i++)
				{
					p = fs.Position;
					bw.Write((short)1);
					bw.Write(Teams[i].Name.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x1A;
					for (int j=0;j<10;j++) bw.Write(Teams[i].AlliedWithTeam[j]);
					for (int j=0;j<6;j++)
					{
						bw.Write(Teams[i].GetEndOfMissionMessage(j).ToCharArray());
						fs.WriteByte(0);
						fs.Position = p + 0x64 + j*0x40;
					}
					fs.Position = p + 0x1E7;
				}
				#endregion
				#region Briefing
				for (i=0;i<8;i++)
				{
					bw.Write(Briefings[i].Length);
					bw.Write(Briefings[i].Unknown1);
					bw.Write(Briefings[i].StartLength);
					bw.Write(Briefings[i].EventsLength);
					bw.Write(Briefings[i].Unknown3);
					fs.Write(Briefings[i].Events, 0, Briefings[i].Events.Length);
					for (int j=0;j<32;j++)
					{
						bw.Write((short)Briefings[i].BriefingTag[j].Length);
						if (Briefings[i].BriefingTag[j].Length != 0) bw.Write(Briefings[i].BriefingTag[j].ToCharArray());
					}
					for (int j=0;j<32;j++)
					{
						bw.Write((short)Briefings[i].BriefingString[j].Length);
						if (Briefings[i].BriefingString[j].Length != 0) bw.Write(Briefings[i].BriefingString[j].ToCharArray());
					}
				}
				#endregion
				#region FG Goal Strings
				for (i=0;i<NumFlightGroups;i++)
				{
					for (int j=0;j<8;j++)
					{
						for (int k=0;k<3;k++)
						{
							p = fs.Position;
							bw.Write(FlightGroups[i].GoalStrings[j, k].ToCharArray());
							fs.WriteByte(0);
							fs.Position = p + 0x40;
						}
					}
				}
				#endregion
				#region Global Goal Strings
				for (i=0;i<10;i++)
				{
					int j;
					for (j=0;j<12;j++)
					{
						for (int k=0;k<3;k++)
						{
							if (j >= 8 && k==0) { fs.Position += 0x40; continue; }	// skip Sec Inc
							if (j >= 4 && k==2) { fs.Position += 0x40; continue; }	// skip Prev & Sec Fail
							p = fs.Position;
							bw.Write(Globals[i].GetGoalString(j, k).ToCharArray());
							fs.WriteByte(0);
							fs.Position = p + 0x40;
						}
					}
					fs.Position += 0xC00;
				}
				#endregion
				#region Debriefs
				p = fs.Position;
				if (BoP)
				{
					bw.Write(_missionSuccessful.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x1000;
					bw.Write(_missionFailed.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x2000;
					bw.Write(_missionDescription.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x3000;
				}
				else
				{
					bw.Write(_missionDescription.ToCharArray());
					fs.WriteByte(0);
					fs.Position = p + 0x400;
				}
				bw.Write((short)0x2106);
				fs.SetLength(fs.Position);
				#endregion
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
		/// <value>Defines if mission is XvT or BoP</value>
		/// <remarks><i>true</i> for Balance of Power</remarks>
		public bool BoP
		{
			get { return _bop; }
			set { _bop = value; }
		}
		/// <value>Gets the number of FlightGroups in the mission</value>
		public short NumFlightGroups { get { return (short)_flightGroups.Count; } }
		/// <value>Gets the number of In-Flight Messages in the mission</value>
		public short NumMessages { get { return (short)_messages.Count; } }
		/// <value>Maximum number of craft that can exist at one time in-game</value>
		/// <remarks>Value is 32</value>
		public const int CraftLimit = 32;
		/// <value>Maximum number of FlightGroups that can exist in the mission file</value>
		/// <remarks>Value is 46</remarks>
		public const int FlightGroupLimit = 46;
		/// <value>Maximum number of In-Flight Messages that can exist in the mission file</value>
		/// <remarks>Value is 64</remarks>
		public const int MessageLimit = 64;
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x06</remarks>
		public byte Unknown1
		{
			get { return _unknown1; }
			set { _unknown1 = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x08</remarks>
		public byte Unknown2
		{
			get { return _unknown2; }
			set { _unknown2 = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x0B</remarks>
		public bool Unknown3
		{
			get { return _unknown3; }
			set { _unknown3 = value; }
		}
		/// <value>Unknown FileHeader value (BoP only?)</value>
		/// <remarks>Offset = 0x28, 16 char</remarks>
		public string Unknown4
		{
			get { return _unknown4; }
			set
			{
				if (value.Length > 16) _unknown4 = value.Substring(0, 16);
				else _unknown4 = value;
			}
		}
		/// <value>Unknown FileHeader value (BoP only?)</value>
		/// <remarks>Offset = 0x50, 16 char</remarks>
		public string Unknown5
		{
			get { return _unknown5; }
			set
			{
				if (value.Length > 16) _unknown5 = value.Substring(0, 16);
				else _unknown5 = value;
			}
		}
		/// <value>Defines which category the mission belongs to</value>
		public byte MissionType
		{
			get { return _missionType; }
			set { _missionType = value; }
		}
		/// <value>Unknown FileHeader value</value>
		/// <remarks>Offset = 0x65</remarks>
		public bool Unknown6
		{
			get { return _unknown6; }
			set { _unknown6 = value; }
		}
		/// <value>Minutes value of the time limit</value>
		public byte TimeLimitMin
		{
			get { return _timeLimitMin; }
			set { _timeLimitMin = value; }
		}
		/// <value>Seconds value of the time limit</value>
		public byte TimeLimitSec
		{
			get { return _timeLimitSec; }
			set { _timeLimitSec = value; }
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
		/// <value>Summary of the mission</value>
		/// <remarks>1023 char limit for XvT, 4095 char limit for BoP</remarks>
		public string MissionDescription
		{
			get { return _missionDescription.Replace("$","\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (s.Length > (_bop ? 0x1000 : 0x400)) _missionDescription = s.Substring(0, (_bop ? 0x1000 : 0x400));
				_missionDescription = s;
			}
		}
		/// <value>Debriefing text (BoP only)</value>
		/// <remarks>4095 char limit</remarks>
		public string MissionFailed
		{
			get { return _missionFailed.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (s.Length > 0x1000) _missionFailed = s.Substring(0, 0x1000);
				else _missionFailed = s;
			}
		}
		/// <value>Debriefing text (BoP only)</value>
		/// <remarks>4095 char limit</remarks>
		public string MissionSuccessful
		{
			get { return _missionSuccessful.Replace("$", "\r\n"); }
			set
			{
				string s = value.Replace("\r\n", "$");
				if (s.Length > 0x1000) _missionSuccessful = s.Substring(0, 0x1000);
				else _missionSuccessful = s;
			}
		}
	}
}
