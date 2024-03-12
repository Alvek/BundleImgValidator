# **DW2 bundle image validation mod**

Mod will log missing image assets from bundles that defined in *.xml files like TroopDefinition.xml

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
Do not remove or rename MissingFilesLog.txt, mod will crush if file is missing. Mod will clear it every time.
