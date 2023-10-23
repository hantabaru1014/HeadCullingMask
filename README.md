# HeadCullingMask

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that make your avatar's head invisible only to your FPV.  
(Looks like NearClip, but others are visible at near distance).  

## Usage
- Attach a Comment component to the avatar's face and hair (what you want to hide) slot and set "`net.hantabaru1014.HeadCullingMask.TargetSlot`" to Text.
- It only works when the Comment component's Enabled is True.
- It is processed when the avatar is equiped, so if the Comment component is setup while the avatar is on, it must be equip again.

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [HeadCullingMask.dll](https://github.com/hantabaru1014/HeadCullingMask/releases/latest/download/HeadCullingMask.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
