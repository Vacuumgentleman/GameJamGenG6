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
        [SerializeField] private float alignSpeed = 360f;

        private HeldItem item;
        private PlayerInteractor interactor;
        private Vector3 anchor;
        private float phase;
        private bool seen;

        private void Awake()
        {
            item = GetComponent<HeldItem>();
            GetComponent<Rigidbody>().isKinematic = true;
            anchor = transform.position + Vector3.up * liftHeight;
            phase = Random.value * Mathf.PI * 2f;
            item.PickedUp += HandlePickedUp;
        }

        private void Start()
        {
            interactor = FindFirstObjectByType<PlayerInteractor>();
        }

        private void OnDestroy()
        {
            item.PickedUp -= HandlePickedUp;
        }

        private void Update()
        {
            if (IsFocused())
            {
                seen = true;
                AlignToViewer();
            }
            else if (!seen)
            {
                transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            }

            Vector3 position = anchor;
            position.y += Mathf.Sin(Time.time * bobSpeed + phase) * bobAmplitude;
            transform.position = position;
        }

        private bool IsFocused()
        {
            return interactor != null && ReferenceEquals(interactor.Focus, item);
        }

        private void AlignToViewer()
        {
            Vector3 toViewer = interactor.transform.position - transform.position;
            toViewer.y = 0f;
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (toViewer.sqrMagnitude < 0.0001f || forward.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Vector3 side = Vector3.Cross(Vector3.up, toViewer).normalized;
            if (Vector3.Dot(side, forward) < 0f)
            {
                side = -side;
            }

            float step = alignSpeed * Time.deltaTime;
            float angle = Vector3.SignedAngle(forward, side, Vector3.up);
            transform.Rotate(Vector3.up, Mathf.Clamp(angle, -step, step), Space.World);
        }

        private void HandlePickedUp()
        {
            enabled = false;
        }
    }
}
