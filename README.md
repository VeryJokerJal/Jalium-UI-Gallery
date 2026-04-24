# Jalium.UI.Gallery

Full-featured control gallery for the Jalium.UI framework, arranged in the
**Full App (Modular)** layout that ships with Jalium One.

| Setting | Value |
| --- | --- |
| Template | Full App (Modular) |
| Platforms | Desktop, Android |
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
  Jalium.UI.Gallery.<Platform>/              # Per-platform entry project (Desktop/Android)
  Modules/
    Jalium.UI.Gallery.Modules.Main/          # Primary module — views, viewmodels, themes
      Themes/                                # AppTheme + GalleryTheme palette
      Views/                                 # MainWindow + Pages/*.jalxaml (control demos)
      ViewModels/                            # ViewAViewModel + future page view models
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
  (+ `.cs`) per showcased control. Add a new entry here and wire it into the
  navigation map in `MainWindow.jalxaml.cs` to ship a new demo.
- **Modules/Jalium.UI.Gallery.Modules.Main/Themes/AppTheme.jalxaml** — the
  `ResourceDictionary` that backs every `{DynamicResource ...}` reference in
  the generated page templates. Edit the `SolidColorBrush` hex values to
  restyle the whole app.
- **Modules/Jalium.UI.Gallery.Modules.Main/Themes/GalleryTheme.cs** — the live
  palette (Dark/Light mode switch) used by `MainWindow` chrome and any page
  that consumes brushes imperatively.
- **Services/Jalium.UI.Gallery.Services.Interfaces/** — service contracts. Add
  a new interface here before wiring up a corresponding implementation in
  `.Services/`.
