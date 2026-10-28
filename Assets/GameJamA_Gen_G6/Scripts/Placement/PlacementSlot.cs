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

        [Header("Silueta")]
        [SerializeField] private Material silhouetteMaterial;

        private RendererTint tinter;
        private GameObject silhouette;

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

        private void Start()
        {
            BuildSilhouette();
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
            if (silhouette != null)
            {
                silhouette.SetActive(false);
            }

            item.Place(snapPoint);
            tinter.Apply(item.gameObject);
            placedCue.Play(snapPoint.position);
            Placed?.Invoke(this);
            return true;
        }

        private void BuildSilhouette()
        {
            if (silhouetteMaterial == null)
            {
                return;
            }

            PlaceableItem source = FindSource();
            if (source == null)
            {
                return;
            }

            silhouette = new GameObject($"Silhouette_{requiredItemId}");
            Transform root = silhouette.transform;
            root.SetParent(snapPoint, false);
            root.localScale = source.transform.localScale;

            foreach (MeshFilter filter in source.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh != null && filter.TryGetComponent(out MeshRenderer _))
                {
                    CreatePart(root, source.transform, filter);
                }
            }
        }

        private PlaceableItem FindSource()
        {
            foreach (PlaceableItem item in FindObjectsByType<PlaceableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (item.ItemId == requiredItemId)
                {
                    return item;
                }
            }

            return null;
        }

        private void CreatePart(Transform root, Transform sourceRoot, MeshFilter filter)
        {
            Transform part = new GameObject(filter.name).transform;
            part.SetParent(root, false);

            Vector3 scale = Vector3.one;
            for (Transform current = filter.transform; current != sourceRoot; current = current.parent)
            {
                scale = Vector3.Scale(scale, current.localScale);
            }

            part.SetLocalPositionAndRotation(
                sourceRoot.InverseTransformPoint(filter.transform.position),
                Quaternion.Inverse(sourceRoot.rotation) * filter.transform.rotation);
            part.localScale = scale;

            part.gameObject.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            MeshRenderer renderer = part.gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = silhouetteMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
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
