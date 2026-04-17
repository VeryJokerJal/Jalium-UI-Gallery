using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for PopupPage.jalxaml demonstrating popup functionality.
/// </summary>
public partial class PopupPage : Page
{
    public PopupPage()
    {
        InitializeComponent();

        // Basic popup toggle
        if (ShowPopupButton != null && BasicPopup != null)
        {
            ShowPopupButton.Click += (s, e) => BasicPopup.IsOpen = !BasicPopup.IsOpen;
        }

        if (ClosePopupButton != null && BasicPopup != null)
        {
            ClosePopupButton.Click += (s, e) => BasicPopup.IsOpen = false;
        }

        // Form popup toggle
        if (ShowFormPopupButton != null && FormPopup != null)
        {
            ShowFormPopupButton.Click += (s, e) => FormPopup.IsOpen = !FormPopup.IsOpen;
        }

        if (CancelFormButton != null && FormPopup != null)
        {
            CancelFormButton.Click += (s, e) => FormPopup.IsOpen = false;
        }

        if (SaveFormButton != null && FormPopup != null)
        {
            SaveFormButton.Click += (s, e) => FormPopup.IsOpen = false;
        }

        LoadCodeExamples();
    }

    private const string XamlExample = @"<!-- Basic Popup -->
<Button x:Name=""ShowPopupButton"" Content=""Show Popup""/>
<Popup x:Name=""BasicPopup""
       PlacementTarget=""{Binding ElementName=ShowPopupButton}""
       Placement=""Bottom"">
    <Border Background=""#3D3D3D""
            BorderBrush=""#5D5D5D""
            BorderThickness=""1""
            CornerRadius=""4""
            Padding=""16"">
        <StackPanel Orientation=""Vertical"">
            <TextBlock Text=""This is a popup!""
                       Foreground=""#FFFFFF""/>
            <Button x:Name=""ClosePopupButton""
                    Content=""Close""/>
        </StackPanel>
    </Border>
</Popup>

<!-- Popup with Form -->
<Popup x:Name=""FormPopup""
       Placement=""Bottom""
       StaysOpen=""True"">
    <Border Padding=""20"" Width=""250"">
        <StackPanel>
            <TextBox PlaceholderText=""Enter name""/>
            <Button Content=""Save""/>
        </StackPanel>
    </Border>
</Popup>";

    private const string CSharpExample = @"// Toggle popup on button click
ShowPopupButton.Click += (s, e) =>
    BasicPopup.IsOpen = !BasicPopup.IsOpen;

// Close popup from inside
ClosePopupButton.Click += (s, e) =>
    BasicPopup.IsOpen = false;

// Form popup with StaysOpen
ShowFormPopupButton.Click += (s, e) =>
    FormPopup.IsOpen = !FormPopup.IsOpen;

CancelFormButton.Click += (s, e) =>
    FormPopup.IsOpen = false;

SaveFormButton.Click += (s, e) =>
    FormPopup.IsOpen = false;";

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
