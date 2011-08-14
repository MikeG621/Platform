using System;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual Team's mission-wide goals</summary>
	[Serializable]
	public class Globals : BaseGlobals
	{
		/// <summary>Goal string indexes</summary>
		/// <remarks>0 = Primary1<br>1 = Primary2...<br>4 = Prevent1...<br>8 = Secondary1...<br>11 = Secondary4</remarks>
		public enum GoalIndex : byte { Primary1, Primary2, Primary3, Primary4, Prevent1, Prevent2, Prevent3, Prevent4, Secondary1, Secondary2, Secondary3, Secondary4 };
		/// <summary>Goal status</summary>
		/// <remarks>0 = Incomplete<br>1 = Complete<br>2 = Failed</remarks>
		public enum GoalState : byte { Incomplete, Complete, Failed };

		private sbyte[] _points = new sbyte[3];
		private byte[] _activeSequence = new byte[3];
		private string[,] _goalStrings = new string[12, 3];
		private bool[] _unknown1 = new bool[3];	// 0x11
		private bool[] _unknown2 = new bool[3];	// 0x29
		private byte[] _unknown3 = new byte[3];	// 0x34
		private byte[] _unknown4 = new byte[3];	// 0x36
		private byte[] _unknown5 = new byte[3];	// 0x37
		private byte[] _unknown6 = new byte[3];	// 0x38;

		/// <summary>Create a new Globals object</summary>
		/// <remarks><i>Triggers</i> contains six triggers of 6 bytes each all set to NONE, <i>AndOr</i>.Length is 9</remarks>
		public Globals()
		{
			_triggers = new byte[12, 6];
			for (int i=0;i<12;i++)
			{
				_triggers[i, 0] = 10;
				for (int j=0;j<3;j++) _goalStrings[i, j] = "";
			}
			for (int i=0;i<9;i++) _andOr[i] = true;
		}

		/// <summary>Gets the number of points awarded / subtracted upon goal completion</summary>
		/// <param name="index">Goal category, 0-2; Primary, Prevent, Secondary</param>
		/// <exception cref="IndexOutOfRangeException"><i>goal</i> is outside the acceptable range</exception>
		/// <returns>Point total</returns>
		public short GetGoalPoints(int goal) { return (short)(_points[goal] * 25); }

		/// <summary>Sets the points awarded or subtracted for the selected goal</summary>
		/// <param name="goal">Goal category, 0-2; Primary, Prevent, Secondary</param>
		/// <param name="points">Point total, between -3200 and 3175 in multiples of 25 (will be rounded)</param>
		/// <exception cref="ArgumentException"><i>points</i> is outside the acceptable range</exception>
		/// <exception cref="IndexOutOfRangeException"><i>goal</i> is outside the acceptable range</exception>
		public void SetGoalPoints(int goal, short points)
		{
			if (points > 3175 || points < -3200) throw new ArgumentException("Points must be between -3,200 and +3,175");
			_points[goal] = (sbyte)(points / 25);
		}

		/// <summary>Returns the string viewed in the Goals display</summary>
		/// <param name="goalIndex">GoalIndex enumeration value</param>
		/// <param name="goalState">GoalState enumeration value</param>
		/// <exception cref="IndexOutOfRangeException"><i>goalIndex</i> or <i>goalState</i> are outside the acceptable range</exception>
		/// <returns>Displayed Goal string</returns>
		public string GetGoalString(int goalIndex, int goalState) { return _goalStrings[goalIndex, goalState]; }

		/// <summary>Sets the string viewed in the Goals display</summary>
		/// <param name="goalIndex">GoalIndex enumeration value</param>
		/// <param name="goalState">GoalState enumeration value</param>
		/// <param name="goalString">The string to be displayed, 63 char limit</param>
		/// <exception cref="IndexOutOfRangeException"><i>goalIndex</i> or <i>goalState</i> are outside the acceptable range</exception>
		public void SetGoalString(int goalIndex, int goalState, string goalString)
		{
			if (goalString.Length > 63) _goalStrings[goalIndex, goalState] = goalString.Substring(0, 63);
			else _goalStrings[goalIndex, goalState] = goalString;
		}
		
		/// <summary>Controls Goal location within the mission's Active Sequence</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public byte[] ActiveSequence
		{
			get { return _activeSequence; }
			//set { _activeSequence = value; }
		}
		/// <summary>Unknown value, Goal offset 0x11</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public bool[] Unknown1
		{
			get { return _unknown1; }
			//set { _unknown1 = value; }
		}
		/// <summary>Unknown value, Goal offset 0x29</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public bool[] Unknown2
		{
			get { return _unknown2; }
			//set { _unknown2 = value; }
		}
		/// <summary>Unknown value, Goal offset 0x34</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public byte[] Unknown3
		{
			get { return _unknown3; }
			//set { _unknown3 = value; }
		}
		/// <summary>Unknown value, Goal offset 0x36</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public byte[] Unknown4
		{
			get { return _unknown4; }
			//set { _unknown4 = value; }
		}
		/// <summary>Unknown value, Goal offset 0x37</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public byte[] Unknown5
		{
			get { return _unknown5; }
			//set { _unknown5 = value; }
		}
		/// <summary>Unknown value, Goal offset 0x38</summary>
		/// <remarks>Index is Goal category, 0-2; Primary, Prevent, Secondary</remarks>
		public byte[] Unknown6
		{
			get { return _unknown6; }
			//set { _unknown6 = value; }
		}
	}
}
