using UnityEngine;

namespace GameJam
{
    public class HubHud : MonoBehaviour
    {
        [SerializeField] private Color accent = new Color(1f, 0.75f, 0.35f, 1f);
        [SerializeField] private Color completedColor = new Color(0.45f, 1f, 0.5f, 1f);

        private LevelCatalog catalog;
        private GUIStyle rightStyle;
        private GUIStyle messageStyle;

        private void Awake()
        {
            catalog = LevelCatalog.Load();
        }

        private void OnGUI()
        {
            if (catalog == null)
            {
                return;
            }

            CreateStyles();
            bool repaired = catalog.Count > 0 && GameProgress.CompletedCount >= catalog.Count;

            Color previous = GUI.contentColor;
            GUI.contentColor = repaired ? completedColor : accent;
            GUI.Label(new Rect(Screen.width - 420f, 16f, 404f, 36f), $"Lineas reparadas: {GameProgress.CompletedCount} / {catalog.Count}", rightStyle);

            if (repaired)
            {
                GUI.Label(new Rect(0f, Screen.height * 0.25f, Screen.width, 60f), "Linea temporal reparada", messageStyle);
            }

            GUI.contentColor = previous;
        }

        private void CreateStyles()
        {
            if (rightStyle != null)
            {
                return;
            }

            rightStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperRight
            };
            messageStyle = new GUIStyle(rightStyle)
            {
                fontSize = 34,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
