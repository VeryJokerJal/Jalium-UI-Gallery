using System.ComponentModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Behaviors;

/// <summary>
/// Restores the original responsive catalog/detail layout while keeping
/// viewport-specific work out of HomePage code-behind.
/// </summary>
public static class HomeCatalogLayoutBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(HomeCatalogLayoutBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private static readonly DependencyProperty ControllerProperty =
        DependencyProperty.RegisterAttached(
            "Controller",
            typeof(Controller),
            typeof(HomeCatalogLayoutBehavior),
            new PropertyMetadata(null));

    public static void SetIsEnabled(
        DependencyObject element,
        bool value) =>
        element.SetValue(IsEnabledProperty, value);

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)(element.GetValue(IsEnabledProperty) ?? false);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Grid root)
        {
            return;
        }

        if (root.GetValue(ControllerProperty) is Controller oldController)
        {
            oldController.Detach();
            root.ClearValue(ControllerProperty);
        }

        if (args.NewValue is true)
        {
            var controller = new Controller(root);
            root.SetValue(ControllerProperty, controller);
            controller.Attach();
        }
    }

    private sealed class Controller
    {
        private readonly Grid _root;
        private HomeViewModel? _viewModel;
        private ListBox? _componentList;
        private bool _active;

        public Controller(Grid root)
        {
            _root = root;
        }

        public void Attach()
        {
            _root.Loaded += OnLoaded;
            _root.Unloaded += OnUnloaded;
            if (_root.IsLoaded)
            {
                Activate();
            }
        }

        public void Detach()
        {
            _root.Loaded -= OnLoaded;
            _root.Unloaded -= OnUnloaded;
            Deactivate();
        }

        private void OnLoaded(object sender, RoutedEventArgs args) =>
            Activate();

        private void OnUnloaded(object sender, RoutedEventArgs args) =>
            Deactivate();

        private void Activate()
        {
            if (_active)
            {
                return;
            }

            _active = true;
            _root.SizeChanged += OnRootSizeChanged;
            _root.DataContextChanged += OnDataContextChanged;
            AttachViewModel(_root.DataContext as HomeViewModel);

            _componentList =
                _root.FindName("ComponentList") as ListBox;
            if (_componentList != null)
            {
                _componentList.SizeChanged += OnComponentListSizeChanged;
            }

            Apply();
        }

        private void Deactivate()
        {
            if (!_active)
            {
                return;
            }

            _active = false;
            _root.SizeChanged -= OnRootSizeChanged;
            _root.DataContextChanged -= OnDataContextChanged;
            AttachViewModel(null);

            if (_componentList != null)
            {
                _componentList.SizeChanged -= OnComponentListSizeChanged;
                _componentList = null;
            }
        }

        private void OnDataContextChanged(
            object sender,
            DependencyPropertyChangedEventArgs args)
        {
            AttachViewModel(args.NewValue as HomeViewModel);
            Apply();
        }

        private void AttachViewModel(HomeViewModel? viewModel)
        {
            if (ReferenceEquals(_viewModel, viewModel))
            {
                return;
            }

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _viewModel = viewModel;
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(
            object? sender,
            PropertyChangedEventArgs args)
        {
            if (args.PropertyName is nameof(HomeViewModel.HasVisibleItems) or
                nameof(HomeViewModel.VisibleItems))
            {
                Apply();
            }
        }

        private void OnRootSizeChanged(
            object sender,
            SizeChangedEventArgs args) =>
            Apply();

        private void OnComponentListSizeChanged(
            object sender,
            SizeChangedEventArgs args) =>
            Apply();

        private void Apply()
        {
            if (!_active)
            {
                return;
            }

            var catalog =
                _root.FindName("CatalogPanel") as Grid;
            var detailColumn =
                _root.FindName("DetailColumn") as ColumnDefinition;
            var detail =
                _root.FindName("DetailPanel") as Border;
            if (catalog == null ||
                detailColumn == null ||
                detail == null)
            {
                return;
            }

            var hasVisibleItems = _viewModel?.HasVisibleItems == true;
            var width = _root.ActualWidth;
            if (width <= 0)
            {
                width = _root.DesiredSize.Width;
            }

            var splitView = width >= 1040;
            detail.Visibility = hasVisibleItems
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (splitView)
            {
                detailColumn.Width =
                    new GridLength(hasVisibleItems ? 360 : 0);
                Grid.SetColumn(detail, 1);
                Grid.SetRow(detail, 0);
                detail.Margin = new Thickness(0);
                detail.BorderThickness = new Thickness(1, 0, 0, 0);
                detail.Padding = new Thickness(22, 2, 0, 0);
                catalog.Margin = new Thickness(0);
            }
            else
            {
                detailColumn.Width = new GridLength(0);
                Grid.SetColumn(detail, 0);
                Grid.SetRow(detail, 1);
                detail.Margin = new Thickness(0, 24, 0, 0);
                detail.BorderThickness = new Thickness(0, 1, 0, 0);
                detail.Padding = new Thickness(0, 22, 0, 0);
                catalog.Margin = new Thickness(0);
            }

            ApplyCardLayout();
        }

        private void ApplyCardLayout()
        {
            if (_componentList == null)
            {
                return;
            }

            var panel =
                FindDescendant<UniformGrid>(_componentList);
            if (panel == null)
            {
                return;
            }

            var width = _root.ActualWidth;
            var hasVisibleItems = _viewModel?.HasVisibleItems == true;
            panel.Columns = width >= 1040
                ? hasVisibleItems
                    ? width >= 1180 ? 3 : 2
                    : 1
                : hasVisibleItems && width >= 680
                    ? 2
                    : 1;
        }

        private static T? FindDescendant<T>(Visual root)
            where T : Visual
        {
            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (var index = 0; index < childCount; index++)
            {
                if (VisualTreeHelper.GetChild(root, index) is not Visual child)
                {
                    continue;
                }

                if (child is T match)
                {
                    return match;
                }

                var nested = FindDescendant<T>(child);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
