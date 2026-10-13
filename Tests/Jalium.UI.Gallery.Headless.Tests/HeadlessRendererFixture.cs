using System.Text.Json;
using Xunit;

namespace Jalium.UI.Gallery.Headless.Tests;

public sealed class HeadlessRendererFixture
{
    [Fact]
    public void Options_DefaultToHomeRouteWithDeterministicAppearance()
    {
        var options = HeadlessOptions.Parse([]);

        Assert.Equal(["home"], options.PageTags);
        Assert.Equal(HeadlessSurface.Route, options.Surface);
        Assert.Equal("en-US", options.LanguageName);
        Assert.False(options.DarkTheme);
        Assert.True(options.EmitSemantics);
    }

    [Fact]
    public void Options_RejectFullPageShell()
    {
        var exception = Assert.Throws<CommandLineException>(
            () => HeadlessOptions.Parse(["button", "--shell", "--full-page"]));

        Assert.Contains("--full-page", exception.Message, StringComparison.Ordinal);
        Assert.Contains("--shell", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ButtonPage_RendersPngAndSemanticSidecarWithoutPlatformWindow()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "jalium-gallery-headless-tests",
            Guid.NewGuid().ToString("N"));
        var pngPath = Path.Combine(directory, "button.png");
        var metadataPath = Path.Combine(directory, "button.json");
        try
        {
            var options = HeadlessOptions.Parse(
            [
                "button",
                "--output", pngPath,
                "--width", "640",
                "--height", "480",
                "--settle-passes", "2",
            ]);
            using var session = HeadlessGallerySession.Create(options);

            var report = session.Render(
                HeadlessCatalog.Require("button"),
                pngPath,
                metadataPath);

            Assert.False(report.ContentClipped && report.PaintedPixels == 0);
            Assert.True(report.PaintedPixels > 10_000);
            Assert.True(report.SemanticTextCount > 10);
            Assert.True(File.Exists(pngPath));
            Assert.True(File.Exists(metadataPath));

            var signature = File.ReadAllBytes(pngPath).AsSpan(0, 8).ToArray();
            Assert.Equal(
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                signature);

            using var metadata = JsonDocument.Parse(File.ReadAllText(metadataPath));
            var root = metadata.RootElement;
            Assert.False(root.GetProperty("platformWindowCreated").GetBoolean());
            Assert.Contains(
                root.GetProperty("semanticText").EnumerateArray(),
                element => element.GetProperty("text").GetString() == "Click Me");
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
