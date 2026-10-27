namespace GameJam
{
    /// <summary>
    /// Contrato para todo lo que se puede regar con agua.
    /// </summary>
    public interface IWaterable
    {
        // Devuelve true si el agua tuvo efecto y se gasto un uso.
        bool TryWater();
    }
}
