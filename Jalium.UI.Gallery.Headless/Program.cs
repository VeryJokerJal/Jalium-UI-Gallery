using System.Text;
using System.Text.Json;

namespace Jalium.UI.Gallery.Headless;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        try
        {
            var options = HeadlessOptions.Parse(args);
            if (options.ShowHelp)
            {
                Console.WriteLine(HeadlessOptions.Usage);
                return 0;
            }

            if (options.ListPages)
            {
                WriteStdout(new HeadlessCatalogReport(
                    1,
                    HeadlessCatalog.Pages.Count,
                    HeadlessCatalog.Pages.Select(page => new HeadlessCatalogPage(
                        page.PageTag,
                        page.Title,
                        page.Category,
                        page.PageType.FullName ?? page.PageType.Name)).ToArray()));
                return 0;
            }

            var pages = ResolvePages(options);
            var output = HeadlessOutputLayout.Create(options, pages.Count);
            var rendered = new List<HeadlessPageReport>(pages.Count);
            var failures = new List<HeadlessFailureReport>();
            HeadlessBatchReport report;

            // Some sample controls intentionally report parser diagnostics with
            // Console.WriteLine. Keep those diagnostics visible on stderr while
            // reserving stdout for one machine-readable JSON document.
            var standardOutput = Console.Out;
            try
            {
                Console.SetOut(Console.Error);
                using var session = HeadlessGallerySession.Create(options);
                foreach (var page in pages)
                {
                    try
                    {
                        var paths = output.GetPagePaths(page.PageTag, options.Surface);
                        rendered.Add(session.Render(page, paths.PngPath, paths.MetadataPath));
                    }
                    catch (Exception exception)
                    {
                        failures.Add(new HeadlessFailureReport(
                            page.PageTag,
                            exception.GetType().Name,
                            exception.Message));
                        Console.Error.WriteLine(
                            $"headless-render: {page.PageTag}: {exception.GetType().Name}: {exception.Message}");
                    }
                }

                report = new HeadlessBatchReport(
                    1,
                    session.RendererName,
                    PlatformWindowCreated: false,
                    options.Surface.ToString(),
                    options.DarkTheme ? "dark" : "light",
                    options.LanguageName,
                    options.Dpi,
                    DateTimeOffset.UtcNow,
                    Path.GetFullPath(output.ManifestPath),
                    rendered,
                    failures);
                HeadlessGallerySession.WriteJson(output.ManifestPath, report);
            }
            finally
            {
                Console.SetOut(standardOutput);
            }

            WriteStdout(report);
            return failures.Count == 0 ? 0 : 1;
        }
        catch (CommandLineException exception)
        {
            Console.Error.WriteLine($"headless-render: {exception.Message}");
            Console.Error.WriteLine("Run with --help for usage.");
            return 2;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"headless-render: fatal: {exception}");
            return 1;
        }
    }

    private static IReadOnlyList<HeadlessPageDescriptor> ResolvePages(HeadlessOptions options)
    {
        var pages = new List<HeadlessPageDescriptor>();
        var selected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (options.AllPages)
        {
            foreach (var page in HeadlessCatalog.Pages)
            {
                if (selected.Add(page.PageTag))
                {
                    pages.Add(page);
                }
            }
        }

        foreach (var tag in options.PageTags)
        {
            var page = HeadlessCatalog.Require(tag);
            if (selected.Add(page.PageTag))
            {
                pages.Add(page);
            }
        }

        if (pages.Count == 0)
        {
            throw new CommandLineException("No pages were selected.");
        }

        return pages;
    }

    private static void WriteStdout<T>(T value)
    {
        Console.WriteLine(JsonSerializer.Serialize(value, HeadlessJson.Options));
    }
}

internal sealed class HeadlessOutputLayout
{
    private readonly string _outputPath;
    private readonly bool _singlePng;

    private HeadlessOutputLayout(string outputPath, bool singlePng, string manifestPath)
    {
        _outputPath = outputPath;
        _singlePng = singlePng;
        ManifestPath = manifestPath;
    }

    public string ManifestPath { get; }

    public static HeadlessOutputLayout Create(HeadlessOptions options, int pageCount)
    {
        var outputPath = Path.GetFullPath(options.OutputPath);
        var singlePng = pageCount == 1 &&
            string.Equals(Path.GetExtension(outputPath), ".png", StringComparison.OrdinalIgnoreCase);
        var manifestPath = singlePng
            ? Path.Combine(
                Path.GetDirectoryName(outputPath)!,
                Path.GetFileNameWithoutExtension(outputPath) + ".manifest.json")
            : Path.Combine(outputPath, "manifest.json");
        return new HeadlessOutputLayout(outputPath, singlePng, manifestPath);
    }

    public (string PngPath, string MetadataPath) GetPagePaths(
        string pageTag,
        HeadlessSurface surface)
    {
        var suffix = surface switch
        {
            HeadlessSurface.Shell => ".shell",
            HeadlessSurface.RawPage => ".raw",
            _ => string.Empty,
        };
        var pngPath = _singlePng
            ? _outputPath
            : Path.Combine(_outputPath, pageTag + suffix + ".png");
        return (pngPath, Path.ChangeExtension(pngPath, ".json"));
    }
}
