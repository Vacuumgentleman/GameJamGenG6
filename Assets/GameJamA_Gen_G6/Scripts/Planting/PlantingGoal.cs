using System;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Objetivo del nivel de plantacion: se completa cuando todas las parcelas de la escena han crecido.
    /// Al completarse detiene el LevelTimer (si existe) y calcula una calificacion segun el tiempo restante.
    /// </summary>
    public class PlantingGoal : MonoBehaviour, ILevelGoal
    {
        [Header("Calificacion por tiempo restante (segundos)")]
        [SerializeField] private float twoStarsRemaining = 45f;
        [SerializeField] private float threeStarsRemaining = 90f;

        [Header("Sonidos")]
        [SerializeField] private AudioCue plotGrownCue;
        [SerializeField] private AudioCue completedCue;

        private PlantingPlot[] plots = Array.Empty<PlantingPlot>();
        private LevelTimer timer;

        public int Total { get; private set; }
        public int Grown { get; private set; }
        public int Stars { get; private set; }
        public bool IsCompleted => Total > 0 && Grown >= Total;

        public event Action Changed;
        public event Action Completed;

        private void Start()
        {
            timer = FindFirstObjectByType<LevelTimer>();
            plots = FindObjectsByType<PlantingPlot>(FindObjectsSortMode.None);
            Total = plots.Length;

            foreach (PlantingPlot plot in plots)
            {
                plot.BecameGrown += HandleGrown;
            }

            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (PlantingPlot plot in plots)
            {
                if (plot != null)
                {
                    plot.BecameGrown -= HandleGrown;
                }
            }
        }

        private void HandleGrown(PlantingPlot plot)
        {
            if (timer != null && timer.IsExpired)
            {
                return;
            }

            Grown++;
            Changed?.Invoke();

            Vector3 listener = Camera.main != null ? Camera.main.transform.position : plot.transform.position;
            if (!IsCompleted)
            {
                plotGrownCue.Play(listener);
                return;
            }

            float remaining = timer != null ? timer.Remaining : 0f;
            Stars = remaining >= threeStarsRemaining ? 3 : remaining >= twoStarsRemaining ? 2 : 1;

            if (timer != null)
            {
                timer.Stop();
            }

            completedCue.Play(listener);
            Completed?.Invoke();
        }
    }
}
