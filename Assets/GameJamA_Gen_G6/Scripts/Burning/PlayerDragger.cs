using System;
using UnityEngine;

namespace GameJam
{
    public class PlayerDragger : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerAim aim;
        [SerializeField] private float reach = 3f;
        [SerializeField] private float grabTime = 0.6f;
        [SerializeField] private LayerMask mask = ~0;
        [SerializeField] private Collider ownerCollider;

        private Draggable candidate;
        private float heldTime;
        private bool requireRelease;

        public Draggable Current { get; private set; }
        public float GrabProgress { get; private set; }
        public Collider OwnerCollider => ownerCollider;
        public Ray AimRay => aim.GetRay();

        public event Action<Draggable> CurrentChanged;

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
            input.InteractPressed += HandlePressed;
        }

        private void OnDisable()
        {
            input.InteractPressed -= HandlePressed;
            ResetGrab();
            Drop();
        }

        private void Update()
        {
            if (!input.InteractHeld)
            {
                requireRelease = false;
            }

            if (Current != null || requireRelease || !input.InteractHeld)
            {
                ResetGrab();
                return;
            }

            Draggable target = FindTarget();
            if (target != candidate)
            {
                candidate = target;
                heldTime = 0f;
            }

            if (candidate == null)
            {
                GrabProgress = 0f;
                return;
            }

            heldTime += Time.deltaTime;
            GrabProgress = Mathf.Clamp01(heldTime / grabTime);
            if (GrabProgress >= 1f)
            {
                Grab(candidate);
            }
        }

        public void Drop()
        {
            if (Current == null)
            {
                return;
            }

            Draggable released = Current;
            Current = null;
            released.ReleaseGrab();
            CurrentChanged?.Invoke(null);
        }

        private void HandlePressed()
        {
            if (Current == null)
            {
                return;
            }

            requireRelease = true;
            Drop();
        }

        private void Grab(Draggable target)
        {
            ResetGrab();
            requireRelease = true;
            Current = target;
            target.Grab(this);
            CurrentChanged?.Invoke(target);
        }

        private Draggable FindTarget()
        {
            if (!aim.Raycast(reach, mask, out RaycastHit hit))
            {
                return null;
            }

            Draggable target = hit.collider.GetComponentInParent<Draggable>();
            return target != null && target.CanBeGrabbed ? target : null;
        }

        private void ResetGrab()
        {
            candidate = null;
            heldTime = 0f;
            GrabProgress = 0f;
        }
    }
}
