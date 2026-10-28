using UnityEngine;

namespace GameJam
{
    public class PlaceableItem : MonoBehaviour, IPickable, IHighlightable
    {
        private const string DefaultPickHint = "Manten E: recoger";

        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private string pickHint = DefaultPickHint;

        [Header("Movimiento en el suelo")]
        [SerializeField] private float spinSpeed = 60f;
        [SerializeField] private float bobAmplitude = 0.1f;
        [SerializeField] private float bobSpeed = 1.5f;

        private Collider[] colliders;
        private AudioBeacon beacon;
        private bool held;
        private Vector3 anchor;
        private float phase;

        public string ItemId => itemId;
        public bool IsPlaced { get; private set; }

        public string DisplayName => displayName;
        public GameObject GameObject => gameObject;
        public bool CanHighlight => !held && !IsPlaced;
        public string HighlightText => $"{displayName}\n{(string.IsNullOrEmpty(pickHint) ? DefaultPickHint : pickHint)}";

        private void Awake()
        {
            Vector3 scale = transform.lossyScale;
            transform.SetParent(null, true);
            transform.localScale = scale;
            anchor = transform.position;
            phase = Random.value * Mathf.PI * 2f;

            colliders = GetComponentsInChildren<Collider>(true);
            beacon = GetComponent<AudioBeacon>();
            foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>(true))
            {
                body.isKinematic = true;
                body.useGravity = false;
            }
        }

        private void Update()
        {
            if (held || IsPlaced)
            {
                return;
            }

            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            Vector3 position = anchor;
            position.y += Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
            transform.position = position;
        }

        public void OnPickedUp()
        {
            held = true;
            SetCollidersEnabled(false);
            SetBeaconEnabled(false);
        }

        public void OnDropped(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            held = false;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, rotation.eulerAngles.y, 0f));
            anchor = position;
            gameObject.SetActive(true);
            SetCollidersEnabled(true);
            SetBeaconEnabled(true);
            PlacementSlot.TryPlaceNear(this, position);
        }

        public void Place(Transform point)
        {
            held = false;
            IsPlaced = true;
            SetCollidersEnabled(false);
            SetBeaconEnabled(false);
            transform.SetPositionAndRotation(point.position, point.rotation);
            gameObject.SetActive(true);
        }

        private void SetBeaconEnabled(bool value)
        {
            if (beacon != null)
            {
                beacon.enabled = value;
            }
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
