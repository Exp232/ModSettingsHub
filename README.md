# Mod Settings Hub (Railroader)

One window for the settings of all your mods. Press **Ctrl+F7**, or click the new button in the top-right menu, and pick
a mod from the list. Each mod keeps its own settings and files; the hub only gives them one place to show them.

Railroader's built-in mod loader installs and loads mods, but has no shared place for their settings, so every mod has
brought its own window and its own hotkey. This is that place.

## For players
Unzip into `Documents\Railroader\Mods` so you have `Mods\ModSettingsHub`. Mods that support the hub show up in its list
by themselves. A mod that supports the hub also works without it: it just opens its own window, as before.

Currently supported: [Auto Engineer Brake Fix](https://github.com/Exp232/AutoEngineerBrakeFix) 2.8.0 and later,
[College-Educated Spouts](https://github.com/Exp232/CollegeEducatedSpouts) 1.1.0 and later.

With the hub installed, a supporting mod's own hotkey (for example Ctrl+F9) opens the hub on that mod's page.

`Settings.xml` in the mod folder:

| Setting | Default | |
| --- | --- | --- |
| `Hotkey`, `HotkeyCtrl` | `<Keyboard>/f7`, true | The window's key (restart to apply). |
| `ShowTopRightButton` | true | The button in the top-right menu (also a switch on the hub's own page). |

## For mod authors
No reference to this mod is needed, and your mod keeps working when it is not installed.

1. Copy `HubLink.Template.cs` into your source and change its namespace.
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
4. Where your own hotkey opened your window, start with `if (HubLink.Toggle()) return;`.

Do not put the hub in your Info.json dependencies. It is found when it loads, in either order, and a dependency would
only add a load-order constraint.

The page is built each time it is shown or rebuilt, so read live values inside the controls' getters, and save your
settings yourself (the hub owns no values). If your page throws, the hub shows the error in place of the page and the
rest of the window keeps working.

API version 1 (`ModSettingsHub.Hub`, found by reflection): `Register(id, title, version, build)`, `Unregister(id)`,
`IsRegistered(id)`, `ToggleFor(id)` (true if the hub handled it), `Open(id)`, `Toggle()`, `Refresh()`. Existing
signatures never change; new ones arrive as overloads. If you do reference the assembly, `HubUi` has `Note`, `Slider`
and `Toggle` helpers.

## Compatibility
The button is made by copying one of the buttons the game keeps in its top-right menu, so a game update that changes
that menu may leave the hotkey as the only way in. The log says so; nothing else is affected.
