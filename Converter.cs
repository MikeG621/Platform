using System;

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
		/// For Triggers, maximum Trigger index of 24, maximum TriggerType of 9, Amounts will be adjusted as 66% to 75%, 33% to 50% and "each" to 100%<br>
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
		/// <exception cref="ArgumentException">Properties incompatable with TIE95 were detected in <i>miss<i></exception>
		public static Tie.Mission XvtBopToTie(Xvt.Mission miss)
		{
			Tie.Mission tie = new Tie.Mission();
			// FG limit is okay, since XvT < TIE for some reason
			if (miss.NumMessages > Tie.Mission.MessageLimit) maxException(true, false, Tie.Mission.MessageLimit);
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
				if (miss.FlightGroups[i].CraftType == 77) tie.FlightGroups[i].CraftType = 31;	// G/PLT
				else if (miss.FlightGroups[i].CraftType == 89) tie.FlightGroups[i].CraftType = 10;	// SHPYD
				else if (miss.FlightGroups[i].CraftType == 90) tie.FlightGroups[i].CraftType = 11;	// REPYD
				else if (miss.FlightGroups[i].CraftType == 91) tie.FlightGroups[i].CraftType = 39;	// M/SC
				else tie.FlightGroups[i].CraftType = miss.FlightGroups[i].CraftType;
				tie.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				tie.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				tie.FlightGroups[i].Missile = miss.FlightGroups[i].Missile;
				tie.FlightGroups[i].Beam = miss.FlightGroups[i].Beam;
				tie.FlightGroups[i].IFF = miss.FlightGroups[i].IFF;
				tie.FlightGroups[i].AI = miss.FlightGroups[i].AI;
				tie.FlightGroups[i].Markings = miss.FlightGroups[i].Markings;
				if (miss.FlightGroups[i].Formation > 12) flightException(1, i, Xwa.Strings.Formation[miss.FlightGroups[i].Formation]);
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
					tie.FlightGroups[i].ArrDepTrigger[0, j] = miss.FlightGroups[i].ArrDepTrigger[0, j];
					tie.FlightGroups[i].ArrDepTrigger[1, j] = miss.FlightGroups[i].ArrDepTrigger[1, j];
					tie.FlightGroups[i].ArrDepTrigger[2, j] = miss.FlightGroups[i].ArrDepTrigger[4, j];
				}
				tie.FlightGroups[i].AT1AndOrAT2 = miss.FlightGroups[i].ArrDepAO[0];
				tieTriggerCheck("Arrival Trigger 1, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 0);
				tieTriggerCheck("Arrival Trigger 2, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 1);
				tieTriggerCheck("Departure Trigger, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 2);
				tie.FlightGroups[i].ArrivalDelayMinutes = miss.FlightGroups[i].ArrivalDelayMinutes;
				tie.FlightGroups[i].ArrivalDelaySeconds = miss.FlightGroups[i].ArrivalDelaySeconds;
				tie.FlightGroups[i].DepartureTimerMinutes = miss.FlightGroups[i].DepartureTimerMinutes;
				tie.FlightGroups[i].DepartureTimerSeconds = miss.FlightGroups[i].DepartureTimerSeconds;
				if (miss.FlightGroups[i].AbortTrigger > 5) flightException(2, i, Xwa.Strings.Abort[miss.FlightGroups[i].AbortTrigger]);
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
				if ((miss.FlightGroups[i].Goals[0,4] == 1) && miss.FlightGroups[i].Goals[0,5] == 0)
				{
					tie.FlightGroups[i].Goals[0] = miss.FlightGroups[i].Goals[0,1];
					tie.FlightGroups[i].Goals[1] = miss.FlightGroups[i].Goals[0,2];
				}
				if ((miss.FlightGroups[i].Goals[1, 4] == 1) && miss.FlightGroups[i].Goals[1, 5] == 0)
				{
					tie.FlightGroups[i].Goals[2] = miss.FlightGroups[i].Goals[1,1];
					tie.FlightGroups[i].Goals[3] = miss.FlightGroups[i].Goals[1,2];
				}
				if ((miss.FlightGroups[i].Goals[2, 4] == 1) && miss.FlightGroups[i].Goals[2, 5] == 0)
				{
					tie.FlightGroups[i].Goals[6] = miss.FlightGroups[i].Goals[2,1];
					tie.FlightGroups[i].Goals[7] = miss.FlightGroups[i].Goals[2,2];
					tie.FlightGroups[i].Goals[8] = miss.FlightGroups[i].Goals[2,3];
				}
				tieGoalsCheck("FlightGroup " + i, tie.FlightGroups[i].Goals);
				#endregion Goals
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 17; k++)
						tie.FlightGroups[i].Orders[j, k] = miss.FlightGroups[i].Orders[j, k];
				for (int j = 0; j < 15; j++)
					for (int k = 0; k < 4; k++)
						tie.FlightGroups[i].Waypoints[j, k] = miss.FlightGroups[i].Waypoints[j, k];
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
					tie.Messages[i].Triggers[0, j] = miss.Messages[i].Triggers[0, j];
					tie.Messages[i].Triggers[1, j] = miss.Messages[i].Triggers[1, j];
				}
				tieTriggerCheck("Trigger 1, Message " + i, tie.Messages[i].Triggers, 0);
				tieTriggerCheck("Trigger 2, Message " + i, tie.Messages[i].Triggers, 1);
			}
			#endregion Messages
			#region Briefing
			for (int i=0; i < tie.Briefing.BriefingTag.Length; i++) tie.Briefing.BriefingTag[i] = miss.Briefings[0].BriefingTag[i];
			for(int i=0; i < tie.Briefing.BriefingString.Length; i++) tie.Briefing.BriefingString[i] = miss.Briefings[0].BriefingString[i];
			tie.Briefing.Unknown1 = miss.Briefings[0].Unknown1;
