# Jalium.UI.Gallery

Full-featured control gallery for the Jalium.UI framework, arranged in the
**Full App (Modular)** layout that ships with Jalium One.

| Setting | Value |
| --- | --- |
| Template | Full App (Modular) |
| Platforms | Windows Desktop, Linux, macOS (development), Android |
| Design pattern | PrismCompat |
| Test framework | XUnitV3 |
| Namespace | Jalium.UI.Gallery |
| Theme | Teal accent, light and dark palettes |
| Jalium.UI | Source reference (../Jalium.UI) |

## Solution layout

```
Jalium.UI.Gallery/
  Jalium.UI.Gallery.slnx
  Jalium.UI.Gallery.Core/                    # MVVM base classes, region names
  Jalium.UI.Gallery.<Platform>/              # Per-platform entry project (Desktop/Linux/MacOS/Android)
  Jalium.UI.Gallery.Headless/                # Window-free PNG + semantic snapshot CLI
  Modules/
    Jalium.UI.Gallery.Modules.Main/          # Primary module — views, viewmodels, themes
      Themes/                                # AppTheme + GalleryTheme palette
      Models/                                # GalleryComponentCatalog (the page registry)
      Controls/                              # Reusable templated Gallery controls
      Views/                                 # MainWindow + Pages/*.cs (typed view surfaces)
      Themes/Pages/                          # Default ControlTemplates and independent previews
      ViewModels/                            # one dedicated view model per page
      Localization/                          # en-US, zh-CN, ja-JP and ko-KR resources
      Assets/                                # Embedded images (logo.png, …)
  Services/
    Jalium.UI.Gallery.Services.Interfaces/   # Public service contracts
    Jalium.UI.Gallery.Services/              # Default implementations
  Tests/
    Jalium.UI.Gallery.Modules.Main.Tests/    # Fixtures for the main module
```

## Build & run

Use the .NET 10 SDK and a sibling `Jalium.UI` source checkout on
`preview-26.11.1`. Build the native payload for the target platform first.
The complete solution includes platform-specific SDKs; build the entry project
for your host instead of restoring every platform's workload.

```powershell
# Windows
dotnet run --project Jalium.UI.Gallery.Desktop
```

```bash
# Linux, with the matching Jalium.UI native libraries available
dotnet run --project Jalium.UI.Gallery.Linux

# Android, on a host with the .NET Android workload and Android SDK
dotnet build Jalium.UI.Gallery.Android
```

The catalog contains 121 examples in nine categories, with search, category
filters, a responsive shell, live previews, XAML/C# code samples and a theme
workbench. The shell and page resources ship in Simplified Chinese, English,
Japanese and Korean. Platform-specific examples expose availability through
the platform capability service.

## macOS development

The Apple Silicon entry uses AppKit, Metal and CoreText. From the sibling
`Jalium.UI` checkout:

```bash
bash eng/apple/run-gallery.sh Debug
```

After the native payload is built and `dotnet` is in `PATH`, run directly
from the Gallery checkout:

```bash
cd Jalium.UI.Gallery.MacOS
dotnet run
```

Gallery Debug builds accept the newer Xcode validated during local development.
Release retains the workload's Xcode version check; pass
`-p:ValidateXcodeVersion=true` to enable it in Debug too. The script's
`--allow-newer-xcode` option explicitly bypasses that check for a local build.
`--build-only` builds without launching.
The script builds native libraries, runs GPU/AppKit smoke tests and creates
`Jalium.UI.Gallery.MacOS.app` in the framework's `artifacts/macos-gallery` folder.
Command+Q quits; the native Edit menu provides standard editing gestures.

For isolated Apple builds, keep `JaliumBuildRoot` on the checkout's volume and
let the SDK choose the bundle and publish paths. Validation used that layout
and checked the signatures of every bundled native library individually.
See [the Gallery validation record](docs/gallery-update-validation-20261117.md)
for the tested configurations and remaining device/packaging scope.

See [macOS development and validation](../Jalium.UI/docs/macos-development.md)
for prerequisites, toolchain details and remaining scope. This development build
includes precompiled shaders; custom `SourceHlsl` effects need the full native
shader compiler build.

## Desktop memory policy

The desktop keeps GPU acceleration with Vello. The home page warms its first
24 card images and loads later cards as virtualization brings them near the
viewport. Decoded artwork has a 32-entry strong LRU cache; live controls can
continue sharing evicted sources through weak references. At most four image
decoders run concurrently. The desktop's native bitmap cache uses 32 entries
and an estimated 16 MiB budget; textures used by the current frame stay protected.

The D3D12 backend releases completed upload staging without waiting for its
swap-chain slot to be reused. After two seconds without a submitted frame, idle
reclamation releases Vello working buffers and pooled outputs once the GPU has
finished using them. A one-second UI timer continues checking after rendering
has stopped. Vello shader pipelines remain cached; working buffers are rebuilt
on the next draw. Rendering quality and the MSAA setting are unchanged.

The desktop also sets `System.GC.ConserveMemory=5` and a 512 KiB generation-zero
budget. These tune natural collection frequency; they are not heap-size limits.
Builds can override them with `GalleryGcConserveMemory` and
`GalleryGen0MaxBudgetBytes`. Memory comparisons should report process working
set, Private Bytes, and dedicated/shared GPU memory separately.

## Headless rendering for AI and automation

`Jalium.UI.Gallery.Headless` renders the production Gallery tree through the
native Software backend into an in-memory render target. It never calls
`Application.Run()`, `Window.Show()`, or creates a platform window. Unlike the
smaller managed `RenderTargetBitmap` test rasterizer, this path includes glyphs,
decoded images, paths, clips, control templates, and renderer effects.

