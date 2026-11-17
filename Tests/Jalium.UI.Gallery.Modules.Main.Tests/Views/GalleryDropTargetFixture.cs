using System.Reflection;
using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Jalium.UI.Markup;
using Jalium.UI.Media;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryDropTargetFixture
{
    [Fact]
    public void RepeatedEnterAndLeaveUseTemplateStateWithoutReplacingBrushBindings()
    {
        _ = new App();
        var target = Create(DragDropTargetRole.ItemList);
        var source = new Border { Background = Brushes.Navy };
        target.SetBinding(Control.BackgroundProperty, new Binding(nameof(Border.Background)) { Source = source });
        var root = Assert.IsType<Border>(target.FindName("PART_Root"));
        var data = Text("Item A");
        Assert.Equal(DragDropEffects.Copy, Send(target, DragDrop.DragEnterEvent, data).Effects);
        Send(target, DragDrop.DragEnterEvent, data);
        Assert.True(target.IsDragOver);
        Assert.True(target.IsDropAllowed);
        Assert.Equal("Accepting", VisualStateManager.GetCurrentStateName(target, "DragStates"));
        Assert.Same(Brushes.Navy, target.Background);
        Assert.NotSame(target.Background, root.Background);
        Send(target, DragDrop.DragLeaveEvent, data);
        Assert.False(target.IsDragOver);
        Assert.Same(target.Background, root.Background);
        Assert.NotNull(BindingOperations.GetBindingBase(target, Control.BackgroundProperty));
    }

    [Fact]
    public void TextDropUpdatesTheExistingViewModelAndClearsTransientState()
    {
        _ = new App();
        var vm = ViewModel();
        var target = Create(DragDropTargetRole.ItemList, vm);
        var data = Text("Item A");
        Send(target, DragDrop.DragEnterEvent, data);
        Assert.Equal("Release to drop", vm.DropTargetHint);
        Send(target, DragDrop.DropEvent, data);
        Assert.Equal(["Item A"], vm.DroppedItems);
        Assert.Equal("Drop items here", vm.DropTargetHint);
        Assert.False(target.IsDragOver);
        Assert.False(target.IsDropAllowed);
        Assert.Equal("Idle", VisualStateManager.GetCurrentStateName(target, "DragStates"));
        Assert.Equal("Test target", target.GetAutomationPeer()!.GetName());
    }

    [Fact]
    public void FileTargetsRejectTextAndDeliverOnlyNonEmptyFilePayloads()
    {
        _ = new App();
        var vm = ViewModel();
        var target = Create(DragDropTargetRole.ShellFiles, vm);
        var rejected = Send(target, DragDrop.DragEnterEvent, Text("not a file"));
        Assert.Equal(DragDropEffects.None, rejected.Effects);
        Assert.Equal("Rejecting", VisualStateManager.GetCurrentStateName(target, "DragStates"));
        Send(target, DragDrop.DropEvent, Text("not a file"));
        Assert.Empty(vm.ShellDroppedFiles);
        var files = new DataObject();
        files.SetData(DataFormats.FileDrop, new[] { "/tmp/first.txt", "/tmp/second.txt" });
        Assert.Equal(DragDropEffects.Copy, Send(target, DragDrop.DragEnterEvent, files).Effects);
        Send(target, DragDrop.DropEvent, files);
        Assert.Equal(["first.txt", "second.txt"], vm.ShellDroppedFiles);
        Assert.False(target.IsDragOver);
    }

    [Fact]
    public void EffectsDisabledAndAllowDropAreEnforcedBeforeDelivery()
    {
        _ = new App();
        var vm = ViewModel();
        var target = Create(DragDropTargetRole.ItemList, vm);
        var data = Text("Item A");
        Assert.Equal(DragDropEffects.None, Send(target, DragDrop.DragEnterEvent, data, DragDropEffects.Move).Effects);
        target.IsEnabled = false;
        Assert.False(target.IsDragOver);
        Assert.Equal("Drop items here", vm.DropTargetHint);
        Send(target, DragDrop.DropEvent, data);
        Assert.Empty(vm.DroppedItems);
        target.IsEnabled = true;
        target.AllowDrop = false;
        Assert.Equal(DragDropEffects.None, Send(target, DragDrop.DragEnterEvent, data).Effects);
        Send(target, DragDrop.DropEvent, data);
        Assert.Empty(vm.DroppedItems);
    }

    [Fact]
    public void CommandAcceptanceUsesACopiedPayloadWithoutDuplicateSinkDelivery()
    {
        _ = new App();
        var vm = ViewModel();
        var target = Create(DragDropTargetRole.ShellFiles, vm);
        var command = new CaptureCommand();
        target.DropCommand = command;
        var paths = new[] { "/tmp/original.txt" };
        var data = new DataObject(); data.SetData(DataFormats.FileDrop, paths);
        Assert.Equal(DragDropEffects.None, Send(target, DragDrop.DragEnterEvent, data).Effects);
        command.Allowed = true;
        Assert.Equal(DragDropEffects.Copy, Send(target, DragDrop.DragOverEvent, data).Effects);
        Send(target, DragDrop.DropEvent, data);
        var request = Assert.IsType<GalleryDropRequest>(command.Value);
        paths[0] = "/tmp/changed.txt";
        Assert.Equal("/tmp/original.txt", Assert.Single(request.Files));
        Assert.Equal("Test target", request.TargetName);
        Assert.Empty(vm.ShellDroppedFiles);
    }

    [Fact]
    public void ContextUnloadAndTemplateChangesRetireThePreviousDrag()
    {
        _ = new App();
        var first = ViewModel(); var second = ViewModel();
        var target = Create(DragDropTargetRole.ItemList, first); var data = Text("Item B");
        Send(target, DragDrop.DragEnterEvent, data);
        target.DataContext = second;
        Assert.Equal("Drop items here", first.DropTargetHint);
        Assert.False(target.IsDragOver);
        Send(target, DragDrop.DragEnterEvent, data);
        var custom = new ControlTemplate(typeof(GalleryDropTarget)); custom.SetVisualTree(() => new Border());
        target.Template = custom; Layout(target);
        Assert.False(target.IsDragOver);
        Assert.Equal("Drop items here", second.DropTargetHint);
        Send(target, DragDrop.DropEvent, data);
        Assert.Equal(["Item B"], second.DroppedItems);
        Send(target, DragDrop.DragEnterEvent, data);
        var state = typeof(FrameworkElement).GetMethod("SetLoadedState", BindingFlags.Instance | BindingFlags.NonPublic)!;
        state.Invoke(target, [true]); state.Invoke(target, [false]);
        Assert.False(target.IsDragOver);
        Assert.Equal("Drop items here", second.DropTargetHint);
    }

    [Fact]
    public void RealPageAndIndependentPreviewUseTemplatedTargets()
    {
        _ = new App();
        var vm = ViewModel(); var page = new DragDropPage { DataContext = vm }; Layout(page);
        var targets = Walk(page).OfType<GalleryDropTarget>().ToArray();
        Assert.Equal(6, targets.Length);
        Assert.Equal(3, targets.Count(target => target.TargetRole == DragDropTargetRole.Feedback));
        var itemTarget = Assert.Single(targets.Where(target => target.TargetRole == DragDropTargetRole.ItemList));
        Send(itemTarget, DragDrop.DropEvent, Text("Real page"));
        Assert.Contains("Real page", vm.DroppedItems);
        var preview = Assert.IsAssignableFrom<FrameworkElement>(GalleryControlThemes.FindPreviewTemplate("Gallery.Preview.DragDropPage")!.LoadContent());
        preview.DataContext = vm; Layout(preview);
        Assert.Single(Walk(preview).OfType<GalleryDropTarget>());
    }

    private static GalleryDropTarget Create(DragDropTargetRole role, object? data = null)
    {
        var target = new GalleryDropTarget { TargetRole = role, TargetName = "Test target", DataContext = data, Content = "Drop" };
        Layout(target); return target;
    }
    private static DragDropViewModel ViewModel() => new(Mock.Of<IGalleryDragFileService>());
    private static DataObject Text(string value) { var data = new DataObject(); data.SetText(value); return data; }
    private static DragEventArgs Send(GalleryDropTarget target, RoutedEvent route, IDataObject data, DragDropEffects effects = DragDropEffects.Copy)
    {
        var args = new DragEventArgs(route, data, DragDropKeyStates.None, effects, new Point(4, 4)) { Source = target };
        target.RaiseEvent(args); return args;
    }
    private static IEnumerable<Visual> Walk(Visual visual)
    {
        yield return visual;
        for (int index = 0; index < visual.VisualChildrenCount; index++)
            foreach (var child in Walk(visual.GetVisualChild(index)!)) yield return child;
    }
    private static void Layout(UIElement element)
    {
        for (int pass = 0; pass < 3; pass++)
        {
            element.Measure(new Size(1000, 2400)); element.Arrange(new Rect(0, 0, 1000, element.DesiredSize.Height)); element.UpdateLayout();
        }
    }
    private sealed class CaptureCommand : ICommand
    {
        public bool Allowed { get; set; }
        public object? Value { get; private set; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => Allowed;
        public void Execute(object? parameter) => Value = parameter;
    }
}