//			tie.Briefing.StartLength = miss.Briefings[0].StartLength;
			tie.Briefing.Length = (short)(miss.Briefings[0].Length * Tie.Briefing.TicksPerSecond / Xvt.Briefing.TicksPerSecond);
			byte[] evntVars = { 0, 0, 0, 0, 1, 1, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 4, 4, 4, 4, 4, 4, 4, 4, 3, 2, 3, 2, 1, 0, 0, 0, 0 };
			for(int i=0; i < tie.Briefing.Events.Length; i += 4)
			{
				short time = BitConverter.ToInt16(miss.Briefings[0].Events, i);
				short evnt = BitConverter.ToInt16(miss.Briefings[0].Events, i+2);
				shortToBytes(tie.Briefing.Events, i + 2, evnt);
				if (time == 9999 && evnt == 0x22)
				{
					shortToBytes(tie.Briefing.Events, i, time);
					break;
				}
				shortToBytes(tie.Briefing.Events, i, (short)(time * Tie.Briefing.TicksPerSecond / Xvt.Briefing.TicksPerSecond));
				for (int j = 0; j < evntVars[evnt]; j++)
				{
					shortToBytes(tie.Briefing.Events, i + 4, BitConverter.ToInt16(miss.Briefings[0].Events, i + 4));
					// j isn't factored in due to i incrementing
					i += 2;
				}
			}
			#endregion Briefing
			#region Globals
			tie.GlobalGoals.AndOr[0] = miss.Globals[0].AndOr[0];	// Primary
			tie.GlobalGoals.AndOr[2] = miss.Globals[0].AndOr[6];	// Secondary to Bonus, Prevent will be ignored
			for (int i=0; i<4; i++)
			{
				// only take first 2 triggers for Primary and Bonus
				tie.GlobalGoals.Triggers[0, i] = miss.Globals[0].Triggers[0, i];
				tie.GlobalGoals.Triggers[1, i] = miss.Globals[0].Triggers[1, i];
				tie.GlobalGoals.Triggers[4, i] = miss.Globals[0].Triggers[8, i];
				tie.GlobalGoals.Triggers[5, i] = miss.Globals[0].Triggers[9, i];
			}
			tieTriggerCheck("Primary Goal Trigger 1", tie.GlobalGoals.Triggers, 0);
			tieTriggerCheck("Primary Goal Trigger 2", tie.GlobalGoals.Triggers, 1);
			tieTriggerCheck("Bonus Goal Trigger 1", tie.GlobalGoals.Triggers, 4);
			tieTriggerCheck("Bonus Goal Trigger 2", tie.GlobalGoals.Triggers, 5);
			#endregion Globals
			#region IFF/Team
			for (int i = 0; i < 6; i++) tie.SetEndOfMissionMessage(i, miss.Teams[0].GetEndOfMissionMessage(i));
			for (int i = 2; i < 6; i++)
			{
				tie.SetIff(i, miss.Teams[i].Name);
				tie.SetIffHostile(i, !miss.Teams[0].AlliedWithTeam[i]);
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
		/// For Triggers, maximum Trigger index of 46, maximum TriggerType of 23, Amounts will be adjusted as "each special" to "100% special"<br>
		/// Only Start and Hyp WPs converted, manual placement for WP1-8 required.<br>
		/// For the Briefing, first 32 strings and text tags are copied, events are ignored (due to using icons instead of Craft)<br>
		/// Filename will end in "_XvT.tie" or "_.BoP.tie"</remarks>
		/// <param name="miss">XWA mission to convert</param>
		/// <param name="bop">Determines if mission is to be converted to BoP instead of XvT</param>
		/// <returns>Downgraded mission</returns>
		/// <exception cref="ArgumentException">Properties incompatable with XvT/BoP were detected in <i>miss<i></exception>
		public static Xvt.Mission XwaToXvtBop(Xwa.Mission miss, bool bop)
		{
			Xvt.Mission xvt = new Xvt.Mission();
			xvt.BoP = bop;
			if (miss.NumFlightGroups > Xvt.Mission.FlightGroupLimit) maxException(false, true, Xvt.Mission.FlightGroupLimit);
			if (miss.NumMessages > Xvt.Mission.MessageLimit) maxException(false, false, Xvt.Mission.MessageLimit);
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
				xvt.FlightGroups[i].CraftType = xwaCraftCheck("FG " + i, miss.FlightGroups[i].CraftType, false);
				xvt.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				if (xvt.FlightGroups[i].Status1 > 21) flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status1]);
				xvt.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				if (xvt.FlightGroups[i].Status2 > 21) flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status2]);
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
				xvt.FlightGroups[i].Unknowns.Unknown1 = miss.FlightGroups[i].Unknown3;
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
					for (int k=0;k<4;k++) xvt.FlightGroups[i].ArrDepTrigger[j, k] = miss.FlightGroups[i].ArrDepTrigger[j, k];
					if (xvt.FlightGroups[i].ArrDepTrigger[j, 1] == 2) xvt.FlightGroups[i].ArrDepTrigger[j, 2] = xwaCraftCheck("FG " + i + ", AD Trig" + j + " ", xvt.FlightGroups[i].ArrDepTrigger[j, 2], false);
					xvtTriggerCheck("AD Trigger " + j + ", FG " + i, xvt.FlightGroups[i].ArrDepTrigger, j);
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
					for (int k=0; k<6; k++)
						xvt.FlightGroups[i].Goals[j, k] = miss.FlightGroups[i].Goals[j, k];
					if (xvt.FlightGroups[i].Goals[j, 1] > 46)
						triggerException(0, "FG " + i + " Goal " + j, Xwa.Strings.Trigger[xvt.FlightGroups[i].Goals[j, 1]]);
					if (xvt.FlightGroups[i].Goals[j, 2] == 19) xvt.FlightGroups[i].Goals[j, 2] = 6;
				}
				xvt.FlightGroups[i].GoalStrings = miss.FlightGroups[i].GoalStrings;
				#endregion Goals
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 18; k++)
						xvt.FlightGroups[i].Orders[j, k] = miss.FlightGroups[i].Orders[j, k];
					if (xvt.FlightGroups[i].Orders[j, 0] > 39) flightException(3, i, Xwa.Strings.Orders[xvt.FlightGroups[i].Orders[j, 0]]);
					xvt.FlightGroups[i].SkipToOrder4Trigger[0, j] = miss.FlightGroups[i].SkipToOrder[6, j];
					xvt.FlightGroups[i].SkipToOrder4Trigger[1, j] = miss.FlightGroups[i].SkipToOrder[7, j];
				}
				xvt.FlightGroups[i].SkipToO4T1AndOrT2 = miss.FlightGroups[i].SkipAndOr[3];
				xvtTriggerCheck("FG " + i + ", Skip Order 0", xvt.FlightGroups[i].SkipToOrder4Trigger, 0);
				xvtTriggerCheck("FG " + i + ", Skip Order 1", xvt.FlightGroups[i].SkipToOrder4Trigger, 1);
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 4; k++)
						xvt.FlightGroups[i].Waypoints[j, k] = miss.FlightGroups[i].Waypoints[j, k];
				for (int k = 0; k < 4; k++)
						xvt.FlightGroups[i].Waypoints[13, k] = miss.FlightGroups[i].Waypoints[3, k];
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
				xvt.Messages[i].SentToTeam = miss.Messages[i].SentTo;
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 4; k++)
						xvt.Messages[i].Triggers[j, k] = miss.Messages[i].Triggers[j, k];
					if (xvt.Messages[i].Triggers[j, 1] == 2) xvt.Messages[i].Triggers[j, 2] = xwaCraftCheck("Trig " + j + ", Message " + i, xvt.Messages[i].Triggers[j, 2], false);
					xvtTriggerCheck("Trig " + j + ", Message " + i, xvt.Messages[i].Triggers, j);
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
				for (int j = 0; j < 9; j++)
					xvt.Globals[i].AndOr[j] = miss.Globals[i].AndOr[j];
				for (int j = 0; j < 12; j++)
				{
					for (int k = 0; k < 4; k++)
						xvt.Globals[i].Triggers[j, k] = miss.Globals[i].Triggers[j, k];
					if (xvt.Globals[i].Triggers[j, 1] == 2) xvt.Globals[i].Triggers[j, 2] = xwaCraftCheck("Global Goal " + i + " Trig " + j, xvt.Globals[i].Triggers[j, 2], false);
					for (int k = 0; k < 3; k++)
						xvt.Globals[i].SetGoalString(j, k, miss.Globals[i].GetGoalString(j, k));
					xvtTriggerCheck("Global Goal " + i + " Trig " + j, xvt.Globals[i].Triggers, j);
				}
				for (int j = 0; j < 3; j++)
					xvt.Globals[i].SetGoalPoints(j, (short)(miss.Globals[i].GetGoalPoints(j) * 10));
			}
			#endregion Globals
			#region Team
			for (int i = 0; i < 10; i++)
			{
				xvt.Teams[i].Name = miss.Teams[i].Name;
				for (int j = 0; j < 6; j++)
					xvt.Teams[i].SetEndOfMissionMessage(j, miss.Teams[i].GetEndOfMissionMessage(j));
				for (int j = 0; j < 10; j++)
					xvt.Teams[i].AlliedWithTeam[j] = (miss.Teams[i].Allies[j] == Idmr.Platform.Xwa.Team.Allegeance.Friendly);
			}
			#endregion Team
			xvt.MissionPath = miss.MissionPath.ToUpper().Replace(".TIE", "_XVT.tie");
			return xvt;
		}

		/// <summary>Downgrades XWA missions to TIE95</summary>
		/// <remarks>G/PLT, SHPYD, REPYD and M/SC craft will have their indexes changed to reflect IDMR TIE95 Ships patch numbering. Triggers will update.<br>
		/// FG.Radio is not converted, since TIE behaviour is different<br>
		/// Maximum FG.Formation value of 12 allowed<br>
		/// For Triggers, maximum Trigger index of 24, maximum TriggerType of 9, Amounts will be adjusted as 66% to 75%, 33% to 50% and "each" to 100%<br>
		/// Maximum Abort index of 5<br>
		/// Maximum FG.Goal Amount index of 6, 75% converted to 100%, 25% to 50%. First three XvT Goals will be used as Primary, Secondary and Bonus goals. Bonus points will be scaled appropriately. Goals only used if set for Team[0] and Enabled<br>
		/// First two Arrival triggers used, first Departure trigger used. First three Region 1 Orders used, max index of 38. Start WPs and HYP WP used.<br>
		/// Only Start and Hyp WPs converted, manual placement for WP1-8 required.<br>
		/// For Messages, first two triggers used.<br>
		/// For the Briefing, first 16 strings and text tags are copied, events are ignored (due to using icons instead of Craft)<br>
		/// Primary Global goals used, XvT Secondary goals converted to Bonus goals. Prevent goals ignored<br>
		/// Team[0] EndOfMissionMessages used, Teams[2-6] Name and Hostility towards Team[0] used for IFF<br>
		/// BriefingQuestions generated using MissionSucc/Fail/Desc strings. Flight Officer has a single pre-mission entry for the Description, two post-mission entries for the Success and Fail. Line breaks must be entered manually<br>
		/// Filename will end in "_TIE.tie"</remarks>
		/// <param name="miss">XWA mission to convert</param>
		/// <returns>Downgraded mission</returns>
		/// <exception cref="ArgumentException">Properties incompatable with TIE95 were detected in <i>miss<i></exception>
		public static Tie.Mission XwaToTie(Xwa.Mission miss)
		{
			Tie.Mission tie = new Tie.Mission();
			if (miss.NumFlightGroups > Tie.Mission.FlightGroupLimit) maxException(true, true, Tie.Mission.FlightGroupLimit);
			if (miss.NumMessages > Tie.Mission.MessageLimit) maxException(true, false, Tie.Mission.MessageLimit);
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
				tie.FlightGroups[i].CraftType = xwaCraftCheck("FG " + i, miss.FlightGroups[i].CraftType, true);
				tie.FlightGroups[i].NumberOfCraft = miss.FlightGroups[i].NumberOfCraft;
				tie.FlightGroups[i].Status1 = miss.FlightGroups[i].Status1;
				if (tie.FlightGroups[i].Status1 > 19) flightException(0, i, Xwa.Strings.Status[miss.FlightGroups[i].Status1]);
				tie.FlightGroups[i].Missile = miss.FlightGroups[i].Missile;
				tie.FlightGroups[i].Beam = miss.FlightGroups[i].Beam;
				tie.FlightGroups[i].IFF = miss.FlightGroups[i].IFF;
				tie.FlightGroups[i].AI = miss.FlightGroups[i].AI;
				tie.FlightGroups[i].Markings = miss.FlightGroups[i].Markings;
				if (miss.FlightGroups[i].Formation > 12) flightException(1, i, Xwa.Strings.Formation[miss.FlightGroups[i].Formation]);
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
					tie.FlightGroups[i].ArrDepTrigger[0, j] = miss.FlightGroups[i].ArrDepTrigger[0, j];
					tie.FlightGroups[i].ArrDepTrigger[1, j] = miss.FlightGroups[i].ArrDepTrigger[1, j];
					tie.FlightGroups[i].ArrDepTrigger[2, j] = miss.FlightGroups[i].ArrDepTrigger[4, j];
				}
				for (int j=0; j<3; j++) if (tie.FlightGroups[i].ArrDepTrigger[j, 1] == 2)
					tie.FlightGroups[i].ArrDepTrigger[j, 2] = xwaCraftCheck("FG " + i + ", AD Trig " + j, tie.FlightGroups[i].ArrDepTrigger[j, 2], true);
				tie.FlightGroups[i].AT1AndOrAT2 = miss.FlightGroups[i].ArrDepAndOr[0];
				tieTriggerCheck("Arrival Trigger 1, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 0);
				tieTriggerCheck("Arrival Trigger 2, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 1);
				tieTriggerCheck("Departure Trigger, FG " + i, tie.FlightGroups[i].ArrDepTrigger, 2);
				tie.FlightGroups[i].ArrivalDelayMinutes = miss.FlightGroups[i].ArrivalDelayMinutes;
				tie.FlightGroups[i].ArrivalDelaySeconds = miss.FlightGroups[i].ArrivalDelaySeconds;
				tie.FlightGroups[i].DepartureTimerMinutes = miss.FlightGroups[i].DepartureTimerMinutes;
				tie.FlightGroups[i].DepartureTimerSeconds = miss.FlightGroups[i].DepartureTimerSeconds;
				if (miss.FlightGroups[i].AbortTrigger > 5) flightException(2, i, Xwa.Strings.Abort[miss.FlightGroups[i].AbortTrigger]);
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
				if ((miss.FlightGroups[i].Goals[0, 4] == 1) && miss.FlightGroups[i].Goals[0, 5] == 0)
				{
					tie.FlightGroups[i].Goals[0] = miss.FlightGroups[i].Goals[0,1];
					tie.FlightGroups[i].Goals[1] = miss.FlightGroups[i].Goals[0,2];
				}
				if ((miss.FlightGroups[i].Goals[1, 4] == 1) && miss.FlightGroups[i].Goals[1, 5] == 0)
				{
					tie.FlightGroups[i].Goals[2] = miss.FlightGroups[i].Goals[1,1];
					tie.FlightGroups[i].Goals[3] = miss.FlightGroups[i].Goals[1,2];
				}
				if ((miss.FlightGroups[i].Goals[2, 4] == 1) && miss.FlightGroups[i].Goals[2, 5] == 0)
				{
					tie.FlightGroups[i].Goals[6] = miss.FlightGroups[i].Goals[2,1];
					tie.FlightGroups[i].Goals[7] = miss.FlightGroups[i].Goals[2,2];
					tie.FlightGroups[i].Goals[8] = miss.FlightGroups[i].Goals[2,3];
				}
				tieGoalsCheck("FlightGroup " + i, tie.FlightGroups[i].Goals);
				#endregion Goals
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 17; k++)
						tie.FlightGroups[i].Orders[j, k] = miss.FlightGroups[i].Orders[j, k];
					if (tie.FlightGroups[i].Orders[j, 0] > 38) flightException(3, i, Xwa.Strings.Orders[tie.FlightGroups[i].Orders[j, 0]]);
				}
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 4; k++)
						tie.FlightGroups[i].Waypoints[j, k] = miss.FlightGroups[i].Waypoints[j, k];
				for (int k = 0; k < 4; k++)
						tie.FlightGroups[i].Waypoints[13, k] = miss.FlightGroups[i].Waypoints[3, k];
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
					tie.Messages[i].Triggers[0, j] = miss.Messages[i].Triggers[0, j];
					tie.Messages[i].Triggers[1, j] = miss.Messages[i].Triggers[1, j];
				}
				if (tie.Messages[i].Triggers[0, 1] == 2) tie.Messages[i].Triggers[0, 2] = xwaCraftCheck("Trig 1, Message " + i, tie.Messages[i].Triggers[0, 2], true);
				if (tie.Messages[i].Triggers[1, 1] == 2) tie.Messages[i].Triggers[1, 2] = xwaCraftCheck("Trig 2, Message " + i, tie.Messages[i].Triggers[1, 2], true);
				tieTriggerCheck("Trigger 1, Message " + i, tie.Messages[i].Triggers, 0);
				tieTriggerCheck("Trigger 2, Message " + i, tie.Messages[i].Triggers, 1);
			}
			#endregion Messages
			#region Briefing
			for (int i=0; i < tie.Briefing.BriefingTag.Length; i++) tie.Briefing.BriefingTag[i] = miss.Briefings[0].BriefingTag[i];
			for(int i=0; i < tie.Briefing.BriefingString.Length; i++) tie.Briefing.BriefingString[i] = miss.Briefings[0].BriefingString[i];
			tie.Briefing.Unknown1 = miss.Briefings[0].Unknown1;
			tie.Briefing.Length = (short)(miss.Briefings[0].Length * Tie.Briefing.TicksPerSecond / Xwa.Briefing.TicksPerSecond);
			#endregion Briefing
			#region Globals
			tie.GlobalGoals.AndOr[0] = miss.Globals[0].AndOr[0];	// Primary
			tie.GlobalGoals.AndOr[2] = miss.Globals[0].AndOr[6];	// Secondary to Bonus, Prevent will be ignored
			for (int i=0; i<4; i++)
			{
				// only take first 2 triggers for Primary and Bonus
				tie.GlobalGoals.Triggers[0, i] = miss.Globals[0].Triggers[0, i];
				tie.GlobalGoals.Triggers[1, i] = miss.Globals[0].Triggers[1, i];
				tie.GlobalGoals.Triggers[4, i] = miss.Globals[0].Triggers[8, i];
				tie.GlobalGoals.Triggers[5, i] = miss.Globals[0].Triggers[9, i];
			}
			if (tie.GlobalGoals.Triggers[0, 1] == 2) tie.GlobalGoals.Triggers[0, 2] = xwaCraftCheck("Prim Goal Trig 1", tie.GlobalGoals.Triggers[0, 2], true);
			if (tie.GlobalGoals.Triggers[1, 1] == 2) tie.GlobalGoals.Triggers[1, 2] = xwaCraftCheck("Prim Goal Trig 2", tie.GlobalGoals.Triggers[1, 2], true);
			if (tie.GlobalGoals.Triggers[4, 1] == 2) tie.GlobalGoals.Triggers[4, 2] = xwaCraftCheck("Bonus Goal Trig 1", tie.GlobalGoals.Triggers[4, 2], true);
			if (tie.GlobalGoals.Triggers[5, 1] == 2) tie.GlobalGoals.Triggers[5, 2] = xwaCraftCheck("Bonus Goal Trig 2", tie.GlobalGoals.Triggers[5, 2], true);
			tieTriggerCheck("Primary Goal Trigger 1", tie.GlobalGoals.Triggers, 0);
			tieTriggerCheck("Primary Goal Trigger 2", tie.GlobalGoals.Triggers, 1);
			tieTriggerCheck("Bonus Goal Trigger 1", tie.GlobalGoals.Triggers, 4);
			tieTriggerCheck("Bonus Goal Trigger 2", tie.GlobalGoals.Triggers, 5);
			#endregion Globals
			#region IFF/Team
			for (int i = 0; i < 6; i++) tie.SetEndOfMissionMessage(i, miss.Teams[0].GetEndOfMissionMessage(i));
			for (int i = 2; i < 6; i++)
			{
				tie.SetIff(i, miss.Teams[i].Name);
				tie.SetIffHostile(i, ((int)miss.Teams[0].Allies[i] == 0));
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

		/// <summary>Writes two bytes to the array</summary>
		/// <param name="array">The byte array to be written in</param>
		/// <param name="index">The first index of the value</param>
		/// <param name="value">The new short value</param>
		static void shortToBytes(byte[] array, int index, short value)
		{
			array[index] = BitConverter.GetBytes(value)[0];
			array[index + 1] = BitConverter.GetBytes(value)[1];
		}
		
		/// <summary>Validates Trigger conversions for TIE</summary>
		/// <remarks>Converts G/PLT, SHPYD, REPYD, M/SC indexes<br>
		/// Converts 66% to 75%, 33% to 50%, "each" to 100%</remarks>
		/// <param name="label">Identifier used in error message</param>
		/// <param name="array">The complete Trigger array to check</param>
		/// <param name="trigger">The single Trigger index being checked</param>
		/// <exception cref="ArgumentException">Invalid Trigger or TrigType detected</exception>
		static void tieTriggerCheck(string label, byte[,] array, int trigger)
		{
			if (array[trigger, 0] > 24) triggerException(0, label, Xwa.Strings.Trigger[array[trigger, 0]]);
			if (array[trigger, 1] > 9) if (array[trigger, 1] > 23) triggerException(1, label, Xwa.Strings.TriggerType[array[trigger, 1]]);
			else if (array[trigger, 1] == 2)
			{
				if (array[trigger, 2] == 77) array[trigger, 2] = 31;	// G/PLT
				else if (array[trigger, 2] == 89) array[trigger, 2] = 10;	// SHPYD
				else if (array[trigger, 2] == 90) array[trigger, 2] = 11;	// REPYD
				else if (array[trigger, 2] == 91) array[trigger, 2] = 39;	// M/SC
				else if (array[trigger, 2] > 91) if (array[trigger, 2] > 91) triggerException(2, label, Xwa.Strings.CraftType[array[trigger, 2]]);
			}
			if (array[trigger, 3] == 16) array[trigger, 3] = 1;	// 66% to 75%
			else if (array[trigger, 3] == 17) array[trigger, 3] = 2;	// 33% to 50%
			else if (array[trigger, 3] == 18) array[trigger, 3] = 0;	// "each" to 100%
		}

		/// <summary>Validates Trigger conversions for XvT</summary>
		/// <remarks>Converts "each special" Amount values to "special"</remarks>
		/// <param name="label">Identifier used in error message</param>
		/// <param name="array">The complete Trigger array to check</param>
		/// <param name="trigger">The single Trigger index being checked</param>
		/// <exception cref="ArgumentException">Invalid Trigger or TrigType detected</exception>
		static void xvtTriggerCheck(string label, byte[,] array, int trigger)
		{
			if (array[trigger, 0] > 46) triggerException(0, label, Xwa.Strings.Trigger[array[trigger, 0]]);
			if (array[trigger, 1] > 23) triggerException(1, label, Xwa.Strings.TriggerType[array[trigger, 1]]);
			else if (array[trigger, 1] == 2)
				if (array[trigger, 2] > 91) triggerException(2, label, Xwa.Strings.CraftType[array[trigger, 2]]);
			if (array[trigger, 3] == 19) array[trigger, 3] = 6;	// "each special" to "100% special"
		}
		
		/// <summary>Validates FlightGroup.Goals for TIE</summary>
		/// <remarks>Converts 75% to 100%, 25% to 50%</remarks>
		/// <param name="label">Identifier used in error message</param>
		/// <param name="array">The complete Goal array to check</param>
		/// <exception cref="ArgumentException">Invalid Goal.Amount detected</exception>
		static void tieGoalsCheck(string label, byte[] array)
		{
			for(int i=0; i<8; i += 2)
			{
				if (i == 4) continue;	// Secret goal, not converted
				if (array[i] > 24) triggerException(0, label + " Goal " + i, Xwa.Strings.Trigger[array[i]]);
				if (array[i+1] > 6) triggerException(0, label + " Goal " + i, Xwa.Strings.Amount[array[i+1]]);
				else if (array[i+1] == 1) array[i+1] = 0;	// 75 to 100
				else if (array[i+1] > 1) array[i+1] -= 2;	// 25 to 50, slide everything after
			}
		}
		
		/// <summary>Makes necessary adjustments for CraftType from XWA</summary>
		/// <remarks>Falcon to CORT, SAT3 to SAT1, moves G/PLT SHPYD REPYD M/SC (TIE only), Hyp/RDV Buoys to NavBuoy1, C/CN to CN/A, MC80 Liberty to MC80a, VSD II to VSD, ISD II to ISD</remarks>
		/// <param name="label">Indentifier used in error messages</param>
		/// <param name="xwaCraft">CraftType index from XWA</param>
		/// <param name="toTie">When <i>true</i> moves G/PLT, SHPYD, REPYD and M/SC</param>
		/// <returns>Resultant CraftType index</returns>
		/// <exception cref="ArgumentException">Invalid CraftType detected</exception>
		static byte xwaCraftCheck(string label, byte xwaCraft, bool toTie)
		{
			if (xwaCraft == 10 || xwaCraft == 11 || xwaCraft == 31)
				throw new ArgumentException("Invalid FlightGroup CraftType detected. " + label + ", " + Xwa.Strings.CraftType[xwaCraft]);
			else if (xwaCraft == 39) return 38;	// Falcon to CORT
			else if (xwaCraft == 71) return 69;	// SAT3 to SAT1
			else if (xwaCraft == 77 && toTie) return 31;	// G/PLT
			else if (xwaCraft == 84 || xwaCraft == 87) return 82;	// HypBuoy/RDVBuoy to NavBuoy1
			else if (xwaCraft == 88) return 59;	// Cargo Canister to CN/A
			else if (xwaCraft == 89 && toTie) return 10;	// SHPYD
			else if (xwaCraft == 90 && toTie) return 11;	// REPYD
			else if (xwaCraft == 91 && toTie) return 39;	// M/SC
			else if (xwaCraft == 227) return 48;	// MC80 Liberty to MC80a
			else if (xwaCraft == 228) return 51;	// VSD II to VSD
			else if (xwaCraft == 229) return 52;	// ISD II to ISD
			else if (xwaCraft > 91)
				throw new ArgumentException("Invalid FlightGroup CraftType detected. " + label + ", " + Xwa.Strings.CraftType[xwaCraft]);
			else return xwaCraft;
		}

		/// <summary>Throws an ArgumentException for MissionLimits based on the inputs</summary>
		/// <param name="platform"><i>true</i> for TIE95, <i>false</i> for XvT</param>
		/// <param name="mode"><i>true</i> for FlightGroups, <i>false</i> for Messages</param>
		/// <param name="limit">The appropriate Mission Limit value</param>
		static void maxException(bool platform, bool mode, int limit)
		{
			string s = (mode ? "FlightGroups" : "In-Flight Messages");
			throw new ArgumentException("Number of " + s + " exceeds " + (platform ? "TIE95" : "XvT")
				+ " maximum (" + limit + "). Remove " + s + " before converting");
		}
		
		/// <summary>Throws an ArgumentException for Triggers based on the inputs</summary>
		/// <param name="mode">0 for Trigger condition, 1 for Trigger Type, 2 for Trigger Craft Type, 3 for Amount</param>
		/// <param name="label">Trigger indentifier string</param>
		/// <param name="id">String for the invalid value</param>
		static void triggerException(byte mode, string label, string id)
		{
			throw new ArgumentException("Invalid Trigger "
				+ (mode == 0 ? "Trigger condition" : (mode == 1 ? "TriggerType" : (mode == 2 ? "Trigger Craft" : " Trigger Amount")))
				+ " detected. " + label + ", "
				+ (mode == 0 ? "Trigger" : (mode == 1 ? "TrigType" : (mode == 2 ? "CraftType" : "Amount")))
				+ ": " + id);
		}
		
		/// <summary>Throws an ArgumentException for FlightGroups based on the inputs</summary>
		/// <param name="mode">0 for Status, 1 for Formation, 2 for Abort, 3 for Order</param>
		/// <param name="index">FG index</param>
		/// <param name="id">String for the invalid value</param>
		static void flightException(byte mode, int index, string id)
		{
			throw new ArgumentException("Invalid FlightGroup "
			+ (mode == 0 ? "Status" : (mode == 1 ? "Formation" : (mode == 2 ? "Abort condition" : "Order")))
			+ " detected. FG " + index + ", " + (mode == 3 ? "Order: " : "") + id);
		}
	}
}
