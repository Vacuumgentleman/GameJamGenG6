using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Cuervo: vuela hasta una parcela sembrada, picotea y se come lo plantado si nadie lo espanta.
    /// Se asusta cuando el jugador se acerca o cuando le cae una semilla lanzada, y huye.
    /// No necesita NavMesh: vuela en linea recta.
    /// </summary>
    public class Crow : MonoBehaviour, IScareable
    {
        [Header("Vuelo")]
        [SerializeField] private float flySpeed = 8f;
        [SerializeField] private float fleeSpeed = 12f;
        [SerializeField] private float fleeLifetime = 4f;

        // Altura sobre la parcela a la que se posa.
        [SerializeField] private float perchHeight = 0.4f;

        [Header("Comportamiento")]
        // Segundos que picotea antes de comerse lo plantado.
        [SerializeField] private float eatTime = 4f;

        // Distancia a la que el jugador lo asusta.
        [SerializeField] private float scareRadius = 7f;

        [Header("Alas")]
        [SerializeField] private Transform wingLeft;
        [SerializeField] private Transform wingRight;
        [SerializeField] private float flapSpeed = 20f;
        [SerializeField] private float flapAngle = 40f;
        [SerializeField] private float restAngle = -15f;

        [Header("Sonidos")]
        [SerializeField] private AudioCue arriveCue;
        [SerializeField] private AudioCue scaredCue;
        [SerializeField] private AudioCue eatCue;

        private enum Phase
        {
            Approach,
            Eating,
            Flee
        }

        private PlantingPlot plot;
        private Transform player;
        private Vector3 perch;
        private Vector3 fleeDirection;
        private Phase phase;
        private float timer;
        private bool counted;

        // Cuantos cuervos estan comiendo ahora mismo. Lo usa la interfaz para avisar.
        public static int EatingCount { get; private set; }

        public PlantingPlot Plot => plot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            EatingCount = 0;
        }

        public void Initialize(PlantingPlot target)
        {
            plot = target;
            perch = target.transform.position + Vector3.up * perchHeight;
            phase = Phase.Approach;
        }

        private void Start()
        {
            PlayerMovement found = FindFirstObjectByType<PlayerMovement>();
            player = found != null ? found.transform : null;
        }

        private void OnDestroy()
        {
            SetCounted(false);
        }

        private void Update()
        {
            switch (phase)
            {
                case Phase.Approach:
                    UpdateApproach();
                    break;
                case Phase.Eating:
                    UpdateEating();
                    break;
                case Phase.Flee:
                    UpdateFlee();
                    break;
            }
        }

        public void Scare(Vector3 source)
        {
            if (phase == Phase.Flee)
            {
                return;
            }

            Vector3 away = transform.position - source;
            away.y = 0f;
            if (away.sqrMagnitude < 0.01f)
            {
                away = transform.forward;
            }

            StartFleeing(away.normalized);
            scaredCue.Play(transform.position);
        }

        private void UpdateApproach()
        {
            if (plot == null || !plot.IsVulnerable)
            {
                StartFleeing(transform.forward);
                return;
            }

            if (PlayerIsNear())
            {
                Scare(player.position);
                return;
            }

            Vector3 toPerch = perch - transform.position;
            Face(toPerch);
            transform.position = Vector3.MoveTowards(transform.position, perch, flySpeed * Time.deltaTime);
            Flap(true);

            if (toPerch.sqrMagnitude < 0.01f)
            {
                phase = Phase.Eating;
                timer = eatTime;
                SetCounted(true);
                arriveCue.Play(transform.position);
            }
        }

        private void UpdateEating()
        {
            Flap(false);

            if (plot == null || !plot.IsVulnerable)
            {
                StartFleeing(transform.forward);
                return;
            }

            if (PlayerIsNear())
            {
                Scare(player.position);
                return;
            }

            timer -= Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }

            plot.TryEat();
            eatCue.Play(transform.position);
            StartFleeing(transform.forward);
        }

        private void UpdateFlee()
        {
            Flap(true);
            Face(fleeDirection);
            transform.position += fleeDirection * (fleeSpeed * Time.deltaTime);

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void StartFleeing(Vector3 horizontalDirection)
        {
            phase = Phase.Flee;
            timer = fleeLifetime;
            fleeDirection = (new Vector3(horizontalDirection.x, 0f, horizontalDirection.z).normalized + Vector3.up * 0.7f).normalized;
            SetCounted(false);
        }

        private bool PlayerIsNear()
        {
            if (player == null)
            {
                return false;
            }

            Vector3 offset = player.position - transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude < scareRadius * scareRadius;
        }

        private void Face(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, Quaternion.LookRotation(direction), 720f * Time.deltaTime);
            }
        }

        private void Flap(bool flying)
        {
            float angle = flying ? Mathf.Sin(Time.time * flapSpeed) * flapAngle : restAngle;

            if (wingLeft != null)
            {
                wingLeft.localRotation = Quaternion.Euler(0f, 0f, -angle);
            }

            if (wingRight != null)
            {
                wingRight.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void SetCounted(bool value)
        {
            if (counted == value)
            {
                return;
            }

            counted = value;
            EatingCount += value ? 1 : -1;
        }
    }
}
