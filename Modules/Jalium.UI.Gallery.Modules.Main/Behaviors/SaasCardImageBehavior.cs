using System.Runtime.CompilerServices;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;
using Jalium.UI.Threading;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Loads the original component-card artwork asynchronously for an Image.
/// Loading and cache management stay in the View layer; the ViewModel exposes
/// only the component page tag.
/// </summary>
public static class SaasCardImageBehavior
{
    private static readonly ConditionalWeakTable<Dispatcher, DispatcherLoadRegistry>
        LoadRegistries = new();

    public static readonly DependencyProperty PageTagProperty =
        DependencyProperty.RegisterAttached(
            "PageTag",
            typeof(string),
            typeof(SaasCardImageBehavior),
            new PropertyMetadata(null, OnPageTagChanged));

    public static void SetPageTag(
        DependencyObject element,
        string? value) =>
        element.SetValue(PageTagProperty, value);

    public static string? GetPageTag(DependencyObject element) =>
        element.GetValue(PageTagProperty) as string;

    private static void OnPageTagChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Image image)
        {
            return;
        }

        var pageTag = args.NewValue as string;
        if (string.IsNullOrWhiteSpace(pageTag))
        {
            image.Source = null;
            return;
        }

        if (SaasCardBackgrounds.TryGetSource(pageTag, out var cached))
        {
            image.Source = cached;
            return;
        }

        // A recycled card must never display the previous item's artwork while
        // its new source is being decoded.
        image.Source = null;
        RegisterPendingImage(image, pageTag);
    }

    private static void RegisterPendingImage(Image image, string pageTag)
    {
        var dispatcher = image.Dispatcher;
        var registry = LoadRegistries.GetValue(
            dispatcher,
            static _ => new DispatcherLoadRegistry());

        TagLoadState state;
        var startLoad = false;
        lock (registry.SyncRoot)
        {
            if (!registry.PendingLoads.TryGetValue(pageTag, out state!))
            {
                state = new TagLoadState();
                registry.PendingLoads.Add(pageTag, state);
                startLoad = true;
            }

            state.Waiters.Add(new WeakReference<Image>(image));
        }

        if (startLoad)
        {
            _ = LoadAndPublishAsync(
                dispatcher,
                registry,
                state,
                pageTag);
        }
    }

    private static async Task LoadAndPublishAsync(
        Dispatcher dispatcher,
        DispatcherLoadRegistry registry,
        TagLoadState state,
        string pageTag)
    {
        ImageSource source;
        try
        {
            // All Images waiting for the same tag share this one continuation.
            // Capturing the UI context once per recycled Image previously
            // generated 500+ dispatcher operations per second at wide widths.
            source = await SaasCardBackgrounds
                .GetSourceAsync(pageTag)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            lock (registry.SyncRoot)
            {
                if (registry.PendingLoads.TryGetValue(pageTag, out var pending) &&
                    ReferenceEquals(pending, state))
                {
                    registry.PendingLoads.Remove(pageTag);
                }
            }

            System.Diagnostics.Debug.WriteLine(
                $"Failed to load component card '{pageTag}': {exception}");
            return;
        }

        // Publish at Normal, not Background. The framework's own render loop
        // runs at Normal (RaiseRendering / ProcessRender / the virtualization
        // catch-up pass), and the dispatcher drains strictly highest-priority
        // first with a per-batch budget — so a Background item is structurally
        // starved for as long as scrolling keeps that Normal traffic flowing,
        // which is exactly when the blank card is on screen. Normal makes the
        // publish queue FIFO alongside the render work instead. The callback is
        // a property assignment, so it never competes for frame time.
        if (dispatcher.CheckAccess())
        {
            // Already on the UI thread — the source was cached and the await
            // above completed synchronously. Skip the queue entirely.
            PublishLoadedSource(registry, state, pageTag, source);
            return;
        }

        _ = dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            () => PublishLoadedSource(
                registry,
                state,
                pageTag,
                source));
    }

    private static void PublishLoadedSource(
        DispatcherLoadRegistry registry,
        TagLoadState state,
        string pageTag,
        ImageSource source)
    {
        WeakReference<Image>[] waiters;
        lock (registry.SyncRoot)
        {
            if (!registry.PendingLoads.TryGetValue(pageTag, out var pending) ||
                !ReferenceEquals(pending, state))
            {
                return;
            }

            registry.PendingLoads.Remove(pageTag);
            waiters = state.Waiters.ToArray();
        }

        foreach (var waiter in waiters)
        {
            // Matching PageTag is the whole correctness condition: it proves the
            // container still wants this artwork. Deliberately NOT gated on the
            // Image being in the visual tree — a container recycled out of view
            // while its decode was in flight would be skipped forever, because a
            // recycled container rebound to the SAME item never raises a Content
            // change and so never re-runs OnPageTagChanged, leaving Source at
            // null permanently. Assigning to a detached Image is harmless; if it
            // is later rebound to a different item, PageTag changes and this runs
            // again for the new tag.
            if (waiter.TryGetTarget(out var image) &&
                string.Equals(
                    GetPageTag(image),
                    pageTag,
                    StringComparison.OrdinalIgnoreCase))
            {
                image.Source = source;
            }
        }
    }

    private sealed class DispatcherLoadRegistry
    {
        internal object SyncRoot { get; } = new();

        internal Dictionary<string, TagLoadState> PendingLoads { get; } =
            new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class TagLoadState
    {
        internal List<WeakReference<Image>> Waiters { get; } = [];
    }
}
