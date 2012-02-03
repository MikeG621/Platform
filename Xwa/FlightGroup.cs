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

namespace Idmr.Platform.Xwa
{
	[Serializable]
	/// <summary>Object for individual FlightGroups</summary>
	public class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		Mission.Trigger[] _arrDepTriggers = new Mission.Trigger[6];
		string _role = "";
		bool[] _arrDepAndOr = new bool[4];
		Order[,] _orders = new Order[4,4];
		Goal[] _goals = new Goal[8];
		string[,] _goalStrings = new string[8, 3];
		bool[] _optLoadout = new bool[15];	// [0-7]=warheads, [8-11]=beams, [12-14]=CMs
		OptionalCraft[] _optCraft = new OptionalCraft[10];
		string _pilotID = "";
		Waypoint[] _waypoints = new Waypoint[4];
		LoadoutIndexer _loadoutIndexer;

		/// <summary>Indexes for <i>ArrDepTrigger</i></summary>
		public enum ArrDepTriggerIndex : byte { Arrival1, Arrival2, Arrival3, Arrival4, Departure1, Departure2 }
		/// <summary>Values for <i>OptCraftCategory</i></summary>
		public enum OptionalCraftCategory : byte { None, AllFlyable, AllRebelFlyable, AllImperialFlyable, Custom }

		
		// TODO: UnknownValues array
		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All Orders set to 100% Throttle (16 total, 4 per Region), Goals set to NONE, SP1 Enabled, most Unknowns 0/false, Unknown1 to 2</remarks>
		public FlightGroup()
		{
			for (int i = 0; i < 6; i++) _arrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < 4; i++) _waypoints[i] = new Waypoint();
			for (int i = 0; i < 16; i++) _orders[i / 4, i % 4] = new Order();
			for (int i = 0; i < 8; i++) _goals[i] = new Goal();
			_optLoadout[0] = true;
			_optLoadout[8] = true;
			_optLoadout[12] = true;
			_waypoints[0].Enabled = true;
			Unknowns.Unknown1 = 2;
		}

		#region craft
		/// <summary>Determines if <i>Designation1</i> is used</summary>
		public bool EnableDesignation1;
		/// <summary>Determines if <i>Designation2</i> is used</summary>
		public bool EnableDesignation2;
		/// <summary>Primary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation1;
		/// <summary>Secondary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation2;
		/// <summary>The <i>Mission.GlobalCargo</i> index to be used instead of <i>Cargo</i></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalCargo;
		/// <summary>The <i>Mission.GlobalCargo</i> index to be used instead of <i>SpecialCargo</i></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalSpecialCargo;
		/// <summary>Gets or sets a text form of the craft role</summary>
		/// <remarks>19 character limit, appears to be an editor note</remarks>
		public string Role
		{
			get { return _role; }
			set { _role = StringFunctions.GetTrimmed(value, _stringLength); }
		}
		/// <summary>Gets or sets the allegiance value that controls goals and IFF behaviour</summary>
		public byte Team;
		/// <summary>Gets or sets team or player number to which the craft communicates with</summary>
		public byte Radio;
		/// <summary>Gets or sets if the craft has a human or AI pilot</summary>
		/// <remarks>Value of zero defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present</remarks>
		public byte PlayerNumber;
		/// <summary>Gets or sets if craft is required to be player-controlled</summary>
		/// <remarks>When <i>true</i>, craft with PlayerNumber set will not appear without a human player</remarks>
		public bool ArriveOnlyIfHuman;
		#endregion
		#region arrdep
		/// <summary>Returns <i>true</i> if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds
		{
			get
			{
				if (_arrDepTriggers[0].Condition == 0 && _arrDepTriggers[1].Condition == 0 && _arrDepTriggers[2].Condition == 0 && _arrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30) return true;
				else return false;
			}
		}
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Use the <i>ArrDepTriggerIndex</i> enumeration for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get { return _arrDepTriggers; } }
		/// <summary>Gets which ArrDep triggers must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}</remarks>
		public bool[] ArrDepAndOr { get { return _arrDepAndOr; } }
		#endregion
		/// <summary>Gets the FlightGroup objective commands</summary>
		/// <remarks>Array is [Region, OrderIndex]</remarks>
		public Order[,] Orders { get { return _orders; } }
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 8</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Gets the FlightGroup start location markers</summary>
		/// <remarks>Array is length 4</remarks>
		public Waypoint[] Waypoints { get { return _waypoints; } }
		#region Unks and Option
		/// <summary>Determines if the FlightGroup should share numbering across the GlobalUnit</summary>
		public bool GlobalNumbering;
		/// <summary>Gets or sets the defenses available to the FG</summary>
		public byte Countermeasures;
		/// <summary>Gets or sets the duration of death animation</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime;
		/// <summary>Gets or sets the second condition of FlightGroup upon creation</summary>
		public byte Status2;
		/// <summary>Gets or sets the additional grouping assignment, can share craft numbering</summary>
		public byte GlobalUnit;
		/// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use the <i>LoadoutIndexer.Indexes</i> enumeration for indexes</remarks>
		public LoadoutIndexer OptLoadout { get { return _loadoutIndexer; } }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get { return _optCraft; } }
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory = OptionalCraftCategory.None;

		/// <summary>Gets or sets the editor note primarily used to signifiy the pilot voice</summary>
		/// <remarks>15 character limit</remarks>
		public string PilotID
		{
			get { return _pilotID; }
			set { _pilotID = StringFunctions.GetTrimmed(value, 15); }
		}
		/// <summary>For Backdrop <i>CraftTypes</i>, is the image image</summary>
		/// <remarks>Zero denotes a light source</remarks>
		public byte Backdrop = 0;
		/// <summary>The unknown values container</summary>
		/// <remarks>All values except Unknown1 initialize to 0 or <i>false</i>, Unknown1 initializes to 2. Orders contain Unknown9-14, Goals contain Unknown15</remarks>
		public UnknownValues Unknowns;
		#endregion
		
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
		
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0018, in Craft section</remarks>
			public byte Unknown1;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x007B, in Craft section</remarks>
			public byte Unknown3;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0084, in Craft section</remarks>
			public byte Unknown4;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0087, in Arr/Dep section</remarks>
			public byte Unknown5;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0097, in Arr/Dep section</remarks>
			public bool Unknown6;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00BF, in Arr/Dep section</remarks>
			public byte Unknown7;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00C0, in Arr/Dep section</remarks>
			public byte Unknown8;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAE, in Unks/Options section</remarks>
			public byte Unknown16;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAF, in Unks/Options section</remarks>
			public byte Unknown17;
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB0, in Unks/Options section</remarks>
			public byte Unknown18;
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB1, in Unks/Options section</remarks>
			public byte Unknown19;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB2, in Unks/Options section</remarks>
			public byte Unknown20;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB3, in Unks/Options section</remarks>
			public byte Unknown21;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB4, in Unks/Options section</remarks>
			public bool Unknown22;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB6, in Unks/Options section</remarks>
			public byte Unknown23;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB7, in Unks/Options section</remarks>
			public byte Unknown24;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB8, in Unks/Options section</remarks>
			public byte Unknown25;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB9, in Unks/Options section</remarks>
			public byte Unknown26;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBA, in Unks/Options section</remarks>
			public byte Unknown27;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBB, in Unks/Options section</remarks>
			public byte Unknown28;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBC, in Unks/Options section</remarks>
			public bool Unknown29;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC0, in Unks/Options section</remarks>
			public bool Unknown30;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC1, in Unks/Options section</remarks>
			public bool Unknown31;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC5, in Unks/Options section</remarks>
			public byte Unknown32;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC6, in Unks/Options section</remarks>
			public byte Unknown33;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E29, in Unks/Options section</remarks>
			public bool Unknown34;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2B, in Unks/Options section</remarks>
			public bool Unknown35;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2D, in Unks/Options section</remarks>
			public bool Unknown36;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2F, in Unks/Options section</remarks>
			public bool Unknown37;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E31, in Unks/Options section</remarks>
			public bool Unknown38;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E33, in Unks/Options section</remarks>
			public bool Unknown39;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E35, in Unks/Options section</remarks>
			public bool Unknown40;
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E37, in Unks/Options section</remarks>
			public bool Unknown41;
		}
		
		/// <summary>Object to provide array access to the Optional Craft Loadout values</summary>
		[Serializable] public class LoadoutIndexer
		{
			FlightGroup _owner;
			public enum Indexes : byte { NoWarheads, SpaceBomb, HeavyRocket, Missile, Torpedo, AdvMissile, AdvTorpedo, MagPulse, NoBeam, TractorBeam, JammingBeam, DecoyBeam, NoCountermeasures, Chaff, Flare }
			
			/// <summary>Initializes the indexer</summary>
			/// <param name="parent">The parent FlightGroup</param>
			public LoadoutIndexer(FlightGroup parent) { _owner = parent; }
			
			/// <summary>Gets the size of the array</summary>
			public int Length { get { return _owner._optLoadout.Length; } }
			
			/// <summary>Gets or sets the Role values</summary>
			/// <param name="index">Indexes enumerated value, 0-15</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public bool this[int index]
			{
				get { return _owner._optLoadout[index]; }
				set
				{
					if ((index == 0 || index == 8 || index == 12) && !value) return;
					_owner._optLoadout[index] = value;
					if (index == 0 && value) for (int i = 1; i < 8; i++) _owner._optLoadout[i] = false;	// set NoWarheads, clear warheads
					else if (index == 8 && value) for (int i = 9; i < 12; i++) _owner._optLoadout[i] = false;	// set NoBeam, clear beams
					else if (index == 12 && value) for (int i = 13; i < 15; i++) _owner._optLoadout[i] = false;	// set NoCMs, clear CMs
					else if (index < 8 && value) _owner._optLoadout[0] = false;	// set a warhead, clear NoWarheads
					else if (index < 12 && value) _owner._optLoadout[8] = false;	// set a beam, clear NoBeam
					else if (index < 15 && value) _owner._optLoadout[12] = false;	// set a CM, clear NoCMs
					else if (index < 8)
					{
						bool used = false;
						for (int i = 1; i < 8; i++) used |= _owner._optLoadout[i];
						if (!used) _owner._optLoadout[0] = true;	// cleared last warhead, set NoWarheads
					}
					else if (index < 12)
					{
						bool used = false;
						for (int i = 9; i < 12; i++) used |= _owner._optLoadout[i];
						if (!used) _owner._optLoadout[8] = true;	// cleared last beam, set NoBeam
					}
					else
					{
						bool used = false;
						for (int i = 13; i < 15; i++) used |= _owner._optLoadout[i];
						if (!used) _owner._optLoadout[12] = true;	// cleared last CM, set NoCMs
					}
				}
			}
			
			/// <summary>Gets or sets the Role values</summary>
			/// <param name="index">Indexes enumerated value</param>
			/// <remarks>Cannot manually clear <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> indexes<br>
			/// Setting <i>NoWarheads</i>, <i>NoBeam</i> or <i>NoCountermeasures</i> will clear the appropriate indexes.<br>
			/// Setting any warhead, beam or countermeasure will clear the appropriate <i>No*</i> value.<br>
			/// Manually clearing all warheads, beams or countermeasures will set the appropriate <i>No*</i> value</remarks>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public bool this[Indexes index]
			{
				get { return _owner._optLoadout[(int)index]; }
				set { this[(int)index] = value; }	// make the other indexer do the work
			}
		}
		
		/// <summary>Object for a single Waypoint</summary>
		[Serializable] public class Waypoint : IWaypoint
		{
			short _rawX = 0;
			short _rawY = 0;
			short _rawZ = 0;
			bool _enabled = false;
			byte _region = 0;
			
			/// <summary>Gets or sets the Region that the Waypoint is located in</summary>
			public byte Region
			{
				get { return _region; }
				set { _region = value; }
			}
			
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
			byte[] _raw = new byte[0x10];
			string _incompleteText = "";
			string _completeText = "";
			string _failedText = "";
			
			/// <summary>Unknown value</summary>
			/// <remarks>Goal offset 0x4F</remarks>
			public bool Unknown15;
			
			/// <summary>Initializes a blank Goal</summary>
			/// <remarks>Condition is set to "never (FALSE)"</remarks>
			public Goal() { _raw[1] = 10; }
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data, must have Length of 16</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Goal(byte[] raw)
			{
				if (raw.Length != 0x10) throw new ArgumentException("Incorrect raw data length", "raw");
				_raw = raw;
			}
			
			/// <summary>Initlialize a new Goal from raw data</summary>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			public Goal(byte[] raw, int startIndex) { ArrayFunctions.TrimArray(raw, startIndex, _raw); }
			
			/// <summary>Array form of the Goal</summary>
			/// <param name="index">Valid indexes are 0-15. Indexes 6-13 are read-only</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set { if (index < 6 || index > 13) _raw[index] = value; }
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
			/// <remarks>Equals <i>RawPoints</i> * 25, limited from -3200 to +3175</remarks>
			public short Points
			{
				get { return (short)(RawPoints * 25); }
				set { RawPoints = (sbyte)((value > 3175 ? 3175 : (value < -3200 ? -3200 : value)) / 25); }
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
			/// <summary>Gets or sets the additional Goal setting</summary>
			public byte Parameter
			{
				get { return _raw[14]; }
				set { _raw[14] = value; }
			}
			/// <summary>Gets or sets the location within te Active Sequence</summary>
			public byte ActiveSequence
			{
				get { return _raw[15]; }
				set { _raw[15] = value; }
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
		
		/// <summary>Container for a single Order</summary>
		[Serializable] public class Order : IOrder
		{
			string _customText = "";
			byte[] _raw = new byte[19];
			Waypoint[] _waypoints = new Waypoint[8];
			Mission.Trigger[] _skipTriggers = new Mission.Trigger[2];
			
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x72</remarks>
			public byte Unknown10;
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x73</remarks>
			public bool Unknown11;
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x74</remarks>
			public bool Unknown12;
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x7B</remarks>
			public bool Unknown13;
			/// <summary>Unknown value</summary>
			/// <remarks>Order offset 0x81</remarks>
			public bool Unknown14;
			/// <summary>Whether or not the Skip Triggers are exclusive</summary>
			/// <remarks>Default is "Or" (true) due to default "never (FALSE)" Trigger</remarks>
			public bool SkipT1AndOrT2 = true;
			
			#region constructors
			/// <summary>Initializes a blank Order</summary>
			/// <remarks>Throttle set to 100%, AndOr values set to "Or", <i>SkipTriggers</i> sets to "never (FALSE)"</remarks>
			public Order()
			{
				_raw[1] = 10;	// Throttle
				_raw[10] = _raw[16] = 1;	// AndOrs
				initialize();
			}
			
			/// <summary>Initlializes a new Order from raw data</summary>
			/// <remarks><i>SkipTriggers</i> sets to "never (FALSE)"</remarks>
			/// <param name="raw">Raw byte data, must have Length of 19</param>
			/// <exception cref="ArgumentException">Incorrect <i>raw</i>.Length</exception>
			public Order(byte[] raw)
			{
				if (raw.Length != 19) throw new ArgumentException("Incorrect raw data length", "raw");
				_raw = raw;
				initialize();
			}
			
			/// <summary>Initlialize a new Order from raw data</summary>
			/// <remarks><i>SkipTriggers</i> sets to "never (FALSE)"</remarks>
			/// <param name="raw">Raw byte data</param>
			/// <param name="startIndex">Offset within <i>raw</i> to begin reading</param>
			/// <exception cref="IndexOutOfBoundsException"><i>startIndex</i> causes reading outside the range of <i>raw</i></exception>
			public Order(byte[] raw, int startIndex)
			{
				ArrayFunctions.TrimArray(raw, startIndex, _raw);
				initialize();
			}
			
			void initialize()
			{
				for (int i = 0; i < 8; i++) _waypoints[i] = new Waypoint();
				_skipTriggers[0] = new Mission.Trigger();
				_skipTriggers[1] = new Mission.Trigger();
				_skipTriggers[0].Condition = 10;
				_skipTriggers[1].Condition = 10;
			}
			#endregion constructors
			
			#region public properties
			/// <summary>Gets or sets the third order-specific setting</summary>
			public byte Variable3
			{
				get { return _raw[4]; }
				set { _raw[4] = value; }
			}
			/// <summary>Gets or sets the unknown value</summary>
			/// <remarks>Order offset 0x05</remarks>
			public byte Unknown9
			{
				get { return _raw[5]; }
				set { _raw[5] = value; }
			}
			/// <summary>Gets or sets the specific max velocity</summary>
			public byte Speed
			{
				get { return _raw[18]; }
				set { _raw[18] = value; }
			}
			/// <summary>Gets or sets the order description</summary>
			/// <remarks>Limited to 63 characters</remarks>
			public string CustomText
			{
				get { return _customText; }
				set { _customText = StringFunctions.GetTrimmed(value, 63); }
			}
			
			/// <summary>Gets the order-specific location markers</summary>
			/// <remarks>Array is length 8</remarks>
			public Waypoint[] Waypoints { get { return _waypoints; } }
			
			/// <summary>Gets the triggers that cause the FlightGroup to proceed directly to the order</summary>
			/// <remarks>Array is length 2</remarks>
			public Mission.Trigger[] SkipTriggers { get { return _skipTriggers; } }
			#endregion public properties
			
			#region IOrder Members
			/// <summary>Array form of the Order</summary>
			/// <remarks>Indexes 11 and 17 are read-only</remarks>
			/// <param name="index">Index, 0-18</param>
			/// <exception cref="IndexOutOfBoundsException">Invalid <i>index</i> value</exception>
			public byte this[int index]
			{
				get { return _raw[index]; }
				set { if (index != 11 && index != 17) _raw[index] = value; }
			}
			
			/// <summary>Gets the length of the array</summary>
			/// <remarks>To be used for the indexer, Orders through Speed</remarks>
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
