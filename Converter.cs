/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in /help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using Idmr.Common;

namespace Idmr.Platform
{
	/// <summary>Object for Mission Platform conversions</summary>
	/// <remarks>Primarily handles downgrading of platforms, due to existing utilities for upgrading<br>
	/// Converted files will use same MissionPath with platform included ("test.tie" to "test_Xvt.tie")</remarks>
	public static class Converter
	{
		/// <summary>Downgrades XvT and BoP missions to TIE95</summary>
		/// <remarks>G/PLT, SHPYD, REPYD and M/SC craft will have their indexes changed to reflect IDMR TIE95 Ships patch numbering. Triggers will update.<br>
		/// FG.Radio is not converted, since TIE behaviour is different<br>
		/// Maximum FG.Formation value of 12 allowed<br>
		/// For Triggers, maximum Trigger index of 24, maximum VariableType of 9, Amounts will be adjusted as 66% to 75%, 33% to 50% and "each" to 100%<br>
		/// Maximum Abort index of 5<br>
		/// Maximum FG.Goal Amount index of 6, 75% converted to 100%, 25% to 50%. First three XvT Goals will be used as Primary, Secondary and Bonus goals. Bonus points will be scaled appropriately. Goals only used if set for Team[0] and Enabled<br>
		/// First two Arrival triggers used, first Departure trigger used. First three Orders used. All standard WPs and first Briefing WP used.<br>
		/// For Messages, first two triggers used.<br>
		/// For the Briefing, entire thing should be able to be used unless the original actually uses close to 200 commands (yikes). There is a conversion on the Zoom factor, this is a legacy factor from my old Converter program, I don't remember why.<br>
		/// Primary Global goals used, XvT Secondary goals converted to Bonus goals. Prevent goals ignored<br>
		/// Team[0] EndOfMissionMessages used, Teams[2-6] Name and Hostility towards Team[0] used for IFF<br>
		/// BriefingQuestions generated using MissionSucc/Fail/Desc strings. Flight Officer has a single pre-mission entry for the Description, two post-mission entries for the Success and Fail. Line breaks must be entered manually<br>
		/// Filename will end in "_TIE.tie"</remarks>
		/// <param name="miss">XvT/BoP mission to convert</param>
		/// <returns>Downgraded mission</returns>
		/// <exception cref="System.ArgumentException">Properties incompatable with TIE95 were detected in <i>miss</i></exception>
		public static Tie.Mission XvtBopToTie(Xvt.Mission miss)
		{
			Tie.Mission tie = new Tie.Mission();
			// FG limit is okay, since XvT < TIE for some reason
			if (miss.NumMessages > Tie.Mission.MessageLimit) throw maxException(true, false, Tie.Mission.MessageLimit);
			tie.FlightGroups = new Tie.FlightGroupCollection(miss.NumFlightGroups);
			if (miss.NumMessages > 0) tie.Messages = new Tie.MessageCollection(miss.NumMessages);
			#region FGs
			for (int i=0; i < tie.NumFlightGroups; i++)
			{
				#region Craft
				// Radio is omitted intentionally
				tie.FlightGroups[i].Name = miss.FlightGroups[i].Name;
				tie.FlightGroups[i].Cargo = miss.FlightGroups[i].Cargo;
				tie.FlightGroups[i].SpecialCargo = miss.FlightGroups[i].SpecialCargo;
				tie.FlightGroups[i].SpecialCargoCraft = miss.FlightGroups[i].SpecialCargoCraft;
				tie.FlightGroups[i].RandSpecCargo = miss.FlightGroups[i].RandSpecCargo;
				tie.FlightGroups[i].CraftType = craftCheck("FG " + i, miss.FlightGroups[i].CraftType, 1);
				tie.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				tie.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				tie.FlightGroups[i].Missile = miss.FlightGroups[i].Missile;
				tie.FlightGroups[i].Beam = miss.FlightGroups[i].Beam;
				tie.FlightGroups[i].IFF = miss.FlightGroups[i].IFF;
				tie.FlightGroups[i].AI = miss.FlightGroups[i].AI;
				tie.FlightGroups[i].Markings = miss.FlightGroups[i].Markings;
				if (miss.FlightGroups[i].Formation > 12) throw flightException(1, i, Xwa.Strings.Formation[miss.FlightGroups[i].Formation]);
				else tie.FlightGroups[i].Formation = miss.FlightGroups[i].Formation;
				tie.FlightGroups[i].FormDistance= miss.FlightGroups[i].FormDistance;
				tie.FlightGroups[i].GlobalGroup = miss.FlightGroups[i].GlobalGroup;
				tie.FlightGroups[i].FormLeaderDist = miss.FlightGroups[i].FormLeaderDist;
				tie.FlightGroups[i].NumberOfWaves = miss.FlightGroups[i].NumberOfWaves;
				tie.FlightGroups[i].PlayerCraft = miss.FlightGroups[i].PlayerCraft;
				tie.FlightGroups[i].Yaw = miss.FlightGroups[i].Yaw;
				tie.FlightGroups[i].Pitch = miss.FlightGroups[i].Pitch;
				tie.FlightGroups[i].Roll = miss.FlightGroups[i].Roll;
				#endregion Craft
				#region ArrDep
				tie.FlightGroups[i].Difficulty = miss.FlightGroups[i].Difficulty;
				for (int j = 0; j < 4; j++)
				{
					tie.FlightGroups[i].ArrDepTriggers[0][j] = miss.FlightGroups[i].ArrDepTriggers[0][j];
					tie.FlightGroups[i].ArrDepTriggers[1][j] = miss.FlightGroups[i].ArrDepTriggers[1][j];
					tie.FlightGroups[i].ArrDepTriggers[2][j] = miss.FlightGroups[i].ArrDepTriggers[4][j];
				}
				tie.FlightGroups[i].AT1AndOrAT2 = miss.FlightGroups[i].ArrDepAO[0];
				triggerCheck("Arrival Trigger 1, FG " + i, tie.FlightGroups[i].ArrDepTriggers[0], 1);
				triggerCheck("Arrival Trigger 2, FG " + i, tie.FlightGroups[i].ArrDepTriggers[1], 1);
				triggerCheck("Departure Trigger, FG " + i, tie.FlightGroups[i].ArrDepTriggers[2], 1);
				tie.FlightGroups[i].ArrivalDelayMinutes = miss.FlightGroups[i].ArrivalDelayMinutes;
				tie.FlightGroups[i].ArrivalDelaySeconds = miss.FlightGroups[i].ArrivalDelaySeconds;
				tie.FlightGroups[i].DepartureTimerMinutes = miss.FlightGroups[i].DepartureTimerMinutes;
				tie.FlightGroups[i].DepartureTimerSeconds = miss.FlightGroups[i].DepartureTimerSeconds;
				if (miss.FlightGroups[i].AbortTrigger > 5) throw flightException(2, i, Xwa.Strings.Abort[miss.FlightGroups[i].AbortTrigger]);
				else tie.FlightGroups[i].AbortTrigger = miss.FlightGroups[i].AbortTrigger;
				tie.FlightGroups[i].ArrivalCraft1 = miss.FlightGroups[i].ArrivalCraft1;
				tie.FlightGroups[i].ArrivalMethod1 = miss.FlightGroups[i].ArrivalMethod1;
				tie.FlightGroups[i].ArrivalCraft2 = miss.FlightGroups[i].ArrivalCraft2;
				tie.FlightGroups[i].ArrivalMethod2 = miss.FlightGroups[i].ArrivalMethod2;
				tie.FlightGroups[i].DepartureCraft1 = miss.FlightGroups[i].DepartureCraft1;
				tie.FlightGroups[i].DepartureMethod1 = miss.FlightGroups[i].DepartureMethod1;
				tie.FlightGroups[i].DepartureCraft2 = miss.FlightGroups[i].DepartureCraft2;
				tie.FlightGroups[i].DepartureMethod2 = miss.FlightGroups[i].DepartureMethod2;
				#endregion ArrDep
				#region Goals
				if ((miss.FlightGroups[i].Goals[0].Enabled == true) && miss.FlightGroups[i].Goals[0].Team == 0)
				{
					tie.FlightGroups[i].Goals.PrimaryCondition = miss.FlightGroups[i].Goals[0].Condition;
					tie.FlightGroups[i].Goals.PrimaryAmount = miss.FlightGroups[i].Goals[0].Amount;
				}
				if ((miss.FlightGroups[i].Goals[1].Enabled == true) && miss.FlightGroups[i].Goals[1].Team == 0)
				{
					tie.FlightGroups[i].Goals.SecondaryCondition = miss.FlightGroups[i].Goals[1].Condition;
					tie.FlightGroups[i].Goals.SecondaryAmount = miss.FlightGroups[i].Goals[1].Amount;
				}
				if ((miss.FlightGroups[i].Goals[2].Enabled == true) && miss.FlightGroups[i].Goals[2].Team == 0)
				{
					tie.FlightGroups[i].Goals.BonusCondition = miss.FlightGroups[i].Goals[2].Condition;
					tie.FlightGroups[i].Goals.BonusAmount = miss.FlightGroups[i].Goals[2].Amount;
					tie.FlightGroups[i].Goals.RawBonusPoints = miss.FlightGroups[i].Goals[2].RawPoints;
				}
				tieGoalsCheck("FlightGroup " + i, tie.FlightGroups[i].Goals);
				#endregion Goals
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 17; k++)
						tie.FlightGroups[i].Orders[j][k] = miss.FlightGroups[i].Orders[j][k];
				for (int j = 0; j < 15; j++)
					for (int k = 0; k < 4; k++)
						tie.FlightGroups[i].Waypoints[j][k] = miss.FlightGroups[i].Waypoints[j][k];
			}
			#endregion FGs
			#region Messages
			for (int i=0; i < tie.NumMessages; i++)
			{
				tie.Messages[i].MessageString = miss.Messages[i].MessageString;
				tie.Messages[i].Color = miss.Messages[i].Color;
				tie.Messages[i].Delay = miss.Messages[i].Delay;
				tie.Messages[i].Short = miss.Messages[i].Note;
				tie.Messages[i].Trig1AndOrTrig2 = miss.Messages[i].T1AndOrT2;
				for (int j=0; j<4; j++)
				{
					tie.Messages[i].Triggers[0][j] = miss.Messages[i].Triggers[0][j];
					tie.Messages[i].Triggers[1][j] = miss.Messages[i].Triggers[1][j];
				}
				triggerCheck("Trigger 1, Message " + i, tie.Messages[i].Triggers[0], 1);
				triggerCheck("Trigger 2, Message " + i, tie.Messages[i].Triggers[1], 1);
			}
			#endregion Messages
			#region Briefing
			for (int i = 0; i < tie.Briefing.BriefingTag.Length; i++) tie.Briefing.BriefingTag[i] = miss.Briefings[0].BriefingTag[i];
			for (int i = 0; i < tie.Briefing.BriefingString.Length; i++) tie.Briefing.BriefingString[i] = miss.Briefings[0].BriefingString[i];
			tie.Briefing.Unknown1 = miss.Briefings[0].Unknown1;
			tie.Briefing.Length = (short)(miss.Briefings[0].Length * Tie.Briefing.TicksPerSecond / Xvt.Briefing.TicksPerSecond);
			byte[] evntVars = { 0, 0, 0, 0, 1, 1, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 4, 4, 4, 4, 4, 4, 4, 4, 3, 2, 3, 2, 1, 0, 0, 0, 0 };
			for (int i = 0; i < tie.Briefing.Events.Length; i += 4)
			{
				short time = BitConverter.ToInt16(miss.Briefings[0].Events, i);
				short evnt = BitConverter.ToInt16(miss.Briefings[0].Events, i+2);
				ArrayFunctions.WriteToArray(evnt, tie.Briefing.Events, i + 2);
				if (time == 9999 && evnt == 0x22)
				{
					ArrayFunctions.WriteToArray(time, tie.Briefing.Events, i);
					break;
				}
				ArrayFunctions.WriteToArray((short)(time * Tie.Briefing.TicksPerSecond / Xvt.Briefing.TicksPerSecond), tie.Briefing.Events, i);
				if (evntVars[evnt] != 0) Buffer.BlockCopy(miss.Briefings[0].Events, i + 4, tie.Briefing.Events, i + 4, 2 * evntVars[evnt]);
				/*for (int j = 0; j < evntVars[evnt]; j++)
				{
					tie.Briefing.Events[i+4] = miss.Briefings[0].Events[i+4];
					tie.Briefing.Events[i+5] = miss.Briefings[0].Events[i+5];
					// j isn't factored in due to i incrementing
					i += 2;
				}*/
			}
			#endregion Briefing
			#region Globals
			tie.GlobalGoals.Goals[0].T1AndOrT2 = miss.Globals[0].Goals[0].T1AndOrT2;	// Primary
			tie.GlobalGoals.Goals[2].T1AndOrT2 = miss.Globals[0].Goals[2].T1AndOrT2;	// Secondary to Bonus, Prevent will be ignored
			for (int i=0; i<4; i++)
			{
				// only take first 2 triggers for Primary and Bonus
				tie.GlobalGoals.Goals[0].Triggers[0][i] = miss.Globals[0].Goals[0].Triggers[0][i];
				tie.GlobalGoals.Goals[0].Triggers[1][i] = miss.Globals[0].Goals[0].Triggers[1][i];
				tie.GlobalGoals.Goals[2].Triggers[0][i] = miss.Globals[0].Goals[2].Triggers[0][i];
				tie.GlobalGoals.Goals[2].Triggers[1][i] = miss.Globals[0].Goals[2].Triggers[1][i];
			}
			triggerCheck("Primary Goal Trigger 1", tie.GlobalGoals.Goals[0].Triggers[0], 1);
			triggerCheck("Primary Goal Trigger 2", tie.GlobalGoals.Goals[0].Triggers[1], 1);
			triggerCheck("Bonus Goal Trigger 1", tie.GlobalGoals.Goals[0].Triggers[0], 1);
			triggerCheck("Bonus Goal Trigger 2", tie.GlobalGoals.Goals[0].Triggers[1], 1);
			#endregion Globals
			#region IFF/Team
			for (int i = 0; i < 6; i++) tie.EndOfMissionMessages[i] = miss.Teams[0].EndOfMissionMessages[i];
			for (int i = 2; i < 6; i++)
			{
				tie.IFFs[i] = miss.Teams[i].Name;
				tie.IffHostile[i] = !miss.Teams[0].AlliedWithTeam[i];
			}
			#endregion IFF/Team
			#region Questions
			if (miss.MissionDescription != "")
			{
				tie.BriefingQuestions.PreMissQuestions[0] = "What are the mission objectives?";
				tie.BriefingQuestions.PreMissAnswers[0] = miss.MissionDescription;	// line breaks will have to be manually placed
			}
			if (miss.MissionSuccessful != "")
			{
				tie.BriefingQuestions.PostMissQuestions[0] = "What have I accomplished?";
				tie.BriefingQuestions.PostMissAnswers[0] = miss.MissionSuccessful;	// again, line breaks
				tie.BriefingQuestions.PostTrigger[0] = 4;
				tie.BriefingQuestions.PostTrigType[0] = 1;
			}
			if (miss.MissionFailed != "")
			{
				tie.BriefingQuestions.PostMissQuestions[1] = "Any suggestions?";
				tie.BriefingQuestions.PostMissAnswers[1] = miss.MissionFailed;	// again, line breaks
				tie.BriefingQuestions.PostTrigger[1] = 5;
				tie.BriefingQuestions.PostTrigType[1] = 1;
			}
			#endregion Questions
			tie.MissionPath = miss.MissionPath.ToUpper().Replace(".TIE", "_TIE.tie");
			return tie;
		}
		
