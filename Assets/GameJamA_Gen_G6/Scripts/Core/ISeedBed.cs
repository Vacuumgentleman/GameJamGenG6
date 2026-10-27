namespace GameJam
{
    /// <summary>
    /// Contrato para terrenos donde se puede sembrar.
    /// </summary>
    public interface ISeedBed
    {
        // Devuelve true si la semilla quedo plantada. instantGrowth hace que crezca sin riego.
        bool TryPlant(CropType crop, bool instantGrowth);
    }
}
