# LaneController
Cities: Skylines 1 - Lane Controller Mod

> ⚠️ This is a **forked branch** with build configuration improvements.
> This branch does **not** modify any core source code / mod logic of the original project.
>
> Original Repository: https://github.com/t1a2l/LaneController

## Branch Info
Branch name: `lane-controller-V1.0.0`
All changes are limited to the MSBuild project file (`LaneController.csproj`) for build workflow optimization.
The in-game behaviour and features are **identical to the upstream original version**.

### Build Configuration Improvements
1. Fixed assembly versioning: Disable automatic Git commit hash injection, assembly version stays consistent on every build.
2. Explicit manifest resource names (`LogicalName`) for all UI assets, fixes occasional missing icons across different build environments.
3. Separate Debug / Release build profiles, supports debugging with Visual Studio.
4. Rewritten post-build deploy target:
   - No longer wipe the entire mod folder
   - Copy only required files selectively
   - Auto-deploy zh-CN localized satellite assembly
   - Skip PDB copy when PDB file does not exist to avoid MSBuild errors
5. Extended reference exclusion list, prevents game assemblies from being accidentally copied into mod output.
6. Globally exclude auto-generated `Designer.cs` files to avoid compile conflicts.
7. Replaced absolute hardcoded paths with relative paths to improve repository portability.

✅ Verified locally: All mod functions, UI sprites and Chinese localization work correctly in-game.

## Development Environment
- Visual Studio 2022
- .NET Framework 3.5
- Cities: Skylines 1

### Build Prerequisites
1. Place this repository at the proper directory level so relative `HintPath` can locate game assemblies under `Cities_Data/Managed`.
2. Prepare dependency DLLs inside the `libs` folder:
   - `MoveItIntegration.dll`
   - `UnifiedUILib.dll`
   - `System.Runtime.Serialization.dll`
3. `Newtonsoft.Json.dll` must be obtained from the corresponding Steam Workshop mod folder.

### Build & Deploy
1. Open the solution `LaneController.sln`
2. Configuration:
   - `Debug`: For development and debugging
   - `Release`: For release distribution
3. After successful build, MSBuild will automatically copy mod files to:
`%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons\Mods\LaneController\`

## About Pull Request
The csproj optimization commits from this branch are prepared for submitting a PR back to upstream.
**This README file is for this fork only and will NOT be included in the upstream PR.**

## License
This project inherits the license terms of the original upstream repository.
