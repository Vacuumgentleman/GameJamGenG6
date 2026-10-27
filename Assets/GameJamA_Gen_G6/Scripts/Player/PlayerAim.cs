using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Punto de apuntado del jugador: el centro de la pantalla.
    /// Cualquier script que necesite saber hacia donde mira el jugador pregunta aqui.
    /// </summary>
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;

        public Camera PlayerCamera => playerCamera;

        // Rayo que sale de la camara hacia adelante.
        public Ray GetRay()
        {
            Transform view = playerCamera.transform;
            return new Ray(view.position, view.forward);
        }

        // Dispara un rayo y devuelve true si golpea algo dentro de la distancia dada.
        public bool Raycast(float distance, LayerMask mask, out RaycastHit hit)
        {
            return Physics.Raycast(GetRay(), out hit, distance, mask, QueryTriggerInteraction.Ignore);
        }
    }
}
