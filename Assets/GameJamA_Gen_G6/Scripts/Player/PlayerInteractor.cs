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

        private IPickable focus;

        // Objeto recogible que se esta mirando, o null.
        public IPickable Focus => focus;

        // Se dispara cada vez que cambia lo que se mira. El valor es null si no se mira nada recogible.
        public event Action<IPickable> FocusChanged;

        private void OnEnable()
        {
            input.InteractPerformed += HandleInteract;
        }

        private void OnDisable()
        {
            input.InteractPerformed -= HandleInteract;
        }

        private void Update()
        {
            IPickable found = null;
            if (aim.Raycast(reach, mask, out RaycastHit hit))
            {
                // Busca la interfaz en el objeto golpeado o en sus padres.
                found = hit.collider.GetComponentInParent<IPickable>();
            }

            SetFocus(found);
        }

        private void HandleInteract()
        {
            if (focus == null)
            {
                return;
            }

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
            FocusChanged?.Invoke(focus);
        }
    }
}
