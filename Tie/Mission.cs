/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using System.IO;
using Idmr.Common;

namespace Idmr.Platform.Tie
{
	/// <summary>Framework for TIE95</summary>
	/// <remarks>This is the primary container object for a TIE95 mission file</remarks>
	public class Mission
	{
		string[] _endOfMissionMessages = new string[6];	//Mission Comp/Fail messages
		string[] _iff = new string[6];
		bool[] _iffHostile = new bool[6];
		IffNameIndexer _iffNameIndexer;
		IffHostileIndexer _iffHostileIndexer;
		EndOfMissionIndexer _endOfMissionIndexer;
		
		/// <summary>Pre- and Post-mission officers</summary>
		/// <remarks>0 = None<br>1 = Both<br>2 = FlightOfficer<br>3 = SecretOrder</remarks>
		public enum BriefingOfficers : byte { None, Both, FlightOfficer, SecretOrder };

		#region constructors
		/// <summary>Default constructor, creates a blank mission</summary>
		public Mission()
		{
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			for (int i=2;i<6;i++) _iff[i] = "";
			_iffNameIndexer = new IffNameIndexer(this);
			_iffHostileIndexer = new IffHostileIndexer(this);
			_endOfMissionIndexer = new EndOfMissionIndexer(this);
		}

		/// <summary>Creates a new mission from a file</summary>
		/// <remarks>Calls LoadFromFile( )</remarks>
		/// <param name="filePath">Full path to the file</param>
		public Mission(string filePath)
		{
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			LoadFromFile(filePath);
		}

		/// <summary>Creates a new mission from an open FileStream</summary>
		/// <remarks>Calls LoadFromStream( )</remarks>
		/// <param name="stream">Opened FileStream to mission file</param>
		public Mission(FileStream stream)
		{
			_iff = Strings.IFF;
			_iffHostile[0] = true;
			LoadFromStream(stream);
		}
		#endregion constructors
		
		#region public methods
		/// <summary>Loads a mission from a file</summary>
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

