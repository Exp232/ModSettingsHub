# ModSettingsHub - notes for Claude Code

A mod for Railroader's built-in mod loader (Unity/Mono, game 0.6.x; entry point `ModEntryPoint` in Main.cs, asset-pack Info.json, settings in Settings.xml): one window, hotkey and top-right button for the settings of all mods. Namespace `ModSettingsHub`, author Exp232. Used (optionally, by reflection) by College Educated Auto Engineer and College-Educated Spouts.

## Build
- `dotnet build -c Release /p:DeployToGame=false` compiles against the game's DLLs (`RailroaderDir` in the csproj, or `/p:RailroaderDir=...`).
- Do not deploy into the game's Mods folder unless asked.
- Your usual job here: build, read compile errors, fix them minimally without changing behaviour, repeat until clean.

## Conventions
- `Hub` (Hub.cs) is public API (`Register`, `Unregister`, `IsRegistered`, `Toggle`, `Refresh`) that other mods call by reflection (`HubLinkTemplate.cs`): keep existing signatures stable, add overloads instead. Bump `ApiVersion` only for a breaking change.
- Relay mods join without calling the hub: `Hub.RelayEntries()` lists the game's custom windows whose identifier starts with `modsettings/` (`WindowManager.GetAllIdentifiers/GetDefinition`, `CustomWindowDefinition.BuilderAction`) and runs their builder on the hub's own builder. It uses game types outside `Railroader.Modding`, so look at it first when a game update breaks the hub.
- The hub owns no mod's values: mods save their own settings.
- The mod must never be listed as a dependency of another mod (load-order trouble); links are by reflection in both load orders.
- The top-right button copies a button from the game's `TopRightArea` (serialized private fields `timetableButton` / `tutorialButton`). If that fails it must only log, never throw.
- Never use a version number that also appears in the csproj (the package reference `Microsoft.NETFramework.ReferenceAssemblies` is 1.0.3): Build Watcher swaps the new version for the old one everywhere in the csproj, so the project then looks changed and the import waits for review.
- When changing behaviour or a setting, bump the version in `Info.json`, `BUILDING.md` and the csproj together and add a `CHANGELOG.md` entry (newest first, under `# Changelog`). Update `README.md` for user-visible changes.
