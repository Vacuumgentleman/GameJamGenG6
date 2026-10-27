namespace GameJam
{
    /// <summary>
    /// Contrato para fuentes donde se rellena un recipiente de agua (pozo, cisterna, rio).
    /// </summary>
    public interface IWaterSource
    {
        // Devuelve true si se puede sacar agua en este momento.
        bool TryDraw();
    }
}