		/// <summary>Loads a mission from an open FileStream</summary>
		/// <param name="stream">Opened FileStream to mission file</param>
		/// <exception cref="System.IO.InvalidDataException"><i>stream</i> is not a valid TIE mission file</exception>
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
			for (i=0;i<6;i++) EndOfMissionMessages[i] = new string(br.ReadChars(64));
			stream.Position += 2;
			byte[] buffer = new byte[64];
			for (i=2;i<6;i++) IFFs[i] = new string(br.ReadChars(12));
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			for (i=0;i<NumFlightGroups;i++)
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
				if (FlightGroups[i].SpecialCargoCraft >= FlightGroups[i].NumberOfCraft) FlightGroups[i].SpecialCargoCraft = 0;
				else FlightGroups[i].SpecialCargoCraft++;
				FlightGroups[i].Status1 = buffer[4];
				FlightGroups[i].Missile = buffer[5];
				FlightGroups[i].Beam = buffer[6];
				FlightGroups[i].IFF = buffer[7];
				FlightGroups[i].AI = buffer[8];
				FlightGroups[i].Markings = buffer[9];
				FlightGroups[i].Radio = Convert.ToBoolean(buffer[0xA]);
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
				FlightGroups[i].Unknowns.Unknown9 = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].Unknowns.Unknown10 = buffer[0x17];
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
				for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) FlightGroups[i].Waypoints[k][j] = br.ReadInt16();
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
				GlobalGoals.Goals[2].Triggers[1] = new Trigger(buffer, 4);
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
				for (int j=0;j<64;j++) { Briefing.Events[j+64*i] = buffer[j]; }
			}
			stream.Read(buffer, 0, 0x20);
			for (i=0;i<32;i++) { Briefing.Events[0x300+i] = buffer[i]; }
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				//if (j > 0) for (int k=0;k<j;k++) { Briefing.BriefingTag[i] += Convert.ToChar(stream.ReadByte()).ToString(); }
				if (j > 0) Briefing.BriefingTag[i] = new string(br.ReadChars(j));
			}
			for (i=0;i<32;i++)
			{
				int j = br.ReadInt16();
				//if (j > 0) for (int k=0;k<j;k++) { Briefing.BriefingString[i] += Convert.ToChar(stream.ReadByte()).ToString(); }
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
			MissionPath = stream.Name;
		}

		/// <summary>Saves the mission to <i>MissionPath</i></summary>
		public void Save()
		{
			FileStream fs = null;
			try
			{

				if (File.Exists(MissionPath)) File.Delete(MissionPath);
				fs = File.OpenWrite(MissionPath);
				BinaryWriter bw = new BinaryWriter(fs);
				bw.Write((short)-1);
				bw.Write(NumFlightGroups);
				bw.Write(NumMessages);
				bw.Write((short)3);
				fs.Position = 0xA;
				fs.WriteByte((byte)OfficersPresent);
				fs.Position = 0xD;
				bw.Write(CapturedOnEjection);
				fs.Position = 0x18;
				for (int i=0;i<6;i++)
				{
					long p = fs.Position;
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
				for (int i=0;i<NumFlightGroups;i++)
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
					bw.Write(FlightGroups[i].Radio);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown1);
					fs.WriteByte(FlightGroups[i].Formation);
					fs.WriteByte(FlightGroups[i].FormDistance);
					fs.WriteByte(FlightGroups[i].GlobalGroup);
					fs.WriteByte(FlightGroups[i].FormLeaderDist);
					fs.WriteByte((byte)(FlightGroups[i].NumberOfWaves-1));
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown5);
					fs.WriteByte(FlightGroups[i].PlayerCraft);
					fs.WriteByte((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					fs.WriteByte((byte)((FlightGroups[i].Pitch >= 64 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					fs.WriteByte((byte)(FlightGroups[i].Roll * 0x100 / 360));
					bw.Write(FlightGroups[i].Unknowns.Unknown9);
					fs.WriteByte(FlightGroups[i].Unknowns.Unknown10);
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
					for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) bw.Write(FlightGroups[i].Waypoints[k][j]);
					fs.Position = p + 0x124;
				}
				#endregion
				#region Messages
				for (int i=0;i<NumMessages;i++)
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
				fs.Write(Briefing.Events, 0, 0x320);
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
					string str_q = BriefingQuestions.PostMissQuestions[i];
					string str_a = BriefingQuestions.PostMissAnswers[i];
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
					fs.WriteByte(BriefingQuestions.PostTrigger[i]);
					fs.WriteByte(BriefingQuestions.PostTrigType[i]);
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
					fs.Position = p;
					bw.Write((short)j);		//calc length on the fly
					fs.Position += j;
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

		/// <summary>Saves the mission to a new <i>MissionPath</i></summary>
		/// <param name="filePath">Full path to the new <i>MissionPath</i></param>
		public void Save(string filePath)
		{
			MissionPath = filePath;
			Save();
		}
		#endregion public methods
		
		#region public properties
		/// <summary>Gets the array accessor for the IFF names</summary>
		public IffNameIndexer IFFs { get { return _iffNameIndexer; } }
		/// <summary>Gets the array accessor for the IFF behaviour</summary>
		public IffHostileIndexer IffHostile { get { return _iffHostileIndexer; } }
		/// <summary>Gets the array accessor for the EoM Messages</summary>
		public EndOfMissionIndexer EndOfMissionMessages { get { return _endOfMissionIndexer; } }
		
		/// <summary>Gets or sets the full path to the mission file</summary>
		/// <remarks>Defaults to "\\NewMission.tie"</remarks>
		public string MissionPath = "\\NewMission.tie";
		/// <summary>Gets the file name of the mission file</summary>
		/// <remarks>Defaults to "NewMission.tie"</remarks>
		public string MissionFileName { get { return StringFunctions.GetFileName(MissionPath); } }
		/// <summary>Gets the number of FlightGroups in the mission</summary>
		public short NumFlightGroups { get { return (short)FlightGroups.Count; } }
		/// <summary>Gets the number of In-Flight Messages in the mission</summary>
		public short NumMessages { get { return (short)Messages.Count; } }
		/// <summary>Maximum number of craft that can exist at one time in-game</summary>
		/// <remarks>Value is 28</summary>
		public const int CraftLimit = 28;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file</summary>
		/// <remarks>Value is 48</remarks>
		public const int FlightGroupLimit = 48;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file</summary>
		/// <remarks>Value is 16</remarks>
		public const int MessageLimit = 16;
		/// <summary>Gets or sets the officers present before and after the mission</summary>
		/// <remarks>Defaults to <i>FlightOfficer</i></remarks>
		public BriefingOfficers OfficersPresent = BriefingOfficers.FlightOfficer;
		/// <summary>Gets or sets if the pilot is captured upon ejection or destruction</summary>
		/// <remarks><i>true</i> results in capture, <i>false</i> results in rescue (default)</remarks>
		public bool CapturedOnEjection = false;
		/// <summary>Gets or sets the FlightGroups for the mission</summary>
		/// <remarks>Defaults to one FlightGroup</remarks>
		public FlightGroupCollection FlightGroups = new FlightGroupCollection();
		/// <summary>Gets or sets the In-Flight Messages for the mission</summary>
		/// <remarks>Defaults to zero messages</remarks>
		public MessageCollection Messages = new MessageCollection();
		/// <summary>Gets or sets the Global Goals for the mission</summary>
		public Globals GlobalGoals  = new Globals();
		/// <summary>Gets or sets the Briefing for the mission</summary>
		public Briefing Briefing = new Briefing();
		/// <summary>Gets or sets the questions for the Briefing Officers</summary>
		public Questions BriefingQuestions = new Questions();
		#endregion public properties
		
		/// <summary>Object to provide array access to the IFF names</summary>
		public class IffNameIndexer	// : IIndexer<string>
		{	//TODO: Idmr.Common.Generics.IIndexer
			Mission _owner;
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent Mission</param>
			public IffNameIndexer(Mission parent) { _owner = parent; }
			
			/// <summary>Gets the length of the array</summary>
			public int Length { get { return _owner._iff.Length; } }	// IIndexer.Length { get; }
			
			/// <summary>Gets or sets the IFF Name</summary>
			/// <remarks>11 character limit, Rebel and Imperial are read-only</remarks>
			/// <param name="index">IFF index</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public string this[int index]	// <string> IIndexer.this[int index]
			{
				get { return _owner._iff[index]; }
				set
				{
					if (index < 2) return;
					if (value != "" && value[0] == '1')
					{
						_owner._iffHostile[index] = true;
						value = value.Substring(1);
					}
					_owner._iff[index] = StringFunctions.GetTrimmed(value, 11);
				}
			}
		}
		
		/// <summary>Object to provide array access to the IFF behaviours</summary>
		public class IffHostileIndexer
		{
			Mission _owner;
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent Mission</param>
			public IffHostileIndexer(Mission parent) { _owner = parent; }
			
			/// <summary>Gets the length of the array</summary>
			public int Length { get { return _owner._iffHostile.Length; } }
			
			/// <summary>Gets or sets the IFF behaviour</summary>
			/// <remarks>Imperial is read-only</remarks>
			/// <param name="index">IFF index</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public bool this[int index]
			{
				get { return _owner._iffHostile[index]; }
				set { if (index > 1) _owner._iffHostile[index] = value; }
			}
		}

		/// <summary>Object to provide array access to the EoM Messages</summary>
		public class EndOfMissionIndexer
		{
			Mission _owner;
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent Mission</param>
			public EndOfMissionIndexer(Mission parent) { _owner = parent; }
			
			/// <summary>Gets the length of the array</summary>
			public int Length { get { return _owner._endOfMissionMessages.Length; } }
			
			/// <summary>Gets or sets the EoM Message</summary>
			/// <remarks>63 character limit</remarks>
			/// <param name="index">EoM index</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public string this[int index]
			{
				get { return _owner._endOfMissionMessages[index]; }
				set { _owner._endOfMissionMessages[index] = StringFunctions.GetTrimmed(value, 63); }
			}
		}
		/// <summary>Object for a single Trigger</summary>
		[Serializable] public class Trigger : ITrigger
		{
			byte _condition = 0;
			byte _variableType = 0;
			byte _variable = 0;
			byte _amount = 0;
			
			/// <summary>Initializes a blank Trigger</summary>
			public Trigger() { }
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data, must have Length of 4</param>
			/// <exception cref="ArgumentException">Invalid <i>raw</i>Length value</exception>
			public Trigger(byte[] raw)
			{
				if (raw.Length != 4) throw new ArgumentException("raw does not have the correct length", "raw");
				_condition = raw[0];
				_variableType = raw[1];
				_variable = raw[2];
				_amount = raw[3];
			}
			
			/// <summary>Initializes a new Trigger from raw data</summary>
			/// <param name="raw">Raw data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="IndexOutOfBoundsException"><i>startIndex</i> results in reading outside the range of <i>raw</i></exception>
			public Trigger(byte[] raw, int startIndex)
			{
				_condition = raw[startIndex];
				_variableType = raw[startIndex + 1];
				_variable = raw[startIndex + 2];
				_amount = raw[startIndex + 3];
			}

			#region ITrigger Members
			/// <summary>The array form of the Trigger</summary>
			/// <param name="index">Condition, VariableType, Variable, Amount</param>
			/// <exception cref="ArgumentException"><i>index</i> must be 0-3</exception>
			public byte this[int index]
			{
				get
				{
					if (index == 0) return _condition;
					else if (index == 1) return _variableType;
					else if (index == 2) return _variable;
					else if (index == 3) return _amount;
					else throw new ArgumentException("index must be 0-3", "index");
				}
				set
				{
					if (index == 0) _condition = value;
					else if (index == 1) _variableType = value;
					else if (index == 2) _variable = value;
					else if (index == 3) _amount = value;
				}
			}
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return 4; } }
			
			/// <summary>Gets or sets the Trigger itself</summary>
			public byte Condition
			{
				get { return _condition; }
				set { _condition = value; }
			}
			/// <summary>Gets or sets the category <i>Variable</i> belongs to</summary>
			public byte VariableType
			{
				get { return _variableType; }
				set { _variableType = value; }
			}
			/// <summary>Gets or sets the Trigger subject</summary>
			public byte Variable
			{
				get { return _variable; }
				set { _variable = value; }
			}
			/// <summary>Gets or sets the amount required to fire the Trigger</summary>
			public byte Amount
			{
				get { return _amount; }
				set { _amount = value; }
			}
			#endregion ITrigger Members
		}
	}
}
