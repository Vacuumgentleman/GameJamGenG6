using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Lanza de caza. Se recoge con E, se equipa como cualquier objeto y el clic izquierdo la lanza.
    /// Al chocar con algo que implemente IDamageable le hace dano y cae al suelo, donde se puede recoger de nuevo.
    /// No requiere cambios en el jugador: solo usa HeldItem, IUsable e IDamageable.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Spear : HeldItem
    {
        [Header("Lanzamiento")]
        [SerializeField] private float throwSpeed = 28f;

        // Distancia delante de la camara donde aparece la lanza al lanzarla.
        [SerializeField] private float spawnDistance = 1f;

        // Segundos maximos de vuelo antes de tratarla como un objeto normal.
        [SerializeField] private float maxFlightTime = 6f;

        [Header("Dano")]
        [SerializeField] private float damage = 50f;

        [Header("En la mano")]
        [SerializeField] private Vector3 handPosition = new Vector3(0.25f, -0.2f, 0.4f);
        [SerializeField] private Vector3 handEuler = Vector3.zero;

        private Rigidbody rb;
        private Collider[] ownColliders;
        private Collider[] userColliders;
        private GameObject thrower;
        private bool flying;
        private float flightTimer;
        private Vector3 lastVelocity;

        public bool IsFlying => flying;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            ownColliders = GetComponentsInChildren<Collider>(true);
        }

        public override void OnPickedUp()
        {
            EndFlight();
            base.OnPickedUp();
        }

        public override void OnEquipped(Transform holdPoint)
        {
            transform.SetLocalPositionAndRotation(handPosition, Quaternion.Euler(handEuler));
        }

        public override UseResult Use(in UseContext context)
        {
            Vector3 direction = context.Aim.direction;
            Vector3 position = context.Aim.origin + direction * spawnDistance;

            thrower = context.User;
            transform.SetParent(null, true);
            OnDropped(position, Quaternion.LookRotation(direction), direction * throwSpeed);

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            SetIgnoreUser(true);
            flying = true;
            flightTimer = 0f;
            lastVelocity = direction * throwSpeed;

            return UseResult.Consumed;
        }

        private void FixedUpdate()
        {
            if (!flying)
            {
                return;
            }

            flightTimer += Time.fixedDeltaTime;
            if (flightTimer >= maxFlightTime)
            {
                EndFlight();
                return;
            }

            Vector3 velocity = rb.linearVelocity;
            if (velocity.sqrMagnitude > 0.5f)
            {
                lastVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
                rb.MoveRotation(Quaternion.LookRotation(velocity));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!flying)
            {
                return;
            }

            IDamageable target = collision.collider.GetComponentInParent<IDamageable>();
            GameObject source = thrower;
            EndFlight();

            if (target == null || target.IsDead)
            {
                return;
            }

            Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            target.TakeDamage(new DamageInfo(damage, point, lastVelocity.normalized, source));
            rb.linearVelocity *= 0.1f;
        }

        private void EndFlight()
        {
            if (!flying)
            {
                return;
            }

            flying = false;
            SetIgnoreUser(false);
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        // Evita que la lanza choque con el propio jugador mientras sale disparada.
        private void SetIgnoreUser(bool ignore)
        {
            if (ignore && thrower != null)
            {
                userColliders = thrower.GetComponentsInChildren<Collider>();
            }

            if (userColliders == null)
            {
                return;
            }

            foreach (Collider own in ownColliders)
            {
                foreach (Collider user in userColliders)
                {
                    if (own != null && user != null)
                    {
                        Physics.IgnoreCollision(own, user, ignore);
                    }
                }
            }

            if (!ignore)
            {
                userColliders = null;
            }
        }
    }
}
