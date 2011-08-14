using System;

namespace Idmr.Platform.Xvt
{
	[Serializable]
	/// <summary>Object for individual in-flight messages</summary>
	/// <remarks>Class is serializable to allow copy & paste functionality</remarks>
	public class Message : BaseMessage
	{
		private bool[] _sentToTeam = new bool[10];
		private bool _t1AndOrT2 = false;
		private bool _t3AndOrT4 = false;
		private bool _t12AndOrT34 = false;
		private string _note = "";
		private byte _delay = 0;

		/// <summary>Creates a new Message object</summary>
		/// <remarks>Triggers is set to 4 triggers of Length 4</remarks>
		public Message()
		{
			_triggers = new byte[4, 4];
			for (int i=0;i<4;i++) _triggers[i, 0] = 10;
			_sentToTeam[0] = true;
		}

		/// <summary>Controls which teams can receive the message</summary>
		/// <remarks>Array length = 10</remarks>
		/// <exception cref="ArgumentException">Attempting to set with an improperly sized array</exception>
		public bool[] SentToTeam
		{
			get { return _sentToTeam; }
			set
			{
				if (value.Length == _sentToTeam.Length) _sentToTeam = value;
				else throw new ArgumentException("Array must have the correct size");
			}
		}
		/// <value>Determines if both triggers must be completed</value>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T1AndOrT2
		{
			get { return _t1AndOrT2; }
			set { _t1AndOrT2 = value; }
		}
		/// <value>Determines if both triggers must be completed</value>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T3AndOrT4
		{
			get { return _t3AndOrT4; }
			set { _t3AndOrT4 = value; }
		}
		/// <value>Determines if both trigger pairs must be completed</value>
		/// <remarks><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		public bool T12AndOrT34
		{
			get { return _t12AndOrT34; }
			set { _t12AndOrT34 = value; }
		}
		/// <value>Short string used as editor notes</value>
		/// <remarks>Value is restricted to 15 characters</remarks>
		public string Note
		{
			get { return _note; }
			set
			{
				if (value.Length > 0xF) _note = value.Substring(0, 0xF);
				else _note = value;
			}
		}
		/// <value>Seconds after trigger is fired, multiples of five</value>
		/// <remarks>Default is zero. Value of 1 is 5s, 2 is 10s, etc.</remarks>
		public byte Delay
		{
			get { return _delay; }
			set { _delay = value; }
		}
	}
}
