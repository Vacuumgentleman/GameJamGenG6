using System;
using UnityEngine;

namespace GameJam
{
    public class PlacementSlot : MonoBehaviour, IHighlightable
    {
        [SerializeField] private string requiredItemId;
        [SerializeField] private string displayName;
        [SerializeField] private Transform snapPoint;
        [SerializeField] private float dropRadius = 0.8f;
        [SerializeField] private Color placedTint = new Color(0.6f, 1.4f, 0.6f, 1f);
        [SerializeField] private AudioCue placedCue;

        private RendererTint tinter;

        public PlaceableItem Item { get; private set; }
        public bool IsFilled => Item != null;
        public string RequiredItemId => requiredItemId;

        public bool CanHighlight => !IsFilled;
        public string HighlightText => $"Espacio: {displayName}\nE: colocar  |  G: soltar cerca";

        public event Action<PlacementSlot> Placed;

        private void Awake()
        {
            tinter = new RendererTint(placedTint);
            if (snapPoint == null)
            {
                snapPoint = transform;
            }
        }

        public bool Accepts(PlaceableItem item)
        {
            return item != null && !IsFilled && item.ItemId == requiredItemId;
        }

        public bool TryPlace(PlaceableItem item)
        {
            if (!Accepts(item))
            {
                return false;
            }

            Item = item;
            item.Place(snapPoint);
            tinter.Apply(item.gameObject);
            placedCue.Play(snapPoint.position);
            Placed?.Invoke(this);
            return true;
        }

        public static bool TryPlaceNear(PlaceableItem item, Vector3 position)
        {
            PlacementSlot best = null;
            float bestDistance = float.MaxValue;

            foreach (PlacementSlot slot in FindObjectsByType<PlacementSlot>(FindObjectsSortMode.None))
            {
                float distance = Vector3.Distance(slot.snapPoint.position, position);
                if (distance <= slot.dropRadius && distance < bestDistance && slot.Accepts(item))
                {
                    best = slot;
                    bestDistance = distance;
                }
            }

            return best != null && best.TryPlace(item);
        }
    }
}
