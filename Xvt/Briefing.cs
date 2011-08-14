using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Briefing object for XvT and BoP</summary>
	/// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48. Class is serializable for copy/paste functionality</remarks>
	public class Briefing : BaseBriefing
	{
		private short _unknown3 = 0;
		/// <value>Frames per second for briefing animation</value>
		/// <remarks>Value is 20 (0x14)</remarks>
		public const int TicksPerSecond = 0x14;
		/// <value>Maximum number of events that can be held</value>
		/// <remarks>Value is 202 (0xCA)</remarks>
		public const int EventQuantityLimit = 0xCA;

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{	//initialize
            _platform = MissionFile.Platform.XvT;
			_length = 0x384;	// default to 45 seconds
			_events = new byte[0x32A];
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			int i;
			for (i=0;i<0x20;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
			_events[2] = 6;		// move map to (0,0)
			_events[10] = 7;	// zoom map
			_events[12] = 0x30;
			_events[14] = 0x30;
			_events[16] = 0xF;
			_events[17] = 0x27;
			_events[18] = 0x22;
		}

		/// <summary>Unknown setting</summary>
		/// <remarks>Briefing offset 0x08</remarks>
		public short Unknown3
		{
			get { return _unknown3; }
			set { _unknown3 = value; }
		}
	}
}
