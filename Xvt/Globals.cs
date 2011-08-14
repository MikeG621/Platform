using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Object for individual Team's mission-wide goals</summary>
	public class Globals : BaseGlobals
	{
		/// <summary>Goal string indexes</summary>
		/// <remarks>0 = Primary1<br>1 = Primary2...<br>4 = Prevent1...<br>8 = Secondary1...<br>11 = Secondary4</remarks>
		public enum GoalIndex:byte { Primary1, Primary2, Primary3, Primary4, Prevent1, Prevent2, Prevent3, Prevent4, Secondary1, Secondary2, Secondary3, Secondary4 };
		/// <summary>Goal status</summary>
		/// <remarks>0 = Incomplete<br>1 = Complete<br>2 = Failed</remarks>
		public enum GoalState:byte { Incomplete, Complete, Failed };

		private sbyte[] _points = new sbyte[3];	// [0]=Prim, [1]=Prev, [2]=Sec
		private string[,] _goalStrings = new string[12, 3];	// [GoalIndex, GoalState] No PreventFailed, SecondaryIncomplete, SecondaryFailed

		/// <summary>Create a new Globals object</summary>
		/// <remarks><i>Triggers</i> contains six triggers of 4 bytes each all set to NONE, <i>AndOr</i>.Length is 9</remarks>
		public Globals()
		{
			_triggers = new byte[12, 4];
			for (int i=0;i<12;i++)
			{
				_triggers[i, 0] = 10;
				for (int j=0;j<3;j++) _goalStrings[i, j] = "";
			}
			for (int i=0;i<9;i++) _andOr[i] = true;
		}

		/// <summary>Gets the number of points awarded / subtracted upon goal completion</summary>
		/// <param name="goal">Goal category, 0-2; Primary, Prevent, Secondary</param>
		/// <exception cref="IndexOutOfRangeException"><i>goal</i> is outside the acceptable range</exception>
		/// <returns>Point total</returns>
		public short GetGoalPoints(int goal) { return (short)(_points[goal] * 250); }

		/// <summary>Sets the points awarded or subtracted for the selected goal</summary>
		/// <param name="goal">Goal category, 0-2; Primary, Prevent, Secondary</param>
		/// <param name="points">Point total, between -32000 and 31750 in multiples of 250 (will be rounded)</param>
		/// <exception cref="ArgumentException"><i>points</i> is outside the acceptable range</exception>
		/// <exception cref="IndexOutOfRangeException"><i>goal</i> is outside the acceptable range</exception>
		public void SetGoalPoints(int goal, short points)
		{
			if (points > 31750 || points < -32000) throw new ArgumentException("Points must be between -32,000 and +31,750");
			_points[goal] = (sbyte)(points / 250);
		}

		/// <summary>Returns the string viewed in the Goals display</summary>
		/// <param name="goalIndex">GoalIndex enumeration value</param>
		/// <param name="goalState">GoalState enumeration value</param>
		/// <returns>Displayed Goal string</returns>
		public string GetGoalString(int goalIndex, int goalState) { return _goalStrings[goalIndex, goalState]; }

		/// <summary>Sets the string viewed in the Goals display</summary>
		/// <param name="goalIndex">GoalIndex enumeration value</param>
		/// <param name="goalState">GoalState enumeration value</param>
		/// <param name="goalString">The string to be displayed, 63 char limit</param>
		public void SetGoalString(int goalIndex, int goalState, string goalString)
		{
			if (goalString.Length > 63) _goalStrings[goalIndex, goalState] = goalString.Substring(0, 63);
			else _goalStrings[goalIndex, goalState] = goalString;
		}
	}
}
