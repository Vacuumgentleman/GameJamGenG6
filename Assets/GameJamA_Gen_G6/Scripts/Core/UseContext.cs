using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Datos que el jugador entrega a un objeto cuando lo usa.
    /// </summary>
    public readonly struct UseContext
    {
        // Rayo que sale del centro de la camara hacia donde mira el jugador.
        public Ray Aim { get; }

        // GameObject del jugador que usa el objeto.
        public GameObject User { get; }

        public UseContext(Ray aim, GameObject user)
        {
            Aim = aim;
            User = user;
        }
    }
}
