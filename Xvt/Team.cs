using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Object for individual Team settings</summary>
	public class Team
	{
		private string _name = "";
		private bool[] _alliedWithTeam = new bool[10];
		private byte[] _endOfMissionMessageColor = new byte[6];
		private string[] _endOfMissionMessages = new string[6];

		/// <summary>Initialize a new team</summary>
		/// <remarks><i>Name</i> initializes according to <i>teamNumber</i>; 0 = Imperial, 1 = Rebel, other = Team #</remarks>
		/// <param name="teamNumber">Team index being initialized</param>
		public Team(int teamNumber)
		{
			if (teamNumber == 0) { _name = "Imperial"; _alliedWithTeam[0] = true; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber+1).ToString();
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
		}

		/// <value>Team name</value>
		/// <remarks>15 character limit</remarks>
		public string Name
		{
			get { return _name; }
			set
			{
				if (value.Length > 0xE) _name = value.Substring(0, 0xE);
				else _name = value;
			}
		}
		/// <value>Controls allegiance with other teams</value>
		/// <remarks>Array is Length = 10</remarks>
		public bool[] AlliedWithTeam
		{
			get { return _alliedWithTeam; }
			set { if (value.Length == 10) _alliedWithTeam = value; }
		}
		/// <value>Defines the color of the specified EOM Message</value>
		/// <remarks>Array is {PrimComp1, PrimComp2, PrimFail1, PrimFail2, SecComp1, SecComp2}</remarks>
		public byte[] EndOfMissionMessageColor { get { return _endOfMissionMessageColor; } }

		/// <summary>Returns the Inflight Messages shown at goal completion</summary>
		/// <param name="index">0=PrimComplete1, 1=PrimComplete2, 2=PrimFailed1... 5=SecComplete2</param>
		/// <returns>Message string</returns>
		public string GetEndOfMissionMessage(int index) { return _endOfMissionMessages[index]; }

		/// <summary>Sets the Inflight Messages shown at goal completion</summary>
		/// <param name="index">0=PrimComplete1, 1=PrimComplete2, 2=PrimFailed1... 5=SecComplete2</param>
		/// <param name="message">Message string, 63 character limit</param>
		public void SetEndOfMissionMessage(int index, string message)
		{
			if (index < 0 || index > 5) return;
			if (message.Length > 63) _endOfMissionMessages[index] = message.Substring(0, 63);
			else _endOfMissionMessages[index] = message;
		}
	}
}
