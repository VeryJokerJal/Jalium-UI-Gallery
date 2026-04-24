using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for EditControlPage.jalxaml demonstrating the EditControl functionality.
/// </summary>
public partial class EditControlPage : Page
{
    private const string JalxamlSample = """
        <Window xmlns="http://schemas.jalium.ui/2024"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                x:Class="Jalium.UI.Gallery.Modules.Main.Views.MainWindow"
                Title="Jalium UI Gallery"
                Width="1200" Height="800"
                Background="#1E1E1E">

            <!-- Navigation and content layout -->
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="280"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <!-- Sidebar navigation -->
                <NavigationView x:Name="NavView"
                                Grid.Column="0"
                                Background="#252526"
                                Foreground="#CCCCCC"/>

                <!-- Main content area with scroll -->
                <ScrollViewer Grid.Column="1"
                              Padding="32,24,32,24"
                              Background="{StaticResource WindowBackground}">
                    <Border x:Name="ContentArea"
                            Margin="0,0,0,16"/>
                </ScrollViewer>
            </Grid>

            <!--
                Multi-line comment example:
                This demonstrates the JALXAML syntax highlighting
                with proper multi-line comment support.
            -->

            <Window.Resources>
                <ResourceDictionary>
                    <ResourceDictionary.MergedDictionaries>
                        <ResourceDictionary Source="Themes/Colors.jalxaml"/>
                    </ResourceDictionary.MergedDictionaries>

                    <SolidColorBrush x:Key="ControlBackground" Color="#373737"/>
                    <SolidColorBrush x:Key="ControlBackgroundHover" Color="#3E3E3E"/>

                    <!-- Button style with triggers -->
                    <Style TargetType="Button">
                        <Setter Property="Background" Value="{StaticResource ControlBackground}"/>
                        <Setter Property="Foreground" Value="#FFFFFF"/>
                        <Setter Property="Height" Value="32"/>
                        <Setter Property="CornerRadius" Value="4"/>

                        <Style.Triggers>
                            <PropertyTrigger Property="IsMouseOver" Value="True">
                                <Setter Property="Background"
                                        Value="{StaticResource ControlBackgroundHover}"/>
                            </PropertyTrigger>
                        </Style.Triggers>
                    </Style>

                    <!-- Data template with bindings -->
                    <DataTemplate DataType="NavigationItem">
                        <StackPanel Orientation="Horizontal" Margin="8,4">
                            <TextBlock Text="{Binding Icon}" FontSize="16" Margin="0,0,8,0"/>
                            <TextBlock Text="{Binding Title}"
                                       Foreground="{DynamicResource TextForeground}"/>
                        </StackPanel>
                    </DataTemplate>

                    <!-- Control template with TemplateBinding -->
                    <ControlTemplate x:Key="RoundButton" TargetType="Button">
                        <Border Name="BackgroundBorder"
                                Background="{TemplateBinding Background}"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                CornerRadius="16">
                            <ContentPresenter HorizontalAlignment="Center"
                                              VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </ResourceDictionary>
            </Window.Resources>

            <!-- Complex binding examples -->
            <StackPanel Orientation="Vertical" Margin="16">
                <Slider x:Name="ValueSlider" Minimum="0" Maximum="100" Value="50"/>
                <TextBlock Text="{Binding Value, ElementName=ValueSlider, StringFormat='Value: {0:F0}'}"/>

                <!-- Attached properties and nested markup extensions -->
                <ComboBox Grid.Row="1" Grid.Column="2"
                          SelectedIndex="0"
                          Padding="{Binding Padding, RelativeSource={RelativeSource AncestorType=ComboBox}}">
                    <ComboBoxItem Content="Option A"/>
                    <ComboBoxItem Content="Option B"/>
                </ComboBox>

                <TextBox x:Name="SourceTextBox" Text="Hello World"/>
                <TextBlock Text="{Binding Text, ElementName=SourceTextBox, Mode=TwoWay}"/>

                <!-- Boolean and null values -->
                <CheckBox Content="Enable Feature"
                          IsChecked="True"
                          Visibility="Visible"
                          Tag="{x:Null}"/>
            </StackPanel>
        </Window>
        """;

    private const string CSharpSample = """
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Jalium.UI.Gallery;

        /// <summary>
        /// A sample class demonstrating C# syntax highlighting.
        /// </summary>
        public class Calculator
        {
            private readonly Dictionary<string, Func<double, double, double>> _operations = new()
            {
                { "+", (a, b) => a + b },
                { "-", (a, b) => a - b },
                { "*", (a, b) => a * b },
                { "/", (a, b) => b != 0 ? a / b : double.NaN }
            };

            /// <summary>
            /// Evaluates a simple expression.
            /// </summary>
            /// <param name="left">The left operand.</param>
            /// <param name="op">The operator (+, -, *, /).</param>
            /// <param name="right">The right operand.</param>
            /// <returns>The result of the operation.</returns>
            public double Evaluate(double left, string op, double right)
            {
                if (_operations.TryGetValue(op, out var operation))
                {
                    return operation(left, right);
                }

                throw new ArgumentException($"Unknown operator: '{op}'");
            }

            // Quick Fibonacci implementation
            public IEnumerable<long> Fibonacci(int count)
            {
                long a = 0, b = 1;
                for (int i = 0; i < count; i++)
                {
                    yield return a;
                    (a, b) = (b, a + b);
                }
            }

            /*
             * Multi-line comment block.
             * This tests the block comment span rule.
             */
            public static void Main(string[] args)
            {
                var calc = new Calculator();
                var result = calc.Evaluate(3.14, "+", 2.86);
                Console.WriteLine($"Result: {result}");

                // Print first 10 Fibonacci numbers
                foreach (var fib in calc.Fibonacci(10))
                {
                    Console.Write($"{fib} ");
                }

                #region Test Region
                var numbers = Enumerable.Range(1, 100)
                    .Where(n => n % 2 == 0)
                    .Select(n => n * n)
                    .ToList();
                #endregion
            }
        }
        """;

