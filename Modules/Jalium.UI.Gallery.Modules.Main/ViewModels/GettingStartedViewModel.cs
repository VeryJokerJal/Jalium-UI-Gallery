namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class GettingStartedViewModel
{
    public string InstallCommand { get; } =
        "dotnet add package Jalium.UI";

    public string CssExample { get; } = """
        <!-- A style sheet scoped to this subtree, declared in markup -->
        <StackPanel Css.StyleSheet="
                .demo        { background-color: #1e293b; border-radius: 8px; padding: 12px 16px;
                               transition: background-color 0.15s ease-out }
                .demo:hover  { background-color: #2563eb }
                .demo TextBlock { color: #e2e8f0; font-size: 13px }">

            <Border Css.Class="demo"><TextBlock Text="Hover me" /></Border>
        </StackPanel>

        <!-- Or an inline declaration block on one element -->
        <Border Css.Style="background-color: #0f766e; border-radius: 8px; padding: 12px 16px" />
        """;

    public string WindowExample { get; } = """
        using Jalium.UI;
        using Jalium.UI.Controls;

        public sealed class MainWindow : Window
        {
            public MainWindow()
            {
                Title = "My First App";
                Width = 800;
                Height = 600;
                Content = new TextBlock
                {
                    Text = "Hello, Jalium.UI!",
                    FontSize = 24
                };
            }
        }
        """;

    public override string XamlExample => """
        <Window xmlns="http://schemas.jalium.ui/2024"
                Title="My First App"
                Width="800"
                Height="600">
            <StackPanel Margin="20">
                <TextBlock Text="Welcome!" FontSize="24" />
                <Button Content="Click Me" Margin="0,10,0,0" />
            </StackPanel>
        </Window>
        """;

    public string BindingExample { get; } = """
        <TextBlock Text="{Binding UserName}" />
        <Button Content="Save"
                Command="{Binding SaveCommand}" />
        """;
}
