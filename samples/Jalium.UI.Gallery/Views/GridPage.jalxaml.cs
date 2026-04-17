using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class GridPage : Page
{
    private const string XamlExample = @"<!-- Application-style layout with Grid -->
<Grid Margin=""16"">
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto""/>
        <RowDefinition Height=""*""/>
        <RowDefinition Height=""Auto""/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""200""/>
        <ColumnDefinition Width=""*""/>
        <ColumnDefinition Width=""250""/>
    </Grid.ColumnDefinitions>

    <!-- Header spanning all columns -->
    <Border Grid.Row=""0"" Grid.Column=""0"" Grid.ColumnSpan=""3""
            Background=""#0078D4"" Padding=""16,8"">
        <TextBlock Text=""Application Header""
                   FontSize=""18"" FontWeight=""Bold""
                   Foreground=""#FFFFFF""/>
    </Border>

    <!-- Left sidebar -->
    <Border Grid.Row=""1"" Grid.Column=""0""
            Background=""#252526"" Padding=""12"">
        <StackPanel Orientation=""Vertical"">
            <TextBlock Text=""Navigation"" FontWeight=""Bold""
                       Foreground=""#FFFFFF"" Margin=""0,0,0,12""/>
            <TextBlock Text=""Dashboard"" Foreground=""#CCCCCC"" Margin=""0,0,0,8""/>
            <TextBlock Text=""Settings"" Foreground=""#CCCCCC"" Margin=""0,0,0,8""/>
            <TextBlock Text=""Profile"" Foreground=""#CCCCCC""/>
        </StackPanel>
    </Border>

    <!-- Main content area -->
    <Border Grid.Row=""1"" Grid.Column=""1""
            Background=""#1E1E1E"" Padding=""16"">
        <TextBlock Text=""Main Content Area""
                   Foreground=""#FFFFFF""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>

    <!-- Right panel -->
    <Border Grid.Row=""1"" Grid.Column=""2""
            Background=""#2D2D2D"" Padding=""12"">
        <TextBlock Text=""Details Panel""
                   Foreground=""#FFFFFF""/>
    </Border>

    <!-- Footer spanning all columns -->
    <Border Grid.Row=""2"" Grid.Column=""0"" Grid.ColumnSpan=""3""
            Background=""#333333"" Padding=""8"">
        <TextBlock Text=""Status: Ready""
                   Foreground=""#888888""
                   HorizontalAlignment=""Center""/>
    </Border>
</Grid>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class GridLayoutPage : Page
{
    public GridLayoutPage()
    {
        InitializeComponent();
        BuildDynamicGrid();
    }

    private void BuildDynamicGrid()
    {
        var grid = new Grid();

        // Define rows: Auto header, Star content, Auto footer
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

        // Define columns: Fixed sidebar, Star content, Fixed panel
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });

        // Header spanning all columns
        var header = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Padding = new Thickness(16, 8, 16, 8),
            Child = new TextBlock
            {
                Text = ""Dynamic Grid Layout"",
                FontSize = 18,
                Foreground = new SolidColorBrush(Color.White)
            }
        };
        Grid.SetRow(header, 0);
        Grid.SetColumn(header, 0);
        Grid.SetColumnSpan(header, 3);
        grid.Children.Add(header);

        // Sidebar with row span
        var sidebar = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(37, 37, 38)),
            Padding = new Thickness(12)
        };
        Grid.SetRow(sidebar, 1);
        Grid.SetColumn(sidebar, 0);
        Grid.SetRowSpan(sidebar, 2);
        grid.Children.Add(sidebar);

        // Content area
        var content = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
            Padding = new Thickness(16),
            Child = new TextBlock
            {
                Text = ""Content"",
                Foreground = new SolidColorBrush(Color.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetRow(content, 1);
        Grid.SetColumn(content, 1);
        Grid.SetColumnSpan(content, 2);
        grid.Children.Add(content);

        ContentArea.Child = grid;
    }
}";

    public GridPage()
    {
        InitializeComponent();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }
}
