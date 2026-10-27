using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Movimiento del jugador: caminar, correr (Shift), agacharse (C) y gravedad.
    /// No hay salto. Usa CharacterController, que es mas estable que un Rigidbody para primera persona.
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

        [Header("Gravedad")]
        [SerializeField] private float gravity = 20f;

        // Pequena fuerza hacia abajo para mantener al jugador pegado al suelo.
        [SerializeField] private float groundedStickForce = 2f;

        private CharacterController controller;
        private float currentHeight;
        private float verticalVelocity;

        // Estados de solo lectura, utiles para animaciones, sonidos de pasos, etc.
        public bool IsCrouching { get; private set; }
        public bool IsRunning { get; private set; }
        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            currentHeight = standHeight;
            ApplyHeight();
        }

        private void Update()
        {
            UpdateCrouch();

            // Limita la entrada a 1 para que moverse en diagonal no sea mas rapido.
            Vector2 move = Vector2.ClampMagnitude(input.Move, 1f);

            // Solo se corre hacia adelante y de pie.
            IsRunning = input.SprintHeld && !IsCrouching && move.y > 0.1f;

            float speed = IsCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed;

            // Movimiento relativo a hacia donde mira el cuerpo del jugador.
            Vector3 horizontal = (transform.right * move.x + transform.forward * move.y) * speed;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -groundedStickForce;
            }

            verticalVelocity -= gravity * Time.deltaTime;
            CurrentSpeed = horizontal.magnitude;

            controller.Move((horizontal + Vector3.up * verticalVelocity) * Time.deltaTime);
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
