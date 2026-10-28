using System;
using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Rigidbody))]
    public class Draggable : MonoBehaviour, IHighlightable
    {
        public enum State { Free, Grabbed, Locked }

        [SerializeField] private string displayName = "Fantasma";
        [SerializeField] private string grabHint = "E: agarrar y arrastrar";

        [Header("Arrastre")]
        [SerializeField] private float dragDistance = 2.2f;
        [SerializeField] private float minHeightAbovePlayer = 0.3f;
        [SerializeField] private float maxHeightAbovePlayer = 3.5f;
        [SerializeField] private float followSharpness = 10f;

        [Header("Flotacion")]
        [SerializeField] private float bobAmplitude = 0.12f;
        [SerializeField] private float bobSpeed = 1.5f;

        [Header("Temblor al arrastrar")]
        [SerializeField] private float shakePosition = 0.04f;
        [SerializeField] private float shakeAngle = 6f;

        [Header("Sonidos")]
        [SerializeField] private AudioCue grabCue;
        [SerializeField] private AudioCue releaseCue;

        private Rigidbody body;
        private Collider[] colliders;
        private bool wasKinematic;
        private bool wasUsingGravity;
        private Vector3 hoverAnchor;
        private Vector3 heldPosition;
        private float bobPhase;

        public State CurrentState { get; private set; } = State.Free;
        public PlayerDragger Dragger { get; private set; }
        public bool CanBeGrabbed => CurrentState == State.Free;

        public bool CanHighlight => CurrentState == State.Free;
        public string HighlightText => $"{displayName}\n{grabHint}";

        public event Action<Draggable> Consumed;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();
            hoverAnchor = body.position;
            bobPhase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        private void FixedUpdate()
        {
            switch (CurrentState)
            {
                case State.Free:
                    Hover();
                    break;
                case State.Grabbed:
                    Follow();
                    break;
            }
        }

        private void Hover()
        {
            Vector3 position = hoverAnchor;
            position.y += Mathf.Sin(Time.time * bobSpeed + bobPhase) * bobAmplitude;
            body.MovePosition(position);
        }

        private void Follow()
        {
            if (Dragger == null)
            {
                return;
            }

            Ray ray = Dragger.AimRay;
            Vector3 target = ray.origin + ray.direction * dragDistance;

            float playerY = Dragger.transform.position.y;
            target.y = Mathf.Clamp(target.y, playerY + minHeightAbovePlayer, playerY + maxHeightAbovePlayer);

            float t = Mathf.Clamp01(followSharpness * Time.fixedDeltaTime);
            heldPosition = Vector3.Lerp(heldPosition, target, t);

            Quaternion facing = Quaternion.LookRotation(Dragger.FlatForward);
            Quaternion jitter = Quaternion.Euler(
                UnityEngine.Random.Range(-shakeAngle, shakeAngle),
                0f,
                UnityEngine.Random.Range(-shakeAngle, shakeAngle));

            body.MovePosition(heldPosition + UnityEngine.Random.insideUnitSphere * shakePosition);
            body.MoveRotation(Quaternion.Slerp(body.rotation, facing * jitter, t));
        }

        public void Grab(PlayerDragger dragger)
        {
            if (!CanBeGrabbed)
            {
                return;
            }

            CurrentState = State.Grabbed;
            Dragger = dragger;

            wasKinematic = body.isKinematic;
            wasUsingGravity = body.useGravity;
            if (!wasKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            body.isKinematic = true;
            heldPosition = body.position;
            SetIgnorePlayerCollision(true);
            grabCue.Play(transform.position);
        }

        public void ReleaseGrab()
        {
            if (CurrentState != State.Grabbed)
            {
                return;
            }

            SetIgnorePlayerCollision(false);
            body.isKinematic = wasKinematic;
            body.useGravity = wasUsingGravity;
            hoverAnchor = heldPosition;
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            CurrentState = State.Free;
            Dragger = null;
            releaseCue.Play(transform.position);
        }

        public void Lock(Vector3 position)
        {
            if (CurrentState == State.Locked)
            {
                return;
            }

            if (CurrentState == State.Grabbed && Dragger != null)
            {
                Dragger.Drop();
            }

            CurrentState = State.Locked;
            body.isKinematic = true;
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }

            body.position = position;
            transform.position = position;
        }

        public void Consume()
        {
            Consumed?.Invoke(this);
            Destroy(gameObject);
        }

        private void SetIgnorePlayerCollision(bool ignore)
        {
            if (Dragger == null || Dragger.OwnerCollider == null)
            {
                return;
            }

            foreach (Collider c in colliders)
            {
                Physics.IgnoreCollision(c, Dragger.OwnerCollider, ignore);
            }
        }
    }
}
