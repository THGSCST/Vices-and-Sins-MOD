# Vices and Sins MOD
The first and only 1998 Gangsters Organized Crime MOD

## Features
* New graphic symbols for important buildings.
* Recruitment has been made easier.
* The amount of business in a city has been redistributed.
* Police and FBI presence has been reduced.
* Some buildings have a new name and function.
* New hood generation template.
* Ideal fronts for ilegal business has been rethought.
* More realistic economy model data.

## Tools
* __XLSX2Data__ - Batch create TXT files quickly and easily. Edit all your data directly in a familiar spreadsheet format. No more need to convert or open them in separate programs.
* __SPRTYL2PNG__ - Show, replace or export any game gfx resource *.spr and *.tyl files. Support full transparency with PNG import/export.
* __HELP2CSV__ - Converts help/tooltips files (*.idx *.dat) to CSV.
* __XTX2TXT__ - Converter for XTX files.

## How to install?
1. First you MUST HAVE an original game, which can be obtained at GOG.com.
2. Copy all the contents to install folder.
3. If you have a non-GOG version, do not copy the `gangster.exe` or `gangster-cheats.exe` executable.

## Cheats/debug executable `gangster-cheats.exe`
This mode is specifically designed to facilitate debugging or testing.  Custom debug flags are enabled, and intro videos are disabled to streamline the process. It includes the following cheats to assist with testing:
* `Shift + $` Your balance becomes one million dollars.
* `Shift + #` All gangs get 1000 dollars, including you.

## Graphics Fix
To run the game on Windows 10, the vast majority of graphics issues can be solved with [DxWrapper](https://github.com/elishacloud/dxwrapper) credits to [elishacloud](https://github.com/elishacloud) and [narzoul](https://github.com/narzoul).
**You can get an already set up version here: [Tools/Misc](Tools/Misc/).** It's configured and ready to use, just extract the content in the game's installation folder.

For debugging it is highly recommended to run the game in **windowed mode**: Change the configuration file `dxwrapper.ini` to the following:
```
[Compatibility]
Dd7to9                     = 1
[d3d9]
EnableWindowMode           = 1
```