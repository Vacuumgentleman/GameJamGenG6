using UnityEngine;

namespace GameJam
{
    public class ControlsHud : MonoBehaviour
    {
        private static readonly (string key, string action)[] Bindings =
        {
            ("E", "Interactuar / recoger"),
            ("C", "Agacharse"),
            ("Shift", "Correr"),
            ("Espacio", "Saltar"),
            ("Click", "Usar / lanzar")
        };

        [SerializeField] private float introDuration = 5f;
        [SerializeField] private Vector2 margin = new Vector2(16f, 16f);
        [SerializeField] private float rowHeight = 22f;
        [SerializeField] private float keyWidth = 64f;
        [SerializeField] private float actionWidth = 170f;
        [SerializeField] private Color keyColor = new Color(1f, 0.85f, 0.4f, 1f);
        [SerializeField] private Color actionColor = new Color(1f, 1f, 1f, 0.85f);
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.45f);

        private float introEndTime;
        private GUIStyle keyStyle;
        private GUIStyle actionStyle;
        private Texture2D panelTexture;

        private bool Visible => GamePause.IsPaused || Time.realtimeSinceStartup < introEndTime;

        private void Awake()
        {
            introEndTime = Time.realtimeSinceStartup + introDuration;
        }

        private void OnDestroy()
        {
            if (panelTexture != null)
            {
                Destroy(panelTexture);
            }
        }

        private void OnGUI()
        {
            if (!Visible)
            {
                return;
            }

            CreateStyles();

            float width = keyWidth + actionWidth + 24f;
            float height = Bindings.Length * rowHeight + 16f;
            Rect panel = new Rect(margin.x, Screen.height - margin.y - height, width, height);
            GUI.DrawTexture(panel, panelTexture);

            for (int i = 0; i < Bindings.Length; i++)
            {
                float y = panel.y + 8f + i * rowHeight;
                GUI.Label(new Rect(panel.x + 8f, y, keyWidth, rowHeight), Bindings[i].key, keyStyle);
                GUI.Label(new Rect(panel.x + 16f + keyWidth, y, actionWidth, rowHeight), Bindings[i].action, actionStyle);
            }
        }

        private void CreateStyles()
        {
            if (keyStyle != null)
            {
                return;
            }

            panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, panelColor);
            panelTexture.Apply();

            keyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            keyStyle.normal.textColor = keyColor;

            actionStyle = new GUIStyle(keyStyle)
            {
                fontStyle = FontStyle.Normal
            };
            actionStyle.normal.textColor = actionColor;
        }
    }
}
