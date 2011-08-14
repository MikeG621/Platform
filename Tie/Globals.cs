using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the mission-wide goals</summary>
	public class Globals : BaseGlobals
	{
		/// <remarks><i>Triggers</i> contains six triggers of 4 bytes each all set to FALSE, <i>AndOr</i>.Length is 3</remarks>
		public Globals()
		{
			_triggers = new byte[6, 4];
			_andOr = new bool[3];
			for (int i=0;i<6;i++) _triggers[i, 0] = 10;
		}
	}
}
