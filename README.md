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
| Theme | Aureate Amber |
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
      Views/                                 # MainWindow + Pages/*.jalxaml (control demos)
      ViewModels/                            # one dedicated view model per page
      Assets/                                # Embedded images (logo.png, …)
  Services/
    Jalium.UI.Gallery.Services.Interfaces/   # Public service contracts
    Jalium.UI.Gallery.Services/              # Default implementations
  Tests/
    Jalium.UI.Gallery.Modules.Main.Tests/    # Fixtures for the main module
```

## Build & run

```
dotnet build Jalium.UI.Gallery.slnx
dotnet run --project Jalium.UI.Gallery.Desktop
```

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
- **Modules/Jalium.UI.Gallery.Modules.Main/Views/MainWindow.jalxaml(.cs)** —
  primary shell window hosting `NavigationView`, title-bar search and the
  theme toggle. Every control demo page is routed from here.
- **Modules/Jalium.UI.Gallery.Modules.Main/Views/Pages/** — one `*Page.jalxaml`
  (+ `.jalxaml.cs`) per showcased control. Pages are pure markup: the code-behind
  holds nothing but `InitializeComponent()`, and anything tabular or computed
  belongs to the page's view model. Shipping a new demo takes five steps, all
  guarded by the fixtures in `Tests/`:

  1. `Views/Pages/FooPage.jalxaml` — declare `x:Class` and
     `hosting:ViewModelLocator.AutoWireViewModel="True"`, never `DataContext`.
     Include at least one card whose structure is
     `Border` → `StackPanel` → `TextBlock` + `Border`, which is what the home
     page extracts for the live preview.
  2. `Views/Pages/FooPage.jalxaml.cs` — the constructor and nothing else.
  3. `ViewModels/CatalogPageViewModels.cs` — add
     `public sealed partial class FooViewModel : GalleryPageViewModelBase { }`.
     The name is the page's minus `Page`; auto-discovery pairs them, so
     `MainModule.cs` needs no edit.
  4. `Models/GalleryComponentCatalog.cs` — add an `Item<FooPage>("foo", …)`
     entry. The page tag must be ASCII letters and digits only, and a new
     category automatically gains a filter chip on the home page.
  5. `eng/generate-saas-card-backgrounds.ps1` — run it to mint the card artwork
     (a new category also needs a palette and a motif in that script), then
     bump the component count in `Tests/.../SaasCardBackgroundsFixture.cs` and
     `HomePageVirtualizationFixture.cs`.
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
