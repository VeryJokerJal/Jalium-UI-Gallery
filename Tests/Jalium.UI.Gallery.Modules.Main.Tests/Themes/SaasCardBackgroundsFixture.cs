using System.Security.Cryptography;
using Jalium.UI;
using Jalium.UI.Media.Imaging;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Themes;

public class SaasCardBackgroundsFixture
{
    [Fact]
    public async Task RepeatedLoadsShareOneFrozenSource()
    {
        var pageTag = SaasCardBackgrounds.AssetPageTags[^1];

        var first = SaasCardBackgrounds.GetSourceAsync(pageTag);
        var second = SaasCardBackgrounds.GetSourceAsync(pageTag);

        var sources = await Task.WhenAll(first, second);
        var source = sources[0];
        Assert.Same(source, sources[1]);
        Assert.True(source.IsFrozen);
        Assert.True(SaasCardBackgrounds.TryGetSource(pageTag, out var cached));
        Assert.Same(source, cached);
    }

    [Fact]
    public void CatalogAndBackgroundAssetsHaveExactOneToOneCoverage()
    {
        var catalogTags = GalleryComponentCatalog.Items
            .Select(item => item.PageTag)
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var assetTags = SaasCardBackgrounds.AssetPageTags.ToArray();

        Assert.Equal(121, GalleryComponentCatalog.Items.Count);
        Assert.Equal(121, GalleryComponentCatalog.PagesByTag.Count);
        Assert.Equal(121, catalogTags.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(121, assetTags.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(catalogTags, assetTags);
    }

    [Fact]
    public async Task EveryBackgroundIsUniqueAndAsyncLoadable()
    {
        var hashes = new HashSet<string>(StringComparer.Ordinal);

        foreach (var pageTag in SaasCardBackgrounds.AssetPageTags)
        {
            var bytes = SaasCardBackgrounds.ReadAssetBytes(pageTag);
            Assert.NotEmpty(bytes);
            Assert.True(hashes.Add(Convert.ToHexString(SHA256.HashData(bytes))));

            var source = await SaasCardBackgrounds.GetSourceAsync(pageTag);
            Assert.True(source.Width > 0);
            Assert.True(source.Height > 0);
            Assert.True(source.IsFrozen);
        }

        Assert.Equal(121, hashes.Count);
    }

    [Fact]
    public async Task BurstLoadsCompleteAndShareFrozenSources()
    {
        var tags = SaasCardBackgrounds.AssetPageTags.Take(24).ToArray();
        var firstBurst = tags.Select(SaasCardBackgrounds.GetSourceAsync).ToArray();
        var secondBurst = tags.Select(SaasCardBackgrounds.GetSourceAsync).ToArray();

        var firstSources = await Task.WhenAll(firstBurst);
        var secondSources = await Task.WhenAll(secondBurst);

        Assert.All(firstSources, source => Assert.True(source.IsFrozen));
        for (var index = 0; index < tags.Length; index++)
        {
            Assert.Same(firstSources[index], secondSources[index]);
        }
    }

    [Fact]
    public void StartupPrefetchCanGuaranteeTheFirstViewportIsReady()
    {
        var tags = GalleryComponentCatalog.Items
            .Take(12)
            .Select(item => item.PageTag)
            .ToArray();

        SaasCardBackgrounds.BeginStartupPrefetch(tags);
        SaasCardBackgrounds.WaitForStartupPrefetch();

        Assert.All(
            tags,
            tag => Assert.True(
                SaasCardBackgrounds.TryGetSource(tag, out var source) &&
                source is { IsFrozen: true }));
    }

    [Fact]
    public async Task BrowsingTheWholeCatalogKeepsOnlyABoundedHotSet()
    {
        foreach (var tag in SaasCardBackgrounds.AssetPageTags)
        {
            _ = await SaasCardBackgrounds.GetSourceAsync(tag);
            Assert.InRange(SaasCardBackgrounds.CachedSourceCount, 1, 32);
        }
    }

    [Fact]
    public async Task CacheEvictionPreservesASourceStillHeldByAView()
    {
        var tags = SaasCardBackgrounds.AssetPageTags;
        var visibleSource = await SaasCardBackgrounds.GetSourceAsync(tags[0]);
        var bitmap = Assert.IsType<BitmapImage>(visibleSource);
        var before = new byte[384 * 208 * 4];
        bitmap.CopyPixels(new Int32Rect(0, 0, 384, 208), before, 384 * 4, 0);
        foreach (var tag in tags.Skip(1))
        {
            _ = await SaasCardBackgrounds.GetSourceAsync(tag);
        }

        Assert.InRange(SaasCardBackgrounds.CachedSourceCount, 1, 32);
        Assert.True(visibleSource.IsFrozen);
        Assert.Equal(384, visibleSource.Width);
        Assert.Equal(208, visibleSource.Height);
        Assert.Same(visibleSource, await SaasCardBackgrounds.GetSourceAsync(tags[0]));
        var after = new byte[before.Length];
        bitmap.CopyPixels(new Int32Rect(0, 0, 384, 208), after, 384 * 4, 0);
        Assert.Equal(before, after);
    }
}
