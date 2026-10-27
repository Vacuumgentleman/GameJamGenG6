using UnityEngine;

namespace GameJam
{
    /// <summary>
    /// Ilumina lo que el jugador mira (parcelas, pozo) cuando esta al alcance y se puede interactuar con ello,
    /// y muestra su texto explicativo bajo la mira. Funciona con cualquier componente que implemente IHighlightable.
    /// Va una sola vez por escena.
    /// </summary>
    public class AimHighlighter : MonoBehaviour
    {
        [SerializeField] private float reach = 3.5f;
        [SerializeField] private Color tint = new Color(1.6f, 1.6f, 1.9f, 1f);
        [SerializeField] private LayerMask mask = ~0;

        [Header("Texto")]
        [SerializeField] private bool showText = true;
        [SerializeField] private Color textColor = new Color(1f, 0.95f, 0.7f, 1f);

        private PlayerAim aim;
        private RendererTint tinter;
        private bool active;
        private string text = string.Empty;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Awake()
        {
            tinter = new RendererTint(tint);
        }

        private void Start()
        {
            aim = FindFirstObjectByType<PlayerAim>();
        }

        private void OnDisable()
        {
            tinter.Clear();
            active = false;
            text = string.Empty;
        }

        private void LateUpdate()
        {
            IHighlightable target = FindTarget();
            GameObject targetObject = (target as Component)?.gameObject;

            if (targetObject != null)
            {
                tinter.Apply(targetObject);
                text = target.HighlightText;
                active = true;
            }
            else if (active)
            {
                tinter.Clear();
                text = string.Empty;
                active = false;
            }
        }

        private void OnGUI()
        {
            if (!showText || string.IsNullOrEmpty(text))
            {
                return;
            }

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, fontStyle = FontStyle.Bold };
                bodyStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };
            }

            titleStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.028f);
            bodyStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.022f);
            titleStyle.normal.textColor = textColor;
            bodyStyle.normal.textColor = Color.white;

            string[] lines = text.Split('\n');
            float y = Screen.height * 0.5f + Screen.height * 0.09f;
            float width = Screen.width;

            for (int i = 0; i < lines.Length; i++)
            {
                GUIStyle style = i == 0 ? titleStyle : bodyStyle;
                float height = style.fontSize * 1.4f;
                GUI.Label(new Rect(0f, y, width, height), lines[i], style);
                y += height;
            }
        }

        private IHighlightable FindTarget()
        {
            if (aim == null || !aim.Raycast(reach, mask, out RaycastHit hit))
            {
                return null;
            }

            IHighlightable highlightable = hit.collider.GetComponentInParent<IHighlightable>();
            return highlightable != null && highlightable.CanHighlight ? highlightable : null;
        }
    }
}