    private const string XamlCodeExample = """
        <!-- Basic EditControl with syntax highlighting -->
        <EditControl x:Name="MainEditor"
                     Height="400"
                     HorizontalAlignment="Stretch"
                     FontSize="14"
                     ShowLineNumbers="True"
                     HighlightCurrentLine="True"
                     TabSize="4"
                     ConvertTabsToSpaces="True"/>

        <!-- Read-only code viewer -->
        <EditControl x:Name="CodeViewer"
                     Height="200"
                     IsReadOnly="True"
                     ShowLineNumbers="True"
                     FontSize="13"/>

        <!-- Editor options -->
        <CheckBox x:Name="LineNumbersCheckBox"
                  Content="Show Line Numbers" IsChecked="True"/>
        <CheckBox x:Name="HighlightLineCheckBox"
                  Content="Highlight Current Line" IsChecked="True"/>
        <CheckBox x:Name="ReadOnlyCheckBox"
                  Content="Read Only"/>
        <Button x:Name="UndoButton" Content="Undo"/>
        <Button x:Name="RedoButton" Content="Redo"/>
        """;

    private const string CSharpCodeExample = """
        using Jalium.UI.Controls;
        using Jalium.UI.Controls.Editor;

        // Set up C# syntax highlighting
        editor.SyntaxHighlighter =
            RegexSyntaxHighlighter.CreateCSharpHighlighter();

        // Set up JALXAML syntax highlighting
        editor.SyntaxHighlighter =
            JalxamlSyntaxHighlighter.Create();

        // Load text content
        editor.LoadText("public class Hello { }");

        // Configure editor options
        editor.ShowLineNumbers = true;
        editor.HighlightCurrentLine = true;
        editor.TabSize = 4;
        editor.ConvertTabsToSpaces = true;
        editor.IsReadOnly = false;

        // Undo/Redo
        editor.Undo();
        editor.Redo();
        editor.SelectAll();

        // Track document changes
        editor.Document.Changed += (s, e) =>
        {
            var lineCount = editor.Document.LineCount;
            var length = editor.Document.TextLength;
        };

        // Get caret position
        var line = editor.Document.GetLineByOffset(
            editor.CaretOffset);
        int col = editor.CaretOffset - line.Offset;
        """;

    public EditControlPage()
    {
        InitializeComponent();
        SetupEventHandlers();
        LoadSampleCode();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlCodeExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpCodeExample);
        }
    }

    private void SetupEventHandlers()
    {
        if (LineNumbersCheckBox != null)
        {
            LineNumbersCheckBox.Checked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.ShowLineNumbers = true;
            };
            LineNumbersCheckBox.Unchecked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.ShowLineNumbers = false;
            };
        }

        if (HighlightLineCheckBox != null)
        {
            HighlightLineCheckBox.Checked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.HighlightCurrentLine = true;
            };
            HighlightLineCheckBox.Unchecked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.HighlightCurrentLine = false;
            };
        }

        if (ReadOnlyCheckBox != null)
        {
            ReadOnlyCheckBox.Checked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.IsReadOnly = true;
            };
            ReadOnlyCheckBox.Unchecked += (s, e) =>
            {
                if (MainEditor != null) MainEditor.IsReadOnly = false;
            };
        }

        if (UndoButton != null)
            UndoButton.Click += (s, e) => MainEditor?.Undo();

        if (RedoButton != null)
            RedoButton.Click += (s, e) => MainEditor?.Redo();

        if (SelectAllButton != null)
            SelectAllButton.Click += (s, e) => MainEditor?.SelectAll();

        if (ClearButton != null)
            ClearButton.Click += (s, e) => MainEditor?.LoadText(string.Empty);

        if (LoadSampleButton != null)
            LoadSampleButton.Click += (s, e) => LoadSampleCode();

        if (LoadJalxamlSampleButton != null)
            LoadJalxamlSampleButton.Click += (s, e) => LoadJalxamlSample();

        // Track caret position changes
        if (MainEditor != null)
        {
            MainEditor.Document.Changed += OnDocumentChanged;
        }
    }

    private void LoadSampleCode()
    {
        if (MainEditor == null) return;

        // Set up C# syntax highlighting
        MainEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();

        // Load sample text
        MainEditor.LoadText(CSharpSample);

        UpdateStatus();
    }

    private void LoadJalxamlSample()
    {
        if (MainEditor == null) return;

        MainEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
        MainEditor.LoadText(JalxamlSample);
        UpdateStatus();
    }

    private void OnDocumentChanged(object? sender, TextChangeEventArgs e)
    {
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (MainEditor == null) return;

        if (StatusText != null)
        {
            StatusText.Text = $"Lines: {MainEditor.Document.LineCount}  |  Length: {MainEditor.Document.TextLength}";
        }

        if (CursorPositionText != null)
        {
            var line = MainEditor.Document.GetLineByOffset(
                Math.Min(MainEditor.CaretOffset, MainEditor.Document.TextLength));
            int col = MainEditor.CaretOffset - line.Offset;
            CursorPositionText.Text = $"Ln {line.LineNumber}, Col {col}";
        }
    }
}
