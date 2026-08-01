# Sekiro Param Merger

A tool that merges two Sekiro mod `gameparam.parambnd.dcx` 
files at **cell level** — so both mods keep all their changes 
without overriding each other.

## The Problem This Solves
When two Sekiro mods both edit the same param file, 
one completely overrides the other. This tool merges them 
at the individual cell level — if Mod A changes damage values 
and Mod B changes stamina values in the same file, 
the merged output contains BOTH changes.

## Features
- Cell-level param merging (not file-level)
- Visual conflict resolver — see exactly what conflicts and choose which mod wins
- Human readable conflict descriptions
- Auto-detects vanilla gameparam from your Sekiro install
- Remembers game folder and output path between sessions
- Automatically cleans up mod param folders after merging
- Dark UI

## Requirements
- Windows 10/11
- .NET 9 SDK — https://dotnet.microsoft.com/download/dotnet/9.0
- Sekiro: Shadows Die Twice installed
- Game files unpacked with UXM Selective Unpacker
  https://github.com/Nordgaren/UXM-Selective-Unpack/releases

## Setup
1. Clone this repo:
   git clone https://github.com/YOURUSERNAME/sekiroPM.git

2. Clone SoulsFormatsNEXT into the SAME folder as the repo:
   cd sekiroPM
   git clone https://github.com/soulsmods/SoulsFormatsNEXT.git

3. Build:
   dotnet build

4. Run the GUI:
   dotnet run --project SekiroParamMerger.WinForms

   Or run the console version:
   dotnet run --project SekiroParamMerger.Console

## How To Use
1. Launch the app
2. Set your Sekiro game folder (one time only)
3. Select Mod A (higher priority — wins conflicts)
4. Select Mod B
5. Click MERGE
6. Review any conflicts and choose which mod wins each one
7. Save — the merged file is placed in your output folder at:
   param\gameparam\gameparam.parambnd.dcx

## Important Notes
- oo2core_6_win64.dll is automatically copied from your Sekiro 
  install — you do not need to do this manually
- Sekiro paramdefs are bundled inside the tool (from soulsmods/Paramdex)
- SoulsFormatsNEXT must be cloned separately (see Setup above) 
  as it is not included in this repo

## Credits
- SoulsFormats / SoulsFormatsNEXT — TKGP and soulsmods community
  https://github.com/soulsmods/SoulsFormatsNEXT
- Paramdex (Sekiro paramdefs) — soulsmods community
  https://github.com/soulsmods/Paramdex
- UXM Selective Unpacker — Nordgaren
  https://github.com/Nordgaren/UXM-Selective-Unpack

## License
GPL v3 — inherited from SoulsFormatsNEXT dependency