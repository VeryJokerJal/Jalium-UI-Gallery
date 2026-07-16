using System.Security.Cryptography;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Themes;

public class SaasCardBackgroundsFixture
{
    [Fact]
    public void CatalogAndBackgroundAssetsHaveExactOneToOneCoverage()
    {
        var catalogTags = GalleryComponentCatalog.Items
            .Select(item => item.PageTag)
            .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var assetTags = SaasCardBackgrounds.AssetPageTags.ToArray();

        Assert.Equal(114, GalleryComponentCatalog.Items.Count);
        Assert.Equal(114, GalleryComponentCatalog.PageFactories.Count);
        Assert.Equal(114, catalogTags.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(114, assetTags.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(catalogTags, assetTags);
    }

    [Fact]
    public void EveryBackgroundIsUniqueAndDecodable()
    {
        var hashes = new HashSet<string>(StringComparer.Ordinal);

        foreach (var pageTag in SaasCardBackgrounds.AssetPageTags)
        {
            var bytes = SaasCardBackgrounds.ReadAssetBytes(pageTag);
            Assert.NotEmpty(bytes);
            Assert.True(hashes.Add(Convert.ToHexString(SHA256.HashData(bytes))));

            var source = ImageSourceLoader.FromBytes(bytes);
            try
            {
                Assert.True(source.Width > 0);
                Assert.True(source.Height > 0);
            }
            finally
            {
                (source as IDisposable)?.Dispose();
            }
        }

        Assert.Equal(114, hashes.Count);
    }
}
