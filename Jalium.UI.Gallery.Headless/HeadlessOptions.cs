using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Interop;

namespace Jalium.UI.Gallery.Headless;

internal enum HeadlessSurface
{
    Route,
    RawPage,
    Shell,
}

internal sealed class HeadlessOptions
{
    private const int MaxDimension = 16_384;
    private const long MaxPixels = 64L * 1024 * 1024;

    private readonly List<string> _pageTags = [];

    public bool ShowHelp { get; private set; }

    public bool ListPages { get; private set; }

    public bool AllPages { get; private set; }

    public IReadOnlyList<string> PageTags => _pageTags;

    public string OutputPath { get; private set; } = Path.Combine("artifacts", "headless");

    public int PixelWidth { get; private set; } = 1500;

    public int PixelHeight { get; private set; } = 920;

    public int MaxPixelHeight { get; private set; } = 12_000;

    public double Dpi { get; private set; } = 96;

    public GalleryLanguage Language { get; private set; } = GalleryLanguage.English;

    public string LanguageName => GalleryLanguages.Get(Language).CultureName;

    public bool DarkTheme { get; private set; }

    public RenderingEngine RenderingEngine { get; private set; } = RenderingEngine.Impeller;

    public HeadlessSurface Surface { get; private set; } = HeadlessSurface.Route;

    public bool FullPage { get; private set; }

    public bool EmitSemantics { get; private set; } = true;

    public int SettlePasses { get; private set; } = 4;

    public static HeadlessOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var options = new HeadlessOptions();
        for (var index = 0; index < args.Length; index++)
        {
            var raw = args[index];
            var separator = raw.StartsWith("--", StringComparison.Ordinal)
                ? raw.IndexOf('=')
                : -1;
            var name = separator > 0 ? raw[..separator] : raw;
            var inlineValue = separator > 0 ? raw[(separator + 1)..] : null;

            string ReadValue()
            {
                if (inlineValue is not null)
                {
                    if (inlineValue.Length == 0)
                    {
                        throw new CommandLineException($"Option '{name}' requires a value.");
                    }

                    return inlineValue;
                }

                if (++index >= args.Length)
                {
                    throw new CommandLineException($"Option '{name}' requires a value.");
                }

                return args[index];
            }

            switch (name.ToLowerInvariant())
            {
                case "-h":
                case "--help":
                    options.ShowHelp = true;
                    break;

                case "--list":
                    options.ListPages = true;
                    break;

                case "--all":
                    options.AllPages = true;
                    break;

                case "-p":
                case "--page":
                    AddPageTags(options._pageTags, ReadValue());
                    break;

                case "-o":
                case "--output":
                    options.OutputPath = ReadValue();
                    break;

                case "--width":
                    options.PixelWidth = ReadInt(name, ReadValue(), 64, MaxDimension);
                    break;

                case "--height":
                    options.PixelHeight = ReadInt(name, ReadValue(), 64, MaxDimension);
                    break;

                case "--max-height":
                    options.MaxPixelHeight = ReadInt(name, ReadValue(), 64, MaxDimension);
                    break;

                case "--dpi":
                    options.Dpi = ReadDouble(name, ReadValue(), 48, 384);
                    break;

                case "--theme":
                    options.DarkTheme = ReadValue().ToLowerInvariant() switch
                    {
                        "light" => false,
                        "dark" => true,
                        var value => throw new CommandLineException(
                            $"Unknown theme '{value}'. Expected 'light' or 'dark'."),
                    };
                    break;

                case "--language":
                case "--lang":
                    options.Language = ParseLanguage(ReadValue());
                    break;

                case "--engine":
                    options.RenderingEngine = ParseRenderingEngine(ReadValue());
                    break;

                case "--full-page":
                    options.FullPage = true;
                    break;

                case "--shell":
                    options.Surface = HeadlessSurface.Shell;
                    break;

                case "--raw-page":
                    options.Surface = HeadlessSurface.RawPage;
                    break;

                case "--route":
                    options.Surface = HeadlessSurface.Route;
                    break;

                case "--no-semantics":
                    options.EmitSemantics = false;
                    break;

                case "--settle-passes":
                    options.SettlePasses = ReadInt(name, ReadValue(), 1, 12);
                    break;

                default:
                    if (raw.StartsWith("-", StringComparison.Ordinal))
                    {
                        throw new CommandLineException(
                            $"Unknown option '{raw}'. Use --help to see supported options.");
                    }

                    AddPageTags(options._pageTags, raw);
                    break;
            }
        }

