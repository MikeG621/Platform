using System;
using System.IO;

namespace Idmr.Platform.Tie
{
	/// <summary>Framework for TIE95</summary>
	/// <remarks>This is the primary container object for a TIE95 mission file</remarks>
	public class Mission
	{
		private string _missionPath = "\\NewMission.tie";
		private BriefingOfficers _officers = BriefingOfficers.FlightOfficer;
		private bool _capturedOnEjection = false;	//Rescued or Captured
		private string[] _endOfMissionMessages = new string[6];	//Mission Comp/Fail messages
		private string[] _iff = new string[6];
		private bool[] _iffHostile = new bool[6];
		private FlightGroupCollection _flightGroups = new FlightGroupCollection();
		private MessageCollection _messages = new MessageCollection();
		private Globals _globalGoals = new Globals();
		private Briefing _briefing = new Briefing();
		private Questions _briefingQuestions = new Questions();
		
		/// <summary>Pre- and Post-mission officers</summary>
		/// <remarks>0 = None<br>1 = Both<br>2 = FlightOfficer<br>3 = SecretOrder</remarks>
		public enum BriefingOfficers : byte { None, Both, FlightOfficer, SecretOrder };
		/// <summary>Parts of a Trigger array</summary>
		/// <remarks>0 = Trigger<br>1 = TrigType<br>2 = Variable<br>3 = Amount</remarks>
		public enum TriggerIndexes : byte { Trigger, TrigType, Variable, Amount };

		/// <summary>Default constructor, creates a blank mission</summary>
		public Mission()
		{
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			for (int i=2;i<6;i++) _iff[i] = "";
		}

		/// <summary>Create a new mission from a file</summary>
		/// <param name="filePath">Full path to the file</param>
		public Mission(string filePath)
		{
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			LoadFromFile(filePath);
		}

		/// <summary>Create a new mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		public Mission(FileStream stream)
		{
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			LoadFromStream(stream);
		}

