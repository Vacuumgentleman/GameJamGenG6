using System;
using UnityEngine;

namespace GameJam
{
    public class BurnGoal : MonoBehaviour, ILevelGoal
    {
        [SerializeField] private GameObject[] enableOnCompleted;
        [SerializeField] private AudioCue burnedCue;
        [SerializeField] private AudioCue completedCue;

        private Draggable[] draggables = Array.Empty<Draggable>();

        public int Total { get; private set; }
        public int Burned { get; private set; }
        public int Remaining => Total - Burned;
        public bool IsCompleted => Total > 0 && Burned >= Total;

        public event Action Changed;
        public event Action Completed;

        private void Awake()
        {
            SetRewardsActive(false);
        }

        private void Start()
        {
            draggables = FindObjectsByType<Draggable>(FindObjectsSortMode.None);
            Total = draggables.Length;

            foreach (Draggable draggable in draggables)
            {
                draggable.Consumed += HandleConsumed;
            }

            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (Draggable draggable in draggables)
            {
                if (draggable != null)
                {
                    draggable.Consumed -= HandleConsumed;
                }
            }
        }

        private void HandleConsumed(Draggable draggable)
        {
            Burned++;
            Changed?.Invoke();

            Vector3 listener = Camera.main != null ? Camera.main.transform.position : draggable.transform.position;
            if (!IsCompleted)
            {
                burnedCue.Play(listener);
                return;
            }

            SetRewardsActive(true);
            completedCue.Play(listener);
            Completed?.Invoke();
        }

        private void SetRewardsActive(bool active)
        {
            foreach (GameObject reward in enableOnCompleted)
            {
                if (reward == null)
                {
                    continue;
                }

                reward.SetActive(active);
                if (!active)
                {
                    continue;
                }

                foreach (ParticleSystem system in reward.GetComponentsInChildren<ParticleSystem>(true))
                {
                    system.Play();
                }
            }
        }
    }
}
