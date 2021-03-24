Idmr.Platform.dll
=================

Author: Michael Gaisser (mjgaisser@gmail.com)
Version: 5.1
Date: 2021.03.15

Library for editing LucasArts *.TIE mission files for Xwing95, TIE95, XvT and XWA

=========
Version History

v5.2 - xx xxx xxxx
 - (XW) WaitForClick briefing event wasn't converting to Page Break, causing failures after mission conversions [YOGEME#51]

v5.1 - 15 Mar 2021
 - (XWA) Trigger And/Or values now read XWA's method of (value & 1) = TRUE. Still only writes 0/1 [Related to YOGEME#48]

v5.0 - 10 Oct 2020
 - (XWA) Changed Trim to TrimEnd for craft Name and Cargos during load, as there's the potential for leading \0 which would keep the rest of the string
 - Mostly XvT-related updates by Random Starfighter (JB)
 -- Mission.RndSeed discovered
 -- FlightGroup.Unknown2 is now StopArrivingWhen enum
 -- FlightGroup.Unknown3 is now RandomArrivalDelayMinutes
 -- FlightGroup.Unknown4 is now RandomArrivalDelaySeconds
 -- FG Goals discovered to have an array of Enabled values, not just the one followed by a Team value (which happened to just work for SP missions). Consumes Unk10-15.
 -- Documentation updates to Mission_XvT.txt for some of the finer workings of some triggers and conditions
 -- (XWA) Global Group references in Orders and Triggers prepped for string replacement, similar to FGs and Teams.
 
v4.0 - 09 Aug 2020
 - A lot of arrays and such were changed to use auto-properties, many of them with private set
 - A lot of other arrays and objects were set to readonly
 - (TIE.Flightgroup) PermaDeath changed to bool
 - (TIE.Strings) IFF numbers removed from defaults
 - (XWA.Mission) Iffs renamed to IFFs
 - (XWA.Mission.Trigger) IFF substitution implemented
 - (Xwing.Briefings) IsVisible() removed
 - (Xwing.Briefing) EventCount() removed
 - (Xwing.Briefing) Visible renamed to IsVisible
 - (Xwing.Briefing) public fields have PascalCase applied
 - (Xwing.Briefing) EventMapper now private readonly static _eventMapper
 - Lots of fixes and tweaks by Random Starfighter (JB)
 -- (BaseMessage) Message length increased to 64 from 63
 -- (BaseStrings) FormationMine added, ShipClass and ObjectType updated
 -- (*.Order) SafeString implementated
 -- (*.Strings) Ability to reset CraftType and CraftAbbrv to defaults
 -- (*.Trigger) SafeString implementated
 -- (*.Trigger) ToString() now prevents "of of"
 -- (*.Mission) Better Save backup
 -- (*.Mission) Message load null term fixed
 -- (Tie.Mission) Handling to load incomplete briefing questions
 -- (XvT.FlightGroup.Order) TriggerType expanded
 -- (XvT.Mission) Unknown4 and 5 removed, part of new IFF names
 -- (XvT.Mission) Unknown6 renamed to PreventMissionOutcome
 -- (XvT.Mission.IffNameIndexer) New
 -- (XvT.Mission.Trigger) TriggerType expanded
 -- (XvT.Strings) Various updates to throughout
 -- (XWA.Strings) RoleTeams updated
 -- (Xwing.Flightgroup) Raw values for Pitch/Yaw/Roll instead of degrees
 -- (Xwing.Flightgroup) Internal string length increased
 -- (Xwing.Mission) Unknown1 renamed to RndSeed
 -- (Xwing.Mission) Fixed Yaw/Pitch being flipped during save
 -- (Xwing.Mission) FlightGroup limit increased to 255
 -- (Xwing.Mission) MessageLimit decreased to 0
 -- (Xwing.Strings) FormationObject updated

 
v3.1 - 03 Jul 2020
 - (*.Mission) Added backup during save
 - (XWA.Strings) Orbit order details
 - (*.Strings) More details to OverrideShipList length exception
 - (Xwing.Strings) Added OverrideShipList
 - (XWA.Strings) Added missing craft entry to shiplist
 