        options.Validate();
        return options;
    }

    public static string Usage =>
        """
        Render Jalium.UI Gallery into PNG files without creating or showing a platform window.

        Usage:
          dotnet run --project Jalium.UI.Gallery.Headless -- [page] [options]

        Selection:
          -p, --page <tag>       Render one page. Repeat or pass comma-separated tags.
              --all              Render every routable Gallery page.
              --list             Print the page catalog as JSON and exit.

        Output:
          -o, --output <path>    Output directory, or a .png path for one page.
                                Default: artifacts/headless
              --width <pixels>   Output width. Default: 1500.
              --height <pixels>  Viewport height. Default: 920.
              --full-page        Grow page output to its complete content height.
              --max-height <px>  Full-page safety cap. Default: 12000.
              --dpi <number>     Raster DPI. Default: 96.
              --no-semantics     Do not emit the AI-readable text/bounds sidecar.

        Appearance:
              --theme <mode>     light (default) or dark.
              --language <name>  en-US (default), zh-CN, ja-JP, or ko-KR.
              --engine <name>    Impeller (default), Vello, or Auto.

        Surface:
              --route            Render the real routed content/workbench (default).
              --raw-page         Render the underlying sample page directly.
              --shell            Render the complete MainWindow visual tree without Show().

        Advanced:
              --settle-passes N  Detached-tree layout passes. Default: 4.
          -h, --help              Show this help.

        Examples:
          dotnet run --project Jalium.UI.Gallery.Headless -- button
          dotnet run --project Jalium.UI.Gallery.Headless -- --page home --shell --width 1500 --height 920
          dotnet run --project Jalium.UI.Gallery.Headless -- --all --raw-page --full-page --output artifacts/headless/all
        """;

    private void Validate()
    {
        if (!ShowHelp && !ListPages && !AllPages && _pageTags.Count == 0)
        {
            _pageTags.Add("home");
        }

        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            throw new CommandLineException("Output path cannot be empty.");
        }

        if (Surface == HeadlessSurface.Shell && FullPage)
        {
            throw new CommandLineException(
                "--full-page cannot be combined with --shell because the shell owns a fixed viewport.");
        }

        if (MaxPixelHeight < PixelHeight && FullPage)
        {
            throw new CommandLineException("--max-height must be greater than or equal to --height.");
        }

        var effectiveHeight = FullPage ? MaxPixelHeight : PixelHeight;
        if ((long)PixelWidth * effectiveHeight > MaxPixels)
        {
            throw new CommandLineException(
                $"The requested bitmap can grow beyond {MaxPixels:N0} pixels. Reduce --width or --max-height.");
        }

        if ((AllPages || _pageTags.Count > 1) &&
            string.Equals(Path.GetExtension(OutputPath), ".png", StringComparison.OrdinalIgnoreCase))
        {
            throw new CommandLineException(
                "A .png --output path can only be used when rendering one page.");
        }
    }

    private static void AddPageTags(List<string> destination, string value)
    {
        var tags = value.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tags.Length == 0)
        {
            throw new CommandLineException("A page tag cannot be empty.");
        }

        destination.AddRange(tags);
    }

    private static int ReadInt(string name, string value, int minimum, int maximum)
    {
        if (!int.TryParse(value, out var result) || result < minimum || result > maximum)
        {
            throw new CommandLineException(
                $"Option '{name}' must be an integer between {minimum} and {maximum}.");
        }

        return result;
    }

    private static double ReadDouble(string name, string value, double minimum, double maximum)
    {
        if (!double.TryParse(
                value,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result) ||
            !double.IsFinite(result) ||
            result < minimum ||
            result > maximum)
        {
            throw new CommandLineException(
                $"Option '{name}' must be a number between {minimum} and {maximum}.");
        }

        return result;
    }

    private static GalleryLanguage ParseLanguage(string value)
    {
        if (GalleryLanguages.TryParse(value, out var language))
        {
            return language;
        }

        if (Enum.TryParse<GalleryLanguage>(value, ignoreCase: true, out language))
        {
            return language;
        }

        throw new CommandLineException(
            $"Unknown language '{value}'. Expected en-US, zh-CN, ja-JP, or ko-KR.");
    }

    private static RenderingEngine ParseRenderingEngine(string value)
    {
        if (Enum.TryParse<RenderingEngine>(value, ignoreCase: true, out var engine) &&
            engine is RenderingEngine.Auto or RenderingEngine.Impeller or RenderingEngine.Vello)
        {
            return engine;
        }

        throw new CommandLineException(
            $"Unknown rendering engine '{value}'. Expected Auto, Impeller, or Vello.");
    }
}
