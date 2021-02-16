# I-surrender
OK, OK, Don't shoot - I give up!

## About: ##

I Surrender is a GTA V (ScripthookV.Net) script allowing you to surrender to police when wanted. 
You can either give up to the police without them shooting you or just clear your wanted level altogether (you cheater). 

A list of potential keybindings can be found: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netframework-4.8 or to disable a binding (e.g. the clear wanted option) set it to None. Modifiers allowing key combinations can be set to Alt, Control, Shift or None.

Controller support available using a combination of buttons which can be disabled by setting to None. 

## Features: ##

- Surrender by putting hands up until "Busted"
- Wanted level kept at 1 star to prevent agressive cops while surrendering
- Cancel and try to keep running (still busted if they catch you)
- (If enabled) Cheat and clear your wanted level
- Option to drop weapon in hand on surrendering (gun may misfire when hitting ground)

## Current limitations or known issues: ##

- [AddonPeds Mod](https://www.gta5-mods.com/scripts/addonpeds-asi-pedselector) will force model back to main character.
  - An optional replacement PedSelector.dll is included to prevent this if required


## Installation: ##

1. Ensure you have the latest version of ScripthookV and Scripthookv.Net installed correctly.
2. Copy the scripts folder into your "Grand Theft Auto V" install folder
3. Edit the .ini to meet your requirements

## Use of trainers: ##
Certain trainer scripts will reset your player back to Michael, Trevor, or Frankin. To prevent this, you will need to change the settings for the triner in question, for example: 

### Simple Trainer: ###
Change the following value in the trainer.ini file: 
> OverrideLoopFix=1

### Menyoo: ###
Either turn off **Reset player model on death** in the menu and save the settings or edit menyooConfig.ini setting the following value: 
> DeathModelReset = false

## Version history: ## 

1.0.0 
- Initial public release

## Credits: ## 
- [leec22](https://gtaforums.com/profile/1170715-leec22): Helping understand scripthookv.net coding
- [mrtank2333](https://github.com/mrtank2333): Source code for [Player Death No Reset](https://github.com/mrtank2333/Player-Death-No-Reset-Model-Source-code) helped solve game hanging on wasted/busted for non protaganist. 
- [Meth0d](https://www.gta5-mods.com/users/Meth0d): Providing source code for [PedSelector.dll](https://www.gta5-mods.com/scripts/addonpeds-asi-pedselector) to allow the modification
- [huckleberrypie](https://www.gta5-mods.com/users/huckleberrypie) Allowing [Elsa Add-On Ped](https://www.gta5-mods.com/player/elsa-of-arendelle-frozen-ii-addon-streamed-ped) for testing and promotional video
