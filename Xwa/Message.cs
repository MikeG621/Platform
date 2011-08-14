using System;

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Object for individual in-flight messages</summary>
	/// <remarks>Class is serializable to allow copy & paste functionality</remarks>
	public class Message : BaseMessage
	{
		private bool[] _sentTo = new bool[10];
		private bool[] _trigAndOr = new bool[4];
		private byte _unknown1 = 0;	// 0x66
		private string _voiceID = "";
		private byte _originatingFG = 0;
		private byte _delaySeconds = 0;
		private byte _delayMinutes = 0;
		private bool _unknown2 = false;	// 0xA0
		private string _note = "";

		/// <summary>Creates a new Message object</summary>
		/// <remarks>Triggers is set to 6 triggers of Length 6. Last two Triggers are Cancel Triggers.</remarks>
		public Message()
		{
			_triggers = new byte[6, 6];
			for (int i=0;i<6;i++) _triggers[i, 0] = 10;
			_sentTo[0] = true;
		}

		/// <summary>Controls which teams can receive the message</summary>
		/// <remarks>Array length = 10</remarks>
		/// <exception cref="ArgumentException">Attempting to set with an improperly sized array</exception>
		public bool[] SentTo
		{
			get { return _sentTo; }
			set
			{
				if (value.Length == _sentTo.Length) _sentTo = value;
				else throw new ArgumentException("Array must have the correct size");
			}
		}
		/// <summary>Determines if both triggers must be completed</summary>
		/// <remarks>Array is {1AO2, 3AO4, 12AO34, Cancel1AO2}.<br><i>false</i> is "And", <i>true</i> is "Or", defaults to <i>false</i></remarks>
		/// <exception cref="ArgumentException">Attempting to set with an improperly sized array</exception>
		public bool[] TrigAndOr
		{
			get { return _trigAndOr; }
			set
			{
				if (value.Length == _trigAndOr.Length) _trigAndOr = value;
				else throw new ArgumentException("Array must have the correct size");
			}
		}
		/// <summary>Unknown value</summary>
		/// <remarks>Default is zero, offset 0x66</remarks>
		public byte Unknown1
		{
			get { return _unknown1; }
			set { _unknown1 = value; }
		}
		/// <summary>Editor note typically used to signify the speaker</summary>
		/// <remarks>Value is restricted to 8 characters</remarks>
		public string VoiceID
		{
			get { return _voiceID; }
			set
			{
				if (value.Length > 8) _voiceID = value.Substring(0, 8);
				else _voiceID = value;
			}
		}
		/// <summary>Editor note typically used to signify the FlightGroup index of the speaker</summary>
		/// <remarks>Defaults to zero, there appears to be no game mechanic or consequence for using this value</remark>
		public byte OriginatingFG
		{
			get { return _originatingFG; }
			set { _originatingFG = value; }
		}
		/// <value>Seconds after trigger is fired</value>
		/// <remarks>Default is zero. Can be combined with <i>DelayMinutes</i></remarks>
		public byte DelaySeconds
		{
			get { return _delaySeconds; }
			set { _delaySeconds = value; }
		}
		/// <value>Minutes after trigger is fired</value>
		/// <remarks>Default is zero. Can be combined with <i>DelaySeconds</i></remarks>
		public byte DelayMinutes
		{
			get { return _delayMinutes; }
			set { _delayMinutes = value; }
		}
		/// <summary>Unknown value</summary>
		/// <remarks>Default is <i>false</i>, offset 0xA0</remarks>
		public bool Unknown2
		{
			get { return _unknown2; }
			set { _unknown2 = value; }
		}
		/// <value>String used as editor notes</value>
		/// <remarks>Value is restricted to 63 characters</remarks>
		public string Note
		{
			get { return _note; }
			set
			{
				if (value.Length > 0x63) _note = value.Substring(0, 0x63);
				else _note = value;
			}
		}
	}
}