Render one real routed page/workbench:

```powershell
dotnet run --project Jalium.UI.Gallery.Headless -- button `
  --output artifacts/headless/button.png
```

Render the complete `MainWindow` visual tree without showing it:

```powershell
dotnet run --project Jalium.UI.Gallery.Headless -- button --shell `
  --width 1500 --height 920 `
  --output artifacts/headless/button-shell.png
```

Render a sample page directly at its full content height:

```powershell
dotnet run --project Jalium.UI.Gallery.Headless -- button --raw-page --full-page `
  --width 1200 --max-height 12000 `
  --output artifacts/headless/button-full.png
```

Discover tags or snapshot the entire catalog:

```powershell
dotnet run --project Jalium.UI.Gallery.Headless -- --list
dotnet run --project Jalium.UI.Gallery.Headless -- --all `
  --output artifacts/headless/all
```

Every successful render writes:

- a PNG with the requested physical pixel size;
- a per-page JSON sidecar containing dimensions, theme/language, clipping state,
  pixel hash, painted bounds, and visible text with pixel-space element bounds;
- a batch `manifest.json` (also printed to stdout) with absolute paths and any
  per-page failures. Diagnostics go to stderr, so stdout remains machine-readable.

PNG and JSON files are completed in the destination directory and atomically
replaced, so a watcher never observes a half-written snapshot.

Defaults are deterministic: route surface, light theme, `en-US`, 1500×920 at
96 DPI, Impeller on the Software backend, and four detached-tree layout passes.
Use `--theme dark`, `--language zh-CN`, `--dpi 144`, `--engine Vello`, or
`--no-semantics` as needed. `--help` documents every option. WebView and map
pages keep their layout but disable external/native-child content, making batch
output offline and repeatable.

The focused regression suite includes a real native readback and PNG/semantic
contract check:

```powershell
dotnet test Tests/Jalium.UI.Gallery.Headless.Tests
```

## Where to look first

- **Jalium.UI.Gallery.Core/Mvvm/** — `ViewModelBase` + `RegionViewModelBase`. Inherit
  from these in every view model.
- **Modules/Jalium.UI.Gallery.Modules.Main/MainModule.cs** — module composition
  entry; the shell calls in here to build the primary view model.
- **Modules/Jalium.UI.Gallery.Modules.Main/AppBuilderExtensions.cs** —
  `UseShared()` wires the accent palette, the `GalleryTheme` live-skin, the
  `AppTheme.jalxaml` merge, and constructs `MainWindow` with DI.
- **Modules/Jalium.UI.Gallery.Modules.Main/Views/MainWindow.cs** and
  **Themes/GalleryWindow.jalxaml** — the typed shell and its replaceable
  template, including navigation, search, language and theme commands.
- **Modules/Jalium.UI.Gallery.Modules.Main/Views/Pages/** — one small typed
  `*Page.cs` surface per example, deriving from `GalleryPage`. Visual trees live
  in default templates; application state and commands live in dedicated view
  models. Shipping a new demo takes these steps, guarded by the fixtures in
  `Tests/`:

  1. Add `Views/Pages/FooPage.cs` deriving from `GalleryPage`.
  2. Add `Themes/Pages/FooPage.jalxaml` with a typed default `ControlTemplate`
     and `hosting:ViewModelLocator.AutoWireViewModel="True"`. Merge it into
     `Themes/GalleryControlThemes.jalxaml`. Do not set `x:Class` or `DataContext`
     in page templates. Use the shared header, section and demo controls.
  3. Add a dedicated `FooViewModel` deriving from `GalleryPageViewModelBase`.
     The name is the page's minus `Page`; module auto-discovery pairs them.
     Provide working commands and code examples that match the live demo.
  4. Add an `Item<FooPage>("foo", …)` entry to
     `Models/GalleryComponentCatalog.cs`. Tags use ASCII letters and digits;
     categories automatically become home-page filters. Supply an independent
     `Gallery.Preview.FooPage` data template instead of reparenting page content.
  5. Add the matching `Assets/SaasCardBackgrounds/foo.png`, localize new copy
     in all four resource dictionaries, and update catalog coverage fixtures.
     Run both test projects and inspect headless snapshots at desktop and
     narrow widths.
- **Modules/Jalium.UI.Gallery.Modules.Main/Themes/AppTheme.jalxaml** — the
  `ResourceDictionary` that backs every `{DynamicResource ...}` reference in
  the generated page templates. Edit the `SolidColorBrush` hex values to
  restyle the whole app.
- **Modules/Jalium.UI.Gallery.Modules.Main/Themes/GalleryTheme.cs** — the live
  palette (Dark/Light mode switch) used by `MainWindow` chrome and any page
  that consumes brushes imperatively.
- **Modules/Jalium.UI.Gallery.Modules.Main/Models/GalleryCssExtensions.cs** —
  the custom CSS vocabulary the styling pages demonstrate, registered through
  the framework's public `CssMappings` surface.
- **Services/Jalium.UI.Gallery.Services.Interfaces/** — service contracts. Add
  a new interface here before wiring up a corresponding implementation in
  `.Services/`.

## Validation

```bash
dotnet test Tests/Jalium.UI.Gallery.Modules.Main.Tests
dotnet test Tests/Jalium.UI.Gallery.Headless.Tests
```

On macOS, the main-module tests check that the native payload exports match
the current framework before running. Rebuild it after updating the sibling
framework. Architecture, code samples, layout, localization, theme and control
interaction fixtures complement the headless PNG/semantic contract tests.
Headless rendering does not validate a physical device, OS dialogs, native
child windows, assistive technology or production packaging.
