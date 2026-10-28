using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam
{
    public class PlacementGoal : MonoBehaviour
    {
        [SerializeField] private AudioCue completedCue;
        [SerializeField] private UnityEvent completed;

        private PlacementSlot[] slots = Array.Empty<PlacementSlot>();

        public int Total { get; private set; }
        public int Placed { get; private set; }
        public bool IsCompleted => Total > 0 && Placed >= Total;

        public event Action Changed;
        public event Action Completed;

        private void Start()
        {
            slots = FindObjectsByType<PlacementSlot>(FindObjectsSortMode.None);
            Total = slots.Length;

            foreach (PlacementSlot slot in slots)
            {
                slot.Placed += HandlePlaced;
            }

            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (PlacementSlot slot in slots)
            {
                if (slot != null)
                {
                    slot.Placed -= HandlePlaced;
                }
            }
        }

        private void HandlePlaced(PlacementSlot slot)
        {
            Placed++;
            Changed?.Invoke();

            if (!IsCompleted)
            {
                return;
            }

            Vector3 listener = Camera.main != null ? Camera.main.transform.position : slot.transform.position;
            completedCue.Play(listener);
            completed?.Invoke();
            Completed?.Invoke();
        }
    }
}
