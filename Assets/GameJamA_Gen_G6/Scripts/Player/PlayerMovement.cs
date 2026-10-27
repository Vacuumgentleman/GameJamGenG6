using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Movimiento del jugador: caminar, correr (Shift), agacharse (C), saltar (Espacio) y gravedad.
    /// Usa CharacterController, que es mas estable que un Rigidbody para primera persona.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;

        // Se baja al agacharse para que la vista baje con el cuerpo.
        [SerializeField] private Transform cameraRoot;

        [Header("Velocidades (metros por segundo)")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float crouchSpeed = 2f;

        [Header("Cuerpo")]
        [SerializeField] private float standHeight = 1.8f;
        [SerializeField] private float crouchHeight = 1f;

        // Distancia entre la coronilla y los ojos.
        [SerializeField] private float eyeOffset = 0.2f;

        // Rapidez con la que cambia la altura al agacharse o levantarse.
        [SerializeField] private float heightChangeSpeed = 6f;

        [Header("Terreno")]
        // Altura maxima de un escalon que se sube caminando, sin saltar.
        [SerializeField] private float stepHeight = 0.4f;

        // Inclinacion maxima (en grados) de una rampa que se puede subir.
        [SerializeField] private float slopeLimit = 50f;

        [Header("Gravedad")]
        [SerializeField] private float gravity = 20f;

        // Pequena fuerza hacia abajo para mantener al jugador pegado al suelo.
        [SerializeField] private float groundedStickForce = 2f;

        [Header("Salto")]
        // Altura del salto en metros. La velocidad inicial se calcula a partir de esto y la gravedad.
        [SerializeField] private float jumpHeight = 0.6f;

        // Cuanto cambia la velocidad horizontal por segundo mientras se esta en el aire.
        // Valor bajo = el salto conserva el impulso y se corrige poco.
        [SerializeField] private float airControl = 6f;

        // Tiempo (segundos) en que aun se puede saltar despues de dejar de pisar el suelo.
        [SerializeField] private float coyoteTime = 0.12f;

        // Tiempo (segundos) que se recuerda un salto pulsado justo antes de tocar el suelo.
        [SerializeField] private float jumpBufferTime = 0.12f;

        private CharacterController controller;
        private float currentHeight;
        private float verticalVelocity;
        private Vector3 horizontalVelocity;
        private float coyoteTimer;
        private float jumpBufferTimer;

        // Estados de solo lectura, utiles para animaciones, sonidos de pasos, etc.
        public bool IsCrouching { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsGrounded { get; private set; }
        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            controller.stepOffset = stepHeight;
            controller.slopeLimit = slopeLimit;
            currentHeight = standHeight;
            ApplyHeight();
        }

        private void OnEnable()
        {
            input.JumpPerformed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            input.JumpPerformed -= OnJumpPerformed;
        }

        private void Update()
        {
            UpdateCrouch();
            UpdateGroundState();

            // Limita la entrada a 1 para que moverse en diagonal no sea mas rapido.
            Vector2 move = Vector2.ClampMagnitude(input.Move, 1f);

            // Solo se corre hacia adelante y de pie.
            IsRunning = input.SprintHeld && !IsCrouching && move.y > 0.1f;

            float speed = IsCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed;

            // Movimiento relativo a hacia donde mira el cuerpo del jugador.
            Vector3 desired = (transform.right * move.x + transform.forward * move.y) * speed;

            // En el suelo se sigue la entrada directamente; en el aire solo se corrige poco a poco.
            horizontalVelocity = IsGrounded
                ? desired
                : Vector3.MoveTowards(horizontalVelocity, desired, airControl * Time.deltaTime);

            TryJump();

            if (IsGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -groundedStickForce;
            }

            verticalVelocity -= gravity * Time.deltaTime;
            CurrentSpeed = horizontalVelocity.magnitude;

            CollisionFlags flags = controller.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);

            // Si se golpea un techo mientras se sube, se cancela el impulso.
            if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
            {
                verticalVelocity = 0f;
            }
        }

        private void OnJumpPerformed()
        {
            jumpBufferTimer = jumpBufferTime;
        }

        private void UpdateGroundState()
        {
            IsGrounded = controller.isGrounded;
            coyoteTimer = IsGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
            jumpBufferTimer -= Time.deltaTime;
        }

        private void TryJump()
        {
            // No se puede saltar agachado.
            if (jumpBufferTimer <= 0f || coyoteTimer <= 0f || IsCrouching)
            {
                return;
            }

            verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            IsGrounded = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        private void UpdateCrouch()
        {
            if (input.CrouchHeld)
            {
                IsCrouching = true;
            }
            else if (IsCrouching && CanStand())
            {
                // Solo se levanta si no hay un techo encima.
                IsCrouching = false;
            }

            float target = IsCrouching ? crouchHeight : standHeight;
            currentHeight = Mathf.MoveTowards(currentHeight, target, heightChangeSpeed * Time.deltaTime);
            ApplyHeight();
        }

        // Lanza una esfera hacia arriba para comprobar si hay espacio para ponerse de pie.
        private bool CanStand()
        {
            float rise = standHeight - currentHeight;
            if (rise <= 0.01f)
            {
                return true;
            }

            Vector3 origin = transform.position + Vector3.up * (currentHeight - controller.radius);
            return !Physics.SphereCast(
                origin,
                controller.radius * 0.9f,
                Vector3.up,
                out _,
                rise,
                ~0,
                QueryTriggerInteraction.Ignore);
        }

        // Ajusta el collider y la altura de los ojos a la altura actual.
        private void ApplyHeight()
        {
            controller.height = currentHeight;
            controller.center = new Vector3(0f, currentHeight * 0.5f, 0f);
            cameraRoot.localPosition = new Vector3(0f, currentHeight - eyeOffset, 0f);
        }
    }
}
