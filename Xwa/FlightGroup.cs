using System;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		#region basic
		private bool _enableDesignation1 = false;
		private bool _enableDesignation2 = false;
		private byte _designation1 = 0;
		private byte _designation2 = 0;
		private byte _unknown1 = 2;	// 0x0018
		private byte _globalCargo = 0;	// binary value is 255, 0 used for YOGEME
		private byte _globalSpecialCargo = 0;
		private string _role = "";
		private byte _team = 0;
		private byte _radio = 0;
		private byte _unknown3 = 0;		// 0x007B
		private byte _playerNumber = 0;
		private bool _arriveOnlyIfHuman = false;
		private byte _unknown4 = 0;		// 0x0084
		#endregion
		#region ArrDep
		private byte _unknown5 = 0;		// 0x0087
		private bool[] _arrDepAndOr = new bool[4];
		private bool _unknown6 = false;	// 0x0097
		private byte _unknown7 = 0;	// 0x00BF
		private byte _unknown8 = 0;	// 0x00C0
		#endregion
		#region orders
		// bOrder contains Unk9 @ [,5]
		private string[] _orderStrings = new string[16];
		private byte[] _unknown10 = new byte[16];	// 0x72 from order origin
		private bool[] _unknown11 = new bool[16];	// 0x73 from order origin
		private bool[] _unknown12 = new bool[16];	// 0x74 from order origin
		private bool[] _unknown13 = new bool[16];	// 0x7B from order origin
		private bool[] _unknown14 = new bool[16];	// 0x81 from order origin
		private byte[,] _skipToOrder = new byte[32, 6];		// Skip to Order, [0,]=R1O1T1, [1,]=R1O1T2, [2,]=R1O2T1...
		private bool[] _stOAndOr = new bool[16];
		#endregion
		// goals contain Unk15(bool)
		private byte[,] _goals = new byte[8, 9];		//condensed FG Goals, non-consecutive
		private string[,] _goalStrings = new string[8, 3];
		#region Unks and Option
		private byte _unknown16 = 0;	// 0xDAE
		private byte _unknown17 = 0;	// 0xDAF
		private byte _unknown18 = 0;	// 0xDB0
		private byte _unknown19 = 0;	// 0xDB1
		private byte _unknown20 = 0;	// 0xDB2
		private byte _unknown21 = 0;	// 0xDB3
		private bool _unknown22 = false;	// 0xDB4
		private byte _unknown23 = 0;	// 0xDB6
		private byte _unknown24 = 0;	// 0xDB7
		private byte _unknown25 = 0;	// 0xDB8
		private byte _unknown26 = 0;	// 0xDB9
		private byte _unknown27 = 0;	// 0xDBA
		private byte _unknown28 = 0;	// 0xDBB
		private bool _unknown29 = false;	// 0xDBC
		private bool _unknown30 = false;	// 0xDC0
		private bool _unknown31 = false;	// 0xDC1
		private bool _globalNumbering = false;
		private byte _unknown32 = 0;	// 0xDC5
		private byte _unknown33 = 0;	// 0xDC6
		private byte _countermeasures = 0;
		private byte _explosionTime = 0;
		private byte _status2 = 0;
		private byte _globalUnit = 0;
		private bool[] _optLoadout = new bool[15];	// [0-7]=warheads, [8-11]=beams, [12-14]=CMs
		private byte[,] _optCraft = new byte[10, 3];	// [,0]=craft, [,1]=num, [,2]=waves
		private byte _optCraftCategory = 0;
		private string _pilotID = "";
		private byte _backdrop = 0;
		private bool _unknown34 = false;	// 0xE29
		private bool _unknown35 = false;	// 0xE2B
		private bool _unknown36 = false;	// 0xE2D
		private bool _unknown37 = false;	// 0xE2F
		private bool _unknown38 = false;	// 0xE31
		private bool _unknown39 = false;	// 0xE33
		private bool _unknown40 = false;	// 0xE35
		private bool _unknown41 = false;	// 0xE37
		#endregion


		public FlightGroup()
		{
			_arrDepTriggers = new byte[6, 6];
			_orders = new byte[16, 19];
			_waypoints = new short[132, 5];
			for (int i=0;i<16;i++)
			{
				_orders[i, 1] = 10;	// 100% throttle
				_orders[i, 10] = 1;	// T3 OR T4
				_orders[i, 16] = 1;	// T1 OR T2
				_orderStrings[i] = "";
			}
			for (int i=0;i<8;i++)
				for (int j=0;j<3;j++)
					_goalStrings[i,j] = "";
			_goals[0, 1] = 10;		// Goal 1 must NONE
			_goals[1, 1] = 10;
			_goals[2, 1] = 10;
			_goals[3, 1] = 10;
			_goals[4, 1] = 10;
			_goals[5, 1] = 10;
			_goals[6, 1] = 10;
			_goals[7, 1] = 10;
			_optLoadout[0] = true;
			_optLoadout[8] = true;
			_optLoadout[12] = true;
			_waypoints[0, 3] = 1;	// enable SP1
		}

		#region craft
		public bool EnableDesignation1
		{
			get { return _enableDesignation1; }
			set { _enableDesignation1 = value; }
		}
		public bool EnableDesignation2
		{
			get { return _enableDesignation2; }
			set { _enableDesignation2 = value; }
		}
		public byte Designation1
		{
			get { return _designation1; }
			set { _designation1 = value; }
		}
		public byte Designation2
		{
			get { return _designation2; }
			set { _designation2 = value; }
		}
		public byte Unknown1
		{
			get { return _unknown1; }
			set { _unknown1 = value; }
		}
		public byte GlobalCargo
		{
			get { return _globalCargo; }
			set { _globalCargo = value; }
		}
		public byte GlobalSpecialCargo
		{
			get { return _globalSpecialCargo; }
			set { _globalSpecialCargo = value; }
		}
		public string Role
		{
			get { return _role; }
			set
			{
				if (value.Length > _stringLength) _role = value.Substring(0, _stringLength);
				else _role = value;
			}
		}
		public byte Team
		{
			get { return _team; }
			set { _team = value; }
		}
		public byte Radio
		{
			get { return _radio; }
			set { _radio = value; }
		}
		public byte Unknown3
		{
			get { return _unknown3; }
			set { _unknown3 = value; }
		}
		public byte PlayerNumber
		{
			get { return _playerNumber; }
			set { _playerNumber = value; }
		}
		public bool ArriveOnlyIfHuman
		{
			get { return _arriveOnlyIfHuman; }
			set { _arriveOnlyIfHuman = value; }
		}
		public byte Unknown4
		{
			get { return _unknown4; }
			set { _unknown4 = value; }
		}
		#endregion
		#region arrdep
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0, 0] == 0 && _arrDepTriggers[1, 0] == 0 && _arrDepTriggers[2, 0] == 0 && _arrDepTriggers[3, 0] == 0 && _arrivalDelayMinutes == 0 && _arrivalDelaySeconds <= 30 && _waypoints[0,4] == 0) return true;
				else return false;
			}
		}
		public byte Unknown5
		{
			get { return _unknown5; }
			set { _unknown5 = value; }
		}
		public bool[] ArrDepAndOr { get { return _arrDepAndOr; } }
		public bool Unknown6
		{
			get { return _unknown6; }
			set { _unknown6 = value; }
		}
		public byte Unknown7
		{
			get { return _unknown7; }
			set { _unknown7 = value; }
		}
		public byte Unknown8
		{
			get { return _unknown8; }
			set { _unknown8 = value; }
		}
		#endregion
		#region orders
		// TODO: OrderStrings Get and Set functions
		public string[] OrderStrings
		{
			get { return _orderStrings; }
			set { _orderStrings = value; }
		}
		public byte[] Unknown10 { get { return _unknown10; } }
		public bool[] Unknown11 { get { return _unknown11; } }
		public bool[] Unknown12 { get { return _unknown12; } }
		public bool[] Unknown13 { get { return _unknown13; } }
		public bool[] Unknown14 { get { return _unknown14; } }
		public byte[,] SkipToOrder { get { return _skipToOrder; } }
		public bool[] SkipAndOr { get { return _stOAndOr; } }
		#endregion
		public byte[,] Goals { get { return _goals; } }
		/// <summary>Gets the points awarded or subtracted for the selected goal</summary>
		/// <param name="index">Flightgroup Goal index, 0-7</param>
		/// <returns>Point total</returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public short GetGoalPoints(int index) { return (short)((sbyte)Goals[index, 3] * 25); }
		/// <summary>Sets the points awarded or subtracted for the selected goal</summary>
		/// <param name="index">Flightgroup Goal, 0-7</param>
		/// <param name="points">Point total, between -3200 and 3175 in multiples of 25 (will be rounded)</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void SetGoalPoints(int index, short points)
		{
			if (points > 3175 || points < -3200) throw new ArgumentException("value must be between -3200 and +3175");
			Goals[index, 3] = Convert.ToByte((points / 25));
		}
		public string[,] GoalStrings { get { return _goalStrings; } }
		#region Unks and Option
		public byte Unknown16
		{
			get { return _unknown16; }
			set { _unknown16 = value; }
		}
		public byte Unknown17
		{
			get { return _unknown17; }
			set { _unknown17 = value; }
		}
		public byte Unknown18
		{
			get { return _unknown18; }
			set { _unknown18 = value; }
		}
		public byte Unknown19
		{
			get { return _unknown19; }
			set { _unknown19 = value; }
		}
		public byte Unknown20
		{
			get { return _unknown20; }
			set { _unknown20 = value; }
		}
		public byte Unknown21
		{
			get { return _unknown21; }
			set { _unknown21 = value; }
		}
		public bool Unknown22
		{
			get { return _unknown22; }
			set { _unknown22 = value; }
		}
		public byte Unknown23
		{
			get { return _unknown23; }
			set { _unknown23 = value; }
		}
		public byte Unknown24
		{
			get { return _unknown24; }
			set { _unknown24 = value; }
		}
		public byte Unknown25
		{
			get { return _unknown25; }
			set { _unknown25 = value; }
		}
		public byte Unknown26
		{
			get { return _unknown26; }
			set { _unknown26 = value; }
		}
		public byte Unknown27
		{
			get { return _unknown27; }
			set { _unknown27 = value; }
		}
		public byte Unknown28
		{
			get { return _unknown28; }
			set { _unknown28 = value; }
		}
		public bool Unknown29
		{
			get { return _unknown29; }
			set { _unknown29 = value; }
		}
		public bool Unknown30
		{
			get { return _unknown30; }
			set { _unknown30 = value; }
		}
		public bool Unknown31
		{
			get { return _unknown31; }
			set { _unknown31 = value; }
		}
		public bool GlobalNumbering
		{
			get { return _globalNumbering; }
			set { _globalNumbering = value; }
		}
		public byte Unknown32
		{
			get { return _unknown32; }
			set { _unknown32 = value; }
		}
		public byte Unknown33
		{
			get { return _unknown33; }
			set { _unknown33 = value; }
		}
		public byte Countermeasures
		{
			get { return _countermeasures; }
			set { _countermeasures = value; }
		}
		public byte ExplosionTime
		{
			get { return _explosionTime; }
			set { _explosionTime = value; }
		}
		public byte Status2
		{
			get { return _status2; }
			set { _status2 = value; }
		}
		public byte GlobalUnit
		{
			get { return _globalUnit; }
			set { _globalUnit = value; }
		}
		public bool[] OptLoadout { get { return _optLoadout; } }
		public byte[,] OptCraft { get { return _optCraft; } }
		public byte OptCraftCategory
		{
			get { return _optCraftCategory; }
			set { _optCraftCategory = value; }
		}
		public string PilotID
		{
			get { return _pilotID; }
			set
			{
				if (value.Length > 15) _pilotID = value.Substring(0, 15);
				else _pilotID = value;
			}
		}
		public byte Backdrop
		{
			get { return _backdrop; }
			set { _backdrop = value; }
		}
		public bool Unknown34
		{
			get { return _unknown34; }
			set { _unknown34 = value; }
		}
		public bool Unknown35
		{
			get { return _unknown35; }
			set { _unknown35 = value; }
		}
		public bool Unknown36
		{
			get { return _unknown36; }
			set { _unknown36 = value; }
		}
		public bool Unknown37
		{
			get { return _unknown37; }
			set { _unknown37 = value; }
		}
		public bool Unknown38
		{
			get { return _unknown38; }
			set { _unknown38 = value; }
		}
		public bool Unknown39
		{
			get { return _unknown39; }
			set { _unknown39 = value; }
		}
		public bool Unknown40
		{
			get { return _unknown40; }
			set { _unknown40 = value; }
		}
		public bool Unknown41
		{
			get { return _unknown41; }
			set { _unknown41 = value; }
		}
		#endregion
	}
}
