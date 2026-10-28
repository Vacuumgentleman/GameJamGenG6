using System;
using System.Collections;
using UnityEngine;

namespace GameJam
{
    public class BurnZone : MonoBehaviour, IHighlightable
    {
        [SerializeField] private string displayName = "Caldero";
        [SerializeField] private string hint = "Arrastra un fantasma hasta aqui para quemarlo";

        [Header("Quema")]
        [SerializeField] private float burnTime = 3f;
        [SerializeField] private Vector3 burnPointOffset = new Vector3(0f, 0.45f, 0f);
        [SerializeField] private float shakeAmount = 0.05f;
        [SerializeField, Range(0f, 1f)] private float endScale = 0.2f;

        [Header("Particulas")]
        [SerializeField] private Color flameColor = new Color(1f, 0.55f, 0.15f, 1f);
        [SerializeField] private Color smokeColor = new Color(0.35f, 0.35f, 0.4f, 1f);
        [SerializeField] private float emberInterval = 0.25f;

        [Header("Sonidos")]
        [SerializeField] private AudioCue enterCue;
        [SerializeField] private AudioCue burnedCue;

        public int BurningCount { get; private set; }
        public Vector3 BurnPoint => transform.TransformPoint(burnPointOffset);

        public bool CanHighlight => true;
        public string HighlightText => $"{displayName}\n{hint}";

        public event Action<Draggable> Burned;

        private void OnTriggerEnter(Collider other)
        {
            Draggable draggable = other.GetComponentInParent<Draggable>();
            if (draggable == null || draggable.CurrentState == Draggable.State.Locked)
            {
                return;
            }

            StartCoroutine(BurnRoutine(draggable));
        }

        private IEnumerator BurnRoutine(Draggable draggable)
        {
            Vector3 point = BurnPoint;
            draggable.Lock(point);
            BurningCount++;
            enterCue.Play(point);

            Transform target = draggable.transform;
            Vector3 baseScale = target.localScale;
            float elapsed = 0f;
            float nextEmber = 0f;

            while (elapsed < burnTime && draggable != null)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / burnTime);

                target.position = point + UnityEngine.Random.insideUnitSphere * (shakeAmount * progress);
                target.localScale = baseScale * Mathf.Lerp(1f, endScale, progress);

                if (elapsed >= nextEmber)
                {
                    ParticleBurst.Play(point, flameColor, 6, 1.5f, 0.08f, 0.1f, 0.6f);
                    nextEmber = elapsed + emberInterval;
                }

                yield return null;
            }

            BurningCount--;
            if (draggable == null)
            {
                yield break;
            }

            ParticleBurst.Play(point, flameColor, 40, 3.5f, 0.14f, 0.3f, 0.9f, 60f);
            ParticleBurst.Play(point, smokeColor, 20, 1.8f, 0.2f, -0.1f, 1.4f, 40f);
            burnedCue.Play(point);

            Burned?.Invoke(draggable);
            draggable.Consume();
        }
    }
}
