/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2025 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 7.0+
 * 
 * CHANGELOG
 * [NEW] Condition.Always
 * [NEW] facial expressions
 * [UPD] Condition.None renamed to Never
 * [DEL] Type.None
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
			/// <summary>Primary goals.</summary>
			Primary = 1,
			/// <summary>Secondary goals.</summary>
			Secondary
		}
		/// <summary>Values for <see cref="PostTrigger"/>.</summary>
		public enum QuestionCondition : byte
		{
			/// <summary>Not shown.</summary>
			Never,
			/// <summary>Goals completed.</summary>
			Completed = 4,
			/// <summary>Goals failed.</summary>
			Failed,
			/// <summary>Always shown, regardless of outcome.</summary>
			/// <remarks>Important note: this is *not* a real value, but a placeholder for Platform to denote that the condition is being used.</remarks>
			Always
		}
		/// <summary>The expressions per Officer question.</summary>
		public enum OfficerFacialExpression : byte
		{
			/// <summary>The standard neutral expression.</summary>
			Normal = 1,
			/// <summary>Clearly smiling.</summary>
			Happy,
			/// <summary>A restrained smile, could be considered looking smug.</summary>
			Smirking,
			/// <summary>Very mad, almost growling.</summary>
			Angry,
			/// <summary>Judging, not happy.</summary>
			Disappointed,
			/// <summary>The standard neutral expression, but he does not blink.</summary>
			NormalUnblinking
		}
		/// <summary>The expressions per Secret Order officer question.</summary>
		/// <remarks>Not being able to see most of his face, many expressions are duplicates, and unblinking for some reason.</remarks>
		public enum SecretOrderExpression : byte
		{
			/// <summary>The standard neutral expression.</summary>
			Normal = 1,
			/// <summary>Could be inquisitive, or happy, but he doesn't blink.</summary>
			RaisedEyebrowUnblinking,
			/// <summary>Same as previous.</summary>
			RaisedEyebrowUnblinking2,
			/// <summary>Very mad.</summary>
			Angry,
			/// <summary>Same as others.</summary>
			RaisedEyebrowUnblinking3,
			/// <summary>The standard neutral expression, but he does not blink.</summary>
			NormalUnblinking
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
				OfficerFacialExpressions[i] = OfficerFacialExpression.Normal;
				SecretOrderExpressions[i] = SecretOrderExpression.Normal;
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
		/// <summary>Gets the facial expressions of the briefing officer.</summary>
		public OfficerFacialExpression[] OfficerFacialExpressions { get; } = new OfficerFacialExpression[10];
		/// <summary>Gets the facial expressions of the secret order officer.</summary>
		public SecretOrderExpression[] SecretOrderExpressions { get;  } = new SecretOrderExpression[10];
	}
}