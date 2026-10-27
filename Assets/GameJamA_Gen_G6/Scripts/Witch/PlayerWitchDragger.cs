using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Parte del jugador que agarra y suelta a la bruja con E.
    /// No usa el inventario: la bruja no se guarda ni se oculta, se arrastra por el mundo.
    /// </summary>
    public class PlayerWitchDragger : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerAim aim;

        // Distancia maxima para agarrar a la bruja, en metros.
        [SerializeField] private float reach = 2.5f;

        // Capas que se pueden detectar.
        [SerializeField] private LayerMask mask = ~0;

        // no choque con el jugador mientras la arrastra. 
        [SerializeField] private Collider ownerCollider;

        // Bruja que se esta arrastrando ahora mismo, o null.
        public Witch Current { get; private set; }

        public Collider OwnerCollider => ownerCollider;

        // Direccion horizontal hacia donde mira el jugador (sin inclinacion vertical).
        public Vector3 FlatForward
        {
            get
            {
                Vector3 forward = Vector3.ProjectOnPlane(aim.GetRay().direction, Vector3.up);
                return forward.sqrMagnitude > 0.0001f ? forward.normalized : transform.forward;
            }
        }

        private void Awake()
        {
            if (ownerCollider == null)
            {
                ownerCollider = GetComponentInParent<Collider>();
            }
        }

        private void OnEnable()
        {
            input.InteractPerformed += HandleInteract;
        }

        private void OnDisable()
        {
            input.InteractPerformed -= HandleInteract;
            Release();
        }

        private void HandleInteract()
        {
            // Si ya arrastra a una bruja, E la suelta.
            if (Current != null)
            {
                Release();
                return;
            }

            if (!aim.Raycast(reach, mask, out RaycastHit hit))
            {
                return;
            }

            Witch witch = hit.collider.GetComponentInParent<Witch>();
            if (witch != null && witch.CanBeGrabbed)
            {
                Current = witch;
                witch.Grab(this);
            }
        }

        // Suelta a la bruja que se arrastra (si hay una).
        public void Release()
        {
            if (Current == null)
            {
                return;
            }

            Witch witch = Current;
            Current = null;
            witch.ReleaseGrab();

            Rigidbody rb = witch.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }
}