using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jalium.UI.Gallery.Core.Mvvm;

/// <summary>
/// Lightweight <see cref="INotifyPropertyChanged"/> base that matches the Prism
/// <c>BindableBase</c> surface without adding the Prism dependency.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets the backing field and raises <see cref="PropertyChanged"/> when the new
    /// value differs from the current one.
    /// </summary>
    /// <returns><c>true</c> if the value changed, otherwise <c>false</c>.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    /// <summary>
    /// Raises <see cref="PropertyChanged"/> for <paramref name="propertyName"/> without
    /// touching a backing field — useful for computed / derived properties.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the view model is leaving the navigation stack. Override to
    /// release subscriptions, cancel pending work, or flush state.
    /// </summary>
    public virtual void Destroy() { }
}
