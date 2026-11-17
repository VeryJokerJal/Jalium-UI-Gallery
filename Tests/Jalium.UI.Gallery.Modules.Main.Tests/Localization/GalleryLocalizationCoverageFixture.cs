using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Localization;

public sealed class GalleryLocalizationCoverageFixture
{
    [Theory]
    [InlineData("Strings", "zh-CN")]
    [InlineData("Strings", "ja-JP")]
    [InlineData("Strings", "ko-KR")]
    [InlineData("Pages", "zh-CN")]
    [InlineData("Pages", "ja-JP")]
    [InlineData("Pages", "ko-KR")]
    public void EveryLanguageDefinesTheCompleteResourceSet(string dictionary, string culture)
    {
        var directory = FindLocalizationDirectory();
        var english = ReadEntries(Path.Combine(directory, $"{dictionary}.en-US.jalxaml"));
        var localized = ReadEntries(Path.Combine(directory, $"{dictionary}.{culture}.jalxaml"));

        Assert.NotEmpty(english);
        Assert.Equal(english.Keys.Order(), localized.Keys.Order());
        Assert.All(localized, entry => Assert.False(
            string.IsNullOrWhiteSpace(entry.Value),
            $"{dictionary}.{culture}: {entry.Key} is empty."));
    }

    private static Dictionary<string, string> ReadEntries(string path)
    {
        XNamespace xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
        return XDocument.Load(path).Descendants()
            .Where(element => element.Attribute(xaml + "Key") != null)
            .ToDictionary(element => element.Attribute(xaml + "Key")!.Value,
                element => element.Value, StringComparer.Ordinal);
    }

    private static string FindLocalizationDirectory([CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(sourceFile)!);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Jalium.UI.Gallery.slnx")))
                return Path.Combine(directory.FullName, "Modules",
                    "Jalium.UI.Gallery.Modules.Main", "Localization");
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Cannot locate Gallery localization resources.");
    }
}
