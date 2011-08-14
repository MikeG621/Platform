using System;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for in-flight messages</summary>
	/// <remarks>Serializable to allow copy-paste functionality</remarks>
	public abstract class BaseMessage
	{
		protected string _messageString = "New Message";
		protected byte _color = 0;
		protected byte[,] _triggers;

		/// <value>Gets or sets the in-flight message string</value>
		/// <remarks>MessageString is restricted to 63 characters, defaults to /"New Message/"</remarks>
		public string MessageString
		{
			get { return _messageString; }
			set
			{
				if (value.Length > 0x3F) _messageString = value.Substring(0, 0x3F);
				else _messageString = value;
			}
		}
		/// <value>Gets or sets the message color index</value>
		/// <remarks>Default index of 0</remarks>
		public byte Color
		{
			get { return _color; }
			set { _color = value; }
		}
		/// <value>Array of triggers</value>
		/// <remarks>Size is set depending on platform</remarks>
		public byte[,] Triggers { get { return _triggers; } }
	}
}
