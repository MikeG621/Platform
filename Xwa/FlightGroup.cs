/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2020 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 4.0
 */

/* CHANGELOG
 * v4.0, 200809
 * [UPD] auto-properties
 * v3.0, 180903
 * [UPD] Energy Beam and Cluster Mine added [JB]
 * [UPD] disable Designations on init [JB]
 * [UPD] added info to ToString() [JB]
 * [NEW] helper functions for dete/swap FGs [JB]
 * v2.6.1, 171118
 * [ADD] backdrop-specific properties
 * v2.5, 170107
 * [ADD] Ion Pulse
 * v2.1, 141214
 * [UPD] change to MPL
 * v2.0, 120525
 * [NEW] ToString() overrides
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xwa
{
	/// <summary>Object for individual FlightGroups</summary>
	[Serializable]
	public partial class FlightGroup : BaseFlightGroup
	{
		// offsets are local within FG
		string _role = "";
		readonly bool[] _optLoadout = new bool[18]; //[JB] Added Energy Beam and Cluster Mine
		string _pilotID = "";

		/// <summary>Indexes for <see cref="ArrDepTriggers"/></summary>
		public enum ArrDepTriggerIndex : byte
		{
			/// <summary>First Arrival trigger</summary>
			Arrival1,
			/// <summary>Second Arrival trigger</summary>
			Arrival2,
			/// <summary>Third Arrival trigger</summary>
			Arrival3,
			/// <summary>Fourth Arrival trigger</summary>
			Arrival4,
			/// <summary>First Departure trigger</summary>
			Departure1,
			/// <summary>Second Departure trigger</summary>
			Departure2
		}
		/// <summary>Values for <see cref="OptCraftCategory"/></summary>
		public enum OptionalCraftCategory : byte
		{
			/// <summary>No other available craft</summary>
			None,
			/// <summary>All craft set to 'Flyable' are available</summary>
			AllFlyable,
			/// <summary>All Rebel craft set to 'Flyable' are available</summary>
			AllRebelFlyable,
			/// <summary>All Imperial craft set tp 'Flyable' are available</summary>
			AllImperialFlyable,
			/// <summary>Available craft are determined by <see cref="OptCraft"/></summary>
			Custom
		}

		/// <summary>Initializes a new FlightGroup</summary>
		/// <remarks>All <see cref="Orders"/> set to <b>100%</b> <see cref="BaseFlightGroup.BaseOrder.Throttle"/> (16 total, 4 per Region), <see cref="Goals"/> set to <b>NONE</b>, SP1 <b>Enabled</b>, most <see cref="Unknowns"/> <b>0/false</b>, <see cref="UnknownValues.Unknown1"/> to <b>2</b></remarks>
		public FlightGroup()
		{
			for (int i = 0; i < 6; i++) ArrDepTriggers[i] = new Mission.Trigger();
			for (int i = 0; i < 4; i++) Waypoints[i] = new Waypoint();
			for (int i = 0; i < 16; i++) Orders[i / 4, i % 4] = new Order();
			for (int i = 0; i < 8; i++) Goals[i] = new Goal();
			_optLoadout[0] = true;
            _optLoadout[9] = true;  //[JB] Adjusted these indexes for added Energy Beam, Cluster Mine
			_optLoadout[14] = true;
			Waypoints[0].Enabled = true;
			Unknowns.Unknown1 = 2;
			OptLoadout = new LoadoutIndexer(_optLoadout);
            EnableDesignation1 = 255; //[JB] Set to disabled
            EnableDesignation2 = 255;
		}

		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <returns>Short representation of the FlightGroup as <b>"<see cref="Strings.CraftAbbrv"/>.<see cref="BaseFlightGroup.Name"/>"</b></returns>
		public override string ToString() { return ToString(false); }
		/// <summary>Gets a string representation of the FlightGroup</summary>
		/// <remarks>Parenthesis indicate "if applicable" fields, doubled (( )) being "if applicable" and include literal parenthesis.<br/>
		/// Short form is <b>"<see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (<see cref="BaseFlightGroup.EditorCraftNumber"/>)"</b><br/><br/>
		/// Long form is <b>"<see cref="Team"/> - <see cref="BaseFlightGroup.GlobalGroup">GG</see> - (IsPlayer * indicator)
		/// <see cref="BaseFlightGroup.NumberOfWaves"/> x <see cref="BaseFlightGroup.NumberOfCraft"/> 
		/// <see cref="Strings.CraftAbbrv"/> <see cref="BaseFlightGroup.Name"/> (&lt;<see cref="BaseFlightGroup.EditorCraftNumber"/>&gt;) ((<see cref="GlobalUnit"/>))
		/// ([(Plr: <see cref="PlayerNumber"/>) ("hu" if <see cref="ArriveOnlyIfHuman"/>)])"</b></remarks>
		/// <param name="verbose">When <b>true</b> returns long form</param>
		/// <returns>Representation of the FlightGroup</returns>
		public string ToString(bool verbose)
		{
			string longName = Strings.CraftAbbrv[CraftType] + " " + Name;
            if (EditorCraftNumber > 0) //[JB] Added numbering information.
                longName += EditorCraftExplicit ? " " + EditorCraftNumber : " <" + EditorCraftNumber + ">";
            if (!verbose) return longName;

            //[JB] Added player position and difficulty tags, by feature request.
            string plr = "";
            if (PlayerNumber > 0 || ArriveOnlyIfHuman == true)
            {
                if (PlayerNumber > 0) plr = "Plr:" + PlayerNumber;
                if (ArriveOnlyIfHuman) plr += (plr.Length > 0 ? " " : "") + "hu";
                if (plr.Length > 0) plr = " [" + plr + "]";
            }
            string ret = (Team + 1) + " - " + GlobalGroup + " - " + (PlayerNumber != 0 ? "*" : "") + NumberOfWaves + "x" + NumberOfCraft + " " + longName + (GlobalUnit != 0 ? " (" + GlobalUnit + ")" : "") + plr;
            if (Difficulty >= 1 && Difficulty <= 6)
                ret += " [" + Strings.DifficultyAbbrv[Difficulty] + "]";

            return ret;
		}
        /// <summary>Changes all Flight Group indexes, to be used during a FG Swap (Move) or Delete operation.</summary>
        /// <remarks>FG references may exist in other mission properties, ensure those properties are adjusted too.</remarks>
        /// <param name="srcIndex">The FG index to match and replace (Move), or match and Delete.</param>
        /// <param name="dstIndex">The FG index to replace with.  Specify -1 to Delete, or zero or above to Move.</param>
        /// <returns>True if something was changed.</returns>
        public bool TransformFGReferences(int srcIndex, int dstIndex)
        {
            bool change = false;
            bool delete = false;
            byte dst = (byte)dstIndex;
            if (dstIndex < 0)
            {
                dst = 0;
                delete = true;
            }

            //If the FG matches, replace (and delete if necessary).  Else if our index is higher and we're supposed to delete, decrement index.
            if (ArrivalCraft1 == srcIndex) { change = true; ArrivalCraft1 = dst; if (delete) { ArrivalMethod1 = false; } } else if (ArrivalCraft1 > srcIndex && delete == true) { change = true; ArrivalCraft1--; }
            if (ArrivalCraft2 == srcIndex) { change = true; ArrivalCraft2 = dst; if (delete) { ArrivalMethod2 = false; } } else if (ArrivalCraft2 > srcIndex && delete == true) { change = true; ArrivalCraft2--; }
            if (DepartureCraft1 == srcIndex) { change = true; DepartureCraft1 = dst; if (delete) { DepartureMethod1 = false; } } else if (DepartureCraft1 > srcIndex && delete == true) { change = true; DepartureCraft1--; }
            if (DepartureCraft2 == srcIndex) { change = true; DepartureCraft2 = dst; if (delete) { DepartureMethod2 = false; } } else if (DepartureCraft2 > srcIndex && delete == true) { change = true; DepartureCraft2--; }
            for (int i = 0; i < ArrDepTriggers.Length; i++)
            {
                Mission.Trigger adt = ArrDepTriggers[i];
                change |= adt.TransformFGReferences(srcIndex, dstIndex, true);  //First 2 are arrival.  Set those to true.
            }

            //Skip triggers are part of the Order in XWA, don't need to handle them here.

            foreach (FlightGroup.Order order in Orders)
                change |= order.TransformFGReferences(srcIndex, dstIndex);
            return change;
        }
        /// <summary>Changes all Message indexes, to be used during a Message Swap (Move) or Delete operation.</summary>
        /// <remarks>Same concept as for Flight Groups.  Triggers may depend on Message indexes, and this function helps ensure indexes are not broken.</remarks>
        /// <param name="srcIndex">The Message index to match and replace (Move), or match and Delete.</param>
        /// <param name="dstIndex">The Message index to replace with.  Specify -1 to Delete, or zero or above to Move.</param>
        /// <returns>True if something was changed.</returns>
        public bool TransformMessageReferences(int srcIndex, int dstIndex)
        {
            bool change = false;

            for (int i = 0; i < ArrDepTriggers.Length; i++)
            {
                Mission.Trigger adt = ArrDepTriggers[i];
                change |= adt.TransformMessageRef(srcIndex, dstIndex);
            }

            foreach(FlightGroup.Order order in Orders)
            {
                foreach(Mission.Trigger trig in order.SkipTriggers)
                    change |= trig.TransformMessageRef(srcIndex, dstIndex);
            }

            return change;
        }
		
		#region craft
		/// <summary>Determines if <see cref="Designation1"/> is used, and which teams it applies to.</summary>
		public byte EnableDesignation1 { get; set; }   //[JB] Changed designations from bool to byte, since it usually stores teams.
        /// <summary>Determines if <see cref="Designation2"/> is used, and which teams it applies to.</summary>
		public byte EnableDesignation2 { get; set; }
		/// <summary>Primary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation1 { get; set; }
		/// <summary>Secondary craft role, such as Command Ship or Strike Craft. Also used for Hyper Buoys</summary>
		public byte Designation2 { get; set; }
		/// <summary>The <see cref="Mission.GlobalCargo"/> index to be used instead of <see cref="BaseFlightGroup.Cargo"/></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalCargo { get; set; }
		/// <summary>The <see cref="Mission.GlobalCargo"/> index to be used instead of <see cref="BaseFlightGroup.SpecialCargo"/></summary>
		/// <remarks>One-indexed, zero is "N/A". Corrected from/to zero-indexed during Load/Save</remarks>
		public byte GlobalSpecialCargo { get; set; }
		/// <summary>Gets or sets a text form of the craft role</summary>
		/// <remarks>19 character limit, appears to be an editor note</remarks>
		public string Role
		{
			get => _role;
			set => _role = StringFunctions.GetTrimmed(value, _stringLength);
		}
		/// <summary>Gets or sets the allegiance value that controls goals and IFF behaviour</summary>
		public byte Team { get; set; }
		/// <summary>Gets or sets team or player number to which the craft communicates with</summary>
		public byte Radio { get; set; }
		/// <summary>Gets or sets if the craft has a human or AI pilot</summary>
		/// <remarks>Value of <b>zero</b> defined as AI-controlled craft. Human craft will launch as AI-controlled if no player is present</remarks>
		public byte PlayerNumber { get; set; }
		/// <summary>Gets or sets if craft is required to be player-controlled</summary>
		/// <remarks>When <b>true</b>, craft with PlayerNumber set will not appear without a human player</remarks>
		public bool ArriveOnlyIfHuman { get; set; }

		/// <summary>Gets or sets the RGB values of the Backdrop</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.Name"/>.</remarks>
		public string LightRGB
		{
			get => Name;
			set => Name = value;
		}
		/// <summary>Gets or sets the Shadow value of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="GlobalCargo"/>, does not have error-checking.</remarks>
		public byte Shadow
		{
			get => GlobalCargo;
			set => GlobalCargo = value;
		}
		/// <summary>Gets or sets the numerical Brightness value of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.Cargo"/>.</remarks>
		public string Brightness
		{
			get => Cargo;
			set => Cargo = value;
		}
		/// <summary>Gets or sets the numerical Size of the Backdrop.</summary>
		/// <remarks>Mapped to <see cref="BaseFlightGroup.SpecialCargo"/>.</remarks>
		public string BackdropSize
		{
			get => SpecialCargo;
			set => SpecialCargo = value;
		}
		#endregion
		#region arrdep
		/// <summary>Returns <b>true</b> if the FlightGroup is created within 30 seconds of mission start</summary>
		/// <remarks>Looks for a blank trigger and a delay of 30 seconds or less</remarks>
		public bool ArrivesIn30Seconds { get { return (ArrDepTriggers[0].Condition == 0 && ArrDepTriggers[1].Condition == 0 && ArrDepTriggers[2].Condition == 0 && ArrDepTriggers[3].Condition == 0 && ArrivalDelayMinutes == 0 && ArrivalDelaySeconds <= 30); } }
		/// <summary>Gets the Arrival and Departure trigger array</summary>
		/// <remarks>Use <see cref="ArrDepTriggerIndex"/> for indexes</remarks>
		public Mission.Trigger[] ArrDepTriggers { get; } = new Mission.Trigger[6];
		/// <summary>Gets which <see cref="ArrDepTriggers"/> must be completed</summary>
		/// <remarks>Array is {Arr1AOArr2, Arr3AOArr4, Arr12AOArr34, Dep1AODep2}</remarks>
		public bool[] ArrDepAndOr { get; } = new bool[4];
		#endregion
		/// <summary>Gets the FlightGroup objective commands</summary>
		/// <remarks>Array is [Region, OrderIndex]</remarks>
		public Order[,] Orders { get; } = new Order[4, 4];
		/// <summary>Gets the FlightGroup-specific mission goals</summary>
		/// <remarks>Array is Length = 8</remarks>
		public Goal[] Goals { get; } = new Goal[8];

		/// <summary>Gets the FlightGroup start location markers</summary>
		/// <remarks>Array is length 4</remarks>
		public Waypoint[] Waypoints { get; } = new Waypoint[4];
		#region Unks and Option
		/// <summary>Determines if the FlightGroup should share numbering across the <see cref="GlobalUnit"/></summary>
		public bool GlobalNumbering { get; set; }
		/// <summary>Gets or sets the defenses available to the FG</summary>
		public byte Countermeasures { get; set; }
		/// <summary>Gets or sets the duration of death animation</summary>
		/// <remarks>Unknown multiplier, appears to react differently depending on craft class</remarks>
		public byte ExplosionTime { get; set; }
		/// <summary>Gets or sets the second condition of FlightGroup upon creation</summary>
		public byte Status2 { get; set; }
		/// <summary>Gets or sets the additional grouping assignment, can share craft numbering</summary>
		public byte GlobalUnit { get; set; }
		/// <summary>Gets the array of alternate weapons the player can select</summary>
		/// <remarks>Use the <see cref="LoadoutIndexer.Indexes"/> enumeration for indexes</remarks>
		public LoadoutIndexer OptLoadout { get; private set; }
		/// <summary>Gets the array of alternate craft types the player can select</summary>
		/// <remarks>Array is Length = 10</remarks>
		public OptionalCraft[] OptCraft { get; } = new OptionalCraft[10];
		/// <summary>The alternate craft types the player can select by list</summary>
		public OptionalCraftCategory OptCraftCategory = OptionalCraftCategory.None;

		/// <summary>Gets or sets the editor note primarily used to signifiy the pilot voice</summary>
		/// <remarks>15 character limit</remarks>
		public string PilotID
		{
			get => _pilotID;
			set => _pilotID = StringFunctions.GetTrimmed(value, 15);
		}
		/// <summary>For Backdrop <see cref="BaseFlightGroup.CraftType">CraftTypes</see>, is the backdrop image</summary>
		/// <remarks>Zero denotes a light source</remarks>
		public byte Backdrop { get; set; }
		/// <summary>The unknown values container</summary>
		/// <remarks>All values except <see cref="UnknownValues.Unknown1"/> initialize to <b>0</b> or <b>false</b>, Unknown1 initializes to <b>2</b>. <see cref="Orders"/> contain Unknown9-14, <see cref="Goals"/> contain Unknown15</remarks>
		public UnknownValues Unknowns;
		#endregion
		
		/// <summary>Container for the optional craft settings</summary>
		[Serializable] public struct OptionalCraft
		{
			/// <summary>The Craft Type</summary>
			public byte CraftType { get; set; }
			/// <summary>The number of ships per Wave</summary>
			public byte NumberOfCraft { get; set; }
			/// <summary>The number of waves available</summary>
			public byte NumberOfWaves { get; set; }
		}
		
		/// <summary>Container for unknown values</summary>
		[Serializable] public struct UnknownValues
		{
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0018, in Craft section</remarks>
			public byte Unknown1 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x007B, in Craft section</remarks>
			public byte Unknown3 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0084, in Craft section</remarks>
			public byte Unknown4 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0087, in Arr/Dep section</remarks>
			public byte Unknown5 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0097, in Arr/Dep section</remarks>
			public bool Unknown6 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00BF, in Arr/Dep section</remarks>
			public byte Unknown7 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x00C0, in Arr/Dep section</remarks>
			public byte Unknown8 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAE, in Unks/Options section</remarks>
			public byte Unknown16 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DAF, in Unks/Options section</remarks>
			public byte Unknown17 { get; set; }
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB0, in Unks/Options section</remarks>
			public byte Unknown18 { get; set; }
		
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB1, in Unks/Options section</remarks>
			public byte Unknown19 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB2, in Unks/Options section</remarks>
			public byte Unknown20 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB3, in Unks/Options section</remarks>
			public byte Unknown21 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB4, in Unks/Options section</remarks>
			public bool Unknown22 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB6, in Unks/Options section</remarks>
			public byte Unknown23 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB7, in Unks/Options section</remarks>
			public byte Unknown24 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB8, in Unks/Options section</remarks>
			public byte Unknown25 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DB9, in Unks/Options section</remarks>
			public byte Unknown26 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBA, in Unks/Options section</remarks>
			public byte Unknown27 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBB, in Unks/Options section</remarks>
			public byte Unknown28 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DBC, in Unks/Options section</remarks>
			public bool Unknown29 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC0, in Unks/Options section</remarks>
			public bool Unknown30 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC1, in Unks/Options section</remarks>
			public bool Unknown31 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC5, in Unks/Options section</remarks>
			public byte Unknown32 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0DC6, in Unks/Options section</remarks>
			public byte Unknown33 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E29, in Unks/Options section</remarks>
			public bool Unknown34 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2B, in Unks/Options section</remarks>
			public bool Unknown35 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2D, in Unks/Options section</remarks>
			public bool Unknown36 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E2F, in Unks/Options section</remarks>
			public bool Unknown37 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E31, in Unks/Options section</remarks>
			public bool Unknown38 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E33, in Unks/Options section</remarks>
			public bool Unknown39 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E35, in Unks/Options section</remarks>
			public bool Unknown40 { get; set; }
			
			/// <summary>Unknown value</summary>
			/// <remarks>Offset 0x0E37, in Unks/Options section</remarks>
			public bool Unknown41 { get; set; }
		}
	}
}
