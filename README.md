# **DW2 bundle image validation mod**

Mod will log missing image assets from bundles that defined in *.xml files like TroopDefinition.xml. Content mods included too if they are loaded by game. Checks for saved games not supported.

# **Mod installation**

1. Download current release
2. Copy mod content to "DW2\mods\" folder
3. Add parameter to game with one of the folowing methods:
  - In Steam library open DW2 properties and add launch parameters: --low-level-inject mods\BundleImgValidator\BIV.dll!Mod.Init
  - Create shortcut to exe and add parameters to Target like this (replace path to your DW2 folder): "D:\Games\Distant Worlds 2\DistantWorlds2.exe" --low-level-inject "mods\BundleImgValidator\BIV.dll!Mod.Init"
  - Create .bat file that runs the game, something like this (replace path to your DW2 folder):"D:\Games\Distant Worlds 2\DistantWorlds2.exe" --low-level-inject "mods\BundleImgValidator\BIV.dll!Mod.Init"
4. Run game using method you created in step 2 (run steam or use shortcut\bat)
5. Check MissingFilesLog.txt file in mod folder for missing assets

# Warning
Do not remove or rename MissingFilesLog.txt, mod will crash if file is missing. Mod will clear it every time.

# Building
In case you need to build version yourself:

Set your DW2 game folder path in "Directory.Build.props" 'DW2_ROOT' variable. That will allow debugging and using latest dependencies from game folder.

Copy MonoMod.*.dll, Mono.*.dll and 0Harmony.dll from current release to your Debug\Release folder.
