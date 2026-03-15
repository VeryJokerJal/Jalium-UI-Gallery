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
        var bitmapImage = new BitmapImage(new Uri(ImageUrl));

        // Main demo image
        if (ImageContainer != null)
        {
            var image = new Image
            {
                Source = bitmapImage,
                Stretch = Controls.Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            ImageContainer.Child = image;
        }

        // Update status text when loaded
        bitmapImage.OnImageLoaded += (s, e) =>
        {
            if (ImageStatusText != null)
                ImageStatusText.Text = $"Image loaded: {bitmapImage.Width:F0} x {bitmapImage.Height:F0} pixels";

            // Once image is loaded, populate stretch mode and styled demos
            PopulateStretchModes(bitmapImage);
            PopulateStyledImages(bitmapImage);
        };
    }

    private void PopulateStretchModes(BitmapImage source)
    {
        // Stretch.None
        if (StretchNoneContainer != null)
        {
            StretchNoneContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        // Stretch.Fill
        if (StretchFillContainer != null)
        {
            StretchFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Stretch.Uniform
        if (StretchUniformContainer != null)
        {
            StretchUniformContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.Uniform,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Stretch.UniformToFill
        if (StretchUniformToFillContainer != null)
        {
            StretchUniformToFillContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
    }

    private void PopulateStyledImages(BitmapImage source)
    {
        // Rounded corners (CornerRadius=16)
        if (RoundedImageContainer != null)
        {
            RoundedImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.UniformToFill,
                CornerRadius = new CornerRadius(16),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Circle (CornerRadius=75 on 150x150 container)
        if (CircleImageContainer != null)
        {
            CircleImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.UniformToFill,
                CornerRadius = new CornerRadius(75),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }

        // Bordered
        if (BorderedImageContainer != null)
        {
            BorderedImageContainer.Child = new Image
            {
                Source = source,
                Stretch = Controls.Stretch.UniformToFill,
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)),
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(8),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
        }
    }
}
