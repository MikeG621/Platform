/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 200212 - Indexer<T> implementation
 * *** v2.0 ***
 */
using System;

namespace Idmr.Platform
{
	/// <summary>Base class for mission Triggers</summary>
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
