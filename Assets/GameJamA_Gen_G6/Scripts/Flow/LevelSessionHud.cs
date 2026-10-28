using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(LevelSession))]
    public class LevelSessionHud : MonoBehaviour
    {
        [SerializeField] private float lowTimeThreshold = 30f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowColor = new Color(1f, 0.4f, 0.35f, 1f);
        [SerializeField] private Color messageColor = new Color(1f, 0.75f, 0.35f, 1f);

        private LevelSession session;
        private bool hasOwnTimerHud;
        private GUIStyle timerStyle;
        private GUIStyle messageStyle;

        private void Awake()
        {
            session = GetComponent<LevelSession>();
        }

        private void Start()
        {
            hasOwnTimerHud = FindFirstObjectByType<PlantingHud>() != null;
        }

        private void OnGUI()
        {
            CreateStyles();
            Color previous = GUI.contentColor;

            if (!hasOwnTimerHud && session.Timer != null)
            {
                DrawTimer();
            }

            DrawMessage();
            GUI.contentColor = previous;
        }

        private void DrawTimer()
        {
            float remaining = Mathf.Max(0f, session.Timer.Remaining);
            int total = Mathf.CeilToInt(remaining);
            GUI.contentColor = remaining <= lowTimeThreshold ? lowColor : normalColor;
            GUI.Label(new Rect(0f, 16f, Screen.width, 40f), $"{total / 60}:{total % 60:00}", timerStyle);
        }

        private void DrawMessage()
        {
            string message = session.State switch
            {
                LevelSession.SessionState.Completed => "Regresando a la linea temporal...",
                LevelSession.SessionState.Expired => "Tiempo agotado: la realidad se distorsiona. Regresando...",
                _ => null
            };

            if (message == null)
            {
                return;
            }

            GUI.contentColor = messageColor;
            GUI.Label(new Rect(0f, Screen.height - 110f, Screen.width, 40f), message, messageStyle);
        }

        private void CreateStyles()
        {
            if (timerStyle != null)
            {
                return;
            }

            timerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };
            messageStyle = new GUIStyle(timerStyle)
            {
                fontSize = 26
            };
        }
    }
}
