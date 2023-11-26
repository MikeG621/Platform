/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2014 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.1
 */

/* CHANGELOG
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 */

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the officer briefing questions</summary>
	/// <remarks>In each array, 0-4 are Flight Officer, 5-9 are Secret Officer</remarks>
	public class Questions
	{
		/// <summary>Creates a blank question set</summary>
		public Questions()
		{
			int i;
			for (i=0;i<10;i++)
			{
				PreMissQuestions[i] = "";
				PreMissAnswers[i] = "";
				PostMissQuestions[i] = "";
				PostMissAnswers[i] = "";
			}
		}

		/// <summary>Gets the briefing questions</summary>
		public string[] PreMissQuestions { get; } = new string[10];
		/// <summary>Gets the briefing answers</summary>
		public string[] PreMissAnswers { get; } = new string[10];
		/// <summary>Gets the debriefing condition</summary>
		public byte[] PostTrigger { get; } = new byte[10];
		/// <summary>Gets the debriefing condition type</summary>
		public byte[] PostTrigType { get; } = new byte[10];
		/// <summary>Gets the debriefing questions</summary>
		public string[] PostMissQuestions { get; } = new string[10];
		/// <summary>Gets the debriefing answers</summary>
		public string[] PostMissAnswers { get; } = new string[10];
	}
}
