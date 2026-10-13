namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// Common presentation surface for gallery pages. Concrete pages always use a
/// dedicated ViewModel type; this base is never registered as a DataContext.
/// </summary>
public abstract class GalleryPageViewModelBase
    : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private string _status = string.Empty;

    public string Status
    {
        get => _status;
        protected set => SetProperty(ref _status, value);
    }

    public virtual string XamlExample => string.Empty;

    public virtual string CSharpExample => string.Empty;
}