		/// <summary>Load a mission from a file</summary>
		/// <remarks>Calls LoadFromStream()</remarks>
		/// <param name="filePath">Full path to the file</param>
		/// <exception cref="System.IO.FileNotFoundException"><i>filePath</i> does not exist</exception>
		/// <exception cref="System.IO.InvalidDataException"><i>filePath</i> is not a TIE mission file</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			if (MissionFile.GetPlatform(filePath) != MissionFile.Platform.TIE) throw new InvalidDataException("File is not a valid TIE mission file");
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Load a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="System.IO.InvalidDataException">Thrown when stream is not a valid TIE mission file</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (MissionFile.GetPlatform(stream) != MissionFile.Platform.TIE) throw new InvalidDataException("Stream is not a valid TIE mission file");
			BinaryReader br = new BinaryReader(stream);
			int i;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			stream.Position = 0xA;
			try { OfficersPresent = (Mission.BriefingOfficers)br.ReadByte(); }
			catch { OfficersPresent = Mission.BriefingOfficers.Both; }
			stream.Position = 0xD;
			CapturedOnEjection = br.ReadBoolean();
			stream.Position = 0x18;
			for (i=0;i<6;i++) SetEndOfMissionMessage(i, new string(br.ReadChars(64)).Trim('\0'));
			stream.Position += 2;
			byte[] buffer = new byte[64];
			#region IFFs
			for (i=2;i<6;i++)
			{
				SetIffHostile(i, false);
				SetIff(i, new string(br.ReadChars(12)).Trim('\0'));
				if (GetIff(i) != "" && GetIff(i)[0] == '1')
				{
					SetIffHostile(i, true);
					SetIff(i, GetIff(i).Substring(1));
				}
			}
			#endregion
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			for (i=0;i<NumFlightGroups;i++)
			{
				#region Craft
				int j;
				FlightGroups[i].Name = new string(br.ReadChars(12)).Trim('\0');
				if (FlightGroups[i].Name.IndexOf('\0') != -1) FlightGroups[i].Name = FlightGroups[i].Name.Substring(0, FlightGroups[i].Name.IndexOf('\0'));
				FlightGroups[i].Pilot = new string(br.ReadChars(12)).Trim('\0');	//not used by TIE
				FlightGroups[i].Cargo = new string(br.ReadChars(12)).Trim('\0');
				FlightGroups[i].SpecialCargo = new string(br.ReadChars(12)).Trim('\0');
				stream.Read(buffer, 0, 0x38);
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
				FlightGroups[i].AI = buffer[8];
				FlightGroups[i].Markings = buffer[9];
				FlightGroups[i].Radio = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Unknowns[0] = buffer[0xB];
				FlightGroups[i].Formation = buffer[0xC];
				FlightGroups[i].FormDistance = buffer[0xD];
				FlightGroups[i].GlobalGroup = buffer[0xE];
				FlightGroups[i].FormLeaderDist = buffer[0xF];
				FlightGroups[i].NumberOfWaves = (byte)(buffer[0x10]+1);
				FlightGroups[i].Unknowns[1] = buffer[0x11];
				FlightGroups[i].PlayerCraft = buffer[0x12];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x13] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x14] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x15] * 360 / 0x100);
				FlightGroups[i].Unknowns[2] = buffer[0x16];
				FlightGroups[i].Unknowns[3] = buffer[0x17];
				FlightGroups[i].Unknowns[4] = buffer[0x18];
				#endregion
				#region Arr/Dep
				FlightGroups[i].Difficulty = buffer[0x19];
				/// Triggers are built as 4-byte arrays
				/// [0] = Trigger, [1] = Trigger type, [2] = Variable, [3] = Amount
				for (j=0;j<4;j++)
				{
					FlightGroups[i].ArrDepTrigger[0, j] = buffer[0x1A+j];
					FlightGroups[i].ArrDepTrigger[1, j] = buffer[0x1E+j];
				}
				FlightGroups[i].AT1AndOrAT2 = Convert.ToBoolean(buffer[0x22]);
				FlightGroups[i].Unknowns[5] = buffer[0x23];
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				for (j=0;j<4;j++) { FlightGroups[i].ArrDepTrigger[2, j] = buffer[0x26+j]; }
				FlightGroups[i].DepartureTimerMinutes = buffer[0x2A];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x2B];
				FlightGroups[i].AbortTrigger = buffer[0x2C];
				FlightGroups[i].Unknowns[6] = buffer[0x2D];
				FlightGroups[i].Unknowns[7] = buffer[0x2E];
				FlightGroups[i].Unknowns[8] = buffer[0x2F];
				FlightGroups[i].ArrivalCraft1 = buffer[0x30];
				FlightGroups[i].ArrivalMethod1 = Convert.ToBoolean(buffer[0x31]);
				FlightGroups[i].DepartureCraft1 = buffer[0x32];
				FlightGroups[i].DepartureMethod1 = Convert.ToBoolean(buffer[0x33]);
				FlightGroups[i].ArrivalCraft2 = buffer[0x34];
				FlightGroups[i].ArrivalMethod2 = Convert.ToBoolean(buffer[0x35]);
				FlightGroups[i].DepartureCraft2 = buffer[0x36];
				FlightGroups[i].DepartureMethod2 = Convert.ToBoolean(buffer[0x37]);
				#endregion
				#region Orders
				stream.Read(buffer, 0, 0x36);
				for (j=0;j<3;j++)
				{
					int k;
					for (k=0;k<18;k++) { FlightGroups[i].Orders[j, k] = buffer[k+j*18]; }
				}
				#endregion
				stream.Read(buffer, 0, 0xA);
				for (j=0;j<9;j++) { FlightGroups[i].Goals[j] = buffer[j]; }
				#region Waypoints
				for (j=0;j<4;j++)
				{
					if (j == 0 || j == 2) stream.Read(buffer, 0, 60);
					int k = 0;
					if (j == 1 || j == 3) k = 30;
					int h;
					for (h=0;h<15;h++) FlightGroups[i].Waypoints[h, j] = BitConverter.ToInt16(buffer, h*2+k);
				}
				#endregion
				stream.Position += 4;
			}
			#endregion
			#region Messages
			if (numMessages != 0)
			{
				Messages = new MessageCollection(numMessages);
				for (i=0;i<NumMessages;i++)
				{
					Messages[i].MessageString = new string(br.ReadChars(64)).Trim('\0');
					if (Messages[i].MessageString.IndexOf('\0') != -1) Messages[i].MessageString = Messages[i].MessageString.Substring(0, Messages[i].MessageString.IndexOf('\0'));
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
					stream.Read(buffer, 0, 0x1A);
					int j;
					for (j=0;j<4;j++)
					{
						if (buffer[j] == 0) break;
						Messages[i].Triggers[0, j] = buffer[j];
						Messages[i].Triggers[1, j] = buffer[j+4];
					}
					Messages[i].Short = "";
					for (j=8;j<24;j++) { Messages[i].Short += Convert.ToChar(buffer[j]).ToString(); }
					Messages[i].Delay = buffer[24];
					Messages[i].Trig1AndOrTrig2 = Convert.ToBoolean(buffer[25]);
				}
			}
			else { Messages.Clear(); }
			#endregion
			#region Globals
			for (i=0;i<3;i++)
			{
				stream.Read(buffer, 0, 8);
				for (int j=0;j<4;j++)
				{
					GlobalGoals.Triggers[2*i, j] = buffer[j];
					GlobalGoals.Triggers[2*i+1, j] = buffer[j+4];
				}
				stream.Position += 0x11;
				//for some reason, there's triggers with Var set with no Type
				if (GlobalGoals.Triggers[2*i, 1] == 0) GlobalGoals.Triggers[2*i, 2] = 0;
				if (GlobalGoals.Triggers[2*i+1, 1] == 0) GlobalGoals.Triggers[2*i+1, 2] = 0;
				GlobalGoals.AndOr[i] = br.ReadBoolean();
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
				for (int j=0;j<64;j++) { Briefing.Events[j+64*i] = buffer[j]; }
			}
			stream.Read(buffer, 0, 0x20);
			for (i=0;i<32;i++) { Briefing.Events[0x300+i] = buffer[i]; }
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				if (j > 0) for (int k=0;k<j;k++) { Briefing.BriefingTag[i] += Convert.ToChar(stream.ReadByte()).ToString(); }
			}
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				if (j > 0) for (int k=0;k<j;k++) { Briefing.BriefingString[i] += Convert.ToChar(stream.ReadByte()).ToString(); }
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
					int b = stream.ReadByte();
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
					stream.Position += 3;	// goddamn TFW-isms
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
					int b = stream.ReadByte();
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
				bw.Write((short)-1);
				bw.Write(NumFlightGroups);
				bw.Write(NumMessages);
				bw.Write((short)3);
				fs.Position = 0xA;
				fs.WriteByte((byte)_officers);
				fs.Position = 0xD;
				bw.Write(_capturedOnEjection);
				fs.Position = 0x18;
				#region End of Mission Messages
				for (int i=0;i<6;i++)
				{
					long p = fs.Position;
					bw.Write(_endOfMissionMessages[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x40;
				}
				#endregion
				#region IFFs
				fs.Position += 2;
				for (int i=2;i<6;i++)
				{
					long p = fs.Position;
					if (_iffHostile[i]) bw.Write('1');
					bw.Write(_iff[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0xC;
				}
				#endregion
				#region Flightgroups
				for (int i=0;i<NumFlightGroups;i++)
				{
					long p = fs.Position;
					int j;
					#region Craft
					bw.Write(_flightGroups[i].Name.ToCharArray());
					fs.Position = p + 0xC;
					bw.Write(_flightGroups[i].Pilot.ToCharArray());
					fs.Position = p + 0x18;
					bw.Write(_flightGroups[i].Cargo.ToCharArray());
					fs.Position = p + 0x24;
					bw.Write(_flightGroups[i].SpecialCargo.ToCharArray());
					fs.Position = p + 0x30;
					if (_flightGroups[i].SpecialCargoCraft == 0) fs.WriteByte(_flightGroups[i].NumberOfCraft);
					else fs.WriteByte((byte)(_flightGroups[i].SpecialCargoCraft-1));
					bw.Write(_flightGroups[i].RandSpecCargo);
					fs.WriteByte(_flightGroups[i].CraftType);
					fs.WriteByte(_flightGroups[i].NumberOfCraft);
					fs.WriteByte(_flightGroups[i].Status1);
					fs.WriteByte(_flightGroups[i].Missile);
					fs.WriteByte(_flightGroups[i].Beam);
					fs.WriteByte(_flightGroups[i].IFF);
					fs.WriteByte(_flightGroups[i].AI);
					fs.WriteByte(_flightGroups[i].Markings);
					bw.Write(_flightGroups[i].Radio);
					fs.WriteByte(_flightGroups[i].Unknowns[0]);
					fs.WriteByte(_flightGroups[i].Formation);
					fs.WriteByte(_flightGroups[i].FormDistance);
					fs.WriteByte(_flightGroups[i].GlobalGroup);
					fs.WriteByte(_flightGroups[i].FormLeaderDist);
					fs.WriteByte((byte)(_flightGroups[i].NumberOfWaves-1));
					fs.WriteByte(_flightGroups[i].Unknowns[1]);
					fs.WriteByte(_flightGroups[i].PlayerCraft);
					fs.WriteByte((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					fs.WriteByte((byte)((FlightGroups[i].Pitch >= 64 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					fs.WriteByte((byte)(FlightGroups[i].Roll * 0x100 / 360));
					fs.WriteByte(_flightGroups[i].Unknowns[2]);
					fs.WriteByte(_flightGroups[i].Unknowns[3]);
					fs.WriteByte(_flightGroups[i].Unknowns[4]);
					#endregion
					#region Arr/Dep
					fs.WriteByte(_flightGroups[i].Difficulty);
					fs.Write(MissionFile.Trigger(_flightGroups[i].ArrDepTrigger, 0), 0, 4);
					fs.Write(MissionFile.Trigger(_flightGroups[i].ArrDepTrigger, 1), 0, 4);
					bw.Write(_flightGroups[i].AT1AndOrAT2);
					fs.WriteByte(_flightGroups[i].Unknowns[5]);
					fs.WriteByte(_flightGroups[i].ArrivalDelayMinutes);
					fs.WriteByte(_flightGroups[i].ArrivalDelaySeconds);
					fs.Write(MissionFile.Trigger(_flightGroups[i].ArrDepTrigger, 2), 0, 4);
					fs.WriteByte(_flightGroups[i].DepartureTimerMinutes);
					fs.WriteByte(_flightGroups[i].DepartureTimerSeconds);
					fs.WriteByte(_flightGroups[i].AbortTrigger);
					fs.WriteByte(_flightGroups[i].Unknowns[6]);
					fs.WriteByte(_flightGroups[i].Unknowns[7]);
					fs.WriteByte(_flightGroups[i].Unknowns[8]);
					fs.WriteByte(_flightGroups[i].ArrivalCraft1);
					bw.Write(_flightGroups[i].ArrivalMethod1);
					fs.WriteByte(_flightGroups[i].DepartureCraft1);
					bw.Write(_flightGroups[i].DepartureMethod1);
					fs.WriteByte(_flightGroups[i].ArrivalCraft2);
					bw.Write(_flightGroups[i].ArrivalMethod2);
					fs.WriteByte(_flightGroups[i].DepartureCraft2);
					bw.Write(_flightGroups[i].DepartureMethod2);
					#endregion
					for (j=0;j<3;j++)
					{
						int k;
						for (k=0;k<18;k++) fs.WriteByte(_flightGroups[i].Orders[j, k]);
					}
					fs.Write(_flightGroups[i].Goals, 0, 10);
					for (j=0;j<4;j++)
					{
						int k;
						for (k=0;k<15;k++) bw.Write(_flightGroups[i].Waypoints[k, j]);
					}
					fs.Position = p + 0x124;
				}
				#endregion
				#region Messages
				for (int i=0;i<NumMessages;i++)
				{
					long p = fs.Position;
					if (_messages[i].Color != 0) bw.Write((byte)(_messages[i].Color + 0x30));
					bw.Write(_messages[i].MessageString.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x3F;
					fs.WriteByte(0);
					fs.Write(MissionFile.Trigger(_messages[i].Triggers, 0), 0, 4);
					fs.Write(MissionFile.Trigger(_messages[i].Triggers, 1), 0, 4);
					bw.Write(_messages[i].Short.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x57;
					fs.WriteByte(0);
					fs.WriteByte(_messages[i].Delay);
					bw.Write(_messages[i].Trig1AndOrTrig2);
				}
				#endregion
				#region Globals
				for (int i=0;i<3;i++)
				{
					fs.Write(MissionFile.Trigger(_globalGoals.Triggers, i*2), 0, 4);
					fs.Write(MissionFile.Trigger(_globalGoals.Triggers, i*2+1), 0, 4);
					fs.Position += 0x11;
					bw.Write(_globalGoals.AndOr[i]);
					fs.Position += 2;
				}
				#endregion
				#region Briefing
				bw.Write(_briefing.Length);
				bw.Write(_briefing.Unknown1);
				bw.Write(_briefing.StartLength);
				bw.Write(_briefing.EventsLength);
				fs.Position += 2;
				fs.Write(_briefing.Events, 0, 0x320);
				for (int i=0;i<32;i++)
				{
					string str_t = _briefing.BriefingTag[i].Replace("\r", "");
					int j = _briefing.BriefingTag[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				for (int i=0;i<32;i++)
				{
					string str_t = _briefing.BriefingString[i].Replace("\r", "");
					int j = _briefing.BriefingString[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				#endregion
				#region Questions
				for (int i=0;i<10;i++)
				{
					int j;
					string str_q = _briefingQuestions.PreMissQuestions[i];
					string str_a = _briefingQuestions.PreMissAnswers[i];
					str_q = str_q.Replace("\r", "");
					str_a = str_a.Replace("\r", "");
					j = str_q.Length + str_a.Length;
					if (j == 0)
					{
						bw.Write((short)0);
						continue;	//if it doesn't exist, don't even bother with the rest of this
					}
					j++;	//takes into account the q/a spacer
					long p = fs.Position;
					fs.Position += 2;
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
					long p2 = fs.Position;
					fs.Position = p;
					bw.Write((short)j);		//calc length on the fly
					fs.Position = p2;
				}
				for (int i=0;i<10;i++)
				{
					int j;
					string str_q = _briefingQuestions.PostMissQuestions[i];
					string str_a = _briefingQuestions.PostMissAnswers[i];
					str_q = str_q.Replace("\r", "");
					str_a = str_a.Replace("\r", "");
					j = str_q.Length + str_a.Length;
					if (j == 0)
					{
						bw.Write((short)0);
						continue;
					}
					j += 3;
					long p = fs.Position;
					fs.Position += 2;
					fs.WriteByte(_briefingQuestions.PostTrigger[i]);
					fs.WriteByte(_briefingQuestions.PostTrigType[i]);
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
					long p2 = fs.Position;
					fs.Position = p;
					bw.Write((short)j);		//calc length on the fly
					fs.Position = p2;
				}
				#endregion
				bw.Write((short)0x2106); fs.WriteByte(0xFF);
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

		/// <summary>Returns the Messages displayed at end of mission</summary>
		/// <param name="index">Index of Message: 0=PrimComp1, 1=PrimComp2, 2=SecComp1... 5=PrimFail2</param>
		/// <returns>Message after the mission goal condition is met</returns>
		public string GetEndOfMissionMessage(int index) { try { return _endOfMissionMessages[index]; } catch { return ""; } }

		/// <summary>Sets the Message displayed at the end of mission</summary>
		/// <remarks>If <i>message</i> exceeds the limit it will be truncated</remarks>
		/// <param name="index">Index of Message: 0=PrimComp1, 1=PrimComp2, 2=SecComp1... 5=PrimFail2</param>
		/// <param name="message">Message, 63 char limit</param>
		public void SetEndOfMissionMessage(int index, string message)
		{
			if (index < 0 || index > 5) return;
			if (message.Length > 63) _endOfMissionMessages[index] = message.Substring(0, 63);
			else _endOfMissionMessages[index] = message;
		}

		/// <summary>Returns the name of the IFFs</summary>
		/// <param name="index">Index of IFF, 0-5</param>
		/// <returns>Name of IFF</returns>
		public string GetIff(int index) { try { return _iff[index]; } catch { return ""; } }

		/// <summary>Sets the name of the IFFs</summary>
		/// <remarks>If <i>iff</i> exceeds the limit it will be truncated. Rebel and Imperial names are fixed.</remarks>
		/// <param name="index">Index of IFF, 2-5</param>
		/// <param name="iff">IFF name, 11 character limit</param>
		public void SetIff(int index, string iff)
		{
			if (index < 2 || index > 5) return;
			if (iff.Length > 11) _iff[index] = iff.Substring(0, 11);
			else _iff[index] = iff;
		}

		/// <summary>Returns the IFF behaviour towards player</summary>
		/// <param name="index">Index of IFF, 0-5</param>
		/// <returns><i>true</i> if Hostile, <i>false</i> if friendly</returns>
		public bool GetIffHostile(int index) { try { return _iffHostile[index]; } catch { return false; } }

		/// <summary>Sets the IFF behaviour towards player</summary>
		/// <remarks>Rebel and Imperial behaviours are fixed</remarks>
		/// <param name="index">Index of IFF, 2-5</param>
		/// <param name="hostile">i>true</i> if hostile, <i>false</i> if friendly</param>
		public void SetIffHostile(int index, bool hostile)
		{
			if (index < 2 || index > 5) return;
			_iffHostile[index] = hostile;
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
		/// <value>Maximum number of craft that can exist at one time in-game</value>
		/// <remarks>Value is 28</value>
		public const int CraftLimit = 28;
		/// <value>Maximum number of FlightGroups that can exist in the mission file</value>
		/// <remarks>Value is 48</remarks>
		public const int FlightGroupLimit = 48;
		/// <value>Maximum number of In-Flight Messages that can exist in the mission file</value>
		/// <remarks>Value is 16</remarks>
		public const int MessageLimit = 16;
		/// <value>Determines officers present before and after the mission</value>
		public BriefingOfficers OfficersPresent
		{
			get { return _officers; }
			set { _officers = value; }
		}
		/// <value>Determines if the pilot is captured upon ejection or destruction</value>
		/// <remarks><i>true</i> results in capture, <i>false</i> results in rescue</remarks>
		public bool CapturedOnEjection
		{
			get { return _capturedOnEjection; }
			set { _capturedOnEjection = value; }
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
		public Globals GlobalGoals { get { return _globalGoals; } }
		/// <value>The Briefing for the mission</value>
		public Briefing Briefing
		{
			get { return _briefing; }
			set { _briefing = value; }
		}
		/// <value>The questions for the Briefing Officers</value>
		public Questions BriefingQuestions { get { return _briefingQuestions; } }
	}
}
