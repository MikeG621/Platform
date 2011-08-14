using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for individual in-flight messages</summary>
	/// <remarks>Class is serializable to allow copy & paste functionality</remarks>
	[Serializable]
	public class Message : BaseMessage
	{
		private string _short = "";
		private byte _delay = 0;
		private bool _trig1AndOrTrig2 = false;

		/// <summary>Creates a new Message object</summary>
		/// <remarks>Triggers is set to 2 triggers of Length 4</remarks>
		public Message()
		{
			_triggers = new byte[2, 4];
			_triggers[0, 0] = 10;
			_triggers[1, 0] = 10;
		}

		/// <value>Short string used as editor notes</value>
		/// <remarks>Value is restricted to 12 characters</remarks>
		public string Short
		{
			get { return _short; }
			set
			{
				if (value.Length > 0xC) _short = value.Substring(0, 0xC);
				else _short = value;
			}
		}
		/// <value>Seconds after trigger is fired, multiples of five</value>
		/// <remarks>Default is zero. Value of 1 is 5s, 2 is 10s, etc.</remarks>
		public byte Delay
		{
			get { return _delay; }
			set { _delay = value; }
		}
		/// <value>Determines if both triggers must be completed</value>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool Trig1AndOrTrig2
		{
			get { return _trig1AndOrTrig2; }
			set { _trig1AndOrTrig2 = value; }
		}
	}
}
