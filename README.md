# EDDB_Scraper
Voice Attack Plugin to scrape [eddb.io](https://eddb.io/) for information regarding a specific two station loop.

Requires [EDDI](https://github.com/cmdrmcdonald/EliteDangerousDataProvider) version 2+, [Html Agility Pack](https://htmlagilitypack.codeplex.com/) and VoiceAttack version 1.5.9+

The included VoiceAttack profile should offer the full functionality and import just fine into exisitng profiles.

## Installation
##### Method 1
1. Download and extract [Html Agility Pack](https://htmlagilitypack.codeplex.com/)
2. Clone or download repo
3. Compile with [Visual Studio Community](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) (free)

    a. Make sure reference to Html Agility Pack points to /Net40/HtmlAgilityPack.dll

4. Copy `EDDB_Scraper.dll` and `HtmlAgilityPack.dll` to `C:\Program Files (x86)\VoiceAttack\Apps\EDDB_Scraper`
5. Import `EDDB_Scraper-Profile.vap`

    a. Keybinding commands are identical to [HCS](http://www.hcsvoicepacks.com/) profiles so you can skip import of those if you use. Key commands are referenced by name for easy import, see below for list.

##### Method 2

1. Download compiled plugin from [Releases](https://github.com/SavageCore/EDDB_Scraper/releases) page
2. Extract and copy `EDDB_Scraper.dll` and `HtmlAgilityPack.dll` to `C:\Program Files (x86)\VoiceAttack\Apps\EDDB_Scraper`
3. Import `EDDB_Scraper-Profile.vap`

    a. Keybinding commands are identical to [HCS](http://www.hcsvoicepacks.com/) profiles so you can skip import of those if you use. Key commands are referenced by name for easy import, see below for list.

## How to use

1. Edit command `((EDDB_Scraper - Config))`
2. Set Integer `EDLP_LoopID` to numerical value from [`https://eddb.io/trade/loop/1481127`](https://eddb.io/trade/loop/1481127)
3. Optionally enable Debug log printing by setting Boolean `EDLP_Debug` to True
4. Say `Plot next trade route`
5. Done - VA should open Galaxy map, search for System, open System Map and finally plot route to station

If you forget which item you should buy at current station just say `What should I buy next`

## Commands

You will need to import or have commands set up with the following names to press in-game keys

```
((UI Accept))

((UI Down))

((UI Left))

((UI Next))

((UI Right))

((System Map))

((Galaxy Map))
```

#### Returns

| Variable | Description |
|:--------:|:-----------:|
| {TXT:EDLP_System1} | Left hand system name
| {TXT:EDLP_System2} | Right hand system name
| {INT:EDLP_System1_id} | System 1 EDDB ID
| {INT:EDLP_System2_id} | System 2 EDDB ID
| {TXT:EDLP_Station1} | System 1 station
| {TXT:EDLP_Station2} | System 2 station
| {INT:EDLP_Station1_id} | Station 1 EDDB ID
| {INT:EDLP_Station2_id} | Station 2 EDDB ID
| {INT:EDLP_Station1Distance} | Station 1 position from sun
| {INT:EDLP_Station2Distance} | Station 2 position from sun
| {TXT:EDLP_Buy1} | Item to buy at system 1
| {TXT:EDLP_Buy2} | Item to buy at system 2
