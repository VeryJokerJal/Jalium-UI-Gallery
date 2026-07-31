using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

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
        if (DemoHost == null) return;

        // Basic ComboBox — string items.
        var basicComboBox = new ComboBox { PlaceholderText = "Select a fruit", Width = 220 };
        foreach (var fruit in new[] { "Apple", "Banana", "Cherry", "Orange", "Grape" })
            basicComboBox.Items.Add(fruit);
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Basic", "A simple dropdown populated with string items.", basicComboBox));

        // Pre-selected item.
        var selectedComboBox = new ComboBox { PlaceholderText = "Select a color", Width = 220 };
        foreach (var color in new[] { "Red", "Green", "Blue", "Yellow" })
            selectedComboBox.Items.Add(color);
        selectedComboBox.SelectedIndex = 2; // Blue
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Pre-selected item", "Opens with an item already chosen via SelectedIndex.", selectedComboBox));

        // Selection-changed event.
        var eventComboBox = new ComboBox
        {
            PlaceholderText = "Select a day",
            Width = 220,
            Margin = new Thickness(0, 0, 16, 0)
        };
        foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
            eventComboBox.Items.Add(day);

        var resultText = GalleryUi.ValueLabel("Selected: (none)");
        eventComboBox.SelectionChanged += (s, e) =>
        {
            var selected = eventComboBox.SelectedItem?.ToString() ?? "(none)";
            resultText.Text = $"Selected: {selected}";
        };

        var eventStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        eventStack.Children.Add(eventComboBox);
        eventStack.Children.Add(resultText);
        DemoHost.Children.Add(GalleryUi.SectionCard(
            "Selection changed", "Handle SelectionChanged to react to the chosen item.", eventStack));
    }
}
