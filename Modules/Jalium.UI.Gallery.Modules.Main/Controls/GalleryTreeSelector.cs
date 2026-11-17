using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A Gallery tree selector with data templates and an optional selection-state command.</summary>
public class GalleryTreeSelector : TreeSelector
{
    static GalleryTreeSelector()
    {
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.Label), value => ((SelectionTreeNode)value).Label);
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.Children), value => ((SelectionTreeNode)value).Children);
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.IsExpanded), value => ((SelectionTreeNode)value).IsExpanded);
        ItemTemplateProperty.OverrideMetadata(typeof(GalleryTreeSelector), new PropertyMetadata(GalleryControlThemes.FindDataTemplate("Gallery.TreeSelectorNodes")));
        ItemContainerStyleProperty.OverrideMetadata(typeof(GalleryTreeSelector), new PropertyMetadata(GalleryControlThemes.FindStyle("Gallery.TreeSelectorItem")));
    }

    public static readonly DependencyProperty StateKeyProperty = DependencyProperty.Register(nameof(StateKey), typeof(string), typeof(GalleryTreeSelector), new PropertyMetadata(string.Empty, OnStateKeyChanged));
    public static readonly DependencyProperty StateChangedCommandProperty = DependencyProperty.Register(nameof(StateChangedCommand), typeof(ICommand), typeof(GalleryTreeSelector), new PropertyMetadata(null));
    public string StateKey { get => (string)GetValue(StateKeyProperty)!; set => SetValue(StateKeyProperty, value); }
    public ICommand? StateChangedCommand { get => (ICommand?)GetValue(StateChangedCommandProperty); set => SetValue(StateChangedCommandProperty, value); }

    public GalleryTreeSelector()
    {
        GalleryControlThemes.EnsureRegistered();
        SelectionChanged += (_, e) => { if (ReferenceEquals(e.OriginalSource, this)) PublishState(); };
        ItemCheckStateChanged += (_, _) => PublishState();
    }

    protected override void OnItemsSourceChanged(System.Collections.IEnumerable? oldValue, System.Collections.IEnumerable? newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        PublishState();
    }

    private static void OnStateKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((GalleryTreeSelector)d).PublishState();

    private void PublishState()
    {
        var label = SelectedItem switch
        {
            SelectionTreeNode node => node.Label,
            TreeSelectorItem item => item.Header?.ToString(),
            _ => SelectedItem?.ToString()
        };
        var state = new GalleryTreeSelectorState(StateKey, label, SelectedItems.Count, CheckedItems.Count);
        if (StateChangedCommand?.CanExecute(state) == true) StateChangedCommand.Execute(state);
        if (!string.IsNullOrWhiteSpace(StateKey) && DataContext is ITreeSelectorStateSink sink)
            sink.UpdateTreeSelectorState(state.Key, state.Label, state.SelectedCount, state.CheckedCount);
    }
}

public sealed record GalleryTreeSelectorState(string Key, string? Label, int SelectedCount, int CheckedCount);
