/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0
 */

/* CHANGELOG
 * v3.0, 180903
 * [UPD] added IonPulse, EnergyBeam and ClusterMine [JB]
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
				/// <summary>MagPulse disruption warheads</summary>
				MagPulse,
                /// <summary>Ion Pulse warheads</summary>  
                IonPulse,
                /// <summary>All beams unavailable</summary>
				NoBeam,
				/// <summary>Tractor beam</summary>
				TractorBeam,
				/// <summary>Targeting Jamming beam</summary>
				JammingBeam,
				/// <summary>Decoy beam</summary>
				DecoyBeam,
                /// <summary>Energy beam</summary>
                EnergyBeam,
                /// <summary>All countermeasures unavailable</summary>
				NoCountermeasures,
				/// <summary>Chaff countermeasures</summary>
				Chaff,
				/// <summary>Flare countermeasures</summary>
				Flare,
				/// <summary>Cluster Mine countermeasures</summary>
				ClusterMine
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
			/// <exception cref="IndexOutOfRangeException">Invalid <paramref name="index"/> value</exception>
			public override bool this[int index]
			{
				get => _items[index];
				set => this[(Indexes)index] = value;    // make the other indexer do the work
			}

			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br/>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br/>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br/>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			public bool this[Indexes index]
			{
				get => _items[(int)index];
				set
				{
					if ((index == Indexes.NoWarheads || index == Indexes.NoBeam || index == Indexes.NoCountermeasures) && !value) return;
					_items[(int)index] = value;
					if (index == Indexes.NoWarheads) for (int i = (int)Indexes.SpaceBomb; i <= (int)Indexes.IonPulse; i++) _items[i] = false;
					else if (index == Indexes.NoBeam) for (int i = (int)Indexes.TractorBeam; i <= (int)Indexes.EnergyBeam; i++) _items[i] = false;
					else if (index == Indexes.NoCountermeasures) for (int i = (int)Indexes.Chaff; i <= (int)Indexes.ClusterMine; i++) _items[i] = false;
					else if ((int)index <= (int)Indexes.IonPulse && value) _items[(int)Indexes.NoWarheads] = false;
					else if ((int)index <= (int)Indexes.EnergyBeam && value) _items[(int)Indexes.NoBeam] = false;
					else if ((int)index <= (int)Indexes.ClusterMine && value) _items[(int)Indexes.NoCountermeasures] = false;
					else if ((int)index <= (int)Indexes.IonPulse)
					{
						bool used = false;
						for (int i = (int)Indexes.SpaceBomb; i <= (int)Indexes.IonPulse; i++) used |= _items[i];
						_items[(int)Indexes.NoWarheads] = !used;
					}
					else if ((int)index <= (int)Indexes.EnergyBeam)
					{
						bool used = false;
						for (int i = (int)Indexes.TractorBeam; i <= (int)Indexes.EnergyBeam; i++) used |= _items[i];
						_items[(int)Indexes.NoBeam] = !used;
					}
					else
					{
						bool used = false;
						for (int i = (int)Indexes.Chaff; i <= (int)Indexes.ClusterMine; i++) used |= _items[i];
						_items[(int)Indexes.NoCountermeasures] = !used;
					}
				}
			}
		}
	}
}
