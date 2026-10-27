namespace GameJam
{
    /// <summary>
    /// Contrato para superficies que se pueden cavar con una azada.
    /// </summary>
    public interface IDiggable
    {
        // Devuelve true si el terreno se cavo.
        bool TryDig();
    }
}
