using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam
{
    /// <summary>
    /// Base de los objetos que se lanzan manteniendo el clic izquierdo: acumula fuerza, muestra la trayectoria,
    /// controla la fatiga del brazo y gestiona el vuelo. Al chocar llama a OnImpact, que cada objeto define.
    /// Se saca del inventario al soltar el clic; despues cae como un objeto normal y se puede volver a recoger.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ChargedThrowable : HeldItem
    {
        [Header("Lanzamiento")]
        [SerializeField] private float minThrowSpeed = 8f;
        [SerializeField] private float maxThrowSpeed = 32f;

        // Segundos de clic sostenido para llegar a la fuerza maxima.
        [SerializeField] private float chargeTime = 1.2f;

        [Header("Fatiga")]
        // Segundos que el jugador aguanta la fuerza maxima antes de que el brazo se canse.
        [SerializeField] private float maxHoldTime = 1f;

        // Que pasa al agotar el tiempo: la fuerza baja poco a poco, o el objeto sale solo.
        [SerializeField] private FatigueMode fatigueMode = FatigueMode.Decay;

        // Segundos que tarda la fuerza en bajar de maxima a cero cuando el brazo esta cansado.
        [SerializeField] private float fatigueDropTime = 1.5f;

        // Segundos que se puede seguir sosteniendo sin fuerza antes de que el lanzamiento se cancele.
        [SerializeField] private float cancelDelay = 1f;

        // Intensidad del temblor al acercarse al limite.
        [SerializeField] private float tremble = 0.015f;

        [Header("Vuelo")]
        // Distancia delante de la camara donde aparece el objeto al lanzarlo.
        [SerializeField] private float spawnDistance = 1f;

        // Segundos maximos de vuelo antes de tratarlo como un objeto normal.
        [SerializeField] private float maxFlightTime = 6f;

        [Header("En la mano")]
        [SerializeField] private Vector3 handPosition = new Vector3(0.25f, -0.2f, 0.4f);
        [SerializeField] private Vector3 handEuler = Vector3.zero;

        // Cuanto retrocede el objeto en la mano al cargar la fuerza completa.
        [SerializeField] private Vector3 chargePullback = new Vector3(0f, 0.03f, -0.3f);

        [Header("Trayectoria")]
        [SerializeField] private bool showTrajectory = true;
        [SerializeField] private float trajectoryStep = 0.04f;
        [SerializeField] private float trajectoryMaxTime = 3f;
        [SerializeField] private float trajectoryWidth = 0.03f;
        [SerializeField] private Color trajectoryColor = Color.white;
        [SerializeField] private Color trajectoryFullChargeColor = new Color(1f, 0.35f, 0.15f);

        // Material de la linea. Si se deja vacio se crea uno simple automaticamente.
        [SerializeField] private Material trajectoryMaterial;

        [Header("Sonidos")]
        [SerializeField] private AudioCue pickupCue;
        [SerializeField] private AudioCue equipCue;
        [SerializeField] private AudioCue chargeCue;
        [SerializeField] private AudioCue releaseCue;

        private Rigidbody rb;
        private Collider[] ownColliders;
        private Collider[] userColliders;
        private GameObject thrower;
        private PlayerAim aim;
        private LineRenderer line;
        private Transform marker;
        private Renderer markerRenderer;
        private bool charging;
        private float charge;
        private float holdTimer;
        private bool tired;
        private float exhaustedTimer;
        private bool flying;
        private float flightTimer;
        private Vector3 lastVelocity;

        // Objeto que se esta cargando ahora mismo. Lo usa la interfaz para mostrar la barra de fuerza.
        public static ChargedThrowable Charging { get; private set; }

        public bool IsFlying => flying;
        public bool IsCharging => charging;
        public float Charge01 => charge;

        // 0 a 1: que tan cerca esta el brazo de cansarse.
        public float Strain01 => tired ? 1f : Mathf.Clamp01(holdTimer / Mathf.Max(0.01f, maxHoldTime));

        public bool IsTired => tired;

        protected Rigidbody Body => rb;
        protected GameObject Thrower => thrower;
        protected Vector3 LastVelocity => lastVelocity;

        public enum FatigueMode
        {
            Decay,
            AutoThrow
        }

        private enum ChargeResult
        {
            Continue,
            Release,
            Cancel
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Charging = null;
        }

        // Se llama una vez al chocar en vuelo, con el vuelo ya terminado. Cada objeto decide que hacer.
        protected abstract void OnImpact(Collision collision, Vector3 point);

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            ownColliders = GetComponentsInChildren<Collider>(true);
            CreateTrajectoryVisuals();
        }

        private void OnDisable()
        {
            CancelCharge();
        }

        public override void OnPickedUp()
        {
            CancelCharge();
            EndFlight();
            base.OnPickedUp();
            pickupCue.Play(transform.position);
        }

        public override void OnDropped(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            CancelCharge();
            base.OnDropped(position, rotation, velocity);
        }

        public override void OnEquipped(Transform holdPoint)
        {
            transform.SetLocalPositionAndRotation(handPosition, Quaternion.Euler(handEuler));
            equipCue.Play(transform.position);
        }

        public override void OnUnequipped()
        {
            CancelCharge();
        }

        // El clic izquierdo solo inicia la carga. El lanzamiento ocurre al soltar el boton.
        public override UseResult Use(in UseContext context)
        {
            if (charging || flying)
            {
                return UseResult.Kept;
            }

            thrower = context.User;
            aim = thrower.GetComponent<PlayerAim>();

            // Sin PlayerAim no se puede apuntar mientras se carga: lanzamiento inmediato.
            if (aim == null)
            {
                Launch(context.Aim, maxThrowSpeed);
                return UseResult.Consumed;
            }

            charging = true;
            charge = 0f;
            holdTimer = 0f;
            tired = false;
            exhaustedTimer = 0f;
            Charging = this;
            SetTrajectoryVisible(showTrajectory);
            chargeCue.Play(transform.position);
            return UseResult.Kept;
        }

        private void Update()
        {
            if (!charging || GamePause.IsPaused)
            {
                return;
            }

            ChargeResult result = UpdateCharge();
            if (result == ChargeResult.Cancel)
            {
                CancelCharge();
                return;
            }

            Ray ray = aim.GetRay();
            float speed = Mathf.Lerp(minThrowSpeed, maxThrowSpeed, Charge01);

            Vector3 pose = Vector3.Lerp(handPosition, handPosition + chargePullback, Charge01);
            transform.localPosition = pose + TrembleOffset();

            if (showTrajectory)
            {
                DrawTrajectory(ray, speed);
            }

            if (result == ChargeResult.Release || !IsAttackHeld())
            {
                Release(ray, speed);
            }
        }

        // Sube la fuerza hasta el maximo; si se mantiene demasiado, el brazo se cansa.
        // Si ademas se sigue sosteniendo sin fuerza, el lanzamiento se cancela.
        private ChargeResult UpdateCharge()
        {
            float dt = Time.deltaTime;

            if (tired)
            {
                charge = Mathf.Max(0f, charge - dt / Mathf.Max(0.01f, fatigueDropTime));
                if (charge > 0f)
                {
                    return ChargeResult.Continue;
                }

                exhaustedTimer += dt;
                return exhaustedTimer >= cancelDelay ? ChargeResult.Cancel : ChargeResult.Continue;
            }

            if (charge < 1f)
            {
                charge = Mathf.Min(1f, charge + dt / Mathf.Max(0.01f, chargeTime));
                return ChargeResult.Continue;
            }

            holdTimer += dt;
            if (holdTimer < maxHoldTime)
            {
                return ChargeResult.Continue;
            }

            if (fatigueMode == FatigueMode.AutoThrow)
            {
                return ChargeResult.Release;
            }

            tired = true;
            return ChargeResult.Continue;
        }

        private Vector3 TrembleOffset()
        {
            float strain = Strain01;
            if (strain < 0.5f || tremble <= 0f)
            {
                return Vector3.zero;
            }

            float amount = tremble * (strain - 0.5f) * 2f;
            float t = Time.time * 40f;
            return new Vector3(
                Mathf.PerlinNoise(t, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, t) - 0.5f,
                0f) * (2f * amount);
        }

        private void Release(Ray ray, float speed)
        {
            StopCharging();
            RemoveFromInventory();
            Launch(ray, speed);
        }

        private void Launch(Ray ray, float speed)
        {
            Vector3 direction = ray.direction;
            Vector3 position = ray.origin + direction * spawnDistance;

            transform.SetParent(null, true);
            OnDropped(position, Quaternion.LookRotation(direction), direction * speed);

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            SetIgnoreUser(true);
            flying = true;
            flightTimer = 0f;
            lastVelocity = direction * speed;
            releaseCue.Play(position);
        }

        private void RemoveFromInventory()
        {
            if (thrower == null)
            {
                return;
            }

            PlayerInventory inventory = thrower.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                return;
            }

            for (int i = 0; i < inventory.Capacity; i++)
            {
                if (ReferenceEquals(inventory.GetItem(i), this))
                {
                    inventory.Remove(i);
                    return;
                }
            }
        }

        private void CancelCharge()
        {
            if (!charging)
            {
                return;
            }

            StopCharging();
            transform.localPosition = handPosition;
        }

        private void StopCharging()
        {
            charging = false;
            if (Charging == this)
            {
                Charging = null;
            }

            SetTrajectoryVisible(false);
        }

        private static bool IsAttackHeld()
        {
            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.isPressed)
            {
                return true;
            }

            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.enterKey.isPressed;
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

            Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            EndFlight();
            OnImpact(collision, point);
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

        // Evita que el objeto choque con el propio jugador mientras sale disparado.
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

        // Calcula el arco con la misma formula de la fisica (posicion inicial + velocidad + gravedad).
        private void DrawTrajectory(Ray ray, float speed)
        {
            if (line == null)
            {
                return;
            }

            Vector3 start = ray.origin + ray.direction * spawnDistance;
            Vector3 velocity = ray.direction * speed;
            Vector3 gravity = Physics.gravity;

            int maxSteps = Mathf.Max(1, Mathf.CeilToInt(trajectoryMaxTime / trajectoryStep));
            line.positionCount = 1;
            line.SetPosition(0, start);

            Vector3 previous = start;
            bool hitSomething = false;
            Vector3 end = start;

            for (int i = 1; i <= maxSteps; i++)
            {
                float t = i * trajectoryStep;
                Vector3 point = start + velocity * t + 0.5f * gravity * t * t;

                if (Physics.Linecast(previous, point, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
                {
                    point = hit.point;
                    hitSomething = true;
                }

                line.positionCount = i + 1;
                line.SetPosition(i, point);
                end = point;
                previous = point;

                if (hitSomething)
                {
                    break;
                }
            }

            Color color = Color.Lerp(trajectoryColor, trajectoryFullChargeColor, Charge01);
            line.startColor = color;
            line.endColor = new Color(color.r, color.g, color.b, 0.25f);

            if (marker != null)
            {
                marker.gameObject.SetActive(hitSomething);
                marker.position = end;
                markerRenderer.material.color = color;
            }
        }

        private void CreateTrajectoryVisuals()
        {
            Material material = trajectoryMaterial;
            if (material == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null)
                {
                    showTrajectory = false;
                    return;
                }

                material = new Material(shader);
            }

            GameObject lineObject = new GameObject("TrajectoryLine");
            lineObject.transform.SetParent(transform, false);
            line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.widthMultiplier = trajectoryWidth;
            line.numCapVertices = 2;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.sharedMaterial = material;
            line.enabled = false;

            GameObject markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            markerObject.name = "TrajectoryMarker";
            Destroy(markerObject.GetComponent<Collider>());
            marker = markerObject.transform;
            marker.SetParent(transform, false);
            marker.localScale = Vector3.one * 0.2f;
            markerRenderer = markerObject.GetComponent<Renderer>();
            markerRenderer.sharedMaterial = new Material(material);
            markerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            markerRenderer.receiveShadows = false;
            markerObject.SetActive(false);
        }

        private void SetTrajectoryVisible(bool visible)
        {
            if (line != null)
            {
                line.enabled = visible;
            }

            if (!visible && marker != null)
            {
                marker.gameObject.SetActive(false);
            }
        }
    }
}
