# Changelog

## 1.0.1

- The button's icon is drawn in thin outline like the game's own icons (hollow knobs instead of solid ones).
- Removed the "open the hub on a mod's page from that mod's own hotkey" feature (`Hub.ToggleFor`, `HubLink.Toggle`). A mod's own hotkey opens its own window, as it did before the hub.

## 1.0.0

First version. A hub window (list of mods on the left, the selected mod's page on the right) opened with Ctrl+F7 or a
button in the game's top-right menu; API version 1 for mods to add their page (`HubLinkTemplate.cs` shows the
reflection link, so mods need no reference and work without the hub); a page for the hub's own settings.
