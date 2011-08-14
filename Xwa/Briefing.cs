using System;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Briefing object for XWA</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 32. Class is serializable for copy/paste functionality</remarks>
	public class Briefing : BaseBriefing
	{
		private bool[] _team = new bool[10];
		public const int TicksPerSecond = 0x19;
		public const int EventQuantityLimit = 0x1100;

		/// <summary>Initializes a blank briefing</summary>
		public Briefing()
		{	//initialize
            _platform = MissionFile.Platform.XWA;
			_length = 0x465;		// default to 45 seconds
			_events = new byte[0x4400];
			_briefingTags = new string[0x80];
			_briefingStrings = new string[0x80];
			int i;
			for (i=0;i<0x80;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events[2] = 6;		// move map to (0,0)
			_events[10] = 7;	// zoom map
			_events[12] = 0x20;
			_events[14] = 0x20;
			_events[16] = 0xF;
			_events[17] = 0x27;
			_events[18] = 0x22;
			_team[0] = true;
		}

		/// <summary>For use in multiplayer missions, determines which teams view the briefing</summary>
		/// <remarks>Array length = 10</remarks>
		public bool[] Team
		{
			get { return _team; }
			set { _team = value; }
		}
	}
}