		/// <summary>Downgrades XWA missions to XvT and BoP</summary>
		/// <remarks>Maximum CraftType of 91. Triggers will update.<br>
		/// For Triggers, maximum Trigger index of 46, maximum VariableType of 23, Amounts will be adjusted as "each special" to "100% special"<br>
		/// Only Start and Hyp WPs converted, manual placement for WP1-8 required.<br>
		/// For the Briefing, first 32 strings and text tags are copied, events are ignored (due to using icons instead of Craft)<br>
		/// Filename will end in "_XvT.tie" or "_.BoP.tie"</remarks>
		/// <param name="miss">XWA mission to convert</param>
		/// <param name="bop">Determines if mission is to be converted to BoP instead of XvT</param>
		/// <returns>Downgraded mission</returns>
		/// <exception cref="System.ArgumentException">Properties incompatable with XvT/BoP were detected in <i>miss</i></exception>
		public static Xvt.Mission XwaToXvtBop(Xwa.Mission miss, bool bop)
		{
			Xvt.Mission xvt = new Xvt.Mission();
			xvt.BoP = bop;
			if (miss.NumFlightGroups > Xvt.Mission.FlightGroupLimit) throw maxException(false, true, Xvt.Mission.FlightGroupLimit);
			if (miss.NumMessages > Xvt.Mission.MessageLimit) throw maxException(false, false, Xvt.Mission.MessageLimit);
			xvt.FlightGroups = new Xvt.FlightGroupCollection(miss.NumFlightGroups);
			if (miss.NumMessages > 0) xvt.Messages = new Xvt.MessageCollection(miss.NumMessages);
			xvt.MissionDescription = miss.MissionDescription;
			xvt.MissionFailed = miss.MissionFailed;
			xvt.MissionSuccessful = miss.MissionSuccessful;
			#region FGs
			for (int i = 0; i < xvt.NumFlightGroups; i++)
			{
				#region craft
				xvt.FlightGroups[i].Name = miss.FlightGroups[i].Name;
				xvt.FlightGroups[i].Cargo = miss.FlightGroups[i].Cargo;
				xvt.FlightGroups[i].SpecialCargo = miss.FlightGroups[i].SpecialCargo;
				xvt.FlightGroups[i].SpecialCargoCraft = miss.FlightGroups[i].SpecialCargoCraft;
				xvt.FlightGroups[i].RandSpecCargo = miss.FlightGroups[i].RandSpecCargo;
				xvt.FlightGroups[i].CraftType = craftCheck("FG " + i, miss.FlightGroups[i].CraftType, 2);
				xvt.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				if (xvt.FlightGroups[i].Status1 > 21) throw flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status1]);
				xvt.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				if (xvt.FlightGroups[i].Status2 > 21) throw flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status2]);
				xvt.FlightGroups[i].Status2 = miss.FlightGroups[i].Status2;
				xvt.FlightGroups[i].Missile = miss.FlightGroups[i].Missile;
				xvt.FlightGroups[i].Beam = miss.FlightGroups[i].Beam;
				xvt.FlightGroups[i].IFF = miss.FlightGroups[i].IFF;
				xvt.FlightGroups[i].Team = miss.FlightGroups[i].Team;
				xvt.FlightGroups[i].AI = miss.FlightGroups[i].AI;
				xvt.FlightGroups[i].Markings = miss.FlightGroups[i].Markings;
				xvt.FlightGroups[i].Radio = miss.FlightGroups[i].Radio;
				xvt.FlightGroups[i].Formation = miss.FlightGroups[i].Formation;
				xvt.FlightGroups[i].FormDistance= miss.FlightGroups[i].FormDistance;
				xvt.FlightGroups[i].GlobalGroup = miss.FlightGroups[i].GlobalGroup;
				xvt.FlightGroups[i].FormLeaderDist = miss.FlightGroups[i].FormLeaderDist;
				xvt.FlightGroups[i].NumberOfWaves = miss.FlightGroups[i].NumberOfWaves;
				xvt.FlightGroups[i].Unknowns.Unknown1 = miss.FlightGroups[i].Unknowns.Unknown3;
				xvt.FlightGroups[i].PlayerNumber = miss.FlightGroups[i].PlayerNumber;
				xvt.FlightGroups[i].ArriveOnlyIfHuman = miss.FlightGroups[i].ArriveOnlyIfHuman;
				xvt.FlightGroups[i].PlayerCraft = miss.FlightGroups[i].PlayerCraft;
				xvt.FlightGroups[i].Yaw = miss.FlightGroups[i].Yaw;
				xvt.FlightGroups[i].Pitch = miss.FlightGroups[i].Pitch;
				xvt.FlightGroups[i].Roll = miss.FlightGroups[i].Roll;
				#endregion craft
				#region ArrDep
				xvt.FlightGroups[i].Difficulty = miss.FlightGroups[i].Difficulty;
				for (int j = 0; j < 6; j++)
				{
					for (int k=0;k<4;k++) xvt.FlightGroups[i].ArrDepTriggers[j][k] = miss.FlightGroups[i].ArrDepTriggers[j][k];
					triggerCheck("AD Trigger " + j + ", FG " + i, xvt.FlightGroups[i].ArrDepTriggers[j], 2);
				}
				for (int j=0; j<4; j++) xvt.FlightGroups[i].ArrDepAO[j] = miss.FlightGroups[i].ArrDepAndOr[j];
				xvt.FlightGroups[i].ArrivalDelayMinutes = miss.FlightGroups[i].ArrivalDelayMinutes;
				xvt.FlightGroups[i].ArrivalDelaySeconds = miss.FlightGroups[i].ArrivalDelaySeconds;
				xvt.FlightGroups[i].DepartureTimerMinutes = miss.FlightGroups[i].DepartureTimerMinutes;
				xvt.FlightGroups[i].DepartureTimerSeconds = miss.FlightGroups[i].DepartureTimerSeconds;
				xvt.FlightGroups[i].AbortTrigger = miss.FlightGroups[i].AbortTrigger;
				xvt.FlightGroups[i].ArrivalCraft1 = miss.FlightGroups[i].ArrivalCraft1;
				xvt.FlightGroups[i].ArrivalMethod1 = miss.FlightGroups[i].ArrivalMethod1;
				xvt.FlightGroups[i].ArrivalCraft2 = miss.FlightGroups[i].ArrivalCraft2;
				xvt.FlightGroups[i].ArrivalMethod2 = miss.FlightGroups[i].ArrivalMethod2;
				xvt.FlightGroups[i].DepartureCraft1 = miss.FlightGroups[i].DepartureCraft1;
				xvt.FlightGroups[i].DepartureMethod1 = miss.FlightGroups[i].DepartureMethod1;
				xvt.FlightGroups[i].DepartureCraft2 = miss.FlightGroups[i].DepartureCraft2;
				xvt.FlightGroups[i].DepartureMethod2 = miss.FlightGroups[i].DepartureMethod2;
				#endregion ArrDep
				#region Goals
				for (int j=0; j<8; j++)
				{
					for (int k = 0; k < 6; k++) xvt.FlightGroups[i].Goals[j][k] = miss.FlightGroups[i].Goals[j][k];
					if (xvt.FlightGroups[i].Goals[j].Condition > 46)
						throw triggerException(0, "FG " + i + " Goal " + j, Xwa.Strings.Trigger[xvt.FlightGroups[i].Goals[j].Condition]);
					if (xvt.FlightGroups[i].Goals[j].Amount == 19) xvt.FlightGroups[i].Goals[j].Amount = 6;
					xvt.FlightGroups[i].Goals[j].IncompleteText = miss.FlightGroups[i].Goals[j].IncompleteText;
					xvt.FlightGroups[i].Goals[j].CompleteText = miss.FlightGroups[i].Goals[j].CompleteText;
					xvt.FlightGroups[i].Goals[j].FailedText = miss.FlightGroups[i].Goals[j].FailedText;
				}
				#endregion Goals
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 18; k++)
						xvt.FlightGroups[i].Orders[j][k] = miss.FlightGroups[i].Orders[0, j][k];
					if (xvt.FlightGroups[i].Orders[j].Command > 39) throw flightException(3, i, Xwa.Strings.Orders[xvt.FlightGroups[i].Orders[j].Command]);
					xvt.FlightGroups[i].SkipToOrder4Trigger[0][j] = miss.FlightGroups[i].Orders[0, 3].SkipTriggers[0][j];
					xvt.FlightGroups[i].SkipToOrder4Trigger[1][j] = miss.FlightGroups[i].Orders[0, 3].SkipTriggers[1][j];
				}
				xvt.FlightGroups[i].SkipToO4T1AndOrT2 = miss.FlightGroups[i].Orders[0, 3].SkipT1AndOrT2;
				triggerCheck("FG " + i + ", Skip Order 0", xvt.FlightGroups[i].SkipToOrder4Trigger[0], 2);
				triggerCheck("FG " + i + ", Skip Order 1", xvt.FlightGroups[i].SkipToOrder4Trigger[1], 2);
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 4; k++)
						xvt.FlightGroups[i].Waypoints[j][k] = miss.FlightGroups[i].Waypoints[j][k];
				for (int k = 0; k < 4; k++)
					xvt.FlightGroups[i].Waypoints[13][k] = miss.FlightGroups[i].Waypoints[3][k];
			}
			#endregion FGs
			#region Messages
			for (int i = 0; i < xvt.NumMessages; i++)
			{
				xvt.Messages[i].MessageString = miss.Messages[i].MessageString;
				xvt.Messages[i].Color = miss.Messages[i].Color;
				xvt.Messages[i].Delay = (byte)((miss.Messages[i].DelaySeconds + miss.Messages[i].DelayMinutes * 60) / 5);	// should throw if delay > 21:15
				xvt.Messages[i].Note = miss.Messages[i].Note;
				xvt.Messages[i].T1AndOrT2 = miss.Messages[i].TrigAndOr[0];
				xvt.Messages[i].T3AndOrT4 = miss.Messages[i].TrigAndOr[1];
				xvt.Messages[i].T12AndOrT34 = miss.Messages[i].TrigAndOr[2];
				for (int j = 0; j < 10; j++) xvt.Messages[i].SentToTeam[j] = miss.Messages[i].SentTo[j];
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 4; k++) xvt.Messages[i].Triggers[j][k] = miss.Messages[i].Triggers[j][k];
					triggerCheck("Trig " + j + ", Message " + i, xvt.Messages[i].Triggers[j], 2);
				}
			}
			#endregion Messages
			#region Briefing
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < xvt.Briefings[i].BriefingTag.Length; j++) xvt.Briefings[i].BriefingTag[j] = miss.Briefings[i].BriefingTag[j];
				for (int j = 0; j < xvt.Briefings[i].BriefingString.Length; j++) xvt.Briefings[i].BriefingString[j] = miss.Briefings[i].BriefingString[j];
				xvt.Briefings[i].Unknown1 = miss.Briefings[i].Unknown1;
				xvt.Briefings[i].Length = (short)(miss.Briefings[i].Length * Xvt.Briefing.TicksPerSecond / Xwa.Briefing.TicksPerSecond);
			}
			#endregion Briefing
			#region Globals
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					xvt.Globals[i].Goals[j].T1AndOrT2 = miss.Globals[i].Goals[j].T1AndOrT2;
					xvt.Globals[i].Goals[j].T3AndOrT4 = miss.Globals[i].Goals[j].T3AndOrT4;
					xvt.Globals[i].Goals[j].T12AndOrT34 = miss.Globals[i].Goals[j].T12AndOrT34;
					//string[,] s = miss.Globals[i].Goals[j].GoalStrings;
					for (int k = 0; k < 12; k++) xvt.Globals[i].Goals[j].GoalStrings[k / 3, k % 3] = miss.Globals[i].Goals[j].GoalStrings[k / 3, k % 3];
					xvt.Globals[i].Goals[j].RawPoints = miss.Globals[i].Goals[j].RawPoints;
					for (int h = 0; h < 4; h++)
					{
						for (int k = 0; k < 4; k++) xvt.Globals[i].Goals[j].Triggers[h][k] = miss.Globals[i].Goals[j].Triggers[h][k];
						triggerCheck("Global Goal " + i + " Trig " + j, xvt.Globals[i].Goals[j].Triggers[h], 2);
					}
				}
			}
			#endregion Globals
			#region Team
			for (int i = 0; i < 10; i++)
			{
				xvt.Teams[i].Name = miss.Teams[i].Name;
				for (int j = 0; j < 6; j++)
					xvt.Teams[i].EndOfMissionMessages[j] = miss.Teams[i].EndOfMissionMessages[j];
				for (int j = 0; j < 10; j++)
					xvt.Teams[i].AlliedWithTeam[j] = (miss.Teams[i].Allies[j] == Xwa.Team.Allegeance.Friendly);
			}
			#endregion Team
			xvt.MissionPath = miss.MissionPath.ToUpper().Replace(".TIE", "_XVT.tie");
			return xvt;
		}

		/// <summary>Downgrades XWA missions to TIE95</summary>
		/// <remarks>G/PLT, SHPYD, REPYD and M/SC craft will have their indexes changed to reflect IDMR TIE95 Ships patch numbering. Triggers will update.<br>
		/// FG.Radio is not converted, since TIE behaviour is different<br>
		/// Maximum FG.Formation value of 12 allowed<br>
		/// For Triggers, maximum Trigger index of 24, maximum VariableType of 9, Amounts will be adjusted as 66% to 75%, 33% to 50% and "each" to 100%<br>
		/// Maximum Abort index of 5<br>
		/// Maximum FG.Goal Amount index of 6, 75% converted to 100%, 25% to 50%. First three XvT Goals will be used as Primary, Secondary and Bonus goals. Bonus points will be scaled appropriately. Goals only used if set for Team[0] and Enabled<br>
		/// First two Arrival triggers used, first Departure trigger used. First three Region 1 Orders used, max index of 38.<br>
		/// Only Start and Hyp WPs converted, manual placement for WP1-8 required.<br>
		/// For Messages, first two triggers used.<br>
		/// For the Briefing, first 16 strings and text tags are copied, events are ignored (due to using icons instead of Craft)<br>
		/// Primary Global goals used, XWA Secondary goals converted to Bonus goals. Prevent goals ignored<br>
		/// Team[0] EndOfMissionMessages used, Teams[2-6] Name and Hostility towards Team[0] used for IFF<br>
		/// BriefingQuestions generated using MissionSucc/Fail/Desc strings. Flight Officer has a single pre-mission entry for the Description, two post-mission entries for the Success and Fail. Line breaks must be entered manually<br>
		/// Filename will end in "_TIE.tie"</remarks>
		/// <param name="miss">XWA mission to convert</param>
		/// <returns>Downgraded mission</returns>
		/// <exception cref="System.ArgumentException">Properties incompatable with TIE95 were detected in <i>miss</i></exception>
		public static Tie.Mission XwaToTie(Xwa.Mission miss)
		{
			Tie.Mission tie = new Tie.Mission();
			if (miss.NumFlightGroups > Tie.Mission.FlightGroupLimit) throw maxException(true, true, Tie.Mission.FlightGroupLimit);
			if (miss.NumMessages > Tie.Mission.MessageLimit) throw maxException(true, false, Tie.Mission.MessageLimit);
			tie.FlightGroups = new Tie.FlightGroupCollection(miss.NumFlightGroups);
			if (miss.NumMessages > 0) tie.Messages = new Tie.MessageCollection(miss.NumMessages);
			#region FGs
			for (int i=0; i < tie.NumFlightGroups; i++)
			{
				#region Craft
				// Radio is omitted intentionally
				tie.FlightGroups[i].Name = miss.FlightGroups[i].Name;
				tie.FlightGroups[i].Cargo = miss.FlightGroups[i].Cargo;
				tie.FlightGroups[i].SpecialCargo = miss.FlightGroups[i].SpecialCargo;
				tie.FlightGroups[i].SpecialCargoCraft = miss.FlightGroups[i].SpecialCargoCraft;
				tie.FlightGroups[i].RandSpecCargo = miss.FlightGroups[i].RandSpecCargo;
				tie.FlightGroups[i].CraftType = craftCheck("FG " + i, miss.FlightGroups[i].CraftType, 3);
				tie.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				tie.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				if (tie.FlightGroups[i].Status1 > 19) throw flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status1]);
				tie.FlightGroups[i].Missile = miss.FlightGroups[i].Missile;
				tie.FlightGroups[i].Beam = miss.FlightGroups[i].Beam;
				tie.FlightGroups[i].IFF = miss.FlightGroups[i].IFF;
				tie.FlightGroups[i].AI = miss.FlightGroups[i].AI;
				tie.FlightGroups[i].Markings = miss.FlightGroups[i].Markings;
				if (miss.FlightGroups[i].Formation > 12) throw flightException(1, i, Xwa.Strings.Formation[miss.FlightGroups[i].Formation]);
				else tie.FlightGroups[i].Formation = miss.FlightGroups[i].Formation;
				tie.FlightGroups[i].FormDistance= miss.FlightGroups[i].FormDistance;
				tie.FlightGroups[i].GlobalGroup = miss.FlightGroups[i].GlobalGroup;
				tie.FlightGroups[i].FormLeaderDist = miss.FlightGroups[i].FormLeaderDist;
				tie.FlightGroups[i].NumberOfWaves = miss.FlightGroups[i].NumberOfWaves;
				tie.FlightGroups[i].PlayerCraft = miss.FlightGroups[i].PlayerCraft;
				tie.FlightGroups[i].Yaw = miss.FlightGroups[i].Yaw;
				tie.FlightGroups[i].Pitch = miss.FlightGroups[i].Pitch;
				tie.FlightGroups[i].Roll = miss.FlightGroups[i].Roll;
				#endregion Craft
				#region ArrDep
				tie.FlightGroups[i].Difficulty = miss.FlightGroups[i].Difficulty;
				for (int j = 0; j < 4; j++)
				{
					tie.FlightGroups[i].ArrDepTriggers[0][j] = miss.FlightGroups[i].ArrDepTriggers[0][j];
					tie.FlightGroups[i].ArrDepTriggers[1][j] = miss.FlightGroups[i].ArrDepTriggers[1][j];
					tie.FlightGroups[i].ArrDepTriggers[2][j] = miss.FlightGroups[i].ArrDepTriggers[4][j];
				}
				tie.FlightGroups[i].AT1AndOrAT2 = miss.FlightGroups[i].ArrDepAndOr[0];
				triggerCheck("Arrival Trigger 1, FG " + i, tie.FlightGroups[i].ArrDepTriggers[0], 3);
				triggerCheck("Arrival Trigger 2, FG " + i, tie.FlightGroups[i].ArrDepTriggers[1], 3);
				triggerCheck("Departure Trigger, FG " + i, tie.FlightGroups[i].ArrDepTriggers[2], 3);
				tie.FlightGroups[i].ArrivalDelayMinutes = miss.FlightGroups[i].ArrivalDelayMinutes;
				tie.FlightGroups[i].ArrivalDelaySeconds = miss.FlightGroups[i].ArrivalDelaySeconds;
				tie.FlightGroups[i].DepartureTimerMinutes = miss.FlightGroups[i].DepartureTimerMinutes;
				tie.FlightGroups[i].DepartureTimerSeconds = miss.FlightGroups[i].DepartureTimerSeconds;
				if (miss.FlightGroups[i].AbortTrigger > 5) throw flightException(2, i, Xwa.Strings.Abort[miss.FlightGroups[i].AbortTrigger]);
				else tie.FlightGroups[i].AbortTrigger = miss.FlightGroups[i].AbortTrigger;
				tie.FlightGroups[i].ArrivalCraft1 = miss.FlightGroups[i].ArrivalCraft1;
				tie.FlightGroups[i].ArrivalMethod1 = miss.FlightGroups[i].ArrivalMethod1;
				tie.FlightGroups[i].ArrivalCraft2 = miss.FlightGroups[i].ArrivalCraft2;
				tie.FlightGroups[i].ArrivalMethod2 = miss.FlightGroups[i].ArrivalMethod2;
				tie.FlightGroups[i].DepartureCraft1 = miss.FlightGroups[i].DepartureCraft1;
				tie.FlightGroups[i].DepartureMethod1 = miss.FlightGroups[i].DepartureMethod1;
				tie.FlightGroups[i].DepartureCraft2 = miss.FlightGroups[i].DepartureCraft2;
				tie.FlightGroups[i].DepartureMethod2 = miss.FlightGroups[i].DepartureMethod2;
				#endregion ArrDep
				#region Goals
				if ((miss.FlightGroups[i].Goals[0].Enabled) && miss.FlightGroups[i].Goals[0].Team == 0)
				{
					tie.FlightGroups[i].Goals[0] = miss.FlightGroups[i].Goals[0][1];
					tie.FlightGroups[i].Goals[1] = miss.FlightGroups[i].Goals[0][2];
				}
				if ((miss.FlightGroups[i].Goals[1].Enabled) && miss.FlightGroups[i].Goals[1].Team == 0)
				{
					tie.FlightGroups[i].Goals[2] = miss.FlightGroups[i].Goals[1][1];
					tie.FlightGroups[i].Goals[3] = miss.FlightGroups[i].Goals[1][2];
				}
				if ((miss.FlightGroups[i].Goals[2].Enabled) && miss.FlightGroups[i].Goals[2].Team == 0)
				{
					tie.FlightGroups[i].Goals[6] = miss.FlightGroups[i].Goals[2][1];
					tie.FlightGroups[i].Goals[7] = miss.FlightGroups[i].Goals[2][2];
					tie.FlightGroups[i].Goals[8] = miss.FlightGroups[i].Goals[2][3];
				}
				tieGoalsCheck("FlightGroup " + i, tie.FlightGroups[i].Goals);
				#endregion Goals
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 17; k++)
						tie.FlightGroups[i].Orders[j][k] = miss.FlightGroups[i].Orders[0,j][k];
					if (tie.FlightGroups[i].Orders[j].Command > 38) throw flightException(3, i, Xwa.Strings.Orders[tie.FlightGroups[i].Orders[j].Command]);
				}
				for (int j = 0; j < 3; j++)
					for(int k = 0; k < 4; k++)
						tie.FlightGroups[i].Waypoints[j][k] = miss.FlightGroups[i].Waypoints[j][k];
				for (int k = 0; k < 4; k++)
					tie.FlightGroups[i].Waypoints[13][k] = miss.FlightGroups[i].Waypoints[3][k];
			}
			#endregion FGs
			#region Messages
			for (int i=0; i < tie.NumMessages; i++)
			{
				tie.Messages[i].MessageString = miss.Messages[i].MessageString;
				tie.Messages[i].Color = miss.Messages[i].Color;
				tie.Messages[i].Delay = (byte)((miss.Messages[i].DelaySeconds + miss.Messages[i].DelayMinutes * 60) / 5);	// should throw if delay > 21:15
				tie.Messages[i].Short = miss.Messages[i].Note;
				tie.Messages[i].Trig1AndOrTrig2 = miss.Messages[i].TrigAndOr[0];
				for (int j=0; j<4; j++)
				{
					tie.Messages[i].Triggers[0][j] = miss.Messages[i].Triggers[0][j];
					tie.Messages[i].Triggers[1][j] = miss.Messages[i].Triggers[1][j];
				}
				for (int j = 0; j < 2; j++) triggerCheck("Trigger " + j + ", Message " + i, tie.Messages[i].Triggers[j], 3);
			}
			#endregion Messages
			#region Briefing
			for (int i=0; i < tie.Briefing.BriefingTag.Length; i++) tie.Briefing.BriefingTag[i] = miss.Briefings[0].BriefingTag[i];
			for(int i=0; i < tie.Briefing.BriefingString.Length; i++) tie.Briefing.BriefingString[i] = miss.Briefings[0].BriefingString[i];
			tie.Briefing.Unknown1 = miss.Briefings[0].Unknown1;
			tie.Briefing.Length = (short)(miss.Briefings[0].Length * Tie.Briefing.TicksPerSecond / Xwa.Briefing.TicksPerSecond);
			#endregion Briefing
			#region Globals
			tie.GlobalGoals.Goals[0].T1AndOrT2 = miss.Globals[0].Goals[0].T1AndOrT2;	// Primary
			tie.GlobalGoals.Goals[2].T1AndOrT2 = miss.Globals[0].Goals[2].T1AndOrT2;	// Secondary to Bonus, Prevent will be ignored
			for (int i=0; i<4; i++)
			{
				// only take first 2 triggers for Primary and Bonus
				tie.GlobalGoals.Goals[0].Triggers[0][i] = miss.Globals[0].Goals[0].Triggers[0][i];
				tie.GlobalGoals.Goals[0].Triggers[1][i] = miss.Globals[0].Goals[0].Triggers[1][i];
				tie.GlobalGoals.Goals[2].Triggers[0][i] = miss.Globals[0].Goals[2].Triggers[0][i];
				tie.GlobalGoals.Goals[2].Triggers[1][i] = miss.Globals[0].Goals[2].Triggers[1][i];
			}
			triggerCheck("Primary Goal Trigger 1", tie.GlobalGoals.Goals[0].Triggers[0], 3);
			triggerCheck("Primary Goal Trigger 2", tie.GlobalGoals.Goals[0].Triggers[1], 3);
			triggerCheck("Bonus Goal Trigger 1", tie.GlobalGoals.Goals[2].Triggers[0], 3);
			triggerCheck("Bonus Goal Trigger 2", tie.GlobalGoals.Goals[2].Triggers[1], 3);
			#endregion Globals
			#region IFF/Team
			for (int i = 0; i < 6; i++) tie.EndOfMissionMessages[i] = miss.Teams[0].EndOfMissionMessages[i];
			for (int i = 2; i < 6; i++)
			{
				tie.IFFs[i] = miss.Teams[i].Name;
				tie.IffHostile[i] = ((int)miss.Teams[0].Allies[i] == 0);
			}
			#endregion IFF/Team
			#region Questions
			if (miss.MissionDescription != "")
			{
				tie.BriefingQuestions.PreMissQuestions[0] = "What are the mission objectives?";
				tie.BriefingQuestions.PreMissAnswers[0] = miss.MissionDescription;	// line breaks will have to be manually placed
			}
			if (miss.MissionSuccessful != "")
			{
				tie.BriefingQuestions.PostMissQuestions[0] = "What have I accomplished?";
				tie.BriefingQuestions.PostMissAnswers[0] = miss.MissionSuccessful;	// again, line breaks
				tie.BriefingQuestions.PostTrigger[0] = 4;
				tie.BriefingQuestions.PostTrigType[0] = 1;
			}
			if (miss.MissionFailed != "")
			{
				tie.BriefingQuestions.PostMissQuestions[1] = "Any suggestions?";
				tie.BriefingQuestions.PostMissAnswers[1] = miss.MissionFailed;	// again, line breaks
				tie.BriefingQuestions.PostTrigger[1] = 5;
				tie.BriefingQuestions.PostTrigType[1] = 1;
			}
			#endregion Questions
			tie.MissionPath = miss.MissionPath.ToUpper().Replace(".TIE", "_TIE.tie");
			return tie;
		}

		/// <summary>Validates Trigger conversions</summary>
		/// <remarks>Updates CraftType as necessary.<br>
		/// Converts 66% to 75%, 33% to 50%, "each" to 100% when converting to TIE, converts "each special" to "100% special" from XWA</remarks>
		/// <param name="label">Identifier used in error message</param>
		/// <param name="trigger">The single Trigger being checked</param>
		/// <param name="mode">The Conversion method; 1 = XvT-TIE, 2 = XWA-XvT, 3 = XWA-TIE</param>
		/// <exception cref="ArgumentException">Invalid Trigger or TrigType detected</exception>
		static void triggerCheck(string label, ITrigger trigger, byte mode)
		{
			bool toTie = ((mode & 1) == 1);
			if (trigger.Condition > (toTie ? 24 : 46)) throw triggerException(0, label, Xwa.Strings.Trigger[trigger.Condition]);
			if (trigger.VariableType > (toTie ? 9 : 23)) throw triggerException(1, label, Xwa.Strings.VariableType[trigger.VariableType]);
			else if (trigger.VariableType == 2)
			{
				try { trigger.Variable = craftCheck(label, trigger.Variable, mode); }
				catch (ArgumentException) { throw triggerException(2, label, Xwa.Strings.CraftType[trigger.Variable]); }
				/*if (trigger.Variable == 77) trigger.Variable = 31;	// G/PLT
				else if (trigger.Variable == 89) trigger.Variable = 10;	// SHPYD
				else if (trigger.Variable == 90) trigger.Variable = 11;	// REPYD
				else if (trigger.Variable == 91) trigger.Variable = 39;	// M/SC
				else if (trigger.Variable > 91) throw triggerException(2, label, Xwa.Strings.CraftType[trigger.Variable]);*/
			}
			byte a = trigger.Amount;
			if (toTie) trigger.Amount = (byte)(a == 16 ? 1 : (a == 17 ? 2 : (a == 18 ? 0 : a)));	// 66% to 75%, 33% to 50%, "each" to 100%
			if (trigger.Amount == 19) trigger.Amount = 6;	// "each special" to "100% special"
		}
		
		/// <summary>Validates FlightGroup.Goals for TIE</summary>
		/// <remarks>Converts 75% to 100%, 25% to 50%</remarks>
		/// <param name="label">Identifier used in error message</param>
		/// <param name="array">The complete Goal to check</param>
		/// <exception cref="ArgumentException">Invalid Goal.Amount detected</exception>
		static void tieGoalsCheck(string label, Tie.FlightGroup.FGGoals goals)
		{
			for(int i=0; i<8; i += 2)
			{
				if (i == 4) continue;	// Secret goal, not converted
				if (goals[i] > 24) throw triggerException(0, label + " Goal " + i, Xwa.Strings.Trigger[goals[i]]);
				if (goals[i+1] > 6) throw triggerException(0, label + " Goal " + i, Xwa.Strings.Amount[goals[i+1]]);
				else if (goals[i+1] == 1) goals[i+1] = 0;	// 75 to 100
				else if (goals[i+1] > 1) goals[i+1] -= 2;	// 25 to 50, slide everything after
			}
		}
		
		/// <summary>Makes necessary adjustments for CraftType and returns the updated CraftIndex</summary>
		/// <remarks>(Only from XWA) Falcon to CORT, SAT3 to SAT1, Hyp/RDV Buoys to NavBuoy1, C/CN to CN/A, MC80 Liberty to MC80a, VSD II to VSD, ISD II to ISD. (Only to TIE) G/PLT, SHPYD, REPYD and M/SC indexes updated</remarks>
		/// <param name="label">Indentifier used in error messages</param>
		/// <param name="craft">CraftType index</param>
		/// <param name="mode">The Conversion method; 1 = XvT-TIE, 2 = XWA-XvT, 3 = XWA-TIE</param>
		/// <returns>Resultant CraftType index</returns>
		/// <exception cref="ArgumentException">Invalid CraftType detected</exception>
		static byte craftCheck(string label, byte craft, byte mode)
		{
			bool toTie = ((mode & 1) == 1);
			bool fromXwa = ((mode & 2) == 2);
			if (craft == 10 || craft == 11 || craft == 31)
				throw new ArgumentException("Invalid FlightGroup CraftType detected. " + label + ", " + Xwa.Strings.CraftType[craft]);
			else if (fromXwa && craft == 39) return 38;	// Falcon to CORT
			else if (fromXwa && craft == 71) return 69;	// SAT3 to SAT1
			else if (craft == 77 && toTie) return 31;	// G/PLT
			else if (fromXwa && (craft == 84 || craft == 87)) return 82;	// HypBuoy/RDVBuoy to NavBuoy1
			else if (fromXwa && craft == 88) return 59;	// Cargo Canister to CN/A
			else if (craft == 89 && toTie) return 10;	// SHPYD
			else if (craft == 90 && toTie) return 11;	// REPYD
			else if (craft == 91 && toTie) return 39;	// M/SC
			else if (craft == 227) return 48;	// MC80 Liberty to MC80a
			else if (craft == 228) return 51;	// VSD II to VSD
			else if (craft == 229) return 52;	// ISD II to ISD
			else if (craft > 91)
				throw new ArgumentException("Invalid FlightGroup CraftType detected. " + label + ", " + Xwa.Strings.CraftType[craft]);
			else return craft;
		}

		/// <summary>Returns an ArgumentException formatted for MissionLimits based on the inputs</summary>
		/// <param name="toTie"><i>true</i> for TIE95, <i>false</i> for XvT</param>
		/// <param name="mode"><i>true</i> for FlightGroups, <i>false</i> for Messages</param>
		/// <param name="limit">The appropriate Mission Limit value</param>
		static ArgumentException maxException(bool toTie, bool mode, int limit)
		{
			string s = (mode ? "FlightGroups" : "In-Flight Messages");
			return new ArgumentException("Number of " + s + " exceeds " + (toTie ? "TIE95" : "XvT")
				+ " maximum (" + limit + "). Remove " + s + " before converting");
		}
		
		/// <summary>Returns an ArgumentExceptionformatted  for Triggers based on the inputs</summary>
		/// <param name="index">0 for Trigger condition, 1 for Trigger Type, 2 for Trigger Craft Type, 3 for Amount</param>
		/// <param name="label">Trigger indentifier string</param>
		/// <param name="id">String for the invalid value</param>
		static ArgumentException triggerException(byte index, string label, string id)
		{
			return new ArgumentException("Invalid Trigger "
				+ (index == 0 ? "Trigger condition" : (index == 1 ? "VariableType" : (index == 2 ? "Trigger Craft" : " Trigger Amount")))
				+ " detected. " + label + ", "
				+ (index == 0 ? "Trigger" : (index == 1 ? "TrigType" : (index == 2 ? "CraftType" : "Amount")))
				+ ": " + id);
		}
		
		/// <summary>Returns an ArgumentException formatted for FlightGroups based on the inputs</summary>
		/// <param name="mode">0 for Status, 1 for Formation, 2 for Abort, 3 for Order</param>
		/// <param name="index">FG index</param>
		/// <param name="id">String for the invalid value</param>
		static ArgumentException flightException(byte mode, int index, string id)
		{
			return new ArgumentException("Invalid FlightGroup "
			+ (mode == 0 ? "Status" : (mode == 1 ? "Formation" : (mode == 2 ? "Abort condition" : "Order")))
			+ " detected. FG " + index + ", " + (mode == 3 ? "Order: " : "") + id);
		}
	}
}