v3.0.1 - 19 Sep 2018
 - (*.Mission) Fixed Pitch value check during write
 - (Xwing.Mission) Add object angle conversion to/from degrees

v3.0 - 03 Sep 2018
 - Lots of fixes, tweaks and new features by Random Starfighter (JB)
 -- Xwing95 platform support
 -- helper functions throughout for FG/Message move/delete
 -- (All) EditorCraftNumber and DifficultyAbbrv support
 -- (BaseBriefing) EventParamterCount now a virtual function
 -- (BaseBriefing) virtual helper functions added
 -- (BaseStrings) some rewording
 -- (BaseStrings) SafeString function
 -- (Converter) capped TIE AI level
 -- (Converter) fixed errors in TIE PlayerCraft, TIE Briefing.Events, XWA MessageDelay
 -- (*.FlightGroup) ToString() updated
 -- (*.Mission) updated string encodings
 -- (*.Mission) fixed a null exception during fs.Close()
 -- (*.Strings) some rewording
 -- (Tie.FlightGroup) added Campaign perma-death, formerly Unk9 and Unk10
 -- (Tie.Mission) added EndOfMissionColor
 -- (Tie.Mission) fixed read/write of highlighting in officer questions
 -- (Tie.Strings) added missing CraftWhen values
 -- (Xvt.Briefing) removed team initialization
 -- (Xvt.BriefingCollection) improved team initialization
 -- (Xvt.FlightGroup) added Energy Beam, Ion Pulse, Cluster Mine
 -- (Xvt FlightGroup) added PreventCraftNumbering, DepartureClockMinutes, DepartureClockSeconds, formerly Unk 19, 20, 21
 -- (Xvt.FlightGroup) Skip trigger default to TRUE
 -- (Xvt.FlightGroup.Goal) TimeLimit, formerly Unk16
 -- (Xvt.FlightGroup.LoadoutIndexer) added IonPulse, EnergyBeam and ClusterMine
 -- (Xvt.FlightGroup.Order) added "All IFFs except" target type
 -- (Xvt.Mission) Renamed MissionType.MPMelee
 -- (Xvt.Mission) fixed Departure and Arrival2 R/W, other R/W corrections to match format updates
 -- (Xvt.Mission) Mission Succ/Fail/Desc load changed to TrimEnd
 -- (Xvt.Mission) YOGEME signature moved to within MissionDescription instead of "outside" format
 -- (Xvt.Strings) added RoleTeams, Difficulty and DIfficultyAbbrv arrays, EnergyBeam value
 -- (Xwa.Briefing) removed team initialization
 -- (Xwa.BriefingCollection) improved team initialization
 -- (Xwa.FlightGroup) added Energy Beam, Cluster Mine
 -- (Xwa.FlightGroup) Designations are disabled by default
 -- (Xwa.FlightGroup.Goal) Unk43 added
 -- (Xwa.FlightGroup.LoadoutIndexer) Energy Beam, Ion Pulse, Cluster Mine added/implemented
 -- (Xwa.FlightGroup.Order) now default to TRUE
 -- (Xwa.FlightGroup.Order) time trigger added
 -- (Xwa.FlightGroup.Order) SafeString() implemented
 -- (Xwa.FlightGroup.Order) helper functions for Skip triggers
 -- (Xwa.Message) updated to Delay
 -- (Xwa.Mission) R/W corrections to match format updates
 -- (Xwa.Mission) mission strings changed to TrimEnd on read
 -- (Xwa.Mission) YOGEME signature moved to within MissionDescription instead of "outside" format
 -- (Xwa.Mission) added GetDelaySeconds()
 -- (Xwa.Mission) FlightGroupLimit increased to 192
 -- (Xwa.Mission.Trigger) SafeString() implemented
 -- (Xwa.Mission>Trigger) delay calculation fixed
 -- (Xwa.Strings) RoleTeams added
 -- (Xwa.Strings) order docking text updated
 - (*.Strings) "at least 1" changed to "any"
 - (Tie.Mission) additional cleanup of officer question read/write
 - (Xwa.Strings) removed "?" from "be delievered"
 

