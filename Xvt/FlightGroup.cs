/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.html
 * Version: 2.0
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable] public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[6];
		string[] _roles = new string[4];
		bool[] _arrDepAO = new bool[4];
		Order[] _orders = new Order[4];
		Mission.Trigger[] _skipToOrder4Trigger = new Mission.Trigger[2];
		Goal[] _goals = new Goal[8];
		bool[] _optLoad = new bool[15];
		OptionalCraft[] _optCraft = new OptionalCraft[10];
		Waypoint[] _waypoints = new Waypoint[22];
		RolesIndexer _rolesIndexer;
		LoadoutIndexer _loadoutIndexer;

		/// <summary>Indexes for <i>ArrDepTrigger</i></summary>
		public enum ArrDepTriggerIndex : byte { Arrival1, Arrival2, Arrival3, Arrival4, Departure1, Departure2 }
		/// <summary>Indexes for <i>Waypoints</i></summary>
		public enum WaypointIndex : byte { Start1, Start2, Start3, Start4, WP1, WP2, WP3, WP4, WP5, WP6, WP7, WP8, Rendezvous, Hyperspace, Briefing1, Briefing2, Briefing3, Briefing4, Briefing5, Briefing6, Briefing7, Briefing8 }
		/// <summary>Values for <i>OptCraftCategory</i></summary>
		public enum OptionalCraftCategory : byte { None, AllFlyable, AllRebelFlyable, AllImperialFlyable, Custom }
		
		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set to 100% Throttle, Goals set to NONE, SP1 Enabled, Unknowns 0/false</remarks>
		public FlightGroup()
		{
			_stringLength = 0x14;
			for (int i = 0; i < _orders.Length; i++) _orders[i] = new Order();
			for (int i = 0; i < _roles.Length; i++) _roles[i] = "";
			for (int i = 0; i < _arrDepTriggers.Length; i++) _arrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < _skipToOrder4Trigger.Length; i++) { _skipToOrder4Trigger[i] = new Mission.Trigger(); _skipToOrder4Trigger[i].Condition = 10; }
			for (int i = 0; i < _goals.Length; i++) _goals[i] = new Goal();
			_optLoad[0] = true;
			_optLoad[8] = true;
			_optLoad[12] = true;
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
			_rolesIndexer = new RolesIndexer(this);
			_loadoutIndexer = new LoadoutIndexer(this);
		}

		#region craft
		/// <summary>Gets the craft roles, such as Command Ship or Strike Craft</summary>
		/// <remarks>This value has been seen as an AI string with unknown results.</remarks>
		public RolesIndexer Roles { get { return _rolesIndexer; } }
		
		/// <summary>The allegiance value that controls goals and IFF behaviour</summary>
		public byte Team = 0;
		
		/// <summary>The team or player number to which the craft communicates with</summary>
		public byte Radio = 0;
		
		/// <summary>Determines if the craft has a human or AI pilot</summary>
		/// <remarks>Value of zero defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present.</remarks>
		public byte PlayerNumber = 0;
		
		/// <summary>Determines if craft is required to be player-controlled</summary>
		/// <remarks>When <i>true</i>, craft with PlayerNumber set will not appear without a human player.</remarks>
		public bool ArriveOnlyIfHuman = false;
		#endregion
		#region arr/dep
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && _arrDepTriggers[2].Condition == 0 && _arrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30) return true;
				else return false;
			}
		}
		/// <summary>Gets the Arrival and Departure triggers</summary>
		/// <remarks>Use the <i>ArrDepTriggerIndex</i> enumeration for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets which ArrDep triggers must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}</remarks>
		public bool[] ArrDepAO { get { return _arrDepAO; } }
		#endregion
		/// <summary>Gets the Orders used to control FlightGroup behaviour</summary>
		public Order[] Orders { get { return _orders; } }
		/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to Order[3]</summary>
		/// <remarks>Array length is 2</remarks>
		public Mission.Trigger[] SkipToOrder4Trigger { get { return _skipToOrder4Trigger; } }
		/// <summary>Determines if both Skip triggers must be completed</summary>
		/// <remarks><i>true</i> is AND, <i>false</i> is OR</remarks>
		public bool SkipToO4T1AndOrT2 = false;
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 7</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Gets the FlightGroup location markers</summary>
		/// <remarks>Use <i>WaypointIndex</i> enumeration for array indexes</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }
		
		#region Unks and Options
		/// <summary>The defenses available to the FG</summary>
		public byte Countermeasures = 0;
		/// <summary>The duration of death animation</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime = 0;
		/// <summary>The second condition of FlightGroup upon creation</summary>
		public byte Status2 = 0;
		/// <summary>The additional grouping assignment, can share craft numbering</summary>
		public byte GlobalUnit = 0;
		
		/// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use the <i>LoadoutIndexer.Indexes</i> enumeration for indexes</remarks>
		public LoadoutIndexer OptLoadout { get { return _loadoutIndexer; } }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get { return _optCraft; } }
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory = OptionalCraftCategory.None;
		/// <summary>The unknown values container</summary>
		/// <remarks>All values initialize to 0 or <i>false</i>. Orders contain Unknown6-9, Goals contain Unknown10-16</remarks>
		public UnknownValues Unknowns;
		#endregion
		
		/// <summary>Object to provide array access to the Optional Craft Loadout values</summary>
		[Serializable] public class LoadoutIndexer
		{
			FlightGroup _owner;
			/// <summary>Indexes for <i>this</i>[]</summary>
			public enum Indexes : byte { NoWarheads, SpaceBomb, HeavyRocket, Missile, Torpedo, AdvMissile, AdvTorpedo, MagPulse, NoBeam, TractorBeam, JammingBeam, DecoyBeam, NoCountermeasures, Chaff, Flare }
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent FlightGroup</param>
			public LoadoutIndexer(FlightGroup parent) { _owner = parent; }
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _owner._optLoad.Length; } }
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value, 0-15</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public bool this[int index]
			{
				get { return _owner._optLoad[index]; }
				set
				{
					if ((index == 0 || index == 8 || index == 12) && !value) return;
					_owner._optLoad[index] = value;
					if (index == 0 && value) for (int i = 1; i < 8; i++) _owner._optLoad[i] = false;	// set NoWarheads, clear warheads
					else if (index == 8 && value) for (int i = 9; i < 12; i++) _owner._optLoad[i] = false;	// set NoBeam, clear beams
					else if (index == 12 && value) for (int i = 13; i < 15; i++) _owner._optLoad[i] = false;	// set NoCMs, clear CMs
					else if (index < 8 && value) _owner._optLoad[0] = false;	// set a warhead, clear NoWarheads
					else if (index < 12 && value) _owner._optLoad[8] = false;	// set a beam, clear NoBeam
					else if (index < 15 && value) _owner._optLoad[12] = false;	// set a CM, clear NoCMs
					else if (index < 8)
					{
						bool used = false;
						for (int i = 1; i < 8; i++) used |= _owner._optLoad[i];
						if (!used) _owner._optLoad[0] = true;	// cleared last warhead, set NoWarheads
					}
					else if (index < 12)
					{
						bool used = false;
						for (int i = 9; i < 12; i++) used |= _owner._optLoad[i];
						if (!used) _owner._optLoad[8] = true;	// cleared last beam, set NoBeam
					}
					else
					{
						bool used = false;
						for (int i = 13; i < 15; i++) used |= _owner._optLoad[i];
						if (!used) _owner._optLoad[12] = true;	// cleared last CM, set NoCMs
					}
				}
			}
			
			/// <summary>Gets or sets the Loadout values</summary>
			/// <param name="index">LoadoutIndex enumerated value</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public bool this[Indexes index]
			{
				get { return _owner._optLoad[(int)index]; }
				set { this[(int)index] = value; }	// make the other indexer do the work
			}
		}
		
		/// <summary>Object to provide array access to the Role values</summary>
		[Serializable] public class RolesIndexer
		{
			FlightGroup _owner;
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent FlightGroup</param>
			public RolesIndexer(FlightGroup parent) { _owner = parent; }
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _owner._roles.Length; } }
			
			/// <summary>Gets or sets the Role values</summary>
			/// <remarks>Limited to 4 characters</remarks>
			/// <param name="index">Role Index, 0-3</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public string this[int index]
			{
				get { return _owner._roles[index]; }
				set { _owner._roles[index] = StringFunctions.GetTrimmed(value, 4); }
			}
		}
		
		/// <summary>Container for the optional craft settings</summary>
		[Serializable] public struct OptionalCraft
		{
			/// <summary>The Craft Type</summary>
			public byte CraftType;
			/// <summary>The number of ships per Wave</summary>
			public byte NumberOfCraft;
			/// <summary>The number of waves available</summary>
			public byte NumberOfWaves;
		}
		/// <summary>Object for a single Waypoint</summary>
		[Serializable] public class Waypoint : IWaypoint
		{
			short _rawX = 0;
			short _rawY = 0;
			short _rawZ = 0;
			bool _enabled = false;
			
			#region IWaypoint Members
			/// <summary>Array form of the Waypoint.</summary>
			/// <param name="index">X, Y, Z, Enabled</param>
			/// <exception cref="ArgumentException">Invalid <i>index</i> value</exception>
			public short this[int index]
			{
				get
				{
					if (index == 0) return _rawX;
					else if (index == 1) return _rawY;
					else if (index == 2) return _rawZ;
					else if (index == 3) return Convert.ToInt16(_enabled);
					else throw new ArgumentException("index must be 0-3", "index");
				}
				set
				{
					if (index == 0) _rawX = value;
					else if (index == 1) _rawY = value;
					else if (index == 2) _rawZ = value;
					else if (index == 3) _enabled = Convert.ToBoolean(value);
				}
			}
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return 4; } }
			
			/// <summary>Gets or sets if the Waypoint is active for use</summary>
			public bool Enabled
			{
				get { return _enabled; }
				set { _enabled = value; }
			}
			/// <summary>Gets or sets the stored X value</summary>
			public short RawX
			{
				get { return _rawX; }
				set { _rawX = value; }
			}
			/// <summary>Gets or sets the stored Y value</summary>
			public short RawY
			{
				get { return _rawY; }
				set { _rawY = value; }
			}
			/// <summary>Gets or sets the stored Z value</summary>
			public short RawZ
			{
				get { return _rawZ; }
				set { _rawZ = value; }
			}
			/// <summary>Gets or sets the X value in kilometers</summary>
			public double X
			{
				get { return (double)RawX / 160; }
				set { RawX = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Y value in kilometers</summary>
			public double Y
			{
				get { return (double)RawY / 160; }
				set { RawY = (short)(value * 160); }
			}
			/// <summary>Gets or sets the Z value in kilometers</summary>
			public double Z
			{
				get { return (double)RawZ / 160; }
				set { RawZ = (short)(value * 160); }
			}
			#endregion
		}
		/// <summary>Object for a single FlightGroup-specific Goal</summary>
		[Serializable] public class Goal
		{
			byte[] _raw = new byte[15];
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks>Condition is set to "never (FALSE)"</remarks>
			public Goal() { _raw[1] = 10; }
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 15</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length != 15) throw new ArgumentException("Incorrect raw data length", "raw");
				_raw = raw;
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public Goal(byte[] raw, int startIndex) { ArrayFunctions.TrimArray(raw, startIndex, _raw); }
			
			/// <summary>Array form of the Goal</summary>
			/// <param name="index">Valid indexes are 0-14. Indexes 9, 10 and 13 are read-only</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set { if (index != 9 && index != 10 && index != 13) _raw[index] = value; }
			}
			
			#region public properties
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _raw.Length; } }
			
			/// <summary>Gets or sets the goal behaviour</summary>
			/// <remarks>Values are 0-3; must, must not (Prevent), BONUS must, BONUS must not (bonus prevent)</remarks>
			public byte Argument
			{
				get { return _raw[0]; }
				set { _raw[0] = value; }
			}
			/// <summary>Gets or sets the Goal trigger</summary>
			public byte Condition
			{
				get { return _raw[1]; }
				set { _raw[1] = value; }
			}
			/// <summary>Gets or sets the amount of the FlightGroup required to meet <i>Condition</i></summary>
			public byte Amount
			{
				get { return _raw[2]; }
				set { _raw[2] = value; }
			}
			/// <summary>Gets or sets the points value stored in the file</summary>
			public sbyte RawPoints
			{
				get { return (sbyte)_raw[3]; }
				set { _raw[3] = (byte)value; }
			}
			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equals <i>RawPoints</i> * 250, limited from -32000 to +31750</remarks>
			public short Points
			{
				get { return (short)(_raw[3] * 250); }
				set { _raw[3] = Convert.ToByte((value > 31750 ? 31750 : (value < -32000 ? -32000 : value)) / 250); }
			}
			/// <summary>Gets or sets if the Goal is active</summary>
			public bool Enabled
			{
				get { return Convert.ToBoolean(_raw[4]); }
				set { _raw[4] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets which Team the Goal applies to</summary>
			public byte Team
			{
				get { return _raw[5]; }
				set { _raw[5] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x06</summary>
			public bool Unknown10
			{
				get { return Convert.ToBoolean(_raw[6]); }
				set { _raw[6] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x07</summary>
			public bool Unknown11
			{
				get { return Convert.ToBoolean(_raw[7]); }
				set { _raw[7] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x08</summary>
			public bool Unknown12
			{
				get { return Convert.ToBoolean(_raw[8]); }
				set { _raw[8] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x0B</summary>
			public byte Unknown13
			{
				get { return _raw[11]; }
				set { _raw[11] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x0C</summary>
			public bool Unknown14
			{
				get { return Convert.ToBoolean(_raw[12]); }
				set { _raw[12] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Goal offset 0x0E</summary>
			public byte Unknown16
			{
				get { return _raw[14]; }
				set { _raw[14] = value; }
			}
			
			/// <summary>Gets or sets the goal text shown before completion</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary goals</remarks>
			public string IncompleteText
			{
				get { return _incompleteText; }
				set { _incompleteText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after completion</summary>
			/// <remarks>String is limited to 63 char</remarks>
			public string CompleteText
			{
				get { return _completeText; }
				set { _completeText = StringFunctions.GetTrimmed(value, 63); }
			}
			/// <summary>Gets or sets the goal text shown after failure</summary>
			/// <remarks>String is limited to 63 char. Not used for Secondary or Prevent goals</remarks>
			public string FailedText
			{
				get { return _failedText; }
				set { _failedText = StringFunctions.GetTrimmed(value, 63); }
			}
			#endregion public properties
		}
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0062, in Craft section</remarks>
			public byte Unknown1;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0063, in Craft section</remarks>
			public bool Unknown2;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0085, in Arr/Dep section, may be ArrDelay30Sec</remarks>
			public byte Unknown3;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0096, in Arr/Dep section</remarks>
			public byte Unknown4;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0098, in Arr/Dep section</remarks>
			public byte Unknown5;

			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0516, in Unknowns/Options section</remarks>
			public bool Unknown17;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0518, in Unknowns/Options section</remarks>
			public bool Unknown18;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0520, in Unknowns/Options section</remarks>
			public bool Unknown19;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0521, in Unknowns/Options section</remarks>
			public byte Unknown20;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0522, in Unknowns/Options section</remarks>
			public byte Unknown21;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0527, in Unknowns/Options section</remarks>
			public bool Unknown22;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0528, in Unknowns/Options section</remarks>
			public bool Unknown23;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0529, in Unknowns/Options section</remarks>
			public bool Unknown24;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052A, in Unknowns/Options section</remarks>
			public bool Unknown25;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052B, in Unknowns/Options section</remarks>
			public bool Unknown26;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052C, in Unknowns/Options section</remarks>
			public bool Unknown27;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052D, in Unknowns/Options section</remarks>
			public bool Unknown28;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x052E, in Unknowns/Options section</remarks>
			public bool Unknown29;
		}
		/// <summary>Object for a single Order</summary>
		[Serializable] public class Order : IOrder
		{
			byte[] _raw = new byte[19];
			string _designation = "";
			
			/// <summary>Initializes a blank Order</summary>
			/// <remarks>Throttle set to 100%, AndOr values set to "Or"</remarks>
			public Order()
			{
				_raw[1] = 10;	// Throttle
				_raw[10] = _raw[16] = 1;	// AndOrs
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 19</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Order(byte[] raw)
			{
				if (raw.Length != 19) throw new ArgumentException("Incorrect raw data length", "raw");
				_raw = raw;
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public Order(byte[] raw, int startIndex) { ArrayFunctions.TrimArray(raw, startIndex, _raw); }
			
			#region public properties
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x04</remarks>
			public byte Unknown6
			{
				get { return _raw[4]; }
				set { _raw[4] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown7
			{
				get { return _raw[5]; }
				set { _raw[5] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x0B</remarks>
			public byte Unknown8
			{
				get { return _raw[11]; }
				set { _raw[11] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x11</remarks>
			public byte Unknown9
			{
				get { return _raw[17]; }
				set { _raw[17] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _raw[18]; }
				set { _raw[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 16 characters</remarks>
			public string Designation
			{
				get { return _designation; }
				set { _designation = StringFunctions.GetTrimmed(value, 16); }
			}
			#endregion public properties
			
			#region IOrder Members
			/// <summary>Array form of the Order</summary>
			/// <param name="index">Index, 0-18</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set { _raw[index] = value; }
			}
			
			/// <summary>Gets the length of the array</summary>
			public int Length { get { return _raw.Length; } }
			
			/// <summary>Gets or sets the command for the FlightGroup</summary>
			public byte Command
			{
				get { return _raw[0]; }
				set { _raw[0] = value; }
			}
			/// <summary>Gets or sets the throttle setting</summary>
			/// <remarks>Multiply value by 10 to get Throttle percent</remarks>
			public byte Throttle
			{
				get { return _raw[1]; }
				set { _raw[1] = value; }
			}
			/// <summary>Gets or sets the first order-specific setting</summary>
			public byte Variable1
			{
				get { return _raw[2]; }
				set { _raw[2] = value; }
			}
			/// <summary>Gets or sets the second order-specific setting</summary>
			public byte Variable2
			{
				get { return _raw[3]; }
				set { _raw[3] = value; }
			}
			/// <summary>Gets or sets the Type for Target 3</summary>
			public byte Target3Type
			{
				get { return _raw[6]; }
				set { _raw[6] = value; }
			}
			/// <summary>Gets or sets the Type for target 4</summary>
			public byte Target4Type
			{
				get { return _raw[7]; }
				set { _raw[7] = value; }
			}
			/// <summary>Gets or sets the third target</summary>
			public byte Target3
			{
				get { return _raw[8]; }
				set { _raw[8] = value; }
			}
			/// <summary>Gets or sets the fourth target</summary>
			public byte Target4
			{
				get { return _raw[9]; }
				set { _raw[9] = value; }
			}
			/// <summary>Gets or sets if the T3 and T4 settings are mutually exclusive</summary>
			/// <remarks><i>false</i> is And/If and <i>true</i> is Or</remarks>
			public bool T3AndOrT4
			{
				get { return Convert.ToBoolean(_raw[10]); }
				set { _raw[10] = Convert.ToByte(value); }
			}
			/// <summary>Gets or sets the Type for Target 1</summary>
			public byte Target1Type
			{
				get { return _raw[12]; }
				set { _raw[12] = value; }
			}
			/// <summary>Gets or sets the first target</summary>
			public byte Target1
			{
				get { return _raw[13]; }
				set { _raw[13] = value; }
			}
			/// <summary>Gets or sets the Type for Target 2</summary>
			public byte Target2Type
			{
				get { return _raw[14]; }
				set { _raw[14] = value; }
			}
			/// <summary>Gets or sets the second target</summary>
			public byte Target2
			{
				get { return _raw[15]; }
				set { _raw[15] = value; }
			}
			/// <summary>Gets or sets if the T1 and T2 settings are mutually exclusive</summary>
			/// <remarks><i>false</i> is And/If and <i>true</i> is Or</remarks>
			public bool T1AndOrT2
			{
				get { return Convert.ToBoolean(_raw[16]); }
				set { _raw[16] = Convert.ToByte(value); }
			}
			#endregion IOrder Members
		}
	}
}
