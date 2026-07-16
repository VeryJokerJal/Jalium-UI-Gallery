#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat >&2 <<'USAGE'
Usage: bash eng/linux/publish.sh <rid> [Debug|Release] [--aot]

RIDs: linux-x64, linux-arm64, linux-musl-x64, linux-musl-arm64

Environment:
  JALIUM_UI_ROOT            Jalium.UI checkout (default: sibling ../Jalium.UI)
  JALIUM_DOTNET             dotnet executable (default: dotnet on PATH)
  JALIUM_BUILD_ROOT         isolated MSBuild root (default: /tmp)
  JALIUM_ARTIFACTS_ROOT     output root (default: ./artifacts/linux)
  JALIUM_SKIP_NATIVE_BUILD  set to 1 to reuse an existing native payload
  JALIUM_SKIP_RUN           set to 1 when cross-building
USAGE
}

if [[ $# -lt 1 || $# -gt 3 ]]; then
  usage
  exit 2
fi

rid="$1"
configuration="${2:-Release}"
aot=false
if [[ "${2:-}" == "--aot" ]]; then
  configuration=Release
  aot=true
elif [[ "${3:-}" == "--aot" ]]; then
  aot=true
elif [[ $# -eq 3 ]]; then
  usage
  exit 2
fi

case "$rid" in
  linux-x64|linux-arm64|linux-musl-x64|linux-musl-arm64) ;;
  *) usage; exit 2 ;;
esac
case "$configuration" in
  Debug|Release) ;;
  *) echo "Configuration must be Debug or Release (got '$configuration')." >&2; exit 2 ;;
esac

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
gallery_root="$(cd -- "$script_dir/../.." && pwd)"
framework_root="${JALIUM_UI_ROOT:-$gallery_root/../Jalium.UI}"
framework_root="$(cd -- "$framework_root" && pwd)"
dotnet="${JALIUM_DOTNET:-dotnet}"
project="$gallery_root/Jalium.UI.Gallery.Linux/Jalium.UI.Gallery.Linux.csproj"
flavor=self-contained
if [[ "$aot" == true ]]; then flavor=nativeaot; fi
build_root="${JALIUM_BUILD_ROOT:-/tmp/jalium-gallery-publish-$rid-$flavor}"
artifacts_root="${JALIUM_ARTIFACTS_ROOT:-$gallery_root/artifacts/linux}"
publish_dir="$artifacts_root/$rid/$flavor"
archive="$artifacts_root/Jalium.UI.Gallery-$rid-$flavor.tar.gz"

if ! command -v "$dotnet" >/dev/null 2>&1 && [[ ! -x "$dotnet" ]]; then
  echo "dotnet executable not found: $dotnet" >&2
  exit 1
fi

skip_native_build="${JALIUM_SKIP_NATIVE_BUILD:-0}"
if [[ "$skip_native_build" != "1" ]]; then
  bash "$framework_root/eng/linux/build-native.sh" "$rid" "$configuration"
fi

native_stamp="$framework_root/src/native/bin/native/$rid/$configuration/.jalium-native-complete"
if [[ ! -f "$native_stamp" ]]; then
  echo "Missing complete native payload: $native_stamp" >&2
  exit 1
fi

required_libraries=(
  libjalium.native.core.so
  libjalium.native.media.core.so
  libjalium.native.media.so
  libjalium.native.platform.so
  libjalium.native.software.so
  libjalium.native.text.so
  libjalium.native.vulkan.so
)
native_payload_dir="$framework_root/src/native/bin/native/$rid/$configuration"
for library in "${required_libraries[@]}"; do
  if [[ ! -f "$native_payload_dir/$library" ]]; then
    echo "Native payload is missing $library: $native_payload_dir" >&2
    exit 1
  fi
done

rm -rf -- "$publish_dir"
mkdir -p -- "$publish_dir" "$build_root"

"$dotnet" restore "$project" \
  --runtime "$rid" \
  --disable-parallel \
  -p:JaliumBuildRoot="$build_root" \
  -p:JaliumNativePayloadValidated=true \
  -p:PublishAot="$aot" \
  -p:PublishTrimmed="$aot" \
  -p:GeneratePackageOnBuild=false

publish_args=(
  publish "$project"
  --configuration "$configuration"
  --runtime "$rid"
  --self-contained true
  --no-restore
  -m:1
  --output "$publish_dir"
  -p:JaliumBuildRoot="$build_root"
  -p:JaliumNativePayloadValidated=true
  -p:GeneratePackageOnBuild=false
  -p:PublishAot="$aot"
  -p:PublishTrimmed="$aot"
  -p:InvariantGlobalization=false
)
"$dotnet" "${publish_args[@]}"

for library in "${required_libraries[@]}"; do
  if [[ ! -f "$publish_dir/$library" ]]; then
    echo "Published output is missing $library" >&2
    exit 1
  fi
done

install -Dm644 \
  "$gallery_root/packaging/linux/org.jalium.Gallery.desktop" \
  "$publish_dir/share/applications/org.jalium.Gallery.desktop"
install -Dm644 \
  "$gallery_root/packaging/linux/org.jalium.Gallery.metainfo.xml" \
  "$publish_dir/share/metainfo/org.jalium.Gallery.metainfo.xml"
install -Dm644 \
  "$gallery_root/Modules/Jalium.UI.Gallery.Modules.Main/Assets/logo.png" \
  "$publish_dir/share/pixmaps/org.jalium.Gallery.png"

if [[ "${JALIUM_SKIP_RUN:-0}" != "1" ]]; then
  "$publish_dir/Jalium.UI.Gallery" --diagnostics-only
fi

rm -f -- "$archive"
tar -C "$publish_dir" -czf "$archive" .
archive_name="$(basename -- "$archive")"
(
  cd -- "$artifacts_root"
  sha256sum "$archive_name" > "$archive_name.sha256"
)
echo "Published $rid ($flavor) to $publish_dir"
echo "Archive: $archive"
echo "Checksum: $archive.sha256"
