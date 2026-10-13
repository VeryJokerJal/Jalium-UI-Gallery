using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Input;

namespace Jalium.UI.Gallery.Modules.Main.Presentation;

/// <summary>
/// Keeps the catalog's pointer semantics aligned with its presentation: a
/// split workspace selects items for the inspector, while a stacked workspace
/// treats each card as a direct navigation target.
/// </summary>
public sealed class HomeCatalogListBox : ListBox
{
    private const double PanCancelThreshold = 8;

    public static readonly DependencyProperty IsDirectActivationEnabledProperty =
        DependencyProperty.Register(
            nameof(IsDirectActivationEnabled),
            typeof(bool),
            typeof(HomeCatalogListBox),
            new PropertyMetadata(false, OnDirectActivationChanged));

    public static readonly DependencyProperty ActivateItemCommandProperty =
        DependencyProperty.Register(
            nameof(ActivateItemCommand),
            typeof(ICommand),
            typeof(HomeCatalogListBox),
            new PropertyMetadata(null));

    private uint? _activePointerId;
    private ListBoxItem? _pressedContainer;
    private Point _pressPoint;
    private bool _isTapCandidate;

    public HomeCatalogListBox()
    {
        AddHandler(
            PointerDownEvent,
            new PointerDownEventHandler(OnPointerDown),
            handledEventsToo: true);
        AddHandler(
            PointerMoveEvent,
            new PointerMoveEventHandler(OnPointerMove),
            handledEventsToo: true);
        AddHandler(
            PointerUpEvent,
            new PointerUpEventHandler(OnPointerUp),
            handledEventsToo: true);
        AddHandler(
            PointerCancelEvent,
            new PointerCancelEventHandler(OnPointerCancel),
            handledEventsToo: true);
        AddHandler(
            KeyDownEvent,
            new KeyEventHandler(OnKeyDown),
            handledEventsToo: true);
    }

    public bool IsDirectActivationEnabled
    {
        get => (bool)(GetValue(IsDirectActivationEnabledProperty) ?? false);
        set => SetValue(IsDirectActivationEnabledProperty, value);
    }

    public ICommand? ActivateItemCommand
    {
        get => GetValue(ActivateItemCommandProperty) as ICommand;
        set => SetValue(ActivateItemCommandProperty, value);
    }

    private void OnPointerDown(object sender, PointerDownEventArgs args)
    {
        ResetPointerState();
        if (!IsDirectActivationEnabled ||
            !IsPrimaryActivationPress(args.Pointer) ||
            args.OriginalSource is not DependencyObject source ||
            ContainerFromElement(source) is not ListBoxItem container)
        {
            return;
        }

        _activePointerId = args.Pointer.PointerId;
        _pressedContainer = container;
        _pressPoint = args.Pointer.Position;
        _isTapCandidate = true;
    }

    private void OnPointerMove(object sender, PointerMoveEventArgs args)
    {
        if (!_isTapCandidate ||
            args.Pointer.PointerId != _activePointerId)
        {
            return;
        }

        var deltaX = args.Pointer.Position.X - _pressPoint.X;
        var deltaY = args.Pointer.Position.Y - _pressPoint.Y;
        if ((deltaX * deltaX) + (deltaY * deltaY) >
            PanCancelThreshold * PanCancelThreshold)
        {
            _isTapCandidate = false;
        }
    }

    private void OnPointerUp(object sender, PointerUpEventArgs args)
    {
        if (args.Pointer.PointerId != _activePointerId)
        {
            return;
        }

        var pressedContainer = _pressedContainer;
        var shouldActivate =
            _isTapCandidate &&
            IsPrimaryActivationRelease(args.Pointer) &&
            args.OriginalSource is DependencyObject source &&
            ReferenceEquals(
                ContainerFromElement(source),
                pressedContainer);
        ResetPointerState();

        if (shouldActivate && pressedContainer != null)
        {
            ActivateContainer(pressedContainer);
        }
    }

    private void OnPointerCancel(object sender, PointerCancelEventArgs args)
    {
        if (args.Pointer.PointerId == _activePointerId)
        {
            ResetPointerState();
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs args)
    {
        if (!IsDirectActivationEnabled ||
            args.Handled ||
            args.IsRepeat ||
            args.Key != Key.Enter ||
            SelectedItem == null)
        {
            return;
        }

        var command = ActivateItemCommand;
        if (command?.CanExecute(SelectedItem) == true)
        {
            command.Execute(SelectedItem);
            args.Handled = true;
        }
    }

    private void ActivateContainer(ListBoxItem container)
    {
        var item = ItemContainerGenerator.ItemFromContainer(container);
        var command = ActivateItemCommand;
        if (item == null || command?.CanExecute(item) != true)
        {
            return;
        }

        SelectedItem = item;
        command.Execute(item);
    }

    private void ResetPointerState()
    {
        _activePointerId = null;
        _pressedContainer = null;
        _isTapCandidate = false;
    }

    private static bool IsPrimaryActivationPress(PointerPoint pointer)
    {
        if (!pointer.IsInContact)
        {
            return false;
        }

        return pointer.PointerDeviceType switch
        {
            PointerDeviceType.Mouse =>
                pointer.Properties.IsPrimary &&
                pointer.Properties.PointerUpdateKind ==
                PointerUpdateKind.LeftButtonPressed,
            PointerDeviceType.Pen =>
                !pointer.Properties.IsBarrelButtonPressed &&
                !pointer.Properties.IsEraser,
            _ => pointer.Properties.IsPrimary
        };
    }

    private static bool IsPrimaryActivationRelease(PointerPoint pointer)
    {
        return pointer.PointerDeviceType switch
        {
            PointerDeviceType.Mouse =>
                pointer.Properties.IsPrimary &&
                pointer.Properties.PointerUpdateKind ==
                PointerUpdateKind.LeftButtonReleased,
            PointerDeviceType.Pen =>
                !pointer.Properties.IsBarrelButtonPressed &&
                !pointer.Properties.IsEraser,
            _ => pointer.Properties.IsPrimary
        };
    }

    private static void OnDirectActivationChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is HomeCatalogListBox listBox &&
            args.NewValue is not true)
        {
            listBox.ResetPointerState();
        }
    }
}
