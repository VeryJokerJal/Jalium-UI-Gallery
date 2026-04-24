namespace Jalium.UI.Gallery.Core.Mvvm;

/// <summary>
/// Navigation-aware view model base. Override the hook methods to drive navigation
/// side-effects. Wire this up to Prism regions, Jalium navigation, or your own
/// navigation service — the contract is intentionally minimal so you aren't locked in.
/// </summary>
public abstract class RegionViewModelBase : ViewModelBase
{
    /// <summary>Invoked after the view has been activated inside a region.</summary>
    /// <param name="parameter">Navigation payload the caller supplied, or <c>null</c>.</param>
    public virtual void OnNavigatedTo(object? parameter) { }

    /// <summary>Invoked before the view is deactivated and replaced in the region.</summary>
    public virtual void OnNavigatedFrom() { }

    /// <summary>
    /// Decides whether an existing instance can be re-used for the incoming navigation
    /// request. Returning <c>false</c> forces a fresh instance.
    /// </summary>
    public virtual bool IsNavigationTarget(object? parameter) => true;
}
