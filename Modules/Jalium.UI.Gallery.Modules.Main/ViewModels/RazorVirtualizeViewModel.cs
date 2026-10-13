using System.Collections.ObjectModel;
using System.Windows.Input;
using Jalium.UI;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>One row of the demonstration data.</summary>
public sealed class VirtualizedRow
{
    public VirtualizedRow(int index)
    {
        Index = index;
        Name = $"Row {index:N0}";
        Category = s_categories[index % s_categories.Length];
        Value = ((index * 37) % 1000) / 10d;
    }

    private static readonly string[] s_categories =
        ["Ledger", "Invoice", "Shipment", "Refund", "Adjustment"];

    public int Index { get; }

    public string Name { get; }

    public string Category { get; }

    public double Value { get; }

    /// <summary>Marks roughly every seventh row, so the item template has something to branch on.</summary>
    public bool IsFlagged => Index % 7 == 0;
}

/// <summary>
/// Bindable data for the <c>@virtualize</c> demonstrations.
/// </summary>
/// <remarks>
/// The collection is deliberately large. The point of the page is that the element count follows
/// the viewport rather than the data, so a hundred thousand rows has to be genuinely present for
/// the demonstration to mean anything.
/// </remarks>
public sealed class RazorVirtualizeViewModel : GalleryPageViewModelBase
{
    private const int InitialRowCount = 100_000;

    private const string VirtualizeXaml = """
        <ScrollViewer Height="320">
            @virtualize(var row in Rows)
            {
                <Border Height="44" Padding="12,0">
                    <TextBlock Text="@row.Name" />
                </Border>
            }
        </ScrollViewer>
        """;

    private const string NumericXaml = """
        <ScrollViewer Height="220">
            @virtualize(var i = 0; i < TileCount; i++)
            {
                <Border Height="36" Padding="12,0">
                    <TextBlock Text="Tile @i." />
                </Border>
            }
        </ScrollViewer>
        """;

    private const string ForeachXaml = """
        @foreach(var row in Rows)
        {
            <TextBlock Text="@row.Name" />
        }
        """;

    private int _tileCount = 500;

    public RazorVirtualizeViewModel()
    {
        // Registering typed accessors keeps every path off reflection, which is what makes the
        // page behave the same under PublishTrimmed / PublishAot as it does in a debug run. The
        // item type needs them just as much as the view model: inside the loop body the bindings
        // resolve against a row, not against this.
        Markup.RazorExpressionRegistry.RegisterPropertyAccessors(typeof(RazorVirtualizeViewModel), [
            ("Rows", static o => ((RazorVirtualizeViewModel)o).Rows),
            ("TileCount", static o => ((RazorVirtualizeViewModel)o).TileCount),
            ("RowCountLabel", static o => ((RazorVirtualizeViewModel)o).RowCountLabel),
        ]);

        Markup.RazorExpressionRegistry.RegisterPropertyAccessors(typeof(VirtualizedRow), [
            ("Index", static o => ((VirtualizedRow)o).Index),
            ("Name", static o => ((VirtualizedRow)o).Name),
            ("Category", static o => ((VirtualizedRow)o).Category),
            ("Value", static o => ((VirtualizedRow)o).Value),
            ("IsFlagged", static o => ((VirtualizedRow)o).IsFlagged),
        ]);

        Rows = new ObservableCollection<VirtualizedRow>(
            Enumerable.Range(0, InitialRowCount).Select(static i => new VirtualizedRow(i)));

        PrependRowCommand = new RelayCommand(() =>
        {
            Rows.Insert(0, new VirtualizedRow(-Rows.Count));
            OnPropertyChanged(nameof(RowCountLabel));
        });

        RemoveFirstRowCommand = new RelayCommand(() =>
        {
            if (Rows.Count > 0)
            {
                Rows.RemoveAt(0);
                OnPropertyChanged(nameof(RowCountLabel));
            }
        });

        GrowTilesCommand = new RelayCommand(() => TileCount += 250);

        ShrinkTilesCommand = new RelayCommand(() => TileCount = Math.Max(0, TileCount - 250));
    }

    /// <summary>The rows the collection form iterates. Observable, so edits reach the list.</summary>
    public ObservableCollection<VirtualizedRow> Rows { get; }

    /// <summary>Upper bound of the numeric form, so its length can be changed at run time.</summary>
    public int TileCount
    {
        get => _tileCount;
        set => SetProperty(ref _tileCount, value);
    }

    public string RowCountLabel => $"{Rows.Count:N0} rows";

    public ICommand PrependRowCommand { get; }

    public ICommand RemoveFirstRowCommand { get; }

    public ICommand GrowTilesCommand { get; }

    public ICommand ShrinkTilesCommand { get; }

    public string VirtualizeExample => VirtualizeXaml;

    public string NumericExample => NumericXaml;

    public string ForeachExample => ForeachXaml;
}
