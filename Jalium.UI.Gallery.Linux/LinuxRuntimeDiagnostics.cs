using System.Diagnostics;
using System.Runtime.InteropServices;
using Jalium.UI.Controls.Automation;
using Jalium.UI.Interop;
using Jalium.UI.Notifications;

namespace Jalium.UI.Gallery.Linux;

internal static class LinuxRuntimeDiagnostics
{
    private static readonly string[] s_requiredLibraries =
    [
        "libjalium.native.core.so",
        "libjalium.native.platform.so",
        "libjalium.native.text.so",
        "libjalium.native.software.so",
        "libjalium.native.media.core.so",
        "libjalium.native.media.so",
        "libjalium.native.vulkan.so",
    ];

    internal static bool IsEnabled(string[] args) =>
        args.Any(static argument =>
            argument.Equals("--diagnostics", StringComparison.OrdinalIgnoreCase) ||
            argument.Equals("--diagnostics-only", StringComparison.OrdinalIgnoreCase)) ||
        string.Equals(
            Environment.GetEnvironmentVariable("JALIUM_DIAGNOSTICS"),
            "1",
            StringComparison.Ordinal);

    internal static bool HasCompleteNativePayload =>
        s_requiredLibraries.All(LibraryExists) &&
        s_requiredLibraries.All(CanLoadLibrary);

    internal static void WritePreflight(TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteLine("Jalium.UI Gallery Linux diagnostics");
        writer.WriteLine($"  OS: {RuntimeInformation.OSDescription}");
        writer.WriteLine($"  RID: {RuntimeInformation.RuntimeIdentifier}");
        writer.WriteLine($"  Process architecture: {RuntimeInformation.ProcessArchitecture}");
        writer.WriteLine($"  Base directory: {AppContext.BaseDirectory}");
        writer.WriteLine($"  Session type: {Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") ?? "<unset>"}");
        writer.WriteLine($"  Window override: {Environment.GetEnvironmentVariable("JALIUM_WINDOW_SYSTEM") ?? "auto"}");
        writer.WriteLine($"  DISPLAY: {Environment.GetEnvironmentVariable("DISPLAY") ?? "<unset>"}");
        writer.WriteLine($"  WAYLAND_DISPLAY: {Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") ?? "<unset>"}");
        writer.WriteLine($"  Render override: {Environment.GetEnvironmentVariable("JALIUM_RENDER_BACKEND") ?? "auto"}");
        writer.WriteLine("  Native payload:");

        foreach (string library in s_requiredLibraries)
        {
            string status = !LibraryExists(library)
                ? "missing"
                : CanLoadLibrary(library)
                    ? "ok     "
                    : "unloadable";
            writer.WriteLine($"    {status} {library}");
        }

        writer.WriteLine("  Desktop integration:");
        writer.WriteLine($"    FileChooser portal: {FormatCapability(IsDesktopPortalInterfaceAvailable("org.freedesktop.portal.FileChooser"))}");
        writer.WriteLine($"    Print portal: {FormatCapability(IsDesktopPortalInterfaceAvailable("org.freedesktop.portal.Print"))}");
        writer.WriteLine($"    Notification daemon: {FormatCapability(SystemNotificationManager.Current.IsSupported)}");
        writer.WriteLine($"    AT-SPI2 accessibility: {LinuxAccessibility.AtSpiStatus}");
        if (LinuxAccessibility.AtSpiLastError is { Length: > 0 } accessibilityError)
            writer.WriteLine($"      Error: {accessibilityError}");
    }

    internal static void WriteRenderContext(TextWriter writer, RenderContext context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(context);
        writer.WriteLine($"  Selected backend: {context.Backend}");
        writer.WriteLine($"  Rendering engine: {context.DefaultRenderingEngine}");
    }

    private static bool LibraryExists(string fileName) =>
        File.Exists(Path.Combine(AppContext.BaseDirectory, fileName));

    private static bool CanLoadLibrary(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (!File.Exists(path) || !NativeLibrary.TryLoad(path, out nint handle))
            return false;

        NativeLibrary.Free(handle);
        return true;
    }

    private static bool IsDesktopPortalInterfaceAvailable(string interfaceName)
    {
        if (!OperatingSystem.IsLinux())
            return false;

        try
        {
            var startInfo = new ProcessStartInfo("gdbus")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            startInfo.ArgumentList.Add("introspect");
            startInfo.ArgumentList.Add("--session");
            startInfo.ArgumentList.Add("--dest");
            startInfo.ArgumentList.Add("org.freedesktop.portal.Desktop");
            startInfo.ArgumentList.Add("--object-path");
            startInfo.ArgumentList.Add("/org/freedesktop/portal/desktop");

            using var process = Process.Start(startInfo);
            if (process is null)
                return false;

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(2_000))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // The diagnostic probe may race with normal process exit.
                }

                return false;
            }

            if (!Task.WaitAll([outputTask, errorTask], 1_000))
                return false;

            return process.ExitCode == 0 &&
                   outputTask.Result.Contains(interfaceName, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    private static string FormatCapability(bool available) =>
        available ? "available" : "unavailable";
}
