using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>State and commands for the Button component demo.</summary>
public sealed class ButtonViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <!-- Basic Button -->
        <Button Content="Click Me"
                Width="120"
                Height="36"
                Command="{Binding IncrementCommand}"/>

        <TextBlock Text="{Binding CounterText}"/>

        <!-- Disabled Button -->
        <Button Content="Disabled"
                Width="120"
                Height="36"
                IsEnabled="False"/>

        <!-- Button with custom styling -->
        <Button Width="160"
                Height="40"
                Background="#0078D4"
                Foreground="#FFFFFF"
                FontSize="14"
                FontWeight="SemiBold"
                CornerRadius="4">
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="&#xE710; "
                           FontFamily="Segoe MDL2 Assets"/>
                <TextBlock Text="Add Item"/>
            </StackPanel>
        </Button>

        <!-- Button sizes -->
        <StackPanel Orientation="Horizontal" Spacing="8">
            <Button Content="Small"  Width="80"  Height="28" FontSize="12"/>
            <Button Content="Medium" Width="100" Height="32" FontSize="14"/>
            <Button Content="Large"  Width="120" Height="40" FontSize="16"/>
        </StackPanel>

        <!-- Button with ToolTip -->
        <Button Content="Hover Me"
                ToolTip="This is a helpful tooltip"
                Width="120"
                Height="36"/>
        """;

    private const string CSharpCode = """
        // ButtonViewModel.cs
        using System.Windows.Input;
        using Jalium.UI.Gallery.Core.Mvvm;

        namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

        public sealed class ButtonViewModel : ViewModelBase
        {
            private int _count;

            public ButtonViewModel()
            {
                IncrementCommand = new RelayCommand(Increment);
            }

            public int Count
            {
                get => _count;
                private set
                {
                    if (SetProperty(ref _count, value))
                        OnPropertyChanged(nameof(CounterText));
                }
            }

            public string CounterText => $"Count: {Count}";
            public ICommand IncrementCommand { get; }

            private void Increment() => Count++;
        }
        """;

    private int _count;

    public ButtonViewModel()
    {
        IncrementCommand = new RelayCommand(Increment);
    }

    public int Count
    {
        get => _count;
        private set
        {
            if (SetProperty(ref _count, value))
            {
                OnPropertyChanged(nameof(CounterText));
            }
        }
    }

    public string CounterText => $"Count: {Count}";

    public ICommand IncrementCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private void Increment() => Count++;
}
