using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Markup;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class SectionDemoPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Define a named section -->
    @section NavHeader {
        <Border Background=""#0078D4"" CornerRadius=""4"" Padding=""12,8"">
            <TextBlock Text=""Jalium.UI"" Foreground=""#FFFFFF""
                       FontSize=""16"" FontWeight=""Bold""/>
        </Border>
    }

    @section NavItems {
        <StackPanel Margin=""0,4,0,4"">
            <TextBlock Text=""  Home"" Foreground=""#CCCCCC"" FontSize=""13""/>
            <TextBlock Text=""  Components"" Foreground=""#CCCCCC"" FontSize=""13""/>
            <TextBlock Text=""  Settings"" Foreground=""#CCCCCC"" FontSize=""13""/>
        </StackPanel>
    }

    @section PageContent {
        <StackPanel>
            <TextBlock Text=""Welcome to the dashboard."" Foreground=""#AAAAAA""/>
            <Border Background=""#1A3A5C"" CornerRadius=""4"" Padding=""16"">
                <TextBlock Text=""Card rendered from section"" Foreground=""#88CCFF""/>
            </Border>
        </StackPanel>
    }

    <!-- Render sections in layout -->
    <StackPanel Orientation=""Horizontal"">
        <Border Background=""#252525"" Width=""200"" Padding=""12"">
            <StackPanel>
                @RenderSection(""NavHeader"")
                @RenderSection(""NavItems"")
            </StackPanel>
        </Border>
        <StackPanel Margin=""20,12"" Width=""400"">
            @RenderSection(""PageContent"")
        </StackPanel>
    </StackPanel>
</Page>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Markup;

public partial class SectionDemoPage : Page
{
    public SectionDemoPage()
    {
        InitializeComponent();

        // Sections defined in JALXAML are registered automatically.
        // Clean up on unload to avoid stale section references.
        Unloaded += (_, _) =>
            RazorExpressionRegistry.UnregisterSection(""PaneFooterSection"");
    }

    // Sections can also be used for shell-level slots:
    // @section PaneFooterSection { ... }
    // The shell layout calls @RenderSection(""PaneFooterSection"")
    // to inject page-specific footer content.
    //
    // Key points:
    // - Sections are defined with @section Name { ... }
    // - Rendered with @RenderSection(""Name"")
    // - Can be reused multiple times in the same page
    // - Automatically expanded at preprocess time
}";

    public SectionDemoPage()
    {
        InitializeComponent();
        LoadCodeExamples();
        Unloaded += (_, _) => RazorExpressionRegistry.UnregisterSection("PaneFooterSection");
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
