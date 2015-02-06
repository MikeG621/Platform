/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2015 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.2
 */

/* CHANGELOG
 * v2.2, 150205
 * [FIX] marked Serializable
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] Indexer<T> implemented
 */
using System;

namespace Idmr.Platform
{
	/// <summary>Base class for mission Triggers</summary>
	[Serializable]
	public abstract class BaseTrigger : Idmr.Common.Indexer<byte>
	{
		/// <summary>Default constructor for derived classes</summary>
		protected BaseTrigger() { /* do nothing */ }

		/// <summary>Default constructor for derived classes</summary>
		/// <param name="raw">Raw data</param>
		protected BaseTrigger(byte[] raw) : base(raw) { /* do nothing */ }

		/// <summary>Gets or sets the Trigger itself</summary>
		public byte Condition
		{
			get { return _items[0]; }
			set { _items[0] = value; }
		}
		/// <summary>Gets or sets the category <see cref="Variable"/> belongs to</summary>
		public byte VariableType
		{
			get { return _items[1]; }
			set { _items[1] = value; }
		}
		/// <summary>Gets or sets the Trigger subject</summary>
		public byte Variable
		{
			get { return _items[2]; }
			set { _items[2] = value; }
		}
		/// <summary>Gets or sets the amount required to fire the Trigger</summary>
		public byte Amount
		{
			get { return _items[3]; }
			set { _items[3] = value; }
		}
	}
}
