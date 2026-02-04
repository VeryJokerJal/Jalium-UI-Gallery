using Jalium.UI.Controls;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ImagePage.jalxaml demonstrating Image control functionality.
/// </summary>
public partial class ImagePage : Page
{
    private const string ImageUrl = "http://img.netbian.com/file/2024/0816/221229zX0E3.jpg";

    public ImagePage()
    {
        InitializeComponent();
        LoadDemoImage();
    }

    private void LoadDemoImage()
    {
        if (ImageContainer == null) return;

        try
        {
            var bitmapImage = new BitmapImage(new Uri(ImageUrl));

            var image = new Image
            {
                Source = bitmapImage,
                Stretch = Controls.Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            ImageContainer.Child = image;

            // Update status when image is loaded
            bitmapImage.OnImageLoaded += (s, e) =>
            {
                if (ImageStatusText != null)
                {
                    ImageStatusText.Text = $"Image loaded: {bitmapImage.Width:F0} x {bitmapImage.Height:F0} pixels";
                }
            };
        }
        catch (Exception ex)
        {
            if (ImageStatusText != null)
            {
                ImageStatusText.Text = $"Failed to load image: {ex.Message}";
            }
        }
    }
}
