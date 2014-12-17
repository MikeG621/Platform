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
 * [NEW] Indexer<T> implementation
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	public partial class FlightGroup : BaseFlightGroup
	{
		/// <summary>Object to provide array access to the Optional Craft Loadout values</summary>
		[Serializable] public class LoadoutIndexer : Indexer<bool>
		{
			/// <summary>Indexes for <see cref="this[Indexes]"/></summary>
			public enum Indexes : byte {
				/// <summary>All warheads unavailable</summary>
				NoWarheads,
				/// <summary>Space Bomb warheads</summary>
				SpaceBomb,
				/// <summary>Heavy Rocket warhesds</summary>
				HeavyRocket,
				/// <summary>Missile warheads</summary>
				Missile,
				/// <summary>Torpedo warheads</summary>
				Torpedo,
				/// <summary>Advanced Missile warheads</summary>
				AdvMissile,
				/// <summary>Advanced Torpedo warheads</summary>
				AdvTorpedo,
				/// <summary>MagPulse distruption warheads</summary>
				MagPulse,
				/// <summary>All beams unavailable</summary>
				NoBeam,
				/// <summary>Tractor beam</summary>
				TractorBeam,
				/// <summary>Targeting Jamming beam</summary>
				JammingBeam,
				/// <summary>Decoy beam</summary>
				DecoyBeam,
				/// <summary>All countermeasures unavailable</summary>
				NoCountermeasures,
				/// <summary>Chaff countermeasures</summary>
				Chaff,
				/// <summary>Flare countermeasures</summary>
				Flare
			}
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="loadout">The Loadout array</param>
			public LoadoutIndexer(bool[] loadout) : base(loadout) { }
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value, <b>0-15</b></param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br/>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br/>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br/>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
			public override bool this[int index]
			{
				get { return _items[index]; }
				set
				{
					if ((index == 0 || index == 8 || index == 12) && !value) return;
					_items[index] = value;
					if (index == 0 && value) for (int i = 1; i < 8; i++) _items[i] = false;	// set NoWarheads, clear warheads
					else if (index == 8 && value) for (int i = 9; i < 12; i++) _items[i] = false;	// set NoBeam, clear beams
					else if (index == 12 && value) for (int i = 13; i < 15; i++) _items[i] = false;	// set NoCMs, clear CMs
					else if (index < 8 && value) _items[0] = false;	// set a warhead, clear NoWarheads
					else if (index < 12 && value) _items[8] = false;	// set a beam, clear NoBeam
					else if (index < 15 && value) _items[12] = false;	// set a CM, clear NoCMs
					else if (index < 8)
					{
						bool used = false;
						for (int i = 1; i < 8; i++) used |= _items[i];
						if (!used) _items[0] = true;	// cleared last warhead, set NoWarheads
					}
					else if (index < 12)
					{
						bool used = false;
						for (int i = 9; i < 12; i++) used |= _items[i];
						if (!used) _items[8] = true;	// cleared last beam, set NoBeam
					}
					else
					{
						bool used = false;
						for (int i = 13; i < 15; i++) used |= _items[i];
						if (!used) _items[12] = true;	// cleared last CM, set NoCMs
					}
				}
			}
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br/>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br/>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br/>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>index</i> value</exception>
			public bool this[Indexes index]
			{
				get { return _items[(int)index]; }
				set { this[(int)index] = value; }	// make the other indexer do the work
			}
		}
	}
}
