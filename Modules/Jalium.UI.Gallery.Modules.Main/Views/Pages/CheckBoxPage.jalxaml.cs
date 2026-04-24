using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for CheckBoxPage.jalxaml demonstrating CheckBox functionality.
/// </summary>
public partial class CheckBoxPage : Page
{
    private const string XamlExample = @"<!-- Basic CheckBox -->
<CheckBox>
    <TextBlock Text=""Accept terms and conditions""/>
</CheckBox>

<!-- Pre-checked CheckBox -->
<CheckBox IsChecked=""True"">
    <TextBlock Text=""Enable notifications""/>
</CheckBox>

<!-- Disabled CheckBox -->
<CheckBox IsEnabled=""False"">
    <TextBlock Text=""Unavailable option""/>
</CheckBox>

<!-- Disabled and checked -->
<CheckBox IsChecked=""True"" IsEnabled=""False"">
    <TextBlock Text=""Required (always on)""/>
</CheckBox>

<!-- Three-state CheckBox -->
<CheckBox x:Name=""SelectAllCheckBox""
          IsThreeState=""True""
          Checked=""OnSelectAllChecked""
          Unchecked=""OnSelectAllUnchecked""
          Indeterminate=""OnSelectAllIndeterminate"">
    <TextBlock Text=""Select All""/>
</CheckBox>

<!-- Group of related checkboxes -->
<StackPanel Orientation=""Vertical"" Margin=""24,0,0,0"">
    <CheckBox x:Name=""Option1"" Checked=""OnOptionChanged""
              Unchecked=""OnOptionChanged"">
        <TextBlock Text=""Option A""/>
    </CheckBox>
    <CheckBox x:Name=""Option2"" Checked=""OnOptionChanged""
              Unchecked=""OnOptionChanged"">
        <TextBlock Text=""Option B""/>
    </CheckBox>
    <CheckBox x:Name=""Option3"" Checked=""OnOptionChanged""
              Unchecked=""OnOptionChanged"">
        <TextBlock Text=""Option C""/>
    </CheckBox>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();

        SelectAllCheckBox.Checked += OnSelectAllChecked;
        SelectAllCheckBox.Unchecked += OnSelectAllUnchecked;
        SelectAllCheckBox.Indeterminate += OnSelectAllIndeterminate;

        Option1.Checked += OnOptionChanged;
        Option1.Unchecked += OnOptionChanged;
        Option2.Checked += OnOptionChanged;
        Option2.Unchecked += OnOptionChanged;
        Option3.Checked += OnOptionChanged;
        Option3.Unchecked += OnOptionChanged;
    }

    private void OnSelectAllChecked(object sender, RoutedEventArgs e)
    {
        Option1.IsChecked = true;
        Option2.IsChecked = true;
        Option3.IsChecked = true;
    }

    private void OnSelectAllUnchecked(object sender, RoutedEventArgs e)
    {
        Option1.IsChecked = false;
        Option2.IsChecked = false;
        Option3.IsChecked = false;
    }

    private void OnSelectAllIndeterminate(object sender, RoutedEventArgs e)
    {
        // Indeterminate state - some children checked
    }

    private void OnOptionChanged(object sender, RoutedEventArgs e)
    {
        int checkedCount = 0;
        if (Option1.IsChecked == true) checkedCount++;
        if (Option2.IsChecked == true) checkedCount++;
        if (Option3.IsChecked == true) checkedCount++;

        SelectAllCheckBox.IsChecked = checkedCount switch
        {
            3 => true,      // All checked
            0 => false,     // None checked
            _ => null       // Some checked (indeterminate)
        };
    }
}";

    public CheckBoxPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (DemoCheckBox != null)
        {
            DemoCheckBox.Checked += OnDemoCheckBoxChecked;
            DemoCheckBox.Unchecked += OnDemoCheckBoxUnchecked;
        }

        if (ThreeStateCheckBox != null)
        {
            ThreeStateCheckBox.IsChecked = null;
            ThreeStateCheckBox.Checked += OnThreeStateCheckBoxChecked;
            ThreeStateCheckBox.Unchecked += OnThreeStateCheckBoxUnchecked;
            ThreeStateCheckBox.Indeterminate += OnThreeStateCheckBoxIndeterminate;
        }

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

    private void OnDemoCheckBoxChecked(object sender, RoutedEventArgs e)
    {
        if (CheckBoxStatus != null)
        {
            CheckBoxStatus.Text = "Status: Checked";
        }
    }

    private void OnDemoCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
        if (CheckBoxStatus != null)
        {
            CheckBoxStatus.Text = "Status: Unchecked";
        }
    }

    private void OnThreeStateCheckBoxChecked(object sender, RoutedEventArgs e)
    {
        if (ThreeStateStatus != null)
        {
            ThreeStateStatus.Text = "Status: Checked";
        }
    }

    private void OnThreeStateCheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
        if (ThreeStateStatus != null)
        {
            ThreeStateStatus.Text = "Status: Unchecked";
        }
    }

    private void OnThreeStateCheckBoxIndeterminate(object sender, RoutedEventArgs e)
    {
        if (ThreeStateStatus != null)
        {
            ThreeStateStatus.Text = "Status: Indeterminate";
        }
    }
}
