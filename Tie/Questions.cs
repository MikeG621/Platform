/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0
 * 
 * CHANGELOG
 * v7.0, 241006
 * [NEW] enums
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 */

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the officer briefing questions.</summary>
	/// <remarks>In each array, 0-4 are Flight Officer, 5-9 are Secret Officer.</remarks>
	public class Questions
	{
		/// <summary>Values for <see cref="PostTrigType"/>.</summary>
		public enum QuestionType : byte
		{
			/// <summary>Does not show.</summary>
			None,
			/// <summary>Primary goals.</summary>
			Primary,
			/// <summary>Secondary goals.</summary>
			Secondary
		}
		/// <summary>Values for <see cref="PostTrigger"/>.</summary>
		public enum QuestionCondition : byte
		{
			/// <summary>Not shown.</summary>
			None,
			/// <summary>Goals completed.</summary>
			Completed = 4,
			/// <summary>Goals failed.</summary>
			Failed
		}
		/// <summary>Creates a blank question set.</summary>
		public Questions()
		{
			for (int i = 0; i < 10; i++)
			{
				PreMissQuestions[i] = "";
				PreMissAnswers[i] = "";
				PostMissQuestions[i] = "";
				PostMissAnswers[i] = "";
			}
		}

		/// <summary>Gets the briefing questions.</summary>
		public string[] PreMissQuestions { get; } = new string[10];
		/// <summary>Gets the briefing answers.</summary>
		public string[] PreMissAnswers { get; } = new string[10];
		/// <summary>Gets the debriefing condition.</summary>
		public QuestionCondition[] PostTrigger { get; } = new QuestionCondition[10];
		/// <summary>Gets the debriefing condition type.</summary>
		public QuestionType[] PostTrigType { get; } = new QuestionType[10];
		/// <summary>Gets the debriefing questions.</summary>
		public string[] PostMissQuestions { get; } = new string[10];
		/// <summary>Gets the debriefing answers.</summary>
		public string[] PostMissAnswers { get; } = new string[10];
	}
}