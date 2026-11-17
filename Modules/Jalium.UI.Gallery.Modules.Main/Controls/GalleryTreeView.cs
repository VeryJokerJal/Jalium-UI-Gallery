using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Controls;

/// <summary>A data-driven Gallery tree using the framework's hierarchical containers and input contracts.</summary>
public class GalleryTreeView : TreeView
{
    static GalleryTreeView()
    {
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.Label), value => ((SelectionTreeNode)value).Label);
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.Children), value => ((SelectionTreeNode)value).Children);
        PropertyAccessorRegistry.Register(typeof(SelectionTreeNode), nameof(SelectionTreeNode.IsExpanded), value => ((SelectionTreeNode)value).IsExpanded);
        // Retain TreeView's native style key. Only the data projection defaults
        // differ; theme and consumer styles can still replace ItemTemplate.
        ItemTemplateProperty.OverrideMetadata(typeof(GalleryTreeView), new PropertyMetadata(GalleryControlThemes.FindDataTemplate("Gallery.TreeNodes")));
    }

    public static readonly DependencyProperty PublishSelectionProperty = DependencyProperty.Register(nameof(PublishSelection), typeof(bool), typeof(GalleryTreeView), new PropertyMetadata(false));
    public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand), typeof(GalleryTreeView), new PropertyMetadata(null));
    public bool PublishSelection { get => (bool)GetValue(PublishSelectionProperty)!; set => SetValue(PublishSelectionProperty, value); }
    public ICommand? SelectionChangedCommand { get => (ICommand?)GetValue(SelectionChangedCommandProperty); set => SetValue(SelectionChangedCommandProperty, value); }

    public GalleryTreeView()
    {
        GalleryControlThemes.EnsureRegistered();
        SelectedItemChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, RoutedPropertyChangedEventArgs<object?> e)
    {
        if (!ReferenceEquals(e.OriginalSource, this)) return;
        if (SelectionChangedCommand?.CanExecute(e.NewValue) == true) SelectionChangedCommand.Execute(e.NewValue);
        if (!PublishSelection || DataContext is not ITreeViewStateSink sink) return;
        string? label = e.NewValue switch
        {
            SelectionTreeNode node => node.Label,
            TreeViewItem item => item.Header?.ToString(),
            _ => e.NewValue?.ToString(),
        };
        sink.UpdateTreeSelection(label);
    }
}
