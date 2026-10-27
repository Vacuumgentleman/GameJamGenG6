using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        [Header("Pasos")]
        [SerializeField] private AudioCue walkCue;
        [SerializeField] private AudioCue runCue;
        [SerializeField] private AudioCue crouchCue;

        [Header("Metros entre pasos")]
        [SerializeField] private float walkStride = 2.2f;
        [SerializeField] private float runStride = 2.8f;
        [SerializeField] private float crouchStride = 1.2f;
        [SerializeField] private float minMoveSpeed = 0.2f;

        [Header("Salto y caida")]
        [SerializeField] private AudioCue jumpCue;
        [SerializeField] private AudioCue landCue;
        [SerializeField] private float minAirTime = 0.25f;

        private bool wasGrounded = true;
        private float airTime;
        private float stepDistance;
        private float lastY;

        private void Reset()
        {
            movement = GetComponent<PlayerMovement>();
        }

        private void Awake()
        {
            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }

            lastY = transform.position.y;
        }

        private void LateUpdate()
        {
            bool grounded = movement.IsGrounded;

            if (wasGrounded && !grounded)
            {
                OnLeftGround();
            }
            else if (!wasGrounded && grounded)
            {
                OnLanded();
            }

            if (grounded)
            {
                UpdateSteps();
            }
            else
            {
                airTime += Time.deltaTime;
            }

            wasGrounded = grounded;
            lastY = transform.position.y;
        }

        private void OnLeftGround()
        {
            airTime = 0f;

            if (transform.position.y > lastY)
            {
                jumpCue.Play(transform.position);
            }
        }

        private void OnLanded()
        {
            if (airTime >= minAirTime)
            {
                landCue.Play(transform.position);
            }

            airTime = 0f;
            stepDistance = 0f;
        }

        private void UpdateSteps()
        {
            if (movement.CurrentSpeed < minMoveSpeed)
            {
                return;
            }

            stepDistance += movement.CurrentSpeed * Time.deltaTime;

            float stride = movement.IsCrouching ? crouchStride : movement.IsRunning ? runStride : walkStride;
            if (stepDistance < stride)
            {
                return;
            }

            stepDistance -= stride;
            CurrentStepCue().Play(transform.position);
        }

        private AudioCue CurrentStepCue()
        {
            return movement.IsCrouching ? crouchCue : movement.IsRunning ? runCue : walkCue;
        }
    }
}
