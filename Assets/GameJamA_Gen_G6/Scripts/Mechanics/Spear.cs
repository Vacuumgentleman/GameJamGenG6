using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Lanza de caza. Se recoge con E y se equipa como cualquier objeto.
    /// Mantener el clic izquierdo acumula fuerza y muestra la trayectoria; al soltar se lanza.
    /// Al chocar con algo que implemente IDamageable le hace dano; si no, se clava en la superficie.
    /// </summary>
    public class Spear : ChargedThrowable
    {
        [Header("Clavado")]
        // Distancia del centro de la lanza hasta la punta.
        [SerializeField] private float tipLength = 0.95f;

        // Cuanto de la punta queda enterrada en la superficie.
        [SerializeField] private float stickDepth = 0.35f;

        [Header("Dano")]
        [SerializeField] private float damage = 50f;

        [Header("Sonidos de impacto")]
        [SerializeField] private AudioCue hitCreatureCue;
        [SerializeField] private AudioCue hitSurfaceCue;

        protected override void OnImpact(Collision collision, Vector3 point)
        {
            IDamageable target = collision.collider.GetComponentInParent<IDamageable>();

            if (target == null || target.IsDead)
            {
                hitSurfaceCue.Play(point);
                StickIn(collision, point, target as Component);
                return;
            }

            hitCreatureCue.Play(point);
            target.TakeDamage(new DamageInfo(damage, point, LastVelocity.normalized, Thrower));
            Body.linearVelocity *= 0.1f;
        }

        // Clava la punta en la superficie: queda quieta y se puede recoger con E.
        private void StickIn(Collision collision, Vector3 point, Component carcass)
        {
            Vector3 forward = LastVelocity.sqrMagnitude > 0.01f ? LastVelocity.normalized : transform.forward;
            Vector3 position = point - forward * (tipLength - stickDepth);

            Body.linearVelocity = Vector3.zero;
            Body.angularVelocity = Vector3.zero;
            Body.isKinematic = true;
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(forward));

            // Si lo golpeado se mueve (un cuerpo con Rigidbody o un animal caido), la lanza lo acompana.
            if (carcass != null)
            {
                transform.SetParent(carcass.transform, true);
            }
            else if (collision.rigidbody != null)
            {
                transform.SetParent(collision.transform, true);
            }
        }
    }
}
