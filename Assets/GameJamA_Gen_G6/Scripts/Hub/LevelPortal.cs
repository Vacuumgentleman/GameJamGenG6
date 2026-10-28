using UnityEngine;

namespace GameJam
{
    public class LevelPortal : MonoBehaviour, IHighlightable, IInteractable, IFocusAware
    {
        [SerializeField] private string levelId;
        [SerializeField] private Renderer indicator;
        [SerializeField] private Material pendingMaterial;
        [SerializeField] private Material completedMaterial;
        [SerializeField] private Light glow;
        [SerializeField] private Color pendingColor = new Color(1f, 0.25f, 0.2f, 1f);
        [SerializeField] private Color completedColor = new Color(0.3f, 1f, 0.45f, 1f);
        private const float LabelHeight = 2.6f;
        private const float LabelSize = 0.04f;

        private LevelDefinition level;
        private Transform label;

        public bool IsCompleted => GameProgress.IsCompleted(levelId);
        public bool CanHighlight => level != null;

        public string HighlightText => IsCompleted
            ? $"{level.DisplayName}\nLinea temporal reparada"
            : $"{level.DisplayName}\nE: viajar a esta linea temporal";

        private void Awake()
        {
            LevelCatalog catalog = LevelCatalog.Load();
            if (catalog == null || !catalog.TryGetById(levelId, out level))
            {
                Debug.LogError($"LevelPortal: no existe el nivel '{levelId}' en LevelCatalog.", this);
                return;
            }

            label = CreateLabel(level.DisplayName);
        }

        private void Start()
        {
            Refresh();
        }

        private void LateUpdate()
        {
            Camera view = Camera.main;
            if (label != null && view != null)
            {
                label.rotation = view.transform.rotation;
            }
        }

        public void SetFocused(bool focused)
        {
            if (label != null)
            {
                label.gameObject.SetActive(!focused);
            }
        }

        public void Interact()
        {
            if (level == null || IsCompleted)
            {
                return;
            }

            GameFlow.EnterLevel(level);
        }

        private void Refresh()
        {
            bool completed = IsCompleted;
            if (indicator != null)
            {
                indicator.sharedMaterial = completed ? completedMaterial : pendingMaterial;
            }

            if (glow != null)
            {
                glow.color = completed ? completedColor : pendingColor;
            }
        }

        private Transform CreateLabel(string text)
        {
            GameObject holder = new GameObject("Label");
            holder.transform.SetParent(transform, false);
            holder.transform.localPosition = Vector3.up * LabelHeight;

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            TextMesh mesh = holder.AddComponent<TextMesh>();
            mesh.font = font;
            mesh.text = text;
            mesh.fontSize = 64;
            mesh.characterSize = LabelSize;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            holder.GetComponent<MeshRenderer>().sharedMaterial = font.material;
            return holder.transform;
        }
    }
}
