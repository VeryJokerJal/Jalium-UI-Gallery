using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for RadioButtonPage.jalxaml demonstrating RadioButton functionality.
/// </summary>
public partial class RadioButtonPage : Page
{
    private const string XamlExample = @"<!-- Basic RadioButton group -->
<StackPanel Orientation=""Vertical"">
    <RadioButton x:Name=""RadioSmall"" GroupName=""Size"">
        <TextBlock Text=""Small""/>
    </RadioButton>
    <RadioButton x:Name=""RadioMedium"" GroupName=""Size""
                 IsChecked=""True"">
        <TextBlock Text=""Medium""/>
    </RadioButton>
    <RadioButton x:Name=""RadioLarge"" GroupName=""Size"">
        <TextBlock Text=""Large""/>
    </RadioButton>
</StackPanel>

<!-- Multiple independent groups -->
<StackPanel Orientation=""Horizontal"" Spacing=""32"">
    <!-- Theme group -->
    <StackPanel Orientation=""Vertical"">
        <TextBlock Text=""Theme"" FontWeight=""SemiBold""/>
        <RadioButton GroupName=""Theme"" IsChecked=""True"">
            <TextBlock Text=""Light""/>
        </RadioButton>
        <RadioButton GroupName=""Theme"">
            <TextBlock Text=""Dark""/>
        </RadioButton>
        <RadioButton GroupName=""Theme"">
            <TextBlock Text=""System Default""/>
        </RadioButton>
    </StackPanel>

    <!-- Language group -->
    <StackPanel Orientation=""Vertical"">
        <TextBlock Text=""Language"" FontWeight=""SemiBold""/>
        <RadioButton GroupName=""Language"" IsChecked=""True"">
            <TextBlock Text=""English""/>
        </RadioButton>
        <RadioButton GroupName=""Language"">
            <TextBlock Text=""Japanese""/>
        </RadioButton>
        <RadioButton GroupName=""Language"">
            <TextBlock Text=""Chinese""/>
        </RadioButton>
    </StackPanel>
</StackPanel>

<!-- Disabled RadioButton -->
<RadioButton IsEnabled=""False"">
    <TextBlock Text=""Unavailable option""/>
</RadioButton>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class PreferencesPage : Page
{
    public PreferencesPage()
    {
        InitializeComponent();

        // Wire up Checked events for each radio button
        RadioSmall.Checked += OnSizeChanged;
        RadioMedium.Checked += OnSizeChanged;
        RadioLarge.Checked += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radio)
        {
            string selection = radio switch
            {
                _ when radio == RadioSmall  => ""Small"",
                _ when radio == RadioMedium => ""Medium"",
                _ when radio == RadioLarge  => ""Large"",
                _ => ""Unknown""
            };
            StatusText.Text = $""Selected size: {selection}"";
        }
    }

    // Create RadioButtons programmatically
    private void BuildColorGroup()
    {
        string[] colors = { ""Red"", ""Green"", ""Blue"", ""Yellow"" };

        foreach (var color in colors)
        {
            var radio = new RadioButton
            {
                GroupName = ""DynamicColors""
            };
            radio.Content = new TextBlock { Text = color };
            radio.Checked += (s, e) =>
            {
                ResultText.Text = $""Color: {color}"";
            };
            ColorPanel.Children.Add(radio);
        }

        // Select the first one by default
        if (ColorPanel.Children[0] is RadioButton first)
        {
            first.IsChecked = true;
        }
    }
}";

    public RadioButtonPage()
    {
        InitializeComponent();

        // Set up event handlers after component initialization
        if (RadioRed != null)
        {
            RadioRed.Checked += OnRadioChecked;
        }
        if (RadioGreen != null)
        {
            RadioGreen.Checked += OnRadioChecked;
        }
        if (RadioBlue != null)
        {
            RadioBlue.Checked += OnRadioChecked;
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

    private void OnRadioChecked(object sender, RoutedEventArgs e)
    {
        if (RadioStatus == null) return;

        if (sender == RadioRed)
        {
            RadioStatus.Text = "Selected: Red";
        }
        else if (sender == RadioGreen)
        {
            RadioStatus.Text = "Selected: Green";
        }
        else if (sender == RadioBlue)
        {
            RadioStatus.Text = "Selected: Blue";
        }
    }
}
