# Changelog

## 1.2.1

- The log now shows mods that joined the hub before it had started (AutoEngineerBrakeFix and CollegeEducatedSpouts load first, so their "Registered ..." lines were missing). Those lines are written when the hub starts, marked "(before the hub started)". No change to behaviour.

## 1.2.0

- **Relay mods can add a settings page.** A mod written in Relay (a `Main.rl` script) makes an ordinary window whose identifier starts with `modsettings/`, for example `ui.createWindow("modsettings/yourname/yourmod", "My Mod", buildPage)`. The hub lists it and draws its builder inside the hub window. No call to the hub and no dependency on it, so the mod works the same when the hub is not installed. The hub looks for these windows each time its window is built, so they come back after a map change like any other Relay window.
- Pages added this way show no version number in the list. A Relay mod has no hotkey of its own to open the hub on its page (Relay has no hotkeys).
- No change for DLL mods: `Hub.Register` and the rest of API version 1 are unchanged.

## 1.1.0

First public release (Nexus Mods). Complete user documentation: install and update steps, uninstall, using the window, every setting, troubleshooting, multiplayer and compatibility notes, and a guide for mod authors. No change to the mod itself since 1.0.4.

## 1.0.4

- The button's icon has thinner lines (about 30% less ink), measured against the game's other top-right icons.

## 1.0.2

- The button's icon uses the same cream colour as the game's other top-right icons (it was white).

## 1.0.1

- The button's icon is drawn in thin outline like the game's own icons (hollow knobs instead of solid ones).
- Removed the "open the hub on a mod's page from that mod's own hotkey" feature (`Hub.ToggleFor`, `HubLink.Toggle`). A mod's own hotkey opens its own window, as it did before the hub.

## 1.0.0

First version. A hub window (list of mods on the left, the selected mod's page on the right) opened with Ctrl+F7 or a
button in the game's top-right menu; API version 1 for mods to add their page (`HubLinkTemplate.cs` shows the
reflection link, so mods need no reference and work without the hub); a page for the hub's own settings.
