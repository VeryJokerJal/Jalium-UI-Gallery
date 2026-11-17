using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A copied drop payload that does not retain the native drag data object.</summary>
public sealed record GalleryDropRequest(
    DragDropTargetRole Role, string TargetName, string? Text, IReadOnlyList<string> Files);

/// <summary>A reusable, templated drop surface with optional command or Gallery state-sink delivery.</summary>
[TemplatePart(Name = "PART_Root", Type = typeof(Border))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Idle", GroupName = "DragStates")]
[TemplateVisualState(Name = "Accepting", GroupName = "DragStates")]
[TemplateVisualState(Name = "Rejecting", GroupName = "DragStates")]
public class GalleryDropTarget : ContentControl
{
    public static readonly DependencyProperty TargetRoleProperty = DependencyProperty.Register(
        nameof(TargetRole), typeof(DragDropTargetRole), typeof(GalleryDropTarget),
        new PropertyMetadata(DragDropTargetRole.None, OnTargetOptionChanged),
        value => value is DragDropTargetRole role && Enum.IsDefined(role));
    public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register(
        nameof(TargetName), typeof(string), typeof(GalleryDropTarget),
        new PropertyMetadata(string.Empty, OnTargetOptionChanged));
    public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register(
        nameof(DropCommand), typeof(ICommand), typeof(GalleryDropTarget),
        new PropertyMetadata(null, OnTargetOptionChanged));

    private static readonly DependencyPropertyKey IsDragOverPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsDragOver), typeof(bool), typeof(GalleryDropTarget), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDragOverProperty = IsDragOverPropertyKey.DependencyProperty;
    private static readonly DependencyPropertyKey IsDropAllowedPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsDropAllowed), typeof(bool), typeof(GalleryDropTarget), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDropAllowedProperty = IsDropAllowedPropertyKey.DependencyProperty;

    public DragDropTargetRole TargetRole
    {
        get => (DragDropTargetRole)GetValue(TargetRoleProperty)!;
        set => SetValue(TargetRoleProperty, value);
    }
    public string TargetName
    {
        get => (string)GetValue(TargetNameProperty)!;
        set => SetValue(TargetNameProperty, value);
    }
    /// <summary>Receives a GalleryDropRequest. When supplied, replaces state-sink drop delivery.</summary>
    public ICommand? DropCommand
    {
        get => (ICommand?)GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }
    public bool IsDragOver => (bool)GetValue(IsDragOverProperty)!;
    public bool IsDropAllowed => (bool)GetValue(IsDropAllowedProperty)!;

    private IDragDropStateSink? _enteredSink;
    private DragDropTargetRole _enteredRole;
    private string _enteredName = string.Empty;

    static GalleryDropTarget()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryDropTarget), new PropertyMetadata(typeof(GalleryDropTarget)));
        AllowDropProperty.OverrideMetadata(typeof(GalleryDropTarget), new PropertyMetadata(true));
    }

    public GalleryDropTarget()
    {
        UseTemplateContentManagement();
        GalleryControlThemes.EnsureRegistered();
        Unloaded += (_, _) => EndDrag(true);
    }

    protected override Jalium.UI.Automation.Peers.AutomationPeer? OnCreateAutomationPeer() => new DropTargetPeer(this);

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        EndDrag(true);
        UpdateStates();
    }

    protected override void OnIsEnabledChanged(bool oldValue, bool newValue)
    {
        base.OnIsEnabledChanged(oldValue, newValue);
        if (!newValue) EndDrag(true);
        UpdateStates();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == DataContextProperty || e.Property == AllowDropProperty)
            EndDrag(true);
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
        base.OnDragEnter(e);
        if (!e.Handled) UpdateDrag(e);
    }

    protected override void OnDragOver(DragEventArgs e)
    {
        base.OnDragOver(e);
        if (!e.Handled) UpdateDrag(e);
    }

    protected override void OnDragLeave(DragEventArgs e)
    {
        base.OnDragLeave(e);
        if (!e.Handled && IsDragOver)
        {
            EndDrag(true);
            e.Handled = true;
        }
    }

    protected override void OnDrop(DragEventArgs e)
    {
        base.OnDrop(e);
        if (e.Handled) return;
        bool allowed = TryCreateRequest(e, out var request);
        var command = DropCommand;
        var sink = DataContext as IDragDropStateSink;
        e.Effects = allowed ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = IsEnabled && AllowDrop && TargetRole != DragDropTargetRole.None;
        EndDrag(!allowed);
        if (!allowed) return;
        if (command != null) command.Execute(request);
        else if (request!.Role == DragDropTargetRole.ShellFiles)
            sink?.OnFilesDropped(request.TargetName, request.Files);
        else sink?.OnTextDropped(request!.Role, request.TargetName, request.Text!);
    }

    private void UpdateDrag(DragEventArgs e)
    {
        if (!IsEnabled || !AllowDrop || TargetRole == DragDropTargetRole.None)
        {
            e.Effects = DragDropEffects.None;
            EndDrag(true);
            return;
        }
        if (!IsDragOver)
        {
            _enteredSink = DataContext as IDragDropStateSink;
            _enteredRole = TargetRole;
            _enteredName = EffectiveName;
            SetValue(IsDragOverPropertyKey, true);
            _enteredSink?.OnDragEntered(_enteredRole, _enteredName);
        }
        bool allowed = TryCreateRequest(e, out _);
        SetValue(IsDropAllowedPropertyKey, allowed);
        e.Effects = allowed ? DragDropEffects.Copy : DragDropEffects.None;
        if (TargetRole == DragDropTargetRole.ShellFiles)
        {
            if (allowed) e.SetDropDescription(DropImageType.Copy, "Copy to %1", "Gallery demo");
            else e.SetDropDescription(DropImageType.None, "Only files are accepted");
        }
        e.Handled = true;
        UpdateStates();
    }

    private string EffectiveName => string.IsNullOrWhiteSpace(TargetName) ? Name : TargetName;

    private bool TryCreateRequest(DragEventArgs e, out GalleryDropRequest? request)
    {
        request = null;
        if (!IsEnabled || !AllowDrop || TargetRole == DragDropTargetRole.None ||
            (e.AllowedEffects & DragDropEffects.Copy) == 0) return false;
        if (TargetRole == DragDropTargetRole.ShellFiles)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] paths || paths.Length == 0) return false;
            var copy = paths.Where(path => !string.IsNullOrWhiteSpace(path)).ToArray();
            if (copy.Length == 0) return false;
            request = new GalleryDropRequest(TargetRole, EffectiveName, null, Array.AsReadOnly(copy));
        }
        else
        {
            var text = e.Data.GetData(DataFormats.UnicodeText) as string ?? e.Data.GetData(DataFormats.Text) as string;
            if (text == null) return false;
            request = new GalleryDropRequest(TargetRole, EffectiveName, text, Array.Empty<string>());
        }
        return DropCommand?.CanExecute(request) ?? true;
    }

    private void EndDrag(bool notifyLeave)
    {
        var sink = _enteredSink;
        var role = _enteredRole;
        var name = _enteredName;
        bool entered = IsDragOver;
        _enteredSink = null;
        _enteredName = string.Empty;
        SetValue(IsDragOverPropertyKey, false);
        SetValue(IsDropAllowedPropertyKey, false);
        UpdateStates();
        if (notifyLeave && entered) sink?.OnDragLeft(role, name);
    }

    private void UpdateStates()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", false);
        VisualStateManager.GoToState(this, !IsDragOver ? "Idle" : IsDropAllowed ? "Accepting" : "Rejecting", false);
    }

    private static void OnTargetOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GalleryDropTarget)d).EndDrag(true);

    private sealed class DropTargetPeer(GalleryDropTarget owner)
        : Jalium.UI.Automation.Peers.FrameworkElementAutomationPeer(owner)
    {
        protected override Jalium.UI.Automation.Peers.AutomationControlType GetAutomationControlTypeCore() =>
            Jalium.UI.Automation.Peers.AutomationControlType.Group;
        protected override string GetClassNameCore() => nameof(GalleryDropTarget);
        protected override string GetNameCore()
        {
            var explicitName = Jalium.UI.Automation.AutomationProperties.GetName(owner);
            if (!string.IsNullOrWhiteSpace(explicitName)) return explicitName;
            return string.IsNullOrWhiteSpace(owner.EffectiveName) ? base.GetNameCore() : owner.EffectiveName;
        }
    }
}
