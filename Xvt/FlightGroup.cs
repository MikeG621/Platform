using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Object for individual FlightGroups</summary>
	public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		#region Craft
		private string[] _roles = new string[4];
		private byte _team = 0;
		private byte _radio = 0;
		private byte _playerNumber = 0;
		private bool _human = false;
		#endregion
		private bool[] _arrDepAO = new bool[4];
		private string[] _orderDesignations = new string[4];
		private byte[,] _skipToOrder4Trigger = new byte[2, 4];		// Skip to Order 4
		private bool _skipToO4T1AndOrT2 = false;
		private byte[,] _goals = new byte[8, 15];		//condensed FG Goals
		private string[,] _goalsStrings = new string[8, 3];
		public UnknownValues Unknowns;
		#region Opts
		private byte _countermeasures = 0;
		private byte _explosionTime = 0;
		private byte _status2 = 0;
		private byte _globUnit = 0;
		private bool[] _optLoad = new bool[15];
		private byte[,] _optCraft = new byte[10, 3];
		private byte _optCat = 0;
		#endregion

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set to 100% Throttle, Goals set to NONE, SP1 Enabled, Unknowns 0/false</remarks>
		public FlightGroup()
		{
			_stringLength = 0x14;
			_arrDepTriggers = new byte[6, 4];
			_orders = new byte[4, 19];
			_waypoints = new short[22, 4];
			for (int i=0;i<4;i++)
			{
				_orders[i, 1] = 10;	// 100% throttle
				_orders[i, 10] = 1;	// T3 OR T4
				_orders[i, 16] = 1;	// T1 OR T2
				_roles[i] = "";	// craft roles
				_orderDesignations[i] = "";	// order designation
			}
			for (int i=0;i<8;i++)
				for (int j=0;j<3;j++)
					_goalsStrings[i, j] = "";
			_goals[0, 1] = 10;		// Goal 1 must NONE
			_goals[1, 1] = 10;
			_goals[2, 1] = 10;
			_goals[3, 1] = 10;
			_goals[4, 1] = 10;
			_goals[5, 1] = 10;
			_goals[6, 1] = 10;
			_goals[7, 1] = 10;
			_optLoad[0] = true;
			_optLoad[8] = true;
			_optLoad[12] = true;
			_waypoints[0, 3] = 1;	// enable SP1
			Unknowns.Unknown1 = 0;
			Unknowns.Unknown2 = false;
			Unknowns.Unknown3 = 0;
			Unknowns.Unknown4 = 0;
			Unknowns.Unknown5 = 0;
			Unknowns.Unknown17 = false;
			Unknowns.Unknown18 = false;
			Unknowns.Unknown19 = false;
			Unknowns.Unknown20 = 0;
			Unknowns.Unknown21 = 0;
			Unknowns.Unknown22 = false;
			Unknowns.Unknown23 = false;
			Unknowns.Unknown24 = false;
			Unknowns.Unknown25 = false;
			Unknowns.Unknown26 = false;
			Unknowns.Unknown27 = false;
			Unknowns.Unknown28 = false;
			Unknowns.Unknown29 = false;
		}

		#region craft
		/// <value>Craft role, such as Command Ship or Strike Craft</value>
		/// <remarks>Array is Length = 4. Each string is 4 characters, first char defines team which it applies to, last three start the designation name.<br>This value has been seen as an AI string with unknown results.</remarks>
		public string[] Roles
		{
			get { return _roles; }
			set { if (value.Length == _roles.Length) _roles = value; }
		}
		/// <value>Allegiance value to control goals and IFF behaviour</value>
		public byte Team
		{
			get { return _team; }
			set { _team = value; }
		}
		/// <value>Determines team or player number to which the craft communicates with</value>
		public byte Radio
		{
			get { return _radio; }
			set { _radio = value; }
		}
		/// <value>Defines human or AI pilot</value>
		/// <remarks>Value of zero defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present.</remarks>
		public byte PlayerNumber
		{
			get { return _playerNumber; }
			set { _playerNumber = value; }
		}
		/// <value>Prevents Player craft from being AI-controlled</value>
		/// <remarks.When <i>true</i>, craft with PlayerNumber set will not appear without a human player.</remarks>
		public bool ArriveOnlyIfHuman
		{
			get { return _human; }
			set { _human = value; }
		}
		#endregion
		#region arr/dep
		/// <value>Reference value, if the FlightGroup is created within 30 seconds of mission start</value>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0, 0] == 0 && _arrDepTriggers[1, 0] == 0 && _arrDepTriggers[2, 0] == 0 && _arrDepTriggers[3, 0] == 0 && _arrivalDelayMinutes == 0 && _arrivalDelaySeconds <= 30) return true;
				else return false;
			}
		}
		/// <value>Determines which ArrDep triggers must be completed</value>
		/// <remarks> Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}</remarks>
		public bool[] ArrDepAO { get { return _arrDepAO; } }
		#endregion
		/// <value>Describes orders</value>
		/// <remarks>Array is Length = 4, strings are limited to 16 char</remarks>
		public string[] OrderDesignations
		{
			get { return _orderDesignations; }
			set 
			{
				if (value.Length != _orderDesignations.Length) return;
				for (int i = 0; i < _orderDesignations.Length; i++)
				{
					_orderDesignations[i] = value[i].Trim('\0');
					if (_orderDesignations[i].Length > 16) _orderDesignations[i] = _orderDesignations[i].Substring(0, 16);
				}
			}
		}
		/// <summary>Sets the order description</summary>
		/// <param name="index">Order index, 0-3</param>
		/// <param name="designation">Description, limited to 16 char</param>
		public void SetOrderDesignation(int index, string designation)
		{
			if (index < 0 || index > 3) return;
			_orderDesignations[index] = (designation.Length > 16 ? designation.Substring(0, 16) : designation);
		}
		/// <value>FlightGroup will proceed directly to Order[3] upon firing</value>
		/// <remarks>Array is two standard Triggers</remarks>
		public byte[,] SkipToOrder4Trigger
		{
			get { return _skipToOrder4Trigger; }
			set { _skipToOrder4Trigger = value; }
		}
		/// <value>Determines if both Skip triggers must be completed</value>
		public bool SkipToO4T1AndOrT2
		{
			get { return _skipToO4T1AndOrT2; }
			set { _skipToO4T1AndOrT2 = value; }
		}
		/// <value>FlightGroup-specific mission goals</value>
		/// <remarks>Array is defined as [GoalIndex (0-7), {Argument, Condition, Amount, Points (sbyte), Enabled (bool), Team, Unknown10 (bool), Unknown11 (bool), Unknown12 (bool), Unknown13, Unknown14 (bool), Unknown15 (0), Unknown16}]. Note different types and <i>Unknown15</i>'s value of zero<br><i>GoalIndex</i> is 0-7.<br>Multiply <i>Points</i> by 250 to get actual value</remarks>
		public byte[,] Goals
		{
			get { return _goals; }
			set { _goals = value; }
		}
		/// <value>Gets the points awarded or subtracted for the selected goal</value>
		/// <remarks>Value will be between -32,000 and 31,750 in multiples of 250</remarks>
		/// <param name="index">Flightgroup Goal index, 0-7</param>
		/// <returns>Point total</returns>
		/// <exception cref="IndexOutOfRangeException"><i>index</i> is outside the bounds of the array</exception>
		public short GetGoalPoints(int index) { return (short)((sbyte)Goals[index, 3] * 250); }
		/// <value>Sets the points awarded or subtracted for the selected goal</value>
		/// <param name="index">Flightgroup Goal, 0-7</param>
		/// <param name="points">Point total, between -32000 and 31750 in multiples of 250 (will be rounded)</param>
		/// <exception cref="IndexOutOfRangeException"><i>index</i> is outside the bounds of the array</exception>
		/// <exception cref="ArgumentException"><i>points</i> is outside the acceptable range</exception>
		public void SetGoalPoints(int index, short points)
		{
			if (points > 31750 || points < -32000) throw new ArgumentException("value must be between -32000 and +31750");
			Goals[index, 3] = Convert.ToByte((points / 250));
		}
		/// <value>Custom strings used in place of defaults</value>
		/// <remarks>Array is [GoalIndex (0-7), {Incomplete, Complete, Failed}]. Strings are limited to 64 char, null-termed.</remarks>
		public string[,] GoalStrings
		{
			get { return _goalsStrings; }
			set { _goalsStrings = value; }
		}
		#region Unks and Options
		/// <value>Missile defenses available to the FG</value>
		public byte Countermeasures
		{
			get { return _countermeasures; }
			set { _countermeasures = value; }
		}
		/// <value>Controls duration of death animation</value>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime
		{
			get { return _explosionTime; }
			set { _explosionTime = value; }
		}
		/// <value>Second condition of FlightGroup upon creation</value>
		public byte Status2
		{
			get { return _status2; }
			set { _status2 = value; }
		}
		/// <value>Additional grouping assignment, can share craft numbering</value>
		public byte GlobalUnit
		{
			get { return _globUnit; }
			set { _globUnit = value; }
		}
		/// <value>Controls alternate weapons the player can select</value>
		/// <remarks>Array is Length = 15. Indexes 0-7 are warheads, 8-12 are beams, 13-15 are countermeasures</remarks>
		public bool[] OptLoadout
		{
			get { return _optLoad; }
		}
		/// <value>Controls alternate craft types the player can select</value>
		/// <remarks>Array is [CraftIndex (0-9), {CraftType, NumberOfCraft, NumberOfWaves}]</remarks>
		public byte[,] OptCraft
		{
			get { return _optCraft; }
			set { _optCraft = value; }
		}
		/// <value>Controls alternate craft types the player can select by list</value>
		/// <remarks>Values are {None, All Flyable, All Rebel Flyable, All Imperial Flyable, Custom}</remarks>
		public byte OptCraftCategory
		{
			get { return _optCat; }
			set { _optCat = value; }
		}
		#endregion
		
		/// <summary>Container for unknown values</summary>
		/// <remarks>All values initialize to 0 or <i>false</i></remarks>
		public struct UnknownValues
		{
			// craft
			public byte Unknown1 { get; set; }	// 0x0062
			public bool Unknown2 { get; set; }	// 0x0063
			// arr/dep
			public byte Unknown3 { get; set; }	// 0x0085, ArrDelay30Sec?
			public byte Unknown4 { get; set; }	// 0x0096
			public byte Unknown5 { get; set; }	// 0x0098
			// orders contain Unk6-9
			// goals contain Unk10-16
			// unks and options
			public bool Unknown17 { get; set; }	// 0x0516
			public bool Unknown18 { get; set; }	// 0x0518
			public bool Unknown19 { get; set; }	// 0x0520
			public byte Unknown20 { get; set; }	// 0x0521
			public byte Unknown21 { get; set; }	// 0x0522
			public bool Unknown22 { get; set; }	// 0x0527
			public bool Unknown23 { get; set; }	// 0x0528
			public bool Unknown24 { get; set; }	// 0x0529
			public bool Unknown25 { get; set; }	// 0x052A
			public bool Unknown26 { get; set; }	// 0x052B
			public bool Unknown27 { get; set; }	// 0x052C
			public bool Unknown28 { get; set; }	// 0x052D
			public bool Unknown29 { get; set; }	// 0x052E
		}
	}
}
