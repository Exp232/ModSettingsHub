# Nexus Mods page text (copy and paste; not part of the mod)

## Name
Mod Settings Hub

## Summary (short description)
One window, one hotkey (Ctrl+F7) and one top-right menu button for the settings of all your mods. Mods add their page to it; mods that support it work the same without it.

## Suggested category and tags
Category: Utilities (or the closest "Miscellaneous / Gameplay" category the Railroader page offers).
Tags: settings, menu, interface, ui, utility, mod menu, hotkey, quality of life.

## Requirements (Nexus "Requirements" box)
Railroader 0.6.0 or later. No other mods and no Unity Mod Manager. (Only useful with a mod that supports it.)

## Description (BBCode)

```
[size=5][b]Mod Settings Hub[/b][/size]

One window for the settings of all your mods. Press [b]Ctrl+F7[/b], or click the button this mod adds to the top-right menu, and pick a mod from the list. Each mod keeps its own settings and files; the hub only gives them one place to show up.

Railroader's built-in mod loader installs and loads mods, but has no shared place for their settings, so every mod has brought its own window and its own hotkey. This is that place.

[list]
[*]One hotkey (Ctrl+F7) and one button for every mod that supports it, in the game's own window style.
[*]Mods that support it appear in the list by themselves. Nothing to configure.
[*]A mod that supports the hub works the same without it: it keeps its own window and its own hotkey.
[*]Interface only. The hub saves nothing in your saves, changes nothing about the game and sends nothing over the network, so it is safe in multiplayer.
[/list]

[size=4][b]Requirements[/b][/size]
Railroader 0.6.0 or later (the game's built-in mod loader; Unity Mod Manager is not used). The hub is only useful together with at least one mod that supports it.

[size=4][b]Mods that support it[/b][/size]
[list]
[*][url=https://github.com/Exp232/CollegeEducatedAutoEngineer]College Educated Auto Engineer[/url] 1.0.0 and later
[*][url=https://github.com/Exp232/CollegeEducatedSpouts]College-Educated Spouts[/url] 1.1.1 and later
[/list]
Their own hotkeys (Ctrl+F8 and Ctrl+F9) still open their own windows. Mods written in Relay (scripts) can add a page too, see "For mod authors".

[size=4][b]Install[/b][/size]
[list=1]
[*]Download the zip and unzip it, so you have a folder [b]ModSettingsHub[/b] that contains ModSettingsHub.dll and Info.json.
[*]Copy that folder into [b]Documents\Railroader\Mods[/b] (create Mods if it is not there). If you start the game with a -homedir launch option, use the Mods folder inside that folder instead.
[*]Start Railroader. In the main menu click [b]Mods[/b], then [b]Manage Mods[/b], switch [b]Mod Settings Hub[/b] on and click [b]Apply[/b]. (A mod you have just copied in only shows up after the game has been restarted once.)
[*]Load a game. The button appears in the top-right menu and [b]Ctrl+F7[/b] opens the window. Neither works in the main menu, only once a game is loaded.
[/list]
Update: replace the ModSettingsHub folder with the new one (copy your Settings.xml over if you changed it). Uninstall: switch the mod off under Mods > Manage Mods, apply, and delete its folder. Mods that supported it go back to using their own windows.

[size=4][b]Using it[/b][/size]
[list]
[*]Press Ctrl+F7 or click the slider-shaped button in the top-right menu (the menu fades out when the mouse is away from it: move the mouse to the top-right corner). Press the hotkey or click the button again to close the window.
[*]Click a mod in the list on the left to see its settings on the right. The window can be moved and resized like the game's others.
[*]A mod's settings work as they always did: a slider or switch changes the mod at once, and the mod saves it itself, in its own folder.
[*]Mod Settings Hub is in the list too: its page shows the hotkey, a switch for the top-right button and the mods using the hub.
[/list]

[size=4][b]Settings[/b][/size]
Settings.xml in the mod folder (created the first time the game runs with the mod). Close the game before editing it.
[list]
[*][b]Hotkey[/b] (default <Keyboard>/f7): the key, as an Input System binding path such as <Keyboard>/f10 or <Keyboard>/m. Empty for no hotkey. Applies the next time the game starts.
[*][b]HotkeyCtrl[/b] (default true): hold Ctrl with the key, so it cannot clash with the game's own keys.
[*][b]ShowTopRightButton[/b] (default true): the button in the top-right menu (also a switch on the hub's own page).
[/list]

[size=4][b]Troubleshooting[/b][/size]
[list]
[*][b]Ctrl+F7 does nothing:[/b] you must be in a loaded game, not the main menu. Check that the mod is switched on under Mods > Manage Mods. Another mod may use the same key: change Hotkey in Settings.xml.
[*][b]The window says no mod has added its settings:[/b] none of your mods supports the hub, or they are switched off.
[*][b]A mod I expected is not in the list:[/b] it has to support the hub (see above) and be switched on. The mod needs to be a version that supports it; the hub does not need updating for that.
[*][b]No button in the top-right menu:[/b] use the hotkey. The button is made by copying one of the game's menu buttons, so a game update that changes that menu may leave the hotkey as the only way in. Check that ShowTopRightButton is true.
[*][b]A page shows an error instead of settings:[/b] that mod's page failed to build; the rest of the window still works. Report it to that mod's author with your log.
[*][b]Log:[/b] %USERPROFILE%\AppData\LocalLow\Giraffe Lab LLC\Railroader\Player.log. The hub's lines end with ModPack: "exp232/modsettingshub".
[/list]

[size=4][b]Compatibility[/b][/size]
Needs Railroader 0.6.0 or later (tested with 0.6.2). Works next to any other mod: it adds one object to the top-right menu and does not patch the game's code.

[size=4][b]For mod authors[/b][/size]
Supporting the hub takes a few lines and no reference to this mod, and your mod keeps working when it is not installed: copy one small file into your source, register a method that draws your settings page with the game's own panel builder, and unregister it on unload. Do not list the hub in your mod's dependencies. Full guide, example and API on GitHub: https://github.com/Exp232/ModSettingsHub

[b]Relay mods[/b] need no file and no call: create a window whose identifier starts with [b]modsettings/[/b] (for example ui.createWindow("modsettings/yourname/yourmod", "My Mod", buildPage)) and the hub lists it and draws it inside its window. Example on GitHub.

[size=4][b]Source[/b][/size]
https://github.com/Exp232/ModSettingsHub
```

## Changelog entry for the first file
1.2.1: The log shows mods that joined the hub before it started. No other change.
1.2.0: Relay mods can add a settings page: a window whose identifier starts with modsettings/ is listed in the hub. No change for other mods.
1.1.0: First public release.

## File
Main file: ModSettingsHub-<version>.zip (the folder ModSettingsHub with ModSettingsHub.dll and Info.json), made by `dotnet build -c Release /p:Package=true`, or downloaded from the GitHub release.
