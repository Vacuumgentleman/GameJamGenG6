using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(HeldItem), typeof(Rigidbody))]
    public class ItemHover : MonoBehaviour
    {
        [SerializeField] private float liftHeight = 0.3f;
        [SerializeField] private float spinSpeed = 60f;
        [SerializeField] private float bobAmplitude = 0.08f;
        [SerializeField] private float bobSpeed = 1.5f;

        private HeldItem item;
        private AudioBeacon beacon;
        private Vector3 anchor;
        private float phase;

        private void Awake()
        {
            item = GetComponent<HeldItem>();
            beacon = GetComponent<AudioBeacon>();
            GetComponent<Rigidbody>().isKinematic = true;
            anchor = transform.position + Vector3.up * liftHeight;
            phase = Random.value * Mathf.PI * 2f;
            item.PickedUp += HandlePickedUp;
        }

        private void OnDestroy()
        {
            item.PickedUp -= HandlePickedUp;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            Vector3 position = anchor;
            position.y += Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
            transform.position = position;
        }

        private void HandlePickedUp()
        {
            if (beacon != null)
            {
                beacon.enabled = false;
            }

            enabled = false;
        }
    }
}
