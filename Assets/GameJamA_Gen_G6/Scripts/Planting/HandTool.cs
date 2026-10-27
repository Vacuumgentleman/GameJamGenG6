using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Base de las herramientas de mano (azada, anfora): un clic se puede dar siempre, apunte o no a algo.
    /// Lanza un rayo al frente y las clases hijas deciden el efecto en Apply; si lo hay, se reproduce
    /// el movimiento de la herramienta.
    /// </summary>
    public abstract class HandTool : HeldItem
    {
        [Header("Uso")]
        [SerializeField] private float reach = 3.5f;
        [SerializeField] private float cooldown = 0.5f;

        [Header("En la mano")]
        [SerializeField] private Vector3 handPosition = new Vector3(0.3f, -0.25f, 0.5f);
        [SerializeField] private Vector3 handEuler = Vector3.zero;

        [Header("Movimiento al usar")]
        [SerializeField] private Vector3 swingOffset = new Vector3(0f, -0.08f, 0.25f);
        [SerializeField] private Vector3 swingEuler = new Vector3(45f, 0f, 0f);
        [SerializeField] private float swingDuration = 0.25f;

        [Header("Sonidos")]
        [SerializeField] private AudioCue pickupCue;
        [SerializeField] private AudioCue equipCue;
        [SerializeField] private AudioCue useCue;

        private float nextUseTime;
        private float swingTimer;

        // hasHit indica si el rayo golpeo algo. Devuelve true si el uso se realiza (movimiento y sonido).
        protected abstract bool Apply(bool hasHit, in RaycastHit hit);

        public override void OnPickedUp()
        {
            base.OnPickedUp();
            pickupCue.Play(transform.position);
        }

        public override void OnEquipped(Transform holdPoint)
        {
            swingTimer = 0f;
            transform.SetLocalPositionAndRotation(handPosition, Quaternion.Euler(handEuler));
            equipCue.Play(transform.position);
        }

        public override void OnUnequipped()
        {
            swingTimer = 0f;
        }

        public override UseResult Use(in UseContext context)
        {
            if (Time.time < nextUseTime)
            {
                return UseResult.Kept;
            }

            nextUseTime = Time.time + cooldown;

            bool hasHit = Physics.Raycast(context.Aim, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore);
            if (Apply(hasHit, hit))
            {
                useCue.Play(hasHit ? hit.point : context.Aim.origin + context.Aim.direction * reach);
                swingTimer = swingDuration;
            }

            return UseResult.Kept;
        }

        private void Update()
        {
            if (swingTimer <= 0f)
            {
                return;
            }

            swingTimer = Mathf.Max(0f, swingTimer - Time.deltaTime);
            float k = Mathf.Sin((1f - swingTimer / Mathf.Max(0.01f, swingDuration)) * Mathf.PI);
            transform.SetLocalPositionAndRotation(
                handPosition + swingOffset * k,
                Quaternion.Euler(handEuler + swingEuler * k));
        }
    }
}
