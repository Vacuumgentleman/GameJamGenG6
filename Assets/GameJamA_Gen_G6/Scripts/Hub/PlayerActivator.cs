using UnityEngine;

namespace GameJam
{
    public class PlayerActivator : MonoBehaviour
    {
        [SerializeField] private float reach = 3.5f;
        [SerializeField] private LayerMask mask = ~0;

        private PlayerInputReader input;
        private PlayerAim aim;

        private void Start()
        {
            input = FindFirstObjectByType<PlayerInputReader>();
            aim = FindFirstObjectByType<PlayerAim>();

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
            if (aim == null || !aim.Raycast(reach, mask, out RaycastHit hit))
            {
                return;
            }

            hit.collider.GetComponentInParent<IInteractable>()?.Interact();
        }
    }
}
