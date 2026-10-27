namespace GameJam
{
    /// <summary>
    /// Resultado de usar un objeto.
    /// </summary>
    public enum UseResult
    {
        // El objeto sigue en el inventario (ejemplo: una linterna).
        Kept,

        // El objeto sale del inventario y se encarga de si mismo (ejemplo: una lanza lanzada).
        Consumed
    }
}
