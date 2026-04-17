using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Gallery.Theme;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class ComboBoxPage : Page
{
    private const string XamlExample = @"<!-- Basic ComboBox with string items -->
<ComboBox x:Name=""FruitComboBox""
          PlaceholderText=""Select a fruit""
          Width=""200""/>

<!-- ComboBox with pre-selected item -->
<ComboBox x:Name=""ColorComboBox""
          PlaceholderText=""Select a color""
          Width=""200""
          SelectedIndex=""2""/>

<!-- ComboBox with selection event -->
<StackPanel Orientation=""Horizontal"" Spacing=""16"">
    <ComboBox x:Name=""DayComboBox""
              PlaceholderText=""Select a day""
              Width=""200""
              SelectionChanged=""OnDaySelectionChanged""/>
    <TextBlock x:Name=""SelectedDayText""
               Text=""Selected: (none)""
               VerticalAlignment=""Center""/>
</StackPanel>

<!-- Disabled ComboBox -->
<ComboBox PlaceholderText=""Disabled""
          Width=""200""
          IsEnabled=""False""/>

<!-- ComboBox in a form layout -->
<StackPanel Orientation=""Vertical"" Spacing=""12"">
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""Country"" FontSize=""12""/>
        <ComboBox x:Name=""CountryComboBox""
                  PlaceholderText=""Select country""
                  Width=""250""
                  SelectionChanged=""OnCountryChanged""/>
    </StackPanel>
    <StackPanel Orientation=""Vertical"" Spacing=""4"">
        <TextBlock Text=""City"" FontSize=""12""/>
        <ComboBox x:Name=""CityComboBox""
                  PlaceholderText=""Select city""
                  Width=""250""
                  IsEnabled=""False""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

public partial class OrderPage : Page
{
    public OrderPage()
    {
        InitializeComponent();

        // Populate ComboBox items programmatically
        string[] fruits = {
            ""Apple"", ""Banana"", ""Cherry"",
            ""Orange"", ""Grape"", ""Mango""
        };
        foreach (var fruit in fruits)
            FruitComboBox.Items.Add(fruit);

        // Pre-select an item
        ColorComboBox.Items.Add(""Red"");
        ColorComboBox.Items.Add(""Green"");
        ColorComboBox.Items.Add(""Blue"");
        ColorComboBox.SelectedIndex = 2; // Blue

        // Handle selection changes
        DayComboBox.SelectionChanged += OnDayChanged;

        // Cascading ComboBoxes
        CountryComboBox.SelectionChanged += OnCountryChanged;
        PopulateCountries();
    }

    private void OnDayChanged(object sender,
        SelectionChangedEventArgs e)
    {
        var selected = DayComboBox.SelectedItem
            ?.ToString() ?? ""(none)"";
        SelectedDayText.Text = $""Selected: {selected}"";
    }

    private void PopulateCountries()
    {
        CountryComboBox.Items.Add(""United States"");
        CountryComboBox.Items.Add(""Japan"");
        CountryComboBox.Items.Add(""Germany"");
    }

    private void OnCountryChanged(object sender,
        SelectionChangedEventArgs e)
    {
        CityComboBox.Items.Clear();
        CityComboBox.IsEnabled = true;

        var country = CountryComboBox.SelectedItem
            ?.ToString();

        var cities = country switch
        {
            ""United States"" => new[] {
                ""New York"", ""Los Angeles"", ""Chicago"" },
            ""Japan"" => new[] {
                ""Tokyo"", ""Osaka"", ""Kyoto"" },
            ""Germany"" => new[] {
                ""Berlin"", ""Munich"", ""Hamburg"" },
            _ => Array.Empty<string>()
        };

        foreach (var city in cities)
            CityComboBox.Items.Add(city);

        if (cities.Length > 0)
            CityComboBox.SelectedIndex = 0;
    }
}";

    public ComboBoxPage()
    {
        InitializeComponent();
        CreateContent();
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

    private void CreateContent()
    {
        if (ContentPanel == null) return;

        // Title
        var title = new TextBlock
        {
            Text = "ComboBox",
            FontSize = GalleryTheme.FontSizeTitle,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 0, 0, 8)
        };
        ContentPanel.Children.Add(title);

        // Description
        var description = new TextBlock
        {
            Text = "A dropdown control that presents a list of options for selection.",
            FontSize = GalleryTheme.FontSizeBody,
            Foreground = GalleryTheme.TextSecondaryBrush,
            Margin = new Thickness(0, 0, 0, 24),
            TextWrapping = TextWrapping.Wrap
        };
        ContentPanel.Children.Add(description);

        // Basic ComboBox
        AddSection("Basic ComboBox", "A simple ComboBox with string items.");

        var basicComboBox = new ComboBox
        {
            PlaceholderText = "Select a fruit",
            Width = 200,
            Margin = new Thickness(0, 0, 0, 16)
        };
        basicComboBox.Items.Add("Apple");
        basicComboBox.Items.Add("Banana");
        basicComboBox.Items.Add("Cherry");
        basicComboBox.Items.Add("Orange");
        basicComboBox.Items.Add("Grape");
        ContentPanel.Children.Add(basicComboBox);

        // ComboBox with pre-selected item
        AddSection("Pre-selected Item", "A ComboBox with an item already selected.");

        var selectedComboBox = new ComboBox
        {
            PlaceholderText = "Select a color",
            Width = 200,
            Margin = new Thickness(0, 0, 0, 16)
        };
        selectedComboBox.Items.Add("Red");
        selectedComboBox.Items.Add("Green");
        selectedComboBox.Items.Add("Blue");
        selectedComboBox.Items.Add("Yellow");
        selectedComboBox.SelectedIndex = 2; // Blue
        ContentPanel.Children.Add(selectedComboBox);

        // ComboBox with selection event
        AddSection("Selection Changed Event", "Displays the selected item in a TextBlock.");

        var eventStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 16)
        };

        var eventComboBox = new ComboBox
        {
            PlaceholderText = "Select a day",
            Width = 200,
            Margin = new Thickness(0, 0, 16, 0)
        };
        eventComboBox.Items.Add("Monday");
        eventComboBox.Items.Add("Tuesday");
        eventComboBox.Items.Add("Wednesday");
        eventComboBox.Items.Add("Thursday");
        eventComboBox.Items.Add("Friday");
        eventComboBox.Items.Add("Saturday");
        eventComboBox.Items.Add("Sunday");

        var resultText = new TextBlock
        {
            Text = "Selected: (none)",
            Foreground = GalleryTheme.TextSecondaryBrush,
            VerticalAlignment = VerticalAlignment.Center
        };

        eventComboBox.SelectionChanged += (s, e) =>
        {
            var selected = eventComboBox.SelectedItem?.ToString() ?? "(none)";
            resultText.Text = $"Selected: {selected}";
        };

        eventStack.Children.Add(eventComboBox);
        eventStack.Children.Add(resultText);
        ContentPanel.Children.Add(eventStack);
    }

    private void AddSection(string titleText, string descriptionText)
    {
        if (ContentPanel == null) return;

        var sectionTitle = new TextBlock
        {
            Text = titleText,
            FontSize = GalleryTheme.FontSizeSubtitle,
            FontWeight = FontWeights.SemiBold,
            Foreground = GalleryTheme.TextPrimaryBrush,
            Margin = new Thickness(0, 16, 0, 4)
        };
        ContentPanel.Children.Add(sectionTitle);

        var sectionDesc = new TextBlock
        {
            Text = descriptionText,
            FontSize = GalleryTheme.FontSizeBody,
            Foreground = GalleryTheme.TextTertiaryBrush,
            Margin = new Thickness(0, 0, 0, 12)
        };
        ContentPanel.Children.Add(sectionDesc);
    }
}
