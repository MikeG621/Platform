using System;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Object for individual Team settings</summary>
	public class Team
	{
		/// <summary>IFF relations to other Teams</summary>
		/// <remarks>0 = Hostile<br/>1 = Friendly<br/>2 = Neutral</remarks>
		public enum Allegeance : byte { Hostile, Friendly, Neutral }
		
		private string _name;
		private string[] _endOfMissionMessages = new string[6];
		private Allegeance[] _allies = new Allegeance[10];
		private byte[] _unknowns = new byte[6];
		private string[] _voiceIDs = new string[3];

		/// <summary>Initialize a new team</summary>
		/// <remarks><i>Name</i> initializes according to <i>teamNumber</i>; 0 = Imperial, 1 = Rebel, other = Team #</remarks>
		/// <param name="teamNumber">Team index being initialized</param>
		public Team(int teamNumber)
		{
			if (teamNumber == 0) { _name = "Imperial"; _allies[0] = Allegeance.Friendly; }
			else if (teamNumber == 1) _name = "Rebel";
			else _name = "Team " + (teamNumber+1).ToString();
			for (int i=0;i<6;i++) _endOfMissionMessages[i] = "";
			for (int i=0;i<3;i++) _voiceIDs[i] = "";
		}
		/// <value>Team name</value>
		/// <remarks>17 character limit</remarks>
		public string Name
		{
			get { return _name; }
			set
			{
				if (value.Length > 0x11) _name = value.Substring(0, 0x11);
				else _name = value;
			}
		}

		/// <summary>Returns the Inflight Messages shown at goal completion</summary>
		/// <param name="index">0=PrimComplete1, 1=PrimComplete2, 2=PrimFailed1... 5=OutstandingComplete2</param>
		/// <returns>Message string</returns>
		public string GetEndOfMissionMessage(int index) { return _endOfMissionMessages[index]; }

		/// <summary>Sets the Inflight Messages shown at goal completion</summary>
		/// <param name="index">0=PrimComplete1, 1=PrimComplete2, 2=PrimFailed1... 5=OutstandingComplete2</param>
		/// <param name="message">Message string, 63 character limit</param>
		public void SetEndOfMissionMessage(int index, string message)
		{
			if (message.Length > 63) _endOfMissionMessages[index] = message.Substring(0, 63);
			else _endOfMissionMessages[index] = message;
		}

		/// <summary>Defines main team interactions, one value for each team</summary>
		/// <remarks>Array is Length = 10</remarks>
		/// <exception cref="ArgumentException">Attempting to set with the wrong array size</exception>
		public Allegeance[] Allies
		{
			get { return _allies; }
			set
			{
				if (value.Length != 10) throw new ArgumentException("Array must have Length of 10");
				_allies = value;
			}
		}
		/// <summary>Unknown values</summary>
		/// <remarks>Array is Length = 6</remarks>
		/// <exception cref="ArgumentException>Attempting to set with the wrong array size</exception>
		public byte[] Unknowns
		{
			get { return _unknowns; }
			set
			{
				if (value.Length != 6) throw new ArgumentException("Array must have Length of 6");
				_unknowns = value;
			}
		}
		/// <summary>Short notes for EndOfMissionMessages</summary>
		/// <remarks>Array is Length = 3, only 20 characters are saved</remarks>
		/// <exception cref="ArgumentException>Attempting to set with the wrong array size</exception>
		public string[] VoiceIDs
		{
			get { return _voiceIDs; }
			set
			{
				if (value.Length != 3) throw new ArgumentException("Array must have Length of 3");
				_voiceIDs = value;
			}
		}
	}
}
