# Building FreeFly

Use Windows, Visual Studio MSBuild with .NET Framework 4.8 targeting tools, and a local Valheim installation with BepInEx 5.

The included `Landoria.SharedLib` sources are embedded into the plugin by ILRepack. Their MIT license is included in that directory.

Build HarmonyValidator from the `HarmonyValidator/HarmonyValidator` project in the Landoria workspace and supply its DLL using `HarmonyValidatorAssembly`. The build validates Harmony targets before merging the shared library. Game, BepInEx, and validator binaries are not distributed in this repository.

Run from a Visual Studio developer shell:

```powershell
msbuild Landoria.FreeFly.csproj /restore /t:Build /p:Configuration=Release /p:ValheimGamePath="C:\Games\Valheim" /p:BepInExPath="C:\Games\Valheim\BepInEx" /p:HarmonyValidatorAssembly="C:\Tools\HarmonyValidator.dll"
```

The plugin is written to `bin/Release/Landoria.FreeFly.dll`. Install that DLL into the client's `BepInEx/plugins` directory.

Build and release workflows are maintained in `landoria-gaming/LandoriaModsAutomation`.