namespace Jalium.UI.Gallery.Modules.Main.Models;

/// <summary>
/// One row of a CSS reference table on the styling pages: the CSS spelling, what it reaches
/// in the property system, and the conversion worth knowing about.
/// </summary>
public sealed class CssReferenceRow
{
    public CssReferenceRow(string css, string target, string note)
    {
        Css = css;
        Target = target;
        Note = note;
    }

    public string Css { get; }

    public string Target { get; }

    public string Note { get; }
}
