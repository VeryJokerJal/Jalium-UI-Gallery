using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for SplitterPage.jalxaml demonstrating splitter functionality.
/// </summary>
public partial class SplitterPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Horizontal GridSplitter: resize left/right panels -->
    <Grid Height=""200"">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""*"" MinWidth=""100""/>
            <ColumnDefinition Width=""Auto""/>
            <ColumnDefinition Width=""*"" MinWidth=""100""/>
        </Grid.ColumnDefinitions>

        <Border Grid.Column=""0"" Background=""#252525"" Padding=""16"">
            <TextBlock Text=""Left Panel"" HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""/>
        </Border>

        <GridSplitter Grid.Column=""1"" Width=""6""
                      Background=""#3D3D3D""
                      ResizeBehavior=""PreviousAndNext""/>

        <Border Grid.Column=""2"" Background=""#303030"" Padding=""16"">
            <TextBlock Text=""Right Panel"" HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""/>
        </Border>
    </Grid>

    <!-- Vertical GridSplitter: resize top/bottom panels -->
    <Grid Height=""250"" Margin=""0,16,0,0"">
        <Grid.RowDefinitions>
            <RowDefinition Height=""*"" MinHeight=""50""/>
            <RowDefinition Height=""Auto""/>
            <RowDefinition Height=""*"" MinHeight=""50""/>
        </Grid.RowDefinitions>

        <Border Grid.Row=""0"" Background=""#252525"" Padding=""16"">
            <TextBlock Text=""Top Panel"" HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""/>
        </Border>

        <GridSplitter Grid.Row=""1"" Height=""6""
                      Background=""#3D3D3D""
                      HorizontalAlignment=""Stretch""
                      ResizeDirection=""Rows""/>

        <Border Grid.Row=""2"" Background=""#303030"" Padding=""16"">
            <TextBlock Text=""Bottom Panel"" HorizontalAlignment=""Center""
                       VerticalAlignment=""Center""/>
        </Border>
    </Grid>

    <!-- IDE-style multi-panel layout -->
    <Grid Height=""300"" Margin=""0,16,0,0"">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""150"" MinWidth=""100""/>
            <ColumnDefinition Width=""Auto""/>
            <ColumnDefinition Width=""*""/>
        </Grid.ColumnDefinitions>

        <Border Grid.Column=""0"" Background=""#202020"" Padding=""12"">
            <StackPanel Orientation=""Vertical"">
                <TextBlock Text=""Sidebar"" FontWeight=""SemiBold"" Margin=""0,0,0,12""/>
                <TextBlock Text=""Item 1"" Margin=""0,0,0,4""/>
                <TextBlock Text=""Item 2"" Margin=""0,0,0,4""/>
                <TextBlock Text=""Item 3""/>
            </StackPanel>
        </Border>

        <GridSplitter Grid.Column=""1"" Width=""6"" Background=""#3D3D3D""/>

        <Grid Grid.Column=""2"">
            <Grid.RowDefinitions>
                <RowDefinition Height=""2*"" MinHeight=""80""/>
                <RowDefinition Height=""Auto""/>
                <RowDefinition Height=""*"" MinHeight=""60""/>
            </Grid.RowDefinitions>

            <Border Grid.Row=""0"" Background=""#252525"" Padding=""16"">
                <TextBlock Text=""Main Content"" HorizontalAlignment=""Center""
                           VerticalAlignment=""Center""/>
            </Border>

            <GridSplitter Grid.Row=""1"" Height=""6""
                          Background=""#3D3D3D""
                          HorizontalAlignment=""Stretch""
                          ResizeDirection=""Rows""/>

            <Border Grid.Row=""2"" Background=""#303030"" Padding=""16"">
                <TextBlock Text=""Output / Console"" HorizontalAlignment=""Center""
                           VerticalAlignment=""Center""/>
            </Border>
        </Grid>
    </Grid>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace MyApp;

public partial class SplitterDemo : Page
{
    public SplitterDemo()
    {
        InitializeComponent();
        CreateResizableLayout();
    }

    private void CreateResizableLayout()
    {
        // Build an IDE-style layout with GridSplitters
        var mainGrid = new Grid { Height = 400 };

        // Define three columns: sidebar, splitter, content
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = new GridLength(200),
            MinWidth = 120
        });
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = GridLength.Auto
        });
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition
        {
            Width = new GridLength(1, GridUnitType.Star)
        });

        // Left sidebar panel
        var sidebar = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
            Padding = new Thickness(12)
        };
        var sidebarContent = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        sidebarContent.Children.Add(new TextBlock
        {
            Text = ""Explorer"",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 12)
        });
        for (int i = 1; i <= 5; i++)
        {
            sidebarContent.Children.Add(new TextBlock
            {
                Text = $""File{i}.cs"",
                Margin = new Thickness(0, 0, 0, 4)
            });
        }
        sidebar.Child = sidebarContent;
        Grid.SetColumn(sidebar, 0);
        mainGrid.Children.Add(sidebar);

        // Horizontal splitter
        var hSplitter = new GridSplitter
        {
            Width = 6,
            Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
            ResizeBehavior = GridResizeBehavior.PreviousAndNext
        };
        Grid.SetColumn(hSplitter, 1);
        mainGrid.Children.Add(hSplitter);

        // Right content area with vertical split
        var rightGrid = new Grid();
        rightGrid.RowDefinitions.Add(new RowDefinition
        {
            Height = new GridLength(2, GridUnitType.Star),
            MinHeight = 100
        });
        rightGrid.RowDefinitions.Add(new RowDefinition
        {
            Height = GridLength.Auto
        });
        rightGrid.RowDefinitions.Add(new RowDefinition
        {
            Height = new GridLength(1, GridUnitType.Star),
            MinHeight = 80
        });

        var editor = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(37, 37, 37)),
            Padding = new Thickness(16)
        };
        editor.Child = new TextBlock { Text = ""Code Editor Area"" };
        Grid.SetRow(editor, 0);
        rightGrid.Children.Add(editor);

        var vSplitter = new GridSplitter
        {
            Height = 6,
            Background = new SolidColorBrush(Color.FromRgb(61, 61, 61)),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ResizeDirection = GridResizeDirection.Rows
        };
        Grid.SetRow(vSplitter, 1);
        rightGrid.Children.Add(vSplitter);

        var output = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(48, 48, 48)),
            Padding = new Thickness(16)
        };
        output.Child = new TextBlock { Text = ""Output Panel"" };
        Grid.SetRow(output, 2);
        rightGrid.Children.Add(output);

        Grid.SetColumn(rightGrid, 2);
        mainGrid.Children.Add(rightGrid);

        ContentPanel.Children.Add(mainGrid);
    }
}";

    public SplitterPage()
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
