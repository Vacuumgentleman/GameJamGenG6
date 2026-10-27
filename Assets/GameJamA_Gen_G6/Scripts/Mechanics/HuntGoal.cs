using System;
using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Objetivo del nivel de caza: cuenta cuantos animales de la escena han muerto.
    /// Se completa cuando mueren todos. Otros sistemas pueden suscribirse a Changed y Completed.
    /// </summary>
    public class HuntGoal : MonoBehaviour
    {
        [Header("Sonidos")]
        [SerializeField] private AudioCue preyHuntedCue;
        [SerializeField] private AudioCue completedCue;

        private Animal[] animals = Array.Empty<Animal>();

        public int Total { get; private set; }
        public int Hunted { get; private set; }
        public bool IsCompleted => Total > 0 && Hunted >= Total;

        public event Action Changed;
        public event Action Completed;

        private void Start()
        {
            animals = FindObjectsByType<Animal>(FindObjectsSortMode.None);
            Total = animals.Length;

            foreach (Animal animal in animals)
            {
                animal.Died += HandleDied;
            }

            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (Animal animal in animals)
            {
                if (animal != null)
                {
                    animal.Died -= HandleDied;
                }
            }
        }

        private void HandleDied(Animal animal)
        {
            Hunted++;
            Changed?.Invoke();

            Vector3 listener = Camera.main != null ? Camera.main.transform.position : animal.transform.position;
            if (IsCompleted)
            {
                completedCue.Play(listener);
                Completed?.Invoke();
            }
            else
            {
                preyHuntedCue.Play(listener);
            }
        }
    }
}
