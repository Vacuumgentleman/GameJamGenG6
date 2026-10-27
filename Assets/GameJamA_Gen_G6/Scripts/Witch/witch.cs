using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Bruja de la Era 3: puede ser agarrada y arrastrada por el jugador (ver PlayerWitchDragger)
    /// y quemarse al llegar a la hoguera (ver Bonfire).
    /// </summary>
    public class Witch : MonoBehaviour
    {
        public enum State { Free, Grabbed, Burning }

        // Brujas vivas en la escena y aviso global cuando una termina de quemarse.
        public static readonly List<Witch> Active = new List<Witch>();
        public static event Action<Witch> Burned;

        // Posicion respecto al jugador mientras se arrastra: x = derecha, z = hacia delante.
        // Usa un z negativo si quieres que quede detras del jugador.
        [SerializeField] private Vector3 dragOffset = new Vector3(0f, 0f, 1.2f);

        // Que tan rapido alcanza la posicion de arrastre. Mas alto = mas pegada al jugador.
        [SerializeField] private float followSharpness = 10f;

        // Segundos que dura la quema antes de desaparecer.
        [SerializeField] private float burnDuration = 2f;

        // Efecto de particulas que se reproduce al quemarse.
        [SerializeField] private ParticleSystem burnEffect;

        public State CurrentState { get; private set; } = State.Free;
        public PlayerWitchDragger Dragger { get; private set; }
        public bool CanBeGrabbed => CurrentState == State.Free;

        private Rigidbody body;
        private Collider[] colliders;
        private bool wasKinematic;
        private float grabHeight;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();
        }

        private void OnEnable() => Active.Add(this);
        private void OnDisable() => Active.Remove(this);

        // Lo llama PlayerWitchDragger al agarrarla.
        public void Grab(PlayerWitchDragger dragger)
        {
            CurrentState = State.Grabbed;
            Dragger = dragger;

            wasKinematic = body.isKinematic;
            body.isKinematic = true;
            grabHeight = body.position.y;
            SetIgnorePlayerCollision(true);
        }

        // Lo llama PlayerWitchDragger al soltarla.
        public void ReleaseGrab()
        {
            if (CurrentState != State.Grabbed) return;

            SetIgnorePlayerCollision(false);
            body.isKinematic = wasKinematic;
            CurrentState = State.Free;
            Dragger = null;

            Debug.Log("[Witch] Soltada", this);
        }

        private void FixedUpdate()
        {
            if (CurrentState != State.Grabbed || Dragger == null) return;

            Vector3 forward = Dragger.FlatForward;
            Quaternion facing = Quaternion.LookRotation(forward);

            // Punto de arrastre: respecto al jugador y segun hacia donde mira, a la altura original de la bruja.
            Vector3 target = Dragger.transform.position + facing * dragOffset;
            target.y = grabHeight;

            float t = Mathf.Clamp01(followSharpness * Time.fixedDeltaTime);
            body.MovePosition(Vector3.Lerp(body.position, target, t));
            body.MoveRotation(Quaternion.Slerp(body.rotation, facing, t));
        }

        // Lo llama Bonfire cuando la bruja entra agarrada a la hoguera.
        public void Burn(Vector3 bonfirePosition)
        {
            if (CurrentState == State.Burning) return;

            CurrentState = State.Burning;
            Dragger = null;

            Debug.Log($"[Witch] Burn() llamado; empieza la quema y desparecera en {burnDuration} $", this);

            body.isKinematic = true;
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }
            body.position = bonfirePosition;
            transform.position = bonfirePosition;

            if (burnEffect != null) burnEffect.Play();

            StartCoroutine(BurnRoutine());
        }

        private IEnumerator BurnRoutine()
        {
            yield return new WaitForSeconds(burnDuration);
            Burned?.Invoke(this);
            Destroy(gameObject);
        }

        // Evita que la bruja y el jugador choquen entre si mientras se arrastra.
        private void SetIgnorePlayerCollision(bool ignore)
        {
            if (Dragger == null || Dragger.OwnerCollider == null) return;

            foreach (Collider c in colliders)
            {
                Physics.IgnoreCollision(c, Dragger.OwnerCollider, ignore);
            }
        }
    }
}
