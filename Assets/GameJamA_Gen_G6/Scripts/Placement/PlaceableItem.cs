using UnityEngine;

namespace GameJam
{
    public class PlaceableItem : MonoBehaviour, IPickable, IHighlightable
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private string pickHint = "Manten E: recoger";

        private Collider[] colliders;
        private bool held;

        public string ItemId => itemId;
        public bool IsPlaced { get; private set; }

        public string DisplayName => displayName;
        public GameObject GameObject => gameObject;
        public bool CanHighlight => !held && !IsPlaced;
        public string HighlightText => $"{displayName}\n{pickHint}";

        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider>(true);
            foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
            {
                body.isKinematic = true;
                body.useGravity = false;
            }
        }

        public void OnPickedUp()
        {
            held = true;
            SetCollidersEnabled(false);
        }

        public void OnDropped(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            held = false;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, rotation.eulerAngles.y, 0f));
            gameObject.SetActive(true);
            SetCollidersEnabled(true);
            PlacementSlot.TryPlaceNear(this, position);
        }

        public void Place(Transform point)
        {
            held = false;
            IsPlaced = true;
            SetCollidersEnabled(false);
            transform.SetPositionAndRotation(point.position, point.rotation);
            gameObject.SetActive(true);
        }

        private void SetCollidersEnabled(bool value)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = value;
            }
        }
    }
}
