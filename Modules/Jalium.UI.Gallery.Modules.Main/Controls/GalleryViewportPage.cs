using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>
/// A Gallery page whose template can size its body to the containing viewport.
/// The caller's Height value and binding remain owned by the caller.
/// </summary>
[TemplateVisualState(Name = "Bound", GroupName = "ViewportStates")]
[TemplateVisualState(Name = "Unbound", GroupName = "ViewportStates")]
public class GalleryViewportPage : GalleryPage
{
    public static readonly DependencyProperty IsViewportHeightEnabledProperty =
        DependencyProperty.Register(nameof(IsViewportHeightEnabled), typeof(bool), typeof(GalleryViewportPage),
            new PropertyMetadata(true, OnViewportOptionChanged));

    public static readonly DependencyProperty MinimumViewportHeightProperty =
        DependencyProperty.Register(nameof(MinimumViewportHeight), typeof(double), typeof(GalleryViewportPage),
            new PropertyMetadata(240.0, OnViewportOptionChanged),
            value => value is double height && double.IsFinite(height) && height >= 0);

    private static readonly DependencyPropertyKey ViewportHeightPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(ViewportHeight), typeof(double), typeof(GalleryViewportPage),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty ViewportHeightProperty = ViewportHeightPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey IsViewportBoundPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsViewportBound), typeof(bool), typeof(GalleryViewportPage),
            new PropertyMetadata(false));
    public static readonly DependencyProperty IsViewportBoundProperty = IsViewportBoundPropertyKey.DependencyProperty;

    /// <summary>Gets or sets whether the default template follows an ancestor ScrollViewer.</summary>
    public bool IsViewportHeightEnabled
    {
        get => (bool)GetValue(IsViewportHeightEnabledProperty)!;
        set => SetValue(IsViewportHeightEnabledProperty, value);
    }

    /// <summary>Gets or sets the minimum body height while following a viewport.</summary>
    public double MinimumViewportHeight
    {
        get => (double)GetValue(MinimumViewportHeightProperty)!;
        set => SetValue(MinimumViewportHeightProperty, value);
    }

    /// <summary>Gets the template body height, or Auto when no viewport is available.</summary>
    public double ViewportHeight => (double)GetValue(ViewportHeightProperty)!;
    public bool IsViewportBound => (bool)GetValue(IsViewportBoundProperty)!;

    private ScrollViewer? _host;
    private bool _suspended;
    private bool _listeningForLayout;
    private bool _refreshing;
    private double _measuredViewportHeight = double.NaN;

    static GalleryViewportPage() => VerticalAlignmentProperty.OverrideMetadata(typeof(GalleryViewportPage),
        new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsArrange));

    public GalleryViewportPage()
    {
        Loaded += OnViewportLoaded;
        Unloaded += OnViewportUnloaded;
        UpdateLayoutSubscription();
        RefreshViewport();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RefreshViewport();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        RefreshViewport();
        var desired = base.MeasureOverride(availableSize);
        _measuredViewportHeight = ViewportHeight;
        return double.IsFinite(ViewportHeight) ? new Size(desired.Width, ViewportHeight) : desired;
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == HeightProperty || e.Property == MarginProperty ||
            e.Property == MinHeightProperty || e.Property == MaxHeightProperty)
        {
            RefreshViewport();
        }
    }

    private void OnViewportLoaded(object sender, RoutedEventArgs e)
    {
        _suspended = false;
        UpdateLayoutSubscription();
        RefreshViewport();
    }

    private void OnViewportUnloaded(object sender, RoutedEventArgs e)
    {
        _suspended = true;
        UpdateLayoutSubscription();
        RefreshViewport();
    }

    private void UpdateLayoutSubscription()
    {
        bool listen = !_suspended && IsViewportHeightEnabled;
        if (_listeningForLayout == listen) return;
        _listeningForLayout = listen;
        if (listen) LayoutUpdated += OnViewportLayoutUpdated;
        else LayoutUpdated -= OnViewportLayoutUpdated;
    }

    private void OnViewportLayoutUpdated(object? sender, EventArgs e)
    {
        RefreshViewport();
        // A viewer can publish its new viewport after measuring its content.
        // Reconcile that second value after layout, when the parent can retain
        // the invalidation, rather than leaving the previous content extent.
        if (!_measuredViewportHeight.Equals(ViewportHeight)) InvalidateMeasure();
    }
    private void OnHostSizeChanged(object sender, SizeChangedEventArgs e) => RefreshViewport();
    private void OnHostScrollChanged(object sender, ScrollChangedEventArgs e) => RefreshViewport();

    private void SetHost(ScrollViewer? host)
    {
        if (ReferenceEquals(_host, host)) return;
        if (_host != null)
        {
            _host.SizeChanged -= OnHostSizeChanged;
            _host.ScrollChanged -= OnHostScrollChanged;
        }
        _host = host;
        if (_host != null)
        {
            _host.SizeChanged += OnHostSizeChanged;
            _host.ScrollChanged += OnHostScrollChanged;
        }
    }

    private ScrollViewer? FindHost()
    {
        // Only walk this page's ancestor chain. Other pages and descendants
        // retain their own sizing and subscription lifetimes.
        for (var ancestor = VisualParent; ancestor != null; ancestor = ancestor.VisualParent)
            if (ancestor is ScrollViewer viewer) return viewer;
        return null;
    }

    private void RefreshViewport()
    {
        if (_refreshing) return;
        _refreshing = true;
        try
        {
            SetHost(!_suspended && IsViewportHeightEnabled ? FindHost() : null);
            double height = double.NaN;
            if (_host != null)
            {
                height = _host.ViewportHeight;
                if (!double.IsFinite(height) || height <= 0)
                    height = _host.ActualHeight - _host.Padding.Top - _host.Padding.Bottom;
                if (double.IsFinite(height) && height > 0)
                {
                    height = double.IsNaN(Height)
                        ? Math.Max(MinimumViewportHeight, height - Margin.Top - Margin.Bottom)
                        : Height;
                    height = Math.Max(MinHeight, Math.Min(MaxHeight, height));
                }
                else height = double.NaN;
            }
            SetValue(ViewportHeightPropertyKey, height);
            SetValue(IsViewportBoundPropertyKey, _host != null);
            VisualStateManager.GoToState(this, IsViewportBound ? "Bound" : "Unbound", false);
        }
        finally { _refreshing = false; }
    }

    private static void OnViewportOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var page = (GalleryViewportPage)d;
        page.UpdateLayoutSubscription();
        page.RefreshViewport();
    }
}
