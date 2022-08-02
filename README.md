# HeadCullingMask

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that make your avatar's head invisible only to your FPV.  
(Looks like NearClip, but others are visible at near distance).  

## Usage
- Attach a Comment component to the avatar's face and hair (what you want to hide) slot and set "`net.hantabaru1014.HeadCullingMask.TargetSlot`" to Text.
- It only works when the Comment component's Enabled is True.
- It is processed when the avatar is equiped, so if the Comment component is setup while the avatar is on, it must be equip again.

## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
2. Place [HeadCullingMask.dll](https://github.com/hantabaru1014/HeadCullingMask/releases/latest/download/HeadCullingMask.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Neos logs.
