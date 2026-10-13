using System.Runtime.CompilerServices;
using Jalium.UI.Controls;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>Owns only the platform-specific scroll preview interaction.</summary>
public static class ScrollViewerPreviewBehavior
{
    private static readonly ConditionalWeakTable<Page, State> States = new();
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(ScrollViewerPreviewBehavior), new PropertyMetadata(false, OnEnabledChanged));
    public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty)!;
    public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);
    private static void OnEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
        if (element is not Page page) return;
        if (States.TryGetValue(page, out var previous)) { previous.Detach(); States.Remove(page); }
        if (args.NewValue is true) States.Add(page, new State(page));
    }

    private sealed class State
    {
        private readonly Page _page;
        private readonly List<(Button Button, RoutedEventHandler Handler)> _handlers = [];
        private ScrollViewer? _bounce;
        private Button? _release;
        private Button? _pull;
        public State(Page page)
        {
            _page = page;
            page.Loaded += OnLoaded;
            page.LayoutUpdated += OnLayoutUpdated;
            page.Unloaded += OnUnloaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs args) => Initialize();
        private void OnLayoutUpdated(object? sender, EventArgs args) => Initialize();
        private void Initialize()
        {
            if (_handlers.Count != 0 || _page.FindName("HorizontalPreviewViewer") is not ScrollViewer horizontal) return;
            if (_page.FindName("BouncePreviewControls") is FrameworkElement controls && !OperatingSystem.IsMacOS())
                controls.Visibility = Visibility.Collapsed;
            _bounce = _page.FindName("BouncePreviewViewer") as ScrollViewer;
            _pull = _page.FindName("PullBounceButton") as Button;
            _release = _page.FindName("ReleaseBounceButton") as Button;
            Connect("ScrollLeftButton", (_, _) => horizontal.PageLeft());
            Connect("ScrollRightButton", (_, _) => horizontal.PageRight());
            Connect("PullBounceButton", OnPull);
            Connect("ReleaseBounceButton", OnRelease);
            _page.LayoutUpdated -= OnLayoutUpdated;
        }
        private void Connect(string name, RoutedEventHandler handler)
        {
            if (_page.FindName(name) is Button button) { button.Click += handler; _handlers.Add((button, handler)); }
        }
        private void OnPull(object sender, RoutedEventArgs args)
        {
            _bounce?.ScrollToVerticalOffset(0);
            for (Visual? parent = _page.VisualParent; parent != null; parent = parent.VisualParent)
                if (parent is ScrollViewer viewer) viewer.ScrollToVerticalOffset(0);
            Gesture(240, MouseWheelPhase.Began);
            if (_release != null) _release.IsEnabled = true;
        }
        private void OnRelease(object sender, RoutedEventArgs args)
        {
            var restoreFocus = _release?.IsKeyboardFocusWithin == true;
            Gesture(0, MouseWheelPhase.Ended);
            if (_release != null) _release.IsEnabled = false;
            if (restoreFocus) _pull?.Focus();
        }
        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            if (_release != null) _release.IsEnabled = false;
        }
        private void Gesture(double delta, MouseWheelPhase phase)
        {
            if (_bounce is null) return;
            var wheel = new MouseWheelEventArgs(UIElement.PreviewMouseWheelEvent, new Point(20, 20), 0, delta, true,
                MouseButtonState.Released, MouseButtonState.Released, MouseButtonState.Released,
                MouseButtonState.Released, MouseButtonState.Released, ModifierKeys.None, Environment.TickCount,
                phase, MouseWheelPhase.None);
            _bounce.RaiseEvent(wheel);
            wheel.RoutedEvent = UIElement.MouseWheelEvent;
            _bounce.RaiseEvent(wheel);
        }
        public void Detach()
        {
            _page.Loaded -= OnLoaded; _page.LayoutUpdated -= OnLayoutUpdated; _page.Unloaded -= OnUnloaded;
            foreach (var (button, handler) in _handlers) button.Click -= handler;
        }
    }
}
