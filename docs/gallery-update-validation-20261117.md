# Gallery update validation — 2026-11-17

The update includes the complete registered catalog: 121 examples in nine
categories and three routed Gallery/foundation screens, for 124 routes. Views
use typed controls with replaceable templates and dedicated view models.
The existing migration, CSS/Razor examples, live workbench, localization,
platform entries and headless renderer are included together.

The checkout was validated against sibling Jalium.UI `343d47dc` on
`preview-26.11.1`, using .NET SDK 10.0.300 on Apple Silicon, macOS 27.2 and
Xcode 27.2. The sibling framework source remained unchanged.

## Changes completed during validation

- Completed 99 missing language entries. Each of en-US, zh-CN, ja-JP and
  ko-KR now defines the same 74 shell keys and 2,026 page keys. Resource
  coverage tests reject missing, duplicate or empty localized entries.
- Added macOS native payload copying and export validation to Headless, so
  the CLI and its tests start without a manually configured library path.
- Loaded the shared logo for detached shell exports. Semantic snapshots now
  exclude hidden/transparent subtrees and intersect ancestor clipping bounds.
- Bound the bar-chart demos to native `DataPoints`, matching their executable
  code samples. All six series contain real data, including grouped, stacked
  and horizontal examples.
- Constrained fixed-width demos to their host, wrapped form rows, scaled the
  geometry canvases and provided horizontal scrolling for the command table.
  Regression tests check input/chart bounds in a phone-sized page.
- Improved secondary/muted text and accent ink. Tests check four reading-text
  tokens and accent ink against seven shipped surfaces in both themes at
  4.5:1 or higher, plus the text on the primary accent fill.
- Aligned all direct XML cryptography package references with framework
  version 10.0.13, resolving the desktop restore downgrade.
- Updated the architecture/build instructions and excluded local OS, IDE and
  CodeGraph cache files from Git.

## Executed checks

| Check | Result |
| --- | --- |
| Main-module suite | 282 passed, 0 failed, 0 skipped |
| Headless suite | 9 passed, 0 failed, 0 skipped |
| Windows desktop Debug, cross-compiled | Passed, 0 warnings, 0 errors on final incremental build |
| Linux portable Debug, cross-compiled | Passed, 0 warnings, 0 errors on final incremental build |
| macOS Debug, SDK-managed bundle paths on the checkout volume | Passed; all 21 dylib signatures and the bundle signature verified individually |
| Desktop shell snapshots, 1500 × 920, light, zh-CN | 124/124 rendered; 0 failures |
| Narrow shell snapshots, 390 × 844, dark, zh-CN | 124/124 rendered; 0 failures |
| Additional 768 × 1024 light snapshots | Home, button, bar chart and design tokens rendered |
| Final contrast snapshots | Home, button and bar chart rendered after the shared palette change |
| macOS actual window | Captured and inspected; keyboard search, Enter navigation, Tab focus and Esc return exercised |

The two complete snapshot matrices were inspected in contact sheets, with
full-size checks of affected forms, charts, the shell and foundation pages.
The last palette adjustment was checked separately on representative screens;
it did not change layout. The automated tests also cover code samples, view
registration, themes, responsive layout, commands and control interactions.

The interface review applied the keyboard, focus, accessible-name, content and
contrast principles relevant to a native application from the
[Web Interface Guidelines](https://raw.githubusercontent.com/vercel-labs/web-interface-guidelines/main/command.md).
This was not a browser DOM or assistive-technology audit.

## Reproduce

Build the matching native cohort in the sibling framework first, then run:

```bash
dotnet test Tests/Jalium.UI.Gallery.Modules.Main.Tests
dotnet test Tests/Jalium.UI.Gallery.Headless.Tests
dotnet build Jalium.UI.Gallery.Desktop -p:EnableWindowsTargeting=true
dotnet build Jalium.UI.Gallery.Linux
dotnet build Jalium.UI.Gallery.MacOS

dotnet run --project Jalium.UI.Gallery.Headless -- --all --shell \
  --width 1500 --height 920 --language zh-CN --engine Vello \
  --output artifacts/gallery-update-20261117/desktop

dotnet run --project Jalium.UI.Gallery.Headless -- --all --shell \
  --width 390 --height 844 --theme dark --language zh-CN --engine Vello \
  --output artifacts/gallery-update-20261117/narrow
```

Screenshots, semantic sidecars and manifests produced for this run remain in
the ignored `artifacts/gallery-update-20261117/` directory. They are local
validation evidence, not release packages.

## Limits and remaining acceptance

Windows/Linux execution on native hosts, Android workload/device builds,
screen-reader use, IME, real file dialogs, printing, media/camera integration,
native child windows and release/AOT/notarized packaging were not accepted by
this run. Headless WebView/map content is intentionally offline/deterministic;
unsupported-platform pages retain their explanatory state. Rendering a route
does not exercise every child control, popup, scrolled section or error state.
The visual matrices used zh-CN; complete en-US, ja-JP and ko-KR visual layouts
were not reviewed beyond the resource-coverage checks.

Some complex native examples retain their own horizontal scrolling/clipping,
including editor/docking/Markdown content at phone widths. Chart annotations
use framework-owned colors; the shared-token contrast checks do not establish
AA contrast for every chart label or every control state.

The macOS workload emits warnings about transitive CSS build-tool files without
bundle metadata, and the test build retains nullable/analyzer warnings. A
temporary cross-volume build with manually overridden bundle/publish paths
produced invalid .NET runtime-library signatures. The final SDK-managed,
same-volume build passed all individual signature checks and launched without
manual signing. Keep that supported output layout when reproducing this run.

Native ABI exports were checked after rebuilding the native package against
the current macOS SDK. Existing precompiled Metal libraries were reused;
fresh Metal shader compilation and the runtime `SourceHlsl` compiler were not
validated because the separate Metal toolchain was unavailable.
