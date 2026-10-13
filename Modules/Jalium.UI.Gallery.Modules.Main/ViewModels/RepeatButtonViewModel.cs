using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Counter state and commands for the RepeatButton demo.</summary>
public sealed class RepeatButtonViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <RepeatButton Delay="500"
                      Interval="100"
                      ToolTip="Decrease"
                      Command="{Binding DecrementCommand}">
            <SymbolIcon Symbol="Remove"/>
        </RepeatButton>
        <TextBlock Text="{Binding Count}"/>
        <RepeatButton Delay="500"
                      Interval="100"
                      ToolTip="Increase"
                      Command="{Binding IncrementCommand}">
            <SymbolIcon Symbol="Add"/>
        </RepeatButton>
        """;

    private const string CSharpCode = """
        // RepeatButtonViewModel.cs
        public sealed class RepeatButtonViewModel : ViewModelBase
        {
            public ICommand IncrementCommand { get; }
            public ICommand DecrementCommand { get; }

            // RepeatButton invokes these commands repeatedly while held.
        }
        """;

    private int _count;

    public RepeatButtonViewModel()
    {
        IncrementCommand = new RelayCommand(() => Count++);
        DecrementCommand = new RelayCommand(() => Count--);
    }

    public int Count
    {
        get => _count;
        private set => SetProperty(ref _count, value);
    }

    public ICommand IncrementCommand { get; }

    public ICommand DecrementCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;
}
