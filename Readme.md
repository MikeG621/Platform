# [Idmr.Platform.dll](https://github.com/MikeG621/Platform)

Author: [Michael Gaisser](mailto:mjgaisser@gmail.com)  
![GitHub Release](https://img.shields.io/github/v/release/MikeG621/Platform)
![GitHub Release Date](https://img.shields.io/github/release-date/MikeG621/Platform)
![GitHub License](https://img.shields.io/github/license/MikeG621/Platform)  
Contributors:
- [Random Starfighter (JB)](https://github.com/RandomStarfighter)
- [Jérémy Ansel](https://github.com/JeremyAnsel)

Library for editing LucasArts *.XWI and *.TIE mission files for Xwing95, TIE95, XvT and XWA.

## Latest Release
#### WIP
- (TIE) "Board to Destroy Cargo" now "Board with no Effect". [[YOGEME#117](https://github.com/MikeG621/YOGEME/issues/117)]
- (XWA) Defect order text now includes IFF. [[YOGEME#113](https://github.com/MikeG621/YOGEME/issues/113)]
- (Strings) MeshType added.
- (XWA) Fixes in the Order Var labels.

#### v7.0, 06 Oct 2024
- Format spec for XWA implemented, includes some backflow into TIE and XvT.
  - This means all XWA Unknowns are defined; many that were left were editor-use only or Unused.
  - Not all editor-use properties are exposed.
  - Various things renamed. ***BREAKING CHANGE***
  - ArrDep mothership properties renamed, the `*Method` bools are now `*ViaMothership` or `*MothershipUsed` per proper boolean naming convention.
- Waypoint refactor. ***BREAKING CHANGE***
  - BaseWaypoint renamed to Waypoint, no longer abstract.
  - Xwa.FlightGroup.Waypoint renamed to XwaWaypoint.
  - Xwing, TIE, and XvT now use Waypoint (formerly BaseWaypoint) directly. Previously derived classes deleted.
  - `Clone()` added.
- (TIE) Added Questions.QuestionType and .QuestionCondition enums, PostTrigger and PostTrigType types changed. ***BREAKING CHANGE***
- (Xwing) GetTIECraftType now returns `byte`.
- (Xwing) ArrDep `*Hyperspace` renamed to `*ViaHyperspace` and changed to `bool`.
- (Xwing) Added `FlightGroup.CommandList` enum
- (Briefing) EventParameters now a singleton class, `this[]` made private in lieu of `GetCount()`. ***BREAKING CHANGE***
- (Briefing) Deleted `EventParameterCount`, since it's redundant with `GetCount()`.
- (Briefing) `ConvertTicksToSeconds` and `ConvertSecondsToTicks` functions added.
- (Briefing) `Events` parameter now a collection new `Event` class objects instead of `short[]`. ***BREAKING CHANGE***
  - This causes many changes throughout. Most of the work is now done within the collection or event itself without
  requiring a lot of array manipulation.
- (TIE-XWA) `BaseFlightGroup.Difficulty` changed to new `Difficulties` enum.
- (Xwing Briefing) Couple internal changes around event conversion.
  - `short[] _eventMapper` now `EventMap[] _eventMaps`.
  - `short getEventMapperIndex()` now `EventMap getEventMapper`.
- (XWA) The TrimEnd fix from v5.0 reapplied and expanded throughout.

---
### Additional Information

#### Dependencies
- [Idmr.Common](https://github.com/MikeG621/Common) (v1.3 or later)

Mission*.txt files contain file structure information per platform.

Programmer's reference can be found in the [help file](help/Idmr.Platform.chm).

### Version History

#### v6.1, 08 Dec 2023
- (Converter) Added TieToXvt( ), TieToBop( ), TieToXvtBop( ), XvtBopToXwa( ), TieToXwa( )
  upgrade paths
- (Converter) XWA orders convert times and craft type to/from TIE and XvT
- (Converter) XvT to XWA Triggers adjust craft type
- (Converter) Added Countermeasures, ExplosionTime, GlobalUnit and Optionals to Xwa
  to Xvt Flightgroups
- (Xwing) Fixed up FlightGroup.WaypointIndex
- (TIE) Added FlightGroup.Order.CommandList, Mission.Trigger.TypeList, .AmountList and
  .ConditionList enums
- (XvT) Briefing EventQuantityLimit corrected to 200
- (XvT) Order and Trigger operators convert time to XWA properly
- (XvT) Added FlightGroup.Order.CommandList, Mission.Trigger.TypeList, .AmountList and
  .ConditionList enums
- (XWA) Added FlightGroup.Order.CommandList, Mission.Trigger.TypeList, .AmountList and
  .ConditionList enums
- (XWA) Fixed the time display for "Before Time" trigger/order target type text
- (XWA) Mission.GetDelaySeconds( ) now static ***BREAKING CHANGE***

#### v6.0, 27 Oct 2023
- (XWA) FG.Designation Unknown 0x14 renamed to "HYP from Any Region"
  [[YOGEME#91](https://github.com/MikeG621/YOGEME/issues/91)]
- (XWA) Arr/Dep Method1 changed to byte to handle value of 2,
  "HYP to region of mothership" ***BREAKING CHANGE***

#### v5.8.1, 14 Oct 2023
- (XWA) Added missing CraftWhen [[YOGEME#90](https://github.com/MikeG621/YOGEME/issues/90)]
  and defined unknown Triggers [[YOGEME#89](https://github.com/MikeG621/YOGEME/issues/89)]

#### v5.8, 04 Aug 2023
- (BaseBriefing) New "Skip Marker" event, for TIE and XvT
- (XWA) Region references in Triggers and Orders prepped for string replacement similar
  to FGs and Teams [[YOGEME#82](https://github.com/MikeG621/YOGEME/issues/82)]

#### v5.7.5, 16 Jan 2023
- (XWA) Fixed Message reading after length increased in 5.7.3
- Updates for TIE. Deleted items are those confirmed to have zero effect in the exectuable. [Issue #12]
  - "Captured on Ejection" and "Secret Goals" removed
  - Trigger "Unknown (arrive?)" now "cannon subsystem disabled"
  - Trigger Type "Craft When" fixed
  - Trigger Type "Misc" now "Adjusted AI Skill", added "Status" and "All Craft" types to match XvT
  - Status "No Lasers" now "No Turrets", everything past "Hyperdrive Added" deleted
  - Orders after "Board to Repair" deleted

#### v5.7.4, 27 Aug 2022
- (XxT) Briefing TicksPerSecond updated to 21 (0x15) instead of 20.

#### v5.7.3, 19 Jun 2022
- (XWA) Message length limit increased to 68.

#### v5.7.2, 25 Feb 2022
- (XWA) Added missing Squadron logo options in Mission.LogoEnum

#### v5.7.1, 08 Feb 2022
- (XWA) Message Trigger And/Or read now checks for 1 instead of any odd value (JB)
 
#### v5.7, 27 Jan 2022
- More work from Random Starfighter
  - (TIE) New ctors for FG.Order, Mission Trigger
  - (XvT) ctors added last rev now call the blanks prior to working
  - (XvT) Strings.OrderDesignation added
  - (XWA) New ctors for FG.Goal, FG Order, FG.Waypoint, Mission.Trigger
  - (XWA) Fixed a Message.OriginatingFG issue during deletes
  - (Xwing.Strings) Formation fixed, first is "Double Vic" and last two are Undefined.

#### v5.6, 03 Jan 2022
- (XvT) New constructors for FG.Goal, FG.Order and Mission.Trigger (JB)

#### v5.5.1, 29 Nov 2021
- (XWA.Strings) Removed "Not Identified" from Status

#### v5.5, 01 Aug 2021
- Some fixes from RandomStarfighter
  - (All) SS Patrol and SS Await Return order strings now show target info
  - (XWA) Hyper to Region order text updated with token
  - (XWA) Fixed some CraftType errors in Order and Trigger strings
   
#### v5.4, 04 Apr 2021
- (Converter) FG Goal amounts fixed when converting from XW [[YOGEME#55](https://github.com/MikeG621/YOGEME/issues/55)] (JB)
- (Converter) Fixed an exception message
 
#### v5.3, 28 Mar 2021
- Several items around X-wing Briefing conversions, most related to [YOGEME#51](https://github.com/MikeG621/YOGEME/issues/51) and [YOGEME#53](https://github.com/MikeG621/YOGEME/issues/53)
  - (Converter) Fixed Description, since it wasn't always splitting out hints properly
  - (Converter) Removed auto creation of Page Break before ClearText
  - (Converter) Removed the MoveMap multiplier in XW-XWA
  - (Converter) Skip over porting "None" events
  - (XW) ClearText event now correctly maps to Page Break, v5.2 WaitForClick conversion removed

#### v5.2 - 24 Mar 2021
- (XW) WaitForClick briefing event wasn't converting to Page Break, causing failures
  after mission conversions [[YOGEME#51](https://github.com/MikeG621/YOGEME/issues/51)]

#### v5.1 - 15 Mar 2021
- (XWA) Trigger And/Or values now read XWA's method of (value & 1) = TRUE. Still only
  writes 0/1 [Related to [YOGEME#48](https://github.com/MikeG621/YOGEME/issues/48)]

#### v5.0 - 10 Oct 2020
- (XWA) Changed Trim to TrimEnd for craft Name and Cargos during load, as there's the
  potential for a leading `'\0'` which would keep the rest of the string
- Mostly XvT-related updates by Random Starfighter ***BREAKING CHANGES***
  - Mission.RndSeed discovered
  - FlightGroup.Unknown2 is now StopArrivingWhen enum
  - FlightGroup.Unknown3 is now RandomArrivalDelayMinutes
  - FlightGroup.Unknown4 is now RandomArrivalDelaySeconds
  - FG Goals discovered to have an array of Enabled values, not just the one followed by a Team value (which happened to just work for SP missions). Consumes Unk10-15.
  - Documentation updates to Mission_XvT.txt for some of the finer workings of some triggers and conditions
  - (XWA) Global Group references in Orders and Triggers prepped for string replacement, similar to FGs and Teams.
 
#### v4.0 - 09 Aug 2020
Many of these are ***BREAKING CHANGES***
- A lot of arrays and such were changed to use auto-properties, many of them with private set
- A lot of other arrays and objects were set to `readonly`
- (TIE.Flightgroup) PermaDeath changed to `bool`
- (TIE.Strings) IFF numbers removed from defaults
- (XWA.Mission) Iffs renamed to IFFs
- (XWA.Mission.Trigger) IFF substitution implemented
- (Xwing.Briefing) IsVisible( ) removed
- (Xwing.Briefing) EventCount( ) removed
- (Xwing.Briefing) Visible renamed to IsVisible
- (Xwing.Briefing) `public` fields have PascalCase applied
- (Xwing.Briefing) EventMapper now `private readonly static _eventMapper`
- Lots of fixes and tweaks by Random Starfighter
  - (BaseMessage) Message length increased to 64 from 63
  - (BaseStrings) FormationMine added, ShipClass and ObjectType updated
  - (*.Order) SafeString implementated
  - (*.Strings) Ability to reset CraftType and CraftAbbrv to defaults
  - (*.Trigger) SafeString implementated
  - (*.Trigger) ToString( ) now prevents "of of"
  - (*.Mission) Better Save backup
  - (*.Mission) Message load null term fixed
  - (Tie.Mission) Handling to load incomplete briefing questions
  - (XvT.FlightGroup.Order) TriggerType expanded
  - (XvT.Mission) Unknown4 and 5 removed, part of new IFF names
  - (XvT.Mission) Unknown6 renamed to PreventMissionOutcome
  - (XvT.Mission.IffNameIndexer) New
  - (XvT.Mission.Trigger) TriggerType expanded
  - (XvT.Strings) Various updates to throughout
  - (XWA.Strings) RoleTeams updated
  - (Xwing.Flightgroup) Raw values for Pitch/Yaw/Roll instead of degrees
  - (Xwing.Flightgroup) Internal string length increased
  - (Xwing.Mission) Unknown1 renamed to RndSeed
  - (Xwing.Mission) Fixed Yaw/Pitch being flipped during save
  - (Xwing.Mission) FlightGroup limit increased to 255
  - (Xwing.Mission) MessageLimit decreased to 0
  - (Xwing.Strings) FormationObject updated


#### v3.1 - 03 Jul 2020
- (*.Mission) Added backup during save
- (XWA.Strings) Orbit order details
- (*.Strings) More details to OverrideShipList length exception
- (Xwing.Strings) Added OverrideShipList
- (XWA.Strings) Added missing craft entry to shiplist

#### v3.0.1 - 19 Sep 2018
- (*.Mission) Fixed Pitch value check during write
- (Xwing.Mission) Add object angle conversion to/from degrees

#### v3.0 - 03 Sep 2018
- Lots of fixes, tweaks and new features by Random Starfighter ***BREAKING CHANGES***
  - Xwing95 platform support
  - helper functions throughout for FG/Message move/delete
  - (All) EditorCraftNumber and DifficultyAbbrv support
  - (BaseBriefing) EventParamterCount now a virtual function
  - (BaseBriefing) virtual helper functions added
  - (BaseStrings) some rewording
  - (BaseStrings) SafeString function
  - (Converter) capped TIE AI level
  - (Converter) fixed errors in TIE PlayerCraft, TIE Briefing.Events, XWA MessageDelay
  - (*.FlightGroup) ToString( ) updated
  - (*.Mission) updated string encodings
  - (*.Mission) fixed a null exception during fs.Close( )
  - (*.Strings) some rewording
  - (Tie.FlightGroup) added Campaign perma-death, formerly Unk9 and Unk10
  - (Tie.Mission) added EndOfMissionColor
  - (Tie.Mission) fixed read/write of highlighting in officer questions
  - (Tie.Strings) added missing CraftWhen values
  - (Xvt.Briefing) removed team initialization
  - (Xvt.BriefingCollection) improved team initialization
  - (Xvt.FlightGroup) added Energy Beam, Ion Pulse, Cluster Mine
  - (Xvt FlightGroup) added PreventCraftNumbering, DepartureClockMinutes, DepartureClockSeconds, formerly Unk 19, 20, 21
  - (Xvt.FlightGroup) Skip trigger default to `true`
  - (Xvt.FlightGroup.Goal) TimeLimit, formerly Unk16
  - (Xvt.FlightGroup.LoadoutIndexer) added IonPulse, EnergyBeam and ClusterMine
  - (Xvt.FlightGroup.Order) added "All IFFs except" target type
  - (Xvt.Mission) Renamed MissionType.MPMelee
  - (Xvt.Mission) fixed Departure and Arrival2 R/W, other R/W corrections to match format updates
  - (Xvt.Mission) Mission Succ/Fail/Desc load changed to TrimEnd
  - (Xvt.Mission) YOGEME signature moved to within MissionDescription instead of "outside" format
  - (Xvt.Strings) added RoleTeams, Difficulty and DIfficultyAbbrv arrays, EnergyBeam value
  - (Xwa.Briefing) removed team initialization
  - (Xwa.BriefingCollection) improved team initialization
  - (Xwa.FlightGroup) added Energy Beam, Cluster Mine
  - (Xwa.FlightGroup) Designations are disabled by default
  - (Xwa.FlightGroup.Goal) Unk43 added
  - (Xwa.FlightGroup.LoadoutIndexer) Energy Beam, Ion Pulse, Cluster Mine added/implemented
  - (Xwa.FlightGroup.Order) now default to `true`
  - (Xwa.FlightGroup.Order) time trigger added
  - (Xwa.FlightGroup.Order) SafeString( ) implemented
  - (Xwa.FlightGroup.Order) helper functions for Skip triggers
  - (Xwa.Message) updated to Delay
  - (Xwa.Mission) R/W corrections to match format updates
  - (Xwa.Mission) mission strings changed to TrimEnd on read
  - (Xwa.Mission) YOGEME signature moved to within MissionDescription instead of "outside" format
  - (Xwa.Mission) added GetDelaySeconds( )
  - (Xwa.Mission) FlightGroupLimit increased to 192
  - (Xwa.Mission.Trigger) SafeString( ) implemented
  - (Xwa.Mission>Trigger) delay calculation fixed
  - (Xwa.Strings) RoleTeams added
  - (Xwa.Strings) order docking text updated
- (*.Strings) "at least 1" changed to "any"
- (Tie.Mission) additional cleanup of officer question read/write
- (Xwa.Strings) removed "?" from "be delievered"
 

#### v2.7 - 09 May 2018
- (XWA.FlightGroup.Goal) Proximity triggers include distance in ToString
- (XWA.FlightGroup.Orders) TriggerType unknowns filled in [#1]
- (XWA.Globals.Goal) Proximity triggers include distance in ToString
- (XWA.Mission.Trigger) Proximity triggers include distance in ToString
- (XWA.Mission.Trigger) TriggerType unknowns filled in [#1]
- (XWA.Mission) FlightGroupLimit was raised to 132 for the time being, this is post-SuperBackDrops install to prevent errors
- (XWA.Strings) TriggerType unknowns filled in [#1]
- (XWA.Strings) Escort order had "Meaningless" replaced with "Position"

#### v2.6.2 - 24 Feb 2018
- (XWA.FlightGroup.Orders) Fixed the waypoint inversion [[YOGEME#16](https://github.com/MikeG621/YOGEME/issues/16)]

#### v2.6.1 - 18 Nov 2017
- (XWA.FlightGroup) Added some Backdrop-specific properties that alias the appropriate standard properties.

#### v2.6 - 15 Oct 2017
- (TIE.Strings) Added missing "*" from Med Trans
- (*.Strings) Added ability to replace craft list [[YOGEME#10](https://github.com/MikeG621/YOGEME/issues/10)]

#### v2.5 - 7 Jan 2017
- Lots of fixes and new features by Random Starfighter
  - (*.Mission) Enforced string encodings
  - (TIE.FlightGroup.Order) Hack for _checkValues added
  - (TIE.Mission) Added Message length check during load
  - (TIE.Mission) Fixed Global Goal loading
  - (TIE.Strings) Added Decoy Beam
  - (XvT.Briefing) Added Team functionality
  - (XvT.FlightGroup.Goal) Fixed points casts
  - (XvT.FlightGroup.Goal) Added Team visiblity
  - (XvT.Mission) Fixed craft options
  - (XvT.Mission) Added Message length check during load
  - (XvT.Mission) Fixed FG Unks
  - (XvT.Mission) Fixed Message Color writing
  - (XvT.Mission) Fixed Team writing
  - (XvT.Mission) Fixed Briefing Team read/write
  - (XvT.Strings) Added empty CraftWhen entries
  - (XvT.Strings) Added "each special craft" amount
  - (XWA.FlightGroup) Added Ion Pulse warhead
  - (XWA.FlightGroup.Goal) Updated ToString
  - (XWA.Mission) Fixed Unk3 initialization
  - (XWA.Mission) Fixed craft Options
  - (XWA.Strings) Added Unk Role values
  - (XWA.Strings) Added Unk Amount values

#### v2.4 - 6 June 2016
- (Converter) Fixed XWA to TIE player's craft conversion
- (All) Inverted the Y axis on all waypoints during read/write so in-game map and editor appearance match, but editor is still "positive up"
- Fixed various references in comments
 
#### v2.3 - 5 Apr 2015
- (BaseTrigger) Added TriggerIndex enum
- (XvT.Globals.Goal) set to Serializable
- (XvT.Globals.Goal) Deleted GoalStringIndexer, strings wrapped into new subclass, Trigger

#### v2.2 - 5 Feb 2015
- (Team.*) set classes to Serializable [[YOGEME#8](https://github.com/MikeG621/YOGEME/issues/8)]
- (BaseTrigger) set to Serializable [[YOGEME#5](https://github.com/MikeG621/YOGEME/issues/5)]
 
#### v2.1 - 14 Dec 2014
- Converted license to MPLv2.0
- (*.FlightGroupCollection) SetCount and IsModified implementation
- (*.MessageCollection) SetCount and IsModified implementation
- Couple other minor fixes that have been sitting here for a while...
 
#### v2.0.1 - 14 Aug 2012
- (BaseBriefing) Fixed bug regarding StartLength calculation
- (*.FlightGroup) Fixed bug preventing proper SpecialCargoCraft handling during Load/Save
- (Tie.FlightGroup) Added Unknowns.Unknown19, 20 and 21
- (Tie.Officers) Fixed bug in Save( ) causing '[' and ']' to save as characters instead of the appropriate highlighting codes
- (Xvt.Briefing) Fixed bug in Save( ) preventing proper Event writing
- (Xvt.Mission) Fixed critical bug in LoadMission( ) that resulted in unhandled exception
- (Xwa.Mission) Fixed critical bug in Save( ) causing infinite loop and filesize
 
#### v2.0 - 25 May 2012
Includes various ***BREAKING CHANGES***
- (BaseBriefing) Added EventParameterCount
- (BaseBriefing) Events[] is now `short[]`
- (BaseFlightGroup.BaseOrder) New
- (BaseFlightGroup.BaseWaypoint) New
- (BaseTrigger) Inherits `Indexer<byte>`
- (*.FlightGroup) Added ToString( ) overrides
- (*.FlightGroup.Goal) Added ToString( ) override, exceptions
- (*.FlightGroup.Goal) Inherits `Indexer<byte>`
- (*.FlightGroup.LoadoutIndexer) Inherits `Indexer<bool>`
- (*.FlightGroup.Order) Added ToString( ) overrides, exceptions, conversions
- (*.FlightGroup.Order) Inherits BaseFlightGroup.BaseOrder
- (*.FlightGroup.Waypoint) Added conversions
- (*.FlightGroup.Waypoint) Inherits BaseFlightGroup.BaseWaypoint
- (*.FlightGroupCollection) Added GetList( )
- (*.Mission) Deleted NumFlightGroups, NumMessages
- (*.Mission) Inherits MissionFile
- (*.Mission.Trigger) Added conversions, exceptions, ToString( ) overrides
- (*.TeamCollection) Added GetList( )
- (Tie.FlightGroup) Radio renamed to FollowsOrders
- (Tie.FlightGroup.FGGoals) Added exceptions
- (Tie.FlightGroup.FGGoals) Inherits `Indexer<byte>`
- (Tie.Mission) Added CraftCheck( ), CheckTarget( )
- (Tie.Mission) IffHostile inherits `Indexer<bool>`
- (Tie.Mission) EndOfMissionMessages inherits `Indexer<string>`
- (Tie.Mission.IffNameIndexer) Inherits `Indexer<string>`
- (Xvt.FlightGroup) Roles is now `Indexer<string>`
- (Xvt.Globals.Goal) Removed AndOrIndexer
- (Xvt.Globals.Goal) AndOr is now `bool[]`
- (Xvt.Mission) Added CraftCheck( ), CheckTarget( ), MissionTypeEnum
- (Xvt.Mission) BoP renamed to IsBop
- (Xvt.Strings) Removed MissType
- (Xvt.Team) EndOfMissionMessages is now `Indexer<string>`
- (Xwa.Briefing) Removed Team.set{}
- (Xwa.Briefing) Added BriefingStringNotes
- (Xwa.FlightGroup.Waypoint) Added ToString( ) override
- (Xwa.Mission) Added LogoEnum, HangarEnum, *Notes
- (Xwa.Mission) GlobalGroups, Regions, Iffs are now `Indexer<string>`
- (Xwa.Mission) Hangar renamed to MissionType
- (Xwa.Mission.Trigger) `ctor(byte[])` now accepts Length = 4
- (Xwa.Mission.Strings) Removed Logo, Hangar
- Back-end updates

---
#### Copyright Information

Copyright © 2009-2024 Michael Gaisser  
This library file and related files are licensed under the Mozilla Public License
v2.0 or later.  See [License.txt](License.txt) for further details.

"Star Wars" and related items are trademarks of LucasFilm Ltd and
LucasArts Entertainment Co.