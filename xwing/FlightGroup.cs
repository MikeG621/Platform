/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2024 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 6.1+
 */

/* CHANGELOG
 * [UPD] GetTIECraftType now returns byte
 * v6.1, 231208
 * [FIX] WaypointIndex
 * v4.0, 200809
 * [NEW] Raw values for Pitch/Yaw/Roll instead of degrees [JB]
 * [UPD] _stringLength increased [JB]
 * v3.0, 180309
 * [NEW] created [JB]
 */

using System;

namespace Idmr.Platform.Xwing
{
	/// <summary>Object for individual FlightGroups<./summary>
	[Serializable] public partial class FlightGroup : BaseFlightGroup
	{
		readonly Waypoint[] _waypoints = new Waypoint[15];

		/// <summary>Gets or sets the object z-axis orientation at start in raw units (64 = 90 degrees).</summary>
		/// <remarks>Unlike BaseFlightGroup, this needs to accept arbitrarily large values to accomodate the Proving Grounds platforms. See also <see cref="Pitch"/> and <see cref="Roll"/>. Only applies to object groups.</remarks>
		public new short Yaw
		{
			get => _yaw;
			set => _yaw = value;
		}
		/// <summary>Gets or sets the object y-axis orientation at start in raw units (64 = 90 degrees).</summary>
		/// <remarks>Unlike BaseFlightGroup, this needs to accept arbitrarily large values to accomodate the Proving Grounds platforms. See also <see cref="Yaw"/> and <see cref="Roll"/>. Only applies to object groups.</remarks>
		public new short Pitch
		{
			get => _pitch;
			set => _pitch = value;
		}
		/// <summary>Gets or sets the object x-axis orientation at start in raw units (64 = 90 degrees).</summary>
		/// <remarks>Unlike BaseFlightGroup, this needs to accept arbitrarily large values to accomodate the Proving Grounds platforms. See also <see cref="Yaw"/> and <see cref="Pitch"/>. Only applies to object groups.</remarks>
		public new short Roll
		{
			get => _roll;
			set => _roll = value;
		}

		/// <summary>Indexes for <see cref="Waypoints"/>.</summary>
		public enum WaypointIndex : byte
		{
			/// <summary>Primary starting coordinate.</summary>
			Start1,
			/// <summary>First coordinate for orders and initial trajectory.</summary>
			WP1,
			/// <summary>Second order coordinate.</summary>
			WP2,
			/// <summary>Third order coordinate.</summary>
			WP3,
			/// <summary>Optional starting coordinate.</summary>
			Start2,
			/// <summary>Optional starting coordinate.</summary>
			Start3,
			/// <summary>Arrival and Departure coordinate.</summary>
			Hyperspace,
			/// <summary>Coordinate Set 1.</summary>
			CS1,
			/// <summary>Coordinate Set 2.</summary>
			CS2,
			/// <summary>Coordinate Set 3.</summary>
			CS3,
			/// <summary>Coordinate Set 4.</summary>
			CS4,
			/// <summary>Coordinate Set 5.</summary>
			CS5,
			/// <summary>Coordinate Set 6.</summary>
			CS6,
			/// <summary>Coordinate Set 7.</summary>
			CS7,
			/// <summary>Coordinate Set 8.</summary>
			CS8
		}

		/// <summary>Initializes a new FlightGroup.</summary>
		/// <remarks>All Orders set Throttle to <b>100%</b>, all Goals set to <b>FALSE</b>, SP1 <b>Enabled</b>.</remarks>
		public FlightGroup()
		{
			_stringLength = 0x10;
			NumberOfWaves = 0;
			NumberOfCraft = 1;
			CraftType = 1;   //0 is None, default to X-wing.
			ObjectType = 0;  //Not an object.
            SpecialCargoCraft = -1;
            ArrivalHyperspace = 1;
			DepartureHyperspace = 1;
			Mothership = -1;  //These variables use a zero based index for FG, or -1 for none.
			TargetPrimary = -1;
			TargetSecondary = -1;
			for (int i = 0; i < _waypoints.Length; i++) _waypoints[i] = new Waypoint();
			_waypoints[(int)WaypointIndex.Start1].Enabled = true;
			Pitch = 64;  //Default for space objects, in raw units (64 = 90 degrees).
		}

		#region functions
		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <returns>Short representation of the FlightGroup as <b>"CraftAbbrv Name"</b>.</returns>
		public override string ToString() => ToString(false);
		/// <summary>Gets a string representation of the FlightGroup.</summary>
		/// <remarks>Short form is <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/></b>.
		/// <br/>Long form is <b>"<see cref="BaseFlightGroup.IFF"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see>
		/// - IsPlayer <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/>.
		/// <see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b>.</remarks>
		/// <param name="verbose">When <b>true</b> returns long form.</param>
		/// <returns>Representation of the FlightGroup.</returns>
		public string ToString(bool verbose)
		{
			string waves = (NumberOfWaves + 1) + "x" + NumberOfCraft;
			string longName;
			if (IsObjectGroup())
			{
				int index = ObjectType - 17;
				if (index < 0) index = 0;
				longName = "{" + Strings.ObjectType[index] + "} " + Name;
			}
			else
			{
				if (CraftType == 2 && Status1 >= 10)  // Index hack for B-wings
					longName = Strings.CraftAbbrv[18] + " " + Name;
				else if (CraftType == 25)
					longName = Strings.CraftAbbrv[18] + " " + Name;
				else
					longName = Strings.CraftAbbrv[CraftType] + " " + Name;

				if (EditorCraftNumber > 0) //[JB] Added numbering information.
					longName += EditorCraftExplicit ? " " + EditorCraftNumber : " <" + EditorCraftNumber + ">";
			}

			if (!verbose) return longName;
            return IFF + " - " + (PlayerCraft != 0 ? "*" : "") + waves + " " + longName;
		}

		/// <summary>Gets the actual IFF code as it would appear in game.</summary>
		/// <remarks>X-wing is unique in the series in that it has a default IFF code which translates into a different IFF in-game depending on CraftType.</remarks>
		/// <returns>The defined IFF or an IFF based on <see cref="BaseFlightGroup.CraftType"/>.</returns>
		public byte GetActualIFF()
		{
            if(IFF > 0) return IFF;
            switch(CraftType)
			{
				case 1: case 2: case 3: case 13: case 14: return 1;                  //Rebel:    X-W,Y-W,A-W,CRS,FRG (and B-W, same CraftType as Y-W, but with a special status)
				case 4: case 5: case 6: case 7: case 8: case 16: case 17: return 2;  //Imperial: T/F,T/I,T/B,GUN,TRN,STD,T/A
				case 9: case 10: case 11: case 12: case 15: return 3;                //Neutral:  SHU,TUG,CON,FRT,CRV
			}
            if (ObjectType == 25) return 1;                                          //Special case for B-wing briefing icon.
            return 0;
		}

		/// <summary>Returns a TIE95 compatible IFF code.</summary>
		/// <returns>The IFF code adjusted for later platforms.</returns>
        public byte GetTIEIFF()
        {
            byte iff = GetActualIFF();
            if (iff >= 1) return (byte)(iff - 1);   //Rebel, Imperial, Neutral
            return 3;  //Purple
        }

		/// <summary>Gets the TIE95 equivalent of <see cref="ObjectType"/> or <see cref="BaseFlightGroup.CraftType"/> as appropriate.</summary>
		/// <returns>Resultant <see cref="BaseFlightGroup.CraftType"/> index, or <b>255</b> if invalid.</returns>
		public byte GetTIECraftType()
        {
            if (ObjectType > 0)
            {
                switch (ObjectType)
                {
                    case 18: case 19: case 20: case 21: return 0x4B; //Mines
                    case 22: return 0x46; //Satellite
                    case 23: return 0x53; //Nav Buoy
                    case 24: return 0x50; //Probe
                    case 25: return 0x4;  //Index hack for B-wings in a BRF listing.
                    default:
                        if (ObjectType >= 26 && ObjectType <= 33) return 0x56;  //Asteroid
                        if (ObjectType >= 34 && ObjectType <= 49) return 0x57;  //Planet
                        return 0x53; //If nothing else, return a Nav-Buoy
                }
            }
            switch (CraftType)
            {
                case 0: return 0x01;   // None
                case 1: return 0x01;   // X-W
                case 2: return (byte)((Status1 >= 10) ? 0x04 : 0x02);  //B-W, Y-W
                case 3: return 0x03;   // A-W
                case 4: return 0x05;   // T/F
                case 5: return 0x06;   // T/I
                case 6: return 0x07;   // T/B
                case 7: return 0x10;   // GUN
                case 8: return 0x15;   // TRN
                case 9: return 0x11;   // SHU
                case 10: return 0x18;  // TUG
                case 11: return 0x1A;  // CON
                case 12: return 0x20;  // FRT
                case 13: return 0x31;  // CRS
                case 14: return 0x2A;  // FRG
                case 15: return 0x28;  // CRV
                case 16: return 0x35;  // ISD
                case 17: return 0x08;  // T/A
            }
            return 255;
        }		

		/// <summary>Gets the calculated minutes from the raw <see cref="ArrivalDelay"/> value.</summary>
		/// <returns>The minutes portion of the delay.</returns>
		public int GetArrivalMinutes()
		{
			if(ArrivalDelay > 20)
				return (ArrivalDelay - 20) / 10;

			return ArrivalDelay;
		}
		/// <summary>Gets the calculated seconds from the raw <see cref="ArrivalDelay"/> value.</summary>
		/// <returns>The seconds portion of the delay.</returns>
		public int GetArrivalSeconds()
		{
			if(ArrivalDelay > 20)
				return (ArrivalDelay % 10) * 6;

			return 0;
		}
		/// <summary>Generate the appropriate value to be used in <see cref="ArrivalDelay"/>.</summary>
		/// <param name="minutes">The minutes value.</param>
		/// <param name="seconds">The seconds value.</param>
		/// <returns>The calculated delay value.</returns>
		public short CreateArrivalDelay(int minutes, int seconds)
		{
			int delay;
			if (minutes < 0) minutes = 0;
			if (seconds < 0) seconds = 0;
			if (seconds == 0 && minutes <= 20)
				delay = minutes;
			else
				delay = 20 + (minutes * 10) + (seconds / 6);
			return (short)delay;
		}

     	/// <summary>Changes all matching references to a FG index.</summary>
		/// <param name="srcIndex">The original FG index.</param>
		/// <param name="dstIndex">The new FG index.</param>
		/// <returns><b>true</b> if any changes are made.</returns>
        public bool ReplaceFGReference(int srcIndex, int dstIndex)
        {
            bool change = false;
            short dest = (short)dstIndex;
            if (ArrivalFG == srcIndex) { change = true; ArrivalFG = dest; if (dstIndex == -1) ArrivalEvent = 0; }
            if (Mothership == srcIndex) { change = true; Mothership = dest; if (dstIndex == -1) { ArrivalHyperspace = 1; DepartureHyperspace = 1; } }
            if (TargetPrimary == srcIndex) { change = true; TargetPrimary = dest; }
            if (TargetSecondary == srcIndex) { change = true; TargetSecondary = dest; }
            return change;
        }

		/// <summary>Gets if this craft is a FlightGroup or not.</summary>
		/// <returns><b>true</b> when <see cref="ObjectType"/> is zero.</returns>
		public bool IsFlightGroup() => (ObjectType == 0);
		/// <summary>Gets if this craft is a Object or not.</summary>
		/// <returns><b>true</b> when <see cref="ObjectType"/> is non-zero.</returns>
		public bool IsObjectGroup() => !IsFlightGroup();
		/// <summary>Gets if this craft is one of the Training Platform Objects.</summary>
		/// <returns><b>true</b> if <see cref="ObjectType"/> is Training Platform 1 through 12.</returns>
		public bool IsTrainingPlatform() => (ObjectType >= 58 && ObjectType <= 69);
		/// <summary>Gets if this craft is the first Training Platform Object.</summary>
		/// <returns><b>true</b> if <see cref="ObjectType"/> is Training Platform 1.</returns>
		public bool IsStartingGate() => (ObjectType == 58);
		/// <summary>Gets an array for the Training Platform's gun layout as stored in <see cref="BaseFlightGroup.Formation"/>.</summary>
		/// <returns>An arry where each value of <b>true</b> denotes a gun that is present.</returns>
		/// <remarks>Array length is 6.</remarks>
		public bool[] PlatformBitfieldUnpack()
        {
            bool[] ret = new bool[6];
            int platIndex = ObjectType - 58;  //Translate object so that Training Platform 1 is index[0];
            if (platIndex >= 1 && platIndex < 12)  //Index[0] doesn't have any guns.
            {
                char[] mapChar = Strings.PlatformBitField[platIndex].ToCharArray();
                for (int i = 0; i < 6; i++)
                {
                    int bit = 1 << i;
                    int gunIndex = mapChar[i] - '1';
                    if (gunIndex >= 0 && gunIndex < 6)
                        ret[gunIndex] = ((Formation & bit) != 0);
                }
            }
            return ret;
        }
		/// <summary>Creates the raw value for the Training Platform's gun layout to be stored in <see cref="BaseFlightGroup.Formation"/>.</summary>
		/// <param name="arr">An arry where each value of <b>true</b> denotes a gun that is present.</param>
		/// <returns>The new value for <see cref="BaseFlightGroup.Formation"/>.</returns>
		public int PlatformBitfieldPack(bool[] arr)
        {
			//TODO XW: needs error checking to prevent exceptions or corruption
            Formation = 0;
            int platIndex = ObjectType - 58;  //Translate object so that Training Platform 1 is index[0];
            if (platIndex >= 1 && platIndex < 12)  //Index[0] doesn't have any guns.
            {
                char[] mapChar = Strings.PlatformBitField[platIndex].ToCharArray();
                for (int i = 0; i < 6; i++)
                {
                    int gunIndex = mapChar[i] - '1';
                    if (gunIndex >= 0 && gunIndex < 6)
                        Formation += (byte)(Convert.ToByte(arr[gunIndex]) << i);
                }
            }
            return Formation;
        }
		#endregion functions

		#region properties
		/// <summary>Gets or sets the Special Craft number.</summary>
		/// <remarks>Redefined as <i>short</i>, no index processing.</remarks>
		new public short SpecialCargoCraft { get; set; }

		/// <summary>Gets or sets the raw delay value.</summary>
		/// <remarks>Arrival delay is encoded strangely. If delay &lt;= 20, it's just minutes.
		/// If delay &gt; 20, minutes are every 20 units, and the remainder between determines seconds (6 seconds per unit).<br/>
		/// Examples:<br/>
		///  Delay=5   is 5 minutes, 0 seconds.<br/>
		///  Delay=21  is 0 minutes, 6 seconds.<br/>
		///  Delay=42  is 1 minute, 12 seconds.<br/>
		///  Delay=63  is 2 minutes, 18 seconds, etc.</remarks>
		public short ArrivalDelay { get; set; }
		/// <summary>Gets or sets the Arrival Trigger.</summary>
		/// <remarks>Values are per <see cref="Strings.Trigger"/>.</remarks>
		public short ArrivalEvent { get; set; }
		/// <summary>Gets or sets the Arrival Trigger target.</summary>
		/// <remarks>Use <b>-1</b> for "None".</remarks>
		public short ArrivalFG { get; set; }
		/// <summary>Gets or sets the FG's arrival/departure ship.</summary>
		/// <remarks>Use <b>-1</b> for "None".</remarks>
		public short Mothership { get; set; }
		/// <summary>Gets or sets the arrival method.</summary>
		/// <remarks>Use <b>01</b> for "Hyperspace", <b>00</b> "for Mothership".</remarks>
		public short ArrivalHyperspace { get; set; }
		/// <summary>Gets or sets the departure method.</summary>
		/// <remarks>Use <b>01</b> for "Hyperspace", <b>00</b> "for Mothership".</remarks>
		public short DepartureHyperspace { get; set; }
		/// <summary>Gets or sets the order's parameter.</summary>
		/// <remarks>For docking orders, this is "Docking Time" in minutes, otherwise it's "Throttle" for patrol/circle orders.</remarks>
		public short DockTimeThrottle { get; set; }
		/// <summary>Gets or sets the FG Goal.</summary>
		public short Objective { get; set; }
		/// <summary>Gets or sets the craft's Object type.</summary>
		public short ObjectType { get; set; }
		/// <summary>Gets or sets the FG's primary order.</summary>
		public short Order { get; set; }

		//public short ObjFormObjective { get; set; } //looks like this isn't used, read into Formation instead

		/// <summary>Gets or sets the first target FG.</summary>
		/// <remarks>Use <b>-1</b> for "None".</remarks>
		public short TargetPrimary { get; set; }
		/// <summary>Gets or sets the second target FG.</summary>
		/// <remarks>Use <b>-1</b> for "None".</remarks>
		public short TargetSecondary { get; set; }
		/// <summary>Gets or sets if the FlightGroup responds to player's orders</summary>
		public bool FollowsOrders { get; set; }
		/// <summary>Gets if the FlightGroup is created within 30 seconds of mission start.</summary>
		/// <remarks>Looks for blank Arrival triggers and a delay of 30 seconds or less.</remarks>
		public bool ArrivesIn30Seconds
        {
            get
            {
                /* [JB] The XWI format uses a single short to encode the arrival time.
                 * ArrivalDelayMinutes and ArrivalDelaySeconds are not actually used.
                 * */

                ArrivalDelayMinutes = (byte)GetArrivalMinutes();
                ArrivalDelaySeconds = (byte)GetArrivalSeconds();
                return (ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30);
            }
        }
		/// <summary>Gets the Waypoint array used to determine starting location and AI pathing.</summary>
		/// <remarks>Array length is 15, use <see cref="WaypointIndex"/> for indexes.</remarks>
		public Waypoint[] Waypoints => _waypoints;
		#endregion properties
	}
}