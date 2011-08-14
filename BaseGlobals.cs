using System;

namespace Idmr.Platform
{
	[Serializable]
	/// <summary>Base class for global goals</summary>
	/// <remarks>Contains the mission-wide goals that are not attached to a single FlightGroup</remarks>
	public abstract class BaseGlobals
	{
		protected byte[,] _triggers;
		protected bool[] _andOr = new bool[9];

		/// <value>Complete 2-D trigger array</value>
		/// <remarks>Size is determined by platform. Format is [index, variable]</remarks>
		public byte[,] Triggers { get { return _triggers; } }
		/// <value>Array of target selection modifiers</value>
		/// <remarks>Array is Length=9</remarks>
		public bool[] AndOr { get { return _andOr; } }
	}
}
