using System.Collections.Concurrent;
using System.Reflection;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;
using Jalium.UI.Media.Imaging;

namespace Jalium.UI.Gallery.Modules.Main.Themes;

/// <summary>
/// Resolves the compact SaaS illustrations used by the component catalog.
/// Image sources are cached because filtering and search rebuild card controls.
/// </summary>
internal static class SaasCardBackgrounds
{
    // Four decoders are enough to fill the viewport without scaling WIC/thread
    // overhead with the machine's core count during a fast scrollbar jump.
    private static readonly int MaxConcurrentDecodes =
        Math.Clamp(Environment.ProcessorCount / 2, 2, 4);
    internal const int CachedSourceLimit = 32;
    private const string ResourcePrefix =
        "Jalium.UI.Gallery.Modules.Main.Assets.SaasCardBackgrounds.";

    private static readonly Assembly ResourceAssembly = typeof(SaasCardBackgrounds).Assembly;
    private static readonly object CacheLock = new();
    private static readonly Dictionary<string, (ImageSource Source, LinkedListNode<string> Node)> SourceCache =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly LinkedList<string> SourceRecency = new();
    // Evict only our strong reference. A visible/recycled Image can still own
    // the source; a weak lookup lets it be shared again without another decode.
    private static readonly Dictionary<string, WeakReference<ImageSource>> WeakSources =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, Lazy<Task<ImageSource>>> PendingSourceLoads =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly SemaphoreSlim DecodeGate =
        new(initialCount: MaxConcurrentDecodes, maxCount: MaxConcurrentDecodes);
    private static Task? s_startupPrefetch;

    private static readonly IReadOnlyList<string> EmbeddedPageTags = ResourceAssembly
        .GetManifestResourceNames()
        .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal) &&
                       name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        .Select(name => name[ResourcePrefix.Length..^4])
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    internal static IReadOnlyList<string> AssetPageTags => EmbeddedPageTags;

    internal static int CachedSourceCount
    {
        get { lock (CacheLock) return SourceCache.Count; }
    }

    public static Image CreateImage()
    {
        return new Image
        {
            Stretch = Stretch.UniformToFill,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = false
        };
    }

    internal static bool TryGetSource(string pageTag, out ImageSource? source)
    {
        ValidatePageTag(pageTag);
        lock (CacheLock)
        {
            return TryGetSourceLocked(pageTag, out source);
        }
    }

    private static bool TryGetSourceLocked(string pageTag, out ImageSource? source)
    {
        if (SourceCache.TryGetValue(pageTag, out var entry))
        {
            SourceRecency.Remove(entry.Node);
            SourceRecency.AddFirst(entry.Node);
            source = entry.Source;
            return true;
        }

        if (WeakSources.TryGetValue(pageTag, out var weak) &&
            weak.TryGetTarget(out source))
        {
            RetainSourceLocked(pageTag, source);
            return true;
        }

        WeakSources.Remove(pageTag);
        source = null;
        return false;
    }

    private static void RetainSourceLocked(string pageTag, ImageSource source)
    {
        SourceCache.Add(pageTag, (source, SourceRecency.AddFirst(pageTag)));
        while (SourceCache.Count > CachedSourceLimit)
        {
            var oldest = SourceRecency.Last!;
            SourceCache.Remove(oldest.Value);
            SourceRecency.RemoveLast();
        }
    }

    /// <summary>
    /// Reads and decodes a card illustration away from the UI thread. A small,
    /// bounded decoder pool keeps fast scrollbar jumps responsive without
    /// flooding the thread pool, while the pending-task cache makes recycled
    /// containers share the same work. The frozen result can be safely
    /// published back to any UI thread.
    /// </summary>
    internal static Task<ImageSource> GetSourceAsync(string pageTag)
    {
        ValidatePageTag(pageTag);
        if (TryGetSource(pageTag, out var cached))
        {
            return Task.FromResult(cached!);
        }

        return PendingSourceLoads.GetOrAdd(
            pageTag,
            static tag => new Lazy<Task<ImageSource>>(
                () => DecodeSourceAsync(tag),
                LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }

    internal static void Prefetch(string pageTag)
    {
        if (TryGetSource(pageTag, out _))
        {
            return;
        }

        _ = ObservePrefetchAsync(pageTag);
    }

    internal static void BeginStartupPrefetch(IEnumerable<string> pageTags)
    {
        ArgumentNullException.ThrowIfNull(pageTags);

        var tags = pageTags
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (tags.Length == 0)
        {
            return;
        }

        var prefetch = ObserveStartupPrefetchAsync(tags);
        // A later application/viewport may request artwork that has since
        // left the bounded cache. Loads still coalesce through PendingSourceLoads.
        _ = Interlocked.Exchange(ref s_startupPrefetch, prefetch);
    }

    internal static void WaitForStartupPrefetch()
    {
        Volatile.Read(ref s_startupPrefetch)?.GetAwaiter().GetResult();
    }

    private static async Task ObserveStartupPrefetchAsync(string[] pageTags)
    {
        try
        {
            await Task.WhenAll(pageTags.Select(GetSourceAsync)).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Failed to prefetch the startup card backgrounds: {exception}");
        }
    }

    private static async Task ObservePrefetchAsync(string pageTag)
    {
        try
        {
            await GetSourceAsync(pageTag).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Failed to prefetch SaaS card background '{pageTag}': {exception}");
        }
    }

    private static async Task<ImageSource> DecodeSourceAsync(string pageTag)
    {
        await DecodeGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (TryGetSource(pageTag, out var cached))
            {
                return cached!;
            }

            var source = await Task.Run(() =>
            {
                // Catalog assets are guaranteed single-frame PNGs. Going
                // directly to BitmapImage avoids the general loader's separate
                // animation frame-count probe (a second WIC decoder pass).
                var decoded = BitmapImage.FromBytes(ReadAssetBytes(pageTag));
                if (decoded.CanFreeze)
                {
                    decoded.Freeze();
                }

                return decoded;
            }).ConfigureAwait(false);

            if (!source.IsFrozen)
            {
                (source as IDisposable)?.Dispose();
                throw new InvalidOperationException(
                    $"SaaS card background '{pageTag}' could not be frozen for cross-thread use.");
            }

            lock (CacheLock)
            {
                WeakSources[pageTag] = new WeakReference<ImageSource>(source);
                RetainSourceLocked(pageTag, source);
            }
            return source;
        }
        finally
        {
            DecodeGate.Release();
            PendingSourceLoads.TryRemove(pageTag, out _);
        }
    }

    internal static byte[] ReadAssetBytes(string pageTag)
    {
        ValidatePageTag(pageTag);
        var resourceName = GetResourceName(pageTag);
        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"SaaS card background '{resourceName}' was not embedded.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    internal static string GetResourceName(string pageTag)
    {
        ValidatePageTag(pageTag);
        return $"{ResourcePrefix}{pageTag}.png";
    }

    private static void ValidatePageTag(string pageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pageTag);
        if (!pageTag.All(char.IsAsciiLetterOrDigit))
        {
            throw new ArgumentException(
                "Gallery page tags may only contain ASCII letters and digits.",
                nameof(pageTag));
        }
    }
}
