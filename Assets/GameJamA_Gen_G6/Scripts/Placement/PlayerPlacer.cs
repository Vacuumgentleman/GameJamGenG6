using System;
using UnityEngine;

namespace GameJam
{
    public class PlayerPlacer : MonoBehaviour
    {
        [SerializeField] private float reach = 3.5f;
        [SerializeField] private LayerMask mask = ~0;

        private PlayerInputReader input;
        private PlayerAim aim;
        private PlayerInventory inventory;

        public event Action<PlacementSlot, PlaceableItem> Rejected;

        public PlaceableItem Held => inventory != null ? inventory.Selected as PlaceableItem : null;

        private void Start()
        {
            input = FindFirstObjectByType<PlayerInputReader>();
            aim = FindFirstObjectByType<PlayerAim>();
            inventory = FindFirstObjectByType<PlayerInventory>();

            if (input != null)
            {
                input.InteractPressed += HandleInteract;
            }
        }

        private void OnDestroy()
        {
            if (input != null)
            {
                input.InteractPressed -= HandleInteract;
            }
        }

        private void HandleInteract()
        {
            PlaceableItem item = Held;
            if (item == null || !aim.Raycast(reach, mask, out RaycastHit hit))
            {
                return;
            }

            PlacementSlot slot = hit.collider.GetComponentInParent<PlacementSlot>();
            if (slot == null || slot.IsFilled)
            {
                return;
            }

            if (!slot.Accepts(item))
            {
                Rejected?.Invoke(slot, item);
                return;
            }

            inventory.Remove(inventory.SelectedIndex);
            slot.TryPlace(item);
        }
    }
}
