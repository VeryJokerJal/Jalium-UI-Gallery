using Jalium.UI.Controls;

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
    }
}
