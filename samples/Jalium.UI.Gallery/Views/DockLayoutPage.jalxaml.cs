using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

public partial class DockLayoutPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- IDE Layout with DockLayout -->
    <DockLayout Height=""400"">
        <DockSplitPanel Orientation=""Horizontal"">
            <!-- Left Sidebar with tabs -->
            <DockTabPanel DockSplitPanel.Size=""220""
                          DockSplitPanel.MinSize=""150"">
                <DockItem Header=""Explorer"">
                    <StackPanel Margin=""12"">
                        <TextBlock Text=""Solution Explorer""
                                   FontWeight=""SemiBold"" Margin=""0,0,0,8""/>
                        <TextBlock Text=""  MyApp/""/>
                        <TextBlock Text=""    src/""/>
                        <TextBlock Text=""      Program.cs""/>
                        <TextBlock Text=""      MainWindow.cs""/>
                    </StackPanel>
                </DockItem>
                <DockItem Header=""Search"">
                    <StackPanel Margin=""12"">
                        <TextBox Margin=""0,0,0,8""/>
                        <TextBlock Text=""Type to search...""/>
                    </StackPanel>
                </DockItem>
            </DockTabPanel>

            <!-- Center and Bottom panels -->
            <DockSplitPanel Orientation=""Vertical"">
                <!-- Editor tabs -->
                <DockTabPanel>
                    <DockItem Header=""MainWindow.cs"">
                        <TextBlock Text=""// Code editor content"" Margin=""12""/>
                    </DockItem>
                    <DockItem Header=""App.cs"">
                        <TextBlock Text=""// App entry point"" Margin=""12""/>
                    </DockItem>
                </DockTabPanel>

                <!-- Bottom panel with output tabs -->
                <DockTabPanel DockSplitPanel.Size=""130""
                              DockSplitPanel.MinSize=""80"">
                    <DockItem Header=""Output"">
                        <TextBlock Text=""Build: 1 succeeded"" Margin=""8""/>
                    </DockItem>
                    <DockItem Header=""Terminal"">
                        <TextBlock Text=""PS> _"" Margin=""8""/>
                    </DockItem>
                    <DockItem Header=""Error List"">
                        <TextBlock Text=""0 Errors, 0 Warnings"" Margin=""8""/>
                    </DockItem>
                </DockTabPanel>
            </DockSplitPanel>
        </DockSplitPanel>
    </DockLayout>

    <!-- Three Column Layout with star sizing -->
    <DockLayout Height=""250"" Margin=""0,16,0,0"">
        <DockSplitPanel Orientation=""Horizontal"">
            <DockTabPanel DockSplitPanel.Size=""*"">
                <DockItem Header=""Files"">
                    <TextBlock Text=""File Browser"" Margin=""12""/>
                </DockItem>
            </DockTabPanel>
            <DockTabPanel DockSplitPanel.Size=""2*"">
                <DockItem Header=""Preview"">
                    <TextBlock Text=""Content Preview (2x width)"" Margin=""12""/>
                </DockItem>
            </DockTabPanel>
            <DockTabPanel DockSplitPanel.Size=""*"">
                <DockItem Header=""Properties"" CanClose=""False"">
                    <TextBlock Text=""Property Inspector"" Margin=""12""/>
                </DockItem>
            </DockTabPanel>
        </DockSplitPanel>
    </DockLayout>
</Page>";

    private const string CSharpExample = @"using Jalium.UI;
using Jalium.UI.Controls;

namespace MyApp;

public partial class DockLayoutDemo : Page
{
    public DockLayoutDemo()
    {
        InitializeComponent();
        SetupDockLayout();
    }

    private void SetupDockLayout()
    {
        var dockLayout = new DockLayout { Height = 500 };

        // Root horizontal split
        var rootSplit = new DockSplitPanel
        {
            Orientation = Orientation.Horizontal
        };

        // Left sidebar tab panel
        var sidebarTabs = new DockTabPanel();
        DockSplitPanel.SetSize(sidebarTabs, ""200"");
        DockSplitPanel.SetMinSize(sidebarTabs, ""120"");

        var explorerItem = new DockItem { Header = ""Explorer"" };
        var explorerContent = new StackPanel { Margin = new Thickness(12) };
        explorerContent.Children.Add(new TextBlock
        {
            Text = ""Solution Explorer"",
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 8)
        });
        explorerContent.Children.Add(new TextBlock { Text = ""Program.cs"" });
        explorerContent.Children.Add(new TextBlock { Text = ""MainWindow.cs"" });
        explorerItem.Content = explorerContent;
        sidebarTabs.Items.Add(explorerItem);

        var searchItem = new DockItem { Header = ""Search"" };
        searchItem.Content = new TextBox { Margin = new Thickness(12) };
        sidebarTabs.Items.Add(searchItem);

        rootSplit.Children.Add(sidebarTabs);

        // Center + bottom vertical split
        var centerSplit = new DockSplitPanel
        {
            Orientation = Orientation.Vertical
        };

        // Editor tab panel
        var editorTabs = new DockTabPanel();
        var mainFile = new DockItem { Header = ""MainWindow.cs"" };
        mainFile.Content = new TextBlock
        {
            Text = ""public class MainWindow : Window\n{\n    ...\n}"",
            Margin = new Thickness(12),
            FontFamily = new FontFamily(""Cascadia Code"")
        };
        editorTabs.Items.Add(mainFile);
        centerSplit.Children.Add(editorTabs);

        // Output tab panel
        var outputTabs = new DockTabPanel();
        DockSplitPanel.SetSize(outputTabs, ""120"");
        DockSplitPanel.SetMinSize(outputTabs, ""80"");

        var outputItem = new DockItem { Header = ""Output"" };
        outputItem.Content = new TextBlock
        {
            Text = ""Build succeeded."",
            Margin = new Thickness(8)
        };
        outputTabs.Items.Add(outputItem);

        var terminalItem = new DockItem { Header = ""Terminal"" };
        terminalItem.Content = new TextBlock
        {
            Text = ""PS> dotnet run"",
            Margin = new Thickness(8),
            FontFamily = new FontFamily(""Cascadia Code"")
        };
        outputTabs.Items.Add(terminalItem);

        centerSplit.Children.Add(outputTabs);
        rootSplit.Children.Add(centerSplit);

        dockLayout.Content = rootSplit;
        ContentPanel.Children.Add(dockLayout);
    }
}";

    public DockLayoutPage()
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
