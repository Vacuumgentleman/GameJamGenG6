namespace GameJam
{
    /// <summary>
    /// Contrato para objetos del mundo que se iluminan al mirarlos cuando se puede interactuar con ellos.
    /// Debe implementarlo un componente (MonoBehaviour).
    /// </summary>
    public interface IHighlightable
    {
        bool CanHighlight { get; }

        // Texto que se muestra al mirarlo (puede tener varias lineas).
        string HighlightText { get; }
    }
}
