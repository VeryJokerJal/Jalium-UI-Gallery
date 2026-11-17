using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Logical drag position and commands for the Thumb demo.</summary>
public sealed class ThumbViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const double MaximumLeft = 380;

    private const string XamlCode = """
        <Thumb Canvas.Left="{Binding Left}"
               Canvas.Top="{Binding Top}"
               behaviors:ThumbCommandBehavior.DragStartedCommand=
                   "{Binding DragStartedCommand}"
               behaviors:ThumbCommandBehavior.DragDeltaCommand=
                   "{Binding DragDeltaCommand}"
               behaviors:ThumbCommandBehavior.DragCompletedCommand=
                   "{Binding DragCompletedCommand}" />
        """;

    private const string CSharpCode = """
        // ThumbViewModel.cs
        public sealed class ThumbViewModel : ViewModelBase
        {
            public double Left { get; private set; }
            public ICommand DragDeltaCommand { get; }

            // Drag events are converted to commands declaratively in JALXAML.
        }
        """;

    private double _left;
    private double _top = 2;
    private bool _isDragging;

    public ThumbViewModel()
    {
        DragStartedCommand = new RelayCommand(() => IsDragging = true);
        DragDeltaCommand = new RelayCommand<double>(
            change => Left = Math.Clamp(Left + change, 0, MaximumLeft));
        DragCompletedCommand = new RelayCommand(() => IsDragging = false);
    }

    public double Left
    {
        get => _left;
        private set
        {
            if (SetProperty(ref _left, value))
            {
                OnPropertyChanged(nameof(PositionText));
            }
        }
    }

    public double Top
    {
        get => _top;
        private set
        {
            if (SetProperty(ref _top, value))
            {
                OnPropertyChanged(nameof(PositionText));
            }
        }
    }

    public bool IsDragging
    {
        get => _isDragging;
        private set
        {
            if (SetProperty(ref _isDragging, value))
            {
                OnPropertyChanged(nameof(PositionText));
            }
        }
    }

    public string PositionText =>
        IsDragging
            ? $"Dragging: {Left:F0}, {Top:F0}"
            : $"Position: {Left:F0}, {Top:F0}";

    public ICommand DragStartedCommand { get; }

    public ICommand DragDeltaCommand { get; }

    public ICommand DragCompletedCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;
}
