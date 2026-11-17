using System.Runtime.InteropServices;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class NavigationStateViewModel
{
    private string _stateTitle = string.Empty;
    private string _stateDetail = string.Empty;
    private bool _isError;

    public string StateTitle
    {
        get => _stateTitle;
        private set => SetProperty(ref _stateTitle, value);
    }

    public string StateDetail
    {
        get => _stateDetail;
        private set => SetProperty(ref _stateDetail, value);
    }

    public bool IsError
    {
        get => _isError;
        private set => SetProperty(ref _isError, value);
    }

    public void ShowMissing(string pageTag)
    {
        IsError = false;
        StateTitle = pageTag;
        StateDetail = "This page is coming soon.";
    }

    public void ShowError(string pageTag, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        IsError = true;
        StateTitle = $"{pageTag} - Load failed";
        StateDetail = $"{exception.GetType().Name}: {exception.Message}";
    }
}

public sealed partial class PlatformUnavailableViewModel
{
    private string _feature = string.Empty;
    private string _reason = string.Empty;
    private string _guidance = string.Empty;

    public string Feature
    {
        get => _feature;
        private set => SetProperty(ref _feature, value);
    }

    public string Reason
    {
        get => _reason;
        private set => SetProperty(ref _reason, value);
    }

    public string Guidance
    {
        get => _guidance;
        private set => SetProperty(ref _guidance, value);
    }

    public string RuntimeDescription =>
        $"Runtime: {RuntimeInformation.OSDescription} ({RuntimeInformation.RuntimeIdentifier})";

    public void Show(string feature, string reason, string guidance)
    {
        Feature = feature;
        Reason = reason;
        Guidance = guidance;
    }
}
