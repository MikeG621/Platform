/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the officer briefing questions</summary>
	/// <remarks>In each array, 0-4 are Flight Officer, 5-9 are Secret Officer</remarks>
	public class Questions
	{
		string[] _preMissQuestions = new string[10];
		string[] _preMissAnswers = new string[10];
		byte[] _postTrigger = new byte[10];
		byte[] _postTrigType = new byte[10];
		string[] _postMissQuestions = new string[10];
		string[] _postMissAnswers = new string[10];

		/// <summary>Creates a blank question set</summary>
		public Questions()
		{
			int i;
			for (i=0;i<10;i++)
			{
				_preMissQuestions[i] = "";
				_preMissAnswers[i] = "";
				_postMissQuestions[i] = "";
				_postMissAnswers[i] = "";
			}
		}

		/// <summary>Gets the briefing questions</summary>
		public string[] PreMissQuestions { get { return _preMissQuestions; } }
		/// <summary>Gets the briefing answers</summary>
		public string[] PreMissAnswers { get { return _preMissAnswers; } }
		/// <summary>Gets the debriefing condition</summary>
		public byte[] PostTrigger { get { return _postTrigger; } }
		/// <summary>Gets the debriefing condition type</summary>
		public byte[] PostTrigType { get { return _postTrigType; } }
		/// <summary>Gets the debriefing questions</summary>
		public string[] PostMissQuestions { get { return _postMissQuestions; } }
		/// <summary>Gets the debriefing answers</summary>
		public string[] PostMissAnswers { get { return _postMissAnswers; } }
	}
}
