using System;
using System.Collections;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Animal de caza: deambula cerca de su punto de origen, huye cuando el jugador se acerca
    /// y muere al recibir suficiente dano. No necesita NavMesh: usa CharacterController.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Animal : MonoBehaviour, IDamageable
    {
        [Header("Vida")]
        [SerializeField] private float maxHealth = 50f;

        [Header("Movimiento")]
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float fleeSpeed = 6.5f;
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private float gravity = 20f;

        [Header("Comportamiento")]
        // Radio alrededor del origen en el que elige puntos al deambular.
        [SerializeField] private float wanderRadius = 8f;

        // Distancia a la que el jugador lo asusta.
        [SerializeField] private float detectRadius = 12f;

        // Distancia a la que se calma y deja de huir.
        [SerializeField] private float calmRadius = 18f;

        // Distancia maxima al origen mientras huye.
        [SerializeField] private float leashRadius = 22f;

        // Pausa (min, max) en segundos al llegar a un punto.
        [SerializeField] private Vector2 idleTime = new Vector2(1.5f, 4f);

        private CharacterController controller;
        private Transform player;
        private Vector3 home;
        private Vector3 wanderTarget;
        private float idleTimer;
        private float health;
        private float verticalVelocity;
        private bool fleeing;

        public bool IsDead { get; private set; }
        public bool IsFleeing => fleeing;

        public event Action<Animal> Died;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            health = maxHealth;
            home = transform.position;
            wanderTarget = home;
        }

        private void Start()
        {
            PlayerMovement found = FindFirstObjectByType<PlayerMovement>();
            if (found != null)
            {
                player = found.transform;
            }
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            Vector3 direction = Vector3.zero;
            float speed = 0f;

            UpdateFleeState();

            if (fleeing)
            {
                direction = FleeDirection();
                speed = fleeSpeed;
            }
            else
            {
                UpdateWander(ref direction, ref speed);
            }

            Move(direction, speed);
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (IsDead)
            {
                return;
            }

            health -= info.Amount;
            fleeing = true;

            if (health <= 0f)
            {
                Die();
            }
        }

        private void UpdateFleeState()
        {
            if (player == null)
            {
                return;
            }

            float distance = FlatDistance(transform.position, player.position);
            if (fleeing)
            {
                fleeing = distance < calmRadius;
            }
            else
            {
                fleeing = distance < detectRadius;
            }
        }

        private Vector3 FleeDirection()
        {
            Vector3 away = player != null ? Flat(transform.position - player.position) : transform.forward;
            if (away.sqrMagnitude < 0.0001f)
            {
                away = transform.forward;
            }

            away.Normalize();

            // Si se aleja demasiado del origen, se mezcla con la direccion de vuelta.
            Vector3 toHome = Flat(home - transform.position);
            if (toHome.magnitude > leashRadius)
            {
                away = (away * 0.4f + toHome.normalized).normalized;
            }

            return away;
        }

        private void UpdateWander(ref Vector3 direction, ref float speed)
        {
            if (idleTimer > 0f)
            {
                idleTimer -= Time.deltaTime;
                return;
            }

            Vector3 toTarget = Flat(wanderTarget - transform.position);
            if (toTarget.magnitude < 0.5f)
            {
                idleTimer = UnityEngine.Random.Range(idleTime.x, idleTime.y);
                Vector2 offset = UnityEngine.Random.insideUnitCircle * wanderRadius;
                wanderTarget = home + new Vector3(offset.x, 0f, offset.y);
                return;
            }

            direction = toTarget.normalized;
            speed = walkSpeed;
        }

        private void Move(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity -= gravity * Time.deltaTime;
            controller.Move((direction * speed + Vector3.up * verticalVelocity) * Time.deltaTime);
        }

        private void Die()
        {
            IsDead = true;
            controller.enabled = false;
            StartCoroutine(FallOver());
            Died?.Invoke(this);
        }

        // Animacion simple: el animal se tumba de lado.
        private IEnumerator FallOver()
        {
            Quaternion fromRotation = transform.rotation;
            Quaternion toRotation = fromRotation * Quaternion.Euler(0f, 0f, 90f);
            Vector3 fromPosition = transform.position;
            Vector3 toPosition = fromPosition + Vector3.up * 0.25f;

            const float duration = 0.4f;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float k = t / duration;
                transform.SetPositionAndRotation(
                    Vector3.Lerp(fromPosition, toPosition, k),
                    Quaternion.Slerp(fromRotation, toRotation, k));
                yield return null;
            }

            transform.SetPositionAndRotation(toPosition, toRotation);
        }

        private static Vector3 Flat(Vector3 value)
        {
            value.y = 0f;
            return value;
        }

        private static float FlatDistance(Vector3 a, Vector3 b)
        {
            return Flat(a - b).magnitude;
        }
    }
}
