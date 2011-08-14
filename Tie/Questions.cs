using System;

namespace Idmr.Platform.Tie
{
	/// <summary>Object for the officer briefing questions</summary>
	/// <remarks>In each array, 0-4 are Flight Officer, 5-9 are Secret Officer</remarks>
	public class Questions
	{
		private string[] _preMissQuestions = new string[10];
		private string[] _preMissAnswers = new string[10];
		private byte[] _postTrigger = new byte[10];
		private byte[] _postTrigType = new byte[10];
		private string[] _postMissQuestions = new string[10];
		private string[] _postMissAnswers = new string[10];

		/// <summary>Create a blank question set</summary>
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

		/// <summary>Briefing questions</summary>
		/// <remarks>Length is 10</remarks>
		public string[] PreMissQuestions { get { return _preMissQuestions; } }
		/// <summary>Briefing answers</summary>
		/// <remarks>Length is 10</remarks>
		public string[] PreMissAnswers { get { return _preMissAnswers; } }
		/// <summary>Debriefing condition</summary>
		/// <remarks>Length is 10</remarks>
		public byte[] PostTrigger { get { return _postTrigger; } }
		/// <summary>Debriefing condition type</summary>
		/// <remarks>Length is 10</remarks>
		public byte[] PostTrigType { get { return _postTrigType; } }
		/// <summary>Debriefing questions</summary>
		/// <remarks>Length is 10</remarks>
		public string[] PostMissQuestions { get { return _postMissQuestions; } }
		/// <summary>Debriefing answers</summary>
		/// <remarks>Length is 10</remarks>
		public string[] PostMissAnswers { get { return _postMissAnswers; } }
	}
}
