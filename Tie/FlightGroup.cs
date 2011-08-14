using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable]
	public class FlightGroup : BaseFlightGroup
	{
		private string _pilot = "";
		private bool _radio = false;
		private bool _at1AndOrAT2 = false;
		private byte[] _goals = new byte[10];		//condensed FG Goals
		private byte[] _unknowns = new byte[9];
		
		/// <value>Indexes for the Goals array</value>
		/// <remarks>0 = PrimaryCondition<br>1 = PrimaryAmount<br>2 = SecondaryCondition<br>3 = SecondaryAmount<br>4 = SecretCondition<br>5 = SecretAmount<br>6 = BonusCondition<br>7 = BonusAmount<br>8 = BonusPoints<br>9 = Empty</remarks>
		public enum GoalIndexes : byte { PrimaryCondition, PrimaryAmount, SecondaryCondition, SecondaryAmount, SecretCondition, SecretAmount, BonusCondition, BonusAmount, BonusPoints, Empty };
		/// <value>Indexes for the Unknowns array</value>
		/// <remarks>0 = Unk1<br>1 = Unk5<br>2 = Unk9<br>3 = Unk10<br>4 = Unk11<br>5 = Unk12<br>6 = Unk15<br>7 = Unk16<br>8 = Unk17</remarks>
		public enum UnknownIndexes : byte { Unk1, Unk5, Unk9, Unk10, Unk11, Unk12, Unk15, Unk16, Unk17 };

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set Throttle to 100%, all Goals set to FALSE, SP1 Enabled</remarks>
		public FlightGroup()
		{
			_stringLength = 0xC;
			_arrDepTriggers = new byte[3, 4];
			_orders = new byte[3, 18];
			_waypoints = new short[15, 4];
			for (int i=0;i<3;i++)
			{
				_orders[i, 1] = 10;	// 100% throttle
				_orders[i, 10] = 1;	// T3 OR T4
				_orders[i, 16] = 1;	// T1 OR T2
			}
			_goals[0] = 10;
			_goals[2] = 10;
			_goals[4] = 10;
			_goals[6] = 10;
			_waypoints[0, 3] = 1;	// enable SP1
		}

		/// <value>Pilot name, used as short note</value>
		/// <remarks>Restricted to 12 characters</remarks>
		public string Pilot
		{
			get { return _pilot; }
			set
			{
				if (value.Length > _stringLength) _pilot = value.Substring(0, _stringLength);
				else _pilot = value;
			}
		}
		/// <value>If <i>true</i>, FlightGroup responds to player's orders</value>
		public bool Radio
		{
			get { return _radio; }
			set { _radio = value; }
		}
		/// <value>Reference value, if the FlightGroup is created within 30 seconds of mission start</value>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0, 0] == 0 && _arrDepTriggers[1, 0] == 0 && _arrivalDelayMinutes == 0 && _arrivalDelaySeconds <= 30) return true;
				else return false;
			}
		}
		/// <value>Determines if both triggers must be completed</value>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool AT1AndOrAT2
		{
			get { return _at1AndOrAT2; }
			set { _at1AndOrAT2 = value; }
		}
		/// <value>FlightGroup-specific mission goals</value>
		/// <remarks>Array is defined as [GoalIndexes]</remarks>
		public byte[] Goals { get { return _goals; } }
		/// <value>Number of points awarded when Bonus Goal is completed</value>
		/// <remarks>Value must be between -6400 and +6350, will be rounded to multiples of 50</remarks>
		/// <exception cref="ArgumentException">Value is outside the range of accepted values</exception>
		public short BonusGoalPoints
		{
			get { return (short)((sbyte)_goals[8] * 50); }
			set
			{
				if (value > 6350 || value < -6400) throw new ArgumentException("Value must be between -6400 and +6350");
				_goals[8] = Convert.ToByte((value / 50));
			}
		}
		/// <value>Unknown values</value>
		/// <remarks>Array is [UnknownIndexes] (TFW numbering)</remarks>
		public byte[] Unknowns { get { return _unknowns; } }
	}
}
