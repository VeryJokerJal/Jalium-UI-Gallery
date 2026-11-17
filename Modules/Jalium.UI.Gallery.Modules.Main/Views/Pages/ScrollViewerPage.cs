using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>A templated scroll workbench with optional preview parts and safe template replacement.</summary>
[TemplatePart(Name = "HorizontalPreviewViewer", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "BouncePreviewViewer", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "ScrollLeftButton", Type = typeof(Button))]
[TemplatePart(Name = "ScrollRightButton", Type = typeof(Button))]
[TemplatePart(Name = "PullBounceButton", Type = typeof(Button))]
[TemplatePart(Name = "ReleaseBounceButton", Type = typeof(Button))]
public sealed class ScrollViewerPage : GalleryPage
{
    public static readonly DependencyProperty IsPreviewEnabledProperty = DependencyProperty.Register(
        nameof(IsPreviewEnabled), typeof(bool), typeof(ScrollViewerPage), new PropertyMetadata(true, OnPreviewEnabledChanged));
    private static readonly DependencyPropertyKey IsPullActivePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsPullActive), typeof(bool), typeof(ScrollViewerPage), new PropertyMetadata(false));
    public static readonly DependencyProperty IsPullActiveProperty = IsPullActivePropertyKey.DependencyProperty;
    private ScrollViewer? _horizontal, _bounce;
    private Button? _left, _right, _pull, _release;
    private bool _attached;

    public ScrollViewerPage() => Unloaded += OnUnloaded;
    public bool IsPreviewEnabled { get => (bool)GetValue(IsPreviewEnabledProperty)!; set => SetValue(IsPreviewEnabledProperty, value); }
    public bool IsPullActive => (bool)GetValue(IsPullActiveProperty)!;

    public override void OnApplyTemplate()
    {
        DetachHandlers();
        base.OnApplyTemplate();
        _horizontal = GetTemplateChild("HorizontalPreviewViewer") as ScrollViewer;
        _bounce = GetTemplateChild("BouncePreviewViewer") as ScrollViewer;
        _left = GetTemplateChild("ScrollLeftButton") as Button;
        _right = GetTemplateChild("ScrollRightButton") as Button;
        _pull = GetTemplateChild("PullBounceButton") as Button;
        _release = GetTemplateChild("ReleaseBounceButton") as Button;
        if (GetTemplateChild("BouncePreviewControls") is FrameworkElement controls)
            controls.SetCurrentValue(VisibilityProperty, OperatingSystem.IsMacOS() ? Visibility.Visible : Visibility.Collapsed);
        AttachHandlers();
    }

    protected override void OnTemplateChanging()
    {
        EndPull(false);
        DetachHandlers();
        _horizontal = _bounce = null;
        _left = _right = _pull = _release = null;
        base.OnTemplateChanging();
    }

    private void AttachHandlers()
    {
        if (_attached || !IsPreviewEnabled) return;
        if (_left != null) _left.Click += OnLeft;
        if (_right != null) _right.Click += OnRight;
        if (_pull != null) _pull.Click += OnPull;
        if (_release != null) _release.Click += OnRelease;
        _attached = true;
    }
    private void DetachHandlers()
    {
        if (!_attached) return;
        if (_left != null) _left.Click -= OnLeft;
        if (_right != null) _right.Click -= OnRight;
        if (_pull != null) _pull.Click -= OnPull;
        if (_release != null) _release.Click -= OnRelease;
        _attached = false;
    }
    private static void OnPreviewEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var page = (ScrollViewerPage)d;
        if (e.NewValue is true) page.AttachHandlers();
        else { page.EndPull(false); page.DetachHandlers(); }
    }
    private void OnLeft(object sender, RoutedEventArgs e) => _horizontal?.PageLeft();
    private void OnRight(object sender, RoutedEventArgs e) => _horizontal?.PageRight();
    private void OnPull(object sender, RoutedEventArgs e)
    {
        _bounce?.ScrollToVerticalOffset(0);
        for (var parent = VisualParent; parent != null; parent = (parent as Visual)?.VisualParent)
            if (parent is ScrollViewer viewer) viewer.ScrollToVerticalOffset(0);
        Gesture(240, MouseWheelPhase.Began);
        SetValue(IsPullActivePropertyKey, true);
    }
    private void OnRelease(object sender, RoutedEventArgs e) => EndPull(true);
    private void OnUnloaded(object sender, RoutedEventArgs e) => EndPull(false);
    private void EndPull(bool restoreFocus)
    {
        if (!IsPullActive) return;
        var hadFocus = restoreFocus && _release?.IsKeyboardFocusWithin == true;
        Gesture(0, MouseWheelPhase.Ended);
        SetValue(IsPullActivePropertyKey, false);
        if (hadFocus) _pull?.Focus();
    }
    private void Gesture(double delta, MouseWheelPhase phase)
    {
        if (_bounce == null) return;
        var args = new MouseWheelEventArgs(UIElement.PreviewMouseWheelEvent, new Point(20, 20), 0, delta, true,
            MouseButtonState.Released, MouseButtonState.Released, MouseButtonState.Released,
            MouseButtonState.Released, MouseButtonState.Released, ModifierKeys.None, Environment.TickCount,
            phase, MouseWheelPhase.None);
        _bounce.RaiseEvent(args);
        args.RoutedEvent = UIElement.MouseWheelEvent;
        _bounce.RaiseEvent(args);
    }
}
