using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Contrato para cualquier objeto del mundo que el jugador pueda recoger con la tecla E.
    /// El jugador solo conoce esta interfaz, nunca la lanza, la antorcha, etc.
    /// </summary>
    public interface IPickable
    {
        // Nombre para mostrar en la interfaz (por ejemplo "Lanza").
        string DisplayName { get; }

        // Acceso al GameObject para poder moverlo, activarlo o desactivarlo.
        GameObject GameObject { get; }

        // Se llama justo cuando entra al inventario. Aqui se apaga la fisica.
        void OnPickedUp();

        // Se llama cuando el jugador lo suelta. Aqui se vuelve a activar la fisica.
        void OnDropped(Vector3 position, Quaternion rotation, Vector3 velocity);
    }
}
