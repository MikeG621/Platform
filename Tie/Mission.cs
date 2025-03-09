/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2025 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.2
 * 
 * CHANGELOG
 * v7.2, 250309
 * [UPD YOGEME #120] Accounted for message qty overflow
 * v7.0, 241006
 * [NEW] Format spec update
 * [UPD] Briefing events I/O
 * v5.7.5, 230116
 * [DEL #12] CapturedOnEjection
 * [UPD #12] Status reset if out of bounds during load
 * v4.0, 200809
 * [UPD] PermaDeath changed to bool
 * [FIX] Handling to load incomplete briefing questions [JB]
 * [FIX] Better save backup [JB]
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
	/// <summary>Framework for TIE95.</summary>
	/// <remarks>This is the primary container object for a TIE95 mission file.</remarks>
	public partial class Mission : MissionFile
	{
		readonly string[] _endOfMissionMessages = new string[6];    //Mission Comp/Fail messages
		readonly string[] _iff = Strings.IFF;
		readonly bool[] _iffHostile = new bool[6];

		/// <summary>Pre- and Post-mission officers.</summary>
		public enum BriefingOfficers : byte
		{
			/// <summary>No officers presen.t</summary>
			None,
			/// <summary>Both officers are present.</summary>
			Both,
			/// <summary>Only the Flight Officer is present.</summary>
			FlightOfficer,
			/// <summary>Only the Secret Order member is present.</summary>
			SecretOrder
		}

		#region constructors
		/// <summary>Default constructor, creates a blank mission.</summary>
		public Mission()
		{
			initialize();
			for (int i = 0; i < 6; i++) _endOfMissionMessages[i] = "";
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
			OfficersPresent = BriefingOfficers.FlightOfficer;
			_invalidError = _invalidError.Replace("{0}", "TIE");
			_iffHostile[0] = true;
			IFFs = new IffNameIndexer(this);
			IffHostile = new Indexer<bool>(_iffHostile, new bool[] { true, true, false, false, false, false });
			EndOfMissionMessages = new Indexer<string>(_endOfMissionMessages, 63);
			FlightGroups = new FlightGroupCollection();
			Messages = new MessageCollection();
			GlobalGoals = new Globals();
			Briefing = new Briefing();
			BriefingQuestions = new Questions();
		}
		#endregion constructors

		#region public methods
		/// <summary>Loads a mission from a file.</summary>
		/// <param name="filePath">Full path to the file.</param>
		/// <exception cref="FileNotFoundException"><paramref name="filePath"/> does not exist.</exception>
		/// <exception cref="InvalidDataException"><paramref name="filePath"/> is not a TIE mission file.</exception>
		public void LoadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException();
			FileStream fs = File.OpenRead(filePath);
			LoadFromStream(fs);
			fs.Close();
		}

		/// <summary>Loads a mission from an open FileStream.</summary>
		/// <param name="stream">Opened FileStream to mission file.</param>
		/// <exception cref="InvalidDataException"><paramref name="stream"/> is not a valid TIE mission file.</exception>
		public void LoadFromStream(FileStream stream)
		{
			if (GetPlatform(stream) != Platform.TIE) throw new InvalidDataException(_invalidError);
			BinaryReader br = new BinaryReader(stream, System.Text.Encoding.GetEncoding(437));  //[JB] Changed encoding to IBM437 (OEM United States) to properly handle the DOS ASCII character set.
			int i;
			stream.Position = 2;
			short numFlightGroups = br.ReadInt16();
			short numMessages = br.ReadInt16();
			stream.Position += 2;
			TimeLimitMin = br.ReadByte();
			TimeLimitSec = br.ReadByte();
			try { OfficersPresent = (BriefingOfficers)br.ReadByte(); }
			catch { OfficersPresent = BriefingOfficers.Both; }
			RandomSeed = br.ReadByte();
			stream.Position += 12;
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
			for (i = 2; i < 6; i++) IFFs[i] = new string(br.ReadChars(12));
			#region FlightGroups
			FlightGroups = new FlightGroupCollection(numFlightGroups);
			for (i = 0; i < FlightGroups.Count; i++)
			{
				#region Craft
				int j;
				FlightGroups[i].Name = new string(br.ReadChars(12));
				FlightGroups[i].Pilot = new string(br.ReadChars(12));   //not used by TIE
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
				if (FlightGroups[i].Status1 > 9 || (FlightGroups[i].Status1 > 7 && FlightGroups[i].CraftType == 0x70)) FlightGroups[i].Status1 = 0;
				FlightGroups[i].Missile = buffer[5];
				FlightGroups[i].Beam = buffer[6];
				FlightGroups[i].IFF = buffer[7];
				FlightGroups[i].AI = buffer[8];
				FlightGroups[i].Markings = buffer[9];
				FlightGroups[i].FollowsOrders = Convert.ToBoolean(buffer[0xA]);
				FlightGroups[i].Formation = buffer[0xC];
				FlightGroups[i].FormDistance = buffer[0xD];
				FlightGroups[i].GlobalGroup = buffer[0xE];
				FlightGroups[i].NumberOfWaves = (byte)(buffer[0x10] + 1);
				FlightGroups[i].WavesDelay = buffer[0x11];
				FlightGroups[i].PlayerCraft = buffer[0x12];
				FlightGroups[i].Yaw = (short)Math.Round((double)(sbyte)buffer[0x13] * 360 / 0x100);
				FlightGroups[i].Pitch = (short)Math.Round((double)(sbyte)buffer[0x14] * 360 / 0x100);
				FlightGroups[i].Pitch += (short)(FlightGroups[i].Pitch < -90 ? 270 : -90);
				FlightGroups[i].Roll = (short)Math.Round((double)(sbyte)buffer[0x15] * 360 / 0x100);
				FlightGroups[i].PermaDeathEnabled = Convert.ToBoolean(buffer[0x16]);
				FlightGroups[i].PermaDeathID = buffer[0x17];
				#endregion
				#region Arr/Dep
				FlightGroups[i].Difficulty = (BaseFlightGroup.Difficulties)buffer[0x19];
				FlightGroups[i].ArrDepTriggers[0] = new Trigger(buffer, 0x1A);
				FlightGroups[i].ArrDepTriggers[1] = new Trigger(buffer, 0x1E);
				FlightGroups[i].AT1OrAT2 = Convert.ToBoolean(buffer[0x22]);
				FlightGroups[i].ArrivalDelayMinutes = buffer[0x24];
				FlightGroups[i].ArrivalDelaySeconds = buffer[0x25];
				FlightGroups[i].ArrDepTriggers[2] = new Trigger(buffer, 0x26);
				FlightGroups[i].DepartureTimerMinutes = buffer[0x2A];
				FlightGroups[i].DepartureTimerSeconds = buffer[0x2B];
				FlightGroups[i].AbortTrigger = buffer[0x2C];
				FlightGroups[i].ArrivalMothership = buffer[0x30];
				FlightGroups[i].ArriveViaMothership = Convert.ToBoolean(buffer[0x31]);
				FlightGroups[i].DepartureMothership = buffer[0x32];
				FlightGroups[i].DepartViaMothership = Convert.ToBoolean(buffer[0x33]);
				FlightGroups[i].AlternateMothership = buffer[0x34];
				FlightGroups[i].AlternateMothershipUsed = Convert.ToBoolean(buffer[0x35]);
				FlightGroups[i].CapturedDepartureMothership = buffer[0x36];
				FlightGroups[i].CapturedDepartViaMothership = Convert.ToBoolean(buffer[0x37]);
				#endregion
				stream.Read(buffer, 0, 0x36);
				for (j = 0; j < 3; j++) FlightGroups[i].Orders[j] = new FlightGroup.Order(buffer, j * 18);
				stream.Read(buffer, 0, 0xA);
				FlightGroups[i].Goals = new FlightGroup.FGGoals(buffer, 0);
				for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) FlightGroups[i].Waypoints[k][j] = (short)(br.ReadInt16() * (j == 1 ? -1 : 1));
				stream.Position += 4;
			}
			#endregion
			#region Messages
			if (numMessages != 0)
			{
				Messages = new MessageCollection(numMessages);
				for (i = 0; i < Messages.Count; i++)
				{
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
					stream.Read(buffer, 0, 8);
					Messages[i].Triggers[0] = new Trigger(buffer, 0);
					Messages[i].Triggers[1] = new Trigger(buffer, 4);
					Messages[i].Short = new string(br.ReadChars(16));
					Messages[i].RawDelay = br.ReadByte();
					Messages[i].Trig1OrTrig2 = br.ReadBoolean();
				}
				if (numMessages > Messages.Count) stream.Position += (numMessages - Messages.Count) * 0x5a;
			}
			else { Messages.Clear(); }
			#endregion
			#region Globals
			for (i = 0; i < 3; i++)
			{
				stream.Read(buffer, 0, 8);
				GlobalGoals.Goals[i].Triggers[0] = new Trigger(buffer, 0);
				GlobalGoals.Goals[i].Triggers[1] = new Trigger(buffer, 4);
				GlobalGoals.Goals[i].Name = new string(br.ReadChars(16));
				stream.Position++;
				//for some reason, there's triggers with Var set with no Type
				if (GlobalGoals.Goals[i].Triggers[0].VariableType == 0) GlobalGoals.Goals[i].Triggers[0].Variable = 0;
				if (GlobalGoals.Goals[i].Triggers[1].VariableType == 0) GlobalGoals.Goals[i].Triggers[1].Variable = 0;
				GlobalGoals.Goals[i].T1AndOrT2 = br.ReadBoolean();
				stream.Position += 2;
			}
			#endregion
			#region Briefing
			Briefing.Length = br.ReadInt16();
			stream.Position += 6;   // CurrentTime StartLength, EventsLength
			Briefing.Tile = br.ReadInt16();
			byte[] rawEvents = br.ReadBytes(Briefing.EventQuantityLimit * 4);
			Briefing.Events = new BaseBriefing.EventCollection(Platform.TIE, rawEvents);
			for (i = 0; i < 32; i++)
			{
				int j = br.ReadInt16();
				if (j > 0) Briefing.BriefingTag[i] = new string(br.ReadChars(j)).Trim('\0');	// shouldn't need the trim
			}
			for (i = 0; i < 32; i++)
			{
				int j = br.ReadInt16();
				if (j > 0) Briefing.BriefingString[i] = new string(br.ReadChars(j)).Trim('\0');
			}
			#endregion
			#region Questions
			for (i = 0; i < 10; i++)
			{
				int j, k, l = 0;
				j = br.ReadInt16(); //read the question length, we're just not going to save it
				if (j == 1)
				{
					stream.Position++;  //if it's just the return, get rid of it
					continue;
				}
				if (j == 0) continue;
				for (k = 0; k < j; k++)
				{
					BriefingQuestions.PreMissQuestions[i] += br.ReadChar().ToString();
					l++;
					if (stream.ReadByte() == 10) break;
					else stream.Position--;
				}
				l++;
				for (k = l; k < j; k++)
				{
					int b = br.ReadChar(); //[JB] Must honor stream encoding for strings, can't use ReadByte
					switch (b)  //TIE uses char(2) and char(1) for bolding in this section
					{
						case 1:
							BriefingQuestions.PreMissAnswers[i] += "]";
							break;
						case 2:
							BriefingQuestions.PreMissAnswers[i] += "[";
							break;
						case 10:
							BriefingQuestions.PreMissAnswers[i] += "\r\n";  //because txt doesn't like \n by itself
							break;
						default:
							BriefingQuestions.PreMissAnswers[i] += Convert.ToChar(b).ToString();
							break;
					}
				}
			}
			//[JB] I've encountered some custom missions with an incomplete question list. A clever form of protection? Even TFW couldn't open it. Wrapping this in a try/catch block.
			try
			{
				for (i = 0; i < 10; i++)
				{
					int j, k, l = 2;
					j = br.ReadInt16(); //also got rid of saving here, calc'ing on the fly
					if (j == 3)
					{
						stream.Position += 3;   // stupid TFW-isms
						continue;
					}
					if (j == 0) continue;
					BriefingQuestions.PostTrigger[i] = (Questions.QuestionCondition)br.ReadByte();
					BriefingQuestions.PostTrigType[i] = (Questions.QuestionType)br.ReadByte();
					for (k = 0; k < j; k++)
					{
						BriefingQuestions.PostMissQuestions[i] += br.ReadChar().ToString();
						l++;
						if (stream.ReadByte() == 10) break;
						else stream.Position--;
					}
					l++;
					for (k = l; k < j; k++)
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
			}
			catch (EndOfStreamException) { /* Do nothing */ }
			#endregion
			MissionPath = stream.Name;
		}

		/// <summary>Saves the mission to <see cref="MissionFile.MissionPath"/>.</summary>
		/// <exception cref="UnauthorizedAccessException">Write permissions for <see cref="MissionFile.MissionPath"/> are denied.</exception>
		public void Save()
		{
			//[JB] Rewrote the backup logic since it was broken.  It should now retain the same protection concept with more robust handling.
			//First check whether the file exists and is read-only.  Copying to a backup will inherit the read-only property and prevent any attempt to delete, silently breaking the backup feature unless the user directly intervenes to manually delete it.
			if (File.Exists(MissionPath) && (File.GetAttributes(MissionPath) & FileAttributes.ReadOnly) != 0) throw new UnauthorizedAccessException("Cannot save, existing file is read-only.");

			FileStream fs = null;
			//The backup filename must be normalized, as filenames are case-insensitive, unlike strings.  If the replace does not work, the resulting backup name will match the source, and attempting to copy will throw an exception.
			string backup = MissionPath.ToLower().Replace(".tie", "_tie.bak");
			bool backupCreated = false, writerCreated = false;

			if (File.Exists(MissionPath) && MissionPath.ToLower() != backup)
			{
				try
				{
					if (File.Exists(backup)) File.Delete(backup);
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
				long p = 0;
				bw.Write((short)-1);
				bw.Write((short)FlightGroups.Count);
				bw.Write((short)Messages.Count);
				bw.Write((short)3);
				bw.Write(TimeLimitMin);
				bw.Write(TimeLimitSec);
				bw.Write((byte)OfficersPresent);
				bw.Write(RandomSeed);
				fs.Position += 12;
				for (int i = 0; i < 6; i++)
				{
					p = fs.Position;
					if (EndOfMissionMessageColor[i] != 0) bw.Write(Convert.ToByte(EndOfMissionMessageColor[i] + 48));
					bw.Write(_endOfMissionMessages[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x40;
				}
				fs.Position += 2;
				for (int i = 2; i < 6; i++)
				{
					p = fs.Position;
					if (_iffHostile[i]) bw.Write('1');
					bw.Write(_iff[i].ToCharArray()); bw.Write('\0');
					fs.Position = p + 0xC;
				}
				#region Flightgroups
				for (int i = 0; i < FlightGroups.Count; i++)
				{
					p = fs.Position;
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
					if (FlightGroups[i].SpecialCargoCraft == 0) bw.Write(FlightGroups[i].NumberOfCraft);
					else bw.Write((byte)(FlightGroups[i].SpecialCargoCraft - 1));
					bw.Write(FlightGroups[i].RandSpecCargo);
					bw.Write(FlightGroups[i].CraftType);
					bw.Write(FlightGroups[i].NumberOfCraft);
					bw.Write(FlightGroups[i].Status1);
					bw.Write(FlightGroups[i].Missile);
					bw.Write(FlightGroups[i].Beam);
					bw.Write(FlightGroups[i].IFF);
					bw.Write(FlightGroups[i].AI);
					bw.Write(FlightGroups[i].Markings);
					bw.Write(FlightGroups[i].FollowsOrders);
					fs.Position++;
					bw.Write(FlightGroups[i].Formation);
					bw.Write(FlightGroups[i].FormDistance);
					bw.Write(FlightGroups[i].GlobalGroup);
					fs.Position++;
					bw.Write((byte)(FlightGroups[i].NumberOfWaves - 1));
					bw.Write(FlightGroups[i].WavesDelay);
					bw.Write(FlightGroups[i].PlayerCraft);
					bw.Write((byte)(FlightGroups[i].Yaw * 0x100 / 360));
					bw.Write((byte)((FlightGroups[i].Pitch >= 90 ? FlightGroups[i].Pitch - 270 : FlightGroups[i].Pitch + 90) * 0x100 / 360));
					bw.Write((byte)(FlightGroups[i].Roll * 0x100 / 360));
					bw.Write(FlightGroups[i].PermaDeathEnabled);
					bw.Write(FlightGroups[i].PermaDeathID);
					fs.Position++;
					#endregion
					#region Arr/Dep
					bw.Write((byte)FlightGroups[i].Difficulty);
					bw.Write(FlightGroups[i].ArrDepTriggers[0].GetBytes());
					bw.Write(FlightGroups[i].ArrDepTriggers[1].GetBytes());
					bw.Write(FlightGroups[i].AT1OrAT2);
					fs.Position++;
					bw.Write(FlightGroups[i].ArrivalDelayMinutes);
					bw.Write(FlightGroups[i].ArrivalDelaySeconds);
					bw.Write(FlightGroups[i].ArrDepTriggers[2].GetBytes());
					bw.Write(FlightGroups[i].DepartureTimerMinutes);
					bw.Write(FlightGroups[i].DepartureTimerSeconds);
					bw.Write(FlightGroups[i].AbortTrigger);
					fs.Position += 3;
					bw.Write(FlightGroups[i].ArrivalMothership);
					bw.Write(FlightGroups[i].ArriveViaMothership);
					bw.Write(FlightGroups[i].DepartureMothership);
					bw.Write(FlightGroups[i].DepartViaMothership);
					bw.Write(FlightGroups[i].AlternateMothership);
					bw.Write(FlightGroups[i].AlternateMothershipUsed);
					bw.Write(FlightGroups[i].CapturedDepartureMothership);
					bw.Write(FlightGroups[i].CapturedDepartViaMothership);
					#endregion
					for (j = 0; j < 3; j++) bw.Write(FlightGroups[i].Orders[j].GetBytes());
					bw.Write(FlightGroups[i].Goals.GetBytes());
					fs.Position++;
					for (j = 0; j < 4; j++) for (int k = 0; k < 15; k++) bw.Write((short)(FlightGroups[i].Waypoints[k][j] * (j == 1 ? -1 : 1)));
					fs.Position = p + 0x124;
				}
				#endregion
				#region Messages
				for (int i = 0; i < Messages.Count; i++)
				{
					p = fs.Position;
					if (Messages[i].Color != 0) bw.Write((byte)(Messages[i].Color + 0x30));
					bw.Write(Messages[i].MessageString.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x40;
					bw.Write(Messages[i].Triggers[0].GetBytes());
					bw.Write(Messages[i].Triggers[1].GetBytes());
					bw.Write(Messages[i].Short.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x58;
					bw.Write(Messages[i].RawDelay);
					bw.Write(Messages[i].Trig1OrTrig2);
				}
				#endregion
				#region Globals
				for (int i = 0; i < 3; i++)
				{
					p = fs.Position;
					bw.Write(GlobalGoals.Goals[i].Triggers[0].GetBytes());
					bw.Write(GlobalGoals.Goals[i].Triggers[1].GetBytes());
					bw.Write(GlobalGoals.Goals[i].Name.ToCharArray()); bw.Write('\0');
					fs.Position = p + 0x19;
					bw.Write(GlobalGoals.Goals[i].T1AndOrT2);
					fs.Position += 2;
				}
				#endregion
				#region Briefing
				bw.Write(Briefing.Length);
				bw.Write((short)0); // CurrentTime
				bw.Write(Briefing.StartLength);
				bw.Write(Briefing.EventsLength);
				bw.Write(Briefing.Tile);
				byte[] briefBuffer = new byte[Briefing.EventQuantityLimit * 4];
				Buffer.BlockCopy(Briefing.Events.GetArray(), 0, briefBuffer, 0, Briefing.Events.Length * 2);
				bw.Write(briefBuffer);
				for (int i = 0; i < 32; i++)
				{
					string str_t = Briefing.BriefingTag[i].Replace("\r", "");
					int j = Briefing.BriefingTag[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				for (int i = 0; i < 32; i++)
				{
					string str_t = Briefing.BriefingString[i].Replace("\r", "");
					int j = Briefing.BriefingString[i].Length;
					bw.Write((short)j);
					bw.Write(str_t.ToCharArray());
				}
				#endregion
				#region Questions
				for (int i = 0; i < 10; i++)
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
						continue;   //if it doesn't exist, don't even bother with the rest of this
					}
					j++;    //takes into account the q/a spacer
					bw.Write((short)j);
					bw.Write(str_q.ToCharArray());
					fs.WriteByte(0xA);
					bw.Write(str_a.ToCharArray());
				}
				for (int i = 0; i < 10; i++)
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
					bw.Write((short)j);
					bw.Write((byte)BriefingQuestions.PostTrigger[i]);
					bw.Write((byte)BriefingQuestions.PostTrigType[i]);
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

		/// <summary>Checks Trigger.Type/Variable or Order.TargetType/Target pairs for values compatible with TIE.</summary>
		/// <remarks>First checks for invalid Types, then runs through allowed values for each Type. Does not verify FlightGroup, CraftWhen, GlobalGroup or Misc.</remarks>
		/// <param name="type">Trigger.Type or Order.TargetType.</param>
		/// <param name="variable">Trigger.Variable or Order.Target, may be updated.</param>
		/// <param name="errorMessage">Error description if found, otherwise empty.</param>
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
			// can't check GlobalGroup
			// don't want to check Misc
			if (errorMessage != "") errorMessage += " (" + variable + ")";
		}

		/// <summary>Deletes a Flight Group, performing all necessary cleanup to avoid broken indexes.</summary>
		/// <param name="fgIndex">The FG index to delete.</param>
		/// <returns>The index of the next available FlightGroup if successfull, otherwise <b>-1</b>.</returns>
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
		/// <param name="srcIndex">The original index.</param>
		/// <param name="dstIndex">The new index.</param>
		/// <remarks>Automatically performs bounds checking and adjusts all references in the mission to prevent breaking any indexes for triggers, orders, etc.</remarks>
		/// <returns>Returns <b>true</b> if an adjustment was performed, <b>false</b> if index validation failed.</returns>
		public bool SwapFG(int srcIndex, int dstIndex)
		{
			if ((srcIndex < 0 || srcIndex >= FlightGroups.Count) || (dstIndex < 0 || dstIndex >= FlightGroups.Count) || (srcIndex == dstIndex)) return false;

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
		/// <summary>Gets the array accessor for the IFF names.</summary>
		public IffNameIndexer IFFs { get; private set; }
		/// <summary>Gets the array accessor for the IFF behaviour.</summary>
		public Indexer<bool> IffHostile { get; private set; }

		/// <summary>Gets the array of EoM Message colors.</summary>
		public byte[] EndOfMissionMessageColor { get; } = new byte[6];
		/// <summary>Gets the array accessor for the EoM Messages.</summary>
		public Indexer<string> EndOfMissionMessages { get; private set; }

		/// <summary>Maximum number of craft that can exist at one time in-game.</summary>
		public const int CraftLimit = 28;
		/// <summary>Maximum number of FlightGroups that can exist in the mission file.</summary>
		public const int FlightGroupLimit = 48;
		/// <summary>Maximum number of In-Flight Messages that can exist in the mission file.</summary>
		public const int MessageLimit = 16;

		/// <summary>Gets or sets the minutes component of the mission countdown timer.</summary>
		public byte TimeLimitMin { get; set; }
		/// <summary>Gets or sets the seconds component of the mission countdown timer.</summary>
		public byte TimeLimitSec { get; set; }
		/// <summary>Gets or sets the officers present before and after the mission.</summary>
		/// <remarks>Defaults to <b>FlightOfficer</b>.</remarks>
		public BriefingOfficers OfficersPresent { get; set; }
		/// <summary>Gets or sets the intial value used for the random number generator.</summary>
		public byte RandomSeed { get; set; }

		/// <summary>Gets or sets the FlightGroups for the mission.</summary>
		/// <remarks>Defaults to one FlightGroup.</remarks>
		public FlightGroupCollection FlightGroups { get; set; }
		/// <summary>Gets or sets the In-Flight Messages for the mission.</summary>
		/// <remarks>Defaults to zero messages.</remarks>
		public MessageCollection Messages { get; set; }
		/// <summary>Gets or sets the Global Goals for the mission.</summary>
		public Globals GlobalGoals { get; set; }
		/// <summary>Gets or sets the Briefing for the mission.</summary>
		public Briefing Briefing { get; set; }
		/// <summary>Gets or sets the questions for the Briefing Officers.</summary>
		public Questions BriefingQuestions { get; set; }
		#endregion public properties
	}
}