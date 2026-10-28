using System;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Detecta que objeto recogible esta mirando el jugador (centro de la pantalla)
    /// y lo recoge al pulsar E. Expone el evento FocusChanged para que la interfaz (Kim)
    /// pueda mostrar mensajes como "E para recoger".
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private PlayerCarrier carrier;

        // Distancia maxima para recoger, en metros.
        [SerializeField] private float reach = 2.5f;

        // Capas que se pueden detectar. Por defecto, todas.
        [SerializeField] private LayerMask mask = ~0;

        // Segundos que hay que mantener E para recoger.
        [SerializeField] private float holdTime = 0.4f;

        private IPickable focus;
        private float heldTime;
        private bool requireRelease;

        // Objeto recogible que se esta mirando, o null.
        public IPickable Focus => focus;

        // Avance del agarre de 0 a 1 mientras se mantiene E sobre un recogible.
        public float GrabProgress => holdTime > 0f ? Mathf.Clamp01(heldTime / holdTime) : 0f;

        // Se dispara cada vez que cambia lo que se mira. El valor es null si no se mira nada recogible.
        public event Action<IPickable> FocusChanged;

        private void Update()
        {
            IPickable found = null;
            if (aim.Raycast(reach, mask, out RaycastHit hit))
            {
                // Busca la interfaz en el objeto golpeado o en sus padres.
                found = hit.collider.GetComponentInParent<IPickable>();
            }

            SetFocus(found);
            UpdateHold();
        }

        private void UpdateHold()
        {
            if (!input.InteractHeld)
            {
                requireRelease = false;
                heldTime = 0f;
                return;
            }

            if (focus == null || requireRelease)
            {
                heldTime = 0f;
                return;
            }

            heldTime += Time.deltaTime;
            if (heldTime < holdTime)
            {
                return;
            }

            heldTime = 0f;
            requireRelease = true;
            if (carrier.Pickup(focus))
            {
                SetFocus(null);
            }
        }

        private void SetFocus(IPickable value)
        {
            if (ReferenceEquals(focus, value))
            {
                return;
            }

            focus = value;
            heldTime = 0f;
            FocusChanged?.Invoke(focus);
        }
    }
}
