using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

public static class TreeDataGridBehavior
{
    public static readonly DependencyProperty ConfigureColumnsProperty =
        DependencyProperty.RegisterAttached(
            "ConfigureColumns",
            typeof(bool),
            typeof(TreeDataGridBehavior),
            new PropertyMetadata(false, OnConfigureColumnsChanged));

    public static readonly DependencyProperty RequestProperty =
        DependencyProperty.RegisterAttached(
            "Request",
            typeof(TreeDataGridRequest),
            typeof(TreeDataGridBehavior),
            new PropertyMetadata(null, OnRequestChanged));

    public static void SetConfigureColumns(
        DependencyObject element,
        bool value) =>
        element.SetValue(ConfigureColumnsProperty, value);

    public static bool GetConfigureColumns(DependencyObject element) =>
        (bool)(element.GetValue(ConfigureColumnsProperty) ?? false);

    public static void SetRequest(
        DependencyObject element,
        TreeDataGridRequest? value) =>
        element.SetValue(RequestProperty, value);

    public static TreeDataGridRequest? GetRequest(
        DependencyObject element) =>
        element.GetValue(RequestProperty) as TreeDataGridRequest;

    private static void OnConfigureColumnsChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeDataGrid grid ||
            args.NewValue is not true ||
            grid.Columns.Count > 0)
        {
            return;
        }

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Jalium.UI.Data.Binding("Name"),
            Width = 250
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Type",
            Binding = new Jalium.UI.Data.Binding("Type"),
            Width = 120
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Size",
            Binding = new Jalium.UI.Data.Binding("SizeDisplay"),
            Width = 100
        });
    }

    private static void OnRequestChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not TreeDataGrid grid ||
            args.NewValue is not TreeDataGridRequest request)
        {
            return;
        }

        switch (request.Action)
        {
            case TreeDataGridAction.ExpandAll:
                grid.ExpandAll();
                break;
            case TreeDataGridAction.CollapseAll:
                grid.CollapseAll();
                break;
        }
    }
}
