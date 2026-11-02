# Building
1. Install the .NET SDK. Edit `RailroaderDir` in the .csproj (or pass `/p:RailroaderDir="D:\...\Railroader"`).
2. `dotnet build -c Release` builds and copies into `Documents\Railroader\Mods\ModSettingsHub` (override with `/p:ModsDir=...`).
3. `dotnet build -c Release /p:Package=true /p:ReleaseVersion=1.2.1` also writes `dist\ModSettingsHub-1.2.1.zip`
   (contains the `ModSettingsHub` folder with the DLL and Info.json; unzip into `Documents\Railroader\Mods`).
Keep `version` in Info.json in step with `ReleaseVersion`.
