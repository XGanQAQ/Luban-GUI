# Luban-GUI Copilot Instructions

## Build and run

- From the repository root, build the GUI and the embedded Luban CLI together with `dotnet build .\LubanGui.slnx`.
- Build only the Luban toolchain with `dotnet build .\lubanSrc\Luban.sln`.
- Run the Avalonia GUI with `dotnet run --project .\LubanGui\LubanGui.csproj`.
- Run the Luban CLI directly with `dotnet run --project .\lubanSrc\Luban\Luban.csproj -- --conf <conf-file> --target <target> [--codeTarget ...] [--dataTarget ...]`.
- Publish the GUI with `dotnet publish .\LubanGui\LubanGui.csproj -c Release -o .\publish`.
- No committed test projects or dedicated lint command were found in the repository, so there is no single-test command to use yet.
- Current builds succeed, but `dotnet build .\LubanGui.slnx` and `dotnet build .\lubanSrc\Luban.sln` emit `NU1902`/`NU1903`/`NU1904` warnings from `lubanSrc\Luban.Core` because of the `Scriban 5.12.0` dependency.

## High-level architecture

- This repository contains two separate codebases:
  - `LubanGui\` is the Avalonia desktop frontend.
  - `lubanSrc\` is the full Luban CLI/toolchain source tree.
- The root solution `LubanGui.slnx` only includes `LubanGui\LubanGui.csproj`. The GUI does not reference Luban projects directly; instead, `LubanGui.csproj` has MSBuild targets that build `lubanSrc\Luban\Luban.csproj` before the GUI build and then copy the CLI output into `LubanGui\bin\<Configuration>\net8.0\luban\`.
- `lubanSrc\Luban\Luban.csproj` is the CLI entry point and references the generator, schema, loader, validator, and format-specific `Luban.*` projects. The CLI entry point in `lubanSrc\Luban\Program.cs` loads the global config, creates a pipeline, and runs generation. Watch mode is implemented in `lubanSrc\Luban\Utils\DirectoryWatcher.cs`.
- `LubanGui\luban\` is a bundled Luban runtime snapshot. Treat it as build output/runtime payload, while `lubanSrc\` is the editable source of truth.
- The documents in `docs\` describe the intended GUI architecture and roadmap: a richer MVVM app with services such as `ExportService`, `ConfigManager`, `LubanExecutor`, and `ProcessManager`. Those documents are important context, but much of that structure is not implemented in `LubanGui\` yet.

## Key conventions

- Prefer the actual Luban CLI source over the design docs when behavior conflicts. The docs still describe simplified arguments like `--input_data_dir`, `--output_dir`, and `--codeFormat`, but the current CLI in `lubanSrc\Luban\Program.cs` actually exposes `--conf`, `--target`, `--codeTarget`, `--dataTarget`, `--watchDir`, and related options.
- Treat `docs\01-软件设计文档.md`, `docs\02-技术方案文档.md`, and `docs\03-开发计划文档.md` as planning/design references rather than proof that code already exists.
- The current GUI follows the default Avalonia + CommunityToolkit.Mvvm pattern:
  - `LubanGui\ViewModels\ViewModelBase.cs` inherits `ObservableObject`.
  - `LubanGui\App.axaml.cs` sets the runtime `DataContext`.
  - `LubanGui\Views\*.axaml` uses `x:DataType` for compiled bindings and separate design-time `DataContext`.
  - `LubanGui\ViewLocator.cs` maps `*ViewModel` to `*View` by naming convention.
- Preserve the validation setup in `LubanGui\App.axaml.cs`: it removes Avalonia's `DataAnnotationsValidationPlugin` to avoid duplicate validation alongside CommunityToolkit.
- The GUI is still template-level today (`MainWindowViewModel` only exposes `Greeting`), so do not assume the service/DI/logging types described in `docs\` are already present.
- `lubanSrc\.editorconfig` is the main style source for the Luban toolchain: Allman braces, braces required (`IDE0011 = error`), and a `max_line_length` of 180.
