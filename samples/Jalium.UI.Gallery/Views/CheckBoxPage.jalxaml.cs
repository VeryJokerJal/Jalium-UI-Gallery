using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for CheckBoxPage.jalxaml demonstrating CheckBox functionality.
/// </summary>
public partial class CheckBoxPage : Page
{
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
