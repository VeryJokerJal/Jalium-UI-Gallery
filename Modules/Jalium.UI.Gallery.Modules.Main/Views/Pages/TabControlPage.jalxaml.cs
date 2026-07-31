using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class TabControlPage : Page
{
    public TabControlPage()
    {
        InitializeComponent();
        CreateContent();
        LoadCodeExamples();
    }

    private void CreateContent()
    {
        if (DemoHost == null) return;

        // Basic TabControl.
        var basicTabControl = new TabControl
        {
            Width = 400,
            Height = 200
        };

        var tab1 = new TabItem { Header = "Home" };
        tab1.Content = CreateTabContent("Welcome to the Home tab!", GalleryTheme.AccentPrimaryBrush);
        basicTabControl.Items.Add(tab1);

        var tab2 = new TabItem { Header = "Profile" };
        tab2.Content = CreateTabContent("Your profile information goes here.", new SolidColorBrush(Color.FromRgb(76, 175, 80)));
        basicTabControl.Items.Add(tab2);

        var tab3 = new TabItem { Header = "Settings" };
        tab3.Content = CreateTabContent("Application settings and preferences.", new SolidColorBrush(Color.FromRgb(255, 152, 0)));
        basicTabControl.Items.Add(tab3);

        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Basic TabControl", "A simple TabControl with multiple tabs.", basicTabControl));

        // TabControl with Bottom placement.
        var bottomTabControl = new TabControl
        {
            Width = 400,
            Height = 200,
            TabStripPlacement = Dock.Bottom
        };

        var bottomTab1 = new TabItem { Header = "Tab 1" };
        bottomTab1.Content = CreateTabContent("Content for Tab 1", GalleryTheme.AccentPrimaryBrush);
        bottomTabControl.Items.Add(bottomTab1);

        var bottomTab2 = new TabItem { Header = "Tab 2" };
        bottomTab2.Content = CreateTabContent("Content for Tab 2", new SolidColorBrush(Color.FromRgb(156, 39, 176)));
        bottomTabControl.Items.Add(bottomTab2);

        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Bottom Tab Placement", "Tabs positioned at the bottom of the control.", bottomTabControl));

        // Selection changed event.
        var resultText = GalleryUi.ValueLabel("Selected tab: Home");
        resultText.Margin = new Thickness(0, 0, 0, 8);

        var eventTabControl = new TabControl
        {
            Width = 400,
            Height = 150
        };

        var eventTab1 = new TabItem { Header = "Home" };
        eventTab1.Content = CreateTabContent("Home content", GalleryTheme.AccentPrimaryBrush);
        eventTabControl.Items.Add(eventTab1);

        var eventTab2 = new TabItem { Header = "Documents" };
        eventTab2.Content = CreateTabContent("Documents content", new SolidColorBrush(Color.FromRgb(33, 150, 243)));
        eventTabControl.Items.Add(eventTab2);

        var eventTab3 = new TabItem { Header = "Downloads" };
        eventTab3.Content = CreateTabContent("Downloads content", new SolidColorBrush(Color.FromRgb(0, 150, 136)));
        eventTabControl.Items.Add(eventTab3);

        eventTabControl.SelectionChanged += (s, e) =>
        {
            if (eventTabControl.SelectedItem is TabItem selectedTab)
            {
                resultText.Text = $"Selected tab: {selectedTab.Header}";
            }
        };

        var eventStack = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        eventStack.Children.Add(resultText);
        eventStack.Children.Add(eventTabControl);

        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Selection Changed Event", "Responds to tab selection changes.", eventStack));
    }

    private Border CreateTabContent(string text, Brush accentColor)
    {
        var border = new Border
        {
            Background = GalleryTheme.BackgroundLightBrush,
            Padding = new Thickness(16)
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        var indicator = new Border
        {
            Width = 40,
            Height = 4,
            Background = accentColor,
            Margin = new Thickness(0, 0, 0, 12),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        stack.Children.Add(indicator);

        var contentText = new TextBlock
        {
            Text = text,
            Foreground = GalleryTheme.TextPrimaryBrush,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        stack.Children.Add(contentText);

        border.Child = stack;
        return border;
    }

    private const string XamlExample =
@"<TabControl Width=""400"" Height=""200"">
    <TabItem Header=""Home"">
        <TextBlock Text=""Welcome to Home tab""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </TabItem>
    <TabItem Header=""Profile"">
        <TextBlock Text=""Profile content""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </TabItem>
    <TabItem Header=""Settings"">
        <TextBlock Text=""Settings content""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </TabItem>
</TabControl>

<!-- Bottom Tab Placement -->
<TabControl TabStripPlacement=""Bottom""
            Width=""400"" Height=""200"">
    <TabItem Header=""Tab 1"" />
    <TabItem Header=""Tab 2"" />
</TabControl>";

    private const string CSharpExample =
@"// Create a TabControl programmatically
var tabControl = new TabControl
{
    Width = 400,
    Height = 200
};

// Add tabs
var tab1 = new TabItem { Header = ""Home"" };
tab1.Content = new TextBlock { Text = ""Home content"" };
tabControl.Items.Add(tab1);

var tab2 = new TabItem { Header = ""Profile"" };
tab2.Content = new TextBlock { Text = ""Profile content"" };
tabControl.Items.Add(tab2);

// Handle selection changes
tabControl.SelectionChanged += (s, e) =>
{
    if (tabControl.SelectedItem is TabItem selected)
    {
        Debug.WriteLine($""Selected: {selected.Header}"");
    }
};

// Bottom tab placement
tabControl.TabStripPlacement = Dock.Bottom;";

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