v2.7 - 09 May 2018
 - (XWA.FlightGroup.Goal) Proximity triggers include distance in ToString
 - (XWA.FlightGroup.Orders) TriggerType unknowns filled in (#1)
 - (XWA.Globals.Goal) Proximity triggers include distance in ToString
 - (XWA.Mission.Trigger) Proximity triggers include distance in ToString
 - (XWA.Mission.Trigger) TriggerType unknowns filled in (#1)
 - (XWA.Mission) FlightGroupLimit was raised to 132 for the time being, this is post-SuperBackDrops install to prevent errors
 - (XWA.Strings) TriggerType unknowns filled in (#1)
 - (XWA.Strings) Escort order had "Meaningless" replaced with "Position"

v2.6.2 - 24 Feb 2018
 - (XWA.FlightGroup.Orders) Fixed the waypoint inversion (YOGEME#16)

v2.6.1 - 18 Nov 2017
 - (XWA.FlightGroup) Added some Backdrop-specific properties that alias the appropriate standard properties.

v2.6 - 15 Oct 2017
 - (TIE.Strings) Added missing "*" from Med Trans
 - (*.Strings) Added ability to replace craft list (YOGEME#10)

v2.5 - 7 Jan 2017
 - Lots of fixes and new features by Random Starfighter (JB)
  -- (*.Mission) Enforced string encodings
  -- (TIE.FlightGroup.Order) Hack for _checkValues added
  -- (TIE.Mission) Added Message length check during load
  -- (TIE.Mission) Fixed Global Goal loading
  -- (TIE.Strings) Added Decoy Beam
  -- (XvT.Briefing) Added Team functionality
  -- (XvT.FlightGroup.Goal) Fixed points casts
  -- (XvT.FlightGroup.Goal) Added Team visiblity
  -- (XvT.Mission) Fixed craft options
  -- (XvT.Mission) Added Message length check during load
  -- (XvT.Mission) Fixed FG Unks
  -- (XvT.Mission) Fixed Message Color writing
  -- (XvT.Mission) Fixed Team writing
  -- (XvT.Mission) Fixed Briefing Team read/write
  -- (XvT.Strings) Added empty CraftWhen entries
  -- (XvT.Strings) Added "each special craft" amount
  -- (XWA.FlightGroup) Added Ion Pulse warhead
  -- (XWA.FlightGroup.Goal) Updated ToString
  -- (XWA.Mission) Fixed Unk3 initialization
  -- (XWA.Mission) Fixed craft Options
  -- (XWA.Strings) Added Unk Role values
  -- (XWA.Strings) Added Unk Amount values

v2.4 - 6 June 2016
 - (Converter) Fixed XWA to TIE player's craft conversion
 - (All) Inverted the Y axis on all waypoints during read/write so in-game map and editor appearance match, but editor is still "positive up"
 - Fixed various references in comments
 
v2.3 - 5 Apr 2015
 - (BaseTrigger) Added TriggerIndex enum
 - (XvT.Globals.Goal) set to Serializable
 - (XvT.Globals.Goal) Deleted GoalStringIndexer, strings wrapped into new subclass, Trigger
 
v2.2 - 5 Feb 2015
 - (Team.*) set classes to Serializable (YOGEME Issue #8)
 - (BaseTrigger) set to Serializable (YOGEME Issue #5)
 
v2.1 - 14 Dec 2014
 - Converted license to MPLv2.0
 - (*.FlightGroupCollection) SetCount and IsModified implementation
 - (*.MessageCollection) SetCount and IsModified implementation
 - Couple other minor fixes that have been sitting here for a while...
 
v2.0.1 - 14 Aug 2012
 - (BaseBriefing) Fixed bug regarding StartLength calculation
 - (*.FlightGroup) Fixed bug preventing proper SpecialCargoCraft handling during Load/Save
 - (Tie.FlightGroup) Added Unknowns.Unknown19, 20 and 21
 - (Tie.Officers) Fixed bug in Save() causing '[' and ']' to save as characters instead of the appropriate highlighiting codes
 - (Xvt.Briefing) Fixed bug in Save() preventing proper Event writing
 - (Xvt.Mission) Fixed critical bug in LoadMission() that resulted in unhandled exception
 - (Xwa.Mission) Fixed critical bug in Save() causing infinite loop and filesize
 
v2.0 - 25 May 2012
 - (BaseBriefing) Added EventParameterCount
 - (BaseBriefing) Events[] is now short[]
 - (BaseFlightGroup.BaseOrder) New
 - (BaseFlightGroup.BaseWaypoint) New
 - (BaseTrigger) Inherits Indexer<byte>
 - (*.FlightGroup) Added ToString() overrides
 - (*.FlightGroup.Goal) Added ToString() override, exceptions
 - (*.FlightGroup.Goal) Inherits Indexer<byte>
 - (*.FlightGroup.LoadoutIndexer) Inherits Indexer<bool>
 - (*.FlightGroup.Order) Added ToString() overrides, exceptions, conversions
 - (*.FlightGroup.Order) Inherits BaseFlightGroup.BaseOrder
 - (*.FlightGroup.Waypoint) Added conversions
 - (*.FlightGroup.Waypoint) Inherits BaseFlightGroup.BaseWaypoint
 - (*.FlightGroupCollection) Added GetList()
 - (*.Mission) Deleted NumFlightGroups, NumMessages
 - (*.Mission) Inherits MissionFile
 - (*.Mission.Trigger) Added conversions, exceptions, ToString() overrides
 - (*.TeamCollection) Added GetList()
 - (Tie.FlightGroup) Radio renamed to FollowsOrders
 - (Tie.FlightGroup.FGGoals) Added exceptions
 - (Tie.FlightGroup.FGGoals) Inherits Indexer<byte>
 - (Tie.Mission) Added CraftCheck(), CheckTarget()
 - (Tie.Mission) IffHostile inherits Indexer<bool>
 - (Tie.Mission) EndOfMissionMessages inherits Indexer<string>
 - (Tie.Mission.IffNameIndexer) Inherits Indexer<string>
 - (Xvt.FlightGroup) Roles is now Indexer<string>
 - (Xvt.Globals.Goal) Removed AndOrIndexer
 - (Xvt.Globals.Goal) AndOr is now bool[]
 - (Xvt.Mission) Added CraftCheck(), CheckTarget(), MissionTypeEnum
 - (Xvt.Mission) BoP renamed to IsBop
 - (Xvt.Strings) Removed MissType
 - (Xvt.Team) EndOfMissionMessages is now Indexer<string>
 - (Xwa.Briefing) Removed Team.set{}
 - (Xwa.Briefing) Added BriefingStringNotes
 - (Xwa.FlightGroup.Waypoint) Added ToString() override
 - (Xwa.Mission) Added LogoEnum, HangarEnum, *Notes
 - (Xwa.Mission) GlobalGroups, Regions, Iffs are now Indexer<string>
 - (Xwa.Mission) Hangar renamed to MissionType
 - (Xwa.Mission.Trigger) ctor(byte[]) now accepts Length = 4
 - (Xwa.Mission.Strings) Removed Logo, Hangar
 - Back-end updates

==========
Additional Information

Idmr.Common.dll (v1.1 or later) is a required reference

Mission*.txt files contain file structure information per platform

Programmer's reference can be found in help/Idmr.Platform.chm

==========
Copyright Information

Copyright © 2009-2020 Michael Gaisser
This library file and related files are licensed under the Mozilla Public License
v2.0 or later.  See MPL.txt for further details.

The Galactic Empire: Empire Reborn is Copyright © 2004- Tiberius Fel

"Star Wars" and related items are trademarks of LucasFilm Ltd and
LucasArts Entertainment Co.

THESE FILES HAVE BEEN TESTED AND DECLARED FUNCTIONAL BY THE IDMR, AS SUCH THE
IDMR AND THE GALACTIC EMPIRE: EMPIRE REBORN CANNOT BE HELD RESPONSIBLE OR
LIABLE FOR UNWANTED EFFECTS DUE ITS USE OR MISUSE. THIS SOFTWARE IS OFFERED AS
IS WITHOUT WARRANTY OF ANY KIND.