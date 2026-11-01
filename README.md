# Mod Settings Hub (Railroader)

One window for the settings of all your mods. Press **Ctrl+F7**, or click the button that this mod adds to the top-right
menu, and pick a mod from the list. Each mod keeps its own settings and files; the hub only gives them one place to show
up.

Railroader's built-in mod loader installs and loads mods, but has no shared place for their settings, so every mod has
brought its own window and its own hotkey. This is that place.

- One hotkey (Ctrl+F7) and one button for every mod that supports it, in the game's own window style.
- Mods that support it appear in the list by themselves. Nothing to configure.
- A mod that supports the hub works the same without it: it keeps its own window and its own hotkey.
- Client-side interface only. The hub saves nothing in your saves and changes nothing about the game.

## Requirements
- Railroader 0.6.0 or later (the game's built-in mod loader). It does not use Unity Mod Manager.
- Nothing else. The hub is only useful with at least one mod that supports it (see below).

## Install
1. Download the zip and unzip it, so you have a folder `ModSettingsHub` that contains `ModSettingsHub.dll` and `Info.json`.
2. Copy that folder into `Documents\Railroader\Mods` (create `Mods` if it is not there), so you have
   `Documents\Railroader\Mods\ModSettingsHub\Info.json`. If you start the game with a `-homedir` launch option, use the
   `Mods` folder inside that folder instead.
3. Start Railroader. In the main menu click **Mods**, then **Manage Mods**, switch **Mod Settings Hub** on, and click
   **Apply**. (A mod you have just copied in only shows up after the game has been restarted once.)
4. Load a game. The button appears in the top-right menu, next to the game's own icons, and **Ctrl+F7** opens the window. Neither
   works in the main menu: the hub lives in the game's windows, which only exist once a game is loaded.

**Update:** replace the `ModSettingsHub` folder with the new one. Your `Settings.xml` is in that folder; copy it over
too if you changed it.

**Uninstall:** switch the mod off under **Mods > Manage Mods** and apply, then delete its folder. Mods that supported it
go back to using their own windows. The hub leaves nothing behind in your saves.

## Using it
- Press **Ctrl+F7** or click the button in the top-right menu (it is the slider icon, beside the game's own icons; the
  menu fades out when the mouse is away from it, so move the mouse to the top-right corner). Press the hotkey or click the
  button again to close the window.
- The list on the left has every supported mod that is loaded. Click one to see its settings on the right. The window can
  be moved and resized like the game's others.
- The settings on a mod's page are the ones it always had, and they work the same: a slider or switch changes the mod at
  once, and the mod saves it itself, in its own folder.
- **Mod Settings Hub** is in the list too: its page shows the hotkey, a switch for the top-right button, and the mods
  using the hub.

### Which mods support it
- [Auto Engineer Brake Fix](https://github.com/Exp232/AutoEngineerBrakeFix) 2.8.1 and later
- [College-Educated Spouts](https://github.com/Exp232/CollegeEducatedSpouts) 1.1.1 and later

Their own hotkeys (Ctrl+F8 and Ctrl+F9) still open their own windows. Authors of other mods: see below, it is a few lines
of code.

## Settings
The hub's own settings are on its page in the window. Everything is in `Settings.xml` in the mod folder (created the first
time the game runs with the mod):

| Setting | Default | |
| --- | --- | --- |
| `Hotkey` | `<Keyboard>/f7` | The key, as an Input System binding path (see below). Empty for no hotkey. |
| `HotkeyCtrl` | `true` | Hold Ctrl with the key. Left on, so the key cannot clash with the game's own. |
| `ShowTopRightButton` | `true` | The button in the top-right menu. Also a switch on the hub's own page. |

Close the game before editing the file. A changed hotkey applies the next time the game starts.

Binding paths look like `<Keyboard>/f7`, `<Keyboard>/f10`, `<Keyboard>/m`, `<Keyboard>/backquote`. If the file is damaged
the hub says so in the log and runs on its defaults without touching the file; delete it to get a fresh one.

## Troubleshooting
- **Ctrl+F7 does nothing:** you must be in a loaded game, not the main menu. Check that the mod is switched on under
  Mods > Manage Mods. Another mod or program may use the same key; change `Hotkey` in `Settings.xml`.
- **The window is empty ("No mod has added its settings here yet"):** none of your mods supports the hub, or they are
  switched off. The hub's own page is always there when the mod is on.
- **A mod I expected is not in the list:** it has to support the hub (see the list above) and be switched on. Also check
  that it is a version that supports it: the hub does not need updating for that, the mod does.
- **There is no button in the top-right menu:** use the hotkey. The button is added by copying one of the game's menu
  buttons, so a game update that changes that menu may leave the hotkey as the only way in. Make sure
  `ShowTopRightButton` is `true`. The log says why when it could not add the button.
- **A mod's page shows an error instead of settings:** that mod's page failed to build. The rest of the window still works.
  Report it to that mod's author with the log.
- **Where is the log?** `%USERPROFILE%\AppData\LocalLow\Giraffe Lab LLC\Railroader\Player.log`. The hub's lines end with
  `ModPack: "exp232/modsettingshub"`: "Registered ..." for each mod that joined, and a warning when something failed.
- **The game asks about a changed mod set when I load a save:** that is the game's own prompt about mods that differ from
  the save, not something the hub stores. The hub changes nothing in the save.

## Multiplayer
The hub only shows a window on your own screen. It adds nothing to the game state and sends nothing, so it is safe to use
on a server whether or not the host or the other players have it. (Settings of other mods are still subject to their own
rules; the hub just shows their pages.)

## Compatibility
- Needs Railroader 0.6.0 or later. Tested with 0.6.2.
- Works next to any other mod. It adds one object to the top-right menu and does not patch the game's code.
- If a future game update changes the top-right menu, the button may not appear; the hotkey and the window are not affected.

## For mod authors
No reference to this mod is needed, and your mod keeps working when it is not installed.

1. Copy `HubLinkTemplate.cs` from this repository into your source, delete its `#if` and `#endif` lines (they keep the
   template out of the hub's own build) and change its namespace.
2. Make a method that draws your page with the game's panel builder. The hub has already put it inside a scroll view, so
   do not add one:

   ```csharp
   internal static void BuildPage(UIPanelBuilder page)
   {
       page.AddTitle("My Mod", "MyMod " + version);
       page.AddSection("Speed", b => { /* sliders, toggles, buttons as in any game window */ });
   }
   ```
3. In your load method: `HubLink.Register("yourname/yourmod", "My Mod", version, SettingsUi.BuildPage);`
   and in your unload method: `HubLink.Unregister();`

Do not put the hub in your Info.json dependencies. It is found when it loads, in either order, and a dependency would
only add a load-order constraint (and a hard requirement for your players).

The page is built each time it is shown or rebuilt, so read live values inside the controls' getters, and save your
settings yourself (the hub owns no values). If your page throws, the hub shows the error in place of the page and the
rest of the window keeps working.

API version 1 (`ModSettingsHub.Hub`, found by reflection): `Register(id, title, version, build)`, `Unregister(id)`,
`IsRegistered(id)`, `Toggle()`, `Refresh()`. Existing signatures never change; new ones arrive as overloads. If you do
reference the assembly, `HubUi` has `Note`, `Slider` and `Toggle` helpers.

[College-Educated Spouts](https://github.com/Exp232/CollegeEducatedSpouts) is the smallest worked example: see
`HubLink.cs`, the `HubLink.Register` call in `Main.cs`, and `BuildPage` in `SettingsUi.cs`.

## Credits
By Exp232. The window is built with the game's own interface builder, so it looks like the rest of the game.
