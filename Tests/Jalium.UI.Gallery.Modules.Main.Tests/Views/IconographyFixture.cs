using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class IconographyFixture
{
    private static readonly Regex ButtonContentPattern = new(
        """
        <(?:Button|RepeatButton)\b
        (?:(?!>).)*?\bContent\s*=\s*"(?<content>[^"]+)"
        (?:(?!>).)*?>
        """,
        RegexOptions.CultureInvariant |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline);

    private static readonly Regex TextBlockTextPattern = new(
        """
        <TextBlock\b
        (?:(?!>).)*?\bText\s*=\s*"(?<text>[^"]+)"
        (?:(?!>).)*?>
        """,
        RegexOptions.CultureInvariant |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline);

    private static readonly Regex ButtonElementPattern = new(
        """
        <(?<type>Button|RepeatButton)\b
        (?<attributes>[^>]*?)(?<!/)>
        (?<body>.*?)
        </\k<type>>
        """,
        RegexOptions.CultureInvariant |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline);

    private static readonly Regex IconElementPattern = new(
        """<(?:SymbolIcon|PathIcon|Path)\b""",
        RegexOptions.CultureInvariant);

    private static readonly Regex OpenDemoLabelPattern = new(
        """
        <x:String\s+x:Key="Loc\.Home\.OpenDemoFormat">
        (?<text>.*?)
        </x:String>
        """,
        RegexOptions.CultureInvariant |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline);

    [Fact]
    public void FunctionalSurfaces_DoNotUseTextOrEmojiAsIcons()
    {
        var repositoryRoot = FindRepositoryRoot();
        var viewsRoot = Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views");
        var violations = new List<string>();

        foreach (var file in Directory.EnumerateFiles(
                     viewsRoot,
                     "*.jalxaml",
                     SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(file);
            var relativePath = Path.GetRelativePath(repositoryRoot, file);

            foreach (Match match in ButtonContentPattern.Matches(source))
            {
                var content = Decode(match.Groups["content"].Value);
                if (!content.StartsWith('{') && LooksLikeTextIcon(content))
                {
                    violations.Add($"{relativePath}: Button Content=\"{content}\"");
                }
            }

            foreach (Match match in TextBlockTextPattern.Matches(source))
            {
                var text = Decode(match.Groups["text"].Value);
                if (IsStandaloneSymbol(text))
                {
                    violations.Add($"{relativePath}: TextBlock Text=\"{text}\"");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Use SymbolIcon, PathIcon, or Path for functional iconography:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void CommandModels_ExposeTypedOrFrameworkOwnedIconMetadata()
    {
        var standardViewModel = new StandardUICommandViewModel();
        var standard = Assert.IsType<Jalium.UI.Input.StandardUICommand>(
            standardViewModel.Commands[0].Command);
        Assert.IsType<string>(standard.IconSource);

        var viewModel = new XamlUICommandViewModel();
        Assert.IsType<Jalium.UI.Input.XamlUICommand>(
            viewModel.AddToFavoritesCommand);
        Assert.IsType<string>(viewModel.AddToFavoritesCommand.IconSource);
        Assert.False(string.IsNullOrWhiteSpace(
            (string)viewModel.AddToFavoritesCommand.IconSource!));
    }

    [Fact]
    public void IconButtons_HaveAccessibleNamesOrTextContent()
    {
        var repositoryRoot = FindRepositoryRoot();
        var viewsRoot = Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views");
        var violations = new List<string>();

        foreach (var file in Directory.EnumerateFiles(
                     viewsRoot,
                     "*.jalxaml",
                     SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(file);
            var relativePath = Path.GetRelativePath(repositoryRoot, file);

            foreach (Match match in ButtonElementPattern.Matches(source))
            {
                var attributes = match.Groups["attributes"].Value;
                var body = match.Groups["body"].Value;
                if (!IconElementPattern.IsMatch(body) ||
                    Regex.IsMatch(
                        attributes,
                        """\bContent\s*=""",
                        RegexOptions.CultureInvariant) ||
                    Regex.IsMatch(
                        attributes,
                        """\bAutomationProperties\.Name\s*=""",
                        RegexOptions.CultureInvariant))
                {
                    continue;
                }

                var line = source.AsSpan(0, match.Index).Count('\n') + 1;
                violations.Add($"{relativePath}:{line}");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Icon and composite-content buttons need an accessible name:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void LocalizedOpenDemoLabels_DoNotEmbedDirectionalIcons()
    {
        var repositoryRoot = FindRepositoryRoot();
        var localizationRoot = Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Localization");
        var files = Directory.EnumerateFiles(
                localizationRoot,
                "Strings.*.jalxaml",
                SearchOption.TopDirectoryOnly)
            .ToArray();

        Assert.NotEmpty(files);
        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            var match = OpenDemoLabelPattern.Match(source);
            Assert.True(
                match.Success,
                $"Missing Loc.Home.OpenDemoFormat in {Path.GetFileName(file)}.");

            var label = Decode(match.Groups["text"].Value);
            var runes = label.EnumerateRunes().ToArray();
            Assert.True(
                runes.Length > 0 && !IsSymbol(runes[^1]),
                $"Use a typed icon beside the label in {Path.GetFileName(file)}, " +
                "not a directional text or emoji suffix.");
        }
    }

    private static bool LooksLikeTextIcon(string content)
    {
        var value = content.Trim();
        if (value is
            "<" or ">" or "|<" or ">|" or "+" or "-" or
            "B" or "I" or "R" or "U")
        {
            return true;
        }

        return value.Length > 0 && IsSymbol(value.EnumerateRunes().First());
    }

    private static bool IsStandaloneSymbol(string text)
    {
        var value = text.Trim();
        var runes = value.EnumerateRunes().ToArray();
        return runes.Length is > 0 and <= 3 && IsSymbol(runes[0]);
    }

    private static bool IsSymbol(Rune rune) =>
        Rune.GetUnicodeCategory(rune) is
            UnicodeCategory.MathSymbol or
            UnicodeCategory.CurrencySymbol or
            UnicodeCategory.ModifierSymbol or
            UnicodeCategory.OtherSymbol;

    private static string Decode(string value) =>
        WebUtility.HtmlDecode(value).Trim();

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(sourceFile)
            ?? throw new InvalidOperationException(
                "Unable to locate test source."));

        while (directory != null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "Jalium.UI.Gallery.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "Unable to locate repository root.");
    }
}
