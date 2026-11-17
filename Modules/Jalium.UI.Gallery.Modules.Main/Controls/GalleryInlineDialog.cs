using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A bindable in-place ContentDialog that owns request and completion lifetime.</summary>
public class GalleryInlineDialog : ContentDialog
{
    private ContentDialogRequest? _handledRequest;
    static GalleryInlineDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryInlineDialog), new PropertyMetadata(typeof(ContentDialog)));
    public GalleryInlineDialog()
    {
        Loaded += (_, _) => TryShowRequest();
        Unloaded += (_, _) => { if (IsShowing) Hide(); };
        Opened += (_, _) =>
        {
            SetValue(IsShowingPropertyKey, true);
            SetCurrentValue(ResultProperty, null);
        };
        Closed += (_, args) =>
        {
            SetValue(IsShowingPropertyKey, false);
            SetCurrentValue(ResultProperty, args.Result);
        };
    }
    public static readonly DependencyProperty RequestProperty = DependencyProperty.Register(
        nameof(Request), typeof(ContentDialogRequest), typeof(GalleryInlineDialog), new PropertyMetadata(null, OnRequestChanged));
    public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
        nameof(Result), typeof(ContentDialogResult?), typeof(GalleryInlineDialog),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    private static readonly DependencyPropertyKey IsShowingPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsShowing), typeof(bool), typeof(GalleryInlineDialog), new PropertyMetadata(false));
    public static readonly DependencyProperty IsShowingProperty = IsShowingPropertyKey.DependencyProperty;
    public ContentDialogRequest? Request { get => (ContentDialogRequest?)GetValue(RequestProperty); set => SetValue(RequestProperty, value); }
    public ContentDialogResult? Result { get => (ContentDialogResult?)GetValue(ResultProperty); set => SetValue(ResultProperty, value); }
    public bool IsShowing => (bool)GetValue(IsShowingProperty)!;

    private static void OnRequestChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) => ((GalleryInlineDialog)target).TryShowRequest();
    private void TryShowRequest()
    {
        if (Request is not { } request || ReferenceEquals(request, _handledRequest)) return;
        if (IsShowing) { _handledRequest = request; return; }
        // Detached previews and cached pages do not open a dialog in another window.
        if (!IsLoaded || VisualParent == null || Window.GetWindow(this) is not { Visibility: Visibility.Visible }) return;
        _handledRequest = request;
        SetValue(IsShowingPropertyKey, true);
        try { _ = ShowAsync(ContentDialogPlacement.InPlace); }
        catch { SetValue(IsShowingPropertyKey, false); throw; }
    }
}
